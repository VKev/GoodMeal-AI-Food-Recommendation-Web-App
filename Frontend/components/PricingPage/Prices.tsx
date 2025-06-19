import React, { useState } from 'react';
import { Row, Col, Typography, Switch, Card, message } from 'antd';
import { CheckCircleOutlined, QuestionCircleOutlined, CloseOutlined } from '@ant-design/icons';
import { useRouter } from 'next/navigation';
import PricingCard from './PricingCard';
import { pricingPlans } from './PricingData';

const { Title, Paragraph, Text } = Typography;

const Prices: React.FC = () => {
    const [isYearly, setIsYearly] = useState(false);
    const [activePlan, setActivePlan] = useState<string>('pro'); // Default to Pro plan
    const router = useRouter();    const handleSelectPlan = (planId: string) => {
        console.log('Selected plan:', planId);
          // Handle free plan - show message instead of redirecting
        if (planId === 'free') {
            message.info({
                content: '🆓 Bạn đang sử dụng gói Miễn phí! Hãy tận hưởng các tính năng cơ bản.',
                duration: 3,
                style: {
                    marginTop: '20vh',
                }
            });
            return;
        }
        
        // Navigate to payment page with plan details for paid plans
        const billingType = isYearly ? 'yearly' : 'monthly';
        router.push(`/payment?plan=${planId}&billing=${billingType}`);
    };

    const handleCardClick = (planId: string) => {
        setActivePlan(planId);
    };

    const handleClose = () => {
        router.push('/id');
    };    // FAQ data
    const faqs = [
        {
            question: 'Tôi có thể hủy đăng ký bất cứ lúc nào không?',
            answer: 'Có, bạn có thể hủy đăng ký bất cứ lúc nào mà không mất phí. Đăng ký của bạn sẽ tiếp tục hoạt động cho đến hết chu kỳ thanh toán hiện tại.'
        },
        {
            question: 'Bạn có hoàn tiền không?',
            answer: 'Chúng tôi cung cấp bảo đảm hoàn tiền 100% trong vòng 30 ngày đầu tiên nếu bạn không hài lòng với dịch vụ của chúng tôi.'
        },
        {
            question: 'Dữ liệu của tôi có an toàn không?',
            answer: 'Hoàn toàn an toàn! Chúng tôi sử dụng mã hóa SSL 256-bit và tuân thủ các tiêu chuẩn bảo mật quốc tế để bảo vệ dữ liệu của bạn.'
        },
        {
            question: 'Tôi có thể thay đổi gói đăng ký của mình không?',
            answer: 'Có, bạn có thể nâng cấp hoặc hạ cấp gói đăng ký bất cứ lúc nào. Thay đổi sẽ có hiệu lực ngay lập tức.'
        }
    ];return (
        <div style={{ 
            minHeight: '100vh',
            background: 'linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)',
            padding: '80px 24px 40px',
            position: 'relative'
        }}>
            {/* Close Button */}
            <div 
                onClick={handleClose}
                style={{
                    position: 'fixed',
                    top: '24px',
                    right: '24px',
                    width: '48px',
                    height: '48px',
                    background: 'rgba(255, 255, 255, 0.1)',
                    borderRadius: '50%',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    cursor: 'pointer',
                    border: '1px solid rgba(255, 255, 255, 0.2)',
                    transition: 'all 0.3s ease',
                    zIndex: 1000
                }}
                onMouseEnter={(e) => {
                    e.currentTarget.style.background = 'rgba(255, 122, 0, 0.2)';
                    e.currentTarget.style.borderColor = '#ff7a00';
                }}
                onMouseLeave={(e) => {
                    e.currentTarget.style.background = 'rgba(255, 255, 255, 0.1)';
                    e.currentTarget.style.borderColor = 'rgba(255, 255, 255, 0.2)';
                }}
            >
                <CloseOutlined style={{ 
                    color: '#ffffff', 
                    fontSize: '20px'
                }} />
            </div>

            {/* Header Section */}
            <div style={{ 
                textAlign: 'center', 
                maxWidth: '800px', 
                margin: '0 auto 64px auto' 
            }}>                <Title level={1} style={{ 
                    margin: '0 0 16px 0',
                    background: 'linear-gradient(45deg, #ffe0b3 0%, #ffd699 100%)',
                    backgroundClip: 'text',
                    WebkitBackgroundClip: 'text',
                    color: 'transparent',
                    fontSize: '48px',
                    fontWeight: 'bold'
                }}>
                    Nâng cấp gói của bạn
                </Title>
                
                <Paragraph style={{ 
                    color: '#b3b3b3', 
                    fontSize: '18px',
                    lineHeight: '1.6',
                    margin: '0 0 32px 0'
                }}>
                    Từ người dùng cá nhân đến doanh nghiệp lớn, chúng tôi có các 
                    giải pháp gợi ý món ăn AI phù hợp với mọi nhu cầu
                </Paragraph>

                {/* Billing Toggle */}
                <div style={{ 
                    display: 'flex', 
                    alignItems: 'center', 
                    justifyContent: 'center',
                    gap: '16px',
                    background: 'rgba(255, 255, 255, 0.05)',
                    padding: '16px 24px',
                    borderRadius: '12px',
                    border: '1px solid rgba(255, 255, 255, 0.1)'
                }}>                    <Text style={{ 
                        color: isYearly ? '#b3b3b3' : '#ffffff',
                        fontWeight: isYearly ? 'normal' : 'medium'
                    }}>
                        Hàng tháng
                    </Text>
                    <Switch
                        checked={isYearly}
                        onChange={setIsYearly}
                        style={{
                            background: isYearly ? '#ff7a00' : 'rgba(255, 255, 255, 0.3)'
                        }}
                    />
                    <Text style={{ 
                        color: isYearly ? '#ffffff' : '#b3b3b3',
                        fontWeight: isYearly ? 'medium' : 'normal'
                    }}>
                        Hàng năm
                    </Text>
                    <div style={{
                        background: 'linear-gradient(45deg, #ff9800 0%, #ffb300 100%)',
                        color: 'white',
                        padding: '4px 8px',
                        borderRadius: '6px',
                        fontSize: '13px',
                        fontWeight: 900,
                        letterSpacing: '0.5px',
                        boxShadow: '0 2px 8px rgba(255, 152, 0, 0.18)'
                    }}>
                        Tiết kiệm 20%
                    </div>
                </div>
            </div>

            {/* Pricing Cards */}
            <div style={{ maxWidth: '1200px', margin: '0 auto 80px auto' }}>                <Row gutter={[24, 24]} justify="center">
                    {pricingPlans.map((plan) => (
                        <Col key={plan.id} xs={24} md={8}>
                            <PricingCard 
                                plan={plan} 
                                isYearly={isYearly}
                                isActive={activePlan === plan.id}
                                onSelectPlan={handleSelectPlan}
                                onCardClick={handleCardClick}
                            />
                        </Col>
                    ))}
                </Row>
            </div>

            {/* Feature Comparison */}
            <div style={{ maxWidth: '800px', margin: '0 auto 80px auto' }}>                <Title level={2} style={{ 
                    textAlign: 'center',
                    color: '#ffffff',
                    marginBottom: '40px'
                }}>
                    So sánh tính năng
                </Title>
                
                <Card style={{
                    background: 'rgba(255, 255, 255, 0.05)',
                    border: '1px solid rgba(255, 255, 255, 0.1)',
                    borderRadius: '16px'
                }}>
                    <div style={{ display: 'flex', justifyContent: 'center', gap: '32px', flexWrap: 'wrap' }}>                        <div style={{ textAlign: 'center' }}>
                            <CheckCircleOutlined style={{ color: '#52c41a', fontSize: '24px', marginBottom: '8px' }} />
                            <div style={{ color: '#ffffff', fontWeight: 'medium' }}>Không cam kết</div>
                            <div style={{ color: '#b3b3b3', fontSize: '14px' }}>Hủy bất cứ lúc nào</div>
                        </div>
                        <div style={{ textAlign: 'center' }}>
                            <CheckCircleOutlined style={{ color: '#52c41a', fontSize: '24px', marginBottom: '8px' }} />
                            <div style={{ color: '#ffffff', fontWeight: 'medium' }}>Hỗ trợ 24/7</div>
                            <div style={{ color: '#b3b3b3', fontSize: '14px' }}>Luôn sẵn sàng giúp đỡ</div>
                        </div>
                        <div style={{ textAlign: 'center' }}>
                            <CheckCircleOutlined style={{ color: '#52c41a', fontSize: '24px', marginBottom: '8px' }} />
                            <div style={{ color: '#ffffff', fontWeight: 'medium' }}>Bảo mật cao</div>
                            <div style={{ color: '#b3b3b3', fontSize: '14px' }}>Mã hóa SSL 256-bit</div>
                        </div>
                    </div>
                </Card>
            </div>

            {/* FAQ Section */}
            <div style={{ maxWidth: '800px', margin: '0 auto' }}>                <Title level={2} style={{ 
                    textAlign: 'center',
                    color: '#ffffff',
                    marginBottom: '40px'
                }}>
                    Câu hỏi thường gặp
                </Title>
                
                <Row gutter={[0, 16]}>
                    {faqs.map((faq, index) => (
                        <Col span={24} key={index}>
                            <Card style={{
                                background: 'rgba(255, 255, 255, 0.05)',
                                border: '1px solid rgba(255, 255, 255, 0.1)',
                                borderRadius: '12px'
                            }}>
                                <div style={{ display: 'flex', alignItems: 'flex-start', gap: '12px' }}>
                                    <QuestionCircleOutlined style={{ 
                                        color: '#ff7a00', 
                                        fontSize: '18px',
                                        marginTop: '2px'
                                    }} />
                                    <div>
                                        <Text style={{ 
                                            color: '#ffffff', 
                                            fontWeight: 'medium',
                                            fontSize: '16px',
                                            display: 'block',
                                            marginBottom: '8px'
                                        }}>
                                            {faq.question}
                                        </Text>
                                        <Text style={{ 
                                            color: '#b3b3b3',
                                            fontSize: '14px',
                                            lineHeight: '1.5'
                                        }}>
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