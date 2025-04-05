@echo off
cd ..
dotnet ef dbcontext scaffold "host=localhost;port=5432;username=postgres;password=new_password;database=hackaton" Npgsql.EntityFrameworkCore.PostgreSQL -o DBModels -t users -t user_sessions --force --no-build
pause