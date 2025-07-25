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
    Divider,
    InputNumber
} from 'antd';
import { 
    PlusOutlined, 
    EditOutlined, 
    DeleteOutlined, 
    SearchOutlined,
    CoffeeOutlined,
    DollarOutlined,
    ClockCircleOutlined,
    CalendarOutlined,
    CheckCircleOutlined,
    StopOutlined,
    StarOutlined,
    FireOutlined
} from '@ant-design/icons';
import {
    getFoods,
    createFood,
    updateFood,
    deleteFood,
    Food
} from '@/services/RestaurantService';
import { FirebaseAuth } from '@/firebase/firebase';
import { businessService, Restaurant } from '@/services/BusinessService';

const { Title, Text } = Typography;
const { TextArea } = Input;
const { Option } = Select;

// Thêm props businessId cho FoodManagement
export function FoodManagement({ businessId }: { businessId: string }) {
    const [foods, setFoods] = useState<Food[]>([]);
    const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [editingFood, setEditingFood] = useState<Food | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [form] = Form.useForm();
    const [api, contextHolder] = notification.useNotification();

    const fetchFoods = useCallback(async () => {
        setLoading(true);
        try {
            const user = FirebaseAuth.currentUser;
            const idToken = user ? await user.getIdToken() : undefined;
            const data = await getFoods(idToken);
            setFoods(data);
        } catch (error) {
            api.error({ message: 'Không thể tải danh sách món ăn' });
        } finally {
            setLoading(false);
        }
    }, [api]);

    const fetchRestaurants = useCallback(async () => {
        if (!businessId) return;
        try {
            const data = await businessService.getBusinessRestaurants(businessId);
            setRestaurants(data);
        } catch (error) {
            api.error({ message: 'Không thể tải danh sách nhà hàng' });
        }
    }, [api, businessId]);

    useEffect(() => {
        fetchFoods();
        fetchRestaurants();
    }, [fetchFoods, fetchRestaurants]);

    const handleSubmit = async (values: any) => {
        try {
            setLoading(true);
            const user = FirebaseAuth.currentUser;
            const idToken = user ? await user.getIdToken() : undefined;
            // Chuẩn bị dữ liệu gửi lên API, loại bỏ preparationTime và isSpicy
            const { preparationTime, isSpicy, ...rest } = values;
            const foodData = {
                ...rest,
                price: Number(rest.price),
                isAvailable: rest.isAvailable !== undefined ? rest.isAvailable : true,
                imageUrl: rest.imageUrl || '',
                restaurantId: rest.restaurantId,
            };
            if (editingFood) {
                await updateFood({ ...editingFood, ...foodData }, idToken);
                api.success({ message: 'Cập nhật món ăn thành công' });
            } else {
                await createFood(foodData, idToken);
                api.success({ message: 'Tạo món ăn mới thành công' });
            }
            setShowModal(false);
            setEditingFood(null);
            form.resetFields();
            fetchFoods();
        } catch (error) {
            api.error({ message: 'Không thể lưu thông tin món ăn' });
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: string) => {
        try {
            setLoading(true);
            const user = FirebaseAuth.currentUser;
            const idToken = user ? await user.getIdToken() : undefined;
            await deleteFood(id, idToken);
            api.success({ message: 'Xóa món ăn thành công' });
            fetchFoods();
        } catch (error) {
            api.error({ message: 'Không thể xóa món ăn' });
        } finally {
            setLoading(false);
        }
    };

    const filteredFoods = foods.filter(food => 
        food.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        food.description.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const columns = [
        {
            title: 'Món ăn',
            key: 'food',
            width: 350,
            render: (record: Food) => (
                <Space>
                    <Avatar 
                        size="large" 
                        style={{ backgroundColor: '#fa8c16' }}
                        icon={<CoffeeOutlined />}
                    >
                        {record.name.charAt(0).toUpperCase()}
                    </Avatar>
                    <div style={{ minWidth: 0, flex: 1 }}>
                        <div style={{ fontWeight: 600, fontSize: '16px' }}>
                            {record.name}
                        </div>
                        <Text type="secondary">{record.description}</Text>
                    </div>
                </Space>
            ),
        },
        {
            title: 'Nhà hàng & Giá',
            key: 'restaurant',
            width: 250,
            render: (record: Food) => (
                <Space direction="vertical" size="small">
                    <Text strong style={{ color: '#52c41a' }}>{restaurants.find(r => r.id === record.restaurantId)?.name || record.restaurantId}</Text>
                    <Text style={{ fontSize: '18px', fontWeight: 600, color: '#fa541c' }}>
                        <DollarOutlined /> {record.price.toLocaleString('vi-VN')} đ
                    </Text>
                </Space>
            ),
        },
        {
            title: 'Trạng thái',
            key: 'status',
            width: 150,
            render: (record: Food) => (
                <Space direction="vertical" size="small">
                    <Tag 
                        icon={record.isAvailable ? <CheckCircleOutlined /> : <StopOutlined />}
                        color={record.isAvailable ? 'success' : 'error'}
                    >
                        {record.isAvailable ? 'Có sẵn' : 'Hết món'}
                    </Tag>
                </Space>
            ),
        },
        {
            title: 'Thao tác',
            key: 'actions',
            width: 150,
            render: (record: Food) => (
                <Space direction="vertical" size="small">
                    <Button 
                        type="primary" 
                        icon={<EditOutlined />} 
                        size="small"
                        block
                        onClick={() => {
                            setEditingFood(record);
                            form.setFieldsValue({
                                ...record
                            });
                            setShowModal(true);
                        }}
                    >
                        Sửa
                    </Button>
                    <Popconfirm
                        title="Xóa món ăn"
                        description="Bạn có chắc chắn muốn xóa món ăn này?"
                        onConfirm={() => handleDelete(record.id)}
                        okText="Có"
                        cancelText="Không"
                    >
                        <Button danger icon={<DeleteOutlined />} size="small" block>
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
            <Row justify="space-between" align="middle" style={{ marginBottom: '24px' }}>
                <Col>
                    <Title level={2} style={{ margin: 0, color: '#fa8c16' }}>
                        <CoffeeOutlined style={{ marginRight: '12px' }} />
                        Quản lý Món ăn
                    </Title>
                    <Text type="secondary" style={{ fontSize: '16px' }}>
                        Quản lý thực đơn và món ăn của các nhà hàng
                    </Text>
                </Col>
                <Col>
                    <Button 
                        type="primary" 
                        icon={<PlusOutlined />} 
                        size="large"
                        onClick={() => {
                            setEditingFood(null);
                            form.resetFields();
                            setShowModal(true);
                        }}
                    >
                        Thêm món ăn mới
                    </Button>
                </Col>
            </Row>

            <Row gutter={[16, 16]} style={{ marginBottom: '24px' }}>
                <Col xs={24} md={12}>
                    <Input.Search
                        placeholder="Tìm kiếm theo tên món, mô tả..."
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
                <Col xs={24} sm={6}>
                    <Statistic
                        title="Món ăn có sẵn"
                        value={foods.filter(f => f.isAvailable).length}
                        valueStyle={{ color: '#3f8600' }}
                        prefix={<CheckCircleOutlined />}
                    />
                </Col>
                <Col xs={24} sm={6}>
                    <Statistic
                        title="Món ăn hết"
                        value={foods.filter(f => !f.isAvailable).length}
                        valueStyle={{ color: '#cf1322' }}
                        prefix={<StopOutlined />}
                    />
                </Col>
                <Col xs={24} sm={6}>
                    <Statistic
                        title="Giá trung bình"
                        value={Math.round(foods.reduce((acc, f) => acc + f.price, 0) / foods.length)}
                        valueStyle={{ color: '#fa541c' }}
                        prefix={<DollarOutlined />}
                        suffix=" đ"
                    />
                </Col>
                <Col xs={24} sm={6}>
                    <Statistic
                        title="Kết quả lọc"
                        value={filteredFoods.length}
                        valueStyle={{ color: '#1890ff' }}
                        prefix={<SearchOutlined />}
                    />
                </Col>
            </Row>

            <Table
                columns={columns}
                dataSource={filteredFoods}
                loading={loading}
                rowKey="id"
                scroll={{ x: 'max-content' }}
                pagination={{
                    pageSize: 10,
                    showSizeChanger: true,
                    showTotal: (total, range) => `${range[0]}-${range[1]} của ${total} mục`,
                }}
            />

            <Modal
                title={
                    <Space>
                        <CoffeeOutlined />
                        {editingFood ? 'Chỉnh sửa món ăn' : 'Thêm món ăn mới'}
                    </Space>
                }
                open={showModal}
                onCancel={() => {
                    setShowModal(false);
                    setEditingFood(null);
                    form.resetFields();
                }}
                footer={null}
                width={800}
            >
                <Form form={form} layout="vertical" onFinish={handleSubmit} size="large">
                    <Row gutter={[16, 0]}>
                        <Col xs={24} md={12}>
                            <Form.Item
                                label="Tên món ăn"
                                name="name"
                                rules={[{ required: true, message: 'Vui lòng nhập tên món ăn!' }]}
                            >
                                <Input placeholder="Nhập tên món ăn" />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24} md={12}>
                            <Form.Item
                                label="Nhà hàng"
                                name="restaurantId"
                                rules={[{ required: true, message: 'Vui lòng chọn nhà hàng!' }]}
                            >
                                <Select placeholder="Chọn nhà hàng">
                                    {restaurants.map(r => (
                                        <Option key={r.id} value={r.id}>{r.name}</Option>
                                    ))}
                                </Select>
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24}>
                            <Form.Item
                                label="Mô tả"
                                name="description"
                                rules={[{ required: true, message: 'Vui lòng nhập mô tả!' }]}
                            >
                                <TextArea placeholder="Mô tả về món ăn" rows={3} />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24}>
                            <Form.Item
                                label="Link ảnh món ăn (imageUrl)"
                                name="imageUrl"
                                rules={[{ required: true, message: 'Vui lòng nhập link ảnh!' }]}
                            >
                                <Input placeholder="https://..." />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24} md={8}>
                            <Form.Item
                                label="Giá (VND)"
                                name="price"
                                rules={[{ required: true, message: 'Vui lòng nhập giá!' }]}
                            >
                                                                <InputNumber 
                                        placeholder="50000"
                                        style={{ width: '100%' }}
                                        min={0}
                                        step={1000}
                                    />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24} md={8}>
                            <Form.Item
                                label="Thời gian chế biến (phút)"
                                name="preparationTime"
                                rules={[{ required: true, message: 'Vui lòng nhập thời gian!' }]}
                            >
                                <InputNumber 
                                    placeholder="15"
                                    style={{ width: '100%' }}
                                    min={1}
                                    max={120}
                                />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24}>
                            <Form.Item
                                label="Món cay"
                                name="isSpicy"
                            >
                                <Select style={{ width: '200px' }}>
                                    <Option value={false}>Không cay</Option>
                                    <Option value={true}>Món cay <FireOutlined style={{ color: '#ff4d4f' }} /></Option>
                                </Select>
                            </Form.Item>
                        </Col>
                    </Row>
                    
                    <Row justify="end" gutter={[8, 0]} style={{ marginTop: '24px' }}>
                        <Col>
                            <Button onClick={() => {
                                setShowModal(false);
                                setEditingFood(null);
                                form.resetFields();
                            }}>
                                Hủy
                            </Button>
                        </Col>
                        <Col>
                            <Button type="primary" htmlType="submit" loading={loading}>
                                {editingFood ? 'Cập nhật' : 'Tạo mới'}
                            </Button>
                        </Col>
                    </Row>
                </Form>
            </Modal>
        </div>
    );
} 