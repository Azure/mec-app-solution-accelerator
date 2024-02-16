'use client'


import React, { useEffect, useState } from 'react';
import dynamic from 'next/dynamic';
import ReactPlayer from 'react-player';

const ReactFlvPlayer = dynamic(() => import('react-flv-player')
  .then((mod) => ({ default: mod.ReactFlvPlayer })), {
  ssr: false,
});

export type CameraProps = {
  src: string;
};

export default function Camera({
  src
}: CameraProps) {
  const [hasWindow, setHasWindow] = useState(false);
  const [playerKey, setPlayerKey] = useState(1);

  useEffect(() => {
    if (typeof window !== "undefined") {
      setHasWindow(true);
    }
  }, []);

  const Player = () => {
    if (src.toLowerCase().endsWith(".flv")) {
      return <ReactFlvPlayer
        key={playerKey}
        url={src}
        loop={true}
        controls={true}
        hasAudio={false}
        isMuted={true}
        isLive={true}
        enableStashBuffer={false}
        type="flv"
        width="100%"
        height="auto"
        playing={true}
        handleError={(e) => {
          // Refresh player in 1 second, during camera initialization
          // We can find 404 until the stream is created/active
          setTimeout(() => {
            setPlayerKey(playerKey + 1);
          }, 1000);
        }}
      />
    } else {
      return <ReactPlayer
        key={playerKey}
        url={src}
        loop={true}
        controls={true}
        muted={true}
        width="100%"
        height="auto"
        playing={true}
        onError={(e) => {
          // Refresh player in 1 second, during camera initialization
          // We can find 404 until the stream is created/active
          setTimeout(() => {
            setPlayerKey(playerKey + 1);
          }, 1000);
        }}
      />
    }
  }

  return (
    <div>
      {hasWindow && <Player />}
    </div>
  )
}
