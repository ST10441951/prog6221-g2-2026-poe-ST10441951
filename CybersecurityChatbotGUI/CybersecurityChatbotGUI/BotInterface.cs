using System;
using System.IO;
using System.Media;
using System.Runtime.Versioning;

namespace CybersecurityChatbotGUI
{
    /* I created this class to handle all the supplementary interface helpers for the GUI.
    It manages the voice greeting (Task 1), the high-fidelity ASCII art (Task 1), and the personalised bridge phrases (Task 5).
    By keeping this separate from the ChatbotEngine, I am demonstrating a strong separation of concerns, which is a key principle of Object-Oriented Programming (Task 8).
    
    References:
    Martin, R.C. (2018). Clean Architecture: A Craftsman's Guide to Software Structure and Design. Boston: Prentice Hall.
    */
    public class BotInterface
    {
        /* I declared the Random instance at the class level so it is only seeded once.
        If I created a new Random() object every time the method was called, it could generate the same number repeatedly due to how the system clock seeds it.
        
        References:
        Skeet, J. (2010). Random number generator only generating one random number. [Online] Stack Overflow. Available at: https://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
        */
        private static readonly Random _rng = new Random();

        /* This method satisfies Task 1 by porting the voice greeting from Part 1 into the GUI.
        I wrapped the whole thing in a try/catch block so the application never crashes if the audio file goes missing, which perfectly handles the edge cases for Task 7.
        
        References:
        Microsoft (2023). SoundPlayer Class. [Online] Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer
        */
        [SupportedOSPlatform("windows")]
        public void PlayVoiceGreeting()
        {
            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "greeting.wav");

                if (File.Exists(audioPath))
                {
                    SoundPlayer player = new SoundPlayer(audioPath);
                    /* Calling Play() instead of PlaySync() so it runs asynchronously and does not freeze the UI thread. */
                    player.Play();
                }
            }
            catch
            {
                /* Graceful edge-case handling: a missing or corrupt audio file is silently ignored so the app continues normally (Task 7). */
            }
        }

        /* This method returns the alternative boxy desktop monitor ASCII banner as a single block string.
        It satisfies Task 1 by translating the Part 1 ASCII art requirements into a clean, proportional GUI layout.
        */
        public string GetAsciiArt()
        {
            return
                "═════════════════════════════════════════\n" +
                "             _______________             \n" +
                "            |  ___________  |            \n" +
                "            | |  O     O  | |            \n" +
                "            | |     v     | |            \n" +
                "            | |___________| |            \n" +
                "            |_______________|            \n" +
                "                   |||                   \n" +
                "               ____|||____               \n" +
                "              (___________)              \n" +
                "           [ CYBERSECURITY CHATBOT ]     \n" +
                "═════════════════════════════════════════\n";
        }

        /* Task 1 and Task 8: Advanced Sequence Animator Array.
        Slices the stylized desktop monitor layout into individual string layers.
        This allows the WPF code-behind loop to pull lines sequentially and simulate a live terminal loading animation inside a single text container block.
        
        References:
        Microsoft (2023). Arrays (C# Programming Guide). [Online] Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/arrays/
        */
        public string[] GetAsciiArtLines()
        {
            return new string[]
            {
                "═════════════════════════════════════════",
                "             _______________             ",
                "            |  ___________  |            ",
                "            | |  O     O  | |            ",
                "            | |     v     | |            ",
                "            | |___________| |            ",
                "            |_______________|            ",
                "                   |||                   ",
                "               ____|||____               ",
                "              (___________)              ",
                "           [ CYBERSECURITY CHATBOT ]     ",
                "═════════════════════════════════════════",
                ""
            };
        }

        /* This is a pool of bridge phrases used to personalise the bot's responses for Task 5. */
        private readonly string[] _bridges =
        {
            "That is a very important question, ",
            "I am glad you are staying curious about that, ",
            "It is vital to understand this: ",
            "Great point! Here is what you should know, ",
            "Excellent question, let me break that down for you, ",
            "You are asking all the right things, "
        };

        /* This method returns a randomly selected personalised bridge phrase and tacks the user's name onto the end.
        I used this to vary the conversational transitions (Task 4) and reinforce the memory-based personalisation (Task 5).
        
        References:
        Microsoft (2023). String.IsNullOrEmpty Method. [Online] Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.string.isnullorempty
        */
        public string GetBridge(string name)
        {
            string bridge = _bridges[_rng.Next(_bridges.Length)];
            return string.IsNullOrEmpty(name) ? bridge : bridge + name + ". ";
        }
    }
}