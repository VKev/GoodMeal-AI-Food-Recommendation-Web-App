"use client"
import PaymentPage from '@/components/PaymentPage/PaymentPage'
import { AuthProvider } from '@/hooks/auths/authContext'
import React from 'react'

function page() {
  return (
    <AuthProvider>
      <PaymentPage />
    </AuthProvider>
  )
}

export default page
