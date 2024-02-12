import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { AppDispatch, RootState } from './store';
import { ServiceFactory } from '@/services/ServiceFactory';
import { AttachedDataNetwork } from '@/models/attachedDataNetwork';

interface AttachedDataNetworkState {
    data: AttachedDataNetwork | null;
    loading: boolean;
}

// Initial state
const initialState: AttachedDataNetworkState = {
    data: null,
    loading: false,
};

export const getAttachedDataNetwork = createAsyncThunk<AttachedDataNetwork, void,
    {
        dispatch: AppDispatch,
        state: RootState
    }>(
        'attachedDataNetwork/fetchAttachedDataNetwork',
        async (_, { getState }) => {
            const attachedDataNetworkService = ServiceFactory.getAttachedDataNetworkService(getState().settings);
            return attachedDataNetworkService.getAttacheDataNetwork();
        }
    );

const cameraSlice = createSlice({
    name: 'attachedDataNetwork',
    initialState,
    reducers: {
    },
    extraReducers: (builder) => {
        builder
            .addCase(getAttachedDataNetwork.pending, (state) => {
                state.loading = true;
            })
            .addCase(getAttachedDataNetwork.fulfilled, (state, action) => {
                state.loading = false;
                state.data = action.payload;
            });
    },
});

export default cameraSlice.reducer;
