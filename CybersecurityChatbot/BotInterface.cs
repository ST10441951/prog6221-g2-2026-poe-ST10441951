using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Runtime.Versioning;

namespace CybersecurityChatbot
{
    public class BotInterface
    {
        /*
         Face (2022). How to add a Wav file to Windows Form Application in Visual Studio. 
         [Online] Stack Overflow. Available at: https://stackoverflow.com/questions/71707808/how-to-add-a-wav-file-to-windows-form-application-in-visual-studio.‌
         */

        // Suppress platform warning for Windows-only SoundPlayer
        [SupportedOSPlatform("windows")]
        public void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");
                if (File.Exists(audioPath))
                {
                    SoundPlayer player = new SoundPlayer(audioPath);
                    player.Play();
                }
                else
                {
                    Console.WriteLine("[System: Audio file missing. Proceeding with text only.]");
                }
            }
            catch (Exception ex)
            {
                // Task 5: Graceful error handling
                Console.WriteLine($"[Audio System Notice: {ex.Message}]");
            }
        }

        public void DisplayAsciiArt()
        {
            /*
             ASCII Art Archive. (2019). ASCII Art Archive. 
             [Online] Available at: https://www.asciiart.eu/.‌
             */

            Console.Clear(); // Clear console for a professional launch
            string border = new string('=', 50);
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(border);
            Console.WriteLine(@"                _________________");
            Console.WriteLine(@"               |  ___________  | ");
            Console.WriteLine(@"               | |  ^     ^  | | ");
            Console.WriteLine(@"               | |     -     | | ");
            Console.WriteLine(@"               | |___________| | ");
            Console.WriteLine(@"               |_______________| ");
            Console.WriteLine(@"               _____|_____|_____ ");
            Console.WriteLine(@"              (_________________)");
            Console.WriteLine("\n          [ CYBER-AWARE ASSISTANT ]");
            Console.WriteLine(border);
            Console.ResetColor();
            Console.WriteLine();
        }

        // TASK 6: Enhanced Typing Effect for Immersive UX

        /*
         thim24 (2020). Updating a label using a for statement (typewriter effect). 
         [Online] Stack Overflow. Available at: https://stackoverflow.com/questions/62597550/updating-a-label-using-a-for-statement-typewriter-effect.
         */

        public void TypeMessage(string message, int speed = 25)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                // Pause longer at sentence ends for natural flow
                if (c == '.' || c == '?' || c == '!') Thread.Sleep(speed * 10);
                else Thread.Sleep(speed);
            }
            Console.WriteLine();
        }

        public void ShowHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(new string('-', 10) + " " + title.ToUpper() + " " + new string('-', 10));
            Console.ResetColor();
        }

        /*
         Refactoring.guru. (2025). Bridge in C# / Design Patterns. 
         [Online] Available at: https://refactoring.guru/design-patterns/bridge/csharp/example [Accessed 31 Mar. 2026].
         */

        private readonly string[] _bridges = {
            "That is a very important question, ",
            "I'm glad you're staying curious about that, ",
            "It is vital to understand this: ",
            "Great point! Here is what you should know, "
        };

        public void TypeBridge(string name)
        {
            Random rng = new Random();
            // Picks a random phrase and attaches the user's name for personalization
            string phrase = _bridges[rng.Next(_bridges.Length)] + name + ".";
            Console.ForegroundColor = ConsoleColor.Cyan;
            TypeMessage(phrase, 20);
            Console.ResetColor();
        }
    }
}