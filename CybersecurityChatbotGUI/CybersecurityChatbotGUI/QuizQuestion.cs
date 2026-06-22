using System.Collections.Generic;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 2: the two question formats the brief asks us to mix. */
    public enum QuestionType
    {
        MultipleChoice,
        TrueFalse
    }

    /* Part 3 / Task 2: one quiz question.
    A single class models BOTH question types: a true/false question is just a multiple-choice
    question whose options are "True" and "False". Storing the options in a List<string> keeps
    the GUI simple (it just builds one button per option) and satisfies the "use lists" hint.

    References:
    Microsoft (2023). List<T> Class. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1
    */
    public class QuizQuestion
    {
        public string Prompt { get; }
        public QuestionType Type { get; }
        public List<string> Options { get; }
        public int CorrectIndex { get; }
        public string Explanation { get; }

        public QuizQuestion(string prompt, QuestionType type, List<string> options,
                            int correctIndex, string explanation)
        {
            Prompt = prompt;
            Type = type;
            Options = options;
            CorrectIndex = correctIndex;
            Explanation = explanation;
        }

        /* Factory helper for a multiple-choice question (keeps the question bank readable). */
        public static QuizQuestion MultipleChoice(string prompt, string[] options,
                                                  int correctIndex, string explanation)
        {
            return new QuizQuestion(prompt, QuestionType.MultipleChoice,
                                    new List<string>(options), correctIndex, explanation);
        }

        /* Factory helper for a true/false question. */
        public static QuizQuestion TrueFalse(string prompt, bool correctAnswer, string explanation)
        {
            return new QuizQuestion(prompt, QuestionType.TrueFalse,
                                    new List<string> { "True", "False" },
                                    correctAnswer ? 0 : 1, explanation);
        }

        /* True if the chosen option index is the correct one. */
        public bool IsCorrect(int selectedIndex) => selectedIndex == CorrectIndex;

        /* The text of the correct option (used in feedback). */
        public string CorrectAnswerText => Options[CorrectIndex];
    }
}
