import { SIM } from '@/models/sim';

class SimService {
    private sims: SIM[] = [
        {
            name: 'SIM1',
            iccid: '8912345678901234566',
            imsi: '001019990010001',
            opc: '63bfa50ee6523365ff14c1f45f88737d',
            groupId: 'group1',
            policyId: 'policy1',
            ki: 'ki',
            ip: '10.0.0.1'
        }, {
            name: 'SIM2',
            iccid: '8922345678901234567',
            imsi: '001019990010002',
            opc: '63bfa50ee6523365ff14c1f45f88738d',
            groupId: 'group1',
            policyId: 'policy1',
            ki: 'ki',
            ip: '10.0.0.2'
        }
    ];

    public async listSims(): Promise<SIM[]> {
        return this.sims;
    }

    public async createSim(sim: SIM): Promise<SIM> {
        this.sims = [...this.sims, sim];
        return sim;
    }

    public async deleteSim(sim: SIM): Promise<boolean> {
        const indexToDelete = this.sims.findIndex(c => c.iccid === sim.iccid);
        if (indexToDelete === -1) return false;
        this.sims = [...this.sims.filter((_, index) => indexToDelete !== index)];
        return true;
    }
}

export default new SimService();