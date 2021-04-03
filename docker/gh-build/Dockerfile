ARG BASE_IMAGE

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS backendBuilder
WORKDIR /app
COPY ./nxplx-backend/ ./
ARG DOTNET_RID
RUN dotnet publish NxPlx.ApplicationHost.Api/NxPlx.ApplicationHost.Api.csproj -c Release -r $DOTNET_RID -o build



FROM node:14-alpine AS frontendBuilder
RUN apk --update --no-cache --quiet add git openssh
WORKDIR /app
COPY ./nxplx-frontend/ ./
RUN yarn install --frozen-lockfile --no-progress --silent
RUN yarn build



FROM $BASE_IMAGE AS runtime
LABEL maintainer="rosenbjerg"
LABEL description="NxPlx"
LABEL repository="github.com/rosenbjerg/NxPlx"
RUN apk add -q --update --no-cache ffmpeg imagemagick icu-libs
RUN apk add -q --update --no-cache libgdiplus --repository=http://dl-cdn.alpinelinux.org/alpine/edge/testing
ARG TINI_VERSION=v0.19.0
ARG TINI_BUILD
RUN wget -O /tini https://github.com/krallin/tini/releases/download/${TINI_VERSION}/${TINI_BUILD} && chmod +x /tini
ENTRYPOINT ["/tini", "-g", "--"]

RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

EXPOSE 5353
ARG build
ENV NXPLX_BUILD=$build
WORKDIR /app
COPY --from=backendBuilder /app/build ./
COPY --from=frontendBuilder /app/build ./public/
CMD ["./NxPlx.ApplicationHost.Api"]