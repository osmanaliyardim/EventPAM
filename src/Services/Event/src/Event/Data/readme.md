dotnet ef migrations add initial --context EventDbContext -o "Data\Migrations"
dotnet ef database update --context EventDbContext

add-migration InitialCreate -Context EventDbContext -OutputDir Data/Migrations -Project Event -StartupProject Event.API
update-database -Context EventDbContext
