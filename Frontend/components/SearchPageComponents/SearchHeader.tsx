import React from 'react';
import { useRouter } from 'next/navigation';
import {
    Layout,
    Typography,
    Flex,
    Avatar,
    Dropdown,
    Menu,
    message
} from 'antd';
import {
    UserOutlined,
    CrownOutlined,
    LogoutOutlined
} from '@ant-design/icons';
import { logOut } from '@/firebase/firebase';

const { Header } = Layout;
const { Title, Text } = Typography;

interface SearchHeaderProps {
    collapsed: boolean;
}

const SearchHeader: React.FC<SearchHeaderProps> = ({ collapsed }) => {
    const router = useRouter();

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
                break;
        }
    };

    const userMenu = (
        <Menu
            style={{
                background: 'rgba(26, 26, 29, 0.95)',
                border: '1px solid rgba(255, 122, 0, 0.3)',
                borderRadius: '8px',
                minWidth: '180px'
            }}
            onClick={({ key }) => handleMenuClick(key)}
        >
            <Menu.Item
                key="subscription"
                icon={<CrownOutlined style={{ color: '#ff7a00' }} />}
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
                    </Title>
                    <Text type="secondary" style={{ fontSize: '14px' }}>
                        Hãy nói tôi nghe tâm trạng của bạn!
                    </Text>
                </div>
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
                                border: '2px solid rgba(255, 122, 0, 0.3)'
                            }}
                            icon={<UserOutlined style={{ fontSize: '20px' }} />}
                        />
                    </div>
                </Dropdown>
            </Flex>
        </Header>
    );
};

export default SearchHeader;
