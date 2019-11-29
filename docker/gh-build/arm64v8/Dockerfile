FROM alpine AS gitCloner
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /project
RUN git clone https://github.com/rosenbjerg/NxPlx.git


FROM mcr.microsoft.com/dotnet/core/sdk:3.0-alpine AS backendBuilder
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-backend/ ./
RUN dotnet publish NxPlx.Infrastructure.WebApi/NxPlx.Infrastructure.WebApi.csproj -c Release -r linux-musl-x64 -o build


FROM node:12.10.0-alpine AS frontendBuilder
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-frontend/ ./
RUN yarn install --no-progress --silent
RUN yarn build


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine-arm64v8 AS runtime
ARG build
LABEL maintainer="Malte Rosenbjerg"
LABEL description="NxPlx on dotnet 3.0"
LABEL repository="github.com/rosenbjerg/NxPlx"
RUN apk add --update --no-cache ffmpeg
WORKDIR /app
COPY --from=backendBuilder /app/build ./
COPY --from=frontendBuilder /app/build ./public/
ENV NXPLX_BUILD=$build
EXPOSE 5353
ENTRYPOINT ["dotnet", "NxPlx.Infrastructure.WebApi.dll"]