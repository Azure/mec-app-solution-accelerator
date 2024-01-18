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
    port: string;
    sim?: SIM;
    rtsp?: string;
    hls?: string;
}