export type imagesize = 100 | 190 | 260 | 270 | 1280;
export const imageUrl = (url: string, width: imagesize, fallbackUrl?: string) => {
	if (!url && !fallbackUrl) return `/assets/images/w${width}.jpg`;
	return `/api/image/w${width}/${url ?? fallbackUrl}`;
};

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
	iso31661: string;
	name: string;
}

export interface SpokenLanguage {
	iso6391: string;
	name: string;
}

export interface FilmDetails {
	id: number;
	fid: number;
	title: string;
	posterPath: string;
	posterBlurHash: string;
	backdropPath: string;
	backdropBlurHash: string;
	subtitles: SubtitleFile[];
	budget: number;
	imdbId: string;
	belongsToCollection: MovieCollection;
	originalTitle: string;
	productionCountries: ProductionCountry[];
	releaseDate: string;
	revenue: number;
	runtime: number;
	spokenLanguages: SpokenLanguage[];
	tagline: string;
	genres: Genre[];
	originalLanguage: string;
	overview: string;
	popularity: number;
	productionCompanies: ProductionCompany[];
	voteAverage: number;
	voteCount: number;
}

export interface SubtitleFile {
	id: string;
	language: string;
}

export interface MovieCollection {
	id: number;
	name: string;
	posterPath: string;
	posterBlurHash: string;
	backdropPath: string;
	backdropBlurHash: string;
	movies: OverviewElement[];
}

export interface Library {
	id: number;
	name: string;
	kind: string;
	language: string;
	path?: string;
}

export interface User {
	sortOrder: number;
	id: number;
	username: string;
	email: string;
	admin: boolean;
	hasChangedPassword: boolean;
	isOnline: boolean;
	lastSeen: string;
	libraries: number[];
}

export interface OverviewElement {
	id: number;
	title: string;
	posterPath: string;
	posterBlurHash: string;
	kind: 'film' | 'series';
}

export interface ContinueWatchingElement {
	fileId: number;
	title: string;
	posterPath: string;
	posterBlurHash: string;
	kind: string;
	watched: string;
	progress: number;
}

export interface FileInfo {
	id: number;
	filePath: string;
	duration: number;
	title: string;
	posterPath: string;
	posterBlurHash: string;
	backdropPath: string;
	backdropBlurHash: string;
	subtitles: string[];
	seriesId?: number;
	seasonNo?: number;
}

export interface Creator {
	name: string;
}

export interface Network {
	name: string;
	logoPath: string;
	logoBlurHash: string;
	originCountry: string;
}

export interface SeriesDetails {
	id: number;
	fid: number;
	backdropPath: string;
	backdropBlurHash: string;
	posterPath: string;
	posterBlurHash: string;
	voteAverage: number;
	voteCount: number;
	name: string;
	networks: Network[];
	genres: Genre[];
	createdBy: Creator[];
	productionCompanies: ProductionCompany[];
	overview: string;
	seasons: SeasonDetails[];
}

export interface SeasonDetails {
	airDate: string;
	id: number;
	name: string;
	overview: string;
	posterPath: string;
	posterBlurHash: string;
	number: number;
	episodes?: EpisodeDetails[];
}

export interface EpisodeDetails {
	name: string;
	fileId: number;
	number: number;
	overview: string;
	airDate: string;
	stillPath: string;
	stillBlurHash: string;
	voteAverage: number;
	voteCount: number;
}

export const round = (num: number) => Math.round(num * 100) / 100;
