import React, { useState, useId, useRef, useEffect } from 'react';
import ChevronDown from '../icons/ChevronDown';
import ChevronUp from '../icons/ChevronUp';

interface ComboBoxProps {
  label: string;
  options: string[];
  selected: string | null;
  onSelect: (option: string) => void;
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

  const handleSelect = (option: string) => {
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
      <label htmlFor={id} className="block text-sm font-medium text-gray-300">{label}</label>
      <div id={id} className="relative block w-full shadow-sm sm:text-sm rounded-md bg-gray-700 text-white focus:border-transparent focus:ring-0 focus:outline-none" ref={wrapperRef}>
        <button
          type="button"
          className="flex justify-between items-center text-white font-semibold w-full text-left pl-4"
          onClick={handleToggle}
        >
          <span>{selected}</span>
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
                {option}
              </li>
            ))}
          </ul>
        )}
      </div>
    </>
  );
};

export default ComboBox;