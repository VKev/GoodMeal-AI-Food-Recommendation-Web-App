"use client"
import SearchPage from '@/components/SearchPageComponents/SearchPage'
import React, { useEffect } from 'react'
import { useSearchParams } from 'next/navigation'
import { message } from 'antd'

function page() {
    const searchParams = useSearchParams()
    
    useEffect(() => {
        // Check if user just completed payment
        const payment = searchParams.get('payment')
        const plan = searchParams.get('plan')
        
        if (payment === 'success') {
            message.success({
                content: `🎉 Welcome to ${plan === 'pro' ? 'Pro' : plan === 'business' ? 'Business' : 'Premium'}! Your subscription is now active.`,
                duration: 5,
                style: {
                    marginTop: '20vh',
                }
            })
            
            // Clear URL parameters after showing message
            window.history.replaceState({}, document.title, window.location.pathname)
        }
    }, [searchParams])

    return (
        <div>
            <SearchPage />
        </div>
    )
}

export default page
