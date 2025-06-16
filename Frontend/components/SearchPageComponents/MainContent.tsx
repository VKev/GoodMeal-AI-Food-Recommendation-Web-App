import React, { useState, useEffect } from 'react';
import {
    Layout,
    Card,
    Avatar,
    Typography,
    Row,
    Col,
    Button,
    Dropdown,
    Menu
} from 'antd';
import {
    MessageOutlined,
    FilterOutlined,
    DownOutlined
} from '@ant-design/icons';
import { useRouter } from 'next/navigation';

const { Content } = Layout;
const { Title, Text } = Typography;

interface FoodImage {
    id: string;
    title: string;
    image: string;
    description: string;
    category: string;
    location: string; // Thêm trường location
}

interface MainContentProps {
    selectedChat: number | null;
}

const MainContent: React.FC<MainContentProps> = ({ selectedChat }) => {
    const [selectedFilter, setSelectedFilter] = useState('Tất cả');
    const [currentImageSet, setCurrentImageSet] = useState(0);
    const router = useRouter();

    // Mock data cho hình ảnh món ăn
    const foodImages: FoodImage[] = [
        { id: '1', title: 'Phở Bò Hà Nội', image: 'https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=300', description: 'Món phở truyền thống', category: 'Món mặn', location: 'Bình Thạnh' },
        { id: '2', title: 'Bánh Mì Việt Nam', image: 'https://images.unsplash.com/photo-1558030006-450675393462?w=300', description: 'Bánh mì thơm ngon', category: 'Món nhanh', location: 'Quận 1' },
        { id: '4', title: 'Chè Đậu Xanh', image: 'https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=300', description: 'Chè ngọt mát', category: 'Món tráng miệng', location: 'Quận 3' },
        { id: '5', title: 'Salad Rau Củ', image: 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=300', description: 'Salad tươi mát', category: 'Món chay', location: 'Thủ Đức' },
        { id: '7', title: 'Sushi Nhật Bản', image: 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=300', description: 'Sushi tươi ngon', category: 'Món Á', location: 'Quận 7' },
        { id: '9', title: 'Bánh Flan', image: 'https://images.unsplash.com/photo-1551024506-0bccd828d307?w=300', description: 'Bánh flan mềm mịn', category: 'Món tráng miệng', location: 'Bình Thạnh' },
    ];

    const filterOptions = ['Tất cả', 'Món chay', 'Món mặn', 'Món tráng miệng', 'Đồ uống', 'Món nhanh', 'Món Á'];

    // Lọc hình ảnh theo filter
    const filteredImages = selectedFilter === 'Tất cả'
        ? foodImages
        : foodImages.filter(item => item.category === selectedFilter);

    // Chia thành các set 6 hình (2x3)
    const imageSets = [];
    for (let i = 0; i < filteredImages.length; i += 6) {
        imageSets.push(filteredImages.slice(i, i + 6));
    }

    // Auto scroll effect
    useEffect(() => {
        if (imageSets.length > 1) {
            const interval = setInterval(() => {
                setCurrentImageSet(prev => (prev + 1) % imageSets.length);
            }, 4000); // Thay đổi mỗi 4 giây

            return () => clearInterval(interval);
        }
    }, [imageSets.length]);    const handleFilterClick = (filter: string) => {
        setSelectedFilter(filter);
        setCurrentImageSet(0); // Reset về set đầu tiên khi thay đổi filter
    };

    const handleFoodImageClick = (food: FoodImage) => {
        // Tạo URL với tên món ăn và địa chỉ
        const encodedTitle = encodeURIComponent(food.title);
        const encodedLocation = encodeURIComponent(food.location);
        router.push(`/restaurants?dish=${encodedTitle}&location=${encodedLocation}`);
    };

    const filterMenu = (
        <Menu>
            {filterOptions.map(option => (
                <Menu.Item
                    key={option}
                    onClick={() => handleFilterClick(option)}
                    style={{
                        color: selectedFilter === option ? '#ff7a00' : '#ffffff',
                        background: selectedFilter === option ? 'rgba(255, 122, 0, 0.1)' : 'transparent'
                    }}
                >
                    {option}
                </Menu.Item>
            ))}
        </Menu>
    );   

    return (
        <Content style={{
            padding: '32px',
            overflow: 'auto',
            flex: 1,
            maxHeight: 'calc(100vh - 200px)',
            scrollbarWidth: 'thin',
            scrollbarColor: 'rgba(255, 122, 0, 0.3) transparent'
        }}>            {!selectedChat ? (
            <div style={{ maxWidth: '1200px', margin: '0 auto' }}>



                <Dropdown
                    overlay={filterMenu}
                    trigger={['click']}
                    placement="bottomRight"
                >
                    <Button
                        icon={<FilterOutlined />}
                        style={{
                            background: 'rgba(255, 122, 0, 0.15)',
                            border: '1px solid rgba(255, 122, 0, 0.4)',
                            color: '#ff7a00',
                            borderRadius: '12px',
                            height: '44px',
                            paddingLeft: '20px',
                            paddingRight: '20px',
                            fontSize: '14px',
                            fontWeight: '500',
                            boxShadow: '0 4px 15px rgba(255, 122, 0, 0.2)',
                            transition: 'all 0.3s ease'
                        }}
                        onMouseEnter={(e) => {
                            e.currentTarget.style.background = 'rgba(255, 122, 0, 0.25)';
                            e.currentTarget.style.transform = 'translateY(-2px)';
                            e.currentTarget.style.boxShadow = '0 6px 20px rgba(255, 122, 0, 0.3)';
                        }}
                        onMouseLeave={(e) => {
                            e.currentTarget.style.background = 'rgba(255, 122, 0, 0.15)';
                            e.currentTarget.style.transform = 'translateY(0)';
                            e.currentTarget.style.boxShadow = '0 4px 15px rgba(255, 122, 0, 0.2)';
                        }}
                    >
                        {selectedFilter} <DownOutlined />
                    </Button>
                </Dropdown>                {/* Food Images Grid với Auto Scroll */}
                <div style={{
                    marginBottom: '32px',
                    position: 'relative'
                }}>
                    <div style={{
                        display: 'flex',
                        justifyContent: 'space-between',
                        alignItems: 'center',
                        marginBottom: '20px'
                    }}>

                        {imageSets.length > 1 && (
                            <div style={{ display: 'flex', gap: '6px' }}>
                                {imageSets.map((_, index) => (
                                    <div
                                        key={index}
                                        style={{
                                            width: '8px',
                                            height: '8px',
                                            borderRadius: '50%',
                                            background: index === currentImageSet ? '#ff7a00' : 'rgba(255, 255, 255, 0.3)',
                                            cursor: 'pointer',
                                            transition: 'all 0.3s ease'
                                        }}
                                        onClick={() => setCurrentImageSet(index)}
                                    />
                                ))}
                            </div>
                        )}
                    </div>

                    {imageSets.length > 0 && (
                        <div style={{
                            overflow: 'hidden',
                            borderRadius: '12px'
                        }}>
                            <div
                                style={{
                                    display: 'flex',
                                    transform: `translateX(-${currentImageSet * 100}%)`,
                                    transition: 'transform 0.5s ease-in-out'
                                }}
                            >
                                {imageSets.map((imageSet, setIndex) => (
                                    <div key={setIndex} style={{ minWidth: '100%' }}>
                                        <Row gutter={[16, 16]}>
                                            {imageSet.map((food) => (                                                <Col xs={12} md={8} key={food.id}>
                                                    <Card
                                                        hoverable
                                                        onClick={() => handleFoodImageClick(food)}
                                                        style={{
                                                            background: 'rgba(255, 255, 255, 0.05)',
                                                            border: '1px solid rgba(128, 128, 128, 0.1)',
                                                            borderRadius: '12px',
                                                            overflow: 'hidden',
                                                            cursor: 'pointer',
                                                            transition: 'all 0.3s ease'
                                                        }}
                                                        bodyStyle={{ padding: '0' }}
                                                        onMouseEnter={(e) => {
                                                            e.currentTarget.style.transform = 'translateY(-4px)';
                                                            e.currentTarget.style.boxShadow = '0 8px 25px rgba(255, 122, 0, 0.3)';
                                                        }}
                                                        onMouseLeave={(e) => {
                                                            e.currentTarget.style.transform = 'translateY(0)';
                                                            e.currentTarget.style.boxShadow = 'none';
                                                        }}
                                                        cover={
                                                            <div style={{
                                                                height: '160px',
                                                                backgroundImage: `url(${food.image})`,
                                                                backgroundSize: 'cover',
                                                                backgroundPosition: 'center',
                                                                position: 'relative'
                                                            }}>
                                                                <div style={{
                                                                    position: 'absolute',
                                                                    bottom: '0',
                                                                    left: '0',
                                                                    right: '0',
                                                                    background: 'linear-gradient(transparent, rgba(0,0,0,0.7))',
                                                                    padding: '12px',
                                                                    color: '#ffffff'
                                                                }}>
                                                                    <div style={{ fontSize: '14px', fontWeight: 'bold', marginBottom: '4px' }}>
                                                                        {food.title}
                                                                    </div>
                                                                    <div style={{ fontSize: '12px', opacity: 0.9 }}>
                                                                        {food.description}
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        }
                                                    />
                                                </Col>
                                            ))}
                                        </Row>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </div>
            </div>
        ) : (
            <div style={{
                display: 'flex',
                flexDirection: 'column',
                justifyContent: 'center',
                alignItems: 'center',
                height: '100%'
            }}>
                <Avatar
                    size={64}
                    style={{
                        background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                        marginBottom: '16px'
                    }}
                    icon={<MessageOutlined style={{ fontSize: '32px' }} />}
                />                    <Title level={3} style={{ color: '#ffffff', marginBottom: '8px' }}>
                    GoodMeal
                </Title>
                <Text type="secondary">
                    Start typing a message below to continue
                </Text>
            </div>
        )}
        </Content>
    );
};

export default MainContent;
