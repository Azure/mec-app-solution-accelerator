import { SIM, SimGroup } from '@/models/sim';

class SimService {
    private _apiUrl: string = "";

    async getApiUrl() {
        if (!this._apiUrl) {
            const response = await fetch('/api/config');
            const data = await response.json();
            this._apiUrl = data.API_URL as string;
        }
        return this._apiUrl;
    }

    public async listSims(): Promise<SIM[]> {
        const apiUrl = await this.getApiUrl();
        const response = await fetch(`${apiUrl}/api/v1/sims`);
        const sims = await response.json() as SIM[];
        console.log(sims);
        return sims;
    }

    public async listSimGroups(): Promise<SimGroup[]> {
        const apiUrl = await this.getApiUrl();
        const response = await fetch(`${apiUrl}/api/v1/simGroups`);
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async listSimPolicies(): Promise<SimGroup[]> {
        const apiUrl = await this.getApiUrl();
        const response = await fetch(`${apiUrl}/api/v1/simPolicies`);
        const groups = await response.json() as SimGroup[];
        return groups;
    }

    public async createSim(sim: SIM): Promise<SIM> {
        const apiUrl = await this.getApiUrl();
        const response = await fetch(`${apiUrl}/api/v1/sims`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(sim)
        });
        return sim;
    }

    public async deleteSim(sim: SIM): Promise<boolean> {
        const apiUrl = await this.getApiUrl();
        const queryParams = new URLSearchParams({
            group: sim.groupId
        });

        const response = await fetch(`${apiUrl}/api/v1/sims/${sim.name}?${queryParams}`, {
            method: 'DELETE'
        });

        return response.ok;
    }
}

export default new SimService();