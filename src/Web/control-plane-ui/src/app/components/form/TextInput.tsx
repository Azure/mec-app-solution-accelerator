import React, { useId } from "react";

export type TextInputProps = {
  label: string;
  value: string;
  type?: string;
  hasError?: boolean;
  onChange: (value: string) => void;
};

export const TextInput = ({
  label,
  value,
  hasError,
  type,
  onChange
}: TextInputProps) => {
  const id = useId();

  return (<>
    <label htmlFor={id} className="block text-m font-medium text-gray-100">{label}</label>
    <input
      type={type ?? "text"}
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
  </>);
}

export default TextInput;