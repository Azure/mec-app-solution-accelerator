import React, { useState } from 'react';
import Modal from "../modal/Modal";
import TextInput from '../form/TextInput';
import Close from '../icons/Close';
import Plus from '../icons/Plus';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import CheckBox from '../form/CheckBox';
import { updateSettings } from '@/stores/settingsSlice';
import ComboBox, { ComboBoxOption } from '../form/ComboBox';
import useAlertsDashboardUri, { AlertsUrlStorageKey } from '@/app/hooks/useAlertsDashboardUri';

export type SettingsModalProps = {
    show: boolean;
    onClose: () => void;
};

export const SettingsModal = ({
    show,
    onClose
}: SettingsModalProps) => {
    const dispatch = useDispatch<AppDispatch>();
    const alertsUiUri = useAlertsDashboardUri();
    const settings = useSelector((state: RootState) => state.settings);
    const [tempSettings, setTempSettings] = useState({ ...settings });
    const reset = () => {
        setTempSettings({ ...settings });
    }

    let value = localStorage.getItem(AlertsUrlStorageKey) || null
    const [alertsUrl, setAlertsUrl] = useState(value)
    const [alertsUrlError, setAlertsUrlError] = useState(false)

    const storeValues = () => {
        let alertsUrlError = false;
        if(alertsUrl){
            const regex = new RegExp('https?:\/\/(www\.)?[-a-zA-Z0-9]{1,256}\.[a-zA-Z0-9()]{1,6}(:[0-9]{4})?(/.*)?')
            if(!!regex.test(alertsUrl ?? '')){
                localStorage.setItem(AlertsUrlStorageKey, alertsUrl!);
            }else{
                alertsUrlError=true;
            }
        }
        if(alertsUrlError){
            setAlertsUrlError(alertsUrlError)
        }else{
            setAlertsUrlError(false)
            dispatch(updateSettings(tempSettings));
            onClose();
        }
    }

    return (<Modal title='Settings'
        isOpen={show}
        onClose={() => {
            onClose();
            setAlertsUrl(localStorage.getItem(AlertsUrlStorageKey) || null);
        }}>
        <form className='mt-9'>
            <div className='mt-4 grid gap-4 items-center grid-cols-[auto_1fr]'>
                <TextInput label='Api Key' value={tempSettings.apiKey ?? ''} onChange={(val) => {
                    setTempSettings({
                        ...tempSettings,
                        apiKey: val,
                    });
                }} />
                <TextInput label='Alerts UI app URL' value={alertsUiUri} hasError={alertsUrlError} onChange={(val) => {
                    setAlertsUrl(val)
                }} />

                <ComboBox label='Logo'
                    options={[{
                        id: 'microsoft',
                        name: 'Microsoft'
                    }, {
                        id: 'leavesbank',
                        name: 'Leavesbank'
                    }]}
                    selected={tempSettings.logo}
                    onSelect={function (option: ComboBoxOption): void {
                        setTempSettings({
                            ...tempSettings,
                            logo: option.id
                        })
                    }} />

                <CheckBox label='Use In Memory'
                    checked={tempSettings.useInMemory}
                    onChange={(val) => setTempSettings({
                        ...tempSettings,
                        useInMemory: val
                    })} />
            </div>
            <div className='flex flex-row gap-10 mt-9 '>
                <button type='button'
                    className='py-2 px-6 flex items-center gap-5 flex-grow justify-center border-2 rounded-full border-[#0DC5B8]'
                    onClick={() => {
                        reset();
                        onClose();
                    }}>
                    Cancel
                    <Close className='w-6 h-6' />
                </button>
                <button type='button'
                    className='py-2 px-6 border rounded-full flex items-center gap-5 flex-grow justify-center bg-gradient-brand border-none text-black'
                    onClick={storeValues}>
                    Save
                    <Plus className='w-6 h-6' />
                </button>
            </div>
        </form>
    </Modal>);
};

export default SettingsModal;