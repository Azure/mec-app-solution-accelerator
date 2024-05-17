import AttachedDataNetworkApiService from './AttachedDataNetworkApiService';
import AttachedDataNetworkInMemoryService from './AttachedDataNetworkInMemoryService';
import IAttachedDataNetworkService from './AttachedDataNetworkService';
import CameraApiService from './CameraApiService';
import CameraInMemoryService from './CameraInMemoryService';
import { ICameraService } from './CameraService';
import SimApiService from './SimApiService';
import SimInMemoryService from './SimInMemoryService';
import ISimService from './SimService1';

export type ServiceSettings = {
    apiKey: string | null,
    useInMemory: boolean
}

export class ServiceFactory {
    // InMemory services should be singleton, since the data it store on them.
    private static cameraInMemoryService = new CameraInMemoryService();
    private static simInMemoryService = new SimInMemoryService();
    private static attachedDataNetworkInMemoryService = new AttachedDataNetworkInMemoryService();

    static getCameraService(settings: ServiceSettings): ICameraService {
        return settings.useInMemory ? this.cameraInMemoryService : new CameraApiService({
            apiKey: settings.apiKey
        });
    }

    static getSimService(settings: ServiceSettings): ISimService {
        return settings.useInMemory ? this.simInMemoryService : new SimApiService({
            apiKey: settings.apiKey
        });
    }

    static getAttachedDataNetworkService(settings: ServiceSettings): IAttachedDataNetworkService {
        return settings.useInMemory ? this.attachedDataNetworkInMemoryService : new AttachedDataNetworkApiService({
            apiKey: settings.apiKey
        });
    }
}