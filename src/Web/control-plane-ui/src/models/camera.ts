import { SIM } from "./sim";

export enum CameraType {
    FiveG = "5G",
    FourG = "4G",
    Wifi = "Wi-fi",
    Ethernet = "Ethernet",
    LTE = "LTE",
    Container = "Container"
}

export type Camera = {
    id: string;
    model: string;
    type: CameraType;
    ip?: string;
    username?: string;
    password?: string;
    port?: string;
    simId?: string;
    rtsp?: string;
    hls?: string;
}