"use client";
import React, { useEffect, useState } from "react";
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
  Image,
  Badge,
  Dropdown,
  Space,
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

const { Content, Header } = Layout;
const { Title, Text } = Typography;

interface MenuItem {
  id: string;
  name: string;
  price: number;
  description: string;
  image: string;
  category: string;
  isPopular?: boolean;
}

interface Review {
  id: string;
  userName: string;
  rating: number;
  comment: string;
  date: string;
  userAvatar?: string;
}

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
  description: string;
  menu: MenuItem[];
  reviews: Review[];
}

const RestaurantDetail: React.FC = () => {
  const router = useRouter();
  const params = useParams();
  const [restaurant, setRestaurant] = useState<Restaurant | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<string>("all");
  const [selectedRating, setSelectedRating] = useState<number>(0);
  const [menuSortBy, setMenuSortBy] = useState<string>("popular");
  const [reviewSortBy, setReviewSortBy] = useState<string>("newest");

  useEffect(() => {
    // Mock data for restaurant detail
    const mockRestaurant: Restaurant = {
      id: params.id as string,
      name: "Phở Hùng",
      address: "123 Nguyễn Văn Dậu, Bình Thạnh, TP.HCM",
      phone: "0901234567",
      rating: 4.5,
      reviewCount: 324,
      openTime: "06:00",
      closeTime: "22:00",
      isOpen: true,
      image:
        "https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=800",
      description:
        "Phở Hùng là một trong những quán phở truyền thống nổi tiếng nhất tại Bình Thạnh. Với hơn 20 năm kinh nghiệm, chúng tôi tự hào mang đến cho khách hàng những tô phở đậm đà hương vị Bắc.",
      menu: [
        {
          id: "1",
          name: "Phở Bò Tái",
          price: 65000,
          description: "Phở bò tái với thịt bò tươi ngon, nước dùng trong vắt",
          image:
            "https://images.unsplash.com/photo-1569562211093-4ed0d0758f12?w=300",
          category: "Phở",
          isPopular: true,
        },
        {
          id: "2",
          name: "Phở Bò Chín",
          price: 60000,
          description: "Phở bò chín với thịt bò mềm, nước dùng đậm đà",
          image:
            "https://th.bing.com/th/id/OIP.px-hhtxrjdS3JkV0RC0uUgHaFJ?rs=1&pid=ImgDetMain",
          category: "Phở",
        },
        {
          id: "3",
          name: "Phở Gà",
          price: 55000,
          description: "Phở gà với thịt gà ta, nước dùng ngọt thanh",
          image:
            "https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=300",
          category: "Phở",
        },
        {
          id: "4",
          name: "Bún Bò Huế",
          price: 70000,
          description: "Bún bò Huế cay nồng với đầy đủ nguyên liệu",
          image:
            "https://images.unsplash.com/photo-1567620905732-2d1ec7ab7445?w=300",
          category: "Bún",
          isPopular: true,
        },
        {
          id: "5",
          name: "Bánh Mì Thịt",
          price: 25000,
          description: "Bánh mì thịt nướng với đầy đủ rau củ",
          image:
            "https://images.unsplash.com/photo-1558030006-450675393462?w=300",
          category: "Bánh Mì",
        },
        {
          id: "6",
          name: "Chè Đậu Xanh",
          price: 20000,
          description: "Chè đậu xanh mát lành, ngọt dịu",
          image:
            "https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=300",
          category: "Tráng miệng",
        },
      ],
      reviews: [
        {
          id: "1",
          userName: "Nguyễn Văn A",
          rating: 5,
          comment:
            "Phở rất ngon, nước dùng đậm đà, thịt bò tươi. Sẽ quay lại lần sau!",
          date: "2024-01-15",
          userAvatar: "https://i.pravatar.cc/150?img=1",
        },
        {
          id: "2",
          userName: "Trần Thị B",
          rating: 4,
          comment:
            "Quán sạch sẽ, phục vụ nhanh. Phở ngon nhưng hơi mặn một chút.",
          date: "2024-01-10",
          userAvatar: "https://i.pravatar.cc/150?img=2",
        },
        {
          id: "3",
          userName: "Lê Văn C",
          rating: 5,
          comment: "Đã ăn ở đây nhiều lần, chất lượng luôn ổn định. Recommend!",
          date: "2024-01-08",
          userAvatar: "https://i.pravatar.cc/150?img=3",
        },
        {
          id: "4",
          userName: "Phạm Thị D",
          rating: 3,
          comment:
            "Phở bình thường, không có gì đặc biệt. Giá hơi cao so với chất lượng.",
          date: "2024-01-05",
          userAvatar: "https://i.pravatar.cc/150?img=4",
        },
        {
          id: "5",
          userName: "Hoàng Văn E",
          rating: 4,
          comment: "Bún bò Huế ngon, cay vừa phải. Nhân viên thân thiện.",
          date: "2024-01-02",
          userAvatar: "https://i.pravatar.cc/150?img=5",
        },
      ],
    };

    setRestaurant(mockRestaurant);
  }, [params.id]);

  const handleBackClick = () => {
    router.back();
  };

  if (!restaurant) {
    return <div>Loading...</div>;
  }
  const categories = [
    "all",
    ...Array.from(new Set(restaurant.menu.map((item) => item.category))),
  ];

  let filteredMenu =
    selectedCategory === "all"
      ? restaurant.menu
      : restaurant.menu.filter((item) => item.category === selectedCategory);

  // Sort menu based on selection
  if (menuSortBy === "popular") {
    filteredMenu = filteredMenu.sort(
      (a, b) => (b.isPopular ? 1 : 0) - (a.isPopular ? 1 : 0)
    );
  } else if (menuSortBy === "price-low") {
    filteredMenu = filteredMenu.sort((a, b) => a.price - b.price);
  } else if (menuSortBy === "price-high") {
    filteredMenu = filteredMenu.sort((a, b) => b.price - a.price);
  }

  let filteredReviews =
    selectedRating === 0
      ? restaurant.reviews
      : restaurant.reviews.filter((review) => review.rating === selectedRating);

  // Sort reviews based on selection
  if (reviewSortBy === "newest") {
    filteredReviews = filteredReviews.sort(
      (a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()
    );
  } else if (reviewSortBy === "oldest") {
    filteredReviews = filteredReviews.sort(
      (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
    );
  } else if (reviewSortBy === "rating-high") {
    filteredReviews = filteredReviews.sort((a, b) => b.rating - a.rating);
  } else if (reviewSortBy === "rating-low") {
    filteredReviews = filteredReviews.sort((a, b) => a.rating - b.rating);
  } // Calculate rating distribution
  const ratingDistribution = [5, 4, 3, 2, 1].map((rating) => ({
    rating,
    count: restaurant.reviews.filter((r) => r.rating === rating).length,
    percentage:
      (restaurant.reviews.filter((r) => r.rating === rating).length /
        restaurant.reviews.length) *
      100,
  }));

  // Menu filter dropdown items
  const menuFilterItems = [
    {
      key: "popular",
      label: "Món phổ biến",
    },
    {
      key: "price-low",
      label: "Giá thấp đến cao",
    },
    {
      key: "price-high",
      label: "Giá cao đến thấp",
    },
  ];

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
      </Header>{" "}
      <Content
        style={{
          padding: "24px",
          overflow: "auto",
        }}
      >
        <div style={{ maxWidth: "1200px", margin: "0 auto" }}>
          {" "}
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
              {" "}
              <Col xs={24} md={10}>
                <Image
                  src={restaurant.image}
                  alt={restaurant.name}
                  style={{
                    width: "100%",
                    height: "300px",
                    objectFit: "cover",
                    borderRadius: "16px",
                  }}
                  preview={false}
                />
              </Col>
              <Col xs={24} md={14}>
                <div style={{ padding: "20px 0" }}>
                  {" "}
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
                      status={restaurant.isOpen ? "success" : "error"}
                      text={restaurant.isOpen ? "Đang mở cửa" : "Đã đóng cửa"}
                      style={{
                        color: restaurant.isOpen ? "#52c41a" : "#ff4d4f",
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
                      {restaurant.rating} ({restaurant.reviewCount} đánh giá)
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
                        {restaurant.address}
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
                        {restaurant.openTime} - {restaurant.closeTime}
                      </Text>
                    </div>

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
                        {restaurant.phone}
                      </Text>
                    </div>
                  </Space>
                  <Divider
                    style={{
                      margin: "20px 0",
                      borderColor: "rgba(255, 255, 255, 0.2)",
                    }}
                  />
                  <Text
                    style={{
                      color: "rgba(255, 255, 255, 0.8)",
                      fontSize: "15px",
                      lineHeight: "1.6",
                      display: "block",
                      marginBottom: "24px",
                    }}
                  >
                    {restaurant.description}
                  </Text>
                  <Space size="middle">
                    <Button
                      type="primary"
                      size="large"
                      style={{
                        background:
                          "linear-gradient(135deg, #ffa366 0%, #ff8c42 100%)",
                        border: "none",
                        borderRadius: "12px",
                        height: "48px",
                        paddingLeft: "32px",
                        paddingRight: "32px",
                        fontWeight: "500",
                      }}
                    >
                      Đặt bàn ngay
                    </Button>
                    <Button
                      size="large"
                      style={{
                        borderRadius: "12px",
                        height: "48px",
                        paddingLeft: "32px",
                        paddingRight: "32px",
                        fontWeight: "500",
                        borderColor: "#ffa366",
                        color: "#ffa366",
                        background: "rgba(255, 163, 102, 0.1)",
                      }}
                    >
                      Gọi điện
                    </Button>
                  </Space>
                </div>
              </Col>
            </Row>
          </Card>{" "}
          <Row gutter={24}>
            {/* Menu Section */}{" "}
            <Col xs={24} lg={14}>
              <Card
                style={{
                  background: "rgba(255, 255, 255, 0.05)",
                  border: "1px solid rgba(255, 255, 255, 0.1)",
                  borderRadius: "20px",
                  marginBottom: "24px",
                  boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
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
                    Thực đơn
                  </Title>
                  <Dropdown
                    menu={{
                      items: menuFilterItems,
                      onClick: ({ key }) => setMenuSortBy(key),
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
                          menuFilterItems.find(
                            (item) => item.key === menuSortBy
                          )?.label
                        }
                        <DownOutlined />
                      </Space>
                    </Button>
                  </Dropdown>
                </div>{" "}
                {/* Category Filter */}
                <div style={{ marginBottom: "24px" }}>
                  {categories.map((category) => (
                    <Button
                      key={category}
                      type={
                        selectedCategory === category ? "primary" : "default"
                      }
                      onClick={() => setSelectedCategory(category)}
                      style={{
                        marginRight: "8px",
                        marginBottom: "8px",
                        borderRadius: "20px",
                        border:
                          selectedCategory === category
                            ? "none"
                            : "1px solid rgba(255, 255, 255, 0.3)",
                        background:
                          selectedCategory === category
                            ? "#ffa366"
                            : "rgba(255, 255, 255, 0.05)",
                        color:
                          selectedCategory === category
                            ? "#000000"
                            : "rgba(255, 255, 255, 0.8)",
                      }}
                    >
                      {category === "all" ? "Tất cả" : category}
                    </Button>
                  ))}
                </div>
                {/* Menu Items */}
                <Row gutter={[16, 16]}>
                  {filteredMenu.map((item) => (
                    <Col xs={24} sm={12} key={item.id}>
                      {" "}
                      <Card
                        hoverable
                        style={{
                          borderRadius: "16px",
                          border: "1px solid rgba(255, 255, 255, 0.1)",
                          background: "rgba(255, 255, 255, 0.05)",
                          overflow: "hidden",
                          transition: "all 0.3s ease",
                        }}
                        cover={
                          <div style={{ position: "relative" }}>
                            <div
                              style={{
                                width: "100%",
                                height: "140px",
                                backgroundImage: `url(${item.image})`,
                                backgroundSize: "cover",
                                backgroundPosition: "center",
                                borderRadius: "8px 8px 0 0",
                              }}
                            />
                            {item.isPopular && (
                              <Tag
                                color="orange"
                                style={{
                                  position: "absolute",
                                  top: "12px",
                                  right: "12px",
                                  borderRadius: "12px",
                                  fontWeight: "500",
                                  background: "#ffa366",
                                  border: "none",
                                }}
                              >
                                Phổ biến
                              </Tag>
                            )}
                          </div>
                        }
                      >
                        <div style={{ padding: "8px" }}>
                          <Title
                            level={5}
                            style={{ color: "#ffffff", marginBottom: "8px" }}
                          >
                            {item.name}
                          </Title>
                          <Text
                            style={{
                              color: "rgba(255, 255, 255, 0.7)",
                              fontSize: "13px",
                              display: "block",
                              marginBottom: "12px",
                              lineHeight: "1.4",
                            }}
                          >
                            {item.description}
                          </Text>
                          <Text
                            style={{
                              color: "#ffa366",
                              fontSize: "16px",
                              fontWeight: "600",
                            }}
                          >
                            {item.price.toLocaleString()}đ
                          </Text>
                        </div>
                      </Card>
                    </Col>
                  ))}
                </Row>
              </Card>
            </Col>{" "}
            {/* Reviews Section */}
            <Col xs={24} lg={10}>
              <Card
                style={{
                  background: "rgba(255, 255, 255, 0.05)",
                  border: "1px solid rgba(255, 255, 255, 0.1)",
                  borderRadius: "20px",
                  boxShadow: "0 8px 32px rgba(0, 0, 0, 0.3)",
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
                      onClick: ({ key }) => setReviewSortBy(key),
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
                    Dựa trên {restaurant.reviewCount} đánh giá
                  </Text>
                </div>{" "}
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
                          setSelectedRating(
                            selectedRating === rating ? 0 : rating
                          )
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
                <Divider style={{ borderColor: "rgba(255, 255, 255, 0.2)" }} />{" "}
                {/* Reviews List */}
                <div style={{ maxHeight: "500px", overflowY: "auto" }}>
                  {filteredReviews.map((review) => (
                    <div
                      key={review.id}
                      style={{
                        marginBottom: "20px",
                        paddingBottom: "20px",
                        borderBottom: "1px solid rgba(255, 255, 255, 0.1)",
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
                          src={review.userAvatar}
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
                              {review.userName}
                            </Text>
                            <Text
                              style={{
                                color: "rgba(255, 255, 255, 0.6)",
                                fontSize: "13px",
                              }}
                            >
                              {review.date}
                            </Text>
                          </div>
                          <Rate
                            disabled
                            defaultValue={review.rating}
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
                        {review.comment}
                      </Text>
                    </div>
                  ))}
                </div>
              </Card>
            </Col>
          </Row>
        </div>
      </Content>
    </Layout>
  );
};

export default RestaurantDetail;
