"use client"
import React, { useEffect, useState } from 'react';
import { useSearchParams, useRouter } from 'next/navigation';
import {
    Layout,
    Typography,
    Card,
    Row,
    Col,
    Rate,
    Button,
    Tag,
    Space
} from 'antd';
import {
    ArrowLeftOutlined,
    EnvironmentOutlined,
    ClockCircleOutlined,
    PhoneOutlined,
} from '@ant-design/icons';

const { Content, Header } = Layout;
const { Title, Text } = Typography;

interface Restaurant {
    id: string;
    name: string;
    address: string;
    phone: string;
    rating: number;
    reviewCount: number;
    openTime: string;
    closeTime: string;
    isOpen: boolean;
    image: string;
    distance: string;
    priceRange: string;
    specialties: string[];
}

const RestaurantsPage: React.FC = () => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const [restaurants, setRestaurants] = useState<Restaurant[]>([]);
    const [dish, setDish] = useState<string>('');
    const [location, setLocation] = useState<string>('');

    useEffect(() => {
        const dishParam = searchParams.get('dish');
        const locationParam = searchParams.get('location');
        
        if (dishParam) setDish(decodeURIComponent(dishParam));
        if (locationParam) setLocation(decodeURIComponent(locationParam));

        // Mock data cho các cửa hàng
        const mockRestaurants: Restaurant[] = [
            {
                id: '1',
                name: 'Phở Hùng',
                address: '123 Nguyễn Văn Dậu, Bình Thạnh, TP.HCM',
                phone: '0901234567',
                rating: 4.5,
                reviewCount: 324,
                openTime: '06:00',
                closeTime: '22:00',
                isOpen: true,
                image: 'https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=400',
                distance: '0.8 km',
                priceRange: '50,000 - 80,000đ',
                specialties: ['Phở Bò', 'Phở Gà', 'Bún Bò Huế']
            },
            {
                id: '2',
                name: 'Quán Việt',
                address: '456 Đinh Bộ Lĩnh, Bình Thạnh, TP.HCM',
                phone: '0907654321',
                rating: 4.2,
                reviewCount: 156,
                openTime: '07:00',
                closeTime: '21:30',
                isOpen: true,
                image: 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400',
                distance: '1.2 km',
                priceRange: '45,000 - 75,000đ',
                specialties: ['Phở Đặc Biệt', 'Bánh Mì', 'Cơm Tấm']
            },
            {
                id: '3',
                name: 'Bếp Nhà',
                address: '789 Xô Viết Nghệ Tĩnh, Bình Thạnh, TP.HCM',
                phone: '0908765432',
                rating: 4.7,
                reviewCount: 89,
                openTime: '08:00',
                closeTime: '20:00',
                isOpen: false,
                image: 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400',
                distance: '1.5 km',
                priceRange: '60,000 - 90,000đ',
                specialties: ['Phở Truyền Thống', 'Bánh Cuốn', 'Chả Cá']
            }
        ];

        setRestaurants(mockRestaurants);
    }, [searchParams]);

    const handleBackClick = () => {
        router.back();
    };

    return (
        <Layout style={{
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%)'
        }}>
            <Header style={{
                background: 'rgba(0, 0, 0, 0.8)',
                padding: '0 24px',
                borderBottom: '1px solid rgba(255, 122, 0, 0.2)'
            }}>
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    height: '100%'
                }}>
                    <Button
                        type="text"
                        icon={<ArrowLeftOutlined />}
                        onClick={handleBackClick}
                        style={{
                            color: '#ff7a00',
                            marginRight: '16px'
                        }}
                    />
                    <Title level={3} style={{
                        color: '#ffffff',
                        margin: 0,
                        flex: 1
                    }}>
                        Quán ăn có {dish} tại {location}
                    </Title>
                </div>
            </Header>

            <Content style={{
                padding: '32px',
                overflow: 'auto'
            }}>
                <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                    <Row gutter={[24, 24]}>
                        {restaurants.map((restaurant) => (
                            <Col xs={24} lg={12} key={restaurant.id}>
                                <Card
                                    hoverable
                                    style={{
                                        background: 'rgba(255, 255, 255, 0.08)',
                                        border: '1px solid rgba(255, 122, 0, 0.2)',
                                        borderRadius: '16px',
                                        overflow: 'hidden'
                                    }}
                                    bodyStyle={{ padding: '0' }}
                                >
                                    <Row>
                                        <Col xs={24} sm={8}>
                                            <div style={{
                                                height: '200px',
                                                backgroundImage: `url(${restaurant.image})`,
                                                backgroundSize: 'cover',
                                                backgroundPosition: 'center',
                                                position: 'relative'
                                            }}>
                                                <div style={{
                                                    position: 'absolute',
                                                    top: '12px',
                                                    right: '12px',
                                                    background: restaurant.isOpen ? 'rgba(0, 255, 0, 0.8)' : 'rgba(255, 0, 0, 0.8)',
                                                    color: '#ffffff',
                                                    padding: '4px 8px',
                                                    borderRadius: '12px',
                                                    fontSize: '12px',
                                                    fontWeight: 'bold'
                                                }}>
                                                    {restaurant.isOpen ? 'Đang mở' : 'Đã đóng'}
                                                </div>
                                            </div>
                                        </Col>
                                        <Col xs={24} sm={16}>
                                            <div style={{ padding: '20px' }}>
                                                <div style={{
                                                    display: 'flex',
                                                    justifyContent: 'space-between',
                                                    alignItems: 'flex-start',
                                                    marginBottom: '12px'
                                                }}>
                                                    <Title level={4} style={{
                                                        color: '#ffffff',
                                                        margin: 0
                                                    }}>
                                                        {restaurant.name}
                                                    </Title>
                                                    <Text style={{
                                                        color: '#ff7a00',
                                                        fontSize: '14px',
                                                        fontWeight: 'bold'
                                                    }}>
                                                        {restaurant.distance}
                                                    </Text>
                                                </div>

                                                <Space direction="vertical" size="small" style={{ width: '100%' }}>
                                                    <div style={{ display: 'flex', alignItems: 'center' }}>
                                                        <Rate
                                                            disabled
                                                            defaultValue={restaurant.rating}
                                                            style={{ fontSize: '14px' }}
                                                        />
                                                        <Text style={{
                                                            color: '#ffffff',
                                                            marginLeft: '8px',
                                                            fontSize: '14px'
                                                        }}>
                                                            {restaurant.rating} ({restaurant.reviewCount} đánh giá)
                                                        </Text>
                                                    </div>

                                                    <div style={{ display: 'flex', alignItems: 'center' }}>
                                                        <EnvironmentOutlined style={{ color: '#ff7a00', marginRight: '8px' }} />
                                                        <Text style={{ color: 'rgba(255, 255, 255, 0.8)', fontSize: '13px' }}>
                                                            {restaurant.address}
                                                        </Text>
                                                    </div>

                                                    <div style={{ display: 'flex', alignItems: 'center' }}>
                                                        <ClockCircleOutlined style={{ color: '#ff7a00', marginRight: '8px' }} />
                                                        <Text style={{ color: 'rgba(255, 255, 255, 0.8)', fontSize: '13px' }}>
                                                            {restaurant.openTime} - {restaurant.closeTime}
                                                        </Text>
                                                    </div>

                                                    <div style={{ display: 'flex', alignItems: 'center' }}>
                                                        <PhoneOutlined style={{ color: '#ff7a00', marginRight: '8px' }} />
                                                        <Text style={{ color: 'rgba(255, 255, 255, 0.8)', fontSize: '13px' }}>
                                                            {restaurant.phone}
                                                        </Text>
                                                    </div>

                                                    <Text style={{
                                                        color: '#ff7a00',
                                                        fontSize: '14px',
                                                        fontWeight: 'bold'
                                                    }}>
                                                        {restaurant.priceRange}
                                                    </Text>

                                                    <div style={{ marginTop: '8px' }}>
                                                        {restaurant.specialties.map((specialty, index) => (
                                                            <Tag
                                                                key={index}
                                                                style={{
                                                                    background: 'rgba(255, 122, 0, 0.1)',
                                                                    border: '1px solid rgba(255, 122, 0, 0.3)',
                                                                    color: '#ff7a00',
                                                                    marginBottom: '4px'
                                                                }}
                                                            >
                                                                {specialty}
                                                            </Tag>
                                                        ))}
                                                    </div>
                                                </Space>

                                                <div style={{
                                                    marginTop: '16px',
                                                    display: 'flex',
                                                    gap: '8px'
                                                }}>
                                                    <Button
                                                        type="primary"
                                                        style={{
                                                            background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                                            border: 'none',
                                                            borderRadius: '8px'
                                                        }}
                                                    >
                                                        Xem menu
                                                    </Button>
                                                    <Button
                                                        style={{
                                                            background: 'rgba(255, 122, 0, 0.1)',
                                                            border: '1px solid rgba(255, 122, 0, 0.4)',
                                                            color: '#ff7a00',
                                                            borderRadius: '8px'
                                                        }}
                                                    >
                                                        Gọi ngay
                                                    </Button>
                                                </div>
                                            </div>
                                        </Col>
                                    </Row>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                </div>
            </Content>
        </Layout>
    );
};

export default RestaurantsPage;
