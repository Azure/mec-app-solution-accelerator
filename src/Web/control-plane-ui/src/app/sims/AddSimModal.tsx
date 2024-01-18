import React, { useState } from 'react';
import Modal from "../components/modal/Modal";
import TextInput from '../components/form/TextInput';
import Close from '../components/icons/Close';
import Plus from '../components/icons/Plus';
import ComboBox from '../components/form/ComboBox';
import SimCSVLoader from './SimCSVLoader';
import { SIM } from '@/models/sim';
import { useDispatch } from 'react-redux';
import { AppDispatch } from '@/stores/store';
import { addSim } from '@/stores/simSlice';

export type AddSimModalProps = {
  show: boolean;
  onClose: () => void;
};

export const AddSimModal = ({
  show,
  onClose
}: AddSimModalProps) => {
  const dispatch = useDispatch<AppDispatch>();
  const [sim, setSim] = useState<Partial<SIM>>({});

  return (<Modal title='Add new SIM'
    isOpen={show}
    onClose={onClose}>
    <form className='mt-9'>
      <div className='flex flex-row-reverse'>
        <SimCSVLoader onSimLoaded={(partialSim) => {
          setSim({
            ...sim,
            ...partialSim,
          });
        }} />
      </div>
      <div className='mt-4 grid gap-4 items-center grid-cols-[auto_1fr]'>
        <TextInput label='Name' value={sim.name ?? ''} onChange={(val) => setSim({
          ...sim,
          name: val
        })} />
        <TextInput label='IMSI' value={sim.IMSI ?? ''} onChange={(val) => setSim({
          ...sim,
          IMSI: val
        })} />
        <TextInput label='ICCID' value={sim.ICCID ?? ''} onChange={(val) => setSim({
          ...sim,
          ICCID: val
        })} />
        <TextInput label='IP' value={sim.ip ?? ''} onChange={(val) => setSim({
          ...sim,
          ip: val
        })} />
        <TextInput label='Ki' value={sim.ki ?? ''} onChange={(val) => setSim({
          ...sim,
          ki: val
        })} />
        <TextInput label='Opc' value={sim.opc ?? ''} onChange={(val) => setSim({
          ...sim,
          opc: val
        })} />
        <ComboBox label='Group'
          selected={sim.group ?? ''}
          options={['Option 1', 'Option 2']}
          onSelect={(val) => setSim({
            ...sim,
            group: val
          })} />
      </div>
      <div className='flex flex-row gap-10 mt-9 '>
        <button type='button'
          className='py-2 px-6 flex items-center gap-5 flex-grow justify-center border-2 rounded-full border-[#0DC5B8]'
          onClick={() => {
            setSim({});
            onClose();
          }}>
          Cancel
          <Close className='w-6 h-6' />
        </button>
        <button type='button'
          className='py-2 px-6 border rounded-full flex items-center gap-5 flex-grow justify-center bg-gradient-brand border-none text-black'
          onClick={() => {
            //TODO: validation
            dispatch(addSim(sim as SIM));
            setSim({});
            onClose();
          }}>
          Add new
          <Plus className='w-6 h-6' />
        </button>
      </div>
    </form>
  </Modal>);
};

export default AddSimModal;