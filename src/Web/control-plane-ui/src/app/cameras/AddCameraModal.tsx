import React, { useEffect, useRef, useState } from 'react';
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
import Loading from '../components/icons/Loading';

const RTSP_MODEL_TEMPLATE: { [key: string]: string, default: string } = {
  default: 'rtsp://{authentication}{ip}:8554',
  'Xingtera XTEE5021': 'rtsp://{authentication}{ip}:554/main',
  'RTSP Stream Container': 'rtsp://{authentication}{ip}:8554/video',
  'Amcrest 4MP ProHD Indoor WiFi/Ethernet': 'rtsp://{authentication}{ip}:554/cam/realmonitor?channel=1&subtype=0',
  'Amcrest 5MP Turret IP Ethernet Camera': 'rtsp://{authentication}{ip}:554/cam/realmonitor?channel=1&subtype=0&unicast=true&proto=Onvif'
}

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
  const loading = useSelector((state: RootState) => state.cameras.createLoading);
  const error = useSelector((state: RootState) => state.cameras.createError);
  const [camera, setCamera] = useState<Partial<Camera>>({});
  const [rtspManual, setRtspManual] = useState(false);
  const hasSubmittedRef = useRef(false);
  const showSIMOption = camera.type === CameraType.FiveG ||
    camera.type === CameraType.FourG ||
    camera.type === CameraType.LTE;

  const reset = () => {
    setCamera({});
    setRtspManual(false);
    setErrors({
      id: false,
      type: false,
      ip: false,
      rtsp: false
    })
  }

  const validateFields = () => {
    const newErrors = {
      id: !camera.id,
      type: !camera.type,
      ip: !camera.ip,
      rtsp: !camera.rtsp || camera.rtsp === ''
    };

    setErrors(newErrors);
    return !Object.values(newErrors).some(isError => isError);
  };

  const [errors, setErrors] = useState({
    id: false,
    type: false,
    ip: false,
    rtsp: false
  });

  const handleSubmit = () => {
    if (validateFields()) {
      dispatch(addCamera(camera as Camera));
      hasSubmittedRef.current = true;
    }
  }

  //Track if added is completed
  useEffect(() => {
    if (hasSubmittedRef.current) {
      if (!loading && !error) {
        reset();
        onClose();
      }
    }
  }, [loading, error]);

  useEffect(() => {
    dispatch(listSims());
  }, []);

  // Update rtsp uri from previous data
  // We wait until we have at least an IP
  useEffect(() => {
    if (!rtspManual && camera.ip) {
      const rtspTemplate = RTSP_MODEL_TEMPLATE[camera.model ?? ''] ?? RTSP_MODEL_TEMPLATE.default;
      const username = camera.username ?? '';
      const password = camera.password ?? '';
      const authPart = username || password ? `${username}${password ? ':' : ''}${password}@` : '';

      const newRtsp = rtspTemplate
        .replace('{ip}', camera.ip)
        .replace('{authentication}', authPart);

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
        <TextInput label='Id*'
          hasError={errors.id}
          value={camera.id ?? ''}
          onChange={(val) => {
            setErrors({
              ...errors,
              id: false
            });
            setCamera({
              ...camera,
              id: val,
            });
          }} />

        <ComboBox label='Type*'
          hasError={errors.type}
          selected={camera.type ?? ''}
          options={Object.values(CameraType).map(x => ({ id: x, name: x }))}
          onSelect={(val) => {
            if ((Object.values(CameraType) as string[]).includes(val.id)) {
              setErrors({
                ...errors,
                type: false
              });
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

        <TextInput label='Ip*'
          hasError={errors.ip}
          value={camera.ip ?? ''}
          onChange={(val) => {
            setErrors({
              ...errors,
              ip: false
            });
            setCamera({
              ...camera,
              ip: val,
            });
          }} />

        <ComboBox label='Model'
          selected={camera.model ?? ''}
          options={[
            {
              id: 'Xingtera XTEE5021',
              name: 'Xingtera XTEE5021',
              type: [
                CameraType.FiveG,
                CameraType.FourG,
                CameraType.LTE,
              ]
            },
            {
              id: 'RTSP Stream Container',
              name: 'RTSP Stream Container',
              type: [
                CameraType.Container
              ]
            },
            {
              id: 'Amcrest 4MP ProHD Indoor WiFi/Ethernet',
              name: 'Amcrest 4MP ProHD Indoor WiFi/Ethernet',
              type: [
                CameraType.Wifi,
                CameraType.Ethernet,
              ]
            },
            {
              id: 'Amcrest 5MP Turret IP Ethernet Camera',
              name: 'Amcrest 5MP Turret IP Ethernet Camera',
              type: [
                CameraType.Ethernet,
              ]
            }]
            .filter(x => x.type.includes(camera.type as CameraType))
            .map(x => ({ id: x.id, name: x.name }))
          }
          onSelect={(val) => {
            setCamera({
              ...camera,
              model: val.id
            })
          }} />
        <TextInput label='Username'
          value={camera.username ?? ''}
          onChange={(val) => {
            setCamera({
              ...camera,
              username: val,
            });
          }} />

        <TextInput label='Password'
          value={camera.password ?? ''}
          type='password'
          onChange={(val) => {
            setCamera({
              ...camera,
              password: val,
            });
          }} />

        <TextInput label='RTSP Uri*'
          hasError={errors.rtsp}
          value={camera.rtsp ?? ''}
          onChange={(val) => {
            setErrors({
              ...errors,
              rtsp: false
            });
            setRtspManual(true);
            setCamera({
              ...camera,
              rtsp: val
            });
          }} />

        <TextInput label='HTTP Uri' value={camera.hls ?? ''} onChange={(val) => {
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
          onClick={() => { handleSubmit() }}>
          {loading ? <Loading className='w-8 h-8' /> : <>Add new <Plus className='w-6 h-6' /></>}
        </button>
      </div>
    </form>
  </Modal>);
};

export default AddCameraModal;