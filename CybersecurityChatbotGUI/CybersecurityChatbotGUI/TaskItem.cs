using System;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 1: Represents a single cybersecurity task stored in the MySQL database.
    The brief requires every task to have a title, a description and an optional reminder, so
    those are the core fields; Id, IsCompleted and CreatedAt support storing and managing the
    task in the database and the GUI.

    References:
    Microsoft (2023). Auto-Implemented Properties (C# Programming Guide). [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties
    */
    public class TaskItem
    {
        /* Primary key, assigned by MySQL (AUTO_INCREMENT) when the task is inserted. */
        public int Id { get; set; }

        /* Short name of the task, e.g. "Review privacy settings". */
        public string Title { get; set; } = string.Empty;

        /* Longer, friendly explanation of what the task involves. */
        public string Description { get; set; } = string.Empty;

        /* Optional reminder. Null means "no reminder set" (the brief makes reminders optional). */
        public DateTime? ReminderDate { get; set; }

        /* Whether the user has marked this task as done. */
        public bool IsCompleted { get; set; }

        /* When the task was created, used to order the list newest-first. */
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /* ---- Display helpers (used directly by the Task Manager GUI data bindings) ---- */

        /* Friendly reminder text for the task card, e.g. "⏰ Wed 25 Jun 2026". */
        public string ReminderDisplay => ReminderDate.HasValue
            ? $"⏰ {ReminderDate.Value:ddd dd MMM yyyy}"
            : "No reminder set";

        /* Friendly status badge for the task card. */
        public string StatusDisplay => IsCompleted ? "✓ Completed" : "○ Pending";

        /* Lets the GUI disable the "Complete" button once a task is already done. */
        public bool CanComplete => !IsCompleted;
    }
}
