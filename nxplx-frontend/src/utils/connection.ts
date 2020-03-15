import ReconnectingWebSocket from 'reconnecting-websocket';

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

    public static Get () {
        const w = window as any;
        if (w.__websocketmanager !== undefined) {
            return w.__websocketmanager;
        }
        return w.__websocketmanager = new WebsocketMessenger();
    }

    private webSocket:ReconnectingWebSocket;
    private connected = false;

    private unsent:Message[] = [];

    private handlers:{ [index:string]:MessageHandler[] } = {};

    constructor() {
        const protocol = location.protocol === 'http:' ? 'ws' : 'wss';
        const host = location.host !== 'localhost:8080' ? location.host : 'localhost:5353' ;
        this.webSocket = new ReconnectingWebSocket(`${protocol}://${host}/api/websocket/connect`);

        this.webSocket.addEventListener('open', this.onOpen);
        this.webSocket.addEventListener('close', this.onClose);
        this.webSocket.addEventListener('message', this.onMessage);
    }
    public send = (message:Message): void => {
        if (this.connected) {
            this.webSocket.send(JSON.stringify(message));
        } else {
            this.unsent.push(message);
        }
    };

    public subscribe = (type:string, handler:MessageHandler) => {
        let handlers = this.handlers[type];
        if (!handlers) handlers = this.handlers[type] = [];
        handlers.push(handler);
    };

    public unsubscribe = (type:string, handler:MessageHandler) => {
        const handlers = this.handlers[type] || [];
        handlers.splice(handlers.indexOf(handler), 1);
    };

    private onOpen = () => {
        console.log('connected');
        this.connected = true;
        this.unsent.forEach(this.send);
        this.unsent = [];
    };

    private onClose = () => {
        console.log('disconnected');
        this.connected = false;
    };

    private onMessage = (ev:MessageEvent) => {
        const data = ev.data;
        console.log('received', data);
    };
}
