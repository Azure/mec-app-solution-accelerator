'use client'

import React, { useEffect, useState } from 'react';
import Table from '../components/table/Table';
import Trash from '../components/icons/Trash';
import AddCameraModal from './AddCameraModal';
import Plus from '../components/icons/Plus';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { deleteCamera, listCameras } from '@/stores/cameraSlice';
import { listSims } from '@/stores/simSlice';
import DeleteConfirmationModal from '../components/modal/DeleteConfirmationModal';
import { Camera } from '@/models/camera';

export const CameraTable = () => {
  const dispatch = useDispatch<AppDispatch>();
  const sims = useSelector((state: RootState) => state.sims.data);
  const cameras = useSelector((state: RootState) => state.cameras.data);
  const isLoading = useSelector((state: RootState) => state.cameras.loading);
  const [showNewCamera, setShowNewCamera] = useState(false);
  const [entityToDelete, setEntityToDelete] = useState<Camera | null>(null);
  const columnOptions = [
    {
      header: 'ID'
    },
    {
      header: 'Model',
    },
    {
      header: 'SIM (5G)',
    },
    {
      header: 'Type',
    },
    {
      header: 'IP',
    },
    { header: '', width: 'auto', padding: 'py-4 px-2' },
  ];

  useEffect(() => {
    dispatch(listSims());
    dispatch(listCameras());
  }, []);

  const handleDeleteCamera = async (cameraId: string) => {
    dispatch(deleteCamera(cameraId));
  };

  return <>
    <div className="bg-gray-500 mt-12 overflow-x-auto relative shadow-md border border-gray-300 sm:rounded-lg">
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
          columnOptions={columnOptions}
          items={cameras}
          isLoading={isLoading}
          itemToRow={(item) => {
            return [
              item.id,
              item.model,
              item.simId ?? '',
              item.type.toString(),
              item.ip,
              <span onClick={() => setEntityToDelete(item)}>
                <Trash className="w-8 h-8 cursor-pointer" />
              </span>
            ]
          }}
        />
      </div>
      <AddCameraModal
        show={showNewCamera}
        onClose={() => setShowNewCamera(false)} />
    </div>
    <DeleteConfirmationModal entity={entityToDelete ? `camera ${entityToDelete.id}` : ''}
      isOpen={entityToDelete !== null}
      onClose={() => setEntityToDelete(null)}
      onDelete={() => {
        if (entityToDelete) {
          handleDeleteCamera(entityToDelete?.id)
          setEntityToDelete(null);
        }
      }} />
  </>;
}

export default CameraTable;