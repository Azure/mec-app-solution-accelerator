import React from 'react';

interface CheckBoxProps {
  label: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
}

export const CheckBox = ({ label, checked, onChange }: CheckBoxProps) => {
  return (
    <label className="flex items-center space-x-3">
      <input
        type="checkbox"
        checked={checked}
        onChange={(e) => onChange(e.target.checked)}
        className="form-checkbox h-6 w-6 text-blue-600 transition duration-150 ease-in-out rounded-md bg-gray-700 border-transparent focus:border-transparent focus:ring-0"
      />
      <span className="text-m font-medium text-gray-100">{label}</span>
    </label>
  );
};

export default CheckBox;