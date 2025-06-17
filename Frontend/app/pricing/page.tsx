"use client"
import Prices from '@/components/PricingPage/Prices'
import { AuthProvider } from '@/hooks/auths/authContext'
import React from 'react'

function page() {
  return (
    <AuthProvider>
      <Prices />
    </AuthProvider>
  )
}

export default page
