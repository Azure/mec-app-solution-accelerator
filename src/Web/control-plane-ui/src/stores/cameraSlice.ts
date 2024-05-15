import { createSlice, createAsyncThunk } from '@reduxjs/toolkit';
import { Camera } from '@/models/camera';
import { AppDispatch, RootState } from './store';
import { ServiceFactory } from '@/services/ServiceFactory';

// Define a type for the slice state
interface CameraState {
    data: Camera[];
    loading: boolean;
    createLoading: boolean;
    createError: string | null;
    error: string | null;
}

// Initial state
const initialState: CameraState = {
    data: [],
    loading: false,
    createLoading: false,
    createError: null,
    error: null
};

export const addCamera = createAsyncThunk<Camera, Camera, { dispatch: AppDispatch, state: RootState }>(
    'cameras/addCamera',
    async (camera: Camera, { getState, dispatch, rejectWithValue }) => {
        try {
            const cameraService = ServiceFactory.getCameraService(getState().settings);
            const response = await cameraService.createCamera(camera);
            dispatch(listCameras());
            return response;
        } catch (error) {
            return rejectWithValue(error);
        }
    }
);

export const listCameras = createAsyncThunk<Camera[], void, { state: RootState }>(
    'cameras/listCameras',
    async (_, { getState }): Promise<Camera[]> => {
        const cameraService = ServiceFactory.getCameraService(getState().settings);
        const response = await cameraService.listCameras();

        return response;
    }
);

export const deleteCamera = createAsyncThunk<
    string,
    string,
    { dispatch: AppDispatch, state: RootState }
>(
    'cameras/deleteCamera',
    async (cameraId: string, { getState, dispatch, rejectWithValue }) => {
        try {
            const cameraService = ServiceFactory.getCameraService(getState().settings);
            const response = await cameraService.deleteCamera(cameraId);
            if (response) {
                dispatch(listCameras());
                return cameraId;
            } else {
                return rejectWithValue('Not removed')
            }
        } catch (error) {
            return rejectWithValue(error);
        }
    }
);

const cameraSlice = createSlice({
    name: 'cameras',
    initialState,
    reducers: {
    },
    extraReducers: (builder) => {
        builder
            .addCase(listCameras.pending, (state) => {
                state.loading = true;
            })
            .addCase(listCameras.fulfilled, (state, action) => {
                state.loading = false;
                state.data = action.payload;
            })
            .addCase(listCameras.rejected, (state, action) => {
                state.loading = false;
                state.error = action.error.message || null;
            })
            .addCase(deleteCamera.pending, (_) => {
            })
            .addCase(deleteCamera.fulfilled, (state, action) => {
                state.data = state.data.filter(camera => camera.id !== action.payload);
            })
            .addCase(deleteCamera.rejected, (state, action) => {
                state.error = action.error.message || null;
            })
            .addCase(addCamera.pending, (state) => {
                state.createLoading = true;
                state.createError = null;
            })
            .addCase(addCamera.rejected, (state, action) => {
                state.createLoading = false;
                state.createError = action?.error?.message || null;
            })
            .addCase(addCamera.fulfilled, (state) => {
                state.createLoading = false;
            });
    },
});

export default cameraSlice.reducer;
