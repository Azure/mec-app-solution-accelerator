import { AttachedDataNetwork } from '@/models/attachedDataNetwork';
import IAttachedDataNetworkService from './AttachedDataNetworkService';

export type AttachedDataNetworkApiSettings = {
    apiKey: string | null
};

class AttachedDataNetworkApiService implements IAttachedDataNetworkService {
    private apiKey: string | null;
    constructor(settings: AttachedDataNetworkApiSettings) {
        this.apiKey = settings.apiKey;
    }

    public async getAttacheDataNetwork(): Promise<AttachedDataNetwork> {
        const response = await fetch(`/api/v1/attachedDataNetwork`, {
            headers: this.apiKeyHeader()
        });
        const attachedDataNetwork = await response.json() as AttachedDataNetwork;
        return attachedDataNetwork;
    }

    private apiKeyHeader(): Record<string, string> {
        if (this.apiKey) {
            return { 'api-key': this.apiKey }
        }

        return {};
    }
}

export default AttachedDataNetworkApiService;