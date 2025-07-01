'use client';

import { useState, useEffect, useCallback } from 'react';
import { adminService } from '@/services/AdminService';
import { useAuth } from '@/hooks/auths/authContext';

interface User {
    uid: string;           // Changed from uid to uid
    email: string;
    name: string;
    roles: string[];
    status?: {
        isDisabled: boolean;
        emailVerified: boolean;
        lastSignInTime: string | null;
        creationTime: string;
    };
}

export function UserManagement() {
    const [users, setUsers] = useState<User[]>([]);
    const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
    const [loading, setLoading] = useState(false);
    const [selectedUser, setSelectedUser] = useState<User | null>(null);
    const [showRoleModal, setShowRoleModal] = useState(false);
    const [newRole, setNewRole] = useState('');
    const [searchQuery, setSearchQuery] = useState('');
    const { currentUser, userRoles } = useAuth();

    const filterUsersBasedOnRole = (allUsers: User[]): User[] => {
        if (!currentUser) return allUsers;
        
        return allUsers.filter(user => {
            // Don't show current user
            if (user.uid === currentUser.uid) {
                return false;
            }
            
            // If current user is Admin, don't show other Admins
            if (userRoles.includes('Admin') && user.roles.includes('Admin')) {
                return false;
            }
            
            return true;
        });
    };

    const loadAllUsers = useCallback(async () => {
        setLoading(true);
        try {
            const response = await adminService.getAllUsers();
            console.log('Raw API Response:', response);
            
            // Convert ApiUser[] to User[] - no need to call getUserStatus as data is already there
            const convertedUsers: User[] = response.users.map((apiUser) => {
                console.log('Converting API User:', apiUser);
                const converted = {
                    uid: apiUser.uid,
                    email: apiUser.email,
                    name: apiUser.displayName || '',
                    roles: apiUser.roles,
                    status: {
                        isDisabled: apiUser.isDisabled,
                        emailVerified: apiUser.isEmailVerified,
                        lastSignInTime: apiUser.lastSignInTime,
                        creationTime: apiUser.creationTime
                    }
                };
                console.log('Converted User:', converted);
                return converted;
            });
            
            // Filter out current user and same-level users
            const filteredUsers = filterUsersBasedOnRole(convertedUsers);
            console.log('Final filtered users:', filteredUsers);
            setUsers(filteredUsers);
        } catch (error) {
            console.error('Error loading users:', error);
            alert('Failed to load users');
        } finally {
            setLoading(false);
        }
    }, [currentUser, userRoles]);

    // Load all users on component mount
    useEffect(() => {
        loadAllUsers();
    }, [loadAllUsers]);

    // Filter users based on search query
    useEffect(() => {
        if (!searchQuery.trim()) {
            setFilteredUsers(users);
        } else {
            const query = searchQuery.toLowerCase();
            const filtered = users.filter(user => {
                const email = user.email || '';
                const name = user.name || '';
                return email.toLowerCase().includes(query) || 
                       name.toLowerCase().includes(query);
            });
            setFilteredUsers(filtered);
        }
    }, [searchQuery, users]);

    const handleAddRole = async () => {
        console.log('handleAddRole called with selectedUser:', selectedUser);
        console.log('selectedUser keys:', selectedUser ? Object.keys(selectedUser) : 'null');
        console.log('selectedUser.uid:', selectedUser?.uid);
        console.log('selectedUser has uid property:', selectedUser ? 'uid' in selectedUser : false);
        
        if (!selectedUser || !newRole) {
            console.error('Missing selectedUser or newRole:', { selectedUser, newRole });
            alert('Please select a user and role');
            return;
        }

        if (!selectedUser.uid) {
            console.error('Missing uid in selectedUser:', selectedUser);
            console.error('selectedUser keys:', Object.keys(selectedUser));
            alert('User uid is missing');
            return;
        }

        try {
            const addRoleRequest = {
                uid: selectedUser.uid,
                roleName: newRole
            };
            
            console.log('Calling addRole with:', addRoleRequest);
            await adminService.addRole(addRoleRequest);

            // Update local state for both users and filteredUsers
            const updateUser = (user: User) =>
                user.uid === selectedUser.uid 
                    ? { ...user, roles: [...user.roles, newRole] }
                    : user;

            setUsers(prevUsers => prevUsers.map(updateUser));
            setFilteredUsers(prevUsers => prevUsers.map(updateUser));

            alert(`Role "${newRole}" added successfully`);
            setShowRoleModal(false);
            setNewRole('');
        } catch (error) {
            console.error('Error adding role:', error);
            alert(`Failed to add role: ${error instanceof Error ? error.message : 'Unknown error'}`);
        }
    };

    const handleRemoveRole = async (user: User, role: string) => {
        try {
            await adminService.removeRole({
                uid: user.uid,
                roleName: role
            });

            // Update local state for both users and filteredUsers
            const updateUser = (u: User) =>
                u.uid === user.uid 
                    ? { ...u, roles: u.roles.filter(r => r !== role) }
                    : u;

            setUsers(prevUsers => prevUsers.map(updateUser));
            setFilteredUsers(prevUsers => prevUsers.map(updateUser));

            alert(`Role "${role}" removed successfully`);
        } catch (error) {
            console.error('Error removing role:', error);
            alert('Failed to remove role');
        }
    };

    const handleToggleUserStatus = async (user: User) => {
        console.log('handleToggleUserStatus called with user:', user);
        console.log('user keys:', Object.keys(user));
        console.log('user.uid:', user.uid);
        console.log('user has uid property:', 'uid' in user);
        
        if (!user.status) {
            console.error('User status is missing:', user);
            alert('User status information is not available');
            return;
        }

        if (!user.uid) {
            console.error('User uid is missing:', user);
            console.error('User object keys:', Object.keys(user));
            alert('User uid is missing');
            return;
        }

        try {
            console.log('Toggling user status for ID:', user.uid);
            
            if (user.status.isDisabled) {
                await adminService.enableUser(user.uid);
                alert('User enabled successfully');
            } else {
                await adminService.disableUser(user.uid);
                alert('User disabled successfully');
            }

            // Update local state for both users and filteredUsers
            const updateUser = (u: User) =>
                u.uid === user.uid && u.status
                    ? { ...u, status: { ...u.status, isDisabled: !u.status.isDisabled } }
                    : u;

            setUsers(prevUsers => prevUsers.map(updateUser));
            setFilteredUsers(prevUsers => prevUsers.map(updateUser));
        } catch (error) {
            console.error('Error toggling user status:', error);
            alert(`Failed to update user status: ${error instanceof Error ? error.message : 'Unknown error'}`);
        }
    };

    const handleDeleteUser = async (user: User) => {
        if (!confirm(`Are you sure you want to delete user "${user.email}"? This action cannot be undone.`)) {
            return;
        }

        try {
            await adminService.deleteUser(user.uid);
            
            // Remove user from both states
            setUsers(prevUsers => prevUsers.filter(u => u.uid !== user.uid));
            setFilteredUsers(prevUsers => prevUsers.filter(u => u.uid !== user.uid));
            
            alert('User deleted successfully');
        } catch (error) {
            console.error('Error deleting user:', error);
            alert('Failed to delete user');
        }
    };

    return (
        <div className="space-y-6">
            {/* Search Section */}
            <div className="bg-white p-6 rounded-lg shadow">
                <h2 className="text-lg font-medium text-gray-900 mb-4">Search Users</h2>
                <div className="flex gap-4">
                    <input
                        type="text"
                        placeholder="Search by email or name..."
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="flex-1 border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                    />
                </div>
            </div>

            {/* Users List */}
            <div className="bg-white rounded-lg shadow overflow-hidden">
                <div className="px-6 py-4 border-b border-gray-200">
                    <h2 className="text-lg font-medium text-gray-900">
                        Users ({filteredUsers.length})
                        {searchQuery && (
                            <span className="text-sm font-normal text-gray-500 ml-2">
                                - Filtered by &quot;{searchQuery}&quot;
                            </span>
                        )}
                    </h2>
                </div>
                
                {loading ? (
                    <div className="text-center py-12">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500 mx-auto"></div>
                        <p className="mt-2 text-gray-500">Loading users...</p>
                    </div>
                ) : filteredUsers.length === 0 ? (
                    <div className="text-center py-12 text-gray-500">
                        {searchQuery ? 'No users found matching your search.' : 'No users available.'}
                    </div>
                ) : (
                    <div className="overflow-x-auto">
                        <table className="min-w-full divide-y divide-gray-200">
                            <thead className="bg-gray-50">
                                <tr>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        User
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Roles
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Status
                                    </th>
                                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                        Actions
                                    </th>
                                </tr>
                            </thead>
                            <tbody className="bg-white divide-y divide-gray-200">
                                {filteredUsers.map((user) => (
                                    <tr key={user.uid}>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div>
                                                <div className="text-sm font-medium text-gray-900">
                                                    {user.name || 'No name'}
                                                </div>
                                                <div className="text-sm text-gray-500">{user.email}</div>
                                                <div className="text-xs text-gray-400 mt-1">
                                                    ID: {user.uid}
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className="flex flex-wrap gap-1">
                                                {user.roles.length === 0 ? (
                                                    <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-gray-100 text-gray-800">
                                                        User
                                                    </span>
                                                ) : (
                                                    user.roles.map((role) => (
                                                        <span
                                                            key={role}
                                                            className="inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                                                        >
                                                            {role}
                                                            <button
                                                                onClick={() => handleRemoveRole(user, role)}
                                                                className="ml-1 text-blue-600 hover:text-blue-800"
                                                                title="Remove role"
                                                            >
                                                                ×
                                                            </button>
                                                        </span>
                                                    ))
                                                )}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            {user.status ? (
                                                <div className="space-y-1">
                                                    <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                                                        user.status.isDisabled 
                                                            ? 'bg-red-100 text-red-800' 
                                                            : 'bg-green-100 text-green-800'
                                                    }`}>
                                                        {user.status.isDisabled ? 'Disabled' : 'Active'}
                                                    </span>
                                                    <div className="text-xs text-gray-500">
                                                        Email: {user.status.emailVerified ? 'Verified' : 'Not verified'}
                                                    </div>
                                                    <div className="text-xs text-gray-500">
                                                        Last login: {user.status.lastSignInTime ? 
                                                            new Date(user.status.lastSignInTime).toLocaleDateString() : 
                                                            'Never'
                                                        }
                                                    </div>
                                                </div>
                                            ) : (
                                                <span className="text-gray-400">Unknown</span>
                                            )}
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                                            <div className="flex gap-2">
                                                <button
                                                    onClick={() => {
                                                        setSelectedUser(user);
                                                        setShowRoleModal(true);
                                                    }}
                                                    className="text-blue-600 hover:text-blue-900"
                                                >
                                                    Add Role
                                                </button>
                                                {user.status && (
                                                    <button
                                                        onClick={() => handleToggleUserStatus(user)}
                                                        className={`${
                                                            user.status.isDisabled 
                                                                ? 'text-green-600 hover:text-green-900' 
                                                                : 'text-yellow-600 hover:text-yellow-900'
                                                        }`}
                                                    >
                                                        {user.status.isDisabled ? 'Enable' : 'Disable'}
                                                    </button>
                                                )}
                                                <button
                                                    onClick={() => handleDeleteUser(user)}
                                                    className="text-red-600 hover:text-red-900"
                                                >
                                                    Delete
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>

            {/* Add Role Modal */}
            {showRoleModal && selectedUser && (
                <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex items-center justify-center z-50">
                    <div className="bg-white p-6 rounded-lg shadow-xl max-w-md w-full mx-4">
                        <h3 className="text-lg font-medium text-gray-900 mb-4">
                            Add Role to {selectedUser.email}
                        </h3>
                        <div className="space-y-4">
                            <select
                                value={newRole}
                                onChange={(e) => setNewRole(e.target.value)}
                                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                            >
                                <option value="">Select a role</option>
                                <option value="Admin">Admin</option>
                                <option value="Business">Business</option>
                                <option value="User">User</option>
                            </select>
                            <div className="flex justify-end gap-3">
                                <button
                                    onClick={() => {
                                        setShowRoleModal(false);
                                        setNewRole('');
                                    }}
                                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 hover:bg-gray-200 rounded-md"
                                >
                                    Cancel
                                </button>
                                <button
                                    onClick={handleAddRole}
                                    disabled={!newRole}
                                    className="px-4 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-md disabled:opacity-50"
                                >
                                    Add Role
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
