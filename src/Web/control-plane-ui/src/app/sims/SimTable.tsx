'use client'

import React, { useEffect, useState } from 'react';
import Table from '../components/table/Table';
import Trash from '../components/icons/Trash';
import DropdownButton from '../components/dropdown-button/DropdownButton';
import AddSimModal from './AddSimModal';
import { SIM } from '@/models/sim';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { deleteSim, listSims } from '@/stores/simSlice';
import DeleteConfirmationModal from '../components/modal/DeleteConfirmationModal';

export const SimTable = () => {
  const dispatch = useDispatch<AppDispatch>();
  const sims = useSelector((state: RootState) => state.sims.data);
  const [showNewSim, setShowNewSim] = useState(false);
  const [entityToDelete, setEntityToDelete] = useState<SIM | null>(null);

  const header = [
    'Name',
    'IMSI',
    'IP',
    'Options'
  ];

  useEffect(() => {
    dispatch(listSims());
  }, []);

  const handleDeleteSim = async (sim: SIM) => {
    dispatch(deleteSim(sim));
  };

  return <><div className="bg-gray-500 mt-12 overflow-x-auto relative shadow-md border border-gray-300">
    <div className="pt-6 pb-6 px-16 flex justify-end items-center">
      <DropdownButton title={'Add SIM'}
        actions={[
          { title: 'Add manually', onClick: () => setShowNewSim(true) },
          { title: 'Import JSON file', onClick: () => console.log('TODO') },
        ]}
      />
    </div>
    <div className='box-border pb-6 px-16'>
      <Table
        headers={header}
        items={sims}
        itemToRow={(item) => {
          return [
            item.name,
            item.imsi,
            item.ip ?? '',
            <span onClick={() => setEntityToDelete(item)}>
              <Trash className="w-8 h-8 cursor-pointer" />
            </span>
          ]
        }}
      />
    </div>
    <AddSimModal
      show={showNewSim}
      onClose={() => setShowNewSim(false)} />
  </div>
    <DeleteConfirmationModal entity={entityToDelete ? `SIM ${entityToDelete.name}` : ''}
      isOpen={entityToDelete !== null}
      onClose={() => setEntityToDelete(null)}
      onDelete={() => {
        if (entityToDelete) {
          handleDeleteSim(entityToDelete)
          setEntityToDelete(null);
        }
      }} />
  </>;
}

export default SimTable;