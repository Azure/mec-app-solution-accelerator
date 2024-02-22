import { useEffect, useState } from 'react';

function useAlertsDashboardUri() {
    const [alertsUiUri, setAlertsUiUri] = useState('');
    useEffect(() => {
        const newUri = `${window.location.protocol}//${window.location.hostname}:88`;
        setAlertsUiUri(newUri);
    }, []);

    return alertsUiUri;
}

export default useAlertsDashboardUri;