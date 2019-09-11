#!/bin/bash

rm -R Migrations
rm ./Data/*.db
dotnet ef migrations add InitialCreate
dotnet ef database update

dotnet ef migrations add InitialCreate --context Nvs.Models.Postgresql.DataBaseContext
dotnet ef database update --context Nvs.Models.Postgresql.DataBaseContext