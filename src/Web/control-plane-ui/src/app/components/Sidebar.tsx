'use client'

import { usePathname } from 'next/navigation';
import React, { useState } from 'react';
import { Camera, CameraDash, Dashboard, Sim } from '@/app/components/icons';
import Link from 'next/link';
import { RootState } from '@/stores/store';
import { useSelector } from 'react-redux';
import { env } from 'next-runtime-env';

export type MenuItemProps = {
  name: string;
  icon: JSX.Element;
  path: string;
}

const MenuItem = ({
  name,
  icon,
  path,
}: MenuItemProps) => {
  const pathname = usePathname();
  const isActive = pathname === path;

  return (
    <Link key={path} href={path} className={[
      'pl-20',
      'py-4',
      'flex',
      'items-center',
      'gap-5',
      'hover:bg-gray-700',
      'transition-colors',
      'duration-200',
      isActive ? 'text-brand' : '',
    ].join(' ')}>
      {icon}
      <span>{name}</span>
    </Link>
  );
}

const Sidebar = () => {
  const alertsUiUri = env('NEXT_PUBLIC_ALERTS_UI_URI') ?? '';
  const logoSettings = useSelector((state: RootState) => state.settings.logo);
  const logoSvg = logoSettings === 'leavesbank' ? 'leavesbank-logo.svg' :
    'microsoft-logo.svg';

  const menuItems = [{
    name: 'Global',
    icon: <Dashboard className='h-8 w-8' />,
    path: '/',
  }, {
    name: 'SIMs Provisioning',
    icon: <Sim className='h-8 w-8' />,
    path: '/sims',
  }, {
    name: 'Cameras Provisioning',
    icon: <Camera className="h-8 w-8" />,
    path: '/cameras',
  }, {
    name: 'Cameras Dashboard',
    icon: <CameraDash className='h-8 w-8' />,
    path: '/camera-dashboard',
  }, {
    name: 'Alerts Dashboard',
    icon: <CameraDash className='h-8 w-8' />,
    path: alertsUiUri,
  }].map(x => MenuItem({
    name: x.name,
    icon: x.icon,
    path: x.path,
  }));

  return (
    <div className="min-h-screen h-full bg-gray-500 text-white min-w-[20rem] w-[20rem] left-0 border-r border-gray-300">
      <Link href="/" className="flex items-center space-x-2">
        <img src={`/${logoSvg}`} alt="Company Logo" className="h-14 w-[13rem] ml-9 mt-9" />
      </Link>
      <nav className='mt-16 flex flex-col gap-8'>
        {menuItems}
      </nav>
    </div>
  );
};

export default Sidebar;