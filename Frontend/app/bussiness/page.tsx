import Login from '@/components/LoginPage/Login'
import { AuthProvider } from '@/hooks/auths/authContext'
import React from 'react'

function page() {
  return (
    <AuthProvider>
      <Login />
    </AuthProvider>
  )
}

export default page
