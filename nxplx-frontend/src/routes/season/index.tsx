import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import Helmet from "preact-helmet";
import Entry from "../../components/Entry";
import Loading from "../../components/Loading";
import { formatInfoPair } from "../../utils/common";
import http from "../../utils/http";
import { imageUrl, SeasonDetails, SeriesDetails } from "../../utils/models";
import * as style from "./style.css";

interface Props {
    id: string,
    season: string
}

interface State {
    series: SeriesDetails,
    season: SeasonDetails,
    bg: string,
    bgImg: string
}

export default class Season extends Component<Props, State> {
    public componentDidMount(): void {
        http.get(`/api/series/detail/${this.props.id}/${this.props.season}`)
            .then(response => response.json())
            .then((seriesDetails: SeriesDetails) => {
                const seasonDetails: SeasonDetails = seriesDetails.seasons[0];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(seriesDetails.backdrop, 1280)}");`;
                this.setState({ series: seriesDetails, season: seasonDetails, bg, bgImg: seriesDetails.backdrop });
            });
    }


    public render(_, { series, season, bg, bgImg }: State) {
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
                    <img class={style.poster} src={imageUrl(season.poster, 500)}
                         alt=""/>
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
                            .map(episode => (<Entry
                                key={episode.number}
                                title={episode.name}
                                href={`/watch/series/${episode.fileId}`}
                                image={imageUrl(episode.still, 300)}
                            />))}
                    </div>
                </div>
            </div>
        );
    }
}
