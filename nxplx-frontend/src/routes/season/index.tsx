import orderBy from 'lodash/orderBy';
import { Component, h } from 'preact';
// @ts-ignore
import Helmet from 'preact-helmet';
import { Link } from "preact-router";
import { formatInfoPair } from '../../commonFilmInfo';
import { EpisodeStill } from "../../components/EpisodeStill";
import Loading from '../../components/loading';
import { EpisodeDetails, imageUrl, SeasonDetails, SeriesDetails } from "../../Details";
import http from '../../Http';
import * as style from './style.css';

interface Props { id:string, season:string }

interface State { series:SeriesDetails, season:SeasonDetails, bg:string, bgImg:string }

export default class Season extends Component<Props, State> {
    public componentDidMount() : void {
        Promise.all(
            [
                http.get(`/api/series/detail/${this.props.id}`).then(response => response.json()),
                http.get(`/api/series/detail/${this.props.id}/${this.props.season}`).then(response => response.json()),
                http.get(`/api/series/episodes/${this.props.id}/${this.props.season}`).then(response => response.json())
            ])
            .then(results => {
                const seasonNumber = parseInt(this.props.season);
                const seriesDetails : SeriesDetails = results[0];
                const seasonDetails : SeasonDetails = results[1];
                const episodeFiles = results[2];

                seasonDetails.episodes = episodeFiles.map(ef => {
                    const details = seasonDetails.episodes.find(e => e.episodeNumber === ef.episodeNumber);
                    if (details) { details.fid = ef.id; }
                    return details;
                }).filter(d => d && d.fid);


                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(seriesDetails.backdrop, 1280)}");`;
                this.setState({ series: seriesDetails, season: seasonDetails, bg, bgImg: seriesDetails.backdrop });
            });
    }


    public render(props:Props, { series, season, bg, bgImg }:State) {
        if (!series) {
            return (<Loading />);
        }
        return (
            <div class={style.bg} style={bg} data-bg={bgImg}>
                <Helmet title={`Season ${season.number} - ${series.name} - NxPlx`} />
                <div>
                    <h2 className={[style.title, style.marked].join(" ")}>{season.name}</h2>
                </div>
                <img className={style.poster} src={imageUrl(season.poster, 342)}
                     alt=""/>
                <span class={[style.info, style.marked].join(" ")}>
                    <div>
                        {
                            [
                                // {title: 'Overview', value: season.overview},
                                {title: 'Released', value: season.airDate.substr(0, 4)}
                            ].map(formatInfoPair)
                        }
                    </div>
                </span>
                <div>
                    {orderBy(season.episodes, ['episodeNumber'], ['asc'])
                        .map(episode =>
                            <EpisodeStill key={episode.episodeNumber} episode={episode} />
                        )}
                </div>
            </div>
        );
    }
}
