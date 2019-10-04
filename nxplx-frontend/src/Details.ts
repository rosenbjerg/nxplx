
export const imageUrl = (url:string, width:154|185|342|1280) => `/api/images/w${width}${url}`;

export interface Genre {
    id: number;
    name: string;
}

export interface ProductionCompany {
    id: number;
    logoPath: string;
    name: string;
    originCountry: string;
}

export interface ProductionCountry {
    iso3166_1: string;
    name: string;
}

export interface SpokenLanguage {
    iso639_1: string;
    name: string;
}
export interface FilmDetails {
    id:number
    fid:number
    title:string
    poster:string
    backdrop:string
    subtitles:SubtitleFile[]
    budget:number
    imdbId:string
    belongsToCollection:MovieCollection
    originalTitle:string
    productionCountries:ProductionCountry[]
    releaseDate:string
    revenue:number
    runtime:number
    spokenLanguages:SpokenLanguage[]
    tagline:string
    genres:Genre[]
    originalLanguage:string
    overview:string
    popularity:number
    productionCompanies:ProductionCompany[]
    voteAverage:number
    voteCount:number
}
export interface SubtitleFile {
    id:string
    language:string
}
export interface MovieCollection {
    id:number
    name:string
    poster:string
    backdrop:string
}

export interface Creator {
    id: number
    creditId: string
    name: string
    profilePath: string
}

export interface Network {
    name: string
    id: number
    logoPath: string
    originCountry: string
}
export interface SeriesDetails {
    id: number;
    fid: number;
    backdrop: string;
    poster: string;
    voteAverage: number;
    voteCount: number;
    name: string;
    networks: Network[];
    genres: Genre[];
    createdBy: Creator[];
    productionCompanies: ProductionCompany[];
    overview: string;
    seasons: SeasonLiteDetails[];
}
export interface SeasonLiteDetails {
    airDate: string
    id: number
    name: string
    overview: string
    poster: string
    number: number
}
export interface SeasonDetails extends SeasonLiteDetails {
    episodes: EpisodeDetails[]
}

export interface EpisodeDetails {
    name: string
    fileId: number
    number: number
    overview: string;
    airDate: string;
    still: string;
    voteAverage: number;
    voteCount: number;
}
export interface Info {
    id:number
    fid:number
    title:string
    poster:string
    backdrop:string
    subtitles:string[]
}

export const round = (num:number) => Math.round(num * 100) / 100;