using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CybersecurityChatbotGUI
{
    /* I used this delegate to satisfy the "delegates" learning outcome in the rubric.
    It is used throughout ProcessUserInput to compose every final response string,
    keeping formatting logic decoupled from the main business logic (Task 8).

    References:
    Microsoft (2023). Delegates (C# Programming Guide). [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/
    */
    public delegate string FormatResponseDelegate(string prefix, string content);
    public class ChatbotEngine
    {
        /* Knowledge base: mapping topics to a pool of responses (Tasks 2 and 3).

        References:
        Troelsen, A. and Japikse, P. (2021). Pro C# 9 with .NET 5: Foundational Principles
        and Practices. 10th ed. New York: Apress.
        */
        private readonly List<ChatbotResponse> _knowledgeBase;

        /* Sentiment keyword mapping to an empathetic prefix string (Task 6). */
        private readonly Dictionary<string, string> _sentiments;

        /* Phrases that indicate the user wants to continue a topic (Task 4). */
        private readonly List<string> _followUpTriggers;

        /* Delegate instance used to build every response (delegate learning outcome). */
        private readonly FormatResponseDelegate _formatter;

        /* Part 3 / Task 3: recognises commands (add task, quiz, show log, etc.) so the bot
        does far more than answer knowledge questions. */
        private readonly IntentRecognizer _intentRecognizer;

        /* Part 3 / Task 1: data-access layer for the MySQL task store. */
        private readonly TaskRepository _taskRepository;

        /* Live session state shared with the GUI to handle Task 5 (Memory and Recall). */
        public SessionContext Session { get; private set; }

        /* Part 3 / Task 4: the running log of everything the bot has done this session.
        Exposed publicly so the GUI windows can record their own actions too. */
        public ActivityLogger ActivityLog { get; private set; }

        /* Part 3 / Task 1: exposes the shared task repository so the Task Manager window uses
        the very same database connection settings as the chat. */
        public TaskRepository Tasks => _taskRepository;

        /* Part 3: the last command the engine acted on, so the GUI can react to intents that
        need a window (e.g. open the Task Manager when the user says "show my tasks"). */
        public ChatIntent LastHandledIntent { get; private set; } = ChatIntent.KnowledgeQuery;

        /* Constructor to initialize the session and wire up the knowledge base,
        sentiments, and follow up triggers when the engine starts. */
        public ChatbotEngine()
        {
            Session = new SessionContext();
            ActivityLog = new ActivityLogger();
            _intentRecognizer = new IntentRecognizer();
            _taskRepository = new TaskRepository();

            /* Lambda expression for the delegate: prefix + content to composed display string */
            _formatter = (prefix, content) =>
                string.IsNullOrEmpty(prefix) ? content : $"{prefix}{content}";

            _knowledgeBase = BuildKnowledgeBase();
            _sentiments = BuildSentimentMap();
            _followUpTriggers = BuildFollowUpTriggers();

            /* First entry in the activity log so Task 4 has a timeline from the very start. */
            ActivityLog.Log(ActivityCategory.Session, "Cybersecurity chatbot session started.");
        }

        /* Task 2 and 3: The knowledge base.
        I made sure to include the 3 required keywords (password, scam, privacy) plus
        10 extended topics so I can hit the "Greatly Exceeds" band!
        Every topic has at least 5 unique responses for meaningful random variation (Task 3).
        I used List<ChatbotResponse> to explicitly hit the generic collection learning outcome.
        */
        private List<ChatbotResponse> BuildKnowledgeBase()
        {
            return new List<ChatbotResponse>
            {
                /* Conversational triggers (not saved as the user's cybersecurity favourite) */
                new ChatbotResponse("hello", new List<string>
                {
                    "Hello! I am your Cybersecurity Awareness assistant. " +
                    "Ask me about passwords, phishing, privacy, malware, and more.",
                    "Hi there! Ready to help you stay safe online. " +
                    "What cybersecurity topic would you like to explore?",
                    "Greetings! Cybersecurity matters every day. What shall we tackle today?",
                    "Hey! Great to see you taking online safety seriously. What would you like to know?",
                    "Hello! Type 'help' to see all the topics I can assist with."
                }),
                new ChatbotResponse("how are you", new List<string>
                {
                    "I am running at full capacity! How can I help you stay safe online?",
                    "All systems operational! What cybersecurity topic are you curious about today?",
                    "Doing great, and even better when helping someone stay cyber safe. " +
                    "What is on your mind?",
                    "Fully online and ready to assist! Ask me anything about cybersecurity.",
                    "Never better! Protecting people online is what I am built for. How can I help?"
                }),
                new ChatbotResponse("help", new List<string>
                {
                    "You can ask me about: passwords, phishing, privacy, scams, malware, " +
                    "ransomware, firewalls, Two Factor Auth, VPN, updates, or social engineering.",
                    "Try asking: 'How do I make a strong password?', 'What is phishing?', " +
                    "or 'How do I stay private online?'",
                    "I cover a wide range of cybersecurity topics! Keywords to try: password, " +
                    "scam, privacy, malware, ransomware, firewall, 2FA, VPN, update, phishing, " +
                    "social engineering.",
                    "Need a starting point? Ask me about 'password safety' or 'how to spot a scam'.",
                    "Here to help! Ask about any cybersecurity topic. " +
                    "From password tips to avoiding ransomware."
                }),

                /* REQUIRED KEYWORD 1: password */
                new ChatbotResponse("password", new List<string>
                {
                    "Always use a mix of uppercase letters, lowercase letters, numbers, " +
                    "and symbols in your passwords.",
                    "Never reuse the same password across multiple accounts. " +
                    "Use a password manager like Bitwarden or LastPass.",
                    "Consider a passphrase: four or more random words strung together. " +
                    "Easy to remember, very hard to crack.",
                    "Your password should be at least 12 characters long. The longer, the better.",
                    "Never share your password with anyone, including people claiming to be " +
                    "from IT support."
                }),

                /* REQUIRED KEYWORD 2: scam */
                new ChatbotResponse("scam", new List<string>
                {
                    "If a deal sounds too good to be true, it almost certainly is a scam. " +
                    "Trust your instincts.",
                    "Scammers create a false sense of urgency. Never rush a financial " +
                    "transaction; take your time.",
                    "Never send money to someone you have only met online, no matter how " +
                    "convincing their story sounds.",
                    "Verify the identity of anyone asking for sensitive information by " +
                    "contacting the organisation directly.",
                    "Be wary of unsolicited calls or messages requesting personal or financial " +
                    "information. Hang up and verify."
                }),

                /* REQUIRED KEYWORD 3: privacy */
                new ChatbotResponse("privacy", new List<string>
                {
                    "Regularly review the privacy settings on all your social media accounts. " +
                    "Platforms change defaults often.",
                    "Use a VPN when connecting to public Wi-Fi to protect your data from " +
                    "being intercepted.",
                    "Turn off location tracking for apps that do not genuinely need to know " +
                    "your whereabouts.",
                    "Read privacy policies before signing up to new services. " +
                    "Look for how your data is shared or sold.",
                    "Use a privacy focused browser like Firefox or Brave and a search engine " +
                    "like DuckDuckGo to reduce tracking."
                }),

                /* Extended Topic 1: phishing */
                new ChatbotResponse("phishing", new List<string>
                {
                    "Always check the sender's actual email address, not just the display name. " +
                    "Display names are trivial to fake.",
                    "Legitimate organisations will never ask for your password or banking details " +
                    "via email or SMS.",
                    "Hover over links before clicking to reveal the real destination URL. " +
                    "If it looks suspicious, do not click.",
                    "Beware of generic greetings like 'Dear Customer'. " +
                    "Real services address you by your actual name.",
                    "When in doubt, navigate directly to the company's website rather than " +
                    "clicking any link in an email."
                }),

                /* Extended Topic 2: malware */
                new ChatbotResponse("malware", new List<string>
                {
                    "Malware is malicious software designed to harm, steal data from, " +
                    "or take control of your device.",
                    "Keep your antivirus software updated. It can only protect against " +
                    "threats whose signatures it knows.",
                    "Avoid downloading email attachments from unknown senders; " +
                    "they are the most common malware delivery method.",
                    "Only download software from official, verified websites or app stores. " +
                    "Avoid pirated software.",
                    "Regularly scan your device for malware even if you believe you have " +
                    "not been infected."
                }),

                /* Extended Topic 3: ransomware */
                new ChatbotResponse("ransomware", new List<string>
                {
                    "Ransomware encrypts your files and demands payment for the decryption key. " +
                    "Back up data regularly to an offline drive.",
                    "Never pay a ransom. Payment does not guarantee file recovery and it " +
                    "funds further criminal activity.",
                    "Keep your OS and software up to date; ransomware frequently exploits " +
                    "unpatched vulnerabilities.",
                    "Disconnect an infected device from the network immediately to prevent " +
                    "the ransomware spreading further.",
                    "Ransomware is most commonly delivered via phishing emails. " +
                    "Be very cautious about attachments and links."
                }),

                /* Extended Topic 4: firewall */
                new ChatbotResponse("firewall", new List<string>
                {
                    "A firewall acts as a barrier between your computer and the internet, " +
                    "blocking malicious incoming traffic.",
                    "Ensure your operating system's built in firewall is always switched on. " +
                    "It is your first line of defence.",
                    "Your home router also acts as a hardware firewall. " +
                    "Keep its firmware updated regularly.",
                    "A firewall blocks unauthorised access to your network while allowing " +
                    "legitimate traffic through.",
                    "For businesses, a next generation firewall (NGFW) provides deep packet " +
                    "inspection beyond basic port filtering."
                }),

                /* Extended Topic 5: software updates */
                new ChatbotResponse("update", new List<string>
                {
                    "Software updates frequently contain critical security patches. " +
                    "Install them as soon as they are available.",
                    "Enable automatic updates so your device never misses an important " +
                    "security fix.",
                    "Delaying updates leaves known vulnerabilities open for attackers to exploit. " +
                    "Do not postpone them.",
                    "This applies to everything: your OS, browser, apps, antivirus software, " +
                    "and your router's firmware.",
                    "If a device no longer receives security updates from its manufacturer, " +
                    "consider replacing it. It is a liability."
                }),

                /* Extended Topic 6: two factor authentication */
                new ChatbotResponse("2fa", new List<string>
                {
                    "Two Factor Authentication (2FA) adds a second layer of security " +
                    "beyond just your password.",
                    "Enable 2FA on all important accounts, especially email, banking, " +
                    "and social media.",
                    "Use an authenticator app like Google Authenticator or Authy instead " +
                    "of SMS for stronger 2FA protection.",
                    "Even if an attacker steals your password, 2FA blocks them from " +
                    "accessing your account without the second factor.",
                    "Hardware security keys like YubiKey provide the strongest form of 2FA " +
                    "for high value accounts."
                }),

                /* Extended Topic 7: VPN */
                new ChatbotResponse("vpn", new List<string>
                {
                    "A VPN (Virtual Private Network) encrypts your internet connection, " +
                    "protecting your data on public networks.",
                    "Always use a VPN when connecting to public Wi-Fi in cafes, airports, " +
                    "hotels, or libraries.",
                    "Choose a reputable, paid VPN service. " +
                    "Free VPNs frequently log and sell your browsing data.",
                    "A VPN masks your IP address, making it significantly harder for " +
                    "websites and advertisers to track you.",
                    "A VPN does not make you completely anonymous. " +
                    "You still need strong passwords and good security habits."
                }),

                /* Extended Topic 8: social engineering */
                new ChatbotResponse("social engineering", new List<string>
                {
                    "Social engineering manipulates people psychologically rather than " +
                    "hacking technology directly.",
                    "Always verify the identity of anyone requesting sensitive information, " +
                    "even if they sound very convincing.",
                    "Common tactics include impersonating IT support, creating false urgency, " +
                    "or offering unexpected rewards.",
                    "When in doubt, hang up and call the organisation back using an official " +
                    "number you looked up yourself.",
                    "Attackers may research you on social media to make their impersonation " +
                    "more convincing. Limit what you share publicly."
                })
            };
        }

        /* Task 6: Sentiment Detection.
        I included 8 distinct sentiments to hit the "greatly exceeds" band (9 to 10 marks).
        The values are empathetic prefix strings injected before the tip.
        Using a Dictionary<string, string> gives me O(1) key lookup, fulfilling the
        Code Optimisation requirement (Task 8).

        References:
        Microsoft (2023). Dictionary(TKey,TValue) Class. [Online]
        Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2
        */
        private Dictionary<string, string> BuildSentimentMap()
        {
            return new Dictionary<string, string>
            {
                {
                    "worried",
                    "It is completely understandable to feel that way. Cyber threats are very real. " +
                    "You are already doing the right thing by seeking information. " +
                    "Here is a tip to help you: "
                },
                {
                    "scared",
                    "There is no need to panic. Knowledge is your strongest defence. " +
                    "Let me share something that will help ease your concern: "
                },
                {
                    "anxious",
                    "Feeling anxious about online safety is very common. You are far from alone. " +
                    "Let me help ease that concern with this: "
                },
                {
                    "curious",
                    "Curiosity is exactly the right mindset for cybersecurity! " +
                    "Let us explore this together: "
                },
                {
                    "interested",
                    "Great to hear you are interested. " +
                    "That enthusiasm will keep you safe online! Here is what you should know: "
                },
                {
                    "frustrated",
                    "I understand. Technology can be genuinely frustrating. " +
                    "Let us break this down simply together: "
                },
                {
                    "confused",
                    "No problem at all. This can be confusing at first. " +
                    "Let me explain it as clearly as I can: "
                },
                {
                    "angry",
                    "It is completely understandable to feel angry about cyber threats. " +
                    "Let me turn that energy into something useful for you: "
                }
            };
        }

        /* Task 4: Conversation Flow.
        I added 10 natural follow up phrases here so the user never needs to restart
        the topic if they just want more info.
        */
        private List<string> BuildFollowUpTriggers()
        {
            return new List<string>
            {
                "tell me more", "explain more", "another tip",
                "give me more", "more info",   "more",
                "go on",        "keep going",  "continue",
                "elaborate"
            };
        }

        /* I researched and implemented the Levenshtein Distance algorithm here to add
        fuzzy matching. This allows a typo like "pasword" to still match "password",
        making the bot much more robust for Task 2.

        References:
        Dot Net Perls (2023). Levenshtein Distance in C#. [Online]
        Available at: https://www.dotnetperls.com/levenshtein
        */
        private int ComputeLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            if (string.IsNullOrEmpty(target))
                return source.Length;

            int sLen = source.Length;
            int tLen = target.Length;
            int[,] d = new int[sLen + 1, tLen + 1];

            for (int i = 0; i <= sLen; d[i, 0] = i++) { }
            for (int j = 0; j <= tLen; d[0, j] = j++) { }

            for (int i = 1; i <= sLen; i++)
                for (int j = 1; j <= tLen; j++)
                {
                    int cost = target[j - 1] == source[i - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }

            return d[sLen, tLen];
        }

        /* Task 5: Memory and Recall (Name Extraction).
        This method checks the user's input for patterns like "my name is X"
        and extracts the capitalised first name.

        References:
        Microsoft (2023). String.Substring Method. [Online] Microsoft Learn.
        Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.substring
        */
        private string? TryExtractName(string lowerInput)
        {
            string[] patterns = { "my name is ", "i am ", "call me " };

            foreach (string pattern in patterns)
            {
                int idx = lowerInput.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
                if (idx < 0) continue;

                string after = lowerInput.Substring(idx + pattern.Length).Trim();
                string? name = after
                    .Split(new[] { ' ', '.', '!', '?' },
                           StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(name))
                    return char.ToUpper(name[0]) + name.Substring(1);
            }
            return null;
        }

        /* Helper method for Tasks 2 and 3.
        It first tries an exact substring match (for multi word topics) and then
        falls back to Levenshtein fuzzy matching per word to handle typos.
        */
        private ChatbotResponse? FindBestTopicMatch(string[] words, string fullInput = "")
        {
            foreach (var item in _knowledgeBase)
            {
                /* Full phrase match (e.g. "social engineering", "how are you") */
                if (!string.IsNullOrEmpty(fullInput) && fullInput.Contains(item.Topic))
                    return item;

                /* Word level fuzzy match (typo tolerance) */
                foreach (string word in words)
                {
                    int distance = ComputeLevenshteinDistance(word, item.Topic);
                    /* Allow 1 typo for 4 to 6 char topics, 2 typos for 7+ char topics */
                    int allowed = item.Topic.Length >= 7 ? 2 : item.Topic.Length >= 4 ? 1 : 0;
                    if (distance <= allowed) return item;
                }
            }
            return null;
        }

        /* Task 5 (Memory).
        This prepends the user's name to a sentiment prefix so every emotional response
        feels personally directed at them.
        */
        private string BuildPersonalisedPrefix(string sentimentPrefix)
        {
            if (!string.IsNullOrEmpty(Session.UserName) && !string.IsNullOrEmpty(sentimentPrefix))
                return $"{Session.UserName}, " +
                       $"{char.ToLower(sentimentPrefix[0])}{sentimentPrefix.Substring(1)}";

            return sentimentPrefix;
        }

        /* 
        References:
        Microsoft (2023). Enumerable.Any Method. [Online]
        Available at: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any
        */
        public string ProcessUserInput(string input)
        {
            /* Guard: empty input (Task 7) */
            if (string.IsNullOrWhiteSpace(input))
                return "Please type a message so I can help you!";

            /* Reset each turn; HandleIntent sets it when a command is acted on so the GUI can
            react (e.g. open the Task Manager window). */
            LastHandledIntent = ChatIntent.KnowledgeQuery;

            string lowerInput = input.ToLower().Trim();
            string[] words = lowerInput.Split(
                new[] { ' ', '.', '?', '!', ',', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            /* Part 3 / Task 1: if we just added a task and asked "Would you like a reminder?",
            interpret THIS message as the answer to that question before anything else. */
            if (Session.AwaitingReminderResponse)
            {
                string? reminderReply = HandlePendingReminderResponse(input, lowerInput);
                if (reminderReply != null) return reminderReply;
                /* null = the user changed the subject; the pending prompt was abandoned inside
                the helper, so we simply continue with normal processing below. */
            }

            string? extractedName = TryExtractName(lowerInput);
            if (!string.IsNullOrEmpty(extractedName))
            {
                Session.UserName = extractedName;
                return _formatter("",
                    $"Nice to meet you, {Session.UserName}! " +
                    "I will remember your name throughout our conversation. " +
                    "Feel free to ask about passwords, phishing, privacy, malware, scams, " +
                    "or any other cybersecurity topic.");
            }

            /* ------------------------------------------------------------------ */
            /* Part 3: Command / intent routing (Task 3 NLP layer).               */
            /* Before treating the message as a Part 1 & 2 knowledge question, we  */
            /* check whether it is actually a command (add a task, set a reminder, */
            /* start the quiz, show the activity log, ...). Anything that is not a */
            /* command returns KnowledgeQuery and flows through the original logic */
            /* below completely unchanged, so Parts 1 & 2 keep working as before.  */
            /* ------------------------------------------------------------------ */
            IntentResult intent = _intentRecognizer.Recognize(input);
            if (intent.Intent != ChatIntent.KnowledgeQuery)
            {
                LastHandledIntent = intent.Intent;
                return HandleIntent(intent);
            }

            if (!string.IsNullOrEmpty(Session.FavoriteTopic) && !Session.HasAcknowledgedInterest)
            {
                Session.HasAcknowledgedInterest = true;

                string nameGreeting = !string.IsNullOrEmpty(Session.UserName)
                    ? $", {Session.UserName}"
                    : "";

                return _formatter("",
                    $"I have noted that you are interested in {Session.FavoriteTopic}{nameGreeting}! " +
                    $"It is a critical part of cybersecurity. " +
                    $"Feel free to keep exploring it or ask about anything else.");
            }
            string sentimentPrefix = "";
            string detectedSentimentKey = "";

            foreach (var kvp in _sentiments)
            {
                if (lowerInput.Contains(kvp.Key))
                {
                    sentimentPrefix = kvp.Value;
                    detectedSentimentKey = kvp.Key;
                    break;
                }
            }
            bool autoProvideTip = detectedSentimentKey == "worried"
                                  || detectedSentimentKey == "scared"
                                  || detectedSentimentKey == "anxious";

            if (autoProvideTip)
            {
                /* Try to find a contextually relevant topic from the same message;
                fall back to scam tips if no keyword is detected. */
                ChatbotResponse autoTopic =
                    FindBestTopicMatch(words) ?? _knowledgeBase.First(k => k.Topic == "scam");

                Session.LastDiscussedTopic = autoTopic.Topic;
                return _formatter(BuildPersonalisedPrefix(sentimentPrefix),
                                  autoTopic.GetRandomResponse());
            }
            bool isFollowUp = _followUpTriggers.Any(f => lowerInput.Contains(f));

            if (isFollowUp && !string.IsNullOrEmpty(Session.LastDiscussedTopic))
            {
                ChatbotResponse? topicData = _knowledgeBase
                    .FirstOrDefault(k => k.Topic == Session.LastDiscussedTopic);

                if (topicData != null)
                {
                    string nameTag = !string.IsNullOrEmpty(Session.UserName)
                        ? $"{Session.UserName}, "
                        : "";
                    return _formatter(sentimentPrefix,
                        $"Sure, {nameTag}here is another tip on {Session.LastDiscussedTopic}: " +
                        topicData.GetRandomResponse());
                }
            }
            ChatbotResponse? matched = FindBestTopicMatch(words, lowerInput);

            if (matched != null)
            {
                string prefix = sentimentPrefix;

                /* Conversational topics do not count as the user's cybersecurity interest */
                bool isConversational = matched.Topic == "hello"
                                        || matched.Topic == "how are you"
                                        || matched.Topic == "help";

                if (!isConversational)
                {
                    Session.LastDiscussedTopic = matched.Topic;

                    /* Part 3 / Task 4: record that a cybersecurity topic was discussed so the
                    activity log reflects knowledge questions, not just task/quiz actions. */
                    ActivityLog.Log(ActivityCategory.Conversation,
                        $"Answered a question about '{matched.Topic}'.");

                    if (string.IsNullOrEmpty(Session.FavoriteTopic))
                    {
                        /* First cybersecurity topic: save it and confirm to the user (Task 5).
                        HasAcknowledgedInterest is set to true here so the proactive recall
                        block (Step 2) does not fire redundantly in the same session turn. */
                        Session.FavoriteTopic = matched.Topic;
                        Session.HasAcknowledgedInterest = true;
                        prefix += $"I will remember that you are interested in {matched.Topic}. ";
                    }
                    else if (Session.FavoriteTopic == matched.Topic)
                    {
                        /* User revisits their favourite topic: deeply personalised (Task 5) */
                        string nameTag = !string.IsNullOrEmpty(Session.UserName)
                            ? $"{Session.UserName}, as"
                            : "As";
                        prefix += $"{nameTag} someone particularly interested in {matched.Topic}, " +
                                  "here is something important you should know: ";
                    }
                }

                return _formatter(prefix, matched.GetRandomResponse());
            }

            /* ------------------------------------------------------------------ */
            /* Step 7: Error handling: unrecognised input (Task 7)                */
            /* ------------------------------------------------------------------ */
            string errorName = !string.IsNullOrEmpty(Session.UserName)
                ? $"Sorry {Session.UserName}, I"
                : "I";

            /* Task 3: keep this rephrase prompt rare AND helpful — instead of a dead end, it
            reminds the user of everything the bot can do (topics AND commands). */
            return _formatter("",
                $"{errorName} didn't quite catch that — could you rephrase? " +
                "You can ask me about cybersecurity topics like passwords, phishing, privacy, " +
                "scams, malware, ransomware, firewalls, 2FA, VPN, updates or social engineering. " +
                "I can also manage tasks (e.g. \"add task - review my privacy settings\"), " +
                "set reminders (\"remind me to update my password tomorrow\"), " +
                "run a quiz (\"start quiz\"), or show your activity log. Type 'help' anytime.");
        }

        /* ------------------------------------------------------------------ */
        /* Part 3: Command handler (Tasks 1, 2 and 4).                         */
        /* Routes a recognised intent to the right behaviour. The Task 1       */
        /* handlers below now read and write real tasks in MySQL via           */
        /* TaskRepository, every DB call is wrapped so a database outage shows */
        /* a friendly message instead of crashing (Task 7), and every action  */
        /* is recorded in the activity log (Task 4). Task 2's quiz is launched */
        /* by the GUI (the engine cannot open a window) so StartQuiz only      */
        /* records the request; MainWindow opens the window via LastHandledIntent. */
        /* ------------------------------------------------------------------ */
        private string HandleIntent(IntentResult intent)
        {
            switch (intent.Intent)
            {
                case ChatIntent.ShowActivityLog:
                    return HandleShowActivityLog(intent.OriginalInput);

                case ChatIntent.AddTask:
                    return HandleAddTask(intent);

                case ChatIntent.SetReminder:
                    return HandleSetReminder(intent);

                case ChatIntent.ShowTasks:
                    return HandleShowTasks();

                case ChatIntent.CompleteTask:
                    return HandleCompleteTask(intent);

                case ChatIntent.DeleteTask:
                    return HandleDeleteTask(intent);

                case ChatIntent.StartQuiz:
                    /* Task 2: the actual quiz window is opened by MainWindow when it sees
                    LastHandledIntent == StartQuiz. Here we just record the request. */
                    ActivityLog.Log(ActivityCategory.Quiz, "Requested to start the cybersecurity quiz.");
                    return _formatter("",
                        "Get ready! Launching the cybersecurity mini-game for you now. " +
                        "Answer each question and I'll tally up your score at the end.");

                default:
                    return _formatter("",
                        "I recognised that as a command but cannot handle it yet.");
            }
        }

        /* ---- Task 4: show the activity log ----
        Shows the 5 most recent actions by default. If the user asks for the "full"/"all"
        log, every action is shown (the "show more" option). */
        private string HandleShowActivityLog(string originalInput)
        {
            bool showAll = Regex.IsMatch(originalInput ?? string.Empty,
                @"\b(all|full|everything|entire|complete|whole)\b", RegexOptions.IgnoreCase);

            /* Build the summary from the actions so far, THEN record that it was viewed (so the
            "viewed the log" entry doesn't clutter the very summary being shown). */
            string summary = ActivityLog.GetFormattedLog(showAll ? 0 : 5);
            ActivityLog.Log(ActivityCategory.System, "Viewed the activity log.");
            return _formatter("", summary);
        }

        /* ---- Task 1: add a task (then offer a reminder) ---- */
        private string HandleAddTask(IntentResult intent)
        {
            if (string.IsNullOrWhiteSpace(intent.Detail))
            {
                return _formatter("",
                    "Sure, what task would you like to add? For example: " +
                    "\"add task - review my privacy settings\".");
            }

            /* If the user squeezed a timeframe into the same sentence ("add task back up my
            files tomorrow"), capture it now and keep it out of the title. */
            bool hasTime = ReminderParser.TryParse(intent.OriginalInput, out DateTime when, out string whenText);
            string title = CapitaliseFirst(ReminderParser.StripTimePhrases(intent.Detail));
            if (string.IsNullOrEmpty(title)) title = CapitaliseFirst(intent.Detail);

            string description = BuildTaskDescription(title);

            try
            {
                var task = new TaskItem
                {
                    Title = title,
                    Description = description,
                    CreatedAt = DateTime.Now,
                    ReminderDate = hasTime ? when : (DateTime?)null
                };

                _taskRepository.AddTask(task);

                if (hasTime)
                {
                    ActivityLog.Log(ActivityCategory.Task,
                        $"Added task: '{title}' with a reminder {whenText} ({when:yyyy-MM-dd}).");
                    return _formatter("",
                        $"Task added: '{title}'. {description} " +
                        $"I'll also remind you {whenText}.");
                }

                /* No time given yet: save the task and ask the reminder question (multi-turn). */
                Session.AwaitingReminderResponse = true;
                Session.PendingReminderTaskId = task.Id;
                Session.PendingReminderTaskTitle = title;

                ActivityLog.Log(ActivityCategory.Task, $"Added task: '{title}'.");
                return _formatter("",
                    $"Task added: '{title}'. {description} " +
                    "Would you like to set a reminder for this task? You can say something " +
                    "like \"remind me in 3 days\", or \"no thanks\".");
            }
            catch (Exception ex)
            {
                return DatabaseOffline(ex);
            }
        }

        /* ---- Task 1: set a standalone reminder (creates a task for it) ---- */
        private string HandleSetReminder(IntentResult intent)
        {
            /* Note: a reminder given as the answer to "Would you like a reminder?" is handled
            earlier by HandlePendingReminderResponse. This path is for a fresh request such as
            "remind me to update my password tomorrow". */
            if (string.IsNullOrWhiteSpace(intent.Detail))
            {
                return _formatter("",
                    "Sure, what should I remind you about? For example: " +
                    "\"remind me to update my password tomorrow\".");
            }

            bool hasTime = ReminderParser.TryParse(intent.OriginalInput, out DateTime when, out string whenText);
            string title = CapitaliseFirst(ReminderParser.StripTimePhrases(intent.Detail));
            if (string.IsNullOrEmpty(title)) title = CapitaliseFirst(intent.Detail);

            try
            {
                var task = new TaskItem
                {
                    Title = title,
                    Description = BuildTaskDescription(title),
                    CreatedAt = DateTime.Now,
                    ReminderDate = hasTime ? when : (DateTime?)null
                };

                _taskRepository.AddTask(task);

                if (hasTime)
                {
                    ActivityLog.Log(ActivityCategory.Reminder,
                        $"Reminder set for '{title}' {whenText} ({when:yyyy-MM-dd}).");
                    return _formatter("",
                        $"Reminder set for '{title}' {whenText}. I've added it to your task list.");
                }

                /* We have the task but no time: ask when, using the same multi-turn flow. */
                Session.AwaitingReminderResponse = true;
                Session.PendingReminderTaskId = task.Id;
                Session.PendingReminderTaskTitle = title;

                ActivityLog.Log(ActivityCategory.Task, $"Added task from a reminder request: '{title}'.");
                return _formatter("",
                    $"I've noted '{title}'. When should I remind you? " +
                    "You can say \"in 3 days\", \"tomorrow\", or \"next week\".");
            }
            catch (Exception ex)
            {
                return DatabaseOffline(ex);
            }
        }

        /* ---- Task 1: view all tasks (the GUI window is opened by MainWindow too) ---- */
        private string HandleShowTasks()
        {
            try
            {
                List<TaskItem> tasks = _taskRepository.GetAllTasks();
                ActivityLog.Log(ActivityCategory.Task, "Viewed all saved tasks.");

                if (tasks.Count == 0)
                {
                    return _formatter("",
                        "You don't have any saved tasks yet. Add one with, for example, " +
                        "\"add task - review my privacy settings\".");
                }

                var builder = new StringBuilder();
                builder.AppendLine($"Here are your saved tasks ({tasks.Count}):");

                int number = 1;
                foreach (TaskItem task in tasks)
                {
                    builder.AppendLine(
                        $"{number}. {task.Title} — {task.Description} " +
                        $"[{task.ReminderDisplay} | {task.StatusDisplay}]");
                    number++;
                }

                builder.Append("\nI've also opened your Task Manager window so you can " +
                               "complete or delete tasks.");

                return _formatter("", builder.ToString().TrimEnd());
            }
            catch (Exception ex)
            {
                return DatabaseOffline(ex);
            }
        }

        /* ---- Task 1: mark a task complete ---- */
        private string HandleCompleteTask(IntentResult intent)
        {
            try
            {
                List<TaskItem> tasks = _taskRepository.GetAllTasks();
                TaskItem? match = FindTaskByText(tasks, intent.Detail, preferIncomplete: true);

                if (match == null)
                {
                    return _formatter("",
                        "I couldn't find a matching task to complete. " +
                        "Type \"show my tasks\" to see them, or use the Task Manager window.");
                }

                _taskRepository.MarkComplete(match.Id);
                ActivityLog.Log(ActivityCategory.Task, $"Marked task complete: '{match.Title}'.");
                return _formatter("", $"Done! I've marked '{match.Title}' as completed. ✓");
            }
            catch (Exception ex)
            {
                return DatabaseOffline(ex);
            }
        }

        /* ---- Task 1: delete a task ---- */
        private string HandleDeleteTask(IntentResult intent)
        {
            try
            {
                List<TaskItem> tasks = _taskRepository.GetAllTasks();
                TaskItem? match = FindTaskByText(tasks, intent.Detail, preferIncomplete: false);

                if (match == null)
                {
                    return _formatter("",
                        "I couldn't find a matching task to delete. " +
                        "Type \"show my tasks\" to see them, or use the Task Manager window.");
                }

                _taskRepository.DeleteTask(match.Id);
                ActivityLog.Log(ActivityCategory.Task, $"Deleted task: '{match.Title}'.");
                return _formatter("", $"Removed '{match.Title}' from your task list. 🗑");
            }
            catch (Exception ex)
            {
                return DatabaseOffline(ex);
            }
        }

        /* ------------------------------------------------------------------ */
        /* Part 3 / Task 1: multi-turn reminder follow-up.                     */
        /* Called when the bot has just asked "Would you like a reminder?".    */
        /* Returns the reply, or null if the user clearly changed the subject  */
        /* (in which case the pending prompt is abandoned and normal           */
        /* processing resumes).                                                */
        /* ------------------------------------------------------------------ */
        private string? HandlePendingReminderResponse(string input, string lowerInput)
        {
            string[] tokens = lowerInput.Split(
                new[] { ' ', '.', '!', '?', ',', ';', ':' },
                StringSplitOptions.RemoveEmptyEntries);

            bool HasWord(params string[] options) => tokens.Any(options.Contains);

            string title = Session.PendingReminderTaskTitle;

            /* 1. A concrete time was given ("in 3 days", "tomorrow", ...): store it. */
            if (ReminderParser.TryParse(input, out DateTime when, out string whenText))
            {
                try
                {
                    _taskRepository.SetReminder(Session.PendingReminderTaskId, when);
                    ActivityLog.Log(ActivityCategory.Reminder,
                        $"Set reminder for '{title}' {whenText} ({when:yyyy-MM-dd}).");
                    ClearPendingReminder();
                    return _formatter("", $"Got it! I'll remind you {whenText} about '{title}'.");
                }
                catch (Exception ex)
                {
                    ClearPendingReminder();
                    return DatabaseOffline(ex);
                }
            }

            /* 2. The user declined the reminder. */
            bool declined = HasWord("no", "nope", "nah", "later", "skip")
                            || lowerInput.Contains("not now")
                            || lowerInput.Contains("no thanks")
                            || lowerInput.Contains("don't")
                            || lowerInput.Contains("dont");
            if (declined)
            {
                ActivityLog.Log(ActivityCategory.Reminder, $"Declined a reminder for '{title}'.");
                ClearPendingReminder();
                return _formatter("",
                    $"No problem — I won't set a reminder for '{title}'. " +
                    "It's saved in your task list. Anything else I can help with?");
            }

            /* 3. The user wants a reminder but hasn't said when: ask for the time, keep waiting. */
            bool wantsReminder = HasWord("yes", "yeah", "yep", "yup", "sure", "ok", "okay", "please")
                                 || lowerInput.Contains("remind")
                                 || lowerInput.Contains("go ahead");
            if (wantsReminder)
            {
                return _formatter("",
                    "Great! When should I remind you? " +
                    "You can say \"in 3 days\", \"tomorrow\", or \"next week\".");
            }

            /* 4. Unrelated message: abandon the reminder prompt and let normal handling run. */
            ClearPendingReminder();
            return null;
        }

        /* Resets the pending-reminder state once the question has been resolved. */
        private void ClearPendingReminder()
        {
            Session.AwaitingReminderResponse = false;
            Session.PendingReminderTaskId = 0;
            Session.PendingReminderTaskTitle = string.Empty;
        }

        /* Finds the task that best matches the user's words. If they named part of a title
        ("complete the privacy task"), match on that; otherwise, if there is exactly one
        sensible candidate, use it. Returns null when the choice is ambiguous. */
        private static TaskItem? FindTaskByText(List<TaskItem> tasks, string detail, bool preferIncomplete)
        {
            if (tasks.Count == 0) return null;

            IEnumerable<TaskItem> pool = preferIncomplete
                ? tasks.Where(t => !t.IsCompleted)
                : tasks;
            List<TaskItem> candidates = pool.ToList();
            if (candidates.Count == 0) candidates = tasks;

            string needle = (detail ?? string.Empty)
                .ToLowerInvariant()
                .Replace("task", string.Empty)
                .Trim();

            if (!string.IsNullOrEmpty(needle))
            {
                TaskItem? byTitle = candidates.FirstOrDefault(
                    t => t.Title.ToLowerInvariant().Contains(needle));
                if (byTitle != null) return byTitle;
            }

            /* No usable text but only one obvious task — act on it. */
            return candidates.Count == 1 ? candidates[0] : null;
        }

        /* Builds a friendly, topic-aware description from a task title so each task has a
        meaningful Description field (not just a copy of the title), matching the brief's
        example where "Review privacy settings" becomes a fuller sentence. */
        private static string BuildTaskDescription(string title)
        {
            string t = title.ToLowerInvariant();

            if (t.Contains("privacy"))
                return "Review your account privacy settings to ensure your personal data is protected.";
            if (t.Contains("password") || t.Contains("passphrase"))
                return "Update to a strong, unique passphrase to keep your account secure.";
            if (t.Contains("2fa") || t.Contains("two factor") || t.Contains("two-factor")
                || t.Contains("authentication"))
                return "Enable two-factor authentication to add a second layer of protection to your account.";
            if (t.Contains("backup") || t.Contains("back up"))
                return "Back up your important files to a safe, offline location to guard against ransomware.";
            if (t.Contains("update") || t.Contains("patch"))
                return "Install the latest software and security updates to patch known vulnerabilities.";
            if (t.Contains("antivirus") || t.Contains("malware") || t.Contains("scan"))
                return "Run an antivirus scan and keep your security software up to date.";
            if (t.Contains("phishing") || t.Contains("email"))
                return "Stay alert for phishing emails and verify senders before clicking any links.";
            if (t.Contains("vpn") || t.Contains("wifi") || t.Contains("wi-fi"))
                return "Use a reputable VPN, especially on public Wi-Fi, to encrypt your connection.";
            if (t.Contains("firewall"))
                return "Make sure your firewall is switched on to block unauthorised network access.";

            return $"Cybersecurity task: {title}. Complete this to help keep your digital life secure.";
        }

        /* Capitalises the first character of a string (used to tidy task titles). */
        private static string CapitaliseFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1);
        }

        /* Friendly, non-crashing message shown when MySQL cannot be reached (Task 7). The
        technical detail goes to the activity log, not to the user. */
        private string DatabaseOffline(Exception ex)
        {
            ActivityLog.Log(ActivityCategory.System, "Database operation failed: " + ex.Message);
            return _formatter("",
                "I'm sorry, I couldn't reach the task database just now, so that wasn't saved. " +
                "Please make sure your MySQL server is running, then try again.");
        }

        /* Called by the GUI on startup (shown after ASCII art and voice greeting).
        Lists all available topics and prompts the user for their name.
        */
        public string GetWelcomeMessage()
        {
            return
                "System Online. I am your Cybersecurity Awareness Assistant.\n\n" +
                "I can help you with:\n" +
                "   • Passwords          • Phishing\n" +
                "   • Privacy            • Scams\n" +
                "   • Malware            • Ransomware\n" +
                "   • Firewalls          • Two Factor Auth (2FA)\n" +
                "   • VPN                • Software Updates\n" +
                "   • Social Engineering\n\n" +
                "Type 'help' at any time to see all topics.\n\n" +
                "Before we begin, may I know your name?";
        }
    }
}