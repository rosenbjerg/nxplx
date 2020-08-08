import { h } from "preact";
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

export const LazyImage = (props: Props2) => {
    const [imageLoaded, setImageLoaded] = useState(false);
    const [loadStarted, setLoadStarted] = useState(false);
    const [img] = useState(new Image());
    if (!loadStarted) {
        img.addEventListener('load', () => {
            setLoadStarted(true);
            setImageLoaded(true);
        });
        img.src = props.src;
    }

    if (imageLoaded)
        return (<img class={props.class} src={props.src} alt={props.alt}/>);
    return (
        <BlurhashCanvas
            hash={props.blurhash || 'LEHV6nWB2yk8pyo0adR*.7kCMdnj'}
            width={props.blurhashWidth}
            height={props.blurhashHeight}
            class={props.class}
            punch={1}
        />
    );
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
