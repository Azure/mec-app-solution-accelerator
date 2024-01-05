import React from 'react';

export type PageTitleProps = {
  title: string;
}

const PageTitle = ({
  title
}: PageTitleProps) => {
  return (
    <header className="mt-[6rem] flex justify-between items-center">
      <h1 className="font-oxygen text-xl text-white">{title}</h1>
    </header>
  );
};

export default PageTitle;