import { SIM } from "./sim";

export enum CameraType {
    FiveG = "5G",
    FourG = "4G",
    Wifi = "Wifi",
    LTE = "LTE",
    Process = "Process"
}

export type Camera = {
    id: string;
    model: string;
    type: CameraType;
    ip: string;
    username?: string;
    password?: string;
    port?: string;
    simId?: string;
    rtsp?: string;
    hls?: string;
}