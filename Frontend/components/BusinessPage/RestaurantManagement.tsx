'use client';

import { useState, useEffect, useCallback } from 'react';
import { 
    Table, 
    Button, 
    Modal, 
    Form, 
    Input, 
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
    Alert
} from 'antd';
import { 
    PlusOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    SearchOutlined,
    BankOutlined,
    PhoneOutlined,
    EnvironmentOutlined,
    CalendarOutlined,
    CheckCircleOutlined,
    StopOutlined,
    InfoCircleOutlined
} from '@ant-design/icons';
import { 
    businessService, 
    Business, 
    Restaurant, 
    CreateRestaurantRequest 
} from '@/services/BusinessService';

const { Title, Text } = Typography;

interface RestaurantManagementProps {
    business: Business | null;
}

export function RestaurantManagement({ business }: RestaurantManagementProps) {
    const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [editingRestaurant, setEditingRestaurant] = useState<Restaurant | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [form] = Form.useForm();
    const [api, contextHolder] = notification.useNotification();

    const loadRestaurants = useCallback(async () => {
        if (!business) return;
        try {
            setLoading(true);
            const data = await businessService.getBusinessRestaurants(business.id);
            const safeData = Array.isArray(data)
                ? data.filter(r => r && typeof r.id === 'string' && typeof r.name === 'string')
                : [];
            setRestaurants(safeData);
        } catch (error) {
            console.error('Error loading restaurants:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể tải danh sách nhà hàng. Vui lòng thử lại hoặc liên hệ admin.',
            });
        } finally {
            setLoading(false);
        }
    }, [business, api]);

    useEffect(() => {
        if (business) {
            loadRestaurants();
        }
    }, [business, loadRestaurants]);

    // Khi setRestaurants, lọc bỏ phần tử null/undefined
    // const loadRestaurants = async () => {
    //     if (!business) return;
    //     try {
    //         setLoading(true);
    //         const data = await businessService.getBusinessRestaurants(business.id);
    //         // Lọc bỏ phần tử null/undefined và chỉ nhận object có id, name
    //         const safeData = Array.isArray(data)
    //             ? data.filter(r => r && typeof r.id === 'string' && typeof r.name === 'string')
    //             : [];
    //         setRestaurants(safeData);
    //     } catch (error) {
    //         console.error('Error loading restaurants:', error);
    //         api.error({
    //             message: 'Lỗi',
    //             description: 'Không thể tải danh sách nhà hàng. Vui lòng thử lại hoặc liên hệ admin.',
    //         });
    //         // Không xóa danh sách cũ nếu lỗi
    //     } finally {
    //         setLoading(false);
    //     }
    // };

    // Filter restaurants based on search
    const filteredRestaurants = restaurants.filter(restaurant => {
        if (!restaurant) return false;
        const name = typeof restaurant.name === 'string' ? restaurant.name : '';
        const address = typeof restaurant.address === 'string' ? restaurant.address : '';
        const phone = typeof restaurant.phone === 'string' ? restaurant.phone : '';
        return (
            name.toLowerCase().includes(searchTerm.toLowerCase()) ||
            address.toLowerCase().includes(searchTerm.toLowerCase()) ||
            phone.includes(searchTerm)
        );
    });

    const handleSubmit = async (values: CreateRestaurantRequest) => {
        if (!business) return;
        try {
            setSubmitting(true);
            console.log('Tạo nhà hàng với:', values);
            await businessService.createRestaurant(business.id, values);
            api.success({
                message: 'Thành công',
                description: 'Tạo nhà hàng mới thành công',
            });
            setShowModal(false);
            setEditingRestaurant(null);
            form.resetFields();
            await loadRestaurants();
        } catch (error: any) {
            console.error('Error saving restaurant:', error);
            let description = 'Không thể lưu thông tin nhà hàng';
            if (error && error.message) description = error.message;
            if (error && error.response) {
                try {
                    const res = await error.response.json();
                    if (res && res.message) description = res.message;
                    if (res && res.error) description = res.error;
                } catch {}
            }
            api.error({
                message: 'Lỗi',
                description,
            });
        } finally {
            setSubmitting(false);
        }
    };

    const handleEdit = (restaurant: Restaurant) => {
        // Hiện tại API không hỗ trợ update restaurant
        api.warning({
            message: 'Thông báo',
            description: 'Chức năng chỉnh sửa nhà hàng chưa được hỗ trợ bởi API',
        });
    };

    const handleDelete = async (restaurantId: string) => {
        // API không có delete endpoint
        api.warning({
            message: 'Thông báo',
            description: 'Chức năng xóa nhà hàng chưa được hỗ trợ bởi API',
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
                        {(typeof record.name === 'string' && record.name.length > 0) ? record.name.charAt(0).toUpperCase() : '?'}
                    </Avatar>
                    <div style={{ minWidth: 0, flex: 1 }}>
                        <div style={{ fontWeight: 600, fontSize: '16px', wordBreak: 'break-word' }}>
                            {record.name || '(Không tên)'}
                        </div>
                        <Text type="secondary" style={{ wordBreak: 'break-word' }}>
                            ID: {record.id}
                        </Text>
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
                    <div>
                        <PhoneOutlined style={{ marginRight: 8, color: '#52c41a' }} />
                        {record.phone}
                    </div>
                    <div style={{ wordBreak: 'break-all' }}>
                        <EnvironmentOutlined style={{ marginRight: 8, color: '#fa541c' }} />
                        {record.address}
                    </div>
                </Space>
            ),
        },
        {
            title: 'Trạng thái',
            key: 'status',
            width: 200,
            render: (record: Restaurant) => (
                <Space direction="vertical" size="small">
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
                        disabled
                    >
                        Sửa
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
                            disabled
                        >
                            Xóa
                        </Button>
                    </Popconfirm>
                </Space>
            ),
        },
    ];

    if (!business) {
        return (
            <div style={{ backgroundColor: '#ffffff', padding: '24px', minHeight: '400px' }}>
                <Alert
                    message="Chưa có doanh nghiệp"
                    description="Vui lòng tạo doanh nghiệp trước khi quản lý nhà hàng."
                    type="warning"
                    icon={<InfoCircleOutlined />}
                    showIcon
                />
            </div>
        );
    }

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
                            Quản lý các nhà hàng thuộc doanh nghiệp: <strong>{business.name}</strong>
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
                            placeholder="Tìm kiếm theo tên, địa chỉ hoặc số điện thoại..."
                            allowClear
                            size="large"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                </Row>
                
                <Divider />
                
                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} sm={8}>
                        <Statistic
                            title="Nhà hàng hoạt động"
                            value={restaurants.filter(r => r.isActive).length}
                            valueStyle={{ color: '#3f8600' }}
                            prefix={<CheckCircleOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={8}>
                        <Statistic
                            title="Tạm ngừng hoạt động"
                            value={restaurants.filter(r => !r.isActive).length}
                            valueStyle={{ color: '#cf1322' }}
                            prefix={<StopOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={8}>
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
                            Thêm nhà hàng mới
                        </Space>
                    }
                    open={showModal}
                    onCancel={() => {
                        setShowModal(false);
                        setEditingRestaurant(null);
                        form.resetFields();
                    }}
                    footer={null}
                    width={600}
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
                                    label="Địa chỉ"
                                    name="address"
                                    rules={[{ required: true, message: 'Vui lòng nhập địa chỉ!' }]}
                                >
                                    <Input placeholder="Nhập địa chỉ đầy đủ" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Số điện thoại"
                                    name="phone"
                                    rules={[{ required: true, message: 'Vui lòng nhập số điện thoại!' }]}
                                >
                                    <Input placeholder="0123 456 789" />
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
                                <Button type="primary" htmlType="submit" loading={submitting}>
                                    Tạo mới
                                </Button>
                            </Col>
                        </Row>
                    </Form>
                </Modal>
            </div>
        </div>
    );
}