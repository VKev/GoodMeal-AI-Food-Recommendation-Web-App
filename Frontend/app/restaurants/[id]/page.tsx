"use client"
import React, { Suspense } from 'react';
import RestaurantDetail from '@/components/RestaurantsPage/RestaurantDetail';

function RestaurantDetailWithParams() {
    return <RestaurantDetail />;
}

function Page() {
    return (
        <div>
            <Suspense fallback={<div>Loading...</div>}>
                <RestaurantDetailWithParams />
            </Suspense>
        </div>
    );
}

export default Page;
