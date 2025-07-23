'use client';

import { useState } from 'react';
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

const { Title, Text } = Typography;
const { TextArea } = Input;
const { Option } = Select;

interface Food {
    id: string;
    name: string;
    description: string;
    price: number;
    category: string;
    restaurantId: string;
    restaurantName: string;
    preparationTime: number;
    rating: number;
    isAvailable: boolean;
    isSpicy: boolean;
    ingredients: string[];
    createdAt: string;
}

// Mock data
const mockFoods: Food[] = [
    {
        id: '1',
        name: 'Phở Bò Tái',
        description: 'Phở bò tái đặc biệt với nước dùng trong, thịt bò tươi ngon',
        price: 85000,
        category: 'Món chính',
        restaurantId: '1',
        restaurantName: 'Nhà hàng Phương Nam',
        preparationTime: 15,
        rating: 4.6,
        isAvailable: true,
        isSpicy: false,
        ingredients: ['Bánh phở', 'Thịt bò', 'Hành lá', 'Ngò gai', 'Chanh'],
        createdAt: '2024-01-15T00:00:00Z'
    },
    {
        id: '2',
        name: 'Gà rán giòn cay',
        description: 'Gà rán Kentucky giòn cay đậm đà, ăn kèm khoai tây chiên',
        price: 125000,
        category: 'Fast Food',
        restaurantId: '2',
        restaurantName: 'KFC Landmark 81',
        preparationTime: 8,
        rating: 4.3,
        isAvailable: true,
        isSpicy: true,
        ingredients: ['Đùi gà', 'Bột tẩm', 'Gia vị cay', 'Khoai tây'],
        createdAt: '2024-02-10T00:00:00Z'
    },
    {
        id: '3',
        name: 'Sashimi cá hồi',
        description: 'Sashimi cá hồi tươi nhập khẩu từ Na Uy, cắt lát mỏng',
        price: 180000,
        category: 'Sashimi',
        restaurantId: '3',
        restaurantName: 'Sushi Hokkaido',
        preparationTime: 5,
        rating: 4.9,
        isAvailable: false,
        isSpicy: false,
        ingredients: ['Cá hồi Na Uy', 'Wasabi', 'Gừng chua', 'Xì dầu'],
        createdAt: '2024-01-20T00:00:00Z'
    }
];

const foodCategories = [
    'Món chính',
    'Khai vị',
    'Tráng miệng',
    'Đồ uống',
    'Fast Food',
    'Pizza',
    'Sushi',
    'Sashimi'
];

const mockRestaurants = [
    { id: '1', name: 'Nhà hàng Phương Nam' },
    { id: '2', name: 'KFC Landmark 81' },
    { id: '3', name: 'Sushi Hokkaido' }
];

export function FoodManagement() {
    const [foods, setFoods] = useState<Food[]>(mockFoods);
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [editingFood, setEditingFood] = useState<Food | null>(null);
    const [searchTerm, setSearchTerm] = useState('');
    const [form] = Form.useForm();
    const [api, contextHolder] = notification.useNotification();

    const filteredFoods = foods.filter(food => 
        food.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        food.description.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const handleSubmit = async (values: any) => {
        try {
            setLoading(true);
            const ingredientsArray = values.ingredients.split(',').map((i: string) => i.trim()).filter(Boolean);
            const restaurantName = mockRestaurants.find(r => r.id === values.restaurantId)?.name || '';
            
            if (editingFood) {
                const updatedFood = {
                    ...editingFood,
                    ...values,
                    ingredients: ingredientsArray,
                    restaurantName
                };
                setFoods(prev => prev.map(f => 
                    f.id === editingFood.id ? updatedFood : f
                ));
                api.success({ message: 'Cập nhật món ăn thành công' });
            } else {
                const newFood: Food = {
                    id: Date.now().toString(),
                    ...values,
                    ingredients: ingredientsArray,
                    restaurantName,
                    rating: 0,
                    isAvailable: true,
                    createdAt: new Date().toISOString()
                };
                setFoods(prev => [...prev, newFood]);
                api.success({ message: 'Tạo món ăn mới thành công' });
            }
            
            setShowModal(false);
            setEditingFood(null);
            form.resetFields();
        } catch (error) {
            api.error({ message: 'Không thể lưu thông tin món ăn' });
        } finally {
            setLoading(false);
        }
    };

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
                            {record.isSpicy && <FireOutlined style={{ marginLeft: 8, color: '#ff4d4f' }} />}
                        </div>
                        <Text type="secondary">{record.description}</Text>
                        <br />
                        <Space>
                            <Tag color="orange">{record.category}</Tag>
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
            title: 'Nhà hàng & Giá',
            key: 'restaurant',
            width: 250,
            render: (record: Food) => (
                <Space direction="vertical" size="small">
                    <Text strong style={{ color: '#52c41a' }}>{record.restaurantName}</Text>
                    <Text style={{ fontSize: '18px', fontWeight: 600, color: '#fa541c' }}>
                        <DollarOutlined /> {record.price.toLocaleString('vi-VN')} đ
                    </Text>
                    <Text type="secondary">
                        <ClockCircleOutlined /> {record.preparationTime} phút
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
                                ...record,
                                ingredients: record.ingredients.join(', ')
                            });
                            setShowModal(true);
                        }}
                    >
                        Sửa
                    </Button>
                    <Popconfirm
                        title="Xóa món ăn"
                        description="Bạn có chắc chắn muốn xóa món ăn này?"
                        onConfirm={() => {
                            setFoods(prev => prev.filter(f => f.id !== record.id));
                            api.success({ message: 'Xóa món ăn thành công' });
                        }}
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
                                    {mockRestaurants.map(restaurant => (
                                        <Option key={restaurant.id} value={restaurant.id}>
                                            {restaurant.name}
                                        </Option>
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
                                label="Danh mục"
                                name="category"
                                rules={[{ required: true, message: 'Vui lòng chọn danh mục!' }]}
                            >
                                <Select placeholder="Chọn danh mục">
                                    {foodCategories.map(category => (
                                        <Option key={category} value={category}>{category}</Option>
                                    ))}
                                </Select>
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
                                label="Nguyên liệu (cách nhau bằng dấu phẩy)"
                                name="ingredients"
                                rules={[{ required: true, message: 'Vui lòng nhập nguyên liệu!' }]}
                            >
                                <TextArea 
                                    placeholder="Thịt bò, Bánh phở, Hành lá, Ngò gai"
                                    rows={2}
                                />
                            </Form.Item>
                        </Col>
                        
                        <Col xs={24}>
                            <Form.Item label="Món cay" name="isSpicy">
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