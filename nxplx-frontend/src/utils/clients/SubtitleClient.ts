import http from '../http';


const SubtitleClient = {
	getPreference: (kind: string, fid: string) => http.getJson<string>(`/api/subtitle/preference/${kind}/${fid}`),
	setPreference: (kind: string, fid: string, language: string) => http.put(`/api/subtitle/preference/${kind}/${fid}`, language),
	getAvailableLanguages: (kind: string, fid: string) => http.getJson<string[]>(`/api/subtitle/languages/${kind}/${fid}`),
};

export default SubtitleClient;