using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize our helper classes
            BotInterface ui = new BotInterface();
            List<ChatbotResponse> knowledgeBase = new List<ChatbotResponse>();

            // Setup the Knowledge Base (Task 4)
            knowledgeBase.Add(new ChatbotResponse("passwords", "Use at least 12 characters with symbols like @ or #. Never reuse them!"));
            knowledgeBase.Add(new ChatbotResponse("phishing", "Never click links in emails that ask for your banking PIN or 'urgent' login."));
            knowledgeBase.Add(new ChatbotResponse("browsing", "Look for the padlock icon in the URL bar. Avoid public Wi-Fi for banking."));

            // 1. Voice Greeting
            ui.PlayVoiceGreeting();

            // 2. ASCII Image
            ui.DisplayAsciiArt();

            // 3. User Interaction - Personalization
            ui.TypeMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            Console.Write("What is your name? ");
            string userName = Console.ReadLine();

            // TASK 5: Basic Input Validation (Empty name)
            if (string.IsNullOrWhiteSpace(userName)) userName = "Friend";

            ui.TypeMessage("\nNice to meet you, " + userName + "!");
            ui.TypeMessage("I am here to help South Africans stay safe online.");

            bool keepRunning = true;
            while (keepRunning)
            {
                ui.ShowHeader("Main Menu");
                ui.TypeMessage("What would you like to learn about? (Type: Passwords, Phishing, Browsing, or Exit)");
                Console.ForegroundColor = ConsoleColor.Green; // User input indicator
                Console.Write("> ");
                string input = Console.ReadLine().ToLower().Trim();
                Console.ResetColor();

                if (input == "exit")
                {
                    ui.TypeMessage("Stay safe, " + userName + "! Goodbye.");
                    keepRunning = false;
                }
                else
                {
                    // Search knowledge base
                    bool found = false;
                    foreach (var item in knowledgeBase)
                    {
                        if (input.Contains(item.Topic))
                        {
                            ui.ShowHeader("Cybersecurity Tip");
                            ui.TypeMessage(item.Information);
                            found = true;
                            break;
                        }
                    }

                    // TASK 5: Input Validation / Default Response
                    if (!found)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ui.TypeMessage("I didn't quite understand that. Could you rephrase? Try asking about 'phishing'!");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}