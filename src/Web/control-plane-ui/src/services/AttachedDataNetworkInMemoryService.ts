import { AttachedDataNetwork } from '@/models/attachedDataNetwork';
import IAttachedDataNetworkService from './AttachedDataNetworkService';

class AttachedDataNetworkInMemoryService implements IAttachedDataNetworkService {
    private data: AttachedDataNetwork = {
        name: 'AttachedDataNetwork',
        staticIpPool: '192.0.2.128/25'
    };

    public async getAttacheDataNetwork(): Promise<AttachedDataNetwork> {
        this.delay();
        return this.data;
    }

    // Delay to simulate API call
    private delay(): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, 1000));
    }
}

export default AttachedDataNetworkInMemoryService;