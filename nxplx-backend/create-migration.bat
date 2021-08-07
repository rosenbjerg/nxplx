@echo off
dotnet-ef migrations add %1 -s NxPlx.ApplicationHost.Api -p NxPlx.Infrastructure.Database -c DatabaseContext