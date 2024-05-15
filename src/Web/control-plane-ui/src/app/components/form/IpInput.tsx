import React, { useEffect, useId, useRef, useState } from "react";
import ChevronUp from "../icons/ChevronUp";
import ChevronDown from "../icons/ChevronDown";

export type IpInputProps = {
    label: string;
    value: string;
    ipSubnet: string;
    hasError?: boolean;
    ipsInUse?: string[];
    onChange: (value: string) => void;
};

function cidrToIpList(cidr: string): string[] {
    if (cidr === '') {
        return [];
    }

    const [ip, prefix] = cidr.split('/');
    const mask = -1 << (32 - parseInt(prefix));
    const start = ipToLong(ip) & mask;
    const end = start + Math.pow(2, (32 - parseInt(prefix))) - 1;

    const ipList: string[] = [];
    for (let i = start + 1; i <= end; i++) {
        ipList.push(longToIp(i));
    }
    return ipList;
}

function ipToLong(ip: string): number {
    return ip.split('.').reduce((acc, octet) => (acc << 8) + parseInt(octet), 0);
}

function longToIp(long: number): string {
    return ((long >>> 24) & 0xFF) + "." + ((long >>> 16) & 0xFF) + "." + ((long >>> 8) & 0xFF) + "." + (long & 0xFF);
}

export const IpInput = ({
    label,
    value,
    hasError,
    ipSubnet,
    ipsInUse,
    onChange
}: IpInputProps) => {
    const wrapperRef = useRef<HTMLDivElement>(null);
    const id = useId();
    const [isOpen, setIsOpen] = useState(false);


    const handleSelect = (ip: string) => {
        onChange(ip);
        setIsOpen(false);
    }

    useEffect(() => {
        function handleClickOutside(event: MouseEvent) {
            if (wrapperRef.current && !wrapperRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        }
        document.addEventListener("mousedown", handleClickOutside);
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [wrapperRef]);

    return (<>
        <label htmlFor={id} className="block text-m font-medium text-gray-100">{label}</label>
        <div id={id}
            className={[
                "relative block w-full shadow-sm sm:text-sm rounded-md  text-white focus:border-transparent focus:ring-0 focus:outline-none",
                hasError ? "border-red-500 border" : ""
            ].join(" ")}
            ref={wrapperRef}>
            <div className="flex items-center">
                <input
                    type="text"
                    name={id}
                    id={id}
                    value={value}
                    autoComplete="off"
                    onChange={(e) => onChange(e.target.value)}
                    className={[
                        "py-3 px-4 leading-6 block w-full shadow-sm rounded-md bg-gray-700 text-white focus:border-transparent focus:ring-0 focus:outline-none",
                        hasError ? "border-red-500 border" : ""
                    ].join(" ")}
                />
                <button
                    type="button"
                    className="flex justify-between items-center text-white font-semibold text-left"
                    onClick={() => setIsOpen(!isOpen)}
                >
                    <span className='py-3 px-4 bg-gray-900'>
                        {isOpen ? <ChevronUp className='w-6 h-6' /> : <ChevronDown className='w-6 h-6' />}
                    </span>
                </button>

                <span className="pl-4">
                    {ipSubnet}
                </span>
            </div>
            {isOpen && (
                <ul className="absolute bg-white rounded-md overflow-hidden top-full w-full z-10">
                    {cidrToIpList(ipSubnet)
                        .filter(ip => !ipsInUse?.includes(ip))
                        .slice(0, 5)
                        .map((ip, index) => (
                            <li
                                key={index}
                                className="cursor-pointer p-2 text-gray-900"
                                onClick={() => handleSelect(ip)}
                            >
                                {ip}
                            </li>
                        ))}
                </ul>
            )}
        </div>
    </>);
}

export default IpInput;