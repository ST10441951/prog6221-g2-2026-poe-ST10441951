using System.Text.RegularExpressions;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Tasks 1-4: The set of "commands" the chatbot can understand on top of the
    Part 1 & 2 knowledge questions. Anything that is NOT one of these falls through to
    KnowledgeQuery, which preserves all of the original topic/sentiment behaviour untouched.
    */
    public enum ChatIntent
    {
        AddTask,         // Task 1: "add task - review privacy settings"
        SetReminder,     // Task 1: "remind me to update my password tomorrow"
        ShowTasks,       // Task 1: "show my tasks"
        CompleteTask,    // Task 1: "mark task as done"
        DeleteTask,      // Task 1: "delete task"
        StartQuiz,       // Task 2: "start the quiz" / "play the game"
        ShowActivityLog, // Task 4: "what have you done for me?"
        KnowledgeQuery   // Default: hand back to the Part 1 & 2 topic engine
    }

    /* The outcome of analysing one user message: which intent it is, plus any useful text
    pulled out of it (e.g. the task description). The chatbot engine uses Detail to build the
    TaskItem when the user adds a task (Task 1).
    */
    public class IntentResult
    {
        public ChatIntent Intent { get; set; } = ChatIntent.KnowledgeQuery;
        public string OriginalInput { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    /* ====================================================================================== */
    /* Part 3 / Task 3: Natural Language Processing (NLP) Simulation.                          */
    /*                                                                                        */
    /* This class is the chatbot's "understanding" layer. The brief asks us to SIMULATE NLP   */
    /* with basic string manipulation so the bot recognises the same request worded in many   */
    /* different ways, and to keep the "I didn't understand" reply to an absolute minimum.     */
    /*                                                                                        */
    /* The simple NLP techniques I researched and applied here are:                            */
    /*                                                                                        */
    /*   1. NORMALISATION (text pre-processing) - Normalise() lower-cases the text, removes    */
    /*      apostrophes and punctuation, and collapses whitespace, so "Can you remind me?" and */
    /*      "can you remind me" are treated identically. This is the same first step real NLP  */
    /*      pipelines call "tokenisation/normalisation".                                       */
    /*                                                                                        */
    /*   2. KEYWORD & SYNONYM DETECTION - each intent has a bank of trigger words/phrases       */
    /*      (e.g. add / create / new / note down all mean AddTask). Matching is done with       */
    /*      string.Contains for distinctive phrases and whole-word regex for short verbs        */
    /*      (so "view" is not found inside "review").                                           */
    /*                                                                                        */
    /*   3. INTENT RANKING BY SPECIFICITY - checks run most-specific first (show/complete/      */
    /*      delete a task before the generic "add"), which resolves ambiguous sentences.        */
    /*                                                                                        */
    /*   4. ENTITY EXTRACTION - regular expressions pull the useful payload out of a command    */
    /*      (the task title, or what to be reminded about).                                     */
    /*                                                                                        */
    /* (Typo tolerance for knowledge questions is handled separately by the Levenshtein-based   */
    /* matcher in ChatbotEngine, which complements this keyword layer.)                         */
    /*                                                                                        */
    /* References:                                                                             */
    /* Microsoft (2023). String.Contains Method. [Online] Microsoft Learn.                     */
    /*   https://learn.microsoft.com/en-us/dotnet/api/system.string.contains                   */
    /* Microsoft (2023). Regular Expressions in .NET. [Online] Microsoft Learn.                */
    /*   https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions      */
    /* Jurafsky, D. and Martin, J.H. (2023). Speech and Language Processing. 3rd ed. draft,    */
    /*   ch. 2 (Regular Expressions, Text Normalization) - background on the techniques above.  */
    /* ====================================================================================== */
    public class IntentRecognizer
    {
        /* --- Phrase banks (Task 3 keyword detection). Add new wordings here at any time. --- */

        private static readonly string[] ActivityLogPhrases =
        {
            "activity log", "show log", "show the log", "view log", "your log", "log of actions",
            "full log", "action log", "entire log", "whole log", "full activity",
            "all my actions", "everything you have done", "everything youve done",
            "what have you done", "what did you do", "what you have done", "what you did",
            "recent actions", "your actions", "actions taken", "summary of actions",
            "history of actions", "action history", "recap", "what have you done for me"
        };

        private static readonly string[] QuizPhrases =
        {
            "quiz", "mini game", "mini-game", "minigame", "start game", "begin game",
            "play game", "play a game", "lets play", "let us play", "i want to play",
            "test my knowledge", "test me", "quiz me", "challenge me", "trivia", "knowledge test"
        };

        private static readonly string[] ShowVerbs =
        {
            "show", "view", "see", "list", "display", "what are", "whats", "check",
            "tell me", "give me", "read"
        };

        private static readonly string[] CompleteVerbs =
        {
            "complete", "completed", "finish", "finished", "done", "mark",
            "tick off", "check off", "accomplished"
        };

        private static readonly string[] DeleteVerbs =
        {
            "delete", "remove", "cancel", "clear", "discard", "scrap", "get rid"
        };

        private static readonly string[] AddVerbs =
        {
            "add", "create", "new", "make", "set up", "set-up", "register", "schedule",
            "note", "note down", "jot", "jot down", "save", "log", "put"
        };

        /* Distinctive reminder triggers (besides the "remind" stem) for varied phrasings. */
        private static readonly string[] ReminderPhrases =
        {
            "remind", "remember to", "dont forget", "do not forget",
            "nudge me", "alert me", "ping me", "notify me"
        };

        /* Analyses one message and returns the detected intent (plus any extracted detail). */
        public IntentResult Recognize(string input)
        {
            var result = new IntentResult { OriginalInput = input ?? string.Empty };

            if (string.IsNullOrWhiteSpace(input))
                return result; // KnowledgeQuery: let the engine's empty-input guard handle it

            /* NLP step 1: normalise the text so wording/punctuation differences don't matter. */
            string text = Normalize(input);

            /* 1. Activity log (Task 4) — checked first so "what have you done" never gets
            misread as a knowledge question. */
            if (ContainsAny(text, ActivityLogPhrases))
            {
                result.Intent = ChatIntent.ShowActivityLog;
                return result;
            }

            /* 2. Quiz / mini-game (Task 2). */
            if (ContainsAny(text, QuizPhrases))
            {
                result.Intent = ChatIntent.StartQuiz;
                return result;
            }

            bool mentionsTask = text.Contains("task")
                                || text.Contains("to-do")
                                || text.Contains("todo");

            /* 3a. An explicit "add/create/make ... task" pattern is detected FIRST, so that a
            show-verb appearing inside the task's wording (e.g. "make a task to CHECK my privacy
            settings") is not misread as "show my tasks". */
            if (Regex.IsMatch(text,
                @"\b(add|create|make|new|note|jot|register|schedule|log|put)\b[\w\s-]*\btask\b"))
            {
                result.Intent = ChatIntent.AddTask;
                result.Detail = ExtractTaskDetail(input);
                return result;
            }

            /* 3. View tasks (Task 1). */
            if ((mentionsTask && ContainsWord(text, ShowVerbs))
                || text.Contains("my tasks")
                || text.Contains("task list")
                || text.Contains("to do list"))
            {
                result.Intent = ChatIntent.ShowTasks;
                return result;
            }

            /* 4. Mark a task complete (Task 1). */
            if (mentionsTask && ContainsWord(text, CompleteVerbs))
            {
                result.Intent = ChatIntent.CompleteTask;
                result.Detail = ExtractTaskDetail(input);
                return result;
            }

            /* 5. Delete a task (Task 1). */
            if (mentionsTask && ContainsWord(text, DeleteVerbs))
            {
                result.Intent = ChatIntent.DeleteTask;
                result.Detail = ExtractTaskDetail(input);
                return result;
            }

            /* 6. Add a task (Task 1). Checked AFTER the more specific task verbs above. */
            if (mentionsTask && ContainsWord(text, AddVerbs))
            {
                result.Intent = ChatIntent.AddTask;
                result.Detail = ExtractTaskDetail(input);
                return result;
            }

            /* 7. Set a reminder (Task 1). Catches "remind me to...", "remember to...",
            "don't forget to...", "nudge me to..." even without the word "task" — exactly the
            flexible phrasing Task 3 asks for. */
            if (ContainsAny(text, ReminderPhrases))
            {
                result.Intent = ChatIntent.SetReminder;
                result.Detail = ExtractReminderDetail(input);
                return result;
            }

            /* 8. Default: not a command, so treat it as a Part 1 & 2 knowledge question. */
            return result;
        }

        /* NLP normalisation: lower-case, strip apostrophes and punctuation (keeping hyphens for
        words like "to-do"), and collapse runs of whitespace. The result is only used for intent
        DETECTION; entity extraction below works on the original input to preserve the user's
        wording and casing. */
        private static string Normalize(string raw)
        {
            string text = raw.ToLowerInvariant();
            text = text.Replace("'", string.Empty).Replace("’", string.Empty);
            text = Regex.Replace(text, @"[^a-z0-9\s-]", " "); // punctuation -> space
            text = Regex.Replace(text, @"\s+", " ").Trim();    // collapse whitespace
            return text;
        }

        /* True if the text contains any of the supplied phrases as a plain substring. Used for
        the distinctive phrase banks (e.g. "activity log", "quiz") where substring matching is
        safe. */
        private static bool ContainsAny(string text, string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (text.Contains(phrase)) return true;
            }
            return false;
        }

        /* True if the text contains any of the supplied phrases as a WHOLE word. This matters
        for short verbs: a naive substring check would wrongly find "view" inside "review" or
        "add" inside "address", so verbs are matched on word boundaries instead. */
        private static bool ContainsWord(string text, string[] phrases)
        {
            foreach (string phrase in phrases)
            {
                if (Regex.IsMatch(text, $@"\b{Regex.Escape(phrase)}\b", RegexOptions.IgnoreCase))
                    return true;
            }
            return false;
        }

        /* Entity extraction: pulls the task description out of a command, preserving the user's
        original casing. Handles the brief's "add task - <description>" style first, then falls
        back to taking whatever follows the word "task" and stripping small connector words. */
        private static string ExtractTaskDetail(string input)
        {
            string working = input.Trim();

            /* Prefer text after a SPACED dash or a colon, e.g. "Add task - Review privacy
            settings" or "Add task: review settings". We require spaces around the dash so a
            hyphen inside a word (like "two-factor") is never mistaken for the separator. */
            int dashIndex = working.IndexOf(" - ", StringComparison.Ordinal);
            int colonIndex = working.IndexOf(':');

            int separator = -1, separatorLength = 0;
            if (dashIndex >= 0) { separator = dashIndex; separatorLength = 3; }
            else if (colonIndex >= 0) { separator = colonIndex; separatorLength = 1; }

            if (separator >= 0 && separator + separatorLength < working.Length)
                return CleanDetail(working.Substring(separator + separatorLength));

            /* Otherwise take everything after the word "task". */
            Match match = Regex.Match(working, @"\btask\b\s*(.*)$", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string after = match.Groups[1].Value;

                /* Drop a leading connector word such as "to", "called", "named", "for". */
                after = Regex.Replace(
                    after,
                    @"^(to|called|named|for|that|of|about|:)\s+",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                return CleanDetail(after);
            }

            return string.Empty;
        }

        /* Entity extraction: pulls the reminder description out of phrases like "remind me to
        update my password", "remember to back up", or "don't forget to enable 2FA". The
        date/timeframe itself is parsed separately by ReminderParser; here we isolate the "what". */
        private static string ExtractReminderDetail(string input)
        {
            string working = input.Trim();

            /* Each pattern captures the action text after the trigger phrase. They are tried in
            order, most explicit first. */
            string[] patterns =
            {
                @"remind me\s*(?:to|that|about)?\s*(.+)$",
                @"remember to\s*(.+)$",
                @"(?:do ?n'?t|dont)\s*forget\s*(?:to|about)?\s*(.+)$",
                @"(?:nudge|alert|ping|notify)\s*me\s*(?:to|about)?\s*(.+)$",
                @"reminder\s*(?:to|for|about|that)?\s*(.+)$"
            };

            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(working, pattern, RegexOptions.IgnoreCase);
                if (match.Success && !string.IsNullOrWhiteSpace(match.Groups[1].Value))
                    return CleanDetail(match.Groups[1].Value);
            }

            return string.Empty;
        }

        /* Tidies an extracted snippet: trims whitespace and strips trailing sentence
        punctuation so "enable 2FA." becomes "enable 2FA". */
        private static string CleanDetail(string detail)
        {
            return detail.Trim().TrimEnd('.', '!', '?', ',', ';');
        }
    }
}
