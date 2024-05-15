'use client'

import React from 'react';
import Loading from '../icons/Loading';

export type ColumnOption = {
    header: JSX.Element | string;
    width?: string;
    padding?: string;
}
export type ColumnOptions = ColumnOption[];
export type TableCell = JSX.Element | string;
export type TableRow = TableCell[];
export type TableProps<TData extends object> = {
    columnOptions: ColumnOptions;
    items: TData[];
    isLoading?: boolean;
    itemToRow?: (item: TData) => TableRow;
};

const defaultItemToRow = <TData extends object,>(item: TData): TableRow => {
    return Object.values(item) as JSX.Element[];
}

export const Table = <TData extends object,>({
    columnOptions,
    items,
    isLoading,
    itemToRow
}: TableProps<TData>) => {
    const renderRow = (item: TData, id: number): JSX.Element => {
        const row = itemToRow ? itemToRow(item) : defaultItemToRow(item);

        return (<tr key={id} className="group hover:bg-gradient-to-r from-[#0DC5B8] to-[#28D890]">
            {row.map((value, index) => {
                const columnOption = columnOptions[index];
                return (<td key={`${id}-${index}`}
                    width={columnOption.width}
                    className={[
                        "border-gray-400 border first:border-l-0 last:border-r-0 group-hover:border-x-0",
                        columnOption.width ?? "",
                        columnOption.padding ?? "py-4 px-6"
                    ].join(" ")}>
                    {value}
                </td>)
            })}
        </tr>);
    }

    const loadingBody = () => {
        return <div className='text-brand mt-8 flex justify-center items-center'>
            <Loading className='w-16 h-16' />
        </div>
    }

    return (<>
        <table className="w-full text-sm text-left text-white">
            <thead className="bg-gray-700">
                <tr className=''>
                    {columnOptions.map((column, index) =>
                        <th key={index}
                            scope="col"
                            className={["font-bold text-lg text-nowrap border-gray-400 border first:border-l-0 last:border-r-0",
                                column.width ?? "",
                                column.padding ?? "py-4 px-6"
                            ].join(" ")}>
                            {column.header}
                        </th>)}
                </tr>
            </thead>
            <tbody>
                {!isLoading && items.map((item, index) => renderRow(item, index))}
            </tbody>
        </table>
        {isLoading && loadingBody()}
    </>)
}

export default Table;