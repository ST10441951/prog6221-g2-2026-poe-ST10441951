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

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}