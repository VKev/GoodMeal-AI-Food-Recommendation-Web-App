import React from 'react';
import { useRouter } from 'next/navigation';
import {
    Layout,
    Typography,
    Flex,
    Avatar,
    Dropdown,
    Menu,
    message,
    Button
} from 'antd';
import {
    UserOutlined,
    SettingOutlined,
    LogoutOutlined,
    LoginOutlined
} from '@ant-design/icons';
import { logOut } from '@/firebase/firebase';
import { useAuth } from '@/hooks/auths/authContext';

const { Header } = Layout;
const { Title, Text } = Typography;

interface SearchHeaderProps {
    collapsed: boolean;
}

const SearchHeader: React.FC<SearchHeaderProps> = ({ collapsed }) => {
    const router = useRouter();
    const { authUser } = useAuth();


    const handleMenuClick = async (key: string) => {
        switch (key) {
            case 'subscription':
                router.push('/my-subscription');
                break;
            case 'logout':
                try {
                    await logOut();
                    message.success('Đăng xuất thành công!');
                    router.push('/sign-in');
                } catch (error) {
                    console.error('Logout error:', error);
                    message.error('Có lỗi xảy ra khi đăng xuất');
                }
                break;
            default:
                break;        }
    };

    // Get user name for avatar display
    const getUserName = () => {
        if (authUser?.name) {
            return authUser.name;
        }
        return 'N';
    };

    // Get avatar text (first letter of name or 'N' if no name)
    const getAvatarText = () => {
        if (authUser?.name) {
            return authUser.name.charAt(0).toUpperCase();
        }
        return 'N';
    };

    const userMenu = (
        <Menu
            style={{
                background: 'rgba(26, 26, 29, 0.95)',
                border: '1px solid rgba(255, 122, 0, 0.3)',
                borderRadius: '8px',
                minWidth: '180px'
            }}            onClick={({ key }) => handleMenuClick(key)}
        >
            {authUser?.email && (
                <>                    <Menu.Item
                        key="email"
                        icon={<UserOutlined style={{ color: '#1890ff' }} />}
                        style={{
                            color: '#ffffff',
                            padding: '12px 16px',
                            cursor: 'default'
                        }}
                        disabled
                    >
                        {authUser.email}
                    </Menu.Item>
                    <Menu.Divider style={{ borderColor: 'rgba(255, 122, 0, 0.2)' }} />
                </>
            )}            <Menu.Item
                key="subscription"
                icon={<SettingOutlined style={{ color: '#ff7a00' }} />}
                style={{
                    color: '#ffffff',
                    padding: '12px 16px'
                }}
            >
                Quản lý tài khoản
            </Menu.Item>
            <Menu.Divider style={{ borderColor: 'rgba(255, 122, 0, 0.2)' }} />
            <Menu.Item
                key="logout"
                icon={<LogoutOutlined style={{ color: '#ff4d4f' }} />}
                style={{
                    color: '#ffffff',
                    padding: '12px 16px'
                }}
            >
                Đăng xuất
            </Menu.Item>
        </Menu>
    );
    return (
        <Header
            style={{
                background: 'rgba(26, 26, 29, 0.5)',
                backdropFilter: 'blur(10px)',
                borderBottom: '1px solid #404040',
                padding: '0 24px',
                height: 'auto',
                maxHeight: '120px',
                lineHeight: 'normal',
                paddingTop: '24px',
                paddingBottom: '24px',
                flexShrink: 0
            }}
        >            <Flex justify="space-between" align="center">
                <div style={{ marginLeft: collapsed ? '60px' : '0', transition: 'all 0.3s' }}>
                    <Title level={2} style={{ margin: 0, color: '#ffffff' }}>
                        Xin chào! Tôi có thể giúp gì cho bạn
                    </Title>                    <Text type="secondary" style={{ fontSize: '14px' }}>
                        Hãy nói tôi nghe tâm trạng của bạn!
                    </Text>
                </div>
                
                {/* Render different UI based on authentication status */}
                {authUser ? (
                    // Authenticated user - show avatar dropdown
                    <Dropdown
                        overlay={userMenu}
                        trigger={['click']}
                        placement="bottomRight"
                    >
                        <div
                            style={{
                                cursor: 'pointer',
                                transition: 'all 0.3s ease',
                                borderRadius: '50%'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.transform = 'scale(1.05)';
                                e.currentTarget.style.filter = 'drop-shadow(0 4px 15px rgba(255, 122, 0, 0.4))';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.transform = 'scale(1)';
                                e.currentTarget.style.filter = 'none';
                            }}
                        >
                            <Avatar
                                size={48}
                                style={{
                                    background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                    border: '2px solid rgba(255, 122, 0, 0.3)',
                                    fontSize: '18px',
                                    fontWeight: 'bold'
                                }}
                            >
                                {getAvatarText()}
                            </Avatar>
                        </div>
                    </Dropdown>
                ) : (
                    // Guest user - show login button
                    <Button
                        type="primary"
                        icon={<LoginOutlined />}
                        size="large"
                        onClick={() => router.push('/sign-in')}
                        style={{
                            background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                            border: 'none',
                            borderRadius: '8px',
                            height: '48px',
                            fontSize: '16px',
                            fontWeight: 'bold',
                            boxShadow: '0 4px 15px rgba(255, 122, 0, 0.3)',
                            transition: 'all 0.3s ease'
                        }}
                        onMouseEnter={(e) => {
                            e.currentTarget.style.transform = 'translateY(-2px)';
                            e.currentTarget.style.boxShadow = '0 6px 20px rgba(255, 122, 0, 0.4)';
                        }}
                        onMouseLeave={(e) => {
                            e.currentTarget.style.transform = 'translateY(0)';
                            e.currentTarget.style.boxShadow = '0 4px 15px rgba(255, 122, 0, 0.3)';
                        }}
                    >
                        Đăng nhập
                    </Button>
                )}
            </Flex>
        </Header>
    );
};

export default SearchHeader;
