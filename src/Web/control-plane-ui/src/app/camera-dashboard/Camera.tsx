'use client'

import React from 'react';
import dynamic from 'next/dynamic';
const ReactPlayer = dynamic(() => import('react-player/lazy'), { ssr: false });

export type CameraProps = {
  src: string;
};

export default function Camera({
  src
}: CameraProps) {
  return (
    <div>
      <ReactPlayer
        url={src}
        loop={true}
        controls={true}
        muted={true}
        width="100%"
        height="auto"
        playing={true}
      />
    </div>
  )
}
