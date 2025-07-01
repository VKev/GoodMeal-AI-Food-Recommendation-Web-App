'use client';

import { useState, useEffect, useCallback } from 'react';
import { 
    Table, 
    Button, 
    Modal, 
    Select, 
    Space, 
    Typography, 
    Row, 
    Col, 
    Tag, 
    Popconfirm,
    notification,
    Statistic,
    Avatar,
    Divider,
    Input,
    Badge
} from 'antd';
import { 
    UserOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    SearchOutlined,
    PlusOutlined,
    EyeOutlined,
    EyeInvisibleOutlined,
    MailOutlined,
    CheckCircleOutlined,
    CloseCircleOutlined,
    ClockCircleOutlined,
    CrownOutlined,
    TeamOutlined,
    ShopOutlined
} from '@ant-design/icons';
import { adminService } from '@/services/AdminService';
import { useAuth } from '@/hooks/auths/authContext';

const { Title, Text } = Typography;
const { Option } = Select;

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
    
    const [api, contextHolder] = notification.useNotification();

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

            api.success({
                message: 'Thành công',
                description: `Vai trò "${newRole}" đã được thêm thành công`,
            });
            setShowRoleModal(false);
            setNewRole('');
        } catch (error) {
            console.error('Error adding role:', error);
            api.error({
                message: 'Lỗi',
                description: `Không thể thêm vai trò: ${error instanceof Error ? error.message : 'Lỗi không xác định'}`,
            });
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

            api.success({
                message: 'Thành công',
                description: `Vai trò "${role}" đã được gỡ bỏ thành công`,
            });
        } catch (error) {
            console.error('Error removing role:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể gỡ bỏ vai trò',
            });
        }
    };

    const handleToggleUserStatus = async (user: User) => {
        console.log('handleToggleUserStatus called with user:', user);
        console.log('user keys:', Object.keys(user));
        console.log('user.uid:', user.uid);
        console.log('user has uid property:', 'uid' in user);
        
        if (!user.status) {
            console.error('User status is missing:', user);
            api.error({
                message: 'Lỗi',
                description: 'Thông tin trạng thái người dùng không khả dụng',
            });
            return;
        }

        if (!user.uid) {
            console.error('User uid is missing:', user);
            console.error('User object keys:', Object.keys(user));
            api.error({
                message: 'Lỗi',
                description: 'Thiếu thông tin định danh người dùng',
            });
            return;
        }

        try {
            console.log('Toggling user status for ID:', user.uid);
            
            if (user.status.isDisabled) {
                await adminService.enableUser(user.uid);
                api.success({
                    message: 'Thành công',
                    description: 'Kích hoạt người dùng thành công',
                });
            } else {
                await adminService.disableUser(user.uid);
                api.success({
                    message: 'Thành công',
                    description: 'Vô hiệu hóa người dùng thành công',
                });
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
            api.error({
                message: 'Lỗi',
                description: `Không thể cập nhật trạng thái người dùng: ${error instanceof Error ? error.message : 'Lỗi không xác định'}`,
            });
        }
    };

    const handleDeleteUser = async (user: User) => {
        try {
            await adminService.deleteUser(user.uid);
            
            // Remove user from both states
            setUsers(prevUsers => prevUsers.filter(u => u.uid !== user.uid));
            setFilteredUsers(prevUsers => prevUsers.filter(u => u.uid !== user.uid));
            
            api.success({
                message: 'Thành công',
                description: 'Xóa người dùng thành công',
            });
        } catch (error) {
            console.error('Error deleting user:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể xóa người dùng',
            });
        }
    };

    // Get role icon and color
    const getRoleConfig = (role: string) => {
        switch (role) {
            case 'Admin':
                return { icon: <CrownOutlined />, color: 'red' };
            case 'Business':
                return { icon: <ShopOutlined />, color: 'blue' };
            default:
                return { icon: <UserOutlined />, color: 'green' };
        }
    };

    // Define table columns
    const columns = [
        {
            title: 'Thông tin người dùng',
            key: 'user',
            width: 300,
            render: (record: User) => (
                <Space>
                    <Badge 
                        status={record.status?.isDisabled ? 'error' : 'success'}
                        offset={[-5, 45]}
                    >
                        <Avatar 
                            size="large" 
                            style={{ backgroundColor: '#1890ff' }}
                            icon={<UserOutlined />}
                        >
                            {(record.name || record.email).charAt(0).toUpperCase()}
                        </Avatar>
                    </Badge>
                    <div style={{ minWidth: 0, flex: 1 }}>
                        <div style={{ fontWeight: 600, fontSize: '16px', wordBreak: 'break-word' }}>
                            {record.name || 'Chưa có tên'}
                        </div>
                        <Text type="secondary" style={{ wordBreak: 'break-all' }}>
                            <MailOutlined style={{ marginRight: 4 }} />
                            {record.email}
                        </Text>
                        <br />
                        <Text type="secondary" style={{ fontSize: '12px', wordBreak: 'break-all' }}>
                            ID: {record.uid}
                        </Text>
                    </div>
                </Space>
            ),
        },
        {
            title: 'Vai trò & Quyền hạn',
            key: 'roles',
            width: 250,
            render: (record: User) => (
                <Space direction="vertical" size="small">
                    <div>
                        {record.roles.length === 0 ? (
                            <Tag icon={<UserOutlined />} color="default">
                                Người dùng mặc định
                            </Tag>
                        ) : (
                            <Space wrap>
                                {record.roles.map((role) => {
                                    const config = getRoleConfig(role);
                                    return (
                                        <Tag
                                            key={role}
                                            icon={config.icon}
                                            color={config.color}
                                            closable
                                            onClose={() => handleRemoveRole(record, role)}
                                        >
                                            {role}
                                        </Tag>
                                    );
                                })}
                            </Space>
                        )}
                    </div>
                </Space>
            ),
        },
        {
            title: 'Trạng thái tài khoản',
            key: 'status',
            width: 200,
            render: (record: User) => (
                record.status ? (
                    <Space direction="vertical" size="small">
                        <Tag 
                            icon={record.status.isDisabled ? <CloseCircleOutlined /> : <CheckCircleOutlined />}
                            color={record.status.isDisabled ? 'error' : 'success'}
                        >
                            {record.status.isDisabled ? 'Bị vô hiệu hóa' : 'Hoạt động'}
                        </Tag>
                        <div>
                            <Text type="secondary" style={{ fontSize: '12px' }}>
                                Email: {record.status.emailVerified ? '✓ Đã xác thực' : '✗ Chưa xác thực'}
                            </Text>
                        </div>
                        <div>
                            <Text type="secondary" style={{ fontSize: '12px' }}>
                                <ClockCircleOutlined /> {
                                    record.status.lastSignInTime ? 
                                    new Date(record.status.lastSignInTime).toLocaleDateString('vi-VN') : 
                                    'Chưa từng'
                                }
                            </Text>
                        </div>
                    </Space>
                ) : (
                    <Text type="secondary">Không rõ trạng thái</Text>
                )
            ),
        },
        {
            title: 'Thao tác',
            key: 'actions',
            width: 200,
            fixed: 'right' as const,
            render: (record: User) => (
                <Space direction="vertical" size="small">
                    <Button 
                        type="primary" 
                        icon={<PlusOutlined />} 
                        size="small"
                        block
                        onClick={() => {
                            setSelectedUser(record);
                            setShowRoleModal(true);
                        }}
                    >
                        Thêm vai trò
                    </Button>
                    {record.status && (
                        <Button 
                            type={record.status.isDisabled ? 'default' : 'dashed'}
                            icon={record.status.isDisabled ? <EyeOutlined /> : <EyeInvisibleOutlined />}
                            size="small"
                            block
                            onClick={() => handleToggleUserStatus(record)}
                        >
                            {record.status.isDisabled ? 'Kích hoạt' : 'Vô hiệu hóa'}
                        </Button>
                    )}
                    <Popconfirm
                        title="Xóa người dùng"
                        description={`Bạn có chắc chắn muốn xóa người dùng "${record.email}"?`}
                        onConfirm={() => handleDeleteUser(record)}
                        okText="Có"
                        cancelText="Không"
                    >
                        <Button 
                            danger 
                            icon={<DeleteOutlined />} 
                            size="small"
                            block
                        >
                            Xóa
                        </Button>
                    </Popconfirm>
                </Space>
            ),
        },
    ];

    return (
        <div style={{ backgroundColor: '#ffffff', padding: '0', minHeight: '100%' }}>
            {contextHolder}
            <div style={{ maxWidth: '100%', margin: '0 auto' }}>
                <Row justify="space-between" align="middle" style={{ marginBottom: '24px' }}>
                    <Col>
                        <Title level={2} style={{ margin: 0, color: '#1890ff' }}>
                            <TeamOutlined style={{ marginRight: '12px' }} />
                            Quản lý Người dùng
                        </Title>
                        <Text type="secondary" style={{ fontSize: '16px' }}>
                            Quản lý tài khoản, vai trò và quyền hạn người dùng
                        </Text>
                    </Col>
                    
                </Row>

                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} md={16}>
                        <Input.Search
                            placeholder="Tìm kiếm theo email hoặc tên..."
                            allowClear
                            size="large"
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                </Row>
                
                <Divider />
                
                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Người dùng hoạt động"
                            value={users.filter(u => u.status && !u.status.isDisabled).length}
                            valueStyle={{ color: '#3f8600' }}
                            prefix={<CheckCircleOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Bị vô hiệu hóa"
                            value={users.filter(u => u.status && u.status.isDisabled).length}
                            valueStyle={{ color: '#cf1322' }}
                            prefix={<CloseCircleOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Quản trị viên"
                            value={users.filter(u => u.roles.includes('Admin')).length}
                            valueStyle={{ color: '#d4380d' }}
                            prefix={<CrownOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Kết quả lọc"
                            value={filteredUsers.length}
                            valueStyle={{ color: '#1890ff' }}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                </Row>

                <Table
                        columns={columns}
                        dataSource={filteredUsers}
                        loading={loading}
                        rowKey="uid"
                        scroll={{ x: 'max-content' }}
                        pagination={{
                            pageSize: 10,
                            showSizeChanger: true,
                            showQuickJumper: true,
                            showTotal: (total, range) => `${range[0]}-${range[1]} của ${total} mục`,
                        }}
                        locale={{
                            emptyText: searchQuery 
                                ? `Không tìm thấy người dùng nào phù hợp với "${searchQuery}"`
                                : 'Không có người dùng nào hiển thị.'
                        }}
                    />

                <Modal
                    title={
                        <Space>
                            <PlusOutlined />
                            Thêm vai trò cho {selectedUser?.email}
                        </Space>
                    }
                    open={showRoleModal}
                    onCancel={() => {
                        setShowRoleModal(false);
                        setNewRole('');
                    }}
                    footer={[
                        <Button key="cancel" onClick={() => {
                            setShowRoleModal(false);
                            setNewRole('');
                        }}>
                            Hủy
                        </Button>,
                        <Button 
                            key="submit" 
                            type="primary" 
                            disabled={!newRole}
                            onClick={handleAddRole}
                        >
                            Thêm vai trò
                        </Button>
                    ]}
                >
                    <Space direction="vertical" style={{ width: '100%' }} size="large">
                        <div>
                            <Text strong>Chọn vai trò để gán:</Text>
                            <Select
                                style={{ width: '100%', marginTop: 8 }}
                                placeholder="Chọn một vai trò..."
                                value={newRole}
                                onChange={setNewRole}
                                size="large"
                            >
                                <Option value="Admin">
                                    <Space>
                                        <CrownOutlined style={{ color: '#cf1322' }} />
                                        Admin - Toàn quyền hệ thống
                                    </Space>
                                </Option>
                                <Option value="Business">
                                    <Space>
                                        <ShopOutlined style={{ color: '#1890ff' }} />
                                        Business - Quản lý doanh nghiệp
                                    </Space>
                                </Option>
                                <Option value="User">
                                    <Space>
                                        <UserOutlined style={{ color: '#52c41a' }} />
                                        User - Người dùng tiêu chuẩn
                                    </Space>
                                </Option>
                            </Select>
                        </div>

                        <div>
                            <Text strong>Vai trò hiện tại:</Text>
                            <div style={{ marginTop: 8 }}>
                                {selectedUser?.roles.length === 0 ? (
                                    <Tag icon={<UserOutlined />} color="default">
                                        Không có vai trò nào
                                    </Tag>
                                ) : (
                                    <Space wrap>
                                        {selectedUser?.roles.map((role) => {
                                            const config = getRoleConfig(role);
                                            return (
                                                <Tag
                                                    key={role}
                                                    icon={config.icon}
                                                    color={config.color}
                                                >
                                                    {role}
                                                </Tag>
                                            );
                                        })}
                                    </Space>
                                )}
                            </div>
                        </div>
                    </Space>
                </Modal>
            </div>
        </div>
    );
}
