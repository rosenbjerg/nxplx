import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import Helmet from "preact-helmet";
import { EpisodeEntry } from "../../components/EpisodeEntry";
import Loading from "../../components/Loading";
import { toMap } from "../../utils/arrays";
import { formatInfoPair } from "../../utils/common";
import http from "../../utils/http";
import { imageUrl, SeasonDetails, SeriesDetails } from "../../utils/models";
import * as style from "./style.css";

interface EpisodeProgress {
    fileId: number
    progress: number
}

interface Props {
    id: string,
    season: string
}

interface State {
    series: SeriesDetails,
    season: SeasonDetails,
    bg: string,
    bgImg: string
    progress?: Map<number, number>
}

export default class Season extends Component<Props, State> {
    public componentDidMount(): void {
        http.getJson(`/api/series/detail/${this.props.id}/${this.props.season}`)
            .then(async (seriesDetails: SeriesDetails) => {
                const seasonDetails: SeasonDetails = seriesDetails.seasons[0];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(seriesDetails.backdrop, 1280)}");`;
                this.setState({ series: seriesDetails, season: seasonDetails, bg, bgImg: seriesDetails.backdrop });

                const progressArray: EpisodeProgress[] = await http.getJson(`/api/progress/season/${this.props.id}/${this.props.season}`);
                const progress = toMap(progressArray, p => p.fileId, p => p.progress);
                this.setState({ progress })
            });
    }

    public render(_, { series, season, bg, bgImg, progress }: State) {
        if (!series) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={bgImg}>
                <Helmet title={`Season ${season.number} - ${series.name} - NxPlx`}/>
                <div class={`nx-scroll ${style.content}`}>
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{series.name} - Season {season.number}</h2>
                    </div>
                    <img class={style.poster} src={imageUrl(season.poster, 500)} alt=""/>
                    <span class={[style.info, style.marked].join(" ")}>
                    <table>
                        {
                            [
                                { title: "Released", value: season.airDate && season.airDate.substr(0, 4) || "?" }
                            ].map(formatInfoPair)
                        }
                    </table>
                </span>
                    <div>
                        {orderBy(season.episodes, ["number"], ["asc"])
                            .map(episode => (<EpisodeEntry key={episode.number} episode={episode} progress={progress?.get(episode.fileId)}/>))}
                    </div>
                </div>
            </div>
        );
    }
}
