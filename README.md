# Cybersecurity Awareness Chatbot (South Africa)
* **Course:** PROG6221 - Portfolio of Evidence (Part 1)  
* **Student Name:** Joshua Marc Lourens  
* **Student Number:** ST10441951
* **YouTube Link:** https://youtu.be/s2OAJbJfyU8?si=X1BKIUUdW83CcdI4
* **GitHub Link:** https://github.com/ST10441951/prog6221-g2-2026-poe-ST10441951

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
![GitHub Action Green Checkmark](https://media.discordapp.net/attachments/1401842449650946118/1488455832646713425/image.png?ex=69ccd804&is=69cb8684&hm=599bc1fdce8bb51df3cc6e990ce6a75cbc1122ef4c7c134e95b311fc43670344&=&format=webp&quality=lossless)

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
