import React from 'react';
import Modal from "../components/modal/Modal";
import Close from '../components/icons/Close';
import { Camera } from '@/models/camera';
import { Field } from '../components/form/Field';

export type CameraDetailsModalProps = {
  show: boolean;
  camera: Camera;
  onClose: () => void;
};

export const CameraDetailsModal = ({
  show,
  camera,
  onClose
}: CameraDetailsModalProps) => {
  return (<Modal title='Camera details'
    isOpen={show}
    onClose={onClose}>
    <div className='mt-9'>
      <div className='mt-4 grid gap-4 items-center grid-cols-[auto_1fr]'>
        <Field label='Id' value={camera.id} />
        <Field label='Type' value={camera.type} />
        <Field label='SIM' value={camera.simId ?? ''} />
        <Field label='IP' value={camera.ip ?? ''} />
        <Field label='Model' value={camera.model ?? ''} />
        <Field label='RTSP Uri' value={camera.rtsp ?? ''} />
        <Field label='HTTP Uri' value={camera.hls ?? ''} />
      </div>
    </div>

    <div className='flex flex-row gap-10 mt-9 '>
      <button type='button'
        className='py-2 px-6 flex items-center gap-5 flex-grow justify-center border-2 rounded-full border-[#0DC5B8]'
        onClick={() => {
          onClose();
        }}>
        Close
        <Close className='w-6 h-6' />
      </button>
    </div>
  </Modal >);
};

export default CameraDetailsModal;