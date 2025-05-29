import React from 'react';
import { Card, Button, Typography, List } from 'antd';
import { CheckOutlined, CrownOutlined } from '@ant-design/icons';
import { PricingPlan } from './PricingData';

const { Title, Text, Paragraph } = Typography;

interface PricingCardProps {
    plan: PricingPlan;
    isYearly: boolean;
    isActive: boolean;
    onSelectPlan: (planId: string) => void;
    onCardClick: (planId: string) => void;
}

const PricingCard: React.FC<PricingCardProps> = ({ plan, isYearly, isActive, onSelectPlan, onCardClick }) => {
    const currentPrice = isYearly ? plan.yearlyPrice : plan.monthlyPrice;
    const period = isYearly ? '/year' : '/month';
    const savings = isYearly && plan.id !== 'free' ? '(Save 20%)' : '';

    return (
        <Card
            className={`pricing-card ${plan.popular ? 'popular' : ''} ${isActive ? 'active' : ''}`}
            style={{
                height: '100%',
                position: 'relative',
                background: isActive 
                    ? 'linear-gradient(135deg, rgba(255, 183, 77, 0.18) 0%, rgba(255, 204, 128, 0.10) 100%)'
                    : plan.popular 
                        ? 'linear-gradient(135deg, rgba(255, 183, 77, 0.10) 0%, rgba(255, 204, 128, 0.06) 100%)'
                        : 'linear-gradient(135deg, rgba(255, 255, 255, 0.05) 0%, rgba(255, 255, 255, 0.02) 100%)',
                border: isActive
                    ? '3px solid #ffb74d'
                    : plan.popular 
                        ? '2px solid #ffb74d'
                        : '1px solid rgba(255, 255, 255, 0.1)',
                borderRadius: '16px',
                boxShadow: isActive
                    ? '0 12px 40px rgba(255, 183, 77, 0.32)'
                    : plan.popular
                        ? '0 8px 32px rgba(255, 183, 77, 0.22)'
                        : '0 4px 16px rgba(0, 0, 0, 0.1)',
                transition: 'all 0.3s ease',
                overflow: 'hidden',
                cursor: 'pointer',
                transform: isActive ? 'scale(1.02)' : 'scale(1)'
            }}
            bodyStyle={{ padding: '32px 24px' }}
            hoverable
            onClick={() => onCardClick(plan.id)}
        >            {/* Popular Badge */}
            {plan.popular && (
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
                    Most Popular
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
                    Selected
                </div>
            )}

            {/* Plan Header */}
            <div style={{ textAlign: 'center', marginBottom: '24px' }}>
                <div style={{ 
                    fontSize: '32px', 
                    marginBottom: '8px',
                    background: plan.popular ? 'linear-gradient(45deg, #ffb74d 0%, #ffcc80 100%)' : 'linear-gradient(45deg, #52c41a 0%, #73d13d 100%)',
                    borderRadius: '50%',
                    width: '64px',
                    height: '64px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    margin: '0 auto 16px auto'
                }}>
                    {plan.icon}
                </div>
                
                <Title level={3} style={{ 
                    margin: '0 0 8px 0',
                    color: plan.popular ? '#ffa726' : '#ffffff',
                    fontWeight: 'bold'
                }}>
                    {plan.name}
                </Title>
                
                <Paragraph style={{ 
                    color: '#b3b3b3', 
                    margin: '0 0 16px 0',
                    fontSize: '14px'
                }}>
                    {plan.description}
                </Paragraph>                <div style={{ display: 'flex', alignItems: 'baseline', justifyContent: 'center', gap: '4px' }}>
                    <Title level={1} style={{ 
                        margin: 0,
                        color: plan.popular ? '#ffa726' : '#ffffff',
                        fontWeight: 'bold'
                    }}>
                        {currentPrice}
                    </Title>
                    <Text style={{ 
                        color: '#b3b3b3',
                        fontSize: '16px'
                    }}>
                        {period}
                    </Text>
                    {savings && (
                        <Text style={{ 
                            color: '#52c41a',
                            fontSize: '12px',
                            marginLeft: '8px'
                        }}>
                            {savings}
                        </Text>
                    )}
                </div>
            </div>

            {/* Features List */}
            <List
                dataSource={plan.features}
                renderItem={(feature) => (
                    <List.Item style={{ 
                        padding: '8px 0',
                        border: 'none',
                        color: '#ffffff'
                    }}>
                        <div style={{ display: 'flex', alignItems: 'flex-start', gap: '12px' }}>
                            <CheckOutlined style={{ 
                                color: plan.popular ? '#ffb74d' : '#52c41a',
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

            {/* CTA Button */}
            <Button
                type={plan.buttonType}
                size="large"
                block
                onClick={() => onSelectPlan(plan.id)}
                style={{
                    height: '48px',
                    borderRadius: '12px',
                    fontWeight: 'medium',
                    fontSize: '16px',
                    background: plan.buttonType === 'primary' 
                        ? 'linear-gradient(45deg, #ffb74d 0%, #ffcc80 100%)'
                        : 'transparent',
                    border: plan.buttonType === 'primary' 
                        ? 'none'
                        : '2px solid rgba(255, 255, 255, 0.2)',
                    color: plan.buttonType === 'primary' ? '#ffffff' : '#ffffff',
                    boxShadow: plan.buttonType === 'primary'
                        ? '0 4px 15px rgba(255, 183, 77, 0.32)'
                        : 'none'
                }}
            >
                {plan.buttonText}
            </Button>
        </Card>
    );
};

export default PricingCard;
