import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { AppDispatch, RootState } from './store';
import { SIM, SimGroup, SimPolicy } from '@/models/sim';
import simService from '@/services/simService';

interface SimState {
    data: SIM[];
    simGroups: SimGroup[];
    simPolicies: SimPolicy[];
    loading: boolean;
    error: string | null;
}

const initialState: SimState = {
    data: [],
    simPolicies: [],
    simGroups: [],
    loading: false,
    error: null
};

export const addSim = createAsyncThunk<SIM, SIM, { dispatch: AppDispatch, state: RootState }>(
    'sims/addSim',
    async (sim: SIM, { dispatch }): Promise<SIM> => {
        const response = await simService.createSim(sim);
        dispatch(listSims());
        return response;
    }
);

export const listSims = createAsyncThunk<SIM[], void, { state: RootState }>(
    'sims/listSims',
    async (): Promise<SIM[]> => {
        const response = await simService.listSims();

        return response;
    }
);

export const listSimGroups = createAsyncThunk<SimGroup[], void, { state: RootState }>(
    'sims/listSimGroups',
    async (): Promise<SimGroup[]> => {
        const response = await simService.listSimGroups();
        return response;
    }
);

export const listSimPolicies = createAsyncThunk<SimPolicy[], void, { state: RootState }>(
    'sims/listSimPolicies',
    async (): Promise<SimPolicy[]> => {
        const response = await simService.listSimPolicies();
        return response;
    }
);

export const deleteSim = createAsyncThunk<
    SIM,
    SIM,
    { dispatch: AppDispatch, state: RootState }
>(
    'sims/deleteSim',
    async (sim: SIM, { rejectWithValue }) => {
        try {
            const response = await simService.deleteSim(sim);
            if (response) {
                return sim;
            } else {
                return rejectWithValue('Not removed')
            }
        } catch (error) {
            return rejectWithValue('Error deleting camera');
        }
    }
);

const simSlice = createSlice({
    name: 'sims',
    initialState,
    reducers: {
    },
    extraReducers: (builder) => {
        builder
            .addCase(listSims.pending, (state) => {
                state.loading = true;
            })
            .addCase(listSims.fulfilled, (state, action) => {
                state.loading = false;
                state.data = action.payload;
            })
            .addCase(listSims.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || null;
            })
            .addCase(deleteSim.pending, (_) => {
            })
            .addCase(deleteSim.fulfilled, (state, action) => {
                state.data = state.data.filter(sim => sim.name !== action.payload.name);
            })
            .addCase(deleteSim.rejected, (state, action) => {
                state.error = action.error.message || null;
            })
            .addCase(listSimGroups.fulfilled, (state, action) => {
                state.simGroups = action.payload
            })
            .addCase(listSimPolicies.fulfilled, (state, action) => {
                state.simPolicies = action.payload
            });
    },
});

export default simSlice.reducer;
