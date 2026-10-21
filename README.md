# BuzzTech-Solutions-Project
# Business Buzz – Smart Compliance Guidance Platform

> A web-based platform designed to help South African entrepreneurs prepare for business registration, understand compliance requirements, organise important documents, and track their business readiness.

---

## 📖 Table of Contents

- [Overview](#-overview)
- [Problem Statement](#-problem-statement)
- [Proposed Solution](#-proposed-solution)
- [Key Modules & Features](#-key-modules--features)
- [System Roles](#-system-roles)
- [Technology Stack](#️-technology-stack)
- [System Architecture](#️-system-architecture)
- [Getting Started](#-getting-started)
- [Development Team](#-development-team)
- [License & Academic Disclaimer](#-license--academic-disclaimer)

---

## 📖 Overview

Business Buzz is a Smart Compliance Guidance Platform developed to support Small, Medium and Micro Enterprises (SMMEs) and aspiring entrepreneurs in South Africa.

Starting and operating a business can involve navigating multiple registration, documentation and compliance requirements. Information is often distributed across different government departments, websites and information sources, making the process difficult to understand, especially for first-time entrepreneurs.

Business Buzz addresses this challenge by providing a centralised platform where entrepreneurs can:

- Understand business registration requirements
- Create and manage their business profile
- Receive a personalised registration preparation roadmap
- Track preparation and compliance tasks
- Upload and organise important documents
- Monitor their business readiness
- Identify missing requirements
- Receive reminders for upcoming compliance activities
- Access simplified guidance and official government resources
- **Interact with an AI Assistant for instant guidance**
- **Manage digital documents through the integrated DigiDocs module**

> ⚠️ **Important Notice:** Business Buzz does **not** replace official government registration platforms or submit applications on behalf of users. Instead, it acts as a preparation and guidance platform that helps entrepreneurs become registration-ready before accessing official government services. All institutional processes (CIPC, SARS, UIF, Home Affairs) are **simulated** for demonstration purposes.

---

## ❗ Problem Statement

Many South African entrepreneurs, particularly first-time business owners, experience difficulties when attempting to establish and maintain a legally compliant business.

The information required to register and operate a business is often distributed across multiple organisations and online platforms.

Entrepreneurs may struggle to determine:

- Which registrations apply to their business
- Which government departments they need to interact with
- What documents are required
- What order registration activities should be completed in
- Which compliance requirements apply to their business
- When important compliance activities need to be completed

These challenges can lead to:

- Incomplete applications
- Missing documentation
- Delays
- Additional costs
- Missed compliance deadlines
- Poor organisation of business records

Business Buzz aims to reduce these difficulties by centralising guidance, preparation, document management and readiness tracking in one platform.

---

## 💡 Proposed Solution

Business Buzz provides entrepreneurs with a structured digital environment for preparing and managing their business registration and compliance journey.

The system collects information about the user's intended business and uses this information to provide relevant preparation tasks and guidance.

### Example Workflow

```text
Create Account
      |
      v
Create Business Profile
      |
      v
Complete Business Assessment
      |
      v
Generate Personalised Roadmap
      |
      v
Complete Preparation Tasks
      |
      v
Upload Required Documents (DigiDocs)
      |
      v
Business Readiness Checker
      |
      v
Identify Outstanding Requirements


✨ Key Modules & Features
🤖 AI Assistant (BuzzGuide)
Business Buzz includes an integrated AI Assistant designed to provide instant, conversational guidance to entrepreneurs.

Natural Language Queries: Users can ask questions like "What documents do I need for CIPC?" and receive instant answers.

Context-Aware: The AI uses the entrepreneur's business profile to provide relevant advice.

Powered by Groq API: Utilises high-speed AI inference for quick responses.

📁 DigiDocs Document Management
A dedicated module for handling all business documentation securely.

Digital Upload & Storage: Users can upload, categorise, and store critical documents (IDs, proof of address, financials).

System-Generated Documents: Automatically generates registration and compliance forms based on captured profile data.

Print-Ready & Watermarked: Generated PDFs include a "SIMULATED - DEMONSTRATION ONLY" watermark and a footer disclaimer.

📊 Personalised Readiness Roadmap
Conditional Logic: The roadmap dynamically changes based on user inputs (e.g., UIF registration only appears if the user indicates they employ people).

Progress Tracking: A visual dashboard shows completed, in-progress, and outstanding tasks.

🔒 Security & Role-Based Access
Role-Based Access Control (RBAC): Separate environments for Entrepreneur, Approver, and Bank roles.

Secure Authentication: Passwords are never stored in plain text; they are hashed using Microsoft's PasswordHasher<TUser>.

Audit Trails: Significant user actions (submissions, approvals) are logged for accountability.

👥 System Roles
The system implements strict Role-Based Access Control (RBAC) to separate the user journeys:

Entrepreneur (Primary User): Creates profiles, follows the roadmap, uses the AI assistant, generates documents via DigiDocs, and tracks readiness.

Approver (Simulated Reviewer): Reviews submitted applications and can approve, reject, or request more information.

Bank (Simulated Assessor): Reviews the entrepreneur's compliance readiness for a simulated financial assessment.

## 🛠️ Technology Stack 
Business Buzz is built using a modern, enterprise-grade technology stack:

Backend
Framework: ASP.NET Core Web API (C#)

ORM: Entity Framework Core

Authentication: ASP.NET Core Identity (Password Hashing)

PDF Generation: iTextSharp (for generating watermarked compliance documents)

AI Integration: Groq API (for the AI Assistant)

External API Integration: HttpClient (for DigiDocs simulations)

Frontend
Markup & Styling: HTML5, CSS3

Framework: Bootstrap 5.3 (Responsive design)

Interactivity: JavaScript (ES6+)

Database & Tools
Database: Microsoft SQL Server (Relational Database)

Version Control: Git & GitHub

IDE: Visual Studio 2022, Visual Studio Code

🏗️ System Architecture
The application follows a standard 3-Tier Layered Architecture:

Presentation Layer: The Bootstrap frontend that interacts with the user.

Application Layer: The ASP.NET Core API that enforces business logic, conditional workflows, AI integration, and security.

Data Layer: Entity Framework Core managing the SQL Server database.

Database Design (14 Core Entities)
The relational database includes the following core tables to manage the workflow:

UserRole, SystemUser, BusinessType, BusinessProfile, JourneyStage, Application, ApprovalReview, DocumentType, Document, ComplianceRecord, ReadinessTask, FundingAssessment, Notification, AuditLog.

## 🚀 Getting Started
Prerequisites
.NET 8.0 SDK

Microsoft SQL Server

Visual Studio 2022 (with ASP.NET and web development workload)

Groq API Key (for the AI Assistant)

Installation & Setup
Clone the repository:

bash
git clone https://github.com/YourUsername/BuzzTech-Solutions-Project.git
Open the Solution:

Open BusinessBuzz.sln in Visual Studio 2022.

Configure the Database:

Open appsettings.json.

Update the DefaultConnection string to point to your local SQL Server instance.

Configure the AI Assistant (Groq API):

Obtain an API key from Groq.

Add your API key to appsettings.json:

json
{
  "GroqApiKey": "YOUR_API_KEY_HERE"
}
Apply Migrations:

Open the Package Manager Console in Visual Studio and run:

powershell
Update-Database
Run the Application:

Press F5 or click the "Run" button in Visual Studio.