import React, { useRef } from 'react';
import Papa, { ParseResult } from 'papaparse';
import { SIM } from '@/models/sim';

interface CSVRow {
  [key: string]: string;
}

export type SimCSVLoaderProps = {
  onSimLoaded: (sim: Partial<SIM>) => void;
}

const SimCSVLoader = ({
  onSimLoaded
}: SimCSVLoaderProps) => {
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const files = event.target.files;
    if (!files) return;

    const file = files[0];
    Papa.parse<CSVRow>(file, {
      header: true,
      transformHeader: header => header.trim().toLowerCase(),
      complete: (result: ParseResult<CSVRow>) => {
        if (result.data.length >= 1) {
          onSimLoaded({
            name: result.data[0]['name'],
            imsi: result.data[0]['imsi'],
            iccid: result.data[0]['iccid'],
            ki: result.data[0]['ki'],
            opc: result.data[0]['opc'],
          });
        }
        // Reset the file input after processing
        if (fileInputRef.current) {
          fileInputRef.current.value = '';
        }
      }
    });
  };

  const loadCsv = () => {
    fileInputRef.current?.click();
  };

  return (
    <>
      <input
        type="file"
        accept=".csv"
        onChange={handleFileChange}
        className='hidden'
        ref={fileInputRef}
      />
      <button type='button'
        onClick={loadCsv}
        className='py-2 px-4 border rounded-full'>
        Fill from CSV
      </button>
    </>
  );
};

export default SimCSVLoader;