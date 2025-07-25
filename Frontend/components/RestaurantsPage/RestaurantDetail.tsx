"use client";
import React, { useEffect, useState, useMemo } from "react";
// ...existing code...
import { useParams, useRouter } from "next/navigation";
import {
  Layout,
  Typography,
  Card,
  Row,
  Col,
  Rate,
  Button,
  Tag,
  Divider,
  Avatar,
  Progress,
  Badge,
  Dropdown,
  Space,
  Spin,
  message,
  Modal,
} from "antd";
import {
  ArrowLeftOutlined,
  EnvironmentOutlined,
  ClockCircleOutlined,
  PhoneOutlined,
  StarFilled,
  UserOutlined,
  FilterOutlined,
  DownOutlined,
  HeartOutlined,
  ShareAltOutlined,
} from "@ant-design/icons";
import Image from "next/image";
import { 
  useRestaurantDetail,
  useRestaurantPhotos,
  useAllRestaurantReviews
} from "../../hooks/useRestaurants";
import LoadingComponent from "../LoadingComponent";

const { Content, Header } = Layout;
const { Title, Text } = Typography;

const RestaurantDetail: React.FC = () => {
  const router = useRouter();
  const params = useParams();
  
  // Local state
  const [selectedRating, setSelectedRating] = useState<number>(0);
  const [reviewSortBy, setReviewSortBy] = useState<string>("newest");
  const [photoModalVisible, setPhotoModalVisible] = useState<boolean>(false);
  const [selectedPhotoIndex, setSelectedPhotoIndex] = useState<number>(0);
  const [showAllReviews, setShowAllReviews] = useState<boolean>(false);

  // Parse restaurant ID from URL params
  const { business_id, place_id } = useMemo(() => {
    if (!params.id) return { business_id: '', place_id: '' };
    
    const id = params.id as string;
    const parts = id.split('__');
    if (parts.length === 2) {
      return {
        business_id: parts[0],
        place_id: parts[1]
      };
    }
    
    console.warn('Unable to parse restaurant ID:', id);
    return {
      business_id: id,
      place_id: id
    };
  }, [params.id]);

  // Use React Query hooks for data fetching
  const {
    data: restaurant,
    isLoading: restaurantLoading,
    error: restaurantError
  } = useRestaurantDetail(business_id, place_id);

  const {
    data: photos = [],
    isLoading: photosLoading
  } = useRestaurantPhotos(business_id, place_id);

  const {
    data: reviews = [],
    isLoading: reviewsLoading
  } = useAllRestaurantReviews(business_id, place_id);

  // Show error message if restaurant query fails
  useEffect(() => {
    if (restaurantError) {
      message.error('Không thể tải thông tin nhà hàng');
    }
  }, [restaurantError]);

  // Get restaurant image with fallback
  const getRestaurantImage = (restaurant: any) => {
    if (photos && photos.length > 0) {
      // Use the first photo from the API, but modify URL for higher quality
      const photoUrl = photos[0].src;
      // Try to get higher resolution by modifying Google image URL parameters
      if (photoUrl.includes('googleusercontent.com')) {
        return photoUrl.replace(/=w\d+-h\d+/, '=w1920-h1080').replace(/-k-no$/, '');
      }
      if (photoUrl.includes('gstatic.com')) {
        return photoUrl.replace(/=w\d+-h\d+/, '=w1920-h1080');
      }
      return photoUrl;
    }
    
    if (!restaurant) return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80';
    
    const restaurantTypes = restaurant.types?.map((type: string) => type.toLowerCase()) || [];
    
    // Check for Vietnamese food
    if (restaurantTypes.some((type: string) => type.includes('việt') || type.includes('phở') || type.includes('bún'))) {
      return 'https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=1920&h=1080&fit=crop&crop=center&q=80';
    }
    
    // Check for general restaurant/food
    if (restaurantTypes.some((type: string)  => type.includes('nhà hàng') || type.includes('quán ăn'))) {
      return 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=1920&h=1080&fit=crop&crop=center&q=80';
    }
    
    // Default restaurant image
    return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80';
  };

  // Enhanced function to get high quality image URL
  const getHighQualityImageUrl = (photoUrl: string) => {
    if (!photoUrl) return 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80';
    
    // Handle Google Photos URLs
    if (photoUrl.includes('googleusercontent.com')) {
      return photoUrl.replace(/=w\d+-h\d+/, '=w1920-h1080').replace(/-k-no$/, '');
    }
    
    // Handle Google Static URLs
    if (photoUrl.includes('gstatic.com')) {
      return photoUrl.replace(/=w\d+-h\d+/, '=w1920-h1080');
    }
    
    // Handle other Google image URLs
    if (photoUrl.includes('googleapis.com')) {
      return photoUrl.replace(/maxwidth=\d+/, 'maxwidth=1920').replace(/maxheight=\d+/, 'maxheight=1080');
    }
    
    return photoUrl;
  };

  // Parse business_id and place_id from URL params - REMOVED (moved to useMemo above)

  useEffect(() => {
    // REMOVED - all data fetching is now handled by React Query hooks
  }, []);



  const handleBackClick = () => {
    router.back();
  };

  const handlePhoneCall = () => {
    if (restaurant?.phone_number) {
      window.open(`tel:${restaurant.phone_number}`, '_self');
    }
  };

  const openPhotoModal = (index: number) => {
    setSelectedPhotoIndex(index);
    setPhotoModalVisible(true);
  };

  // Get working hours for today
  const getCurrentDayWorkingHours = () => {
    if (!restaurant?.working_hours) return 'Không có thông tin';
    
    const today = new Date().toLocaleDateString('vi-VN', { weekday: 'long' });
    const dayMap: Record<string, string> = {
      'Thứ Hai': 'Thứ Hai',
      'Thứ Ba': 'Thứ Ba', 
      'Thứ Tư': 'Thứ Tư',
      'Thứ Năm': 'Thứ Năm',
      'Thứ Sáu': 'Thứ Sáu',
      'Thứ Bảy': 'Thứ Bảy',
      'Chủ Nhật': 'Chủ Nhật'
    };
    
    const todayKey = dayMap[today] || 'Thứ Hai';
    const hours = restaurant.working_hours[todayKey];
    
    if (!hours || hours.length === 0) return 'Đóng cửa';
    return hours.join(', ');
  };

  // Check if restaurant is currently open
  const isCurrentlyOpen = () => {
    if (!restaurant?.working_hours) return false;
    
    const now = new Date();
    const today = now.toLocaleDateString('vi-VN', { weekday: 'long' });
    const dayMap: Record<string, string> = {
      'Thứ Hai': 'Thứ Hai',
      'Thứ Ba': 'Thứ Ba', 
      'Thứ Tư': 'Thứ Tư',
      'Thứ Năm': 'Thứ Năm',
      'Thứ Sáu': 'Thứ Sáu',
      'Thứ Bảy': 'Thứ Bảy',
      'Chủ Nhật': 'Chủ Nhật'
    };
    
    const todayKey = dayMap[today] || 'Thứ Hai';
    const hours = restaurant.working_hours[todayKey];
    
    if (!hours || hours.length === 0) return false;
    
    // Check for 24/7
    if (hours.some(h => h === 'Mở cửa cả ngày')) return true;
    
    const currentTime = now.getHours() * 100 + now.getMinutes(); // Convert to HHMM format
    
    // Check each time range
    for (const timeRange of hours) {
      if (timeRange.includes('–')) {
        const [start, end] = timeRange.split('–');
        const startTime = timeToNumber(start);
        const endTime = timeToNumber(end);
        
        if (startTime <= currentTime && currentTime <= endTime) {
          return true;
        }
      }
    }
    
    return false;
  };

  // Helper function to convert time string to number (e.g., "06:30" -> 630)
  const timeToNumber = (timeStr: string): number => {
    const [hours, minutes] = timeStr.split(':').map(Number);
    return hours * 100 + minutes;
  };

  // Filter and sort reviews
  const filteredAndSortedReviews = useMemo(() => {
    // Filter reviews by rating
    const filteredReviews = selectedRating === 0 
      ? reviews 
      : reviews.filter(review => review.review_rate === selectedRating);

    // Sort reviews
    return [...filteredReviews].sort((a, b) => {
      if (reviewSortBy === "newest") {
        return b.review_timestamp - a.review_timestamp;
      } else if (reviewSortBy === "oldest") {
        return a.review_timestamp - b.review_timestamp;
      } else if (reviewSortBy === "rating-high") {
        return b.review_rate - a.review_rate;
      } else if (reviewSortBy === "rating-low") {
        return a.review_rate - b.review_rate;
      }
      return 0;
    });
  }, [reviews, selectedRating, reviewSortBy]);

  // Calculate rating distribution
  const ratingDistribution = useMemo(() => {
    return [5, 4, 3, 2, 1].map((rating) => ({
      rating,
      count: reviews.filter((r) => r.review_rate === rating).length,
      percentage: reviews.length > 0 
        ? (reviews.filter((r) => r.review_rate === rating).length / reviews.length) * 100 
        : 0,
    }));
  }, [reviews]);

  // Loading state - show loading only if restaurant data is not available
  if (restaurantLoading) {
    return (
      <LoadingComponent 
        message="Đang tải thông tin nhà hàng..." 
        fullScreen={true}
      />
    );
  }

  if (!restaurant) {
    return (
      <Layout style={{ minHeight: "100vh", background: "linear-gradient(135deg, #000000 0%, #1a1a1a 100%)" }}>
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
          <Title level={3} style={{ color: 'rgba(255, 255, 255, 0.6)' }}>
            Không tìm thấy thông tin nhà hàng
          </Title>
        </div>
      </Layout>
    );
  }

  // Review filter dropdown items
  const reviewFilterItems = [
    {
      key: "newest",
      label: "Mới nhất",
    },
    {
      key: "oldest",
      label: "Cũ nhất",
    },
    {
      key: "rating-high",
      label: "Đánh giá cao nhất",
    },
    {
      key: "rating-low",
      label: "Đánh giá thấp nhất",
    },
  ];

  return (
    <Layout
      style={{
        minHeight: "100vh",
        background: "linear-gradient(135deg, #000000 0%, #1a1a1a 100%)",
      }}
    >
      <Header
        style={{
          background: "rgba(0, 0, 0, 0.95)",
          padding: "0 24px",
          borderBottom: "1px solid rgba(255, 255, 255, 0.1)",
          backdropFilter: "blur(10px)",
          position: "sticky",
          top: 0,
          zIndex: 1000,
        }}
      >
        <div
          style={{
            display: "flex",
            alignItems: "center",
            height: "100%",
          }}
        >
          <Button
            type="text"
            icon={<ArrowLeftOutlined />}
            onClick={handleBackClick}
            style={{
              color: "#ffa366",
              marginRight: "16px",
              fontSize: "16px",
            }}
          />
          <Title
            level={3}
            style={{
              color: "#ffffff",
              margin: 0,
              flex: 1,
            }}
          >
            {restaurant.name}
          </Title>
          <Space size="middle">
            <Button
              type="text"
              icon={<HeartOutlined />}
              style={{ color: "#ffa366" }}
            />
            <Button
              type="text"
              icon={<ShareAltOutlined />}
              style={{ color: "#ffa366" }}
            />
          </Space>
        </div>
      </Header>

      <Content
        style={{
          padding: "24px",
          overflow: "auto",
        }}
      >
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          {/* Restaurant Info */}
          <Card
            style={{
              background: "rgba(255, 255, 255, 0.05)",
              border: "1px solid rgba(255, 255, 255, 0.1)",
              borderRadius: "20px",
              marginBottom: "24px",
              boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
            }}
          >
            <Row gutter={32}>
              <Col xs={24} md={12}>
                {/* Main Restaurant Image */}
                <div style={{ position: "relative", marginBottom: "16px" }}>
                  <div
                    style={{
                      width: "100%",
                      height: "400px",
                      backgroundImage: `url(${getRestaurantImage(restaurant)})`,
                      backgroundSize: "cover",
                      backgroundPosition: "center",
                      borderRadius: "16px",
                      position: "relative",
                      overflow: "hidden",
                      boxShadow: "0 8px 24px rgba(0, 0, 0, 0.3)",
                    }}
                    onError={(e) => {
                      const target = e.target as HTMLDivElement;
                      target.style.backgroundImage = `url(https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80)`;
                    }}
                  >
                    {/* Hidden img element for error handling */}
                    <Image 
                      src={getRestaurantImage(restaurant)}
                      alt="Restaurant"
                      width={1200}
                      height={400}
                      style={{ display: 'none' }}
                      onError={(e) => {
                        const target = e.target as HTMLImageElement;
                        const parentDiv = target.parentElement;
                        if (parentDiv) {
                          parentDiv.style.backgroundImage = `url(https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80)`;
                        }
                      }}
                      priority
                    />
                    {/* Overlay gradient for better text readability */}
                    <div style={{
                      position: "absolute",
                      bottom: 0,
                      left: 0,
                      right: 0,
                      height: "100px",
                      background: "linear-gradient(transparent, rgba(0, 0, 0, 0.7))",
                      borderRadius: "0 0 16px 16px"
                    }} />
                  </div>
                </div>
                
                {/* Photo Gallery Grid */}
                {photosLoading ? (
                  <div>
                    <Text style={{ color: "#ffa366", fontWeight: "600", fontSize: "16px", display: "block", marginBottom: "12px" }}>
                      Đang tải hình ảnh...
                    </Text>
                    <Row gutter={8}>
                      {[1, 2, 3, 4].map((index) => (
                        <Col xs={6} key={index}>
                          <div
                            style={{
                              width: "100%",
                              height: "80px",
                              borderRadius: "12px",
                              background: "rgba(255, 255, 255, 0.1)",
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              animation: "pulse 2s infinite",
                            }}
                          >
                            <Spin size="small" />
                          </div>
                        </Col>
                      ))}
                    </Row>
                  </div>
                ) : photos.length > 0 && (
                  <div>
                    <Text style={{ color: "#ffa366", fontWeight: "600", fontSize: "16px", display: "block", marginBottom: "12px" }}>
                      Hình ảnh ({photos.length})
                    </Text>
                    <Row gutter={8}>
                      {photos.slice(0, 4).map((photo, index) => (
                        <Col xs={6} key={index}>
                          <div
                            style={{
                              width: "100%",
                              height: "80px",
                              backgroundImage: `url(${getHighQualityImageUrl(photo.src)})`,
                              backgroundSize: "cover",
                              backgroundPosition: "center",
                              borderRadius: "12px",
                              cursor: "pointer",
                              position: "relative",
                              overflow: "hidden",
                              border: "2px solid rgba(255, 163, 102, 0.3)",
                              transition: "all 0.3s ease",
                              boxShadow: "0 4px 12px rgba(0, 0, 0, 0.2)",
                            }}
                            onClick={() => openPhotoModal(index)}
                            onMouseEnter={(e) => {
                              e.currentTarget.style.transform = "scale(1.05)";
                              e.currentTarget.style.borderColor = "#ffa366";
                            }}
                            onMouseLeave={(e) => {
                              e.currentTarget.style.transform = "scale(1)";
                              e.currentTarget.style.borderColor = "rgba(255, 163, 102, 0.3)";
                            }}
                          >
                            {/* Hidden img element for error handling */}
                            <Image 
                              src={getHighQualityImageUrl(photo.src)}
                              alt={`Restaurant photo ${index + 1}`}
                              width={200}
                              height={80}
                              style={{ display: 'none' }}
                              onError={(e) => {
                                const target = e.target as HTMLImageElement;
                                const parentDiv = target.parentElement;
                                if (parentDiv) {
                                  parentDiv.style.backgroundImage = `url(https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400&h=300&fit=crop&crop=center&q=80)`;
                                }
                              }}
                              priority={index === 0}
                            />
                            {index === 3 && photos.length > 4 && (
                              <div style={{
                                position: "absolute",
                                top: 0,
                                left: 0,
                                right: 0,
                                bottom: 0,
                                background: "rgba(0, 0, 0, 0.7)",
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                color: "#fff",
                                fontSize: "14px",
                                fontWeight: "700",
                                backdropFilter: "blur(2px)"
                              }}>
                                +{photos.length - 4}
                              </div>
                            )}
                          </div>
                        </Col>
                      ))}
                    </Row>
                    
                    {/* View All Photos Button */}
                    {photos.length > 4 && (
                      <Button
                        onClick={() => openPhotoModal(0)}
                        style={{
                          marginTop: "12px",
                          width: "100%",
                          borderColor: "#ffa366",
                          color: "#ffa366",
                          background: "rgba(255, 163, 102, 0.1)",
                          borderRadius: "10px",
                          height: "40px",
                          fontWeight: "500",
                        }}
                      >
                        Xem tất cả {photos.length} ảnh
                      </Button>
                    )}
                  </div>
                )}
              </Col>
              <Col xs={24} md={12}>
                <div style={{ padding: "20px 0" }}>
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      justifyContent: "space-between",
                      marginBottom: "16px",
                    }}
                  >
                    <Title level={2} style={{ color: "#ffffff", margin: 0 }}>
                      {restaurant.name}
                    </Title>
                    <Badge
                      status="processing"
                      text={restaurant.state || "Không có thông tin"}
                      style={{
                        color: "#1890ff",
                        fontSize: "14px",
                        fontWeight: "500",
                      }}
                    />
                  </div>
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      marginBottom: "16px",
                    }}
                  >
                    <Rate
                      disabled
                      defaultValue={restaurant.rating}
                      style={{ fontSize: "20px" }}
                    />
                    <Text
                      style={{
                        color: "#ffffff",
                        marginLeft: "12px",
                        fontSize: "16px",
                        fontWeight: "500",
                      }}
                    >
                      {restaurant.rating} ({restaurant.review_count} đánh giá)
                    </Text>
                  </div>
                  <Space
                    direction="vertical"
                    size="middle"
                    style={{ width: "100%" }}
                  >
                    <div style={{ display: "flex", alignItems: "center" }}>
                      <EnvironmentOutlined
                        style={{
                          color: "#ffa366",
                          marginRight: "12px",
                          fontSize: "16px",
                        }}
                      />
                      <Text
                        style={{
                          color: "rgba(255, 255, 255, 0.8)",
                          fontSize: "15px",
                        }}
                      >
                        {restaurant.full_address}
                      </Text>
                    </div>

                    <div style={{ display: "flex", alignItems: "center" }}>
                      <ClockCircleOutlined
                        style={{
                          color: "#ffa366",
                          marginRight: "12px",
                          fontSize: "16px",
                        }}
                      />
                      <Text
                        style={{
                          color: "rgba(255, 255, 255, 0.8)",
                          fontSize: "15px",
                        }}
                      >
                        {getCurrentDayWorkingHours()}
                      </Text>
                    </div>

                    {restaurant.phone_number && (
                      <div style={{ display: "flex", alignItems: "center" }}>
                        <PhoneOutlined
                          style={{
                            color: "#ffa366",
                            marginRight: "12px",
                            fontSize: "16px",
                          }}
                        />
                        <Text
                          style={{
                            color: "rgba(255, 255, 255, 0.8)",
                            fontSize: "15px",
                          }}
                        >
                          {restaurant.phone_number}
                        </Text>
                      </div>
                    )}

                    {restaurant.types && restaurant.types.length > 0 && (
                      <div style={{ display: "flex", alignItems: "center" }}>
                        <Text
                          style={{
                            color: "#ffa366",
                            marginRight: "12px",
                            fontSize: "14px",
                            fontWeight: "600",
                          }}
                        >
                          Loại hình:
                        </Text>
                        <div>
                          {restaurant.types.map((type, index) => (
                            <Tag
                              key={index}
                              style={{
                                background: "rgba(255, 163, 102, 0.2)",
                                border: "1px solid rgba(255, 163, 102, 0.4)",
                                color: "#ffa366",
                                marginBottom: "4px",
                                borderRadius: "12px",
                                fontSize: "12px",
                              }}
                            >
                              {type}
                            </Tag>
                          ))}
                        </div>
                      </div>
                    )}
                  </Space>
                  <Divider
                    style={{
                      margin: "20px 0",
                      borderColor: "rgba(255, 255, 255, 0.2)",
                    }}
                  />
                  
                  {/* Restaurant Details */}
                  {restaurant.details && Object.keys(restaurant.details).length > 0 && (
                    <>
                      <Title level={4} style={{ color: "#ffa366", marginBottom: "16px" }}>
                        Thông tin chi tiết
                      </Title>
                      <Row gutter={[16, 16]} style={{ marginBottom: "24px" }}>
                        {Object.entries(restaurant.details).map(([category, items]) => (
                          <Col xs={24} sm={12} key={category}>
                            <div style={{
                              background: "rgba(255, 255, 255, 0.05)",
                              padding: "12px",
                              borderRadius: "8px",
                              border: "1px solid rgba(255, 255, 255, 0.1)"
                            }}>
                              <Text style={{ color: "#ffa366", fontWeight: "600", fontSize: "13px", display: "block", marginBottom: "8px" }}>
                                {category}
                              </Text>
                              <div>
                                {items.map((item, index) => (
                                  <Tag
                                    key={index}
                                    style={{
                                      background: "rgba(255, 255, 255, 0.1)",
                                      border: "1px solid rgba(255, 255, 255, 0.2)",
                                      color: "rgba(255, 255, 255, 0.8)",
                                      marginBottom: "4px",
                                      fontSize: "11px",
                                    }}
                                  >
                                    {item}
                                  </Tag>
                                ))}
                              </div>
                            </div>
                          </Col>
                        ))}
                      </Row>
                    </>
                  )}

                  <Space size="middle">
                    <Button
                      type="primary"
                      size="large"
                      onClick={() => {
                        if (restaurant.place_link) {
                          window.open(restaurant.place_link, '_blank');
                        } else {
                          window.open(`https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(restaurant.full_address)}`, '_blank');
                        }
                      }}
                      style={{
                        background: "linear-gradient(135deg, #ffa366 0%, #ff8c42 100%)",
                        border: "none",
                        borderRadius: "12px",
                        height: "48px",
                        paddingLeft: "32px",
                        paddingRight: "32px",
                        fontWeight: "500",
                      }}
                    >
                      Chỉ đường
                    </Button>
                    {restaurant.phone_number && (
                      <Button
                        size="large"
                        onClick={handlePhoneCall}
                        style={{
                          borderRadius: "12px",
                          height: "48px",
                          paddingLeft: "32px",
                          paddingRight: "32px",
                          fontWeight: "500",
                          borderColor: "#ffa366",
                          color: "#ffa366",
                          background: "rgba(255,163,102,0.1)",
                        }}
                      >
                        Gọi điện
                      </Button>
                    )}
                  </Space>
                </div>
              </Col>
            </Row>
          </Card>
          <Row gutter={24}>
            {/* Description Section */}
            <Col xs={24} lg={14}>
              <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
                <Card
                  style={{
                    background: "rgba(255, 255, 255, 0.05)",
                    border: "1px solid rgba(255, 255, 255, 0.1)",
                    borderRadius: "20px",
                    marginBottom: "24px",
                    boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
                    flex: 1,
                  }}
                >
                  <Title level={3} style={{ color: "#ffffff", marginBottom: "24px" }}>
                    Mô tả nhà hàng
                  </Title>
                  
                  {restaurant.description && restaurant.description.length > 0 ? (
                    <div style={{ marginBottom: "32px" }}>
                      {restaurant.description.map((desc, index) => (
                        <Text
                          key={index}
                          style={{
                            color: "rgba(255, 255, 255, 0.8)",
                            lineHeight: "1.6",
                            fontSize: "15px",
                            display: "block",
                            marginBottom: "12px",
                          }}
                        >
                          {desc}
                        </Text>
                      ))}
                    </div>
                  ) : (
                    <Text
                      style={{
                        color: "rgba(255, 255, 255, 0.6)",
                        fontSize: "15px",
                        fontStyle: "italic",
                        marginBottom: "32px",
                        display: "block"
                      }}
                    >
                      Chưa có mô tả cho nhà hàng này.
                    </Text>
                  )}

                  {/* Working Hours Visual Timeline */}
                  {restaurant.working_hours && Object.keys(restaurant.working_hours).length > 0 && (
                    <>
                      <Divider
                        style={{
                          margin: "24px 0",
                          borderColor: "rgba(255, 255, 255, 0.2)",
                        }}
                      />
                      <Title level={4} style={{ color: "#ffa366", marginBottom: "20px" }}>
                        Giờ hoạt động
                      </Title>
                      
                      <div style={{ marginBottom: "20px" }}>
                        {/* Sort days in correct order */}
                        {['Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy', 'Chủ Nhật']
                          .filter(day => restaurant.working_hours[day]) // Only show days that have data
                          .map((day) => {
                          const hours = restaurant.working_hours[day];
                          const today = new Date().toLocaleDateString('vi-VN', { weekday: 'long' });
                          const isToday = day === today;
                          const hasHours = hours && hours.length > 0 && !hours.includes('Đóng cửa');
                          const is24Hours = hours && hours.some(h => h === 'Mở cửa cả ngày');
                          const isCurrentlyOpenNow = isToday && isCurrentlyOpen();
                          
                          return (
                            <div
                              key={day}
                              style={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "center",
                                padding: "12px 16px",
                                marginBottom: "8px",
                                background: isToday 
                                  ? "linear-gradient(135deg, rgba(255, 163, 102, 0.2) 0%, rgba(255, 140, 66, 0.2) 100%)"
                                  : "rgba(255, 255, 255, 0.05)",
                                border: isToday 
                                  ? "2px solid rgba(255, 163, 102, 0.5)"
                                  : "1px solid rgba(255, 255, 255, 0.1)",
                                borderRadius: "12px",
                                transition: "all 0.3s ease",
                              }}
                            >
                              <div style={{ display: "flex", alignItems: "center", gap: "12px" }}>
                                <div style={{
                                  width: "12px",
                                  height: "12px",
                                  borderRadius: "50%",
                                  background: isToday 
                                    ? "#ffa366" 
                                    : hasHours 
                                      ? "#52c41a" 
                                      : "#ff4d4f",
                                  border: isToday ? "3px solid rgba(255, 163, 102, 0.5)" : "none",
                                  boxShadow: isToday ? "0 0 10px rgba(255, 163, 102, 0.5)" : "none"
                                }}>
                                </div>
                                <div>
                                  <Text style={{ 
                                    color: isToday ? "#ffa366" : "#ffffff", 
                                    fontWeight: isToday ? "700" : "600",
                                    fontSize: "15px",
                                    display: "block"
                                  }}>
                                    {day} {isToday && "(Hôm nay)"}
                                  </Text>
                                  {isToday && (
                                    <Text style={{ 
                                      color: "rgba(255, 163, 102, 0.8)", 
                                      fontSize: "12px" 
                                    }}>
                                      Ngày hiện tại
                                    </Text>
                                  )}
                                </div>
                              </div>
                              
                              <div style={{ textAlign: "right" }}>
                                {isToday ? (
                                  // Show current status for today only
                                  is24Hours ? (
                                    <div style={{
                                      background: "linear-gradient(135deg, #52c41a 0%, #389e0d 100%)",
                                      color: "#fff",
                                      padding: "6px 12px",
                                      borderRadius: "20px",
                                      fontSize: "12px",
                                      fontWeight: "600",
                                      display: "inline-block"
                                    }}>
                                      24/7 - Mở cửa cả ngày
                                    </div>
                                  ) : isCurrentlyOpenNow ? (
                                    <div>
                                      <Text style={{ 
                                        color: "#52c41a", 
                                        fontSize: "14px", 
                                        fontWeight: "600",
                                        display: "block"
                                      }}>
                                        Mở cửa
                                      </Text>
                                      <Text style={{ 
                                        color: "rgba(255, 255, 255, 0.8)", 
                                        fontSize: "13px" 
                                      }}>
                                        {hours.join(' • ')}
                                      </Text>
                                    </div>
                                  ) : hasHours ? (
                                    <div>
                                      <Text style={{ 
                                        color: "#ff4d4f", 
                                        fontSize: "14px", 
                                        fontWeight: "600",
                                        display: "block"
                                      }}>
                                        Đóng cửa
                                      </Text>
                                      <Text style={{ 
                                        color: "rgba(255, 255, 255, 0.8)", 
                                        fontSize: "13px" 
                                      }}>
                                        {hours.join(' • ')}
                                      </Text>
                                    </div>
                                  ) : (
                                    <div style={{
                                      background: "rgba(255, 77, 79, 0.2)",
                                      color: "#ff4d4f",
                                      padding: "6px 12px",
                                      borderRadius: "20px",
                                      fontSize: "12px",
                                      fontWeight: "600",
                                      display: "inline-block"
                                    }}>
                                      Đóng cửa
                                    </div>
                                  )
                                ) : (
                                  // For other days, just show the hours without status
                                  hasHours ? (
                                    <Text style={{ 
                                      color: "rgba(255, 255, 255, 0.8)", 
                                      fontSize: "13px" 
                                    }}>
                                      {hours.join(' • ')}
                                    </Text>
                                  ) : (
                                    <Text style={{ 
                                      color: "rgba(255, 255, 255, 0.6)", 
                                      fontSize: "13px" 
                                    }}>
                                      Đóng cửa
                                    </Text>
                                  )
                                )}
                              </div>
                            </div>
                          );
                        })}
                      </div>

                      {/* Quick Status Card */}
                      <div style={{
                        background: "linear-gradient(135deg, rgba(255, 163, 102, 0.2) 0%, rgba(255, 140, 66, 0.2) 100%)",
                        border: "1px solid rgba(255, 163, 102, 0.3)",
                        borderRadius: "16px",
                        padding: "20px",
                        textAlign: "center"
                      }}>
                       
                        <Text style={{ color: "#ffa366", fontWeight: "600", fontSize: "16px", display: "block" }}>
                          Trạng thái hiện tại
                        </Text>
                        <Text style={{ color: "#ffffff", fontSize: "18px", fontWeight: "600", marginTop: "4px", display: "block" }}>
                          {isCurrentlyOpen() ? "Mở cửa" : "Đóng cửa"}
                        </Text>
                        {isCurrentlyOpen() && (
                          <Text style={{ color: "rgba(255, 255, 255, 0.8)", fontSize: "14px", marginTop: "4px", display: "block" }}>
                            {getCurrentDayWorkingHours()}
                          </Text>
                        )}
                      </div>
                    </>
                  )}
                </Card>
              </div>
            </Col>{" "}
            {/* Reviews Section */}
            <Col xs={24} lg={10}>
              <div style={{ display: 'flex', flexDirection: 'column', height: '100%' }}>
                <Card
                  style={{
                    background: "rgba(255, 255, 255, 0.05)",
                    border: "1px solid rgba(255, 255, 255, 0.1)",
                    borderRadius: "20px",
                    boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
                    flex: 1,
                    display: 'flex',
                    flexDirection: 'column',
                  }}
                >
                  <div
                    style={{
                      display: "flex",
                      justifyContent: "space-between",
                      alignItems: "center",
                      marginBottom: "24px",
                    }}
                  >
                    <Title level={3} style={{ color: "#ffffff", margin: 0 }}>
                      Đánh giá khách hàng
                    </Title>
                    <Dropdown
                      menu={{
                        items: reviewFilterItems,
                        onClick: ({ key }) => {
                          setReviewSortBy(key);
                          setShowAllReviews(false); // Reset to show only 4 reviews when sorting changes
                        },
                      }}
                      placement="bottomRight"
                    >
                      <Button
                        style={{
                          borderRadius: "8px",
                          borderColor: "#ffa366",
                          color: "#ffa366",
                          background: "rgba(255, 163, 102, 0.1)",
                        }}
                      >
                        <Space>
                          <FilterOutlined />
                          {
                            reviewFilterItems.find(
                              (item) => item.key === reviewSortBy
                            )?.label
                          }
                          <DownOutlined />
                        </Space>
                      </Button>
                    </Dropdown>
                  </div>
                  
                  {/* Rating Summary */}
                  <div
                    style={{
                      marginBottom: "24px",
                      textAlign: "center",
                      padding: "20px",
                      background:
                        "linear-gradient(135deg, rgba(255, 163, 102, 0.2) 0%, rgba(255, 140, 66, 0.2) 100%)",
                      borderRadius: "16px",
                      border: "1px solid rgba(255, 163, 102, 0.3)",
                    }}
                  >
                    <Title
                      level={2}
                      style={{
                        color: "#ffa366",
                        marginBottom: "8px",
                        fontSize: "2.5rem",
                      }}
                    >
                      {restaurant.rating}
                    </Title>
                    <Rate
                      disabled
                      defaultValue={restaurant.rating}
                      style={{ fontSize: "24px", marginBottom: "8px" }}
                    />
                    <Text
                      style={{
                        color: "rgba(255, 255, 255, 0.8)",
                        display: "block",
                        fontSize: "14px",
                      }}
                    >
                      Dựa trên {restaurant.review_count} đánh giá
                    </Text>
                  </div>

                  {/* Rating Distribution */}
                  <div style={{ marginBottom: "24px" }}>
                    {ratingDistribution.map(({ rating, count, percentage }) => (
                      <div
                        key={rating}
                        style={{
                          display: "flex",
                          alignItems: "center",
                          marginBottom: "12px",
                        }}
                      >
                        <Button
                          size="small"
                          onClick={() =>
                            {
                              setSelectedRating(
                                selectedRating === rating ? 0 : rating
                              );
                              setShowAllReviews(false); // Reset to show only 4 reviews when filter changes
                            }
                          }
                          style={{
                            background:
                              selectedRating === rating
                                ? "#ffa366"
                                : "rgba(255, 255, 255, 0.05)",
                            border: "1px solid rgba(255, 255, 255, 0.2)",
                            color:
                              selectedRating === rating
                                ? "#000000"
                                : "rgba(255, 255, 255, 0.8)",
                            marginRight: "12px",
                            minWidth: "70px",
                            borderRadius: "8px",
                            fontWeight: "500",
                          }}
                        >
                          {rating} <StarFilled />
                        </Button>
                        <Progress
                          percent={percentage}
                          showInfo={false}
                          strokeColor="#ffa366"
                          style={{ flex: 1, marginRight: "12px" }}
                          strokeWidth={8}
                          trailColor="rgba(255, 255, 255, 0.1)"
                        />
                        <Text
                          style={{
                            color: "rgba(255, 255, 255, 0.8)",
                            minWidth: "30px",
                            fontWeight: "500",
                          }}
                        >
                          {count}
                        </Text>
                      </div>
                    ))}
                  </div>
                  <Divider style={{ borderColor: "rgba(255, 255, 255, 0.2)" }} />

                  {/* Reviews List */}
                  <div style={{ flex: 1, overflow: "hidden", display: "flex", flexDirection: "column" }}>
                    <div style={{ flex: 1, overflowY: "auto", paddingRight: "8px" }}>
                      {reviewsLoading ? (
                        <LoadingComponent 
                          message="Đang tải đánh giá..." 
                          fullScreen={false}
                          size="default"
                        />
                      ) : reviews.length === 0 ? (
                        <div style={{ textAlign: 'center', padding: '40px', color: 'rgba(255, 255, 255, 0.6)' }}>
                          <Text style={{ color: 'rgba(255, 255, 255, 0.6)' }}>
                            Chưa có đánh giá nào
                          </Text>
                        </div>
                      ) : (
                        <>
                          {(showAllReviews ? filteredAndSortedReviews : filteredAndSortedReviews.slice(0, 4)).map((review) => (
                            <div
                              key={review.review_id}
                              style={{
                                marginBottom: "16px",
                                background: "rgba(255, 255, 255, 0.05)",
                                padding: "16px",
                                borderRadius: "12px",
                                border: "1px solid rgba(255, 255, 255, 0.1)",
                              }}
                            >
                              <div
                                style={{
                                  display: "flex",
                                  alignItems: "center",
                                  marginBottom: "12px",
                                }}
                              >
                                <Avatar
                                  src={review.user_avatar}
                                  icon={<UserOutlined />}
                                  style={{ marginRight: "12px" }}
                                  size="large"
                                />
                                <div style={{ flex: 1 }}>
                                  <div
                                    style={{
                                      display: "flex",
                                      justifyContent: "space-between",
                                      alignItems: "center",
                                    }}
                                  >
                                    <Text
                                      style={{
                                        color: "#ffffff",
                                        fontWeight: "600",
                                        fontSize: "15px",
                                      }}
                                    >
                                      {review.user_name}
                                    </Text>
                                    <Text
                                      style={{
                                        color: "rgba(255, 255, 255, 0.6)",
                                        fontSize: "13px",
                                      }}
                                    >
                                      {review.review_time}
                                    </Text>
                                  </div>
                                  <Rate
                                    disabled
                                    defaultValue={review.review_rate}
                                    style={{ fontSize: "14px" }}
                                  />
                                </div>
                              </div>
                              <Text
                                style={{
                                  color: "rgba(255, 255, 255, 0.8)",
                                  lineHeight: "1.5",
                                  fontSize: "14px",
                                }}
                              >
                                {review.review_text}
                              </Text>
                            </div>
                          ))}
                          
                          {/* Show more reviews button if there are more than 4 */}
                          {!showAllReviews && filteredAndSortedReviews.length > 4 && (
                            <div style={{ textAlign: 'center', marginTop: '16px' }}>
                              <Button
                                onClick={() => setShowAllReviews(true)}
                                style={{
                                  borderColor: "#ffa366",
                                  color: "#ffa366",
                                  background: "rgba(255, 163, 102, 0.1)",
                                  borderRadius: "8px",
                                }}
                              >
                                Xem thêm {filteredAndSortedReviews.length - 4} đánh giá có sẵn
                              </Button>
                            </div>
                          )}
                        </>
                      )}
                    </div>
                  </div>
                </Card>
              </div>
            </Col>
          </Row>
        </div>
      </Content>

      {/* Photo Gallery Modal */}
      <Modal
        open={photoModalVisible}
        onCancel={() => setPhotoModalVisible(false)}
        footer={null}
        width="95%"
        style={{ top: 10 }}
        bodyStyle={{ 
          padding: 0,
          background: "rgba(0, 0, 0, 0.95)",
          borderRadius: "16px",
          overflow: "hidden"
        }}
        styles={{
          mask: {
            backgroundColor: "rgba(0, 0, 0, 0.9)"
          },
          content: {
            backgroundColor: "transparent",
            boxShadow: "none",
            padding: 0
          }
        }}
        destroyOnClose={true}
        maskClosable={true}
      >
        <div style={{ position: "relative", minHeight: "500px" }}>
          {/* Main Image */}
          <div style={{
            width: "100%",
            height: "80vh",
            maxHeight: "800px",
            position: "relative",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
            background: "rgba(0, 0, 0, 0.8)"
          }}>
            <Image
              src={getHighQualityImageUrl(photos[selectedPhotoIndex]?.src)}
              alt={`Restaurant photo ${selectedPhotoIndex + 1}`}
              width={1200}
              height={800}
              style={{
                maxWidth: "100%",
                maxHeight: "100%",
                objectFit: "contain",
                borderRadius: "8px"
              }}
              onError={(e) => {
                const target = e.target as HTMLImageElement;
                target.src = 'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=1920&h=1080&fit=crop&crop=center&q=80';
              }}
              priority
            />
            
            {/* Loading placeholder */}
            {!photos[selectedPhotoIndex]?.src && (
              <div style={{
                position: "absolute",
                top: "50%",
                left: "50%",
                transform: "translate(-50%, -50%)",
                color: "rgba(255, 255, 255, 0.5)",
                fontSize: "18px",
                display: "flex",
                alignItems: "center",
                gap: "12px"
              }}>
                <Spin size="large" />
                <span>Đang tải...</span>
              </div>
            )}
          </div>
          
          {/* Image Info */}
          <div style={{
            position: "absolute",
            top: "20px",
            left: "20px",
            background: "rgba(0, 0, 0, 0.7)",
            padding: "12px 20px",
            borderRadius: "25px",
            backdropFilter: "blur(10px)"
          }}>
            <Text style={{ color: "#fff", fontSize: "16px", fontWeight: "600" }}>
              {restaurant?.name}
            </Text>
            <br />
            <Text style={{ color: "rgba(255, 255, 255, 0.8)", fontSize: "14px" }}>
              Ảnh {selectedPhotoIndex + 1} / {photos.length}
            </Text>
          </div>

          {/* Close Button */}
          <Button
            type="text"
            onClick={() => setPhotoModalVisible(false)}
            style={{
              position: "absolute",
              top: "20px",
              right: "20px",
              width: "50px",
              height: "50px",
              borderRadius: "50%",
              background: "rgba(0, 0, 0, 0.7)",
              border: "none",
              color: "#fff",
              fontSize: "20px",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              backdropFilter: "blur(10px)"
            }}
          >
            ✕
          </Button>
          
          {/* Navigation Arrows */}
          {photos.length > 1 && (
            <>
              <Button
                type="text"
                disabled={selectedPhotoIndex === 0}
                onClick={() => setSelectedPhotoIndex(prev => Math.max(0, prev - 1))}
                style={{
                  position: "absolute",
                  left: "20px",
                  top: "50%",
                  transform: "translateY(-50%)",
                  width: "60px",
                  height: "60px",
                  borderRadius: "50%",
                  background: selectedPhotoIndex === 0 ? "rgba(0, 0, 0, 0.3)" : "rgba(0, 0, 0, 0.7)",
                  border: "none",
                  color: selectedPhotoIndex === 0 ? "rgba(255, 255, 255, 0.3)" : "#fff",
                  fontSize: "24px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  backdropFilter: "blur(10px)",
                  cursor: selectedPhotoIndex === 0 ? "not-allowed" : "pointer"
                }}
              >
                ←
              </Button>
              
              <Button
                type="text"
                disabled={selectedPhotoIndex === photos.length - 1}
                onClick={() => setSelectedPhotoIndex(prev => Math.min(photos.length - 1, prev + 1))}
                style={{
                  position: "absolute",
                  right: "20px",
                  top: "50%",
                  transform: "translateY(-50%)",
                  width: "60px",
                  height: "60px",
                  borderRadius: "50%",
                  background: selectedPhotoIndex === photos.length - 1 ? "rgba(0, 0, 0, 0.3)" : "rgba(0, 0, 0, 0.7)",
                  border: "none",
                  color: selectedPhotoIndex === photos.length - 1 ? "rgba(255, 255, 255, 0.3)" : "#fff",
                  fontSize: "24px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  backdropFilter: "blur(10px)",
                  cursor: selectedPhotoIndex === photos.length - 1 ? "not-allowed" : "pointer"
                }}
              >
                →
              </Button>
            </>
          )}

          {/* Thumbnail Navigation */}
          {photos.length > 1 && (
            <div style={{
              position: "absolute",
              bottom: "20px",
              left: "50%",
              transform: "translateX(-50%)",
              display: "flex",
              gap: "8px",
              background: "rgba(0, 0, 0, 0.7)",
              padding: "12px",
              borderRadius: "25px",
              backdropFilter: "blur(10px)",
              maxWidth: "90%",
              overflowX: "auto"
            }}>
              {photos.slice(0, 10).map((photo, index) => (
                <div
                  key={index}
                  onClick={() => setSelectedPhotoIndex(index)}
                  style={{
                    width: "60px",
                    height: "40px",
                    backgroundImage: `url(${getHighQualityImageUrl(photo.src)})`,
                    backgroundSize: "cover",
                    backgroundPosition: "center",
                    borderRadius: "8px",
                    cursor: "pointer",
                    border: index === selectedPhotoIndex ? "3px solid #ffa366" : "2px solid rgba(255, 255, 255, 0.3)",
                    transition: "all 0.3s ease",
                    flexShrink: 0,
                    position: "relative",
                    overflow: "hidden"
                  }}
                >
                  {/* Hidden img for error handling */}
                  <Image 
                    src={getHighQualityImageUrl(photo.src)}
                    alt={`Thumbnail ${index + 1}`}
                    width={60}
                    height={40}
                    style={{ display: 'none' }}
                    onError={(e) => {
                      const target = e.target as HTMLImageElement;
                      const parentDiv = target.parentElement;
                      if (parentDiv) {
                        parentDiv.style.backgroundImage = `url(https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=200&h=150&fit=crop&crop=center&q=80)`;
                      }
                    }}
                    priority={index === selectedPhotoIndex}
                  />
                </div>
              ))}
              {photos.length > 10 && (
                <div style={{
                  width: "60px",
                  height: "40px",
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "center",
                  background: "rgba(255, 255, 255, 0.1)",
                  borderRadius: "8px",
                  color: "#fff",
                  fontSize: "12px",
                  fontWeight: "600"
                }}>
                  +{photos.length - 10}
                </div>
              )}
            </div>
          )}
        </div>
      </Modal>
    </Layout>
  );
};

export default RestaurantDetail;
