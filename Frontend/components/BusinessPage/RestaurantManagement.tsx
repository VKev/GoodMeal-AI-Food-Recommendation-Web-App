'use client';

import { useState, useEffect } from 'react';
import { 
    Table, 
    Button, 
    Modal, 
    Form, 
    Input, 
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
    Rate
} from 'antd';
import { 
    PlusOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    SearchOutlined,
    BankOutlined,
    PhoneOutlined,
    MailOutlined,
    GlobalOutlined,
    EnvironmentOutlined,
    CalendarOutlined,
    CheckCircleOutlined,
    StopOutlined,
    StarOutlined,
    ClockCircleOutlined
} from '@ant-design/icons';

const { Title, Text } = Typography;
const { TextArea } = Input;
const { Option } = Select;

interface Restaurant {
    id: string;
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website?: string;
    cuisine: string;
    rating: number;
    openTime: string;
    closeTime: string;
    isActive: boolean;
    createdAt: string;
    imageUrl?: string;
}

interface CreateRestaurantRequest {
    name: string;
    description: string;
    address: string;
    phone: string;
    email: string;
    website?: string;
    cuisine: string;
    openTime: string;
    closeTime: string;
}

// Mock data
const mockRestaurants: Restaurant[] = [
    {
        id: '1',
        name: 'Nhà hàng Phương Nam',
        description: 'Chuyên món ăn miền Nam đậm đà, phong phú',
        address: '123 Nguyễn Huệ, Quận 1, TP.HCM',
        phone: '028 3823 4567',
        email: 'phuongnam@restaurant.com',
        website: 'https://phuongnam.com',
        cuisine: 'Việt Nam',
        rating: 4.5,
        openTime: '06:00',
        closeTime: '22:00',
        isActive: true,
        createdAt: '2024-01-15T00:00:00Z',
        imageUrl: '/images/restaurant1.jpg'
    },
    {
        id: '2',
        name: 'KFC Landmark 81',
        description: 'Gà rán Kentucky ngon nhất thế giới',
        address: 'Landmark 81, Vinhomes Central Park, TP.HCM',
        phone: '028 7308 8888',
        email: 'landmark81@kfc.vn',
        website: 'https://kfc.vn',
        cuisine: 'Fast Food',
        rating: 4.2,
        openTime: '07:00',
        closeTime: '23:00',
        isActive: true,
        createdAt: '2024-02-10T00:00:00Z',
    },
    {
        id: '3',
        name: 'Sushi Hokkaido',
        description: 'Sushi tươi ngon từ Hokkaido',
        address: '456 Lê Lợi, Quận 1, TP.HCM',
        phone: '028 3825 9999',
        email: 'info@sushihokkaido.vn',
        cuisine: 'Nhật Bản',
        rating: 4.8,
        openTime: '11:00',
        closeTime: '22:30',
        isActive: false,
        createdAt: '2024-01-20T00:00:00Z',
    },
    {
        id: '4',
        name: 'Pizza Hut Saigon Centre',
        description: 'Pizza Ý chính thống với nhiều topping hấp dẫn',
        address: 'Saigon Centre, 65 Lê Lợi, Quận 1, TP.HCM',
        phone: '028 3824 7777',
        email: 'saigoncentre@pizzahut.vn',
        website: 'https://pizzahut.vn',
        cuisine: 'Ý',
        rating: 4.0,
        openTime: '10:00',
        closeTime: '23:30',
        isActive: true,
        createdAt: '2024-03-05T00:00:00Z',
    },
    {
        id: '5',
        name: 'Bún bò Huế Cô Ba',
        description: 'Bún bò Huế đậm đà, chuẩn vị xứ Huế',
        address: '789 Pasteur, Quận 3, TP.HCM',
        phone: '028 3930 1234',
        email: 'bunbohue@coba.vn',
        cuisine: 'Việt Nam',
        rating: 4.6,
        openTime: '05:30',
        closeTime: '14:00',
        isActive: true,
        createdAt: '2024-02-28T00:00:00Z',
    }
];

const cuisineTypes = [
    'Việt Nam',
    'Nhật Bản',
    'Hàn Quốc',
    'Trung Quốc',
    'Thái Lan',
    'Ý',
    'Pháp',
    'Mỹ',
    'Fast Food',
    'Buffet',
    'Khác'
];

