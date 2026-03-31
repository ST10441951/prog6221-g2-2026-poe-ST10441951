# Cybersecurity Awareness Chatbot (South Africa)
**Course:** PROG6221 - Portfolio of Evidence (Part 1)  
**Student Name:** Joshua Marc Lourens  
**Student Number:** ST10441951

---

## 🛡️ Project Overview
This is a C# Console Application designed to educate South African citizens on identifying and mitigating cyber threats. The bot simulates a "Cybersecurity Awareness Assistant" and covers topics such as:
* **Phishing Scams:** Identifying fake emails and malicious links.
* **Safe Passwords:** Learning how to create strong, unique credentials.
* **Safe Browsing:** Recognizing HTTPS and the importance of secure connections.

---

## 🚀 Features
* **Multimedia Integration:** Plays a personalized voice greeting upon launch.
* **ASCII Art UI:** A custom-designed "Computer Hero" title screen.
* **Conversational Engine:** Uses a modular knowledge base to answer user questions.
* **Typing Effect:** Simulates a natural conversation feel with a character-by-character typing delay.
* **Input Validation:** Gracefully handles empty or unsupported user queries.

---

## 🛠️ Technical Details
* **Language:** C# (.NET 10 LTS)
* **IDE:** Visual Studio 2022
* **Architecture:** Modular Class Structure (`BotInterface`, `ChatbotResponse`, `Program`)
* **Dependencies:** `System.Windows.Extensions` (for SoundPlayer support)

---

## ✅ Continuous Integration (CI)
This project uses **GitHub Actions** to automatically verify code quality and build success on every commit.

### CI Status Screenshot
> **[INSTRUCTIONS: Replace the placeholder below with your actual screenshot!]**
![GitHub Action Green Checkmark](https://media.discordapp.net/attachments/1401842449650946118/1488433565376057455/Screenshot_2026-03-31_090055.png?ex=69ccc347&is=69cb71c7&hm=7e8472287f9248e8eaa3b39fd61dd81536f7b9242d4e7ecea20b05a61c9a6900&=&format=webp&quality=lossless)

---

## 📥 Setup and Installation
1. Clone the repository to your local machine.
2. Open the solution file `.sln` in **Visual Studio**.
3. Ensure the `greeting.wav` file is present in the project.
4. Right-click `greeting.wav` -> Properties -> Set **Copy to Output Directory** to **Copy always**.
5. Build and Run (**F5**).

---

## 📂 Folder Structure
* `Program.cs` - Main logic and interaction loop.
* `BotInterface.cs` - UI elements, ASCII art, and Multimedia logic.
* `ChatbotResponse.cs` - Data model for the knowledge base.
* `greeting.wav` - Audio file for the voice greeting.
