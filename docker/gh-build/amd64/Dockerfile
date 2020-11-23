FROM alpine AS gitCloner
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /project
RUN git clone https://github.com/rosenbjerg/NxPlx.git


FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS backendBuilder
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-backend/ ./
RUN dotnet publish NxPlx.ApplicationHost.Api/NxPlx.ApplicationHost.Api.csproj -c Release -r linux-musl-x64 -o build


FROM node:13-alpine AS frontendBuilder
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /app
COPY --from=gitCloner /project/NxPlx/nxplx-frontend/ ./
RUN yarn install --frozen-lockfile --no-progress --silent
RUN yarn build


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine-amd64 AS runtime
ARG build
LABEL maintainer="Malte Rosenbjerg"
LABEL description="NxPlx"
LABEL repository="github.com/rosenbjerg/NxPlx"
RUN apk add --update --no-cache ffmpeg imagemagick
RUN apk add --update --no-cache libgdiplus --repository=http://dl-cdn.alpinelinux.org/alpine/edge/testing
WORKDIR /app
COPY --from=backendBuilder /app/build ./
COPY --from=frontendBuilder /app/build ./public/
ENV NXPLX_BUILD=$build
EXPOSE 5353
ENTRYPOINT ["dotnet", "NxPlx.ApplicationHost.Api.dll"]