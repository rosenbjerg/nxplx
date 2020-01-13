export type MessageHandler = (msg: Message) => void;

export interface Message {
    type:string
    data:any
}

export interface Connection {
    send(message:Message):void

    subscribe(type:string, handler:(msg:Message) => void)
    unsubscribe(type:string, handler:(msg:Message) => void)
}

export default class WebsocketMessenger implements Connection {
    private webSocket:WebSocket;
    private connected = false;

    private unsent:Message[] = [];

    private handlers:{ [index:string]:MessageHandler[] } = {};

    constructor() {
        this.webSocket = new WebSocket('/api/broadcast');
        this.webSocket.addEventListener('open', this.onOpen);
        this.webSocket.addEventListener('close', this.onClose);
        this.webSocket.addEventListener('message', this.onMessage);
    }

    public send(message:Message): void {
        if (this.connected) {
            this.webSocket.send(JSON.stringify(message));
        } else {
            this.unsent.push(message);
        }
    }

    public subscribe(type:string, handler:MessageHandler) {
        let handlers = this.handlers[type];
        if (!handlers) handlers = this.handlers[type] = [];
        handlers.push(handler);
    }

    public unsubscribe(type:string, handler:MessageHandler) {
        const handlers = this.handlers[type] || [];
        handlers.splice(handlers.indexOf(handler), 1);
    }

    private onOpen() {
        this.connected = true;
        const unsent = [ ...this.unsent ];
        this.unsent = [];
        unsent.forEach(this.send);
    }

    private onClose() {
        this.connected = false;
    }

    private onMessage(ev:MessageEvent) {
        const data = ev.data;
        console.log('received', data);
    }
}