#!/bin/bash

rm -R Migrations
rm ./Data/*.db
dotnet ef migrations add InitialCreate
dotnet ef database update

