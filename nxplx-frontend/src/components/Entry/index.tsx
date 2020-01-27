import { h } from "preact";
import { Link } from "preact-router";
import * as style from './style.css';

interface Props {
    key: number|string
    title: string
    href: string
    image: string
    progress?: number
}

const Entry = ({ href, image, key, progress, title }: Props) => {
    return (
        <Link key={key} class={style.link} title={title} href={href}>
            <img class={style.entry} src={image} alt={title}/>
            {progress && (
                <span class={style.progress} style={{ 'width': (progress * 100) + '%'}}>&nbsp;</span>
            )}
        </Link>
    );
};
export default Entry;