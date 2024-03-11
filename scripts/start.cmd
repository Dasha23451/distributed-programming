@echo off

cd ../Valuator
start cmd /k dotnet run --urls "http://0.0.0.0:5001"
start cmd /k dotnet run --urls "http://0.0.0.0:5002"

cd С:/nginx
start cmd /k nginx.exe