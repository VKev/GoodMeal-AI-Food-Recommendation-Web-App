import React from 'react';
import { Card, Typography, Space, Row, Col } from 'antd';
import { CheckOutlined } from '@ant-design/icons';
import { OrderSummaryProps } from './types';

const { Title, Text } = Typography;

const OrderSummary: React.FC<OrderSummaryProps> = ({
    selectedPlan,
    isYearly,
    currentPrice,
    period,
    savings
}) => {
    return (
        <Card style={{
            background: 'rgba(255, 255, 255, 0.05)',
            border: '1px solid rgba(255, 255, 255, 0.1)',
            borderRadius: '16px',
            height: 'fit-content'
        }}>
            <Title level={3} style={{ color: '#ffffff', marginBottom: '24px' }}>
                Order Summary
            </Title>
            
            {/* Plan Details */}
            <div style={{
                background: selectedPlan.popular 
                    ? 'linear-gradient(135deg, rgba(255, 122, 0, 0.1) 0%, rgba(255, 149, 0, 0.05) 100%)'
                    : 'rgba(255, 255, 255, 0.02)',
                border: selectedPlan.popular 
                    ? '1px solid rgba(255, 122, 0, 0.3)'
                    : '1px solid rgba(255, 255, 255, 0.1)',
                borderRadius: '12px',
                padding: '20px',
                marginBottom: '24px'
            }}>
                <div style={{ display: 'flex', alignItems: 'center', marginBottom: '12px' }}>
                    <div style={{
                        fontSize: '24px',
                        marginRight: '12px'
                    }}>
                        {selectedPlan.icon}
                    </div>
                    <div>
                        <Text style={{ 
                            color: '#ffffff', 
                            fontSize: '18px', 
                            fontWeight: 'bold',
                            display: 'block'
                        }}>
                            {selectedPlan.name} Plan
                        </Text>
                        <Text style={{ color: '#b3b3b3', fontSize: '14px' }}>
                            {selectedPlan.description}
                        </Text>
                    </div>
                </div>
                
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <Text style={{ color: '#b3b3b3' }}>
                        {isYearly ? 'Annual' : 'Monthly'} billing
                    </Text>
                    <div style={{ textAlign: 'right' }}>
                        <Text style={{ 
                            color: '#ffffff', 
                            fontSize: '24px', 
                            fontWeight: 'bold'
                        }}>
                            {currentPrice}
                        </Text>
                        <Text style={{ color: '#b3b3b3', marginLeft: '4px' }}>
                            {period}
                        </Text>
                        {savings && (
                            <div style={{ 
                                color: '#52c41a', 
                                fontSize: '12px',
                                marginTop: '4px'
                            }}>
                                Save {savings}
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Features Preview */}
            <div>
                <Text style={{ 
                    color: '#ffffff', 
                    fontWeight: 'medium',
                    display: 'block',
                    marginBottom: '16px'
                }}>
                    What's included:
                </Text>
                <Space direction="vertical" size={8} style={{ width: '100%' }}>
                    {selectedPlan.features.slice(0, 5).map((feature: string, index: number) => (
                        <div key={index} style={{ display: 'flex', alignItems: 'flex-start', gap: '8px' }}>
                            <CheckOutlined style={{ 
                                color: '#52c41a', 
                                fontSize: '14px',
                                marginTop: '2px'
                            }} />
                            <Text style={{ 
                                color: '#b3b3b3', 
                                fontSize: '14px',
                                lineHeight: '1.4'
                            }}>
                                {feature}
                            </Text>
                        </div>
                    ))}
                    {selectedPlan.features.length > 5 && (
                        <Text style={{ 
                            color: '#ff7a00', 
                            fontSize: '14px',
                            fontStyle: 'italic'
                        }}>
                            +{selectedPlan.features.length - 5} more features
                        </Text>
                    )}
                </Space>
            </div>
        </Card>
    );
};

export default OrderSummary;
