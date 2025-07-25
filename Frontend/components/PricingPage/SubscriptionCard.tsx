"use client";

import React from 'react';
import { Card, Button, Typography, List } from 'antd';
import { CheckOutlined, CrownOutlined } from '@ant-design/icons';
import { Subscription } from '../../services/SubscriptionService';

const { Title, Text, Paragraph } = Typography;

interface SubscriptionCardProps {
  subscription: Subscription;
  isActive: boolean;
  onSelectSubscription: (subscriptionId: string) => void;
  onCardClick: (subscriptionId: string) => void;
}

const SubscriptionCard: React.FC<SubscriptionCardProps> = ({ 
  subscription, 
  isActive, 
  onSelectSubscription, 
  onCardClick 
}) => {
  // Format price based on currency
  const formatPrice = (price: number, currency: string) => {
    if (currency === 'VND') {
      return new Intl.NumberFormat('vi-VN', { 
        style: 'currency', 
        currency: 'VND',
        maximumFractionDigits: 0 
      }).format(price);
    }
    return new Intl.NumberFormat('en-US', { 
      style: 'currency', 
      currency: currency 
    }).format(price);
  };

  // Determine if this is a premium plan (highest price)
  const isPremium = subscription.price >= 999000;
  
  // Generate features based on subscription data
  const generateFeatures = () => {
    const features = [];
    
    // Basic features for all plans
    features.push(`Gói thuê bao ${subscription.durationInMonths} tháng`);
    
    // Add features based on subscription type
    if (subscription.price < 500000) {
      features.push("Trò chuyện AI Bot hàng ngày");
      features.push("Gợi ý món ăn dựa trên cảm xúc");
      features.push("Xem hình ảnh món ăn từ gợi ý");
      features.push("Tìm nhà hàng trên Google Maps");
    } else if (subscription.price < 1000000) {
      features.push("Trò chuyện AI Bot không giới hạn");
      features.push("Gợi ý món ăn AI thông minh");
      features.push("Phân tích cảm xúc & gợi ý món ăn cá nhân hóa");
      features.push("Xem hình ảnh HD của tất cả món ăn");
      features.push("Tích hợp Google Maps với đánh giá nhà hàng");
      features.push("Lưu món ăn yêu thích không giới hạn");
      features.push("Lịch sử trò chuyện & gợi ý cá nhân hóa");
    } else {
      features.push("Tất cả tính năng cơ bản");
      features.push("Trò chuyện AI Bot không giới hạn");
      features.push("Đăng ký quảng cáo nhà hàng");
      features.push("Hiển thị ưu tiên trong kết quả gợi ý");
      features.push("Hệ thống quản lý thực đơn nhà hàng");
      features.push("Phân tích dữ liệu khách hàng & xu hướng");
      features.push("Hỗ trợ ưu tiên & tư vấn chuyên gia");
    }
    
    return features;
  };

  return (
    <Card
      className={`subscription-card ${isPremium ? 'premium' : ''} ${isActive ? 'active' : ''}`}
      style={{
        height: '100%',
        position: 'relative',
        background: isActive
          ? 'linear-gradient(135deg, rgba(255, 183, 77, 0.18) 0%, rgba(255, 204, 128, 0.10) 100%)'
          : 'linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0.02) 100%)',
        border: isActive
          ? '3px solid #ff7a00'
          : isPremium
            ? '2px solid #ff7a00'
            : '1px solid rgba(255, 255, 255, 0.1)',
        borderRadius: '16px',
        boxShadow: isActive
          ? '0 12px 40px rgba(255, 122, 0, 0.3)'
          : isPremium
            ? '0 8px 32px rgba(255, 122, 0, 0.2)'
            : '0 4px 16px rgba(0, 0, 0, 0.1)',
        transition: 'all 0.3s ease',
        overflow: 'hidden',
        cursor: 'pointer',
        transform: isActive ? 'scale(1.02)' : 'scale(1)'
      }}
      bodyStyle={{ padding: '32px 24px' }}
      hoverable
      onClick={() => onCardClick(subscription.id)}
    >
      {/* Premium Badge */}
      {isPremium && (
        <div style={{
          position: 'absolute',
          top: '16px',
          right: '16px',
          background: 'linear-gradient(45deg, #ffb74d 0%, #ffcc80 100%)',
          color: 'white',
          padding: '4px 12px',
          borderRadius: '12px',
          fontSize: '12px',
          fontWeight: 'bold',
          display: 'flex',
          alignItems: 'center',
          gap: '4px'
        }}>
          <CrownOutlined />
          Phổ biến nhất
        </div>
      )}

      {/* Active Badge */}
      {isActive && (
        <div style={{
          position: 'absolute',
          top: '16px',
          left: '16px',
          background: 'linear-gradient(45deg, #52c41a 0%, #73d13d 100%)',
          color: 'white',
          padding: '4px 12px',
          borderRadius: '12px',
          fontSize: '12px',
          fontWeight: 'bold',
          display: 'flex',
          alignItems: 'center',
          gap: '4px'
        }}>
          <CheckOutlined />
          Đã chọn
        </div>
      )}

      {/* Plan Header */}
      <div style={{ textAlign: 'center', marginBottom: '24px' }}>
        <div style={{ 
          fontSize: '32px', 
          marginBottom: '8px',
          background: isPremium ? 'linear-gradient(45deg, #ffb74d 0%, #ffcc80 100%)' : 'linear-gradient(45deg, #52c41a 0%, #73d13d 100%)',
          borderRadius: '50%',
          width: '64px',
          height: '64px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          margin: '0 auto 16px auto'
        }}>
          {isPremium ? '⭐' : subscription.durationInMonths >= 12 ? '🏆' : '✓'}
        </div>
        
        <Title level={3} style={{ 
          margin: '0 0 8px 0',
          color: isPremium ? '#ffa726' : '#ffffff',
          fontWeight: 'bold'
        }}>
          {subscription.name}
        </Title>
        
        <Paragraph style={{ 
          color: '#b3b3b3', 
          margin: '0 0 16px 0',
          fontSize: '14px',
          height: '40px',
          overflow: 'hidden',
          textOverflow: 'ellipsis',
          display: '-webkit-box',
          WebkitLineClamp: 2,
          WebkitBoxOrient: 'vertical'
        }}>
          {subscription.description}
        </Paragraph>
        <div style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'center', gap: '4px' }}>
          <Title level={1} style={{ 
            margin: 0,
            color: isPremium ? '#ffa726' : '#ffffff',
            fontWeight: 'bold'
          }}>
            {formatPrice(subscription.price, subscription.currency)}
          </Title>
          <Text style={{ 
            color: '#b3b3b3',
            fontSize: '16px'
          }}>
            /{subscription.durationInMonths} tháng
          </Text>
        </div>
        
        {/* CTA Button */}
        <Button
          type={isPremium ? "primary" : "default"}
          size="large"
          block
          onClick={(e) => {
            e.stopPropagation();
            onSelectSubscription(subscription.id);
          }}
          style={{
            height: '48px',
            borderRadius: '12px',
            fontWeight: 'medium',
            fontSize: '16px',
            background: isPremium 
              ? 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)'
              : 'transparent',
            border: isPremium 
              ? 'none'
              : '2px solid rgba(255, 255, 255, 0.2)',
            color: '#ffffff',
            boxShadow: isPremium
              ? '0 4px 15px rgba(255, 122, 0, 0.3)'
              : 'none',
            margin: '24px 0 0 0',
            transition: 'all 0.3s ease',
            transform: isActive ? 'translateY(-2px)' : 'translateY(0)'
          }}
        >
          {isPremium ? 'Đăng ký ngay' : 'Đăng ký gói này'}
        </Button>
      </div>

      {/* Features List */}
      <List
        dataSource={generateFeatures()}
        renderItem={(feature) => (
          <List.Item style={{ 
            padding: '8px 0',
            border: 'none',
            color: '#ffffff'
          }}>
            <div style={{ display: 'flex', alignItems: 'flex-start', gap: '12px' }}>
              <CheckOutlined style={{ 
                color: isPremium ? '#ffb74d' : '#52c41a',
                marginTop: '2px',
                fontSize: '14px'
              }} />
              <Text style={{ 
                color: '#ffffff',
                fontSize: '14px',
                lineHeight: '1.4'
              }}>
                {feature}
              </Text>
            </div>
          </List.Item>
        )}
        style={{ marginBottom: '32px' }}
      />
    </Card>
  );
};

export default SubscriptionCard; 