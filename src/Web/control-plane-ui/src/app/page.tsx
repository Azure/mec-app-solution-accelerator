import Link from 'next/link'
import PageTitle from './components/PageTitle'
import { Camera, CameraDash, Sim } from './components/icons'
import { env } from 'next-runtime-env';

export default function Home() {
  const alertsUiUri = env('NEXT_PUBLIC_ALERTS_UI_URI') ?? '';
  const menuItems = [{
    name: 'Cameras Dashboard',
    icon: <CameraDash className='h-16 w-16' />,
    path: '/camera-dashboard',
    external: false,
  }, {
    name: 'Alerts Dashboard',
    icon: <CameraDash className='h-16 w-16' />,
    path: alertsUiUri,
    external: true,
  }, {
    name: '5G SIMs Provisioning',
    icon: <Sim className="h-16 w-16" />,
    path: '/sims',
    external: false,
  }, {
    name: 'Cameras Provisioning',
    icon: <Camera className='h-16 w-16' />,
    path: '/cameras',
    external: false,
  }]

  return (
    <>
      <PageTitle title='Global' />
      <>
        <div className="mt-8 grid grid-cols-2 gap-8">
          {menuItems.map((item, index) => {
            const linkStyle = "flex flex-col items-center justify-center p-8 bg-gray-500 text-white rounded-lg hover:bg-gradient-brand hover:text-black";
            const linkContent = <>
              {item.icon}
              <span className='mt-4'>{item.name}</span>
            </>;

            return item.external ? <a key={item.name} href={item.path} className={linkStyle}>{linkContent}</a> :
              <Link key={item.name} href={item.path} className={linkStyle}>{linkContent}</Link>;
          })}
        </div>
      </>
    </>
  )
}
