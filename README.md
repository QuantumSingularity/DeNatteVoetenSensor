# SensHagen - De NatteVoeten Sensor

# Publish to RaspberryPi 
It's an ARM device, so it has to be build here and deployed manually.
There's no runtime for the ARM, but it will be complited to an executable. Use `chmod +x SensHagen` on the Pi.

dotnet publish -c Release -r linux-arm
scp -r /home/bem/Projects/SensHagen/bin/Release/netcoreapp2.2/linux-arm/publish/ dotnet@rpi3a.bem.dmz:/home/dotnet/SensHagen/

# Initialize or update schema on Sqlite DataBase
// Create database
dotnet ef migrations add InitialCreate     to scaffold a migration and create the initial set of tables for the model.

// Update
dotnet ef database update                  to apply the new migration to the database. This command creates the database before applying migrations.

