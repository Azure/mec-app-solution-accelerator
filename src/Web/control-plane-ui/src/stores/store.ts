import { configureStore } from '@reduxjs/toolkit';
import cameraReducer from './cameraSlice';
import simReducer from './simSlice';

export const store = configureStore({
    reducer: {
        cameras: cameraReducer,
        sims: simReducer
    },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;