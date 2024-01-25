import { SIM, SimGroup } from '@/models/sim';

class SimService {
    public async listSims(): Promise<SIM[]> {
        const response = await fetch(`/api/v1/sims`);
        const sims = await response.json() as SIM[];
        console.log(sims);
        return sims;
    }

    public async listSimGroups(): Promise<SimGroup[]> {
        const response = await fetch(`/api/v1/simGroups`);
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async listSimPolicies(): Promise<SimGroup[]> {
        const response = await fetch(`/api/v1/simPolicies`);
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async createSim(sim: SIM): Promise<SIM> {
        const response = await fetch(`/api/v1/sims`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sim)
        });
        return sim;
    }

    public async deleteSim(sim: SIM): Promise<boolean> {
        const queryParams = new URLSearchParams({
            group: sim.groupId
        });

        const response = await fetch(`/api/v1/sims/${sim.name}?${queryParams}`, {
            method: 'DELETE'
        });

        return response.ok;
    }
}

export default new SimService();