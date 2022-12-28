**Env variables**
DATABASE_URL=path to database

**migrations**

`dotnet ef migrations add InitialCreate --project DAL.db --startup-project ConsoleApp`

`dotnet ef database update --project DAL.Db --startup-project ConsoleApp`
