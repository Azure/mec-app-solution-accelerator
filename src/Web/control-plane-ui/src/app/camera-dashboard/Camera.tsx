'use client'

import React, { useEffect, useState } from 'react';
import ReactPlayer from 'react-player';

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

  return (
    <div>
      {hasWindow && <ReactPlayer
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
      />}
    </div>
  )
}
