import React, { useId } from 'react';

export type FieldProps = {
    label: string;
    value: string;
};

export const Field = ({
    label, value,
}: FieldProps) => {
    const id = useId();

    return (<>
        <span className="block text-m font-medium text-gray-100">{label}</span>
        <span
            id={id}
            className={[
                "py-3 px-4 leading-6 block w-full shadow-sm rounded-md bg-gray-700 text-white focus:border-transparent focus:ring-0 focus:outline-none",
                "h-12"
            ].join(" ")}>
            {value}
        </span>
    </>);
};
