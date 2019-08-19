import { Component, h } from 'preact';

import videojs from 'video.js';
import 'videojs-chromecast/dist/videojs-chromecast';
import 'videojs-chromecast/dist/videojs-chromecast.css';

export default class Video extends Component {
    private videoNode: any;
    private player?: videojs.Player;
    public componentDidMount() {
        // instantiate Video.js
        const {children, ...props} = this.props;
        // @ts-ignore
        this.player = videojs(this.videoNode, props, () => {
            console.log('onPlayerReady', this)
        });
    }

    // destroy player on unmount
    public componentWillUnmount() {
        if (this.player) {
            this.player.dispose()
        }
    }
    
    public render() {
        return (
            <div>
                <div data-vjs-player>
                    <video ref={this.setRef} className="video-js"/>
                </div>
            </div>
        )
    }

    private setRef = (ref:any) => this.videoNode = ref;
}
