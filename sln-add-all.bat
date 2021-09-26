@echo off

for /f %%p in ('dir /s /d /b *.csproj') do dotnet sln SolutionTemplate.sln add %%p