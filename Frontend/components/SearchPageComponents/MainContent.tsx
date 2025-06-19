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
    const [displayedItems, setDisplayedItems] = useState(6);
    const [loading, setLoading] = useState(false);
    const router = useRouter();    // Mock data cho hình ảnh món ăn - Expanded list
    const foodImages: FoodImage[] = [
        { id: '1', title: 'Phở Bò Hà Nội', image: 'https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=300', description: 'Món phở truyền thống', category: 'Món mặn', location: 'Bình Thạnh' },
        { id: '2', title: 'Bánh Mì Việt Nam', image: 'https://images.unsplash.com/photo-1558030006-450675393462?w=300', description: 'Bánh mì thơm ngon', category: 'Món nhanh', location: 'Quận 1' },
        { id: '4', title: 'Chè Đậu Xanh', image: 'https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=300', description: 'Chè ngọt mát', category: 'Món tráng miệng', location: 'Quận 3' },
        { id: '5', title: 'Salad Rau Củ', image: 'https://images.unsplash.com/photo-1512621776951-a57141f2eefd?w=300', description: 'Salad tươi mát', category: 'Món chay', location: 'Thủ Đức' },
        { id: '7', title: 'Sushi Nhật Bản', image: 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=300', description: 'Sushi tươi ngon', category: 'Món Á', location: 'Quận 7' },
        { id: '7', title: 'Sushi Nhật Bản', image: 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=300', description: 'Sushi tươi ngon', category: 'Món Á', location: 'Quận 7' },
        { id: '7', title: 'Sushi Nhật Bản', image: 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=300', description: 'Sushi tươi ngon', category: 'Món Á', location: 'Quận 7' },
        { id: '7', title: 'Sushi Nhật Bản', image: 'https://images.unsplash.com/photo-1553621042-f6e147245754?w=300', description: 'Sushi tươi ngon', category: 'Món Á', location: 'Quận 7' },
        { id: '9', title: 'Bánh Flan', image: 'https://images.unsplash.com/photo-1551024506-0bccd828d307?w=300', description: 'Bánh flan mềm mịn', category: 'Món tráng miệng', location: 'Bình Thạnh' },
        { id: '10', title: 'Cơm Tấm Sài Gòn', image: 'https://images.unsplash.com/photo-1512058564366-18510be2db19?w=300', description: 'Cơm tấm đặc trưng', category: 'Món mặn', location: 'Quận 1' },
        { id: '11', title: 'Bún Thịt Nướng', image: 'https://images.unsplash.com/photo-1559847844-5315695dadae?w=300', description: 'Bún thịt nướng thơm ngon', category: 'Món mặn', location: 'Quận 3' },
        { id: '12', title: 'Gà Rán KFC', image: 'https://images.unsplash.com/photo-1562967914-608f82629710?w=300', description: 'Gà rán giòn tan', category: 'Món nhanh', location: 'Quận 7' },
        { id: '13', title: 'Pizza Hải Sản', image: 'https://images.unsplash.com/photo-1565299624946-b28f40a0ca4b?w=300', description: 'Pizza hải sản tươi ngon', category: 'Món Á', location: 'Thủ Đức' },
        { id: '14', title: 'Trà Sữa Trân Châu', image: 'https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=300', description: 'Trà sữa ngọt ngào', category: 'Đồ uống', location: 'Bình Thạnh' },
        { id: '15', title: 'Smoothie Xoài', image: 'https://images.unsplash.com/photo-1546173159-315724a31696?w=300', description: 'Smoothie xoài mát lạnh', category: 'Đồ uống', location: 'Quận 1' },
        { id: '16', title: 'Bánh Cuốn Hà Nội', image: 'https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300', description: 'Bánh cuốn mềm mịn', category: 'Món chay', location: 'Quận 3' },
        { id: '17', title: 'Chả Cá Lã Vọng', image: 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=300', description: 'Chả cá truyền thống', category: 'Món mặn', location: 'Quận 7' },
        { id: '18', title: 'Kem Dừa', image: 'https://images.unsplash.com/photo-1567306226416-28f0efdc88ce?w=300', description: 'Kem dừa mát lạnh', category: 'Món tráng miệng', location: 'Thủ Đức' },
        { id: '19', title: 'Hủ Tiếu Nam Vang', image: 'https://images.unsplash.com/photo-1569718212165-3a8278d5f624?w=300', description: 'Hủ tiếu đậm đà', category: 'Món mặn', location: 'Bình Thạnh' },
        { id: '20', title: 'Bánh Tráng Nướng', image: 'https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=300', description: 'Bánh tráng nướng giòn', category: 'Món nhanh', location: 'Quận 1' }
    ];

    const filterOptions = ['Tất cả', 'Món chay', 'Món mặn', 'Món tráng miệng', 'Đồ uống', 'Món nhanh', 'Món Á'];    // Lọc hình ảnh theo filter
    const filteredImages = selectedFilter === 'Tất cả'
        ? foodImages
        : foodImages.filter(item => item.category === selectedFilter);

    // Hiển thị items theo số lượng đã load
    const visibleImages = filteredImages.slice(0, displayedItems);    // Infinite scroll effect
    useEffect(() => {
        const handleScroll = () => {
            if (loading) return;
            
            // Try both document and content container
            const container = document.querySelector('[style*="overflow: auto"]') as HTMLElement;
            const scrollElement = container || document.documentElement;
            
            const scrollTop = scrollElement.scrollTop;
            const scrollHeight = scrollElement.scrollHeight;
            const clientHeight = scrollElement.clientHeight;
            
            console.log('Scroll detected:', { scrollTop, scrollHeight, clientHeight, displayedItems, filteredImagesLength: filteredImages.length });
            
            if (scrollTop + clientHeight >= scrollHeight - 100) {
                if (displayedItems < filteredImages.length) {
                    console.log('Loading more items...');
                    setLoading(true);
                    setTimeout(() => {
                        setDisplayedItems(prev => Math.min(prev + 6, filteredImages.length));
                        setLoading(false);
                    }, 800);
                }
            }
        };

        // Add scroll listener to both window and potential container
        window.addEventListener('scroll', handleScroll);
        const container = document.querySelector('[style*="overflow: auto"]');
        if (container) {
            container.addEventListener('scroll', handleScroll);
        }
        
        return () => {
            window.removeEventListener('scroll', handleScroll);
            if (container) {
                container.removeEventListener('scroll', handleScroll);
            }
        };
    }, [loading, displayedItems, filteredImages.length]);

    // Reset displayed items when filter changes
    useEffect(() => {
        setDisplayedItems(6);
        setLoading(false);
    }, [selectedFilter]);    const handleFilterClick = (filter: string) => {
        setSelectedFilter(filter);
        // displayedItems sẽ được reset trong useEffect
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
    );       return (
        <Content style={{
            padding: '32px',
            overflow: 'auto',
            flex: 1,
            maxHeight: 'calc(100vh - 200px)',
            scrollbarWidth: 'thin',
            scrollbarColor: 'rgba(255, 122, 0, 0.3) transparent'
        }} 
        id="main-content-scroll"
        onScroll={(e) => {
            if (loading) return;
            
            const target = e.currentTarget;
            const scrollTop = target.scrollTop;
            const scrollHeight = target.scrollHeight;
            const clientHeight = target.clientHeight;
            
            console.log('Content scroll:', { scrollTop, scrollHeight, clientHeight, displayedItems, filteredImagesLength: filteredImages.length });
            
            if (scrollTop + clientHeight >= scrollHeight - 100) {
                if (displayedItems < filteredImages.length) {
                    console.log('Loading more from content scroll...');
                    setLoading(true);
                    setTimeout(() => {
                        setDisplayedItems(prev => Math.min(prev + 6, filteredImages.length));
                        setLoading(false);
                    }, 800);
                }
            }
        }}>{!selectedChat ? (
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
                </Dropdown>                {/* Food Images Grid với Infinite Scroll */}
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
                        <Text style={{ color: '#ffffff', fontSize: '16px' }}>
                           {visibleImages.length} / {filteredImages.length} món ăn
                        </Text>
                    </div>

                    {/* Vertical Grid Layout */}
                    <Row gutter={[16, 16]}>
                        {visibleImages.map((food) => (
                            <Col xs={12} md={8} key={food.id}>
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
                                        e.currentTarget.style.background = 'rgba(255, 255, 255, 0.08)';
                                    }}
                                    onMouseLeave={(e) => {
                                        e.currentTarget.style.transform = 'translateY(0)';
                                        e.currentTarget.style.background = 'rgba(255, 255, 255, 0.05)';
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
                    </Row>                    {/* Loading indicator */}
                    {loading && (
                        <div style={{
                            textAlign: 'center',
                            marginTop: '20px',
                            padding: '20px'
                        }}>
                            <div style={{
                                width: '40px',
                                height: '40px',
                                border: '3px solid rgba(255, 122, 0, 0.3)',
                                borderTop: '3px solid #ff7a00',
                                borderRadius: '50%',
                                animation: 'spin 1s linear infinite',
                                margin: '0 auto 10px'
                            }}></div>
                            <Text style={{ color: '#ffffff' }}>Đang tải thêm món ăn...</Text>
                        </div>
                    )}

                    {/* Load More Button (fallback) */}
                    {!loading && displayedItems < filteredImages.length && (
                        <div style={{ textAlign: 'center', marginTop: '20px' }}>
                            <Button
                                type="primary"
                                onClick={() => {
                                    setLoading(true);
                                    setTimeout(() => {
                                        setDisplayedItems(prev => Math.min(prev + 6, filteredImages.length));
                                        setLoading(false);
                                    }, 800);
                                }}
                                style={{
                                    background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                    border: 'none',
                                    borderRadius: '8px',
                                    padding: '10px 30px',
                                    height: 'auto'
                                }}
                            >
                                Tải thêm món ăn ({filteredImages.length - displayedItems} còn lại)
                            </Button>
                        </div>
                    )}

                    {/* End of results indicator */}
                    {displayedItems >= filteredImages.length && filteredImages.length > 0 && (
                        <div style={{
                            textAlign: 'center',
                            marginTop: '20px',
                            padding: '20px',
                            color: 'rgba(255, 255, 255, 0.6)'
                        }}>
                            <Text style={{ color: 'rgba(255, 255, 255, 0.6)' }}>
                                Đã hiển thị tất cả kết quả
                            </Text>
                        </div>                    )}
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

        {/* CSS Animation for spinner */}
        <style jsx>{`
            @keyframes spin {
                0% { transform: rotate(0deg); }
                100% { transform: rotate(360deg); }
            }
        `}</style>
        </Content>
    );
};

export default MainContent;