export function RestaurantManagement() {
    const [restaurants, setRestaurants] = useState<Restaurant[]>(mockRestaurants);
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [editingRestaurant, setEditingRestaurant] = useState<Restaurant | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [filterStatus, setFilterStatus] = useState<'all' | 'active' | 'inactive'>('all');
    const [filterCuisine, setFilterCuisine] = useState<string>('all');
    const [form] = Form.useForm();
    
    const [api, contextHolder] = notification.useNotification();

    // Filter restaurants based on search, status, and cuisine
    const filteredRestaurants = restaurants.filter(restaurant => {
        const matchesSearch = restaurant.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            restaurant.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            restaurant.address.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            restaurant.cuisine.toLowerCase().includes(searchTerm.toLowerCase());
        
        const matchesStatus = filterStatus === 'all' || 
                            (filterStatus === 'active' && restaurant.isActive) ||
                            (filterStatus === 'inactive' && !restaurant.isActive);
        
        const matchesCuisine = filterCuisine === 'all' || restaurant.cuisine === filterCuisine;
        
        return matchesSearch && matchesStatus && matchesCuisine;
    });

    const handleSubmit = async (values: CreateRestaurantRequest) => {
        try {
            setLoading(true);
            
            if (editingRestaurant) {
                // Update existing restaurant
                const updatedRestaurant = {
                    ...editingRestaurant,
                    ...values
                };
                setRestaurants(prev => prev.map(r => 
                    r.id === editingRestaurant.id ? updatedRestaurant : r
                ));
                
                api.success({
                    message: 'Thành công',
                    description: 'Cập nhật nhà hàng thành công',
                });
            } else {
                // Create new restaurant
                const newRestaurant: Restaurant = {
                    id: Date.now().toString(),
                    ...values,
                    rating: 0,
                    isActive: true,
                    createdAt: new Date().toISOString()
                };
                setRestaurants(prev => [...prev, newRestaurant]);
                
                api.success({
                    message: 'Thành công',
                    description: 'Tạo nhà hàng mới thành công',
                });
            }
            
            setShowModal(false);
            setEditingRestaurant(null);
            form.resetFields();
        } catch (error) {
            console.error('Error saving restaurant:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể lưu thông tin nhà hàng',
            });
        } finally {
            setLoading(false);
        }
    };

    const handleEdit = (restaurant: Restaurant) => {
        setEditingRestaurant(restaurant);
        form.setFieldsValue({
            name: restaurant.name,
            description: restaurant.description,
            address: restaurant.address,
            phone: restaurant.phone,
            email: restaurant.email,
            website: restaurant.website,
            cuisine: restaurant.cuisine,
            openTime: restaurant.openTime,
            closeTime: restaurant.closeTime
        });
        setShowModal(true);
    };

    const handleDelete = async (restaurantId: string) => {
        try {
            setRestaurants(prev => prev.filter(r => r.id !== restaurantId));
            api.success({
                message: 'Thành công',
                description: 'Xóa nhà hàng thành công',
            });
        } catch (error) {
            console.error('Error deleting restaurant:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể xóa nhà hàng',
            });
        }
    };

    const handleToggleStatus = (restaurant: Restaurant) => {
        setRestaurants(prev => prev.map(r =>
            r.id === restaurant.id ? { ...r, isActive: !r.isActive } : r
        ));
        
        api.success({
            message: 'Thành công',
            description: `${restaurant.isActive ? 'Tạm ngừng' : 'Kích hoạt'} nhà hàng thành công`,
        });
    };

    const handleAddNew = () => {
        setEditingRestaurant(null);
        form.resetFields();
        setShowModal(true);
    };

    // Define table columns
    const columns = [
        {
            title: 'Nhà hàng',
            key: 'restaurant',
            width: 350,
            render: (record: Restaurant) => (
                <Space>
                    <Avatar 
                        size="large" 
                        style={{ backgroundColor: '#52c41a' }}
                        icon={<BankOutlined />}
                    >
                        {record.name.charAt(0).toUpperCase()}
                    </Avatar>
                    <div style={{ minWidth: 0, flex: 1 }}>
                        <div style={{ fontWeight: 600, fontSize: '16px', wordBreak: 'break-word' }}>
                            {record.name}
                        </div>
                        <Text type="secondary" style={{ wordBreak: 'break-word' }}>
                            {record.description}
                        </Text>
                        <br />
                        <Space>
                            <Tag color="blue">{record.cuisine}</Tag>
                            <Space>
                                <StarOutlined style={{ color: '#faad14' }} />
                                <Text>{record.rating}</Text>
                            </Space>
                        </Space>
                    </div>
                </Space>
            ),
        },
        {
            title: 'Thông tin liên hệ',
            key: 'contact',
            width: 280,
            render: (record: Restaurant) => (
                <Space direction="vertical" size="small">
                    <div style={{ wordBreak: 'break-all' }}>
                        <MailOutlined style={{ marginRight: 8, color: '#1890ff' }} />
                        {record.email}
                    </div>
                    <div>
                        <PhoneOutlined style={{ marginRight: 8, color: '#52c41a' }} />
                        {record.phone}
                    </div>
                    <div style={{ wordBreak: 'break-all' }}>
                        <EnvironmentOutlined style={{ marginRight: 8, color: '#fa541c' }} />
                        {record.address}
                    </div>
                    {record.website && (
                        <div style={{ wordBreak: 'break-all' }}>
                            <GlobalOutlined style={{ marginRight: 8, color: '#722ed1' }} />
                            <a href={record.website} target="_blank" rel="noopener noreferrer">
                                Trang web
                            </a>
                        </div>
                    )}
                </Space>
            ),
        },
        {
            title: 'Giờ hoạt động & Trạng thái',
            key: 'status',
            width: 200,
            render: (record: Restaurant) => (
                <Space direction="vertical" size="small">
                    <div>
                        <ClockCircleOutlined style={{ marginRight: 8, color: '#1890ff' }} />
                        <Text>{record.openTime} - {record.closeTime}</Text>
                    </div>
                    <Tag 
                        icon={record.isActive ? <CheckCircleOutlined /> : <StopOutlined />}
                        color={record.isActive ? 'success' : 'error'}
                    >
                        {record.isActive ? 'Đang hoạt động' : 'Tạm ngừng'}
                    </Tag>
                    <Text type="secondary" style={{ fontSize: '12px' }}>
                        <CalendarOutlined /> {new Date(record.createdAt).toLocaleDateString('vi-VN')}
                    </Text>
                </Space>
            ),
        },
        {
            title: 'Thao tác',
            key: 'actions',
            width: 150,
            fixed: 'right' as const,
            render: (record: Restaurant) => (
                <Space direction="vertical" size="small">
                    <Button 
                        type="primary" 
                        icon={<EditOutlined />} 
                        size="small"
                        block
                        onClick={() => handleEdit(record)}
                    >
                        Sửa
                    </Button>
                    <Button 
                        type={record.isActive ? 'default' : 'primary'}
                        size="small"
                        block
                        onClick={() => handleToggleStatus(record)}
                    >
                        {record.isActive ? 'Tạm ngừng' : 'Kích hoạt'}
                    </Button>
                    <Popconfirm
                        title="Xóa nhà hàng"
                        description="Bạn có chắc chắn muốn xóa nhà hàng này?"
                        onConfirm={() => handleDelete(record.id)}
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
                        <Title level={2} style={{ margin: 0, color: '#52c41a' }}>
                            <BankOutlined style={{ marginRight: '12px' }} />
                            Quản lý Nhà hàng
                        </Title>
                        <Text type="secondary" style={{ fontSize: '16px' }}>
                            Quản lý thông tin các nhà hàng trong hệ thống
                        </Text>
                    </Col>
                    <Col>
                        <Button 
                            type="primary" 
                            icon={<PlusOutlined />} 
                            size="large"
                            onClick={handleAddNew}
                        >
                            Thêm nhà hàng mới
                        </Button>
                    </Col>
                </Row>

                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} md={12}>
                        <Input.Search
                            placeholder="Tìm kiếm theo tên, email, địa chỉ hoặc loại cuisine..."
                            allowClear
                            size="large"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                    <Col xs={24} md={6}>
                        <Select
                            size="large"
                            style={{ width: '100%' }}
                            value={filterStatus}
                            onChange={setFilterStatus}
                        >
                            <Option value="all">Tất cả trạng thái</Option>
                            <Option value="active">Đang hoạt động</Option>
                            <Option value="inactive">Tạm ngừng</Option>
                        </Select>
                    </Col>
                    <Col xs={24} md={6}>
                        <Select
                            size="large"
                            style={{ width: '100%' }}
                            value={filterCuisine}
                            onChange={setFilterCuisine}
                        >
                            <Option value="all">Tất cả cuisine</Option>
                            {cuisineTypes.map(cuisine => (
                                <Option key={cuisine} value={cuisine}>{cuisine}</Option>
                            ))}
                        </Select>
                    </Col>
                </Row>
                
                <Divider />
                
                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Nhà hàng hoạt động"
                            value={restaurants.filter(r => r.isActive).length}
                            valueStyle={{ color: '#3f8600' }}
                            prefix={<CheckCircleOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Tạm ngừng hoạt động"
                            value={restaurants.filter(r => !r.isActive).length}
                            valueStyle={{ color: '#cf1322' }}
                            prefix={<StopOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Đánh giá trung bình"
                            value={restaurants.reduce((acc, r) => acc + r.rating, 0) / restaurants.length}
                            precision={1}
                            valueStyle={{ color: '#faad14' }}
                            prefix={<StarOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={6}>
                        <Statistic
                            title="Kết quả lọc"
                            value={filteredRestaurants.length}
                            valueStyle={{ color: '#1890ff' }}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                </Row>

                <Table
                    columns={columns}
                    dataSource={filteredRestaurants}
                    loading={loading}
                    rowKey="id"
                    scroll={{ x: 'max-content' }}
                    pagination={{
                        pageSize: 10,
                        showSizeChanger: true,
                        showQuickJumper: true,
                        showTotal: (total, range) => `${range[0]}-${range[1]} của ${total} mục`,
                    }}
                    locale={{
                        emptyText: searchTerm 
                            ? `Không tìm thấy nhà hàng nào phù hợp với "${searchTerm}"`
                            : 'Chưa có nhà hàng nào. Hãy thêm nhà hàng đầu tiên!'
                    }}
                />

                <Modal
                    title={
                        <Space>
                            <BankOutlined />
                            {editingRestaurant ? 'Chỉnh sửa nhà hàng' : 'Thêm nhà hàng mới'}
                        </Space>
                    }
                    open={showModal}
                    onCancel={() => {
                        setShowModal(false);
                        setEditingRestaurant(null);
                        form.resetFields();
                    }}
                    footer={null}
                    width={800}
                >
                    <Form
                        form={form}
                        layout="vertical"
                        onFinish={handleSubmit}
                        size="large"
                    >
                        <Row gutter={[16, 0]}>
                            <Col xs={24}>
                                <Form.Item
                                    label="Tên nhà hàng"
                                    name="name"
                                    rules={[{ required: true, message: 'Vui lòng nhập tên nhà hàng!' }]}
                                >
                                    <Input placeholder="Nhập tên nhà hàng" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Mô tả"
                                    name="description"
                                    rules={[{ required: true, message: 'Vui lòng nhập mô tả!' }]}
                                >
                                    <TextArea 
                                        placeholder="Mô tả về nhà hàng"
                                        rows={4}
                                    />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Địa chỉ"
                                    name="address"
                                    rules={[{ required: true, message: 'Vui lòng nhập địa chỉ!' }]}
                                >
                                    <Input placeholder="Nhập địa chỉ đầy đủ" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Số điện thoại"
                                    name="phone"
                                    rules={[{ required: true, message: 'Vui lòng nhập số điện thoại!' }]}
                                >
                                    <Input placeholder="0123 456 789" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Email"
                                    name="email"
                                    rules={[
                                        { required: true, message: 'Vui lòng nhập email!' },
                                        { type: 'email', message: 'Email không hợp lệ!' }
                                    ]}
                                >
                                    <Input placeholder="restaurant@example.com" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Loại cuisine"
                                    name="cuisine"
                                    rules={[{ required: true, message: 'Vui lòng chọn loại cuisine!' }]}
                                >
                                    <Select placeholder="Chọn loại cuisine">
                                        {cuisineTypes.map(cuisine => (
                                            <Option key={cuisine} value={cuisine}>{cuisine}</Option>
                                        ))}
                                    </Select>
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Trang web (Tùy chọn)"
                                    name="website"
                                >
                                    <Input placeholder="https://www.example.com" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Giờ mở cửa"
                                    name="openTime"
                                    rules={[{ required: true, message: 'Vui lòng nhập giờ mở cửa!' }]}
                                >
                                    <Input placeholder="08:00" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24} md={12}>
                                <Form.Item
                                    label="Giờ đóng cửa"
                                    name="closeTime"
                                    rules={[{ required: true, message: 'Vui lòng nhập giờ đóng cửa!' }]}
                                >
                                    <Input placeholder="22:00" />
                                </Form.Item>
                            </Col>
                        </Row>
                        
                        <Row justify="end" gutter={[8, 0]} style={{ marginTop: '24px' }}>
                            <Col>
                                <Button onClick={() => {
                                    setShowModal(false);
                                    setEditingRestaurant(null);
                                    form.resetFields();
                                }}>
                                    Hủy
                                </Button>
                            </Col>
                            <Col>
                                <Button type="primary" htmlType="submit" loading={loading}>
                                    {editingRestaurant ? 'Cập nhật' : 'Tạo mới'}
                                </Button>
                            </Col>
                        </Row>
                    </Form>
                </Modal>
            </div>
        </div>
    );
} 