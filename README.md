# Rise - Aalst02

## Team Members
- Lara Schoukens - lara.schoukens@student.hogent.be - LaraSchoukenss
- Youna Noynaert - youna.noynaert@student.hogent.be - ynoynaert
- Indra Van Wynendaele - indra.vanwynendaele@student.hogent.be -  IndraVanWynendaele
- Anke Janssens - anke.janssens@student.hogent.be - AnkeJanssens
- Glenn Vereecken - glenn.vereecken@student.hogent.be - VereeckenGlenn
- Seppe Visart - seppe.visart@student.hogent.be - SeppeVisart

## Technologies & Packages Used
- [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) - Frontend
- [ASP.NET 8](https://dotnet.microsoft.com/en-us/apps/aspnet) - Backend
- [Entity Framework 8](https://learn.microsoft.com/en-us/ef/) - Database Access
- [EntityFrameworkCore Triggered](https://github.com/koenbeuk/EntityFrameworkCore.Triggered) - Database Triggers
- [User Secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) - Securely store secrets in DEV.
- [GuardClauses](https://github.com/ardalis/GuardClauses) - Validation Helper
- [bUnit](https://bunit.dev) - Blazor Component Testing
- [xUnit](https://xunit.net) - (Unit) Testing
- [nSubstitute](https://nsubstitute.github.io) - Mocking for testing
- [Shouldly](https://docs.shouldly.org) - Helper for testing

## Installation Instructions
1. Clone the repository
2. Open the `Rise.sln` file in Visual Studio or Visual Studio Code
3. Run the project using the `Rise.Server` project as the startup project
4. The project should open in your default browser on port 5001.
5. Initially the database will not exist, so you will need to run the migrations to create the database.

## Creation of the database
To create the database, run the following command in the main folder `Rise`
```
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```
> Make sure your connection string is correct in the `Rise/Server/appsettings.json` file.

## Migrations
Adapting the database schema can be done using migrations. To create a new migration, run the following command:
```
dotnet ef migrations add [MIGRATION_NAME] --startup-project Rise.Server --project Rise.Persistence
```
And then update the database using the following command:
```
dotnet ef database update --startup-project Rise.Server --project Rise.Persistence
```
