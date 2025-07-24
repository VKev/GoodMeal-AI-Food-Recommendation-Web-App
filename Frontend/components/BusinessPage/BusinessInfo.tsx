'use client';

import { useState, useEffect } from 'react';
import { 
    Card, 
    Button, 
    Modal, 
    Form, 
    Input, 
    Space, 
    Typography, 
    Row, 
    Col, 
    Tag, 
    notification,
    Statistic,
    Divider,
    Spin,
    Alert
} from 'antd';
import { 
    EditOutlined, 
    BankOutlined,
    PhoneOutlined,
    MailOutlined,
    GlobalOutlined,
    EnvironmentOutlined,
    CalendarOutlined,
    CheckCircleOutlined,
    StopOutlined,
    InfoCircleOutlined
    // Đã bỏ DeleteOutlined
} from '@ant-design/icons';
import { businessService, Business, CreateBusinessRequest, UpdateBusinessRequest } from '@/services/BusinessService';

const { Title, Text } = Typography;
const { TextArea } = Input;

interface BusinessInfoProps {
    onBusinessChange?: (business: Business) => void;
}

export function BusinessInfo({ onBusinessChange }: BusinessInfoProps) {
    const [business, setBusiness] = useState<Business | null>(null);
    const [loading, setLoading] = useState(true);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [form] = Form.useForm();
    const [api, contextHolder] = notification.useNotification();
    // Bỏ useState showDeleteModal

    useEffect(() => {
        loadBusinessInfo();
    }, []);

    const loadBusinessInfo = async () => {
        try {
            setLoading(true);
            const business = await businessService.getMyBusiness();
            if (business) {
                setBusiness(business);
                onBusinessChange?.(business);
            } else {
                setBusiness(null);
            }
        } catch (error) {
            console.error('Error loading business:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể tải thông tin doanh nghiệp',
            });
        } finally {
            setLoading(false);
        }
    };

    const handleCreateBusiness = async (values: CreateBusinessRequest) => {
        try {
            setSubmitting(true);
            const newBusiness = await businessService.createBusiness(values);
            setBusiness(newBusiness);
            onBusinessChange?.(newBusiness);
            setShowCreateModal(false);
            form.resetFields();
            
            api.success({
                message: 'Thành công',
                description: 'Tạo doanh nghiệp mới thành công',
            });
        } catch (error) {
            console.error('Error creating business:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể tạo doanh nghiệp mới',
            });
        } finally {
            setSubmitting(false);
        }
    };

    const handleUpdateBusiness = async (values: UpdateBusinessRequest) => {
        if (!business) return;
        
        try {
            setSubmitting(true);
            await businessService.updateBusiness(business.id, values);
            await loadBusinessInfo(); // Gọi lại API lấy thông tin mới nhất
            setShowEditModal(false);
            form.resetFields();
            
            api.success({
                message: 'Thành công',
                description: 'Cập nhật thông tin doanh nghiệp thành công',
            });
        } catch (error) {
            console.error('Error updating business:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể cập nhật thông tin doanh nghiệp',
            });
        } finally {
            setSubmitting(false);
        }
    };

    const handleToggleStatus = async () => {
        if (!business) return;
        
        try {
            if (business.isActive) {
                await businessService.deactivateBusiness(business.id);
            } else {
                await businessService.activateBusiness(business.id);
            }
            
            const updatedBusiness = { ...business, isActive: !business.isActive };
            setBusiness(updatedBusiness);
            onBusinessChange?.(updatedBusiness);
            
            api.success({
                message: 'Thành công',
                description: `${business.isActive ? 'Tạm ngừng' : 'Kích hoạt'} doanh nghiệp thành công`,
            });
        } catch (error) {
            console.error('Error toggling business status:', error);
            api.error({
                message: 'Lỗi',
                description: 'Không thể thay đổi trạng thái doanh nghiệp',
            });
        }
    };

    const handleEdit = () => {
        if (!business) return;
        
        form.setFieldsValue({
            name: business.name,
            description: business.description,
            address: business.address,
            phone: business.phone,
            email: business.email,
            website: business.website
        });
        setShowEditModal(true);
    };

    // Bỏ handleDeleteBusiness

    if (loading) {
        return (
            <div style={{ textAlign: 'center', padding: '50px' }}>
                <Spin size="large" />
                <div style={{ marginTop: '16px' }}>
                    <Text>Đang tải thông tin doanh nghiệp...</Text>
                </div>
            </div>
        );
    }

    if (!business) {
        return (
            <div style={{ backgroundColor: '#ffffff', padding: '24px', minHeight: '400px' }}>
                {contextHolder}
                <Alert
                    message="Chưa có doanh nghiệp"
                    description="Bạn chưa có doanh nghiệp nào. Hãy tạo doanh nghiệp đầu tiên để bắt đầu quản lý nhà hàng và món ăn."
                    type="info"
                    icon={<InfoCircleOutlined />}
                    action={
                        <Button type="primary" onClick={() => setShowCreateModal(true)}>
                            Tạo doanh nghiệp mới
                        </Button>
                    }
                    style={{ marginBottom: '24px' }}
                />

                <Modal
                    title={
                        <Space>
                            <BankOutlined />
                            Tạo doanh nghiệp mới
                        </Space>
                    }
                    open={showCreateModal}
                    onCancel={() => {
                        setShowCreateModal(false);
                        form.resetFields();
                    }}
                    footer={null}
                    width={800}
                >
                    <Form
                        form={form}
                        layout="vertical"
                        onFinish={handleCreateBusiness}
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
                            
                            <Col xs={24}>
                                <Form.Item
                                    label="Lý do tạo doanh nghiệp"
                                    name="createReason"
                                    rules={[{ required: true, message: 'Vui lòng nhập lý do tạo doanh nghiệp!' }]}
                                >
                                    <TextArea 
                                        placeholder="Ví dụ: Mở rộng kinh doanh, quản lý chuỗi nhà hàng..."
                                        rows={3}
                                    />
                                </Form.Item>
                            </Col>
                        </Row>
                        
                        <Row justify="end" gutter={[8, 0]} style={{ marginTop: '24px' }}>
                            <Col>
                                <Button onClick={() => {
                                    setShowCreateModal(false);
                                    form.resetFields();
                                }}>
                                    Hủy
                                </Button>
                            </Col>
                            <Col>
                                <Button type="primary" htmlType="submit" loading={submitting}>
                                    Tạo doanh nghiệp
                                </Button>
                            </Col>
                        </Row>
                    </Form>
                </Modal>
            </div>
        );
    }

    return (
        <div style={{ backgroundColor: '#ffffff', padding: '0', minHeight: '100%' }}>
            {contextHolder}
            
            <Row justify="space-between" align="middle" style={{ marginBottom: '24px' }}>
                <Col>
                    <Title level={2} style={{ margin: 0, color: '#52c41a' }}>
                        <BankOutlined style={{ marginRight: '12px' }} />
                        Thông tin Doanh nghiệp
                    </Title>
                </Col>
                <Col>
                    <Space>
                        <Button 
                            type="primary" 
                            icon={<EditOutlined />} 
                            onClick={handleEdit}
                        >
                            Chỉnh sửa
                        </Button>
                        <Button 
                            type={business.isActive ? 'default' : 'primary'}
                            onClick={handleToggleStatus}
                        >
                            {business.isActive ? 'Tạm ngừng' : 'Kích hoạt'}
                        </Button>
                        {/* Đã bỏ nút Xóa doanh nghiệp */}
                    </Space>
                </Col>
            </Row>

            <Card style={{ marginBottom: '24px' }}>
                <Row gutter={[16, 16]}>
                    <Col xs={24} lg={16}>
                        <Space direction="vertical" size="large" style={{ width: '100%' }}>
                            <div>
                                <div style={{ 
                                    fontSize: '24px', 
                                    fontWeight: 600, 
                                    color: '#52c41a',
                                    marginBottom: '8px'
                                }}>
                                    {business.name}
                                </div>
                                <Tag 
                                    icon={business.isActive ? <CheckCircleOutlined /> : <StopOutlined />}
                                    color={business.isActive ? 'success' : 'error'}
                                    style={{ fontSize: '14px' }}
                                >
                                    {business.isActive ? 'Đang hoạt động' : 'Tạm ngừng'}
                                </Tag>
                            </div>

                            <div>
                                <Text type="secondary" style={{ fontSize: '16px' }}>
                                    {business.description}
                                </Text>
                            </div>

                            <Divider />

                            <Row gutter={[16, 16]}>
                                <Col xs={24} md={12}>
                                    <Space>
                                        <EnvironmentOutlined style={{ color: '#fa541c' }} />
                                        <div>
                                            <div style={{ fontWeight: 500 }}>Địa chỉ</div>
                                            <Text type="secondary">{business.address}</Text>
                                        </div>
                                    </Space>
                                </Col>
                                <Col xs={24} md={12}>
                                    <Space>
                                        <PhoneOutlined style={{ color: '#52c41a' }} />
                                        <div>
                                            <div style={{ fontWeight: 500 }}>Số điện thoại</div>
                                            <Text type="secondary">{business.phone}</Text>
                                        </div>
                                    </Space>
                                </Col>
                                <Col xs={24} md={12}>
                                    <Space>
                                        <MailOutlined style={{ color: '#1890ff' }} />
                                        <div>
                                            <div style={{ fontWeight: 500 }}>Email</div>
                                            <Text type="secondary">{business.email}</Text>
                                        </div>
                                    </Space>
                                </Col>
                                {business.website && (
                                    <Col xs={24} md={12}>
                                        <Space>
                                            <GlobalOutlined style={{ color: '#722ed1' }} />
                                            <div>
                                                <div style={{ fontWeight: 500 }}>Trang web</div>
                                                <a href={business.website} target="_blank" rel="noopener noreferrer">
                                                    {business.website}
                                                </a>
                                            </div>
                                        </Space>
                                    </Col>
                                )}
                            </Row>
                        </Space>
                    </Col>
                    
                    <Col xs={24} lg={8}>
                        <Row gutter={[16, 16]}>
                            <Col xs={12} lg={24}>
                                <Statistic
                                    title="Ngày tạo"
                                    value={new Date(business.createdAt).toLocaleDateString('vi-VN')}
                                    prefix={<CalendarOutlined />}
                                />
                            </Col>
                            <Col xs={12} lg={24}>
                                <Statistic
                                    title="Lần cập nhật cuối"
                                    value={new Date(business.updatedAt).toLocaleDateString('vi-VN')}
                                    prefix={<CalendarOutlined />}
                                />
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </Card>

            <Modal
                title={
                    <Space>
                        <BankOutlined />
                        Chỉnh sửa thông tin doanh nghiệp
                    </Space>
                }
                open={showEditModal}
                onCancel={() => {
                    setShowEditModal(false);
                    form.resetFields();
                }}
                footer={null}
                width={800}
            >
                <Form
                    form={form}
                    layout="vertical"
                    onFinish={handleUpdateBusiness}
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
                                setShowEditModal(false);
                                form.resetFields();
                            }}>
                                Hủy
                            </Button>
                        </Col>
                        <Col>
                            <Button type="primary" htmlType="submit" loading={submitting}>
                                Cập nhật
                            </Button>
                        </Col>
                    </Row>
                </Form>
            </Modal>

            {/* Bỏ Modal xác nhận xóa doanh nghiệp */}
        </div>
    );
} 