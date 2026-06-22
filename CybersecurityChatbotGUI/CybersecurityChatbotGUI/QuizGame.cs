using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 2: Cybersecurity Mini-Game (Quiz).
    This class is the "engine" of the quiz: it holds the question bank and tracks the state of
    one play-through (which question we are on and the running score). The GUI window drives it
    one step at a time, which keeps all the game rules here and all the visuals in the window
    (separation of concerns).

    The bank has 13 questions (comfortably more than the 10 required) covering phishing,
    password safety, safe browsing, social engineering and more, and deliberately MIXES
    multiple-choice and true/false formats for variety.

    References:
    Microsoft (2023). List<T> Class. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1
    */
    public class QuizGame
    {
        private static readonly Random _rng = new Random();

        private readonly List<QuizQuestion> _questions;
        private int _index;

        /* Number of correct answers so far. */
        public int Score { get; private set; }

        /* Total number of questions in this play-through. */
        public int Total => _questions.Count;

        /* 1-based number of the current question, for display ("Question 3 of 13"). */
        public int CurrentNumber => _index + 1;

        /* The question currently being shown. */
        public QuizQuestion Current => _questions[_index];

        /* True once every question has been answered. */
        public bool IsFinished => _index >= _questions.Count;

        public QuizGame()
        {
            /* Shuffle so each play-through feels fresh (the order, not the options, so the
            stored correct-answer indexes stay valid). */
            _questions = BuildQuestionBank().OrderBy(_ => _rng.Next()).ToList();
            _index = 0;
            Score = 0;
        }

        /* Records the user's answer to the current question and returns whether it was correct.
        Increments the score on a correct answer. */
        public bool Answer(int selectedIndex)
        {
            bool correct = Current.IsCorrect(selectedIndex);
            if (correct) Score++;
            return correct;
        }

        /* Advances to the next question. */
        public void Next() => _index++;

        /* Task 2: the score-based closing message. */
        public string GetScoreFeedback()
        {
            double percent = Total == 0 ? 0 : (double)Score / Total;

            if (percent >= 0.8)
                return "🏆 Great job! You're a cybersecurity pro!";
            if (percent >= 0.5)
                return "👍 Nice work! You've got the basics — keep sharpening your skills.";
            return "📚 Keep learning to stay safe online!";
        }

        /* The full question bank (Task 2: 10+ questions, mixed formats). */
        private static List<QuizQuestion> BuildQuestionBank()
        {
            return new List<QuizQuestion>
            {
                /* ---- Phishing (the brief's example question) ---- */
                QuizQuestion.MultipleChoice(
                    "What should you do if you receive an email asking for your password?",
                    new[]
                    {
                        "Reply with your password",
                        "Delete the email",
                        "Report the email as phishing",
                        "Ignore it"
                    },
                    2,
                    "Reporting phishing emails helps prevent scams and warns others. " +
                    "Legitimate organisations never ask for your password by email."),

                /* ---- Password safety ---- */
                QuizQuestion.MultipleChoice(
                    "Which of these is the strongest password?",
                    new[]
                    {
                        "password123",
                        "Your pet's name",
                        "A long passphrase of four random words",
                        "Your date of birth"
                    },
                    2,
                    "A long passphrase of random words is both hard to crack and easy to " +
                    "remember. Personal details and common words are easily guessed."),

                QuizQuestion.MultipleChoice(
                    "What is the safest way to manage many different passwords?",
                    new[]
                    {
                        "Use the same password everywhere",
                        "Write them on a sticky note on your monitor",
                        "Use a reputable password manager",
                        "Save them in a plain text file on your desktop"
                    },
                    2,
                    "A password manager stores unique, strong passwords securely so you only " +
                    "have to remember one master password."),

                /* ---- Safe browsing ---- */
                QuizQuestion.MultipleChoice(
                    "A link in an email points to 'paypa1-login.com'. What should you do?",
                    new[]
                    {
                        "Click it quickly before the offer expires",
                        "Do not click; the misspelt address suggests a spoofed site",
                        "Enter your details to check if it is real",
                        "Forward it to all your friends"
                    },
                    1,
                    "Attackers register look-alike domains (paypa1 vs paypal) to trick you. " +
                    "Type the real address yourself instead of clicking."),

                QuizQuestion.MultipleChoice(
                    "What is the safest thing to do before banking online on public Wi-Fi?",
                    new[]
                    {
                        "Nothing, public Wi-Fi is always safe",
                        "Turn off your screen lock",
                        "Connect through a reputable VPN",
                        "Share the network with friends"
                    },
                    2,
                    "A VPN encrypts your connection so others on the same public network " +
                    "cannot intercept your data."),

                /* ---- Social engineering ---- */
                QuizQuestion.MultipleChoice(
                    "You get an urgent call from 'your bank' asking for your PIN. What do you do?",
                    new[]
                    {
                        "Give the PIN since they sound official",
                        "Hang up and call the bank using the number on your card",
                        "Read out your PIN slowly to be safe",
                        "Send it by text instead"
                    },
                    1,
                    "Banks never ask for your full PIN. Hang up and call back on an official " +
                    "number — urgency is a classic social-engineering tactic."),

                QuizQuestion.MultipleChoice(
                    "What is 'phishing'?",
                    new[]
                    {
                        "A type of firewall",
                        "A scam that tricks you into revealing personal information",
                        "A way to speed up your computer",
                        "A secure password method"
                    },
                    1,
                    "Phishing uses fake emails, texts or sites that impersonate trusted " +
                    "organisations to steal your information."),

                /* ---- Two-factor authentication ---- */
                QuizQuestion.MultipleChoice(
                    "What does two-factor authentication (2FA) add to your login?",
                    new[]
                    {
                        "A second layer of security beyond your password",
                        "A faster way to log in",
                        "A backup of your files",
                        "A stronger Wi-Fi signal"
                    },
                    0,
                    "2FA requires a second factor (like a code from an app), so a stolen " +
                    "password alone is not enough to break in."),

                /* ---- Malware / ransomware ---- */
                QuizQuestion.MultipleChoice(
                    "What is ransomware?",
                    new[]
                    {
                        "Software that blocks adverts",
                        "Malware that encrypts your files and demands payment",
                        "A free antivirus tool",
                        "A type of strong password"
                    },
                    1,
                    "Ransomware locks your files and demands a payment. Regular offline " +
                    "backups are the best protection."),

                /* ---- True / False questions ---- */
                QuizQuestion.TrueFalse(
                    "You should use the same strong password for all of your accounts.",
                    false,
                    "Never reuse passwords. If one site is breached, attackers will try that " +
                    "password everywhere else."),

                QuizQuestion.TrueFalse(
                    "A padlock / HTTPS in the address bar guarantees a website is safe and honest.",
                    false,
                    "HTTPS only means the connection is encrypted. Scam sites can use HTTPS too, " +
                    "so still check the address is legitimate."),

                QuizQuestion.TrueFalse(
                    "Installing software updates promptly helps protect you from attacks.",
                    true,
                    "Updates patch security holes that attackers exploit, so install them as " +
                    "soon as they are available."),

                QuizQuestion.TrueFalse(
                    "It is safe to open email attachments from unknown senders if your antivirus is on.",
                    false,
                    "Antivirus can miss brand-new threats. Unexpected attachments from unknown " +
                    "senders are a top way malware spreads — don't open them.")
            };
        }
    }
}
