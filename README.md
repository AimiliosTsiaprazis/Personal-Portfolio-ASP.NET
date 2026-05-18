# Portfolio ASP.NET Core Web App

## Project Structure
```
PortfolioApp/
├── Controllers/
│   ├── AccountController.cs
│   └── PortfolioController.cs
├── Models/
│   ├── LoginViewModel.cs
│   ├── Project.cs
│   └── Skill.cs
├── Repositories/
│   ├── IProjectRepository.cs
│   ├── ProjectRepository.cs
│   ├── ISkillRepository.cs
│   └── SkillRepository.cs
├── Services/
│   ├── IAuthService.cs
│   └── AuthService.cs
├── Data/
│   └── DatabaseInitializer.cs
├── Views/
│   ├── Account/
│   │   └── Login.cshtml
│   ├── Portfolio/
│   │   └── Index.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   └── js/
│       └── site.js
├── appsettings.json
├── Program.cs
└── PortfolioApp.csproj
```

## Setup Instructions

Install NuGet Packages

Open **Tools → NuGet Package Manager → Package Manager Console** and run:

```powershell
Install-Package Microsoft.AspNetCore.Authentication.Cookies -Version 2.2.0
Install-Package Microsoft.Data.SqlClient -Version 5.2.1
Install-Package Microsoft.EntityFrameworkCore -Version 8.0.4
Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 8.0.4
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.4
Install-Package Swashbuckle.AspNetCore -Version 6.6.2
Install-Package BCrypt.Net-Next -Version 4.0.3
```


Configure the Connection String

Open `appsettings.json`. The default uses **LocalDB** (included with Visual Studio):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PortfolioDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

### If using SQL Server Express instead:
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=PortfolioDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### If using a full SQL Server with username/password:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=PortfolioDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

---
Run the App

The `DatabaseInitializer` runs automatically on startup and:
- Creates the `PortfolioDB` database.
- Creates all tables (`AdminUsers`, `Projects`, `Skills`).
- Seeds one admin user.
- Seeds sample projects and skills.

- If there are errors with the Database, just create manually the "PortfolioDB" Database in the MSSQL Dashboard

---

 Log In

Navigate to: `https://localhost:{PORT}/Account/Login`

---

Swagger API Docs

Only available in Development mode.

Navigate to: `https://localhost:{PORT}/swagger`

All CRUD endpoints are documented there.

---

