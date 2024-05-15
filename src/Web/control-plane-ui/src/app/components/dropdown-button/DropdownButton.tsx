import React, { useState, useRef, useEffect } from 'react';
import ChevronUp from '../icons/ChevronUp';
import ChevronDown from '../icons/ChevronDown';

export type DropdownButtonElement = {
  title: JSX.Element | string;
  onClick: () => void;
}

export type DropdownButtonProps = {
  title: JSX.Element | string;
  actions: DropdownButtonElement[];
}

const DropdownButton = ({
  title,
  actions,
}: DropdownButtonProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  const handleClickOutside = (event: MouseEvent) => {
    event.target
    if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
      setIsOpen(false);
    }
  };

  useEffect(() => {
    document.addEventListener('mousedown', handleClickOutside);
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, []);

  return (
    <div className="relative" ref={dropdownRef}>
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="text-black px-4 py-2 focus:outline-none focus:ring-2 bg-gradient-brand rounded-md"
      >
        <span className='flex gap-8'>
          {title}
          {isOpen ? <ChevronUp className='w-6 h-6' /> : <ChevronDown className='w-6 h-6' />}
        </span>
      </button>
      {isOpen && (
        <div className="origin-top-right absolute right-0 mt-2 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5">
          <div className="py-1" role="menu" aria-orientation="vertical" aria-labelledby="options-menu">
            {actions.map((action, index) => {
              return (<button key={index}
                className="block px-4 w-full text-left py-2 text-sm text-nowrap text-gray-700 hover:bg-gray-100"
                role="menuitem"
                onClick={() => {
                  setIsOpen(false);
                  action.onClick();
                }}>
                {action.title}
              </button>)
            })}
          </div>
        </div>
      )}
    </div>
  );
};

export default DropdownButton;