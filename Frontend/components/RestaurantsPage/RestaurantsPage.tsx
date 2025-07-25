"use client";
import React, { useEffect, useState, useMemo } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import {
  Layout,
  Typography,
  Card,
  Row,
  Col,
  Rate,
  Button,
  Tag,
  Space,
  Input,
  Dropdown,
  App,
  ConfigProvider,
  theme,
} from "antd";
import {
  ArrowLeftOutlined,
  EnvironmentOutlined,
  PhoneOutlined,
  SearchOutlined,
  FilterOutlined,
  DownOutlined,
  HeartOutlined,
} from "@ant-design/icons";
import { useGeolocation } from "../../hooks/useGeolocation";
import { useRestaurantsSearch } from "../../hooks/useRestaurants";
import LoadingComponent from "../LoadingComponent";
import Image from "next/image";

const { Content, Header } = Layout;
const { Title, Text } = Typography;

// Inner component that can use useApp hook
const RestaurantsPageContent: React.FC = () => {
  const { message: messageApi } = App.useApp();
  const router = useRouter();
  const searchParams = useSearchParams();
  const { location: userLocation } = useGeolocation();
  
  // Get search parameters
  const searchParam = searchParams.get("search");
  const locationParam = searchParams.get("location");
  const latParam = searchParams.get("lat");
  const lngParam = searchParams.get("lng");

  // Local state
  const [dish, setDish] = useState<string>("");
  const [location, setLocation] = useState<string>("");
  const [searchText, setSearchText] = useState<string>("");
  const [sortBy, setSortBy] = useState<string>("rating");

  // Enhanced function to get high quality image URL
  const getHighQualityImageUrl = (photoUrl: string) => {
    if (!photoUrl) return 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&h=600&fit=crop&crop=center&q=80';
    
    // Handle Google Photos URLs
    if (photoUrl.includes('googleusercontent.com')) {
      return photoUrl.replace(/=w\d+-h\d+/, '=w800-h600').replace(/-k-no$/, '');
    }
    
    // Handle Google Static URLs
    if (photoUrl.includes('gstatic.com')) {
      return photoUrl.replace(/=w\d+-h\d+/, '=w800-h600');
    }
    
    // Handle other Google image URLs
    if (photoUrl.includes('googleapis.com')) {
      return photoUrl.replace(/maxwidth=\d+/, 'maxwidth=800').replace(/maxheight=\d+/, 'maxheight=600');
    }
    
    return photoUrl;
  };

  // Prepare coordinates for API call
  const userCoords = useMemo(() => {
    if (latParam && lngParam) {
      return {
        latitude: parseFloat(latParam),
        longitude: parseFloat(lngParam),
      };
    }
    return userLocation;
  }, [latParam, lngParam, userLocation]);

  // Use React Query for restaurants data
  const {
    data: restaurants = [],
    isLoading,
    error,
    isFetching
  } = useRestaurantsSearch(
    searchParam || "",
    userCoords,
    locationParam || undefined
  );

  // Update local state when URL params change
  useEffect(() => {
    if (searchParam) setDish(decodeURIComponent(searchParam));
    if (locationParam) setLocation(decodeURIComponent(locationParam));
  }, [searchParam, locationParam]);

  // Show error message if query fails
  useEffect(() => {
    if (error) {
      messageApi.error("Không thể tải danh sách nhà hàng");
    }
  }, [error, messageApi]);

  // Filter and sort restaurants
  const filteredAndSortedRestaurants = useMemo(() => {
    let filtered = restaurants.filter((restaurant) => {
      // Add null/undefined checks to prevent errors
      const restaurantName = restaurant.name || '';
      const restaurantAddress = restaurant.full_address || '';
      const restaurantTypes = restaurant.types || [];
      
      const matchesSearch =
        restaurantName.toLowerCase().includes(searchText.toLowerCase()) ||
        restaurantAddress.toLowerCase().includes(searchText.toLowerCase()) ||
        restaurantTypes.some((s) =>
          s && s.toLowerCase().includes(searchText.toLowerCase())
        );

      return matchesSearch;
    });

    // Sort restaurants based on selection
    if (sortBy === "rating") {
      filtered = filtered.sort((a, b) => b.rating - a.rating);
    } else if (sortBy === "distance") {
      // Note: distance calculation would need to be implemented based on coordinates
      filtered = filtered.sort((a, b) => a.name.localeCompare(b.name)); // Fallback sort
    } else if (sortBy === "review_count") {
      filtered = filtered.sort((a, b) => b.review_count - a.review_count);
    }

    return filtered;
  }, [restaurants, searchText, sortBy]);
  const handleBackClick = () => {
    router.back();
  };

  const handleRestaurantClick = (restaurant: any) => {
    // Create a unique identifier that includes both business_id and place_id
    const restaurantId = `${restaurant.business_id}__${restaurant.place_id}`;
    router.push(`/restaurants/${restaurantId}`);
  };

  // Filter dropdown items
  const filterItems = [
    {
      key: "rating",
      label: "Sắp xếp theo đánh giá",
    },
    {
      key: "review_count",
      label: "Sắp xếp theo số lượng đánh giá",
    },
    {
      key: "distance",
      label: "Sắp xếp theo khoảng cách",
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
          {" "}
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
            {dish && location
              ? `Quán ăn ${dish} tại ${location}`
              : dish
              ? `Quán ăn ${dish}`
              : location
              ? `Quán ăn tại ${location}`
              : "Danh sách quán ăn"}
          </Title>
        </div>
      </Header>

      <Content
        style={{
          padding: "24px",
          overflow: "auto",
        }}
      >
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          {/* Loading State */}
          {isLoading && !restaurants.length ? (
            <LoadingComponent 
              message="Đang tìm kiếm nhà hàng..." 
              fullScreen={false}
            />
          ) : (
            <>
              {/* Search and Filter Section */}
              <Card
                style={{
                  background: "rgba(255, 255, 255, 0.05)",
                  padding: "24px",
                  borderRadius: "20px",
                  marginBottom: "24px",
                  border: "1px solid rgba(255, 255, 255, 0.1)",
                  boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
                }}
              >
                <Row gutter={[16, 16]} align="middle">
                  <Col xs={24} md={16} lg={18}>
                    <Input
                      placeholder="Tìm kiếm nhà hàng, địa chỉ, món ăn..."
                      value={searchText}
                      onChange={(e) => setSearchText(e.target.value)}
                      style={{
                        borderRadius: "12px",
                        height: "48px",
                      }}
                      styles={{
                        input: {
                          background: "rgba(255, 255, 255, 0.9)",
                          borderColor: "rgba(255, 163, 102, 0.5)",
                          color: "#000000",
                          fontSize: "16px",
                          paddingLeft: "48px",
                        }
                      }}
                      size="large"
                      prefix={<SearchOutlined style={{ color: "#ffa366", fontSize: "18px" }} />}
                    />
                  </Col>
                  <Col xs={24} md={8} lg={6}>
                    <Dropdown
                      menu={{
                        items: filterItems,
                        onClick: ({ key }) => setSortBy(key),
                      }}
                      placement="bottomRight"
                    >
                      <Button
                        size="large"
                        style={{
                          width: "100%",
                          borderRadius: "12px",
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "space-between",
                          borderColor: "#ffa366",
                          color: "#ffa366",
                          background: "rgba(255, 163, 102, 0.1)",
                          height: "48px",
                          padding: "0 16px",
                          fontWeight: "500",
                          overflow: "hidden",
                        }}
                      >
                        <div style={{ 
                          display: "flex", 
                          alignItems: "center", 
                          gap: "8px",
                          flex: 1,
                          minWidth: 0
                        }}>
                          <FilterOutlined style={{ fontSize: "16px", flexShrink: 0 }} />
                          <span style={{
                            overflow: "hidden",
                            textOverflow: "ellipsis",
                            whiteSpace: "nowrap",
                            fontSize: "14px"
                          }}>
                            {sortBy === "rating" ? "Đánh giá" : 
                             sortBy === "review_count" ? "Số đánh giá" : 
                             sortBy === "distance" ? "Khoảng cách" : "Sắp xếp"}
                          </span>
                        </div>
                        <DownOutlined style={{ fontSize: "12px", flexShrink: 0, marginLeft: "4px" }} />
                      </Button>
                    </Dropdown>
                  </Col>
                </Row>
                
                {/* Refreshing indicator */}
                {isFetching && restaurants.length > 0 && (
                  <div style={{ 
                    marginTop: "16px", 
                    textAlign: "center", 
                    color: "#ffa366",
                    fontSize: "14px",
                    fontWeight: "500"
                  }}>
                    <span style={{ 
                      padding: "8px 16px", 
                      background: "rgba(255, 163, 102, 0.1)",
                      borderRadius: "20px",
                      border: "1px solid rgba(255, 163, 102, 0.3)"
                    }}>
                      Đang cập nhật dữ liệu...
                    </span>
                  </div>
                )}
              </Card>

              {/* Results Count */}
              <div style={{ marginBottom: "24px" }}>
                <Text
                  style={{
                    color: "rgba(255, 255, 255, 0.8)",
                    fontSize: "16px",
                    fontWeight: "500",
                  }}
                >
                  Tìm thấy {filteredAndSortedRestaurants.length} nhà hàng{" "}
                  {dish && `cho "${dish}"`} {location && `tại ${location}`}
                </Text>
              </div>

              <Row gutter={[24, 24]}>
                {filteredAndSortedRestaurants.map((restaurant) => (
                  <Col xs={24} lg={12} key={restaurant.business_id}>
                    <Card
                      hoverable
                      onClick={() =>
                        handleRestaurantClick(restaurant)
                      }
                      style={{
                        background: "rgba(255, 255, 255, 0.05)",
                        border: "1px solid rgba(255, 255, 255, 0.1)",
                        borderRadius: "20px",
                        overflow: "hidden",
                        cursor: "pointer",
                        transition: "all 0.3s ease",
                        boxShadow: "0 4px 20px rgba(0, 0, 0, 0.3)",
                      }}
                                             styles={{ body: { padding: "0", height: "100%" } }}
                    >
                      <Row style={{ height: "100%" }}>
                        <Col xs={24} sm={10}>
                          <div
                            style={{
                              height: "100%",
                              minHeight: "350px",
                              backgroundImage: restaurant.photos && restaurant.photos.length > 0
                                ? `url(${getHighQualityImageUrl(restaurant.photos[0].src)})`
                                : `url(https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&h=600&fit=crop&crop=center&q=80)`,
                              backgroundSize: "cover",
                              backgroundPosition: "center",
                              position: "relative",
                              borderRadius: "20px 0 0 20px",
                              overflow: "hidden"
                            }}
                          >
                            {/* Hidden img element for error handling */}
                            <Image 
                              src={restaurant.photos && restaurant.photos.length > 0 
                                ? getHighQualityImageUrl(restaurant.photos[0].src) 
                                : 'https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&h=600&fit=crop&crop=center&q=80'
                              }
                              alt={restaurant.name}
                              width={350}
                              height={350}
                              style={{ display: 'none' }}
                              onError={(e) => {
                                const target = e.target as HTMLImageElement;
                                const parentDiv = target.parentElement;
                                if (parentDiv) {
                                  parentDiv.style.backgroundImage = `url(https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=800&h=600&fit=crop&crop=center&q=80)`;
                                }
                              }}
                              priority
                            />
                            <div
                              style={{
                                position: "absolute",
                                top: "16px",
                                left: "16px",
                              }}
                            >
                              <Button
                                type="text"
                                icon={<HeartOutlined />}
                                style={{
                                  background: "rgba(0, 0, 0, 0.8)",
                                  border: "none",
                                  borderRadius: "50%",
                                  width: "36px",
                                  height: "36px",
                                  display: "flex",
                                  alignItems: "center",
                                  justifyContent: "center",
                                  color: "#ffa366",
                                }}
                                onClick={(e) => e.stopPropagation()}
                              />
                            </div>
                          </div>
                        </Col>
                        <Col xs={24} sm={14}>
                          <div style={{ padding: "24px" }}>
                            <div
                              style={{
                                display: "flex",
                                justifyContent: "space-between",
                                alignItems: "flex-start",
                                marginBottom: "12px",
                              }}
                            >
                                                             <Title
                                 level={4}
                                 style={{
                                   color: "#ffffff",
                                   margin: 0,
                                   fontSize: "18px",
                                 }}
                               >
                                 {restaurant.name || 'Tên nhà hàng'}
                               </Title>
                            </div>
                            <Space
                              direction="vertical"
                              size="small"
                              style={{ width: "100%" }}
                            >
                              <div
                                style={{
                                  display: "flex",
                                  alignItems: "center",
                                }}
                              >
                                                                 <Rate
                                   disabled
                                   defaultValue={restaurant.rating || 0}
                                   style={{ fontSize: "16px" }}
                                 />
                                                               <Text
                                   style={{
                                     color: "rgba(255, 255, 255, 0.8)",
                                     marginLeft: "8px",
                                     fontSize: "14px",
                                   }}
                                 >
                                   {restaurant.rating || 0} ({restaurant.review_count || 0}{" "}
                                   đánh giá)
                                 </Text>
                              </div>

                              <div
                                style={{
                                  display: "flex",
                                  alignItems: "center",
                                }}
                              >
                                <EnvironmentOutlined
                                  style={{
                                    color: "#ffa366",
                                    marginRight: "8px",
                                  }}
                                />
                                                                 <Text
                                   style={{
                                     color: "rgba(255, 255, 255, 0.7)",
                                     fontSize: "13px",
                                   }}
                                 >
                                   {restaurant.full_address || 'Địa chỉ không có sẵn'}
                                 </Text>
                              </div>

                              {restaurant.phone_number && (
                                <div
                                  style={{
                                    display: "flex",
                                    alignItems: "center",
                                  }}
                                >
                                  <PhoneOutlined
                                    style={{
                                      color: "#ffa366",
                                      marginRight: "8px",
                                    }}
                                  />
                                                                       <Text
                                       style={{
                                         color: "rgba(255, 255, 255, 0.7)",
                                         fontSize: "13px",
                                       }}
                                     >
                                       {restaurant.phone_number || 'Không có số điện thoại'}
                                     </Text>
                                </div>
                              )}

                              {restaurant.price_level && (
                                <Text
                                  style={{
                                    color: "#ffa366",
                                    fontSize: "15px",
                                    fontWeight: "600",
                                    marginTop: "4px",
                                  }}
                                >
                                  {restaurant.price_level}
                                </Text>
                              )}

                                                             <div style={{ marginTop: "12px" }}>
                                 {(restaurant.types || [])
                                   .slice(0, 3)
                                   .map((type, index) => (
                                     <Tag
                                       key={index}
                                       style={{
                                         background: "rgba(255, 163, 102, 0.2)",
                                         border:
                                           "1px solid rgba(255, 163, 102, 0.4)",
                                         color: "#ffa366",
                                         marginBottom: "4px",
                                         borderRadius: "12px",
                                         fontSize: "12px",
                                       }}
                                     >
                                       {type || 'Loại hình'}
                                     </Tag>
                                   ))}
                               </div>
                            </Space>

                            <div
                              style={{
                                marginTop: "20px",
                                display: "flex",
                                gap: "12px",
                              }}
                            >
                              <Button
                                type="primary"
                                onClick={(e) => {
                                  e.stopPropagation();
                                  handleRestaurantClick(restaurant);
                                }}
                                style={{
                                  background:
                                    "linear-gradient(135deg, #ffa366 0%, #ff8c42 100%)",
                                  border: "none",
                                  borderRadius: "10px",
                                  fontWeight: "500",
                                }}
                              >
                                Xem chi tiết
                              </Button>
                              {restaurant.phone_number && (
                                <Button
                                  onClick={(e) => {
                                    e.stopPropagation();
                                    window.open(
                                      `tel:${restaurant.phone_number}`,
                                      "_self"
                                    );
                                  }}
                                  style={{
                                    borderRadius: "10px",
                                    borderColor: "#ffa366",
                                    color: "#ffa366",
                                    fontWeight: "500",
                                    background: "rgba(255, 163, 102, 0.1)",
                                  }}
                                >
                                  Gọi ngay
                                </Button>
                              )}
                            </div>
                          </div>
                        </Col>
                      </Row>
                    </Card>
                  </Col>
                ))}
              </Row>

              {filteredAndSortedRestaurants.length === 0 && !isLoading && (
                <div
                  style={{
                    textAlign: "center",
                    padding: "60px 20px",
                    color: "rgba(255, 255, 255, 0.6)",
                  }}
                >
                  <Title
                    level={3}
                    style={{ color: "rgba(255, 255, 255, 0.6)" }}
                  >
                    {error ? "Đã có lỗi xảy ra" : "Không tìm thấy nhà hàng nào"}
                  </Title>
                  <Text style={{ color: "rgba(255, 255, 255, 0.6)" }}>
                    {error 
                      ? "Vui lòng thử lại sau" 
                      : "Hãy thử tìm kiếm với từ khóa khác hoặc thay đổi vị trí."}
                  </Text>
                </div>
              )}
            </>
          )}
        </div>
      </Content>
    </Layout>
  );
};

// Main component that provides App context
const RestaurantsPage: React.FC = () => {
  return (
    <ConfigProvider
      theme={{
        algorithm: theme.darkAlgorithm,
        token: {
          colorPrimary: "#ff7a00",
          colorBgContainer: "#1f1f23",
          colorBgElevated: "#262629",
          colorBorder: "#ff7a0033",
          colorText: "#ffffff",
          colorTextSecondary: "#b3b3b3",
        },
      }}
    >
      <App>
        <RestaurantsPageContent />
      </App>
    </ConfigProvider>
  );
};

export default RestaurantsPage;
