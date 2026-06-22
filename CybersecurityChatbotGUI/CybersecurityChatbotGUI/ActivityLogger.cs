using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 4: Activity Log Feature.
    These categories let me tag every logged action so the log reads clearly and could be
    filtered later. Using an enum (instead of loose strings) prevents typos and documents
    exactly which kinds of actions the bot records.
    */
    public enum ActivityCategory
    {
        Session,
        Task,
        Reminder,
        Quiz,
        Conversation,
        System
    }

    /* A single immutable entry in the activity log. Each one captures WHEN something happened
    (timestamp) and WHAT happened (category + description), which is exactly what Task 4 asks
    for. Immutability (get-only properties set once in the constructor) means a logged action
    can never be silently altered afterwards.
    */
    public class ActivityEntry
    {
        public DateTime Timestamp { get; }
        public ActivityCategory Category { get; }
        public string Description { get; }

        public ActivityEntry(ActivityCategory category, string description)
        {
            Timestamp = DateTime.Now;
            Category = category;
            Description = description;
        }

        /* ---- Display helpers used by the Activity Log GUI window ---- */

        /* Full time-of-day for the log window (the chat summary uses HH:mm). */
        public string TimeDisplay => Timestamp.ToString("HH:mm:ss");

        /* A small icon per category so the log is easy to scan at a glance. */
        public string Icon => Category switch
        {
            ActivityCategory.Session => "🟢",
            ActivityCategory.Task => "📋",
            ActivityCategory.Reminder => "⏰",
            ActivityCategory.Quiz => "🎮",
            ActivityCategory.Conversation => "💬",
            ActivityCategory.System => "⚙",
            _ => "•"
        };

        /* A colour per category for the category label in the log window. */
        public string CategoryColor => Category switch
        {
            ActivityCategory.Session => "#34D399",
            ActivityCategory.Task => "#10B981",
            ActivityCategory.Reminder => "#FBBF24",
            ActivityCategory.Quiz => "#60A5FA",
            ActivityCategory.Conversation => "#A78BFA",
            ActivityCategory.System => "#94A3B8",
            _ => "#94A3B8"
        };

        /* Human-readable single-line form, e.g. "[14:32] (Task) Added task: 'Enable 2FA'." */
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm}] ({Category}) {Description}";
        }
    }

    /* Part 3 / Task 4: The activity log itself.
    The brief explicitly allows a list OR dictionary to store the actions; I chose a
    List<ActivityEntry> because the order actions happened in is meaningful (a timeline).
    The chatbot records every significant action here (tasks, reminders, quiz attempts, and
    other key events) and the user can ask to see it via the ShowActivityLog intent.

    References:
    Microsoft (2023). List<T> Class. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1
    */
    public class ActivityLogger
    {
        private readonly List<ActivityEntry> _entries = new List<ActivityEntry>();

        /* Records one action. Empty/whitespace descriptions are ignored so the log stays clean.
        This is the single method every other feature calls to write to the log. */
        public void Log(ActivityCategory category, string description)
        {
            if (string.IsNullOrWhiteSpace(description)) return;
            _entries.Add(new ActivityEntry(category, description));
        }

        /* Read-only view so other classes can inspect the log but cannot tamper with the
        underlying list directly (encapsulation). */
        public IReadOnlyList<ActivityEntry> Entries => _entries.AsReadOnly();

        /* How many actions have been recorded this session. */
        public int Count => _entries.Count;

        /* Builds the numbered summary the chatbot shows when the user asks
        "what have you done for me?" / "show activity log". Shows the most recent actions
        (newest last) so the list mirrors the conversation timeline, matching the brief's
        example output style.

        By default it shows only the most recent 'maxItems' (5) actions to keep the log
        readable; pass maxItems <= 0 to show the FULL log. When the list is truncated it tells
        the user how to see the rest, which is the "shows a few at a time, allows show more"
        behaviour Task 4 asks for.
        */
        public string GetFormattedLog(int maxItems = 5)
        {
            if (_entries.Count == 0)
            {
                return "I have not recorded any actions yet this session. " +
                       "Try adding a task, setting a reminder, or playing the quiz, " +
                       "then ask me again!";
            }

            bool truncated = maxItems > 0 && _entries.Count > maxItems;

            /* Take the last 'maxItems' entries (or all of them when not truncated). */
            List<ActivityEntry> shown = truncated
                ? _entries.Skip(_entries.Count - maxItems).ToList()
                : _entries.ToList();

            var builder = new StringBuilder();
            if (truncated)
            {
                builder.AppendLine(
                    $"Here are your {shown.Count} most recent actions (of {_entries.Count} total). " +
                    "Say \"show full log\" or open the 📜 Activity Log to see them all:");
            }
            else
            {
                builder.AppendLine($"Here is a summary of recent actions ({shown.Count} shown):");
            }

            int number = 1;
            foreach (ActivityEntry entry in shown)
            {
                builder.AppendLine($"{number}. [{entry.Timestamp:HH:mm}] {entry.Description}");
                number++;
            }

            return builder.ToString().TrimEnd();
        }
    }
}
