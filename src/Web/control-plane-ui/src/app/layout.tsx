import type { Metadata } from 'next'
import './globals.css'
import Sidebar from './components/Sidebar'
import { Providers } from '@/stores/providers'
import SettingsControl from './components/settings/SettingsControl'
import { PublicEnvScript } from 'next-runtime-env'

export const metadata: Metadata = {
  title: 'Manager',
  description: 'SIM and Cameras management',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {

  return (
    <Providers>
      <html lang="en">
        <head>
          <PublicEnvScript />
        </head>
        <body>
          <div className="flex min-h-screen h-full overflow-x-auto bg-gray-700">
            <Sidebar />
            <SettingsControl />
            <div className="flex-grow px-[6rem]">
              {children}
            </div>
          </div>
        </body>
      </html>
    </Providers>
  );
}
