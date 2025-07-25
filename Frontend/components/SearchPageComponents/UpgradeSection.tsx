import React from "react";
import Link from "next/link";
import { Card, Button, Typography, Flex } from "antd";
import { CrownOutlined } from "@ant-design/icons";

const { Text, Paragraph } = Typography;

const UpgradeSection: React.FC = () => {
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
            "linear-gradient(90deg, rgba(255, 122, 0, 0.1) 0%, rgba(255, 149, 0, 0.05) 100%)",
          border: "1px solid rgba(255, 122, 0, 0.2)",
          borderRadius: "8px",
        }}
        styles={{ body: { padding: "16px" } }}
      >
        <Flex align="center" gap={12} style={{ marginBottom: "8px" }}>
          <CrownOutlined style={{ color: "#ff7a00", fontSize: "20px" }} />
          <Text
            style={{ color: "#ff9500", fontWeight: "medium", fontSize: "14px" }}
          >
            Trở thành Pro
          </Text>
        </Flex>
        <Paragraph
          style={{ color: "#b3b3b3", fontSize: "12px", margin: "0 0 12px 0" }}
        >
          Truy cập không giới hạn và các tính năng cao cấp{" "}
        </Paragraph>
        <Link href="/pricing">
          <Button
            type="primary"
            size="small"
            block
            style={{
              background: "linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)",
              border: "none",
              borderRadius: "6px",
              height: "32px",
              fontSize: "12px",
              fontWeight: "medium",
            }}
          >
            Nâng cấp ngay
          </Button>
        </Link>
      </Card>
    </div>
  );
};

export default UpgradeSection;
