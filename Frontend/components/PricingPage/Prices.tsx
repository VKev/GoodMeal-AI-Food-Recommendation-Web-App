import React, { useState, useEffect } from "react";
import { Row, Col, Typography, Card, message, Spin, Empty, Modal, Button } from "antd";
import {
  QuestionCircleOutlined,
  CloseOutlined,
  LoadingOutlined,
  CheckCircleOutlined,
} from "@ant-design/icons";
import { useRouter } from "next/navigation";
import { useAuth } from "../../hooks/auths/authContext";
import SubscriptionCard from "./SubscriptionCard";
import { 
  getAllSubscriptions, 
  Subscription, 
  registerSubscription, 
  getSubscriptionPaymentStatus 
} from "../../services/SubscriptionService";

const { Title, Paragraph, Text } = Typography;

const Prices: React.FC = () => {
  const [subscriptions, setSubscriptions] = useState<Subscription[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [activeSubscription, setActiveSubscription] = useState<string>("");
  const [processingSubscription, setProcessingSubscription] = useState<boolean>(false);
  const [correlationId, setCorrelationId] = useState<string | null>(null);
  const [statusCheckInterval, setStatusCheckInterval] = useState<NodeJS.Timeout | null>(null);
  const [paymentStatus, setPaymentStatus] = useState<string | null>(null);
  const [paymentModalVisible, setPaymentModalVisible] = useState<boolean>(false);
  
  const router = useRouter();
  const { currentUser } = useAuth();
  
  // Fetch subscriptions from the backend
  useEffect(() => {
    const fetchSubscriptions = async () => {
      setLoading(true);
      try {
        if (currentUser) {
          const idToken = await currentUser.getIdToken();
          const data = await getAllSubscriptions(idToken);
          if (data && data.length > 0) {
            // Sort subscriptions by price (ascending)
            const sortedSubscriptions = [...data].sort((a, b) => a.price - b.price);
            setSubscriptions(sortedSubscriptions);
            
            // Set the middle subscription as active by default
            const middleIndex = Math.floor(sortedSubscriptions.length / 2);
            setActiveSubscription(sortedSubscriptions[middleIndex].id);
          }
        }
      } catch (error) {
        console.error("Error fetching subscriptions:", error);
        message.error("Không thể tải thông tin gói dịch vụ. Vui lòng thử lại sau.");
      } finally {
        setLoading(false);
      }
    };

    fetchSubscriptions();
  }, [currentUser]);

  // Clean up interval when component unmounts
  useEffect(() => {
    return () => {
      if (statusCheckInterval) {
        clearInterval(statusCheckInterval);
      }
    };
  }, [statusCheckInterval]);

  // Check payment status
  const checkPaymentStatus = async () => {
    if (!currentUser || !correlationId) return;

    try {
      const idToken = await currentUser.getIdToken();
      const statusResponse = await getSubscriptionPaymentStatus(idToken, correlationId);
      
      if (statusResponse && statusResponse.isSuccess) {
        const status = statusResponse.value;
        setPaymentStatus(status.status);
        
        if (status.isCompleted) {
          if (statusCheckInterval) {
            clearInterval(statusCheckInterval);
            setStatusCheckInterval(null);
          }
          
          message.success("Đăng ký gói dịch vụ thành công!");
          setPaymentModalVisible(false);
          router.push("/c");
        }
      } else {
        console.error("Error checking payment status:", statusResponse);
      }
    } catch (error) {
      console.error("Error checking payment status:", error);
    }
  };

  const handleSelectSubscription = async (subscriptionId: string) => {
    try {
      if (!currentUser) {
        message.warning("Vui lòng đăng nhập để tiếp tục.");
        router.push("/sign-in");
        return;
      }
      
      setProcessingSubscription(true);
      const idToken = await currentUser.getIdToken();
      const result = await registerSubscription(idToken, subscriptionId);
      
      if (result.success && result.data) {
        setCorrelationId(result.data.correlationId);
        setPaymentStatus("Đang xử lý");
        setPaymentModalVisible(true);
        
        // Start checking payment status
        const interval = setInterval(checkPaymentStatus, 5000); // Check every 5 seconds
        setStatusCheckInterval(interval);
      } else {
        message.error("Không thể đăng ký gói dịch vụ. Vui lòng thử lại sau.");
      }
    } catch (error) {
      console.error("Error handling subscription selection:", error);
      message.error("Có lỗi xảy ra. Vui lòng thử lại sau.");
    } finally {
      setProcessingSubscription(false);
    }
  };

  const handleManualConfirm = async () => {
    if (!correlationId) return;
    
    try {
      await checkPaymentStatus();
      message.info("Đang kiểm tra trạng thái thanh toán...");
    } catch (error) {
      console.error("Error confirming payment:", error);
      message.error("Không thể xác nhận thanh toán. Vui lòng thử lại sau.");
    }
  };

  const handleCardClick = (subscriptionId: string) => {
    setActiveSubscription(subscriptionId);
  };

  const handleClose = () => {
    router.push("/c");
  };

  // FAQ data
  const faqs = [
    {
      question: "Tôi có thể hủy đăng ký bất cứ lúc nào không?",
      answer:
        "Có, bạn có thể hủy đăng ký bất cứ lúc nào mà không mất phí. Đăng ký của bạn sẽ tiếp tục hoạt động cho đến hết chu kỳ thanh toán hiện tại.",
    },
    {
      question: "Bạn có hoàn tiền không?",
      answer:
        "Chúng tôi cung cấp bảo đảm hoàn tiền 100% trong vòng 30 ngày đầu tiên nếu bạn không hài lòng với dịch vụ của chúng tôi.",
    },
    {
      question: "Dữ liệu của tôi có an toàn không?",
      answer:
        "Hoàn toàn an toàn! Chúng tôi sử dụng mã hóa SSL 256-bit và tuân thủ các tiêu chuẩn bảo mật quốc tế để bảo vệ dữ liệu của bạn.",
    },
    {
      question: "Tôi có thể thay đổi gói đăng ký của mình không?",
      answer:
        "Có, bạn có thể nâng cấp hoặc hạ cấp gói đăng ký bất cứ lúc nào. Thay đổi sẽ có hiệu lực ngay lập tức.",
    },
  ];

  return (
    <div
      style={{
        minHeight: "100vh",
        background: "linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)",
        padding: "80px 24px 40px",
        position: "relative",
      }}
    >
      {/* Close Button */}
      <div
        onClick={handleClose}
        style={{
          position: "fixed",
          top: "24px",
          right: "24px",
          width: "48px",
          height: "48px",
          background: "rgba(255, 255, 255, 0.1)",
          borderRadius: "50%",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          cursor: "pointer",
          border: "1px solid rgba(255, 255, 255, 0.2)",
          transition: "all 0.3s ease",
          zIndex: 1000,
        }}
        onMouseEnter={(e) => {
          e.currentTarget.style.background = "rgba(255, 122, 0, 0.2)";
          e.currentTarget.style.borderColor = "#ff7a00";
        }}
        onMouseLeave={(e) => {
          e.currentTarget.style.background = "rgba(255, 255, 255, 0.1)";
          e.currentTarget.style.borderColor = "rgba(255, 255, 255, 0.2)";
        }}
      >
        <CloseOutlined
          style={{
            color: "#ffffff",
            fontSize: "20px",
          }}
        />
      </div>

      {/* Payment Status Modal */}
      <Modal
        title="Trạng thái đăng ký gói dịch vụ"
        open={paymentModalVisible}
        onCancel={() => {
          if (statusCheckInterval) {
            clearInterval(statusCheckInterval);
            setStatusCheckInterval(null);
          }
          setPaymentModalVisible(false);
        }}
        footer={[
          <Button 
            key="confirm" 
            type="primary" 
            onClick={handleManualConfirm}
            loading={processingSubscription}
          >
            Xác nhận đã thanh toán
          </Button>
        ]}
      >
        <div style={{ textAlign: 'center', padding: '20px 0' }}>
          {paymentStatus === "Đang xử lý" ? (
            <>
              <LoadingOutlined style={{ fontSize: 48, color: '#ff7a00', marginBottom: 16 }} />
              <Paragraph>
                Đang xử lý đăng ký gói dịch vụ của bạn...
              </Paragraph>
            </>
          ) : paymentStatus === "Completed" ? (
            <>
              <CheckCircleOutlined style={{ fontSize: 48, color: '#52c41a', marginBottom: 16 }} />
              <Paragraph>
                Đăng ký gói dịch vụ thành công!
              </Paragraph>
            </>
          ) : (
            <Paragraph>
              Vui lòng hoàn tất thanh toán để kích hoạt gói dịch vụ. Sau khi thanh toán, nhấn nút "Xác nhận đã thanh toán".
            </Paragraph>
          )}
        </div>
      </Modal>

      {/* Header Section */}
      <div
        style={{
          textAlign: "center",
          maxWidth: "800px",
          margin: "0 auto 64px auto",
        }}
      >
        <Title
          level={1}
          style={{
            margin: "0 0 16px 0",
            background: "linear-gradient(45deg, #ffe0b3 0%, #ffd699 100%)",
            backgroundClip: "text",
            WebkitBackgroundClip: "text",
            color: "transparent",
            fontSize: "48px",
            fontWeight: "bold",
          }}
        >
          Nâng cấp gói của bạn
        </Title>
        <Paragraph
          style={{
            color: "#b3b3b3",
            fontSize: "18px",
            lineHeight: "1.6",
            margin: "0 0 32px 0",
          }}
        >
          Từ người dùng cá nhân đến doanh nghiệp lớn, chúng tôi có các giải pháp
          gợi ý món ăn AI phù hợp với mọi nhu cầu
        </Paragraph>
      </div>

      {/* Subscription Cards */}
      <div style={{ maxWidth: "1200px", margin: "0 auto 80px auto" }}>
        {loading ? (
          <div style={{ textAlign: "center", padding: "40px 0" }}>
            <Spin size="large" />
            <Paragraph style={{ color: "#b3b3b3", marginTop: "16px" }}>
              Đang tải thông tin gói dịch vụ...
            </Paragraph>
          </div>
        ) : subscriptions.length === 0 ? (
          <Empty
            description={
              <span style={{ color: "#b3b3b3" }}>
                Không tìm thấy gói dịch vụ nào
              </span>
            }
            image={Empty.PRESENTED_IMAGE_SIMPLE}
            style={{ color: "#ffffff" }}
          />
        ) : (
          <Row gutter={[24, 24]} justify="center">
            {subscriptions.map((subscription) => (
              <Col key={subscription.id} xs={24} md={8}>
                <SubscriptionCard
                  subscription={subscription}
                  isActive={activeSubscription === subscription.id}
                  onSelectSubscription={handleSelectSubscription}
                  onCardClick={handleCardClick}
                />
              </Col>
            ))}
          </Row>
        )}
      </div>

      {/* Feature Comparison */}
      <div style={{ maxWidth: "800px", margin: "0 auto 80px auto" }}>
        <Title
          level={2}
          style={{
            textAlign: "center",
            color: "#ffffff",
            marginBottom: "40px",
          }}
        >
          Tại sao nên nâng cấp?
        </Title>
        <Row gutter={[24, 24]}>
          <Col xs={24} md={8}>
            <Card
              style={{
                background: "rgba(255, 255, 255, 0.05)",
                border: "1px solid rgba(255, 255, 255, 0.1)",
                borderRadius: "16px",
                height: "100%",
              }}
            >
              <div style={{ textAlign: "center" }}>
                <div
                  style={{
                    fontSize: "32px",
                    marginBottom: "16px",
                  }}
                >
                  🚀
                </div>
                <Title level={4} style={{ color: "#ffffff" }}>
                  Trải nghiệm không giới hạn
                </Title>
                <Text style={{ color: "#b3b3b3" }}>
                  Truy cập không giới hạn vào tất cả các tính năng gợi ý món ăn AI
                </Text>
              </div>
            </Card>
          </Col>
          <Col xs={24} md={8}>
            <Card
              style={{
                background: "rgba(255, 255, 255, 0.05)",
                border: "1px solid rgba(255, 255, 255, 0.1)",
                borderRadius: "16px",
                height: "100%",
              }}
            >
              <div style={{ textAlign: "center" }}>
                <div
                  style={{
                    fontSize: "32px",
                    marginBottom: "16px",
                  }}
                >
                  🔒
                </div>
                <Title level={4} style={{ color: "#ffffff" }}>
                  Bảo mật cao
                </Title>
                <Text style={{ color: "#b3b3b3" }}>
                  Dữ liệu của bạn luôn được bảo vệ với mã hóa SSL 256-bit
                </Text>
              </div>
            </Card>
          </Col>
          <Col xs={24} md={8}>
            <Card
              style={{
                background: "rgba(255, 255, 255, 0.05)",
                border: "1px solid rgba(255, 255, 255, 0.1)",
                borderRadius: "16px",
                height: "100%",
              }}
            >
              <div style={{ textAlign: "center" }}>
                <div
                  style={{
                    fontSize: "32px",
                    marginBottom: "16px",
                  }}
                >
                  💬
                </div>
                <Title level={4} style={{ color: "#ffffff" }}>
                  Hỗ trợ 24/7
                </Title>
                <Text style={{ color: "#b3b3b3" }}>
                  Đội ngũ hỗ trợ luôn sẵn sàng giúp đỡ bạn mọi lúc
                </Text>
              </div>
            </Card>
          </Col>
        </Row>
      </div>

      {/* FAQ Section */}
      <div style={{ maxWidth: "800px", margin: "0 auto" }}>
        <Title
          level={2}
          style={{
            textAlign: "center",
            color: "#ffffff",
            marginBottom: "40px",
          }}
        >
          Câu hỏi thường gặp
        </Title>
        <Row gutter={[0, 16]}>
          {faqs.map((faq, index) => (
            <Col span={24} key={index}>
              <Card
                style={{
                  background: "rgba(255, 255, 255, 0.05)",
                  border: "1px solid rgba(255, 255, 255, 0.1)",
                  borderRadius: "12px",
                }}
              >
                <div
                  style={{
                    display: "flex",
                    alignItems: "flex-start",
                    gap: "12px",
                  }}
                >
                  <QuestionCircleOutlined
                    style={{
                      color: "#ff7a00",
                      fontSize: "18px",
                      marginTop: "2px",
                    }}
                  />
                  <div>
                    <Text
                      style={{
                        color: "#ffffff",
                        fontWeight: "medium",
                        fontSize: "16px",
                        display: "block",
                        marginBottom: "8px",
                      }}
                    >
                      {faq.question}
                    </Text>
                    <Text
                      style={{
                        color: "#b3b3b3",
                        fontSize: "14px",
                        lineHeight: "1.5",
                      }}
                    >
                      {faq.answer}
                    </Text>
                  </div>
                </div>
              </Card>
            </Col>
          ))}
        </Row>
      </div>
    </div>
  );
};

export default Prices;
