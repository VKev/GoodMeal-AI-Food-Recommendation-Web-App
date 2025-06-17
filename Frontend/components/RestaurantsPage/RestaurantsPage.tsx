"use client";
import React, { useEffect, useState } from "react";
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
  Badge,
} from "antd";
import {
  ArrowLeftOutlined,
  EnvironmentOutlined,
  ClockCircleOutlined,
  PhoneOutlined,
  SearchOutlined,
  FilterOutlined,
  DownOutlined,
  HeartOutlined,
} from "@ant-design/icons";

const { Content, Header } = Layout;
const { Title, Text } = Typography;
const { Search } = Input;

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
  const [filteredRestaurants, setFilteredRestaurants] = useState<Restaurant[]>(
    []
  );
  const [dish, setDish] = useState<string>("");
  const [location, setLocation] = useState<string>("");
  const [searchText, setSearchText] = useState<string>("");
  const [sortBy, setSortBy] = useState<string>("rating");

  useEffect(() => {
    const dishParam = searchParams.get("dish");
    const locationParam = searchParams.get("location");

    if (dishParam) setDish(decodeURIComponent(dishParam));
    if (locationParam) setLocation(decodeURIComponent(locationParam));

    // Mock data cho các cửa hàng
    const mockRestaurants: Restaurant[] = [
      {
        id: "1",
        name: "Phở Hùng",
        address: "123 Nguyễn Văn Dậu, Bình Thạnh, TP.HCM",
        phone: "0901234567",
        rating: 4.5,
        reviewCount: 324,
        openTime: "06:00",
        closeTime: "22:00",
        isOpen: true,
        image:
          "https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=400",
        distance: "0.8 km",
        priceRange: "50,000 - 80,000đ",
        specialties: ["Phở Bò", "Phở Gà", "Bún Bò Huế"],
      },
      {
        id: "2",
        name: "Quán Việt",
        address: "456 Đinh Bộ Lĩnh, Bình Thạnh, TP.HCM",
        phone: "0907654321",
        rating: 4.2,
        reviewCount: 156,
        openTime: "07:00",
        closeTime: "21:30",
        isOpen: true,
        image:
          "https://images.unsplash.com/photo-1555396273-367ea4eb4db5?w=400",
        distance: "1.2 km",
        priceRange: "45,000 - 75,000đ",
        specialties: ["Phở Đặc Biệt", "Bánh Mì", "Cơm Tấm"],
      },
      {
        id: "3",
        name: "Bếp Nhà",
        address: "789 Xô Viết Nghệ Tĩnh, Bình Thạnh, TP.HCM",
        phone: "0908765432",
        rating: 4.7,
        reviewCount: 89,
        openTime: "08:00",
        closeTime: "20:00",
        isOpen: false,
        image:
          "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=400",
        distance: "1.5 km",
        priceRange: "60,000 - 90,000đ",
        specialties: ["Phở Truyền Thống", "Bánh Cuốn", "Chả Cá"],
      },
      {
        id: "4",
        name: "Món Ngon Sài Gòn",
        address: "321 Phan Văn Trị, Bình Thạnh, TP.HCM",
        phone: "0909876543",
        rating: 4.0,
        reviewCount: 203,
        openTime: "06:30",
        closeTime: "23:00",
        isOpen: true,
        image:
          "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=400",
        distance: "2.1 km",
        priceRange: "40,000 - 70,000đ",
        specialties: ["Phở Gà", "Bún Riêu", "Hủ Tiếu"],
      },
      {
        id: "5",
        name: "Quán Cô Ba",
        address: "654 Võ Văn Tần, Quận 3, TP.HCM",
        phone: "0908123456",
        rating: 4.8,
        reviewCount: 412,
        openTime: "07:00",
        closeTime: "22:30",
        isOpen: true,
        image:
          "https://th.bing.com/th/id/OIP.8ClU18yqyTA1_mLhngOB6gHaE7?rs=1&pid=ImgDetMain",
        distance: "3.2 km",
        priceRange: "55,000 - 85,000đ",
        specialties: ["Phở Bò Viên", "Bánh Mì Thịt", "Chè Ba Màu"],
      },
      {
        id: "6",
        name: "Bún Phở Miền Nam",
        address: "987 Nguyễn Thị Minh Khai, Quận 1, TP.HCM",
        phone: "0907321654",
        rating: 3.8,
        reviewCount: 128,
        openTime: "08:00",
        closeTime: "21:00",
        isOpen: false,
        image:
          "https://th.bing.com/th/id/OIP.8ClU18yqyTA1_mLhngOB6gHaE7?rs=1&pid=ImgDetMain",
        distance: "4.5 km",
        priceRange: "35,000 - 65,000đ",
        specialties: ["Phở Tái", "Bún Bò", "Gỏi Cuốn"],
      },
    ];

    setRestaurants(mockRestaurants);
    setFilteredRestaurants(mockRestaurants);
  }, [searchParams]); // Filter function
  useEffect(() => {
    let filtered = restaurants.filter((restaurant) => {
      const matchesSearch =
        restaurant.name.toLowerCase().includes(searchText.toLowerCase()) ||
        restaurant.address.toLowerCase().includes(searchText.toLowerCase()) ||
        restaurant.specialties.some((s) =>
          s.toLowerCase().includes(searchText.toLowerCase())
        );

      return matchesSearch;
    });

    // Sort restaurants based on selection
    if (sortBy === "rating") {
      filtered = filtered.sort((a, b) => b.rating - a.rating);
    } else if (sortBy === "distance") {
      filtered = filtered.sort((a, b) => {
        const distanceA = parseFloat(a.distance.replace(" km", ""));
        const distanceB = parseFloat(b.distance.replace(" km", ""));
        return distanceA - distanceB;
      });
    } else if (sortBy === "open") {
      filtered = filtered.sort(
        (a, b) => (b.isOpen ? 1 : 0) - (a.isOpen ? 1 : 0)
      );
    }

    setFilteredRestaurants(filtered);
  }, [restaurants, searchText, sortBy]);
  const handleBackClick = () => {
    router.back();
  };

  const handleRestaurantClick = (restaurantId: string) => {
    router.push(`/restaurants/${restaurantId}`);
  };

  // Filter dropdown items
  const filterItems = [
    {
      key: "rating",
      label: "Sắp xếp theo đánh giá",
    },
    {
      key: "distance",
      label: "Sắp xếp theo khoảng cách",
    },
    {
      key: "open",
      label: "Quán đang mở cửa",
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
            Quán ăn có {dish} tại {location}
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
          {" "}
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
              {" "}
              <Col xs={24} md={18}>
                <Search
                  placeholder="Tìm kiếm nhà hàng, địa chỉ, món ăn..."
                  value={searchText}
                  onChange={(e) => setSearchText(e.target.value)}
                  style={{
                    borderRadius: "12px",
                  }}
                  size="large"
                  prefix={<SearchOutlined style={{ color: "#ffa366" }} />}
                />
              </Col>
              <Col xs={24} md={6}>
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
                      justifyContent: "center",
                      borderColor: "#ffa366",
                      color: "#ffa366",
                      background: "rgba(255, 163, 102, 0.1)",
                    }}
                  >
                    <Space>
                      <FilterOutlined />
                      {filterItems.find((item) => item.key === sortBy)?.label}
                      <DownOutlined />
                    </Space>
                  </Button>
                </Dropdown>
              </Col>
            </Row>
          </Card>{" "}
          {/* Results Count */}
          <div style={{ marginBottom: "24px" }}>
            <Text
              style={{
                color: "rgba(255, 255, 255, 0.8)",
                fontSize: "16px",
                fontWeight: "500",
              }}
            >
              Tìm thấy {filteredRestaurants.length} nhà hàng
            </Text>
          </div>
          <Row gutter={[24, 24]}>
            {filteredRestaurants.map((restaurant) => (
              <Col xs={24} lg={12} key={restaurant.id}>
                {" "}
                <Card
                  hoverable
                  onClick={() => handleRestaurantClick(restaurant.id)}
                  style={{
                    background: "rgba(255, 255, 255, 0.05)",
                    border: "1px solid rgba(255, 255, 255, 0.1)",
                    borderRadius: "20px",
                    overflow: "hidden",
                    cursor: "pointer",
                    transition: "all 0.3s ease",
                    boxShadow: "0 4px 20px rgba(0, 0, 0, 0.3)",
                  }}                  bodyStyle={{ padding: "0", height: "100%" }}
                >
                  <Row style={{ height: "100%" }}>
                    {" "}                    <Col xs={24} sm={10}>
                      <div
                        style={{
                          height: "100%",
                          minHeight: "350px",
                          backgroundImage: `url(${restaurant.image})`,
                          backgroundSize: "cover",
                          backgroundPosition: "center",
                          position: "relative",
                          borderRadius: "20px 0 0 20px",
                        }}
                      >
                        <div
                          style={{
                            position: "absolute",
                            top: "16px",
                            right: "16px",
                            display: "flex",
                            gap: "8px",
                          }}
                        >
                          {" "}
                          <Badge
                            status={restaurant.isOpen ? "success" : "error"}
                            text={restaurant.isOpen ? "Mở cửa" : "Đóng cửa"}
                            style={{
                              background: "rgba(0, 0, 0, 0.8)",
                              padding: "4px 12px",
                              borderRadius: "20px",
                              fontSize: "12px",
                              fontWeight: "500",
                              color: restaurant.isOpen ? "#52c41a" : "#ff4d4f",
                            }}
                          />
                        </div>
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
                          {" "}
                          <Title
                            level={4}
                            style={{
                              color: "#ffffff",
                              margin: 0,
                              fontSize: "18px",
                            }}
                          >
                            {restaurant.name}
                          </Title>
                          <Text
                            style={{
                              color: "#ffa366",
                              fontSize: "14px",
                              fontWeight: "600",
                            }}
                          >
                            {restaurant.distance}
                          </Text>
                        </div>
                        <Space
                          direction="vertical"
                          size="small"
                          style={{ width: "100%" }}
                        >
                          <div
                            style={{ display: "flex", alignItems: "center" }}
                          >
                            <Rate
                              disabled
                              defaultValue={restaurant.rating}
                              style={{ fontSize: "16px" }}
                            />{" "}
                            <Text
                              style={{
                                color: "rgba(255, 255, 255, 0.8)",
                                marginLeft: "8px",
                                fontSize: "14px",
                              }}
                            >
                              {restaurant.rating} ({restaurant.reviewCount} đánh
                              giá)
                            </Text>
                          </div>

                          <div
                            style={{ display: "flex", alignItems: "center" }}
                          >
                            <EnvironmentOutlined
                              style={{ color: "#ffa366", marginRight: "8px" }}
                            />
                            <Text
                              style={{
                                color: "rgba(255, 255, 255, 0.7)",
                                fontSize: "13px",
                              }}
                            >
                              {restaurant.address}
                            </Text>
                          </div>

                          <div
                            style={{ display: "flex", alignItems: "center" }}
                          >
                            <ClockCircleOutlined
                              style={{ color: "#ffa366", marginRight: "8px" }}
                            />
                            <Text
                              style={{
                                color: "rgba(255, 255, 255, 0.7)",
                                fontSize: "13px",
                              }}
                            >
                              {restaurant.openTime} - {restaurant.closeTime}
                            </Text>
                          </div>

                          <div
                            style={{ display: "flex", alignItems: "center" }}
                          >
                            <PhoneOutlined
                              style={{ color: "#ffa366", marginRight: "8px" }}
                            />
                            <Text
                              style={{
                                color: "rgba(255, 255, 255, 0.7)",
                                fontSize: "13px",
                              }}
                            >
                              {restaurant.phone}
                            </Text>
                          </div>

                          <Text
                            style={{
                              color: "#ffa366",
                              fontSize: "15px",
                              fontWeight: "600",
                              marginTop: "4px",
                            }}
                          >
                            {restaurant.priceRange}
                          </Text>

                          <div style={{ marginTop: "12px" }}>
                            {restaurant.specialties
                              .slice(0, 3)
                              .map((specialty, index) => (
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
                                  {specialty}
                                </Tag>
                              ))}
                          </div>
                        </Space>{" "}
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
                              handleRestaurantClick(restaurant.id);
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
                          <Button
                            onClick={(e) => e.stopPropagation()}
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
