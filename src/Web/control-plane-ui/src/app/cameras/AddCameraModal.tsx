import React, { useEffect, useState } from 'react';
import Modal from "../components/modal/Modal";
import TextInput from '../components/form/TextInput';
import Close from '../components/icons/Close';
import Plus from '../components/icons/Plus';
import ComboBox from '../components/form/ComboBox';
import { Camera, CameraType } from '@/models/camera';
import { SIM } from '@/models/sim';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { addCamera } from '@/stores/cameraSlice';
import { listSims } from '@/stores/simSlice';

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
            options={sims.map(s => s.name)}
            onSelect={(val) => setCamera({
              ...camera,
              ip: sims.find(s => s.name == val)?.ip,
              sim: sims.find(s => s.name == val)
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