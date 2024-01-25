import { Camera, CameraType } from '@/models/camera';

class CameraService {
    public async listCameras(): Promise<Camera[]> {
        const response = await fetch(`/api/v1/cameras`);
        const cameras = await response.json() as Camera[];
        console.log(cameras);
        return cameras;
    }

    public async createCamera(camera: Camera): Promise<Camera> {
        const response = await fetch(`/api/v1/cameras`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(camera),
        });

        if (!response.ok) {
            throw new Error('Failed to create camera');
        }

        return await response.json() as Camera;
    }

    public async deleteCamera(cameraId: string): Promise<boolean> {
        const response = await fetch(`/api/v1/cameras/${cameraId}`, {
            method: 'DELETE',
        });

        return response.ok;
    }
}

const cameraService = new CameraService();
export default cameraService;