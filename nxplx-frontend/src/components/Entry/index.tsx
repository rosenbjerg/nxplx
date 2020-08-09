import { Component, h } from "preact";
import { Link } from "preact-router";
import * as style from "./style.css";
import BlurhashCanvas from "../Blurhash/BlurhashCanvas";
import { useState } from "preact/hooks";

interface Props {
    key: number | string
    title: string
    href: string
    image: string
    imageBlurHash: string
    progress?: number
    autosizeOverride?: string
    children?: any
    blurhashWidth: number
    blurhashHeight: number
}

interface Props2 {
    src: string;
    blurhash: string;
    alt?: string;
    class?: string
    blurhashWidth: number
    blurhashHeight: number
}

export class LazyImage extends Component<Props2> {
    private mounted = false;
    private loadStarted = false;
    private image = new Image();

    componentDidMount() {
        this.mounted = true;
    }
    componentWillUnmount() {
        this.mounted = false;
        if (this.image) this.image.src = '';
    }

    render() {
        const [imageLoaded, setImageLoaded] = useState(false);
        if (!this.loadStarted) {
            this.image.addEventListener("load", () => {
                if (this.mounted) setImageLoaded(true);
            });
            this.image.src = this.props.src;
            this.loadStarted = true;
        }

        if (imageLoaded)
            return (<img class={this.props.class} src={this.props.src} alt={this.props.alt}/>);
        return (
            <BlurhashCanvas
                hash={this.props.blurhash || "LEHV6nWB2yk8pyo0adR*.7kCMdnj"}
                width={this.props.blurhashWidth}
                height={this.props.blurhashHeight}
                class={this.props.class}
                punch={1}
            />
        );
    }
}

const Entry = ({ href, image, imageBlurHash, key, progress, title, autosizeOverride, children, blurhashWidth, blurhashHeight }: Props) => {
    return (
        <Link key={key} class={style.link} title={title} href={href}>
            <LazyImage class={`${style.entry} ${autosizeOverride || style.autosize}`} alt={title} src={image} blurhash={imageBlurHash} blurhashHeight={blurhashHeight} blurhashWidth={blurhashWidth} />
            {children}
            {!!progress && (
                <span class={style.progress} style={{ "width": (progress * 100) + "%" }}>&nbsp;</span>
            )}
        </Link>
    );
};
export default Entry;
