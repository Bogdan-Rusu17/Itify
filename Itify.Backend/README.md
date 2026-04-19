# Itify — Backend (folderul din repo Itify.Backend)

Aplicatie ASP.NET CORE pentru managementul echipamentelor IT intr-o companie.

## Structura soluției

Itify.Api - controllere, middleware, entry point in program

Itify.Services - servicii, data transfer objects, specifications

Itify.Database - entities EF Core, migratii, DbContext

Itify.Infrastructure - repository pattern, jwt handling, erori, configurations

Itify.Tests - unit tests cu xUnit, NSubstitute, FluentAssertions

## Setup

### Pornire baza de date
```sh
cd Deployment
docker-compose -f docker-compose.yml -p itify up -d
```

Se face maparea intre portul host 5433 la portul 5432 al containerului de baza de date (aveam pe 5432 host port baza de date pt licenta):
Are credentialele:
- Database: `itify-app`
- User: `itify-app`
- Password: `itify-app`

### Configurare appsettings
Fișierul `Itify.Api/appsettings.json` are configuratia din schelet, adaugand in plus configuratia pentru mail. Trebuie facut un cont pe [MailTrap](https://mailtrap.io/) si se introduc credentialele in sectiunea de `MailConfiguration`.

### Rulare API
```sh
dotnet run --project Itify.Api
```
Api ul de backend porneste pe `http://localhost:5000`. La rulare se vor aplica migratii generate cu `dotnet-ef` si se creeaza un user implicit admin cu credentialele:

- Email: `admin@default.com`
- Parola: `default`

Swagger UI este disponibil la http://localhost:5000/swagger.
## Migratii
Se instaleaza mai intai tool-ul de dotnef EF (in mod global ca sa fie mai usor de apelat)
```sh
dotnet tool install --global dotnet-ef
```
Pentru a crea o noua migratie
```sh
dotnet ef migrations add <NumeMigrare> --context WebAppDatabaseContext --project Itify.Database --startup-project Itify.Api
```
## Rulare teste (unit tests)
```sh
dotnet test Itify.Tests
```

## Github workflows
In .github/workflows am creat un workflow care descarca dependintele, buildeaza si ruleaza proiectul de Itify.Tests. Conditiile de run sunt push to main sau pr into main.
