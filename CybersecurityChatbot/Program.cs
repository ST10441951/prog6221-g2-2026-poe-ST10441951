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

            // --- TASK 4: BASIC RESPONSE SYSTEM ---

            // 1. Cybersecurity Topics (Required)
            knowledgeBase.Add(new ChatbotResponse("passwords", "Safe passwords should be at least 12 characters long and include symbols like @ or #. Never reuse the same password for different accounts!"));
            knowledgeBase.Add(new ChatbotResponse("phishing", "Phishing is a scam where attackers send fake emails to steal your banking PIN or login details. Never click suspicious links!"));
            knowledgeBase.Add(new ChatbotResponse("browsing", "For safe browsing, always look for the padlock icon (HTTPS) in the URL bar and avoid using public Wi-Fi for sensitive tasks like banking."));

            // 2. Personality & Purpose Questions (Required)
            knowledgeBase.Add(new ChatbotResponse("how are you", "I am functioning perfectly and ready to help you secure your digital life!"));
            knowledgeBase.Add(new ChatbotResponse("purpose", "My purpose is to educate South African citizens on identifying and mitigating cyber threats to prevent financial loss and identity theft."));
            knowledgeBase.Add(new ChatbotResponse("ask", "You can ask me about password safety, phishing, safe browsing, or even my purpose!"));

            // --- START THE PROGRAM ---

            // 1. Voice Greeting (Task 1)
            ui.PlayVoiceGreeting();

            // 2. ASCII Image (Task 2)
            ui.DisplayAsciiArt();

            // 3. User Interaction - Personalization (Task 3)
            ui.TypeMessage("Hello! Welcome to the Cybersecurity Awareness Bot.");
            Console.Write("What is your name? ");
            string userName = Console.ReadLine();

            // TASK 5: Basic Input Validation (Empty name)
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "Friend";
            }

            ui.TypeMessage("\nNice to meet you, " + userName + "!");
            ui.TypeMessage("I am here to help South Africans stay safe online.");

            bool keepRunning = true;
            while (keepRunning)
            {
                ui.ShowHeader("Main Menu");
                ui.TypeMessage("How can I help you? (Type: Passwords, Phishing, Browsing, Purpose, or Exit)");

                // TASK 6: Visual Indicator for User Input
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> ");
                string input = Console.ReadLine().ToLower().Trim();
                Console.ResetColor();

                if (input == "exit" || input == "quit")
                {
                    ui.TypeMessage("Stay safe online, " + userName + "! Goodbye.");
                    keepRunning = false;
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    // TASK 5: Handle Empty Input
                    Console.ForegroundColor = ConsoleColor.Red;
                    ui.TypeMessage("It looks like you didn't type anything. Please ask me a question!");
                    Console.ResetColor();
                }
                else
                {
                    // Search knowledge base
                    bool found = false;
                    foreach (var item in knowledgeBase)
                    {
                        // Using .Contains allows the bot to understand full sentences
                        if (input.Contains(item.Topic))
                        {
                            ui.ShowHeader("Cybersecurity Insight");
                            ui.TypeMessage(item.Information);
                            found = true;
                            break;
                        }
                    }

                    // TASK 5: Default Response for unknown/unsupported queries
                    if (!found)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ui.TypeMessage("I didn't quite understand that. Could you rephrase? Try asking about 'phishing' or 'my purpose'!");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}