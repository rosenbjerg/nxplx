type ShakaMessage = 'state_changed'|'volume_changed'|'muted'|'subtitle_changed'|'time_changed'

function CreateEventBroker() : Events {
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

export interface Events {
    (event:ShakaMessage, data:any):void
    subscribe<TData>(event:ShakaMessage, handler:(data:TData)=>void):void
}

export default CreateEventBroker;
