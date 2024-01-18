'use client'

import React, { useEffect, useState } from 'react';
import Table from '../components/table/Table';
import Trash from '../components/icons/Trash';
import AddCameraModal from './AddCameraModal';
import Plus from '../components/icons/Plus';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { deleteCamera, listCameras } from '@/stores/cameraSlice';

export const CameraTable = () => {
  const dispatch = useDispatch<AppDispatch>();
  const cameras = useSelector((state: RootState) => state.cameras.data);
  const [showNewCamera, setShowNewCamera] = useState(false);
  const header = [
    'ID',
    'Model',
    'SIM (5G)',
    'Type',
    'IP',
    'Port',
    'Options'
  ];

  useEffect(() => {
    dispatch(listCameras());
  }, []);

  const handleDeleteCamera = async (cameraId: string) => {
    dispatch(deleteCamera(cameraId));
  };


  return <div className="bg-gray-500 mt-12 overflow-x-auto relative shadow-md border border-gray-300 sm:rounded-lg">
    <div className="pt-6 pb-6 px-16 flex justify-end items-center">
      <button
        onClick={() => setShowNewCamera(true)}
        className="text-black px-8 py-2 focus:outline-none focus:ring-2 bg-gradient-brand rounded-full"
      >
        <span className='flex gap-5'>
          New Camera
          <Plus className='w-6 h-6' />
        </span>
      </button>
    </div>
    <div className='box-border px-16 pb-16'>
      <Table
        headers={header}
        items={cameras}
        itemToRow={(item) => {
          return [
            item.id,
            item.model,
            item.sim?.name ?? '',
            item.type.toString(),
            item.ip,
            item.port,
            <span onClick={() => handleDeleteCamera(item.id)}>
              <Trash className="w-8 h-8 cursor-pointer" />
            </span>
          ]
        }}
      />
    </div>
    <AddCameraModal
      show={showNewCamera}
      onClose={() => setShowNewCamera(false)} />
  </div>;
}

export default CameraTable;