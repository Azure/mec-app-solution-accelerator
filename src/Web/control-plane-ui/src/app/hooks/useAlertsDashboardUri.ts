import { useEffect, useState } from 'react';

export const AlertsUrlStorageKey = "alerts_url";
function useAlertsDashboardUri() { 
    const [alertsUiUri, setAlertsUiUri] = useState('');
    useEffect(() => {
        const alertsUrl = localStorage.getItem(AlertsUrlStorageKey) || undefined;
        setAlertsUiUri(alertsUrl ?? `${window.location.protocol}//${window.location.hostname}:88`);
    }, [localStorage.getItem(AlertsUrlStorageKey)]);
    return alertsUiUri;
}

export default useAlertsDashboardUri;