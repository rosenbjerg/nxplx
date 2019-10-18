# NxPlx

A home media vault designed for low-power devices such as Raspberry Pi 3/4, ODroid C2, ASUS TinkerBoard, and similar SBCs.

NxPlx is designed with deployment using Docker in mind

### Features
* [x] persist subtitle choice
* [x] persist watching progress
* [x] fast seeking

### Milestones
* [ ] ffprobe file analysis
* [ ] ffmpeg transcode profile generation
* [ ] transcode servers
* [ ] ffprobe file analysis
* [ ] shaka-player
* [ ] chromecast support

### Non-goals
- On-the-fly transcoding - low-power devices 

#### Technologies used
- RedHttpServer (ASP.NET Core)
- Postgres /w Entity Framework Core
- Redis
- Preact
