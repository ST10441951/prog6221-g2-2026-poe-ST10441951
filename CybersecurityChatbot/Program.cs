using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            BotInterface ui = new BotInterface();
            List<ChatbotResponse> knowledgeBase = new List<ChatbotResponse>();

            // TASK 4: Knowledge Base with SA Context
            knowledgeBase.Add(new ChatbotResponse("passwords", "Use 12+ characters with symbols. In SA, never share your banking OTP over the phone!"));
            knowledgeBase.Add(new ChatbotResponse("phishing", "Fake emails often mimic SARS or local banks. Check the sender's address carefully."));
            knowledgeBase.Add(new ChatbotResponse("browsing", "Ensure 'https' is present. Avoid public Wi-Fi for FNB, Capitec, or Standard Bank apps."));
            knowledgeBase.Add(new ChatbotResponse("how are you", "I am optimized and ready to protect South African citizens!"));
            knowledgeBase.Add(new ChatbotResponse("purpose", "To increase cybersecurity literacy and prevent identity theft in our local communities."));
            knowledgeBase.Add(new ChatbotResponse("saps", "Report cybercrimes to your nearest SAPS station or the Cybersecurity Hub at cybersecurityhub.gov.za."));

            ui.DisplayAsciiArt();
            ui.PlayVoiceGreeting();

            // TASK 5: Robust Name Validation
            string userName = "";
            while (string.IsNullOrWhiteSpace(userName))
            {
                ui.TypeMessage("System Online. I am your SA Cyber-Shield Assistant, dedicated to protecting our citizens from digital threats.");
                ui.TypeMessage("Before we secure your connection, may I ask who I am speaking with?");
                Console.Write("Name: ");
                userName = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(userName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Name cannot be empty. Please identify yourself.");
                    Console.ResetColor();
                }
            }

            ui.TypeMessage($"\nWelcome, {userName}. I am your Cybersecurity Awareness Assistant.");

            bool keepRunning = true;
            while (keepRunning)
            {
                ui.ShowHeader("Main Menu");
                ui.TypeMessage("Ask about: Passwords, Phishing, Browsing, SAPS, or type 'Help'. (Type 'Exit' to quit)");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{userName} > ");
                string input = Console.ReadLine()?.ToLower().Trim();
                Console.ResetColor();

                if (input == "exit" || input == "quit")
                {
                    ui.TypeMessage($"Goodbye {userName}. Stay vigilant and stay safe online.");
                    keepRunning = false;
                }
                else if (input == "help")
                {
                    ui.ShowHeader("Assistant Help");
                    ui.TypeMessage("I can recognize keywords. Try asking: 'Tell me about phishing' or 'What is the SAPS contact?'");
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    ui.TypeMessage("Input detected as null. Please provide a query.");
                    Console.ResetColor();
                }
                else
                {
                    bool found = false;

                    /*
                     www.w3schools.com. (2026). C# Foreach Loop. 
                     [Online] Available at: https://www.w3schools.com/cs/cs_foreach_loop.php.
                     */

                    foreach (var item in knowledgeBase)
                    {
                        if (input.Contains(item.Topic))
                        {
                            ui.ShowHeader("Cybersecurity Insight");

                            
                            ui.TypeBridge(userName);

                            ui.TypeMessage(item.Information);
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        ui.TypeMessage("I am sorry, my database does not contain that specific information. Try asking about 'passwords' or 'phishing'.");
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}