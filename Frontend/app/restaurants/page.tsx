"use client"
import React, { Suspense } from 'react';
import RestaurantsPage from '@/components/RestaurantsPage/RestaurantsPage';

function RestaurantsPageWithParams() {
    return <RestaurantsPage />;
}

function Page() {
    return (
        <div>
            <Suspense fallback={<div>Loading...</div>}>
                <RestaurantsPageWithParams />
            </Suspense>
        </div>
    );
}

export default Page;
