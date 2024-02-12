'use client'

import { usePathname } from 'next/navigation';
import React from 'react';
import { Camera, CameraDash, Dashboard, Sim } from '@/app/components/icons';
import Link from 'next/link';

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
  const menuItems = [{
    name: 'Dashboard',
    icon: <Dashboard className='h-8 w-8' />, //className="h-8 w-8" />,
    path: '/',
  }, {
    name: 'SIM Management',
    icon: <Sim className='h-8 w-8' />,
    path: '/sims',
  }, {
    name: 'Cameras',
    icon: <Camera className="h-8 w-8" />,
    path: '/cameras',
  }, {
    name: 'Camera Dashboard',
    icon: <CameraDash className='h-8 w-8' />,
    path: '/camera-dashboard',
  }].map(x => MenuItem({
    name: x.name,
    icon: x.icon,
    path: x.path,
  }));

  return (
    <div className="min-h-screen h-full bg-gray-500 text-white min-w-[20rem] w-[20rem] left-0 border-r border-gray-300">
      <Link href="/" className="flex items-center space-x-2">
        <img src="/microsoft-logo.svg" alt="Company Logo" className="h-14 w-[13rem] ml-9 mt-9" />
      </Link>
      <nav className='mt-16 flex flex-col gap-8'>
        {menuItems}
      </nav>
    </div>
  );
};

export default Sidebar;