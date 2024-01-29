import { configureStore } from '@reduxjs/toolkit';
import cameraReducer from './cameraSlice';
import simReducer from './simSlice';
import settingsSlice from './settingsSlice';

export const store = configureStore({
    reducer: {
        cameras: cameraReducer,
        sims: simReducer,
        settings: settingsSlice,
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;