<img src="https://github.com/rosenbjerg/NxPlx/raw/master/nxplx-frontend/src/assets/images/nxplx-cropped-h120.png">

A home media vault designed for low-power devices such as Raspberry Pi 3/4, ODroid C2, ASUS TinkerBoard, and similar SBCs.

[![Docker Pulls](https://img.shields.io/docker/pulls/mrosenbjerg/nxplx-webapi)](https://hub.docker.com/r/mrosenbjerg/nxplx-webapi)
[![GitHub](https://img.shields.io/github/license/rosenbjerg/NxPlx)](https://github.com/rosenbjerg/NxPlx/blob/master/LICENSE)
[![DeepScan grade](https://deepscan.io/api/teams/7497/projects/9582/branches/126538/badge/grade.svg)](https://deepscan.io/dashboard#view=project&tid=7497&pid=9582&bid=126538)
[![CodeFactor](https://www.codefactor.io/repository/github/rosenbjerg/agentdeploy/badge/main)](https://www.codefactor.io/repository/github/rosenbjerg/nxplx/overview/master)

NxPlx is designed with deployment using Docker in mind

### Features
* [x] persist subtitle choice
* [x] persist watching progress
* [x] fast seeking
* [x] blurhash image placeholder
* [x] automated [multi-arch docker](https://hub.docker.com/r/mrosenbjerg/nxplx-webapi/tags) build through GitHub Actions
* [x] ffprobe file analysis
* [x] snapshot creatin using ffmpeg

### Milestones
* [ ] ffmpeg transcode profile generation
* [ ] transcode servers
* [ ] chromecast support

### Goals
- (Very) low CPU usage during playback on multiple clients - fit for an SBC
- Fully responsive design for enjoyable use on everything from a smartphone to a laptop or desktop
- Automated transcoding of video files that are in a format that cannot be played directly on target device(s)
- Support for remote transcode servers, to offload the SBC


### Non-goals
- On-the-fly transcoding - low-power devices are not good at on-the-fly transcoding

#### Technologies used
- Docker
- ASP.NET Core
- Postgres /w Entity Framework Core
- Redis
- Preact


### Contribute
If you find this project interesting, please star it :)

Contributions in the form of PRs are welcome!

### Thanks to
<img height="100" src="https://www.themoviedb.org/assets/2/v4/logos/408x161-powered-by-rectangle-blue-10d3d41d2a0af9ebcb85f7fb62ffb6671c15ae8ea9bc82a2c6941f223143409e.png"/>
<img height="50" src="https://upload.wikimedia.org/wikipedia/commons/5/5f/FFmpeg_Logo_new.svg"/>
<img height="50" src="https://upload.wikimedia.org/wikipedia/commons/thumb/0/0d/Imagemagick-logo.png/250px-Imagemagick-logo.png"/>
