'use client'

import { useState } from "react";
import { Settings } from "../icons";
import SettingsModal from "./SettingsModal";

export const SettingsControl = () => {
    const [showSettingsModal, setShowSettingsModal] = useState(false);
    return (<>
        <div className="text-brand float-right right-0 absolute p-4"
            onClick={() => setShowSettingsModal(!showSettingsModal)}>
            <Settings className='h-8 w-8' />
        </div>
        <SettingsModal show={showSettingsModal}
            onClose={() => setShowSettingsModal(false)}
        />
    </>);
}

export default SettingsControl;

