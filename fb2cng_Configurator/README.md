# fb2cng_Configurator

A Windows Forms graphical user interface (GUI) template configurator for the [**fb2cng (fbc)**](https://github.com/rupor-github/fb2cng) CLI converter. 

This application allows users to easily customize their own templates and parameters for converting FB2 electronic books. It automatically generates a Go-template for the output filename and modifies advanced configurations within the YAML template file. 

Designed as a complementary tool for [**fb2cng GUI**](https://github.com/Jurchos/fb2cng_GUI.git), forming part of the comprehensive **fb2cng toolkit**.

---

### ⚠️ Project History & Disclaimer
This project was created by a beginner/non-programmer as a learning experiment to better understand coding, with **Gemini AI** serving as the development assistant. 

> **Note:** Due to its educational nature, the source code contains extensive descriptive comments written in **Ukrainian**. We apologize for any inconvenience this may cause to international developers!

---

## 🚀 Features

* 💻 **Windows Forms GUI** – No prior knowledge of the Go template language or manual text editing required.
* ⚙️ **Flexible Configuration** – Fine-tune all template parameters and conversion variables effortlessly.
* ⚡ **Lightweight & Fast** – A minimalist, clean design that ensures a smooth and intuitive user experience.

---

## 🛠️ Built With

* **C#** 
* **.NET Framework 4.8**
* **Windows Forms (WinForms)**
* **YAML** (for configuration parsing)

---

## 💻 System Requirements

* **OS:** Windows 7 / 8 / 10 / 11
* **Runtime:** [.NET Framework 4.8 Runtime](https://microsoft.com) (usually pre-installed on modern Windows)
* **Prerequisite:** The original **fb2cng** command-line tool installed

---

## 📦 Installation & Quick Start

### Option 1: Download Ready-to-Run (Recommended)
1. Go to the [Releases](../../releases) page of this repository.
2. Download the standalone `fb2cng_Configurator.exe` file.
3. Place the file into any folder on your PC and run it.

### Option 2: Build from Source
1. **Clone the repository:**
   ```bash
   git clone https://github.com
   ```
2. **Open & Build:**
   Open the solution file `fb2cng_Configurator.slnx` in **Visual Studio** (with .NET Framework 4.8 targeting pack installed) and build the project.

---

## 🔧 How to Use

1. Ensure the executable has proper access rights to the path where the main **fb2cng** tool is located.
2. Launch the application.
3. Customize your template settings and generate your preferred configuration profile.

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! 
Since the code documentation and comments are currently in **Ukrainian**, feel free to open an issue if you need clarification on specific code blocks or want to help translate them into English.

---

## 📜 License

This project is licensed under the [MIT License](LICENSE) — feel free to use, modify, and distribute it.
