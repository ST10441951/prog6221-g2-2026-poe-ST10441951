using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 1: "View and Manage Tasks".
    This window is the graphical task manager. It reads every task from MySQL and shows it as a
    card with its title, description, reminder and status. Each card has Complete and Delete
    buttons whose changes are written straight back to the database, and the panel at the top
    lets the user add a new task (title, description and an optional reminder) from the GUI.

    The window shares the SAME TaskRepository and ActivityLogger instances as the chat window
    (passed in via the constructor), so tasks added in chat appear here and vice-versa, and
    every action is recorded in the Task 4 activity log.

    References:
    Microsoft (2023). Window Class (WPF). [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.window
    */
    public partial class TaskManagerWindow : Window
    {
        private readonly TaskRepository _repository;
        private readonly ActivityLogger _activityLog;

        public TaskManagerWindow(TaskRepository repository, ActivityLogger activityLog)
        {
            InitializeComponent();
            _repository = repository;
            _activityLog = activityLog;
            LoadTasks();
        }

        /* Pulls all tasks from the database and binds them to the list. Re-called after every
        add / complete / delete so the GUI always reflects the current database state. */
        private void LoadTasks()
        {
            try
            {
                List<TaskItem> tasks = _repository.GetAllTasks();
                TasksList.ItemsSource = tasks;
                CountLabel.Text = tasks.Count == 0 ? string.Empty : $"({tasks.Count})";
                EmptyState.Visibility = tasks.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
                ErrorBanner.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                TasksList.ItemsSource = null;
                EmptyState.Visibility = Visibility.Collapsed;
                ShowError("Could not reach the task database. Please make sure MySQL is running. " +
                          "Details: " + ex.Message);
            }
        }

        /* Adds a task entered through the GUI panel (title required; description and reminder
        optional). The reminder text is parsed by the shared ReminderParser. */
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleInput.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                ShowError("Please enter a task title before adding.");
                return;
            }

            DateTime? reminder = null;
            string reminderText = ReminderInput.Text.Trim();
            if (!string.IsNullOrEmpty(reminderText)
                && ReminderParser.TryParse(reminderText, out DateTime when, out _))
            {
                reminder = when;
            }

            try
            {
                var task = new TaskItem
                {
                    Title = title,
                    Description = string.IsNullOrEmpty(DescInput.Text.Trim())
                        ? "Cybersecurity task."
                        : DescInput.Text.Trim(),
                    ReminderDate = reminder,
                    CreatedAt = DateTime.Now
                };

                _repository.AddTask(task);
                _activityLog.Log(ActivityCategory.Task, $"Added task via Task Manager: '{title}'.");

                TitleInput.Clear();
                DescInput.Clear();
                ReminderInput.Clear();
                LoadTasks();
            }
            catch (Exception ex)
            {
                ShowError("Could not save the task: " + ex.Message);
            }
        }

        /* Marks the chosen task complete in the database, then refreshes the list. */
        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                try
                {
                    _repository.MarkComplete(id);
                    _activityLog.Log(ActivityCategory.Task,
                        $"Marked task #{id} complete via Task Manager.");
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    ShowError("Could not update the task: " + ex.Message);
                }
            }
        }

        /* Deletes the chosen task from the database, then refreshes the list. */
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                try
                {
                    _repository.DeleteTask(id);
                    _activityLog.Log(ActivityCategory.Task,
                        $"Deleted task #{id} via Task Manager.");
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    ShowError("Could not delete the task: " + ex.Message);
                }
            }
        }

        /* Shows the red error banner with a message (used when the database is unreachable). */
        private void ShowError(string message)
        {
            ErrorText.Text = "⚠ " + message;
            ErrorBanner.Visibility = Visibility.Visible;
        }
    }
}
