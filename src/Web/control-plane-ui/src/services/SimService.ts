import { SIM, SimGroup, SimPolicy } from '@/models/sim';

interface ISimService {
    listSims(): Promise<SIM[]>;
    listSimGroups(): Promise<SimGroup[]>;
    listSimPolicies(): Promise<SimPolicy[]>;
    createSim(sim: SIM): Promise<SIM>;
    deleteSim(sim: SIM): Promise<boolean>;
}

export default ISimService;