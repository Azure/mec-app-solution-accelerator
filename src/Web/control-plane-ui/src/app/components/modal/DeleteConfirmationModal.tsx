'use client'

import React from 'react';
import Close from '../icons/Close';
import Modal from './Modal';

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
        <Modal
            isOpen={isOpen}
            title={`Delete ${entity}`}
            onClose={() => onClose()}>
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
        </Modal>
    );
};

export default DeleteConfirmationModal;