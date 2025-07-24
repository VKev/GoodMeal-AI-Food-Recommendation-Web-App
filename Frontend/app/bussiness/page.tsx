'use client';

import { useAuth } from '@/hooks/auths/authContext';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';
import { RestaurantManagement } from '@/components/BusinessPage/RestaurantManagement';
import { FoodManagement } from '@/components/BusinessPage/FoodManagement';
import { BusinessInfo } from '@/components/BusinessPage/BusinessInfo';
import { Business } from '@/services/BusinessService';
import { 
    Layout, 
    Menu, 
    Button, 
    Space, 
    Spin
} from 'antd';
import { 
    ShopOutlined, 
    LogoutOutlined,
    DashboardOutlined,
    MenuFoldOutlined,
    MenuUnfoldOutlined,
    BankOutlined,
    CoffeeOutlined
} from '@ant-design/icons';

const { Content, Sider } = Layout;

export default function BusinessPage() {
    const { isBusiness, loading, authenticated, logout } = useAuth();
    const router = useRouter();
    const [activeTab, setActiveTab] = useState<'business' | 'restaurants' | 'foods'>('business');
    const [collapsed, setCollapsed] = useState(false);
    const [currentBusiness, setCurrentBusiness] = useState<Business | null>(null);

    useEffect(() => {
        if (!loading) {
            if (!authenticated) {
                router.push('/sign-in');
                return;
            }
            
            if (!isBusiness()) {
                router.push('/c'); // Redirect non-business users
                return;
            }
        }
    }, [loading, authenticated, isBusiness, router]);

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

    if (!authenticated || !isBusiness()) {
        return null;
    }

    const menuItems = [
        {
            key: 'business',
            icon: <DashboardOutlined />,
            label: 'Thông tin Doanh nghiệp',
        },
        {
            key: 'restaurants',
            icon: <BankOutlined />,
            label: 'Quản lý Nhà hàng',
        },
        {
            key: 'foods',
            icon: <CoffeeOutlined />,
            label: 'Quản lý Món ăn',
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
            setActiveTab(key as 'restaurants' | 'foods');
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
                            <DashboardOutlined style={{ fontSize: '24px', color: '#52c41a' }} />
                            <div>
                                <div style={{ 
                                    fontSize: '16px', 
                                    fontWeight: 600, 
                                    color: '#52c41a',
                                    lineHeight: '20px'
                                }}>
                                    Business Panel
                                </div>
                                <div style={{ 
                                    fontSize: '12px', 
                                    color: '#666',
                                    lineHeight: '16px'
                                }}>
                                    Quản lý doanh nghiệp
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
                    {activeTab === 'business' && <BusinessInfo onBusinessChange={setCurrentBusiness} />}
                    {activeTab === 'restaurants' && <RestaurantManagement business={currentBusiness} />}
                    {activeTab === 'foods' && <FoodManagement businessId={currentBusiness?.id || ''} />}
                </Content>
            </Layout>
        </Layout>
    );
}
