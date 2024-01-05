'use client'

import React from 'react';
import PageTitle from '@/app/components/PageTitle'
import Camera from './Camera';

export default function Home() {

  return (
    <>
      <PageTitle title='Camera Dashboard' />

      <div className='mt-12 grid lg:grid-cols-2 gap-4'>
        <Camera src="http://127.0.0.1:3001/stream.m3u8" />
        <Camera src="http://127.0.0.1:3001/stream.m3u8" />
        <Camera src="http://127.0.0.1:3001/stream.m3u8" />
      </div>
    </>
  )
}
