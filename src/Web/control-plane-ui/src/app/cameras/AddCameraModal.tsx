import React, { useEffect, useState } from 'react';
import Modal from "../components/modal/Modal";
import TextInput from '../components/form/TextInput';
import Close from '../components/icons/Close';
import Plus from '../components/icons/Plus';
import ComboBox from '../components/form/ComboBox';
import { Camera, CameraType } from '@/models/camera';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { addCamera } from '@/stores/cameraSlice';
import { listSims } from '@/stores/simSlice';

const RTSP_URL_TEMPLATE = "rtsp://{ip}:{port}/{query}";
const RTSP_MODEL_PORT: { [key: string]: string } = {
  'Xingtera XTEE5021': '554',
  'RTSP Stream Container': '8554'
};
const RTSP_MODEL_QUERY: { [key: string]: string } = {
  'Xingtera XTEE5021': 'main',
  'RTSP Stream Container': 'video'
};

export type AddCameraModalProps = {
  show: boolean;
  onClose: () => void;
};

export const AddCameraModal = ({
  show,
  onClose
}: AddCameraModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const sims = useSelector((state: RootState) => state.sims.data);
  const [camera, setCamera] = useState<Partial<Camera>>({});
  const [rtspManual, setRtspManual] = useState(false);
  const showSIMOption = camera.type === CameraType.FiveG ||
    camera.type === CameraType.FourG ||
    camera.type === CameraType.LTE;

  const reset = () => {
    setCamera({});
    setRtspManual(false);
  }

  useEffect(() => {
    dispatch(listSims());
  }, []);

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
        <TextInput label='Id' value={camera.id ?? ''} onChange={(val) => {
          setCamera({
            ...camera,
            id: val,
          });
        }} />

        <ComboBox label='Type'
          selected={camera.type ?? ''}
          options={Object.values(CameraType).map(x => ({ id: x, name: x }))}
          onSelect={(val) => {
            if ((Object.values(CameraType) as string[]).includes(val.id)) {
              setCamera({
                ...camera,
                type: val.id as CameraType
              });
            }
          }} />

        {showSIMOption &&
          <ComboBox label='SIM'
            selected={camera.simId ?? ''}
            options={sims.map(s => ({ id: s.name, name: s.name }))}
            onSelect={(val) => setCamera({
              ...camera,
              ip: sims.find(s => s.name == val.name)?.ip,
              simId: val.id
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
          options={[
            { id: 'Xingtera XTEE5021', name: 'Xingtera XTEE5021' },
            { id: 'RTSP Stream Container', name: 'RTSP Stream Container' },
          ]}
          onSelect={(val) => {
            setCamera({
              ...camera,
              model: val.id,
              port: RTSP_MODEL_PORT[val.id] ?? camera.port
            })
          }} />

        <TextInput label='RTSP Uri' value={camera.rtsp ?? ''} onChange={(val) => {
          setRtspManual(true);
          setCamera({
            ...camera,
            rtsp: val
          });
        }} />

        <TextInput label='HLS Uri' value={camera.hls ?? ''} onChange={(val) => {
          setCamera({
            ...camera,
            hls: val
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
            dispatch(addCamera(camera as Camera));
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