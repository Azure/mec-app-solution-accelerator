'use client'

import React from 'react';

export type TableHeader = JSX.Element | string;
export type TableHeaders = TableHeader[];
export type TableCell = JSX.Element | string;
export type TableRow = TableCell[];
export type TableProps<TData extends object> = {
    headers: TableHeaders;
    items: TData[];
    itemToRow?: (item: TData) => TableRow;
};

const defaultItemToRow = <TData extends object,>(item: TData): TableRow => {
    return Object.values(item) as JSX.Element[];
}

export const Table = <TData extends object,>({
    headers,
    items,
    itemToRow
}: TableProps<TData>) => {
    const renderRow = (item: TData, id: number): JSX.Element => {
        const row = itemToRow ? itemToRow(item) : defaultItemToRow(item);
        return (<tr key={id} className="group hover:bg-gradient-to-r from-[#0DC5B8] to-[#28D890]">
            {row.map((value, index) => {
                return (<td key={`${id}-${index}`} className="py-4 px-6 border-gray-400 border first:border-l-0 last:border-r-0 group-hover:border-x-0">
                    {value}
                </td>)
            })}
        </tr>);
    }

    return (<>
        <table className="w-full text-sm text-left text-white">
            <thead className="">
                <tr className=''>
                    {headers.map((headerCell, index) =>
                        <th key={index} scope="col" className="py-3 px-6 font-bold text-lg text-nowrap border-gray-400 border first:border-l-0 last:border-r-0">
                            {headerCell}
                        </th>)}
                </tr>
            </thead>
            <tbody>
                {/* TODO: Pagination */}
                {items.map((item, index) => renderRow(item, index))}
            </tbody>
        </table>
        {/* Pagination or other controls */}
    </>)
}

export default Table;