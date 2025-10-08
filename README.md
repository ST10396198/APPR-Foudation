A comprehensive ASP.NET Core MVC web application designed to streamline disaster relief operations for the Gift of the Givers Foundation - Africa's largest disaster relief organization.

🚀 Overview
This application serves as a centralized platform for managing disaster relief efforts, coordinating volunteers, and facilitating resource allocation during emergencies. Built with modern web technologies, it enhances the efficiency and reach of humanitarian aid operations.

✨ Features
🔐 User Management
Secure Registration & Authentication - User accounts with role-based access

Profile Management - Personal information and preferences

Session Management - Secure login/logout functionality

🚨 Disaster Incident Reporting
Real-time Incident Reporting - Citizens can report disasters in their areas

Multi-type Disaster Support - Floods, fires, earthquakes, storms, droughts, and more

Severity Classification - Critical, High, Medium, and Low severity levels

Geolocation Tracking - Location-based incident mapping

Status Tracking - Pending, Under Review, Action Taken, Resolved

🎁 Resource Donation Management
Multiple Donation Types - Food, clothing, medical supplies, financial contributions

Donation Tracking - Real-time tracking of donations from receipt to distribution

Inventory Management - Centralized resource allocation system

Drop-off Coordination - Preferred location management

👥 Volunteer Coordination
Volunteer Registration - Skills and availability tracking

Task Management - Create and assign relief tasks

Schedule Coordination - Volunteer availability and assignment

Progress Tracking - Monitor volunteer contributions

Task Categories - Distribution, Rescue, Medical Support, Logistics, Administrative

🛠️ Technology Stack
Backend
ASP.NET Core 7.0 - MVC Framework

Entity Framework Core - ORM with SQL Server

Azure SQL Database - Cloud database solution

Dependency Injection - Built-in IoC container

Frontend
Bootstrap 5.2 - Responsive UI framework

jQuery - JavaScript library for AJAX operations

Font Awesome - Icon toolkit

Razor Pages - Server-side rendering

Security
Session-based Authentication - Secure user management

SHA256 Password Hashing - Secure credential storage

Input Validation - Client and server-side validation

Anti-forgery Tokens - CSRF protection

📋 Prerequisites
.NET 7.0 SDK or later

Visual Studio 2022 or VS Code

Azure SQL Database (or local SQL Server)

Modern web browser

🎯 Usage Guide
For Disaster Victims
Register/Login - Create an account or sign in

Report Incident - Click "Report Emergency" and fill in disaster details

Track Status - Monitor your reported incidents

For Volunteers
Register - Create a volunteer profile with skills and availability

Browse Tasks - View available volunteer opportunities

Sign Up - Commit to specific relief tasks

Track Contributions - Monitor your volunteer activities

For Donors
Make Donation - Select donation type and specify items

Choose Drop-off - Select preferred donation location

Track Impact - See how your donation helps

For Administrators
Monitor Incidents - Review and prioritize reported disasters

Manage Resources - Coordinate donation distribution

Coordinate Volunteers - Assign tasks and monitor progress

🔧 Development
Adding New Features
Create model in Models/ directory

Add DbSet to ApplicationDbContext

Create controller in Controllers/

Develop views in Views/

Run migrations for database updates

Sample Data
Access /SeedData to populate the database with sample:

Volunteer tasks

Disaster incidents

Test users

🌐 Deployment
Azure App Service Deployment
Create Azure App Service

Configure connection strings

Set up continuous deployment

Configure custom domain (optional)

Database Deployment
Azure SQL Database recommended for production

Regular backups configured

Performance monitoring enabled

🔒 Security Features
Password hashing with SHA256

Session-based authentication

Input validation and sanitization

SQL injection prevention

XSS protection

CSRF tokens

📞 Support
Emergency Contact
Hotline: 0800 786 911

Email: info@giftofthegivers.org

Technical Support
For technical issues or feature requests, contact the development team or create an issue in the repository.

🤝 Contributing
We welcome contributions from developers who want to help improve disaster relief efforts. Please:

Fork the repository

Create a feature branch

Make your changes

Submit a pull request

📄 License
This project is developed for the Gift of the Givers Foundation and is intended for humanitarian use.

🙏 Acknowledgments
Gift of the Givers Foundation for their humanitarian work

ASP.NET Core development team

Bootstrap and jQuery communities

All volunteers and contributors

Built with ❤️ for Humanitarian Aid
