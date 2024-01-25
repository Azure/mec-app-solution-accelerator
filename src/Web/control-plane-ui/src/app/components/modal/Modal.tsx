'use client'

import React from 'react';
import Close from '../icons/Close';

export type ModalProps = {
  title?: string;
  isOpen: boolean;
  onClose: () => void;
  children?: React.ReactNode;
}

const Modal = ({
  title,
  isOpen,
  onClose,
  children,
}: ModalProps) => {
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 overflow-y-auto h-full w-full" id="my-modal">
      <div className="relative top-36 mx-auto w-full max-w-[44rem] shadow-lg rounded-md  bg-gray-500 text-white border border-gray-300">
        <div className='absolute right-4 top-4'
          onClick={() => onClose()}>
          <Close className='w-6 h-6' />
        </div>
        <div className="m-16 text-left">
          {title && <h3 className="text-2xl leading-6 font-medium text-white">{title}</h3>}
          {children}
        </div>
      </div>
    </div>
  );
};

export default Modal;