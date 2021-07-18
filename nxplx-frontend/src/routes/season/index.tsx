import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import { EpisodeEntry } from "../../components/EpisodeEntry";
import Loading from "../../components/Loading";
import { toMap } from "../../utils/arrays";
import { formatInfoPair } from "../../utils/common";
import http from "../../utils/http";
import { imageUrl, SeasonDetails, SeriesDetails } from "../../utils/models";
import * as style from "./style.css";
import { LazyImage } from "../../components/Entry";
import AdminOnly from "../../components/AdminOnly";
import { EditDetails } from "../../components/EditDetails";
import { Link } from "preact-router";
import SelectPlaybackMode from "../../modals/SelectPlaybackMode";
import PageTitle from "../../components/PageTitle";

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
    showPlaybackModeSelector: boolean
}

export default class Season extends Component<Props, State> {
    public componentDidMount(): void {
        http.getJson<SeriesDetails>(`/api/series/${this.props.id}/${this.props.season}/detail`)
            .then(async seriesDetails => {
                const seasonDetails: SeasonDetails = seriesDetails.seasons[0];
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(seriesDetails.backdropPath, 1280)}");`;
                this.setState({ series: seriesDetails, season: seasonDetails, bg, bgImg: seriesDetails.backdropPath });

                const progressArray = await http.getJson<EpisodeProgress[]>(`/api/progress/season/${this.props.id}/${this.props.season}`);
                const progress = toMap(progressArray, p => p.fileId, p => p.progress);
                this.setState({ progress })
            });
    }

    public render(_, { series, season, bg, bgImg, progress, showPlaybackModeSelector }: State) {
        if (!series) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={bgImg}>
                <PageTitle title={`Season ${season.number} - ${series.name} - nxplx`}/>
                <div class={`nx-scroll ${style.content}`}>
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>
                            <button class="material-icons" style="border: none; padding: 0px 4px 0px 0px; font-size: 28pt" onClick={this.showPlayModeDiv}>play_arrow</button>
                            <Link style="color: white; text-decoration: none" href={`/series/${series.id}`}>{series.name}</Link> - Season {season.number}
                        </h2>
                        <AdminOnly>
                            <EditDetails setPoster entityType={"season"} entityId={season.id} />
                        </AdminOnly>
                    </div>

                    <LazyImage src={imageUrl(season.posterPath, 270, series.posterPath)} blurhash={season.posterBlurHash || series.posterBlurHash} blurhashHeight={32} blurhashWidth={20} class={style.poster}/>
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
                            .map(episode => (<EpisodeEntry key={episode.number} episode={episode} progress={progress && progress.get(episode.fileId)}/>))}
                    </div>
                </div>

                {showPlaybackModeSelector && (
                    <SelectPlaybackMode onDismiss={() => this.setState({ showPlaybackModeSelector: false })} seriesId={series.id} season={season.number}/>
                )}
            </div>
        );
    }

    private showPlayModeDiv = () => {
        this.setState({ showPlaybackModeSelector: true });
    }
}
