declare module 'react-flv-player' {
    import React from 'react';

    type ReactFlvPlayerProps = {
        url: string;
        height?: string;
        width?: string;
        loop?: boolean;
        controls?: boolean;
        playing?: boolean;
        isMuted: boolean;
        type?: 'flv' | 'mp4';
        isLive?: boolean;
        hasAudio?: boolean;
        hasVideo?: boolean;
        enableStashBuffer?: boolean;
        stashInitialSize?: number;
        handleError?: (err: 'NetworkError' | 'MediaError' | 'OtherError') => void;
        enableWarning?: boolean;
        enableError?: boolean;
    };

    export class ReactFlvPlayer extends React.Component<ReactFlvPlayerProps> { }
}