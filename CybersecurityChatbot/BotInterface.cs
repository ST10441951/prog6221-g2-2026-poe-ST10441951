using System;
using System.Collections.Generic;
using System.Text;
using System.Media;

namespace CybersecurityChatbot
{
    public class BotInterface
    {
        // TASK 1: Voice Greeting
        public void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                SoundPlayer player = new SoundPlayer(audioPath);
                player.Play();
                Console.WriteLine("[System: Voice greeting playing...]");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error: " + ex.Message);
            }
        }

        // TASK 2: ASCII Image Display
        public void DisplayAsciiArt()
        {
            // TASK 6: Enhanced UI - Create a dynamic border
            string border = new string('=', 50);

            // TASK 6: Colour Formatting - Cyan/LightBlue for a "Tech" look
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(border);

            // TASK 2: Friendly Computer Face Logo
            // Using @ (Verbatim String) to handle the backslashes correctly
            Console.WriteLine(@"                _________________");
            Console.WriteLine(@"               |  ___________  | ");
            Console.WriteLine(@"               | |  ^     ^  | | ");
            Console.WriteLine(@"               | |     -     | | ");
            Console.WriteLine(@"               | |___________| | ");
            Console.WriteLine(@"               |_______________| ");
            Console.WriteLine(@"               _____|_____|_____ ");
            Console.WriteLine(@"              (_________________)");
            Console.WriteLine(@"                                 ");
            Console.WriteLine(@"          [ CYBER-AWARE ASSISTANT ]");

            Console.WriteLine(border);

            // Always reset color so the user's input isn't Cyan
            Console.ResetColor();
            Console.WriteLine();
        }

        // TASK 6: Typing Effect
        public void TypeMessage(string message)
        {
            foreach (char c in message)
            {
                Console.Write(c);
                
                Thread.Sleep(30);
            }
            Console.WriteLine();
        }

        // TASK 6: Decorative Header
        public void ShowHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.WriteLine("--- " + title.ToUpper() + " ---");
            Console.ResetColor();
        }
    }
}