'use client'

import React, { useState } from 'react';
import Table from '../components/table/Table';
import { SIM } from './types';
import Trash from '../components/icons/Trash';
import DropdownButton from '../components/dropdown-button/DropdownButton';
import Modal from '../components/modal/Modal';
import AddSimModal from './AddSimModal';

export const SimTable = () => {
  const [showNewSim, setShowNewSim] = useState(false);
  const header = [
    'Name',
    'ICCID',
    'IMSI',
    'KI',
    'OPC',
    'Options'
  ];

  const sims: SIM[] = [
    {
      name: 'SIM Name 1',
      ICCID: '123456',
      IMSI: 'OP 1',
      opc: 'OPC 123',
      group: 'group1',
      policy: 'policy1',
      ki: 'ki',
    }, {
      name: 'SIM Name 2',
      ICCID: '123456',
      IMSI: 'OP 1',
      opc: 'OPC 1234',
      group: 'group1',
      policy: 'policy1',
      ki: 'ki',
    }
  ];

  return <div className="bg-gray-500 mt-12 overflow-x-auto relative shadow-md border border-gray-300">
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
            item.ICCID,
            item.IMSI,
            item.ki,
            item.opc,
            <span><Trash className="w-8 h-8" /></span>
          ]
        }}
      />
    </div>
    <AddSimModal
      show={showNewSim}
      onClose={() => setShowNewSim(false)} />
  </div>;
}

export default SimTable;