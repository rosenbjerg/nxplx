import http from '../http';


const ProgressClient = {
	getProgress: (kind: string, fid: string) => http.getJson<number>(`/api/progress/${kind}/${fid}`),
};

export default ProgressClient;