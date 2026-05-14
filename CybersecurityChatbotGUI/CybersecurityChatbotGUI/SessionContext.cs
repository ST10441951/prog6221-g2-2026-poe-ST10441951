namespace CybersecurityChatbotGUI
{
    /* I created this SessionContext class to handle Task 5 (Memory and Recall) and Task 4 (Conversation Flow).
    Instead of using loose variables, grouping them in a dedicated class demonstrates better Object-Oriented Programming (OOP) structure for my POE.
    
    References:
    Microsoft (2023). Auto-Implemented Properties. [Online] Microsoft Learn. Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties
    */
    public class SessionContext
    {
        /* This stores the user's name captured at the start of the app. 
        It is used to personalize the bot's responses throughout the session.
        */
        public string UserName { get; set; } = string.Empty;

        /* This saves the first cybersecurity topic the user asks about.
        The bot checks this variable later to personalize responses based on their interests, perfectly fulfilling the Task 5 requirement.
        */
        public string FavoriteTopic { get; set; } = string.Empty;

        /* A logic gate used to ensure the bot only acknowledges the user's interest once, 
        preventing repetitive responses during long conversations.
        */
        public bool HasAcknowledgedInterest { get; set; } = false;

        /* I use this to track the most recently discussed topic. 
        If the user types a follow-up phrase like "tell me more", the bot checks this variable so it knows 
        which topic to keep expanding on without resetting (Task 4).
        */
        public string LastDiscussedTopic { get; set; } = string.Empty;

        /* A simple boolean flag so the bot knows the initial name-capture phase is done. 
        This stops the bot from asking for the user's name every time they send a message.
        */
        public bool IsGreetingComplete { get; set; } = false;
    }
}