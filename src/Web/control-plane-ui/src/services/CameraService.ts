import { Camera } from "@/models/camera";

export interface ICameraService {
    listCameras(): Promise<Camera[]>;
    createCamera(camera: Camera): Promise<Camera>;
    deleteCamera(cameraId: string): Promise<boolean>;
}