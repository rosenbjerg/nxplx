import { h } from "preact";
import { imageUrl, SeasonDetails, SeriesDetails } from "../../utils/models";
import Entry from "../Entry";
import * as style from "./style.css";

interface Props {
    key: number
    details: SeriesDetails
    season: SeasonDetails
    progress?: number
}

export const SeasonEntry = ({ details, season, progress }: Props) => (
    <Entry
        key={season.number}
        image={imageUrl(season.poster, 342, details.poster)}
        href={`/app/series/${details.id}/${season.number}`}
        title={`Season ${season.number}`}
        progress={progress}>
        <b class={style.num}>S{season.number}</b>
    </Entry>
);