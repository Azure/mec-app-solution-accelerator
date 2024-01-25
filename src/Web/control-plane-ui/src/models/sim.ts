export type SIM = {
    name: string;
    imsi: string;
    iccid: string;
    ki: string;
    opc: string;
    ip?: string;
    policyId?: string;
    groupId: string;
}

export type SimGroup = {
    id: string;
    name: string;
}

export type SimPolicy = {
    id: string;
    name: string;
}