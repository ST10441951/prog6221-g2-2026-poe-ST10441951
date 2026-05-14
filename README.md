# Cybersecurity Awareness Chatbot (Part 2 — WPF GUI)

* **Course:** PROG6221 - Portfolio of Evidence (Part 2)
* **Student Name:** Joshua Marc Lourens
* **Student Number:** ST10441951
* **YouTube Presentation:** https://youtu.be/WnkHFbt8Sw0
* **GitHub Repository:** https://github.com/ST10441951/prog6221-g2-2026-poe-ST10441951

---

## 📝 Project Overview
This application is a sophisticated **Windows Presentation Foundation (WPF)** chatbot designed to educate users on cybersecurity threats and best practices. Building upon the core logic established in Part 1, this version introduces a high-performance Graphical User Interface, enhanced conversational flow, and advanced features such as **Fuzzy Matching**, **Empathetic Sentiment Detection**, and **Session-based Memory**.

The bot is engineered to recognize 13 distinct cybersecurity topics, providing informative, randomized guidance to ensure a dynamic learning experience.

---

## 🚀 Key Features (POE Task Mapping)

### ✅ Task 1: Professional GUI Implementation
* A custom-styled WPF interface with dedicated input and output sections.
* Integration of **Part 1 Multimedia**: Plays a `greeting.wav` voice greeting on startup and displays custom ASCII Art in the chat window.

### ✅ Task 2 & 3: Intelligent Knowledge Base
* Recognition for **13 keywords** (Exceeding the rubric requirement of 3).
* **Randomized Responses:** Every topic features a list of 5+ unique tips, ensuring the conversation remains engaging and informative.

### ✅ Task 4: Seamless Conversation Flow
* Logic to handle natural follow-up statements such as *"Tell me more"*, *"Explain further"*, or *"Another tip"*, allowing the bot to maintain context without resetting.

### ✅ Task 5: Memory and Recall
* **Name Extraction:** Captures the user's name during greeting.
* **Recall Logic:** Remembers the user's favorite cybersecurity topic and explicitly refers back to it later in the session to personalize guidance.

### ✅ Task 6: Advanced Sentiment Detection
* Detects **8 emotions** (Worried, Scared, Curious, Frustrated, etc.).
* **Distress Response:** If a "worried" or "anxious" sentiment is detected, the bot automatically adjusts its tone to be empathetic and provides an immediate, relevant safety tip without being asked.

### ✅ Task 7 & 8: Optimization & Robustness
* **Fuzzy Matching (Levenshtein Distance):** Typo tolerance logic that allows the bot to understand keywords even if misspelled (e.g., "pasword").
* **Technical Excellence:** Utilizes **Delegates**, **Dictionaries**, and **LINQ** to ensure high performance and clean, modular code.

---

## 🛠️ Technical Stack
* **Language:** C# (.NET 10 LTS)
* **Framework:** WPF (XAML)
* **Design Patterns:** Object-Oriented Programming (OOP) with a decoupled engine logic.
* **Optimization:** Dictionary-based lookups (O(1) complexity) for sentiment and keyword mapping.

---

## ✅ Continuous Integration (CI)
This repository utilizes **GitHub Actions** to automatically verify build success and code integrity on every push.

---

## 📥 Setup and Installation
1. **Clone** the repository to your local drive.
2. Open the solution file `CybersecurityChatbotGUI.slnx` (or `.sln`) in **Visual Studio 2022**.
3. Ensure the `greeting.wav` file is in the `Sounds` folder.
4. Set the `greeting.wav` file properties to **Copy to Output Directory: Copy if newer**.
5. Build and Run (**F5**).

---

## 📂 Project Structure
* `ChatbotEngine.cs` - Core logic for sentiment, keywords, and fuzzy matching.
* `MainWindow.xaml / .cs` - GUI design and multimedia event handling.
* `SessionContext.cs` - State management for user name and favorite topics.
* `ChatbotResponse.cs` - Data model for keyword response pools.
* `BotInterface.cs` - Helper class for ASCII art and UI formatting.
