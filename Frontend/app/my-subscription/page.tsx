"use client"
import React, { Suspense } from 'react';
import MySubscriptionPage from '@/components/SubscriptionPage/MySubscriptionPage';

function MySubscriptionPageWithParams() {
    return <MySubscriptionPage />;
}

function Page() {
    return (
        <div>
            <Suspense fallback={<div>Loading...</div>}>
                <MySubscriptionPageWithParams />
            </Suspense>
        </div>
    );
}

export default Page;
