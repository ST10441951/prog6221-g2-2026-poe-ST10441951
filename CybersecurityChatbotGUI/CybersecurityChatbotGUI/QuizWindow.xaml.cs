using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 2: the Cybersecurity Mini-Game window.
    It shows ONE question at a time, gives immediate right/wrong feedback with an explanation,
    keeps a running score, and ends with a final score and a score-based message. All the game
    rules live in QuizGame; this window is purely the presentation and user interaction.

    It shares the chatbot's ActivityLogger so that starting and finishing the quiz are recorded
    in the Task 4 activity log ("quiz attempts").

    References:
    Microsoft (2023). Window Class (WPF). [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.windows.window
    Microsoft (2023). Creating controls dynamically. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/
    */
    public partial class QuizWindow : Window
    {
        private readonly ActivityLogger _activityLog;
        private QuizGame _game;

        /* Colours reused for option highlighting and feedback. */
        private static readonly SolidColorBrush Green = new SolidColorBrush(Color.FromRgb(0x10, 0xB9, 0x81));
        private static readonly SolidColorBrush Red = new SolidColorBrush(Color.FromRgb(0xEF, 0x44, 0x44));
        private static readonly SolidColorBrush GreenDim = new SolidColorBrush(Color.FromRgb(0x06, 0x4E, 0x3B));
        private static readonly SolidColorBrush RedDim = new SolidColorBrush(Color.FromRgb(0x7F, 0x1D, 0x1D));

        public QuizWindow(ActivityLogger activityLog)
        {
            InitializeComponent();
            _activityLog = activityLog;
            _game = new QuizGame();
            _activityLog.Log(ActivityCategory.Quiz, "Started the cybersecurity quiz.");
            ShowQuestion();
        }

        /* Renders the current question and a fresh set of option buttons. */
        private void ShowQuestion()
        {
            QuizQuestion question = _game.Current;

            ProgressLabel.Text = $"Question {_game.CurrentNumber} of {_game.Total}";
            QuizProgress.Maximum = _game.Total;
            QuizProgress.Value = _game.CurrentNumber;
            ScoreLabel.Text = $"Score: {_game.Score}";
            TypeBadge.Text = question.Type == QuestionType.MultipleChoice
                ? "Multiple choice"
                : "True / False";
            QuestionText.Text = question.Prompt;

            /* Build one button per answer option. */
            OptionsPanel.Children.Clear();
            for (int i = 0; i < question.Options.Count; i++)
            {
                string label = question.Type == QuestionType.MultipleChoice
                    ? $"{(char)('A' + i)})   {question.Options[i]}"
                    : question.Options[i];

                var button = new Button
                {
                    Content = label,
                    Style = (Style)FindResource("OptionButton"),
                    Tag = i,
                    Margin = new Thickness(0, 0, 0, 8)
                };
                button.Click += Option_Click;
                OptionsPanel.Children.Add(button);
            }

            FeedbackPanel.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Collapsed;
        }

        /* Handles the user choosing an answer: scores it, colours the options, shows the
        explanation, and reveals the Next button. */
        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button clicked || clicked.Tag is not int selectedIndex) return;

            bool correct = _game.Answer(selectedIndex);
            int correctIndex = _game.Current.CorrectIndex;

            /* Lock the options and colour them: correct one green, a wrong pick red. */
            foreach (object child in OptionsPanel.Children)
            {
                if (child is Button option && option.Tag is int optionIndex)
                {
                    option.IsEnabled = false;
                    if (optionIndex == correctIndex) option.Background = Green;
                    else if (optionIndex == selectedIndex) option.Background = Red;
                }
            }

            FeedbackPanel.Background = correct ? GreenDim : RedDim;
            FeedbackText.Text = (correct
                ? "✓ Correct! "
                : $"✗ Not quite. The correct answer is \"{_game.Current.CorrectAnswerText}\". ")
                + _game.Current.Explanation;
            FeedbackPanel.Visibility = Visibility.Visible;

            ScoreLabel.Text = $"Score: {_game.Score}";

            NextButton.Content = _game.CurrentNumber >= _game.Total ? "See Results  →" : "Next  →";
            NextButton.Visibility = Visibility.Visible;
        }

        /* Moves to the next question, or shows the results screen if the quiz is finished. */
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _game.Next();
            if (_game.IsFinished) ShowResults();
            else ShowQuestion();
        }

        /* Final screen: total score plus the score-based message (Task 2). */
        private void ShowResults()
        {
            QuestionArea.Visibility = Visibility.Collapsed;
            ResultsArea.Visibility = Visibility.Visible;

            QuizProgress.Value = _game.Total;
            ProgressLabel.Text = "Finished";
            ScoreLabel.Text = $"Score: {_game.Score}";

            ResultScore.Text = $"You scored {_game.Score} out of {_game.Total}";
            ResultFeedback.Text = _game.GetScoreFeedback();

            _activityLog.Log(ActivityCategory.Quiz,
                $"Completed the quiz with a score of {_game.Score}/{_game.Total}.");
        }

        /* Restarts the quiz with a fresh (re-shuffled) game. */
        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            _game = new QuizGame();
            _activityLog.Log(ActivityCategory.Quiz, "Restarted the cybersecurity quiz.");
            ResultsArea.Visibility = Visibility.Collapsed;
            QuestionArea.Visibility = Visibility.Visible;
            ShowQuestion();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
