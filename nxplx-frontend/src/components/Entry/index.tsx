import { h } from "preact";
import { Link } from "preact-router";
import * as style from "./style.css";

interface Props {
    key: number | string
    title: string
    href: string
    image: string
    progress?: number
    autosizeOverride?: string
    children?: any
}

const Entry = ({ href, image, key, progress, title, autosizeOverride, children }: Props) => {
    return (
        <Link key={key} class={style.link} title={title} href={href}>
            <img class={`${style.entry} ${autosizeOverride || style.autosize}`} src={image} alt={title}/>
            {children}
            {!!progress && (
                <span class={style.progress} style={{ "width": (progress * 100) + "%" }}>&nbsp;</span>
            )}
        </Link>
    );
};
export default Entry;
