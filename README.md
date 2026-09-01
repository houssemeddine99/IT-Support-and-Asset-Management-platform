# IT Support and Asset Management Platform

A VB.NET web platform that combines three project ideas:

- IT support ticket management
- Equipment and asset management
- Computer maintenance tracking

## Technology stack

- ASP.NET Web Forms with VB.NET
- .NET Framework 4.8
- SQL Server
- ADO.NET
- Bootstrap, HTML, CSS, and JavaScript

## Planned roles

- Employee: creates and follows support tickets
- Technician: handles tickets and maintenance interventions
- IT Manager: manages assets, assignments, and reports
- Administrator: manages users, roles, and platform settings

## Development prerequisites

- Visual Studio 2022 with the **ASP.NET and web development** workload
- .NET Framework 4.8 Developer Pack
- SQL Server 2019 or later (SQL Server Express is sufficient)

## Open the project

Open `ITSupportAssetManagement.sln` in Visual Studio. Build the solution and run it with IIS Express.

## Database

Create a SQL Server database named `ITSupportAssetManagement`, then run every numbered file in `database` in order. See `database/README.md` for the complete setup instructions.

## Operations and deployment

- Apply all migrations with `scripts/Apply-Migrations.ps1`.
- Create and verify a SQL backup with `scripts/Backup-Database.ps1`.
- Build a deployable Release package with `scripts/Publish-Release.ps1`.
- Follow `deployment/PRODUCTION-CHECKLIST.md` before promoting a release.
