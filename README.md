# Cybersecurity Awareness Chatbot (PROG6221 POE)

* **Course:** PROG6221 - Portfolio of Evidence
* **Student Name:** Joshua Marc Lourens
* **Student Number:** ST10441951
* **YouTube Presentation:** https://youtu.be/WnkHFbt8Sw0
* **GitHub Repository:** https://github.com/ST10441951/prog6221-g2-2026-poe-ST10441951


---

## 📝 Project Overview
A **Windows Presentation Foundation (WPF, .NET 10)** desktop chatbot that teaches cybersecurity
awareness. It began (Parts 1 & 2) as a conversational assistant with a knowledge base, sentiment
detection and memory, and in **Part 3** it gains a **task assistant backed by MySQL**, an
interactive **quiz mini-game**, a simple **NLP** understanding layer, and an **activity log**.

The repository contains both the original console build (`CybersecurityChatbot/`) and the final
WPF GUI application (`CybersecurityChatbotGUI/`), which is the one to mark for Part 3.

---

## 🚀 Feature / Task Mapping

### Parts 1 & 2 (foundation, still fully working)
* **Knowledge base** of 13 cybersecurity topics, each with multiple randomized responses.
* **Fuzzy matching** (Levenshtein distance) for typo tolerance (e.g. "pasword").
* **Sentiment detection** (8 emotions) with empathetic, auto-tip responses for distress.
* **Memory & recall** of the user's name and favourite topic.
* **Multimedia GUI** — voice greeting, animated ASCII boot art, typewriter streaming, chat bubbles.

### Part 3
| Task | Feature | Highlights |
|------|---------|------------|
| **Task 1** | **Task Assistant + Reminders (GUI)** | Add tasks with title, description and optional reminder ("remind me in 3 days"); view/complete/delete in a dedicated **Task Manager** window. |
| **Task 1** | **MySQL Database Integration** | Tasks stored in MySQL; full **CRUD** (add / read / mark complete / delete) syncs between GUI and DB, with graceful error handling if the DB is offline. |
| **Task 2** | **Cybersecurity Quiz** | 13 questions (multiple-choice **and** true/false), one at a time, instant feedback + explanations, live score and a score-based final message. |
| **Task 3** | **NLP Simulation** | Recognises commands worded many ways via text normalisation, keyword/synonym detection and regex (e.g. "Can you remind me to update my password?"). |
| **Task 4** | **Activity Log** | Timestamped record of key actions (tasks, reminders, quiz attempts, conversations); shows the recent 5 with a "show full log" option, plus a dedicated **Activity Log** window. |

---

## 🛠️ Technical Stack
* **Language:** C# (.NET 10)
* **UI:** WPF (XAML) with data binding
* **Database:** MySQL via the official `MySql.Data` NuGet package
* **Design:** Object-oriented, decoupled engine / data / UI layers; `Dictionary` lookups,
  `List<T>` collections, delegates, LINQ and regular expressions.

---

## 📥 Setup and Installation

### 1. Prerequisites
* **.NET 10 SDK** (the GUI targets `net10.0-windows`).
* **MySQL Server** running locally (tested with MySQL 9.4). Note your `root` password.
* **Visual Studio 2026** (or `dotnet` CLI).

### 2. Configure the database (Part 3)
Open `CybersecurityChatbotGUI/CybersecurityChatbotGUI/App.config` and set your MySQL `root`
password in the connection string:

```xml
<add name="CyberChatbotDb"
     connectionString="server=localhost;port=3306;uid=root;pwd=YOUR_PASSWORD;database=cyber_chatbot;SslMode=None;AllowPublicKeyRetrieval=true;"
     providerName="MySql.Data.MySqlClient" />
```

* Only `pwd=` normally needs changing.
* **You do not need to create the database or tables** — the app creates the `cyber_chatbot`
  database and `tasks` table automatically on first run.

### 3. Run
Open `CybersecurityChatbotGUI/CybersecurityChatbotGUI.slnx` in Visual Studio and press **F5**,
or from the command line:

```bash
dotnet run --project CybersecurityChatbotGUI/CybersecurityChatbotGUI/CybersecurityChatbotGUI.csproj
```

---

## 💬 Usage Examples

After greeting the bot and giving your name, try these (type in chat, or use the quick chips:
🔑 Passwords · 🎣 Phishing · 👁️ Privacy · 📋 My Tasks · 🎮 Quiz · 📜 Activity Log · ❓ Help):

```
what is phishing?
add task - review my privacy settings        → bot asks if you want a reminder
remind me in 3 days                          → reminder saved to MySQL
add a task to enable two-factor authentication
show my tasks                                → opens the Task Manager window
start quiz                                    → opens the quiz mini-game
remind me to update my password tomorrow      → NLP understands varied phrasing
what have you done for me?                    → recent activity (say "show full log" for all)
```

---

## 📂 Project Structure (GUI — Part 3)

```
CybersecurityChatbotGUI/CybersecurityChatbotGUI/
├─ App.config                 # MySQL connection string (edit pwd here)
├─ MainWindow.xaml(.cs)       # Chat UI + quick-access chips
├─ ChatbotEngine.cs           # Knowledge, sentiment, memory + command routing
├─ IntentRecognizer.cs        # Task 3 — NLP layer (normalise + keyword/synonym + regex)
├─ ChatbotResponse.cs / ChatMessage.cs / SessionContext.cs / BotInterface.cs
│
├─ DatabaseConfig.cs          # Task 1 — central MySQL connection helper
├─ TaskItem.cs / ReminderParser.cs / TaskRepository.cs   # Task 1 — model, parsing, CRUD
├─ TaskManagerWindow.xaml(.cs)# Task 1 — view/add/complete/delete tasks
│
├─ QuizQuestion.cs / QuizGame.cs / QuizWindow.xaml(.cs)  # Task 2 — quiz
│
└─ ActivityLogger.cs / ActivityLogWindow.xaml(.cs)       # Task 4 — activity log
```

---

## ✅ Continuous Integration
This repository uses **GitHub Actions** to build the project on every push.

---

## 🛠️ Troubleshooting
| Symptom | Fix |
|--------|-----|
| In-app: *"I couldn't reach the task database"* | Check `pwd` in `App.config` and that your MySQL service is running. The app won't crash — it degrades gracefully. |
| `Access denied for user 'root'` | Wrong password in `App.config`. |
| Quiz / chat / log work but tasks don't | That's the DB connection only — everything else works without MySQL. |
