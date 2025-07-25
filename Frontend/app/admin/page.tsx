'use client';

import { useAuth } from '@/hooks/auths/authContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { UserManagement } from '@/components/AdminPage/UserManagement';
import { BusinessManagement } from '@/components/AdminPage/BusinessManagement';
import { 
    Layout, 
    Menu, 
    Button, 
    Space, 
    Spin
} from 'antd';
import { 
    UserOutlined, 
    ShopOutlined, 
    LogoutOutlined,
    DashboardOutlined,
    MenuFoldOutlined,
    MenuUnfoldOutlined
} from '@ant-design/icons';

const { Content, Sider } = Layout;

export default function AdminPage() {
    const { isAdmin, loading, authenticated, logout } = useAuth();
    const router = useRouter();
    const [activeTab, setActiveTab] = useState<'users' | 'businesses'>('users');
    const [collapsed, setCollapsed] = useState(false);

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
            <div style={{ 
                minHeight: '100vh', 
                display: 'flex', 
                alignItems: 'center', 
                justifyContent: 'center',
                backgroundColor: '#ffffff'
            }}>
                <Spin size="large" />
            </div>
        );
    }

    if (!authenticated || !isAdmin()) {
        return null;
    }

    const menuItems = [
        {
            key: 'users',
            icon: <UserOutlined />,
            label: 'Quản lý Người dùng',
        },
        {
            key: 'businesses',
            icon: <ShopOutlined />,
            label: 'Quản lý Doanh nghiệp',
        },
        {
            type: 'divider' as const,
        },
        {
            key: 'logout',
            icon: <LogoutOutlined />,
            label: 'Đăng xuất',
            danger: true,
        },
    ];

    const handleMenuClick = ({ key }: { key: string }) => {
        if (key === 'logout') {
            logout();
        } else {
            setActiveTab(key as 'users' | 'businesses');
        }
    };

    return (
        <Layout style={{ minHeight: '100vh', backgroundColor: '#ffffff' }}>
            <Sider 
                trigger={null} 
                collapsible 
                collapsed={collapsed}
                style={{ 
                    backgroundColor: '#fff',
                    borderRight: '1px solid #e8e8e8',
                    boxShadow: 'none'
                }}
                theme="light"
                width={250}
            >
                <div style={{ 
                    height: '64px', 
                    display: 'flex', 
                    alignItems: 'center', 
                    justifyContent: collapsed ? 'center' : 'flex-start',
                    borderBottom: '1px solid #e8e8e8',
                    padding: '0 16px',
                    gap: '12px'
                }}>
                    <Button
                        type="text"
                        icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                        onClick={() => setCollapsed(!collapsed)}
                        style={{
                            fontSize: '16px',
                            width: 32,
                            height: 32,
                        }}
                    />
                    {!collapsed && (
                        <Space>
                            <DashboardOutlined style={{ fontSize: '24px', color: '#1890ff' }} />
                            <div>
                                <div style={{ 
                                    fontSize: '16px', 
                                    fontWeight: 600, 
                                    color: '#1890ff',
                                    lineHeight: '20px'
                                }}>
                                    Admin Panel
                                </div>
                                <div style={{ 
                                    fontSize: '12px', 
                                    color: '#666',
                                    lineHeight: '16px'
                                }}>
                                    Quản lý hệ thống
                                </div>
                            </div>
                        </Space>
                    )}
                </div>
                <Menu
                    mode="inline"
                    selectedKeys={[activeTab]}
                    items={menuItems}
                    onClick={handleMenuClick}
                    style={{ 
                        border: 'none',
                        backgroundColor: 'transparent',
                        fontSize: '14px',
                        marginTop: '16px'
                    }}
                />
            </Sider>
            
            <Layout style={{ backgroundColor: '#fff' }}>
                <Content style={{ 
                    margin: '24px', 
                    minHeight: 'calc(100vh - 48px)',
                    backgroundColor: '#fff'
                }}>
                    {activeTab === 'users' && <UserManagement />}
                    {activeTab === 'businesses' && <BusinessManagement />}
                </Content>
            </Layout>
        </Layout>
    );
}
