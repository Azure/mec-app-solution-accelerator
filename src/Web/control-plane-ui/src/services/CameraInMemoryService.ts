import { Camera, CameraType } from '@/models/camera';
import { ICameraService } from './CameraService';

class CameraInMemoryService implements ICameraService {
    private cameras: Camera[] = [
        {
            id: 'Id 1',
            model: 'Model 1',
            type: CameraType.FiveG,
            ip: '10.0.0.1',
            port: '1234',
            hls: 'http://127.0.0.1:3001/stream.m3u8',
        }, {
            id: 'Id 2',
            model: 'Model 2',
            type: CameraType.FiveG,
            ip: '10.0.0.1',
            port: '1234',
            hls: 'http://127.0.0.1:3001/stream.m3u8',
        }
    ];

    public async listCameras(): Promise<Camera[]> {
        await this.delay();
        return this.cameras;
    }

    public async createCamera(camera: Camera): Promise<Camera> {
        await this.delay();
        this.cameras = [...this.cameras, camera];
        return camera;
    }

    public async deleteCamera(cameraId: string): Promise<boolean> {
        const indexToDelete = this.cameras.findIndex(c => c.id === cameraId);
        if (indexToDelete === -1) return false;
        this.cameras = [...this.cameras.filter((_, index) => indexToDelete !== index)];
        return true;
    }

    // Delay to simulate API call
    private delay(): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, 250));
    }
}

export default CameraInMemoryService;