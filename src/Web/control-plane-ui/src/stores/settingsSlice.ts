import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { AppDispatch, RootState } from './store';
import { listCameras } from './cameraSlice';
import { listSimGroups, listSimPolicies, listSims } from './simSlice';
import { env } from 'next-runtime-env';

// Define a type for the slice state
interface SettingsState {
    apiKey: string | null;
    useInMemory: boolean;
    logo: string;
}

// Initial state
const initialState: SettingsState = {
    useInMemory: false,
    apiKey: null,
    logo: env('NEXT_PUBLIC_LOGO') ?? 'microsoft'
};

export const updateSettings = createAsyncThunk<SettingsState, SettingsState, { dispatch: AppDispatch, state: RootState }>(
    'settings/updateSettings',
    async (settings: SettingsState, { dispatch, getState }): Promise<SettingsState> => {
        // Reset app state
        dispatch(listCameras());
        dispatch(listSimGroups());
        dispatch(listSimPolicies());
        dispatch(listSims());
        return { ...settings };
    }
);

const cameraSlice = createSlice({
    name: 'settings',
    initialState,
    reducers: {},
    extraReducers: (builder) => {
        builder
            .addCase(updateSettings.fulfilled, (state, action) => {
                state.apiKey = action.payload.apiKey;
                state.useInMemory = action.payload.useInMemory;
                state.logo = action.payload.logo
            });
    },
});

export default cameraSlice.reducer;
