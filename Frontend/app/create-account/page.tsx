import Create from '@/components/CreatePage/Create'
import { AuthProvider } from '@/hooks/auths/authContext'
import React from 'react'

function page() {
  return (
    <AuthProvider>
      <Create />
    </AuthProvider>

  )
}

export default page
