# SensHagen - De NatteVoeten Sensor
For me: [The Syntax](https://help.github.com/articles/basic-writing-and-formatting-syntax).

# Publish to RaspberryPi 
It's an ARM device, so it has to be build here and deployed manually. <br />
There's no runtime for the ARM, but it will be complited to an executable. Use `chmod +x SensHagen` on the Pi.

**Build** <br />
dotnet publish -c Release -r linux-arm

**Copy to RPi** <br />
scp -r /home/bem/Projects/SensHagen/bin/Release/netcoreapp2.2/linux-arm/publish/* dotnet@rpi3a.bem.dmz:/home/dotnet/SensHagen/
scp -r /home/bem/Projects/SensHagen/Data/ dotnet@rpi3a.bem.dmz:/home/dotnet/SensHagen/

**Only changed Assemblies:** <br />
scp -r /home/bem/Projects/SensHagen/bin/Release/netcoreapp2.2/linux-arm/publish/Sens* dotnet@rpi3a.bem.dmz:/home/dotnet/SensHagen/

# Initialize or update schema on Sqlite DataBase
**Create database:** *to scaffold a migration and create the initial set of tables for the model.* <br /> 
dotnet ef migrations add InitialCreate 

**Update** *to apply the new migration to the database. This command creates the database before applying migrations.* <br /> 
dotnet ef database update 

**Next time:** *delete database and recreate. the sqlite driver is not capable of adding/removing columns.* <br />
rm -R Migrations 
rm ./Data/*.db 
dotnet ef migrations add InitialCreate 
dotnet ef database update 

# Sqlite Database Browser
**Install:** sudo apt install sqlitebrowser

# Syntax Api
http://rpi3a.bem.dmz:8443/api/Sensor/Register?data={%22MacAddress%22%3A%22BA:01:02:03:04:05%22%2C%22EmailAddress%22%3A%22bas@nattevoetensensor.nl%22%2C%22SensorName%22%3A%22Als%20het%20Beessie%20maar%20een%20Naam%20heeft%22}
http://rpi3a.bem.dmz:8443/api/Sensor/Event?data={%22MacAddress%22%3A%22BA:01:02:03:04:05%22%2C%22EventType%22%3A%22heartbeat%22%2C%22BatteryVoltage%22%3A3.76}
http://rpi3a.bem.dmz:8443/api/Sensor/Event?data={%22MacAddress%22%3A%22BA:01:02:03:04:05%22%2C%22EventType%22%3A%22DetectionOn%22}
http://rpi3a.bem.dmz:8443/api/Sensor/Event?data={%22MacAddress%22%3A%22BA:01:02:03:04:05%22%2C%22EventType%22%3A%22DetectionOff%22}

