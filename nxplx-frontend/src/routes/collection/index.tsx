import orderBy from "lodash/orderBy";
import { Component, h } from "preact";
import Entry, { LazyImage } from "../../components/Entry";
import Loading from "../../components/Loading";
import http from "../../utils/http";
import { imageUrl, MovieCollection } from "../../utils/models";
import * as style from "./style.css";
import AdminOnly from "../../components/AdminOnly";
import { EditDetails } from "../../components/EditDetails";
import PageTitle from "../../components/PageTitle";

interface Props {
    id: string
}

interface State {
    details: MovieCollection,
    bg: string
}

export default class Collection extends Component<Props, State> {
    public componentDidMount(): void {
        http.get(`/api/film/collection/${this.props.id}/details`)
            .then(response => response.json())
            .then((details: MovieCollection) => {
                const bg = `background-image: linear-gradient(rgba(0, 0, 0, 0.6), rgba(0, 0, 0, 0.4), rgba(0, 0, 0, 0.6)), url("${imageUrl(details.backdropPath, 1280)}");`;
                this.setState({ details, bg });
            });
    }


    public render(_, { details, bg }: State) {
        if (!details) {
            return (<Loading fullscreen/>);
        }
        return (
            <div class={style.bg} style={bg} data-bg={details.backdropPath}>
                <PageTitle title={`${details.name} - nxplx`}/>
                <div class={`nx-scroll ${style.content}`}>
                    <div>
                        <h2 class={[style.title, style.marked].join(" ")}>{details.name}</h2>
                        <AdminOnly>
                            <EditDetails setPoster setBackdrop entityType={"collection"} entityId={details.id} />
                        </AdminOnly>
                    </div>
                    <LazyImage class={style.poster} src={imageUrl(details.posterPath, 270)} blurhash={details.posterBlurHash} blurhashHeight={32} blurhashWidth={20}/>
                    <div>
                        {orderBy(details.movies, ["year", "title"], ["asc"])
                            .map(movie => (
                                    <Entry
                                        key={movie.id}
                                        title={movie.title}
                                        href={`/${movie.kind}/${movie.id}`}
                                        image={imageUrl(movie.posterPath, 190, details.posterPath)}
                                        imageBlurHash={movie.posterBlurHash || details.posterBlurHash}
                                        blurhashWidth={20}
                                        blurhashHeight={32}
                                    />
                                )
                            )}
                    </div>
                </div>
            </div>
        );
    }
}
