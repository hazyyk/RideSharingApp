Overview:

The project is ridesharing website designed to connect customers with drivers. 
The website allows user to book a ride, enter pickup and drop-off loction, 
and track drivers using SignalR, open-source software library for Microsoft ASP.NET that allows 
server code to send asynchronous notifications to client-side web applications.

Architecture:

The website uses the MVVM architecture to separate business logic, UI, and data management,
ensuring scalability and testability. Below are the core components:

Features:
Ride Management: Book rides and payments;

Technology Used:

Backends : ASP.NET Core 8 Web API
Database :  Microsoft SQL Server

Steps up Instructions:

1.Clone this git Repository: https://github.com/hazyyk/RideSharingApp.git

2. Change the connection string in appsettings.json where Server=(local)\\SQLEXPRESS should be replaced with your server.

3. Open Package Manager Console and Run: dotnet ef  database update --context RideSharingDbContext --project RideSharingApp

4. Press F5 to run the project.
