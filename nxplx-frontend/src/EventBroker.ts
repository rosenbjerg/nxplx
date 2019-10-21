import { h } from "preact";

type ShakaMessage = 'state_changed'|'volume_changed'|'muted'|'subtitle_changed'

function CreateEventBroker() : EventBroker {
    const allHandlers:{} = {};

    const publish = (event:ShakaMessage, data:any) => {
        setTimeout(() => {
            const handlers = allHandlers[event] || [];
            handlers.forEach(handler => handler(data))
        }, 0);
    };

    const subscribe = (event:ShakaMessage, handler:(data:any)=>void) => {
        let handlers = allHandlers[event];
        if (!handlers) {
            allHandlers[event] = handlers = [];
        }
        handlers.push(handler);
    };

    publish.subscribe = subscribe;
    return publish;
}

interface EventBroker {
    (event:ShakaMessage, data:any):void
    subscribe(event:ShakaMessage, handler:(data:any)=>void):void
}

export default CreateEventBroker;
