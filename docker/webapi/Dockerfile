FROM alpine AS gitCloner
RUN apk --update add git less openssh && \
    rm -rf /var/lib/apt/lists/* && \
    rm /var/cache/apk/*
WORKDIR /project
RUN git clone https://rosenbjerg:d5ac2fb5af17b030d7ea855ebb53573f870f7b6e@github.com/rosenbjerg/NxPlx.git


FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS backendBuilder
WORKDIR /app
COPY --from=gitCloner /project/nxplx-backend/ ./
RUN dotnet restore
RUN dotnet publish -c Release -o build


FROM node:12.10.0-alpine AS frontendBuilder
WORKDIR /app
COPY --from=gitCloner /project/nxplx-frontend/ ./
RUN npm install
RUN npm build


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine AS runtime
WORKDIR /app
COPY --from=backendBuilder /app/backend/build ./
COPY --from=frontendBuilder /app/frontend/build ./public
EXPOSE 5353
ENTRYPOINT ["dotnet", "NxPlx.Infrastructure.WebApi.dll"]