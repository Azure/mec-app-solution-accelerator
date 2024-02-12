'use client'

import React, { useEffect } from 'react';
import PageTitle from '@/app/components/PageTitle'
import Camera from './Camera';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '@/stores/store';
import { listCameras } from '@/stores/cameraSlice';

export default function Home() {
  const dispatch = useDispatch<AppDispatch>();
  const cameras = useSelector((state: RootState) => state.cameras.data);
  useEffect(() => {
    dispatch(listCameras());
  }, []);

  return (
    <>
      <PageTitle title='Camera Dashboard' />

      <div className='mt-12 grid lg:grid-cols-2 gap-4'>

        {cameras.map(c => {
          const url = c.hls && c.hls !== '' ? c.hls : '/hls/' + c.id + '/stream.m3u8';
          return <div>
            <h2 className='text-white'>{c.id}</h2>
            <Camera key={c.id} src={url} />
          </div>
        })}
      </div>
    </>
  )
}
