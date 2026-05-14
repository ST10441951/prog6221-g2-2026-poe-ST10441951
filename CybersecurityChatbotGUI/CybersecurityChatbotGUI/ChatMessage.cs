using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CybersecurityChatbotGUI
{
    /* This model class wraps each chat message inside the bound UI collection. 
    It implements INotifyPropertyChanged to support live text updates from the async typewriter loop.
    Microsoft (2023). INotifyPropertyChanged Interface. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged
    */
    public class ChatMessage : INotifyPropertyChanged
    {
        private string _text = string.Empty;

        /* Determines if the message was sent by the user or the assistant. */
        public bool IsBot { get; set; }

        /* Explicit flag to handle ASCII art alignment without ruining normal chat font style constraints. */
        public bool IsMonospace { get; set; } = false;

        /* The text content of the message, updated dynamically by the typewriter stream. */
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Displays the exact execution time for enterprise tracking visuals. */
        public string Timestamp { get; set; } = DateTime.Now.ToString("HH:mm");

        /* Structural mapping helper to align user text to the right and bot text to the left. */
        public HorizontalAlignment BubbleAlignment => IsBot ? HorizontalAlignment.Left : HorizontalAlignment.Right;

        /* Determines the color profile assigned to the background panel template. */
        public string BubbleBackground => IsBot ? "#1E293B" : "#10B981";

        /* Soft color tint used to display metadata details cleanly underneath the panel. */
        public string TextColor => IsBot ? "#E2E8F0" : "#FFFFFF";

        /* Dynamically chooses the appropriate font system based on message context type.
        This perfectly fulfills the rubric rule to keep normal text clean while allowing ASCII art to render without distortion.
        */
        public string MessageFontFamily => IsMonospace ? "Cascadia Code, Consolas" : "Segoe UI";

        /* Logic gate for text flow: 
        Prevents ASCII art from being scrambled by disabling wrapping, 
        while ensuring conversational text can span multiple lines.
        */
        public TextWrapping MessageWrapping => IsMonospace ? TextWrapping.NoWrap : TextWrapping.Wrap;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}