using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CybersecurityChatbotGUI
{
    /* Part 3 / Task 1 (+ Task 3 NLP): Turns the natural way people describe a reminder time
    into an actual date. The brief says the user can "specify a date or timeframe", e.g.
    "remind me in 7 days", so this class understands:
        - relative phrases:  "in 3 days", "in a week", "in 2 months", "in 5 hours"
        - keywords:          "tomorrow", "today", "tonight", "next week/month/year"
        - explicit dates:    "on 2026-07-01", "25 December", "12/07/2026"

    Keeping this in its own class (separation of concerns) means both the chat flow and the
    Task Manager window can reuse exactly the same parsing logic.

    References:
    Microsoft (2023). Regular Expressions in .NET. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/standard/base-types/regular-expressions
    Microsoft (2023). DateTime.TryParse Method. [Online] Microsoft Learn.
    Available at: https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tryparse
    */
    public static class ReminderParser
    {
        /* Attempts to read a reminder time out of the user's text.
        Returns true and fills reminderDate + a human-readable phrase ("in 3 days") on success,
        or false if no time could be found (so the caller can ask the user to clarify). */
        public static bool TryParse(string input, out DateTime reminderDate, out string humanReadable)
        {
            reminderDate = DateTime.MinValue;
            humanReadable = string.Empty;

            if (string.IsNullOrWhiteSpace(input)) return false;

            string text = input.ToLowerInvariant();
            DateTime now = DateTime.Now;

            /* 1. Simple keywords. */
            if (text.Contains("tomorrow")) { reminderDate = now.AddDays(1); humanReadable = "tomorrow"; return true; }
            if (text.Contains("tonight")) { reminderDate = now.Date.AddHours(20); humanReadable = "tonight"; return true; }
            if (text.Contains("today")) { reminderDate = now; humanReadable = "today"; return true; }
            if (text.Contains("next week")) { reminderDate = now.AddDays(7); humanReadable = "next week"; return true; }
            if (text.Contains("next month")) { reminderDate = now.AddMonths(1); humanReadable = "next month"; return true; }
            if (text.Contains("next year")) { reminderDate = now.AddYears(1); humanReadable = "next year"; return true; }

            /* 2. Relative "in N units" (N can be a number or the words "a"/"an"). */
            Match relative = Regex.Match(
                text, @"in\s+(\d+|a|an)\s+(hour|hours|day|days|week|weeks|month|months|year|years)");
            if (relative.Success)
            {
                string numberText = relative.Groups[1].Value;
                int amount = (numberText == "a" || numberText == "an") ? 1 : int.Parse(numberText);
                string unit = relative.Groups[2].Value;

                reminderDate =
                      unit.StartsWith("hour") ? now.AddHours(amount)
                    : unit.StartsWith("day") ? now.AddDays(amount)
                    : unit.StartsWith("week") ? now.AddDays(amount * 7)
                    : unit.StartsWith("month") ? now.AddMonths(amount)
                    : now.AddYears(amount);

                /* Build a tidy phrase like "in 1 day" / "in 3 days". */
                string singular = unit.TrimEnd('s');
                string unitWord = amount == 1 ? singular : singular + "s";
                humanReadable = $"in {amount} {unitWord}";
                return true;
            }

            /* 3. Explicit date. Only attempted when the text actually looks like it contains a
            date (a digit or a month name), so ordinary sentences are never mis-parsed. */
            bool looksLikeDate =
                Regex.IsMatch(text, @"\d") ||
                Regex.IsMatch(text, @"\b(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)");

            if (looksLikeDate)
            {
                /* Strip a leading "on " so "remind me on 25 December" parses cleanly. */
                string candidate = Regex.Replace(text, @"^.*?\bon\b\s+", "");

                if (DateTime.TryParse(candidate, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsed)
                    || DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
                {
                    reminderDate = parsed;
                    humanReadable = $"on {parsed:ddd dd MMM yyyy}";
                    return true;
                }
            }

            return false;
        }

        /* Removes any time expressions from a phrase so the leftover text can be used as a clean
        task title. e.g. "update my password tomorrow" -> "update my password". */
        public static string StripTimePhrases(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            string result = text;
            result = Regex.Replace(result,
                @"\bin\s+(\d+|a|an)\s+(hour|day|week|month|year)s?\b", "", RegexOptions.IgnoreCase);
            result = Regex.Replace(result,
                @"\b(tomorrow|tonight|today|next\s+week|next\s+month|next\s+year)\b", "", RegexOptions.IgnoreCase);
            /* Only strip an explicit NUMERIC date ("on 2026-07-01", "on 25/12") so that phrases
            like "turn on 2fa" are left untouched. */
            result = Regex.Replace(result,
                @"\bon\s+\d{1,4}([/\-\.]\d{1,2}){1,2}\b", "", RegexOptions.IgnoreCase);

            return result.Trim().TrimEnd('.', ',', ';').Trim();
        }
    }
}
