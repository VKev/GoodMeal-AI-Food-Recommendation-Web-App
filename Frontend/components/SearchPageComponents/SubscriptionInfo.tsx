import React from "react";
import { Card, Typography, Flex, Tag } from "antd";
import { CheckCircleOutlined, CalendarOutlined } from "@ant-design/icons";

const { Text, Paragraph } = Typography;

interface SubscriptionInfoProps {
  subscription: {
    id: string;
    userId: string;
    subscriptionId: string;
    subscription?: {
      name: string;
      description: string;
      price: number;
      durationInMonths: number;
      currency: string;
    };
    subscriptionName?: string;
    subscriptionPrice?: number;
    durationInMonths?: number;
    currency?: string;
    startDate: string;
    endDate: string;
    isActive: boolean;
  };
}

const SubscriptionInfo: React.FC<SubscriptionInfoProps> = ({ subscription }) => {
  // Format dates
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("vi-VN");
  };

  // Format currency
  const formatCurrency = (amount: number, currency: string) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: currency || "VND",
    }).format(amount);
  };

  const subscriptionName = subscription.subscriptionName || (subscription.subscription?.name || "");
  const subscriptionPrice = subscription.subscriptionPrice || (subscription.subscription?.price || 0);
  const currency = subscription.currency || (subscription.subscription?.currency || "VND");

  return (
    <div
      style={{
        padding: "16px",
        borderTop: "1px solid #404040",
        flexShrink: 0,
        marginTop: "auto",
      }}
    >
      <Card
        style={{
          background:
            "linear-gradient(90deg, rgba(0, 180, 100, 0.1) 0%, rgba(0, 200, 100, 0.05) 100%)",
          border: "1px solid rgba(0, 180, 100, 0.2)",
          borderRadius: "8px",
        }}
        bodyStyle={{ padding: "16px" }}
      >
        <Flex align="center" gap={12} style={{ marginBottom: "8px" }}>
          <CheckCircleOutlined style={{ color: "#00b464", fontSize: "20px" }} />
          <Text
            style={{ color: "#00c864", fontWeight: "medium", fontSize: "14px" }}
          >
            {subscriptionName}
          </Text>
          <Tag color="success" style={{ marginLeft: "auto" }}>
            {subscription.isActive ? "Đang hoạt động" : "Hết hạn"}
          </Tag>
        </Flex>
        <Paragraph
          style={{ color: "#b3b3b3", fontSize: "12px", margin: "0 0 8px 0" }}
        >
          {formatCurrency(subscriptionPrice, currency)}
        </Paragraph>
        <Flex align="center" gap={8} style={{ fontSize: "12px", color: "#b3b3b3" }}>
          <CalendarOutlined />
          <Text style={{ color: "#b3b3b3", fontSize: "12px" }}>
            Hết hạn: {formatDate(subscription.endDate)}
          </Text>
        </Flex>
      </Card>
    </div>
  );
};

export default SubscriptionInfo; 