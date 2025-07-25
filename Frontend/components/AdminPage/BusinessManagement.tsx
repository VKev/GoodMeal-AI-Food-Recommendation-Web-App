'use client';

import { useState, useEffect, useCallback } from 'react';
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
    Divider
} from 'antd';
import { 
    PlusOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    SearchOutlined,
    ShopOutlined,
    PhoneOutlined,
    MailOutlined,
    GlobalOutlined,
    EnvironmentOutlined,
    CalendarOutlined,
    CheckCircleOutlined,
    StopOutlined
} from '@ant-design/icons';
import { adminService, Business, CreateBusinessRequest } from '@/services/AdminService';

const { Title, Text } = Typography;
const { TextArea } = Input;
const { Option } = Select;

export function BusinessManagement() {
    const [businesses, setBusinesses] = useState<Business[]>([]);
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [editingBusiness, setEditingBusiness] = useState<Business | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [filterStatus, setFilterStatus] = useState<'all' | 'active' | 'inactive'>('all');
    const [form] = Form.useForm();
    
    const [api, contextHolder] = notification.useNotification();

    // Filter businesses based on search and status
    const filteredBusinesses = businesses.filter(business => {
        const matchesSearch = business.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            business.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
                            business.address.toLowerCase().includes(searchTerm.toLowerCase());
        
        const matchesStatus = filterStatus === 'all' || 
                            (filterStatus === 'active' && business.isActive) ||
                            (filterStatus === 'inactive' && !business.isActive);
        
        return matchesSearch && matchesStatus;
    });

    const loadBusinesses = useCallback(async () => {
        setLoading(true);
        try {
            const response = await adminService.getAllBusinesses();
            setBusinesses(response.businesses);
        } catch (error) {
            console.error('Error loading businesses:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể tải danh sách doanh nghiệp',
            });
        } finally {
            setLoading(false);
        }
    }, [api]);

    useEffect(() => {
        loadBusinesses();
    }, [loadBusinesses]);

    const handleSubmit = async (values: CreateBusinessRequest) => {
        try {
            if (editingBusiness) {
                await adminService.updateBusiness(editingBusiness.id, values);
                api.success({
                    message: 'Thành công',
                    description: 'Cập nhật doanh nghiệp thành công',
                });
            } else {
                await adminService.createBusiness(values);
                api.success({
                    message: 'Thành công',
                    description: 'Tạo doanh nghiệp mới thành công',
                });
            }
            
            setShowModal(false);
            setEditingBusiness(null);
            form.resetFields();
            loadBusinesses();
        } catch (error) {
            console.error('Error saving business:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể lưu thông tin doanh nghiệp',
            });
        }
    };

    const handleEdit = (business: Business) => {
        setEditingBusiness(business);
        form.setFieldsValue({
            name: business.name,
            description: business.description,
            address: business.address,
            phone: business.phone,
            email: business.email,
            website: business.website
        });
        setShowModal(true);
    };

    const handleDelete = async (businessId: string) => {
        try {
            await adminService.deleteBusiness(businessId);
            setBusinesses(businesses.filter(b => b.id !== businessId));
            api.success({
                message: 'Thành công',
                description: 'Xóa doanh nghiệp thành công',
            });
        } catch (error) {
            console.error('Error deleting business:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể xóa doanh nghiệp',
            });
        }
    };

    const handleAddNew = () => {
        setEditingBusiness(null);
        form.resetFields();
        setShowModal(true);
    };

    // Define table columns
    const columns = [
        {
            title: 'Doanh nghiệp',
            key: 'business',
            width: 350,
            render: (record: Business) => (
                <Space>
                    <Avatar 
                        size="large" 
                        style={{ backgroundColor: '#1890ff' }}
                        icon={<ShopOutlined />}
                    >
                        {record.name.charAt(0).toUpperCase()}
                    </Avatar>
                    <div style={{ minWidth: 0, flex: 1 }}>
                        <div style={{ fontWeight: 600, fontSize: '16px', wordBreak: 'break-word' }}>{record.name}</div>
                        <Text type="secondary" style={{ wordBreak: 'break-word' }}>{record.description}</Text>
                        <br />
                        <Text type="secondary" style={{ wordBreak: 'break-all' }}>
                            <EnvironmentOutlined /> {record.address}
                        </Text>
                    </div>
                </Space>
            ),
        },
        {
            title: 'Thông tin liên hệ',
            key: 'contact',
            width: 250,
            render: (record: Business) => (
                <Space direction="vertical" size="small">
                    <div style={{ wordBreak: 'break-all' }}>
                        <MailOutlined style={{ marginRight: 8, color: '#1890ff' }} />
                        {record.email}
                    </div>
                    <div>
                        <PhoneOutlined style={{ marginRight: 8, color: '#52c41a' }} />
                        {record.phone}
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
            title: 'Trạng thái',
            key: 'status',
            width: 180,
            render: (record: Business) => (
                <Space direction="vertical" size="small">
                    <Tag 
                        icon={record.isActive ? <CheckCircleOutlined /> : <StopOutlined />}
                        color={record.isActive ? 'success' : 'error'}
                    >
                        {record.isActive ? 'Hoạt động' : 'Ngừng hoạt động'}
                    </Tag>
                    <Text type="secondary">
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
            render: (record: Business) => (
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
                    <Popconfirm
                        title="Xóa doanh nghiệp"
                        description="Bạn có chắc chắn muốn xóa doanh nghiệp này?"
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
                        <Title level={2} style={{ margin: 0, color: '#1890ff' }}>
                            <ShopOutlined style={{ marginRight: '12px' }} />
                            Quản lý Doanh nghiệp
                        </Title>
                        <Text type="secondary" style={{ fontSize: '16px' }}>
                            Quản lý thông tin các đối tác doanh nghiệp
                        </Text>
                    </Col>
                    <Col>
                        <Button 
                            type="primary" 
                            icon={<PlusOutlined />} 
                            size="large"
                            onClick={handleAddNew}
                        >
                            Thêm doanh nghiệp mới
                        </Button>
                    </Col>
                </Row>

                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} md={16}>
                        <Input.Search
                            placeholder="Tìm kiếm theo tên, email hoặc địa chỉ..."
                            allowClear
                            size="large"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                    <Col xs={24} md={8}>
                        <Select
                            size="large"
                            style={{ width: '100%' }}
                            value={filterStatus}
                            onChange={setFilterStatus}
                        >
                            <Option value="all">Tất cả trạng thái</Option>
                            <Option value="active">Đang hoạt động</Option>
                            <Option value="inactive">Ngừng hoạt động</Option>
                        </Select>
                    </Col>
                </Row>
                
                <Divider />
                
                <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                    <Col xs={24} sm={8}>
                        <Statistic
                            title="Doanh nghiệp hoạt động"
                            value={businesses.filter(b => b.isActive).length}
                            valueStyle={{ color: '#3f8600' }}
                            prefix={<CheckCircleOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={8}>
                        <Statistic
                            title="Doanh nghiệp ngừng hoạt động"
                            value={businesses.filter(b => !b.isActive).length}
                            valueStyle={{ color: '#cf1322' }}
                            prefix={<StopOutlined />}
                        />
                    </Col>
                    <Col xs={24} sm={8}>
                        <Statistic
                            title="Kết quả lọc"
                            value={filteredBusinesses.length}
                            valueStyle={{ color: '#1890ff' }}
                            prefix={<SearchOutlined />}
                        />
                    </Col>
                </Row>

                <Table
                        columns={columns}
                        dataSource={filteredBusinesses}
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
                                ? `Không tìm thấy doanh nghiệp nào phù hợp với "${searchTerm}"`
                                : 'Chưa có doanh nghiệp nào. Hãy thêm doanh nghiệp đầu tiên!'
                        }}
                    />

                <Modal
                    title={
                        <Space>
                            <ShopOutlined />
                            {editingBusiness ? 'Chỉnh sửa doanh nghiệp' : 'Thêm doanh nghiệp mới'}
                        </Space>
                    }
                    open={showModal}
                    onCancel={() => {
                        setShowModal(false);
                        setEditingBusiness(null);
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
                                    label="Tên doanh nghiệp"
                                    name="name"
                                    rules={[{ required: true, message: 'Vui lòng nhập tên doanh nghiệp!' }]}
                                >
                                    <Input placeholder="Nhập tên doanh nghiệp" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Mô tả"
                                    name="description"
                                    rules={[{ required: true, message: 'Vui lòng nhập mô tả!' }]}
                                >
                                    <TextArea 
                                        placeholder="Mô tả về doanh nghiệp"
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
                                    <Input placeholder="business@example.com" />
                                </Form.Item>
                            </Col>
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Trang web (Tùy chọn)"
                                    name="website"
                                >
                                    <Input placeholder="https://www.example.com" />
                                </Form.Item>
                            </Col>
                        </Row>
                        
                        <Row justify="end" gutter={[8, 0]} style={{ marginTop: '24px' }}>
                            <Col>
                                <Button onClick={() => {
                                    setShowModal(false);
                                    setEditingBusiness(null);
                                    form.resetFields();
                                }}>
                                    Hủy
                                </Button>
                            </Col>
                            <Col>
                                <Button type="primary" htmlType="submit">
                                    {editingBusiness ? 'Cập nhật' : 'Tạo mới'}
                                </Button>
                            </Col>
                        </Row>
                    </Form>
                </Modal>
            </div>
        </div>
    );
}
