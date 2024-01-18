import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { AppDispatch, RootState } from './store';
import { SIM } from '@/models/sim';
import simService from '@/services/simService';

interface SimState {
    data: SIM[];
    loading: boolean;
    error: string | null;
}

const initialState: SimState = {
    data: [],
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

export const deleteSim = createAsyncThunk<
    SIM,
    SIM,
    { dispatch: AppDispatch, state: RootState }
>(
    'sims/deleteSim',
    async (sim: SIM, { dispatch, rejectWithValue }) => {
        try {
            const response = await simService.deleteSim(sim);
            if (response) {
                dispatch(listSims());
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
                state.data = state.data.filter(sim => sim !== action.payload);
            })
            .addCase(deleteSim.rejected, (state, action) => {
                state.error = action.error.message || null;
            });
    },
});

export default simSlice.reducer;
