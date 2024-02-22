import React from 'react';

export type DisplayErrorMessageProps = {
    errorMessage: string
}

const DisplayErrorMessage = ({ errorMessage }: DisplayErrorMessageProps) => {
    const createErrorMessage = (error: string) => {
        try {
            const parsedError = JSON.parse(error);
            return parsedError?.error?.message || error;
        } catch (e) {
            console.log(e);
            return error;
        }
    };

    return (
        <div className='bg-gray-900 p-4'>
            <h3 className='text-red-500'>Error</h3>
            <span className='break-words'>
                {createErrorMessage(errorMessage)}
            </span>
        </div>
    );
};

export default DisplayErrorMessage;