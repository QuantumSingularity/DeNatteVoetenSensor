# SensHagen - De NatteVoeten Sensor
For me: [The Syntax](https://help.github.com/articles/basic-writing-and-formatting-syntax).

# Publish to RaspberryPi 
It's an ARM device, so it has to be build here and deployed manually. <br />
There's no runtime for the ARM, but it will be complited to an executable. Use `chmod +x SensHagen` on the Pi.

**Build** <br />
dotnet publish -c Release -r linux-arm

**Copy to RPi** <br />
scp -r /home/bem/Projects/SensHagen/bin/Release/netcoreapp2.2/linux-arm/publish/ dotnet@rpi3a.bem.dmz:/home/dotnet/SensHagen/

# Initialize or update schema on Sqlite DataBase
**Create database:** *to scaffold a migration and create the initial set of tables for the model.* <br /> 
dotnet ef migrations add InitialCreate 

**Update** *to apply the new migration to the database. This command creates the database before applying migrations.* <br /> 
dotnet ef database update 

**Next time:** *delete database and recreate. the sqlite driver is not capable of adding/removing columns.* <br />
rm -R Migrations <br /> 
rm ./Data/*.db <br /> 
dotnet ef migrations add InitialCreate <br /> 
dotnet ef database update <br /> 

# Sqlite Database Browser
**Install:** sudo apt install sqlitebrowser

