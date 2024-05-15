import { AttachedDataNetwork } from '@/models/attachedDataNetwork';

interface IAttachedDataNetworkService {
    getAttacheDataNetwork(): Promise<AttachedDataNetwork>;
}

export default IAttachedDataNetworkService;