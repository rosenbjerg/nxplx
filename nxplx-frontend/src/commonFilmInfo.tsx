import { h, JSX } from "preact";
import * as style from "./routes/series/style.css";

interface InfoPair { title:string, value:any }



export function formatRunTime (time?:number[]|number) : string {
    if (!time) {
        return '';
    }
    if (Array.isArray(time)) {
        return `${time[0]} min ${(time[1] ? `& ${time[1]} sec` : '')}`
    }
    return `${time} min`;
}

export function formatProgress (time:number){
    const hours = Math.floor(time / 3600);
    const minutes = Math.floor((time % 3600) / 60);
    const seconds = Math.floor(time % 60);
    if (hours > 0) { return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`; }
    return `${minutes}:${seconds.toString().padStart(2, '0')}`; 
}

export function  formatInfoPair (ib:InfoPair):JSX.Element {
    return (
        <tr>
            <td class={style.infoKey}>{ib.title}</td>
            <td class={style.infoValue}>{ib.value}</td>
        </tr>
    );
}