dotnet ef migrations add initialcreate --context IdentityContext -o "Data\Migrations"
dotnet ef database update --context IdentityContext

add-migration InitialCreate -Context IdentityContext -OutputDir Data/Migrations -Project Identity -StartupProject Identity.API
update-database -Context IdentityContext
