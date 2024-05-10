'use client'

import { useState } from "react";
import { Settings } from "../icons";
import SettingsModal from "./SettingsModal";
import { useRouter } from "next/navigation";

export const SettingsControl = () => {
    const router = useRouter();
    const [showSettingsModal, setShowSettingsModal] = useState(false);
    return (<>
        <div className="text-brand float-right right-0 absolute p-4"
            onClick={() => setShowSettingsModal(!showSettingsModal)}>
            <Settings className='h-8 w-8' />
        </div>
        <SettingsModal show={showSettingsModal}
            onClose={() => {
                setShowSettingsModal(false);
                router.refresh();
            }}
        />
    </>);
}

export default SettingsControl;


