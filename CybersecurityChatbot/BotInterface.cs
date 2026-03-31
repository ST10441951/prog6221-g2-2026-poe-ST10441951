using System;
using System.Collections.Generic;
using System.Text;

namespace CybersecurityChatbot
{
    public class BotInterface
    {
        // TASK 1: Voice Greeting
        public void PlayVoiceGreeting()
        {
            try
            {
                // PLACEHOLDER: Ensure your file is named 'greeting.wav' and is in the output folder
                // SoundPlayer player = new SoundPlayer("greeting.wav");
                // player.PlaySync(); 
                Console.WriteLine("[System: Voice greeting playing...]");
            }
            catch (Exception)
            {
                Console.WriteLine("Audio file not found. Skipping voice greeting.");
            }
        }

        // TASK 2: ASCII Image Display
        public void DisplayAsciiArt()
        {
            // TASK 6: Enhanced UI - Dynamic Border
            string topBorder = new string('*', 40);
            string bottomBorder = new string('*', 40);

            // CHANGE COLOUR
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(topBorder);

            
            Console.WriteLine("      [ CYBER-AWARE SA BOT ]            ");
            Console.WriteLine("          _      _                      ");
            Console.WriteLine("         ( )____( )                     ");
            Console.WriteLine("          |      |                      ");
            Console.WriteLine("          |______|                      ");

            Console.WriteLine(bottomBorder);

            
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