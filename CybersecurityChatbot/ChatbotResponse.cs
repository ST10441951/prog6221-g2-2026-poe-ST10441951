using System;
using System.Collections.Generic;
using System.Text;

namespace CybersecurityChatbot
{
    // This class holds the data for specific cybersecurity topics
    public class ChatbotResponse
    {
        /*
         www.w3schools.com. (2026). C# Properties (Get and Set). 
         [Online] Available at: https://www.w3schools.com/cs/cs_properties.php.
         */

        // Automatic Properties
        public string Topic { get; set; }
        public string Information { get; set; }

        // Constructor to initialize the topic
        public ChatbotResponse(string topic, string information)
        {
            this.Topic = topic;
            this.Information = information;
        }
    }
}
