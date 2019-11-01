<img src="https://github.com/rosenbjerg/NxPlx/raw/master/nxplx-frontend/src/assets/images/nxplx-cropped-h120.png">

A home media vault designed for low-power devices such as Raspberry Pi 3/4, ODroid C2, ASUS TinkerBoard, and similar SBCs.

NxPlx is designed with deployment using Docker in mind

### Features
* [x] persist subtitle choice
* [x] persist watching progress
* [x] fast seeking
* [x] shaka-player

### Milestones
* [ ] ffprobe file analysis
* [ ] ffmpeg transcode profile generation
* [ ] transcode servers
* [ ] ffprobe file analysis
* [ ] chromecast support (through shaka-player)
* [ ] automated multi-arch docker build through GitHub Actions

### Goals
- (Very) low CPU usage during playback on multiple clients - fit for an SBC
- Fully responsive design for enjoyable use on everything from a smartphone to a laptop or desktop
- Automated transcoding of video files that are in a format that cannot be played directly on target device(s)
- Support for remote transcode servers, to offload the SBC


### Non-goals
- On-the-fly transcoding - low-power devices are not good at on-the-fly transcoding

#### Technologies used
- Docker
- RedHttpServer (ASP.NET Core)
- Postgres /w Entity Framework Core
- Redis
- Preact


### Contribute
If you find this project interesting, please star it :)
Contributions in the form of PRs are welcome!
