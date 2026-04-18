# MobyLabWebProgramming

This is an example application to teach students the basics of web programming.

Prerequisites:
Because we use .NET 8 for this project you need to install a IDE with the necessary tool, we recommend Visual Studio 2022 Community Edition (https://visualstudio.microsoft.com/) or Enterprise Edition for students (https://azureforeducation.microsoft.com/devtools) if you are using Windows or the DotUltimate platform (https://www.jetbrains.com/dotnet/) with Rider for .NET for students (https://www.jetbrains.com/rider/) if you are using Linux or MacOS X, you can get a license for all JetBrains products via your academic email. For Visual Studio we also recommend the DotUltimate platform for the ReSharper plugin, it may slow down your IDE but you gain a better code suggestions and safety checks. 

To start working with the backend install docker and docker compose from https://docs.docker.com/engine/install/ and enter the command below to launch the Postgresql database while in the Deployment folder:

```sh
docker-compose -f .\docker-compose.yml -p mobylab-app-db up -d
```

You can use PGAdmin (https://www.pgadmin.org/) or DBeaver (https://dbeaver.io/download/) to access the database on localhost:5432 with database/user/password "mobylab-app". 

To work with the database migrations in .NET install the dotnet-ef tool by using the following command:

```sh
dotnet tool install --global dotnet-ef --version 10.*
```

To create a new migration use the following command and replace migration_name with the name of your new migration, usually the first migration is called "InitialCreate":

```sh
dotnet ef migrations add <migration_name> --context WebAppDatabaseContext --project .\MobyLabWebProgramming.Database --startup-project .\MobyLabWebProgramming.Api
```

Example for the first migration:

```sh
dotnet ef migrations add InitialCreate --context WebAppDatabaseContext --project .\MobyLabWebProgramming.Database --startup-project .\MobyLabWebProgramming.Api
```

The project has a worker service that will initialize the database with a first admin user. To log in the default user is "admin@default.com" with password "default".