FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS backendBuilder
WORKDIR /app

COPY nxplx-backend/*.sln .
COPY nxplx-backend/*/*.csproj ./backend/
RUN dotnet restore

COPY nxplx-backend/. ./backend/
WORKDIR /app/backend
RUN dotnet publish -c Release -o build


FROM node:12.10.0-alpine AS frontendBuilder
WORKDIR /app
COPY nxplx-frontend/. ./frontend/
WORKDIR /app/frontend
RUN npm install
RUN npm build


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS runtime
WORKDIR /app
COPY --from=backendBuilder /app/backend/build ./
COPY --from=frontendBuilder /app/frontend/build ./public
ENTRYPOINT ["dotnet", "NxPlx.Infrastructure.WebApi.dll"]