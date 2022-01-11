import { Component, h } from 'preact';
import { useState } from 'preact/hooks';
import { useInView } from 'react-intersection-observer';
import BlurHashCanvas from '../BlurHash';

interface Props {
	src: string;
	blurhash: string;
	alt?: string;
	class?: string;
	blurhashWidth: number;
	blurhashHeight: number;
}

const intersectionOptions = {
	threshold: 0.0,
};

export default class LazyImage extends Component<Props> {
	private mounted = false;
	private loadStarted = false;
	private image = new Image();
	1;

	componentDidMount() {
		this.mounted = true;
	}

	componentWillUnmount() {
		this.mounted = false;
		if (this.image) this.image.src = '';
	}

	render({ alt, blurhash, blurhashHeight, blurhashWidth, class: className, src }: Props) {
		const [imageLoaded, setImageLoaded] = useState(false);

		if (imageLoaded)
			return (<img class={className} src={src} alt={alt} />);

		const [ref, inView, _] = useInView(intersectionOptions);
		if (inView && !this.loadStarted) {
			this.image.addEventListener('load', () => {
				if (this.mounted) setImageLoaded(true);
			});
			this.image.src = src;
			this.loadStarted = true;
		}

		return (
			<BlurHashCanvas
				setRef={ref}
				hash={blurhash || 'LEHV6nWB2yk8pyo0adR*.7kCMdnj'}
				width={blurhashWidth}
				height={blurhashHeight}
				class={className}
				punch={1}
			/>
		);
	}
}