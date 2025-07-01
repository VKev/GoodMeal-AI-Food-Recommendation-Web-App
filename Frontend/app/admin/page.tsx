'use client';

import { useAuth } from '@/hooks/auths/authContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { UserManagement } from '@/components/AdminPage/UserManagement';
import { BusinessManagement } from '@/components/AdminPage/BusinessManagement';

export default function AdminPage() {
    const { isAdmin, loading, authenticated, logout } = useAuth();
    const router = useRouter();
    const [activeTab, setActiveTab] = useState<'users' | 'businesses'>('users');

    useEffect(() => {
        if (!loading) {
            if (!authenticated) {
                router.push('/sign-in');
                return;
            }
            
            if (!isAdmin()) {
                router.push('/c'); // Redirect non-admin users
                return;
            }
        }
    }, [loading, authenticated, isAdmin, router]);

    if (loading) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-blue-500"></div>
            </div>
        );
    }

    if (!authenticated || !isAdmin()) {
        return null;
    }

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Header */}
            <div className="bg-white shadow">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center py-6">
                        <div>
                            <h1 className="text-3xl font-bold text-gray-900">Admin Dashboard</h1>
                            <p className="mt-1 text-sm text-gray-500">Manage users and businesses</p>
                        </div>
                        <button
                            onClick={() => logout()}
                            className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-md transition-colors"
                        >
                            Logout
                        </button>
                    </div>
                </div>
            </div>

            {/* Navigation Tabs */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
                <div className="border-b border-gray-200">
                    <nav className="-mb-px flex space-x-8">
                        <button
                            onClick={() => setActiveTab('users')}
                            className={`py-2 px-1 border-b-2 font-medium text-sm ${
                                activeTab === 'users'
                                    ? 'border-blue-500 text-blue-600'
                                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                            }`}
                        >
                            User Management
                        </button>
                        <button
                            onClick={() => setActiveTab('businesses')}
                            className={`py-2 px-1 border-b-2 font-medium text-sm ${
                                activeTab === 'businesses'
                                    ? 'border-blue-500 text-blue-600'
                                    : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                            }`}
                        >
                            Business Management
                        </button>
                    </nav>
                </div>

                {/* Tab Content */}
                <div className="mt-6">
                    {activeTab === 'users' && <UserManagement />}
                    {activeTab === 'businesses' && <BusinessManagement />}
                </div>
            </div>
        </div>
    );
}
