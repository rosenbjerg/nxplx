export interface EpisodeInfo { id:number; eid:number; number:number; subtitles:string[]; thumbnail:string }

export interface SeasonInfo { number:number; episodes:EpisodeInfo[]; poster:string }

export interface SeriesInfo { id:number; name:string; seasons:SeasonInfo[]; poster:string }

export interface FilmInfo { id:number; eid:number; title:string; subtitles:string[]; poster:string }