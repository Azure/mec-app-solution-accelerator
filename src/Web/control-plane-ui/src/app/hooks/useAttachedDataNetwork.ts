import { getAttachedDataNetwork } from '@/stores/attachedDataNetworkSlice';
import { AppDispatch, RootState } from '@/stores/store';
import { useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';

function useAttachedDataNetwork() {
    const dispatch = useDispatch<AppDispatch>();
    const attachedDataNetwork = useSelector((state: RootState) => state.attachedDataNetwork);

    useEffect(() => {
        if (attachedDataNetwork.data === null) {
            dispatch(getAttachedDataNetwork());
        }
    }, [attachedDataNetwork, dispatch]);

    return attachedDataNetwork;
}

export default useAttachedDataNetwork;