using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CybersecurityChatbotGUI
{
    /* Code-behind driver managing simple message visualization streams.
    Refactored to eliminate obsolete paragraph inline additions by migrating message feeds into clean observable objects.
    All background audio and name extraction routines remain integrated to meet evaluation criteria.
    
    References:
    Microsoft (2023). WPF architecture overview. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/wpf-architecture-performance
    */
    public partial class MainWindow : Window
    {
        private readonly ChatbotEngine _engine;
        private readonly BotInterface _botInterface;
        private bool _isTypingAnimationActive = false;

        /* Central database stream binding directly to the chat items control interface window layout container.
        
        References:
        Microsoft (2023). ObservableCollection(T) Class. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.objectmodel.observablecollection-1
        */
        public ObservableCollection<ChatMessage> MessageHistory { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _engine = new ChatbotEngine();
            _botInterface = new BotInterface();
            MessageHistory = new ObservableCollection<ChatMessage>();

            /* Connects the backing collection resource to the data-bound container control system */
            ChatMessagesControl.ItemsSource = MessageHistory;
        }

        /* Main runtime entry initialization sequence.
        Triggers vocal wav greetings and appends structural greeting data logs.
        
        References:
        Microsoft (2023). Window.Loaded Event. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.window.loaded
        */
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlayVoiceGreeting();

            /* 1. Append the ASCII Art layout using the monospace text rendering flag to prevent structural distortion */
            var asciiArtNode = new ChatMessage
            {
                IsBot = true,
                IsMonospace = true,
                Text = _botInterface.GetAsciiArt()
            };
            MessageHistory.Add(asciiArtNode);

            /* 2. Append the system welcome message guidelines using regular design fonts */
            var initialWelcomeNode = new ChatMessage
            {
                IsBot = true,
                IsMonospace = false,
                Text = _engine.GetWelcomeMessage()
            };
            MessageHistory.Add(initialWelcomeNode);

            /* Automatically shifts visual position layout context indexes to match standard view boundaries */
            ChatScroller.ScrollToEnd();
            await Task.CompletedTask;
        }

        /* Voice greeting feature ported from Part 1 (Task 1).
        Utilizes an explicit operating system guard to clear modern platform compilation errors gracefully.
        
        References:
        Microsoft (2023). SoundPlayer Class. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.media.soundplayer
        Microsoft (2023). OperatingSystem.IsWindows Method. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.operatingsystem.iswindows
        */
        private void PlayVoiceGreeting()
        {
            /* Explicit guard layer to satisfy the platform checker at the compilation level */
            if (!OperatingSystem.IsWindows()) return;

            try
            {
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "greeting.wav");

                if (File.Exists(audioPath))
                {
                    SoundPlayer player = new SoundPlayer(audioPath);
                    player.Play(); /* Dispatched to a separate worker thread execution loop to keep layout tasks responsive */
                }
            }
            catch
            {
                /* Graceful edge case safety guard: prevents execution blocks from interrupting runtime cycles if hardware resources fail */
            }
        }

        /* Core GUI interaction loop. Handles the input processing pipeline, session states, and message distribution routines.
        
        References:
        Troelsen, A. and Japikse, P. (2021). Pro C# 9 with .NET 5: Foundational Principles and Practices. 10th ed. New York: Apress.
        */
        private async Task HandleUserSubmissionAsync()
        {
            if (_isTypingAnimationActive) return;

            string inputTextContent = UserInputBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(inputTextContent)) return;

            UserInputBox.Clear();

            /* Log user message item node inside our collection array layer */
            MessageHistory.Add(new ChatMessage { IsBot = false, Text = inputTextContent });
            ChatScroller.ScrollToEnd();

            /* Step 1: Initial user handshake name acquisition check sequence loop */
            if (!_engine.Session.IsGreetingComplete)
            {
                _engine.Session.UserName = FormatName(inputTextContent);
                _engine.Session.IsGreetingComplete = true;

                string personalizedIntroString = $"Welcome, {_engine.Session.UserName}! Great to meet you. " +
                    "I will remember your name throughout our conversation. " +
                    "You can ask me about passwords, phishing, privacy, scams, malware, " +
                    "ransomware, firewalls, 2FA, VPN, updates, or social engineering. " +
                    "What would you like to learn about first?";

                await StreamTypewriterBotOutputAsync(personalizedIntroString);
                return;
            }

            /* Step 2: Route core questions to backend processing blocks */
            string internalEngineResultResponse = _engine.ProcessUserInput(inputTextContent);
            await StreamTypewriterBotOutputAsync(internalEngineResultResponse);
        }

        /* Routes the mouse click event context into the submission pipeline logic.
        
        References:
        Microsoft (2023). ButtonBase.OnClick Method. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.primitives.buttonbase.onclick
        */
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await HandleUserSubmissionAsync();
        }

        /* Captures physical physical keyboard inputs to detect and handle enter submission events.
        
        References:
        Microsoft (2023). UIElement.KeyDown Event. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.uielement.keydown
        */
        private async void UserInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await HandleUserSubmissionAsync();
        }

        /* Sanitizes, splices, and tracks casing parameters assigned to user registration sequences */
        private string FormatName(string input)
        {
            string trimmed = input.Trim();
            if (string.IsNullOrEmpty(trimmed)) return "User";

            string[] parts = trimmed.Split(' ');
            string firstName = parts[parts.Length - 1];

            return string.IsNullOrEmpty(firstName)
                ? "User"
                : char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();
        }

        /* Advanced punctuation aware typewriter streaming implementation logic.
        Appends single characters into the notification model tracking targets while tracking timing changes to emulate real conversational flows.
        
        References:
        Microsoft (2023). Asynchronous programming with async and await. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
        Task.Delay Method. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.delay
        */
        private async Task StreamTypewriterBotOutputAsync(string completeMessageText)
        {
            _isTypingAnimationActive = true;
            TypingIndicatorRow.Visibility = Visibility.Visible;

            /* Construct container target entry context inside live list array */
            var streamingNodeItem = new ChatMessage { IsBot = true, IsMonospace = false, Text = string.Empty };
            MessageHistory.Add(streamingNodeItem);

            foreach (char characterToken in completeMessageText)
            {
                streamingNodeItem.Text += characterToken;
                ChatScroller.ScrollToEnd();

                /* Preserves exact rhythmic pausing system delays assigned to punctuation anchors */
                int durationDelayMilliseconds = (characterToken == '.' || characterToken == '?' || characterToken == '!') ? 220
                                              : (characterToken == ',') ? 80
                                              : 16;

                await Task.Delay(durationDelayMilliseconds);
            }

            TypingIndicatorRow.Visibility = Visibility.Collapsed;
            _isTypingAnimationActive = false;
        }

        /* Automated parsing adapter routing click events fired by quick navigation chips or cards directly into processing channels.
        Cleans contextual labels to isolate relevant search keywords before driving the input box.
        
        References:
        Microsoft (2023). RoutedEventArgs Class. [Online] Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.routedeventargs
        */
        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button sourceButtonClicked && sourceButtonClicked.Content != null)
            {
                string textExtractedLine = sourceButtonClicked.Content.ToString() ?? string.Empty;

                /* Drops emojis and spacing tokens from leading index boundaries if present */
                if (textExtractedLine.Contains("   "))
                {
                    string[] splitSegments = textExtractedLine.Split(new[] { "   " }, StringSplitOptions.None);
                    if (splitSegments.Length > 1) textExtractedLine = splitSegments[1];
                }
                else if (textExtractedLine.Length > 4 && (textExtractedLine.Contains("  ")))
                {
                    string[] splitSegments = textExtractedLine.Split(new[] { "  " }, StringSplitOptions.None);
                    if (splitSegments.Length > 1) textExtractedLine = splitSegments[1];
                }

                UserInputBox.Text = textExtractedLine;
                SendButton_Click(sender, e);
            }
        }
    }
}