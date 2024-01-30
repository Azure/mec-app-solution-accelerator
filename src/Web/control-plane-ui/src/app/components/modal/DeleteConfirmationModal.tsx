'use client'

import React from 'react';
import Close from '../icons/Close';

export type DeleteConfirmationModalProps = {
    entity: string;
    isOpen: boolean;
    onDelete: () => void;
    onClose: () => void;
}

const DeleteConfirmationModal = ({
    entity,
    isOpen,
    onDelete,
    onClose,
}: DeleteConfirmationModalProps) => {
    if (!isOpen) return null;

    return (
        <div className="z-50 fixed inset-0 bg-black bg-opacity-50 overflow-y-auto h-full w-full" id="my-modal">
            <div className="relative top-36 mx-auto w-full max-w-[44rem] shadow-lg rounded-md  bg-gray-500 text-white border border-gray-300">
                <div className='absolute right-4 top-4'
                    onClick={() => onClose()}>
                    <Close className='w-6 h-6' />
                </div>
                <div className="m-16 text-left">
                    <h3 className="text-2xl leading-6 font-medium text-white">Delete {entity}</h3>
                    <div className="mt-4">
                        Are you sure you want to delete {entity}?
                    </div>
                    <div className='flex flex-row gap-10 mt-9 '>
                        <button type='button'
                            className='py-2 px-6 flex items-center gap-5 flex-grow justify-center border-2 rounded-full border-[#0DC5B8]'
                            onClick={() => {
                                onClose();
                            }}>
                            Cancel
                            <Close className='w-6 h-6' />
                        </button>
                        <button type='button'
                            className='py-2 px-6 border rounded-full flex items-center gap-5 flex-grow justify-center bg-gradient-brand border-none text-black'
                            onClick={() => {
                                onDelete();
                            }}>
                            Delete
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default DeleteConfirmationModal;