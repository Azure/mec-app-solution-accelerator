import React, { useEffect, useState } from 'react';
import Modal from "../components/modal/Modal";
import { Camera, CameraType } from './types';
import TextInput from '../components/form/TextInput';
import Close from '../components/icons/Close';
import Plus from '../components/icons/Plus';
import ComboBox from '../components/form/ComboBox';
import { SIM } from '../sims/types';

const RTSP_URL_TEMPLATE = "rtsp://{ip}:{port}/{query}";
const RTSP_MODEL_PORT: { [key: string]: string } = {
  'Model1': '8554',
  'Model2': '8554'
};
const RTSP_MODEL_QUERY: { [key: string]: string } = {
  'Model1': 'model1',
  'Model2': 'model2'
};

export type AddCameraModalProps = {
  show: boolean;
  onClose: () => void;
};

export const AddCameraModal = ({
  show,
  onClose
}: AddCameraModalProps) => {
  const [camera, setCamera] = useState<Partial<Camera>>({});
  const [rtspManual, setRtspManual] = useState(false);
  const showSIMOption = camera.type === CameraType.FiveG ||
    camera.type === CameraType.FourG ||
    camera.type === CameraType.LTE;

  const reset = () => {
    setCamera({});
    setRtspManual(false);
  }

  // Update rtsp uri from previous data
  // We wait until we have at least an IP
  useEffect(() => {
    if (!rtspManual && camera.ip) {
      const newRtsp = RTSP_URL_TEMPLATE
        .replace('{ip}', camera.ip)
        .replace(':{port}', camera.port ? `:${camera.port}` : '')
        .replace('{query}', RTSP_MODEL_QUERY[camera.model ?? ''] ?? '');
      if (newRtsp !== camera.rtsp) {
        setCamera({
          ...camera,
          rtsp: newRtsp
        });
      }
    }
  }, [camera]);

  return (<Modal title='Add new Camera'
    isOpen={show}
    onClose={onClose}>
    <form className='mt-9'>
      <div className='mt-4 grid gap-4 items-center grid-cols-[auto_1fr]'>
        <ComboBox label='Type'
          selected={camera.type ?? ''}
          options={Object.values(CameraType)}
          onSelect={(val) => {
            if ((Object.values(CameraType) as string[]).includes(val)) {
              setCamera({
                ...camera,
                type: val as CameraType
              });
            }
          }} />

        {showSIMOption &&
          <ComboBox label='SIM'
            selected={camera.sim?.name ?? ''}
            options={['SIM 1', 'SIM 2']}
            onSelect={(val) => setCamera({
              ...camera,
              ip: '10.0.0.1',
              sim: { name: val, ip: '10.0.0.1' } as SIM
            })} />
        }

        <TextInput label='Ip' value={camera.ip ?? ''} onChange={(val) => {
          setCamera({
            ...camera,
            ip: val,
          });
        }} />

        <ComboBox label='Model'
          selected={camera.model ?? ''}
          options={['Model1', 'Model2']}
          onSelect={(val) => {
            setCamera({
              ...camera,
              model: val,
              port: RTSP_MODEL_PORT[val] ?? camera.port
            })
          }} />

        <TextInput label='Port' value={camera.port ?? ''} onChange={(val) => {
          setCamera({
            ...camera,
            port: val,
          });
        }} />

        <TextInput label='RTSP Uri' value={camera.rtsp ?? ''} onChange={(val) => {
          setRtspManual(true);
          setCamera({
            ...camera,
            rtsp: val
          });
        }} />

        <TextInput label='HLS Uri' value={camera.rtsp ?? ''} onChange={(val) => {
          setRtspManual(true);
          setCamera({
            ...camera,
            rtsp: val
          });
        }} />
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
          onClick={() => {
            //TODO: validation
            //TODO: Trigger SIM creation
            reset();
            onClose();
          }}>
          Add new
          <Plus className='w-6 h-6' />
        </button>
      </div>
    </form>
  </Modal>);
};

export default AddCameraModal;