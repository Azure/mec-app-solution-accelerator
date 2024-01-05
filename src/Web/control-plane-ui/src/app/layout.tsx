import type { Metadata } from 'next'
import './globals.css'
import Sidebar from './components/Sidebar'

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
    <html lang="en">
      <body>
        <div className="flex min-h-screen h-full overflow-x-auto bg-gray-700">
          <Sidebar />
          <div className="flex-grow px-[6rem]">
            {children}
          </div>
        </div>
      </body>
    </html>
  )
}
