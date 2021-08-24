import { translate } from "../../utils/localisation";
import * as style from "./style.css";
import Entry from "../Entry";
import { ContinueWatchingElement, imageUrl } from "../../utils/models";
import { h } from "preact";
import { useEffect, useState } from "preact/hooks";
import http from "../../utils/http";

const ContinueWatching = () => {
    const [elements, setElements] = useState(Array<ContinueWatchingElement>());
    useEffect(() => {
        void http.getJson<ContinueWatchingElement[]>("/api/progress/continue")
            .then(data => setElements(data));
    }, []);
    if (elements.length < 1) {
        return null;
    }
    return (
        <div>
            <label>{translate("continue watching")}</label>
            <div class={`nx-scroll ${style.continueWatchingContainer}`}>
                {elements.map(p => (
                    <Entry
                        key={`${p.kind[0]}:${p.fileId}`}
                        title={p.title}
                        href={`/watch/${p.kind}/${p.fileId}`}
                        image={imageUrl(p.posterPath, 190)}
                        imageBlurhash={p.posterBlurHash}
                        progress={p.progress}
                        blurhashWidth={20}
                        blurhashHeight={32}
                    />
                ))}
            </div>
        </div>
    );
}
export default ContinueWatching;