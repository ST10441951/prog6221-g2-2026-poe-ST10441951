using System.Windows;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 4: Activity Log window.
    Provides a graphical, scrollable view of the same activity log the user can request in chat.
    It reads the entries straight from the shared ActivityLogger, so it always reflects exactly
    what the chatbot has recorded this session (tasks added/updated/completed, reminders set,
    quiz attempts, conversations, and other key actions).

    References:
    Microsoft (2023). ItemsControl Class (WPF). [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.itemscontrol
    */
    public partial class ActivityLogWindow : Window
    {
        private readonly ActivityLogger _log;

        public ActivityLogWindow(ActivityLogger log)
        {
            InitializeComponent();
            _log = log;
            LoadEntries();
        }

        /* Binds the current log entries to the list (chronological order). */
        private void LoadEntries()
        {
            var entries = _log.Entries;

            /* Reset the binding so a Refresh picks up any newly added entries. */
            EntriesList.ItemsSource = null;
            EntriesList.ItemsSource = entries;

            CountLabel.Text = entries.Count == 0 ? string.Empty : $"({entries.Count} actions)";
            EmptyState.Visibility = entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            /* Keep the newest action in view. */
            LogScroller.ScrollToEnd();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => LoadEntries();

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
