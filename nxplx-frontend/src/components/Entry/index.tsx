import { h } from "preact";
import { Link } from "preact-router";
import * as style from "./style.css";
import LazyImage from "../LazyImage";

interface Props {
    key: number | string
    title: string
    href: string
    image: string
    imageBlurhash: string
    progress?: number
    autosizeOverride?: string
    children?: any
    blurhashWidth: number
    blurhashHeight: number
}

const Entry = ({ href, image, imageBlurhash, key, progress, title, autosizeOverride, children, blurhashWidth, blurhashHeight }: Props) => {
    return (
        <Link key={key} class={style.link} title={title} href={href}>
            <LazyImage class={`${style.entry} ${autosizeOverride || style.autosize}`} alt={title} src={image} blurhash={imageBlurhash} blurhashHeight={blurhashHeight} blurhashWidth={blurhashWidth} />
            {children}
            {!!progress && (
                <span class={style.progress} style={{ "width": (progress * 100) + "%" }}>&nbsp;</span>
            )}
        </Link>
    );
};
export default Entry;
