# 🛡️ Security Incident Tracker

A clean, modern Windows desktop application for logging, monitoring, and resolving cybersecurity incidents. Built with **C#**, **.NET**, **WPF**, and **SQLite** using **Entity Framework Core**. 

Designed as a **B.Tech Computer Engineering Minor Project** — lightweight, self-contained, easy to demonstrate, and easy to explain.

---

## 🌟 Key Features

* **🔐 Authentication Module**: Secure login interface with input validation and error feedback.
* **📊 SOC Operations Dashboard**: Real-time KPI summary cards:
  * Total Incidents
  * Open Incidents
  * In Investigation
  * Resolved Incidents
  * High & Critical Severity Incidents
  * Recent activity preview table
* **📋 Incident Management Registry**: Full CRUD operations using an interactive WPF `DataGrid`:
  * **Add** new security incidents
  * **Edit / Update** existing incidents
  * **Delete** records with safety confirmation prompts
  * Form field reset and validation
* **🔍 Search & Filter Engine**:
  * Real-time search across **Incident ID**, **Incident Type**, and **Description**
  * Instant filter by **Severity** (*Low*, *Medium*, *High*, *Critical*)
  * Instant filter by **Status** (*Open*, *Investigating*, *Resolved*)
* **💾 Embedded SQLite Database**:
  * Automatically creates the database schema (`security_tracker.db`) on application launch.
  * Auto-seeds the default administrator account and realistic sample incidents for demonstration.
  * Zero external database servers required.
* **🎨 Modern SOC UI**: Sleek, professional slate/dark cybersecurity theme.

---

## 🛠️ Technology Stack

| Component | Technology |
|---|---|
| **Language** | C# |
| **Framework** | .NET (WPF - Windows Presentation Foundation) |
| **Markup** | XAML |
| **Database** | SQLite (Embedded) |
| **ORM** | Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`) |
| **IDE** | Visual Studio 2022 / 2025 |

---

## 📂 Project Architecture

```text
Security Incident Tracker/
│
├── SecurityIncidentTracker.sln              # Visual Studio Solution
├── .gitignore                               # Standard .NET gitignore
├── README.md                                # Project Documentation
│
└── SecurityIncidentTracker/
    ├── SecurityIncidentTracker.csproj       # Project configuration & NuGet dependencies
    ├── App.xaml                             # Application resources & startup URI
    ├── App.xaml.cs                          # Startup database initialization
    │
    ├── Models/                              # Data models
    │   ├── User.cs                          # User / Admin entity
    │   └── Incident.cs                      # Incident entity
    │
    ├── Data/                                # Data access layer
    │   └── AppDbContext.cs                  # EF Core SQLite Context & Seeder
    │
    └── Views/                               # Presentation layer (WPF Windows)
        ├── LoginWindow.xaml / .cs           # Login screen
        ├── DashboardWindow.xaml / .cs       # SOC Metrics & summary
        └── IncidentWindow.xaml / .cs        # Incident registry & CRUD form
```

---

## 🚀 Getting Started

### Prerequisites
* Windows 10 or 11 (64-bit)
* [Visual Studio 2022 or 2025](https://visualstudio.microsoft.com/) with the **".NET desktop development"** workload installed.
* [.NET 8.0 SDK or .NET 10.0 SDK](https://dotnet.microsoft.com/download)

### How to Run

1. **Clone the repository**:
   ```bash
   git clone https://github.com/himanshu24-stack/Security-Incident-Tracker.git
   ```
2. **Open in Visual Studio**:
   * Double-click `SecurityIncidentTracker.sln` to open the solution.
3. **Build and Run**:
   * Press `F5` (or click `▶ SecurityIncidentTracker`).
4. **Log In**:
   * **Username**: `admin`
   * **Password**: `admin123`

---

## 🔑 Default Credentials

| Field | Value |
|---|---|
| **Username** | `admin` |
| **Password** | `admin123` |
| **Role** | Administrator |

---

## 📜 License & Academic Usage

This project was developed as an academic minor project for **B.Tech Computer Science and Engineering**. Feel free to use and adapt it for educational and demonstration purposes.
