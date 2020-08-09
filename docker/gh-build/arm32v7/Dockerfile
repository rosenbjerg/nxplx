FROM alpine AS gitCloner
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /project
RUN git clone https://github.com/rosenbjerg/NxPlx.git


FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS backendBuilder
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-backend/ ./
RUN dotnet publish NxPlx.ApplicationHost.Api/NxPlx.ApplicationHost.Api.csproj -c Release -r linux-arm -o build


FROM node:13-alpine3.10 AS frontendBuilder
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-frontend/ ./
RUN yarn install --frozen-lockfile --no-progress --silent
RUN yarn build


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim-arm32v7 AS runtime
ARG build
LABEL maintainer="Malte Rosenbjerg"
LABEL description="NxPlx"
LABEL repository="github.com/rosenbjerg/NxPlx"
RUN apt-get update && apt-get install -y ffmpeg imagemagick && apt-get autoremove && apt-get clean
WORKDIR /app
COPY --from=backendBuilder /app/build ./
COPY --from=frontendBuilder /app/build ./public/
ENV NXPLX_BUILD=$build
EXPOSE 5353
ENTRYPOINT ["dotnet", "NxPlx.ApplicationHost.Api.dll"]