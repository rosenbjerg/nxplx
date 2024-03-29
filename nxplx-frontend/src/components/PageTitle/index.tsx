import { useEffect } from 'preact/hooks';

export default function PageTitle({ title }: { title: string }) {
	useEffect(() => {
		document.title = `${title} - nxplx`;
	}, [title]);
	return null;
}
