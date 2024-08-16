dotnet ef migrations add initial --context CustomerDbContext -o "Data\Migrations"
dotnet ef database update --context CustomerDbContext

add-migration InitialCreate -Context CustomerDbContext -OutputDir Data/Migrations -Project Customer -StartupProject Customer.API
update-database -Context CustomerDbContext
