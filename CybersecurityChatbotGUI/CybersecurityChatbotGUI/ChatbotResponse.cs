using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /* I created this class to represent a single entry in the chatbot's knowledge base. 
    By using List<string> to hold the responses, I am explicitly demonstrating my ability to use a generic collection to solve a programming problem (a core learning outcome for this POE).
    This class is also the foundation for solving Task 3 (Random Responses).
    Microsoft (2023). List<T> Class. [Online] Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1
    */
    public class ChatbotResponse
    {
        /* I declared the Random instance as static. I learned that if you instantiate a new Random() object inside a method that gets called in rapid succession, it can use the same time-based seed and return the exact same number, defeating the purpose of Task 3!
        Skeet, J. (2010). Random number generator only generating one random number. [Online] Stack Overflow. Available at: https://stackoverflow.com/questions/767999/random-number-generator-only-generating-one-random-number
        */
        private static readonly Random _rng = new Random();

        /* The specific keyword that triggers this response group. Initialized to string.Empty to keep the compiler happy. */
        public string Topic { get; } = string.Empty;

        /* The generic collection that stores the pool of varied responses for this topic. */
        private readonly List<string> _responses;

        /* Constructor to initialize the topic and its list of responses when building the knowledge base. */
        public ChatbotResponse(string topic, List<string> responses)
        {
            Topic = topic;
            _responses = responses;
        }

        /* This method specifically handles Task 3 (Random Responses).
        It gets called every time a topic is matched, ensuring the user sees a different, randomly selected reply from the list instead of the exact same answer every time.
        */
        public string GetRandomResponse()
        {
            return _responses[_rng.Next(_responses.Count)];
        }
    }
}