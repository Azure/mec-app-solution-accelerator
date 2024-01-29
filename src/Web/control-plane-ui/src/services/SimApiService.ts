import { SIM, SimGroup } from '@/models/sim';

export type SimApiServiceSettings = {
    apiKey: string | null
};

class SimApiService {
    private apiKey: string | null;
    constructor(settings: SimApiServiceSettings) {
        this.apiKey = settings.apiKey;
    }

    public async listSims(): Promise<SIM[]> {
        const response = await fetch(`/api/v1/sims`, {
            headers: this.apiKeyHeader()
        });
        const sims = await response.json() as SIM[];
        return sims;
    }

    public async listSimGroups(): Promise<SimGroup[]> {
        const response = await fetch(`/api/v1/simGroups`, {
            headers: this.apiKeyHeader()
        });
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async listSimPolicies(): Promise<SimGroup[]> {
        const response = await fetch(`/api/v1/simPolicies`, {
            headers: this.apiKeyHeader()
        });
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async createSim(sim: SIM): Promise<SIM> {
        const response = await fetch(`/api/v1/sims`, {
            method: 'POST',
            headers: {
                ...this.apiKeyHeader(),
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
            method: 'DELETE',
            headers: this.apiKeyHeader()
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

export default SimApiService;