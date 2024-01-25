import React, { useState, useId, useRef, useEffect } from 'react';
import ChevronDown from '../icons/ChevronDown';
import ChevronUp from '../icons/ChevronUp';

interface ComboBoxProps {
  label: string;
  options: ComboBoxOption[];
  selected: string | null;
  onSelect: (option: ComboBoxOption) => void;
}

export type ComboBoxOption = {
  id: string;
  name: string;
}

export const ComboBox = ({
  label,
  options,
  selected,
  onSelect
}: ComboBoxProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const id = useId();
  const wrapperRef = useRef<HTMLDivElement>(null);

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  const handleSelect = (option: ComboBoxOption) => {
    onSelect(option);
    setIsOpen(false);
  };

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

  return (
    <>
      <label htmlFor={id} className="block text-m font-medium text-gray-100">{label}</label>
      <div id={id} className="relative block w-full shadow-sm sm:text-sm rounded-md bg-gray-700 text-white focus:border-transparent focus:ring-0 focus:outline-none" ref={wrapperRef}>
        <button
          type="button"
          className="flex justify-between items-center text-white font-semibold w-full text-left pl-4"
          onClick={handleToggle}
        >
          <span>{options.find(option => option.id === selected)?.name ?? ''}</span>
          <span className='py-3 px-4 bg-gray-900'>
            {isOpen ? <ChevronUp className='w-6 h-6' /> : <ChevronDown className='w-6 h-6' />}
          </span>
        </button>
        {isOpen && (
          <ul className="absolute bg-white rounded-md overflow-hidden top-full w-full z-10">
            {options.map((option, index) => (
              <li
                key={index}
                className="cursor-pointer p-2 text-gray-900"
                onClick={() => handleSelect(option)}
              >
                {option.name}
              </li>
            ))}
          </ul>
        )}
      </div>
    </>
  );
};

export default ComboBox;