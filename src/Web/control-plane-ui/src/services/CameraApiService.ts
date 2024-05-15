import { Camera } from '@/models/camera';
import { ICameraService } from './CameraService';

export type CameraApiServiceSettings = {
    apiKey: string | null
};

class CameraApiService implements ICameraService {
    private apiKey: string | null;
    constructor(settings: CameraApiServiceSettings) {
        this.apiKey = settings.apiKey;
    }

    public async listCameras(): Promise<Camera[]> {
        const response = await fetch(`/api/v1/cameras`, {
            headers: this.apiKeyHeader()
        });
        const cameras = await response.json() as Camera[];
        return cameras;
    }

    public async createCamera(camera: Camera): Promise<Camera> {
        const response = await fetch(`/api/v1/cameras`, {
            method: 'POST',
            headers: {
                ...this.apiKeyHeader(),
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
            headers: this.apiKeyHeader(),
            method: 'DELETE',
        });

        return response.ok;
    }

    private apiKeyHeader(): Record<string, string> {
        if (this.apiKey) {
            return { 'api-key': this.apiKey }
        }

        return {};
    }
}

export default CameraApiService;