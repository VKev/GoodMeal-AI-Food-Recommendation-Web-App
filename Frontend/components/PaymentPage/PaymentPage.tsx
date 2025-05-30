import React, { useEffect, useState, Suspense } from 'react';
import { Button, Typography, Row, Col, Alert, Spin, message } from 'antd';
import { CreditCardOutlined, PayCircleOutlined, ArrowLeftOutlined, LoadingOutlined } from '@ant-design/icons';
import { useRouter, useSearchParams } from 'next/navigation';
import { pricingPlans } from '../PricingPage/PricingData';
import OrderSummary from './OrderSummary';
import PaymentForm from './PaymentForm';
import { PaymentMethod } from './types';

const { Title, Paragraph } = Typography;

const PaymentPageContent: React.FC = () => {
    const router = useRouter();
    const searchParams = useSearchParams();
    const [selectedPayment, setSelectedPayment] = useState<string>('visa');
    const [isProcessing, setIsProcessing] = useState(false);
    const [isLoading, setIsLoading] = useState(true);

    // Get plan details from URL parameters
    const planId = searchParams.get('plan') || 'pro';
    const billingType = searchParams.get('billing') || 'monthly';
    
    const selectedPlan = pricingPlans.find(plan => plan.id === planId);
    const isYearly = billingType === 'yearly';
    const currentPrice = selectedPlan ? (isYearly ? selectedPlan.yearlyPrice : selectedPlan.monthlyPrice) : '$19';
    const period = isYearly ? '/year' : '/month';
    const savings = isYearly && planId !== 'free' ? '20%' : '';

    useEffect(() => {
        // Simulate loading delay for better UX
        const timer = setTimeout(() => {
            setIsLoading(false);
        }, 500);
        return () => clearTimeout(timer);
    }, []);

    const paymentMethods: PaymentMethod[] = [
        {
            id: 'visa',
            name: 'Credit/Debit Card',
            icon: <CreditCardOutlined style={{ fontSize: '24px', color: '#1890ff' }} />,
            description: 'Visa, Mastercard, American Express, etc.'
        },
        {
            id: 'paypal',
            name: 'PayPal',
            icon: <PayCircleOutlined style={{ fontSize: '24px', color: '#0070ba' }} />,
            description: 'Pay securely with your PayPal account'
        },
        {
            id: 'momo',
            name: 'MoMo E-Wallet',
            icon: <div style={{ 
                width: '24px', 
                height: '24px', 
                background: 'linear-gradient(45deg, #d82d8b 0%, #ff6b9d 100%)', 
                borderRadius: '50%',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: 'white',
                fontWeight: 'bold',
                fontSize: '12px'
            }}>M</div>,
            description: 'Ví điện tử MoMo - Thanh toán nhanh chóng'
        }
    ];const handlePayment = async () => {
        if (!selectedPayment) {
            message.error('Please select a payment method');
            return;
        }

        setIsProcessing(true);
        
        try {
            // Show processing message
            message.loading('Processing your payment...', 0);
            
            // Simulate payment processing based on payment method
            await new Promise(resolve => setTimeout(resolve, 3000));
            
            // Clear loading message
            message.destroy();
            
            // In a real app, you would call your payment API here
            console.log('Payment processed:', {
                plan: selectedPlan?.name,
                amount: currentPrice,
                method: selectedPayment,
                billing: billingType
            });
            
            // Show success message
            message.success('Payment successful! Redirecting...', 2);
            
            // Redirect to success page or back to app
            setTimeout(() => {
                router.push(`/id?payment=success&plan=${planId}`);
            }, 2000);
        } catch (error) {
            console.error('Payment failed:', error);
            message.destroy();
            message.error('Payment failed. Please try again.');
        } finally {
            setIsProcessing(false);
        }
    };

    const handleBack = () => {
        router.back();
    };

    if (isLoading) {
        return (
            <div style={{ 
                minHeight: '100vh',
                background: 'linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
            }}>
                <Spin 
                    indicator={<LoadingOutlined style={{ fontSize: 48, color: '#ff7a00' }} spin />}
                    size="large"
                />
            </div>
        );
    }

    if (!selectedPlan) {
        return (
            <div style={{ 
                minHeight: '100vh',
                background: 'linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                padding: '24px'
            }}>
                <Alert
                    message="Plan Not Found"
                    description="The selected plan could not be found. Please go back and select a valid plan."
                    type="error"
                    showIcon
                    action={
                        <Button onClick={handleBack}>
                            Go Back
                        </Button>
                    }
                />
            </div>
        );
    }

    return (
        <div style={{ 
            minHeight: '100vh',
            background: 'linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)',
            padding: '40px 24px'
        }}>
            <div style={{ maxWidth: '1000px', margin: '0 auto' }}>
                {/* Header */}
                <div style={{ marginBottom: '40px' }}>
                    <Button 
                        icon={<ArrowLeftOutlined />}
                        onClick={handleBack}
                        style={{
                            background: 'transparent',
                            border: '1px solid rgba(255, 255, 255, 0.3)',
                            color: '#ffffff',
                            marginBottom: '24px'
                        }}
                    >
                        Back to Pricing
                    </Button>
                      <Title level={1} style={{ 
                        margin: '0 0 8px 0',
                        background: 'linear-gradient(45deg, #ff9500 0%, #ff7a00 100%)',
                        backgroundClip: 'text',
                        WebkitBackgroundClip: 'text',
                        color: 'transparent'
                    }}>
                        Complete Your Purchase
                    </Title>
                    <Paragraph style={{ 
                        color: '#b3b3b3',
                        fontSize: '16px',
                        margin: 0                    }}>
                        You&apos;re just one step away from unlocking premium features
                    </Paragraph>
                </div>                <Row gutter={[32, 32]}>
                    {/* Order Summary */}
                    <Col xs={24} lg={10}>
                        <OrderSummary
                            selectedPlan={selectedPlan}
                            isYearly={isYearly}
                            currentPrice={currentPrice}
                            period={period}
                            savings={savings}
                        />
                    </Col>

                    {/* Payment Form */}
                    <Col xs={24} lg={14}>
                        <PaymentForm
                            selectedPayment={selectedPayment}
                            setSelectedPayment={setSelectedPayment}
                            isProcessing={isProcessing}
                            onPayment={handlePayment}
                            currentPrice={currentPrice}
                            period={period}
                            paymentMethods={paymentMethods}
                        />
                    </Col>
                </Row>
            </div>
        </div>    );
};

const PaymentPage: React.FC = () => {
    return (
        <Suspense fallback={
            <div style={{ 
                minHeight: '100vh',
                background: 'linear-gradient(180deg, #0f0f12 0%, #1a1a1d 100%)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
            }}>
                <Spin 
                    indicator={<LoadingOutlined style={{ fontSize: 48, color: '#ff7a00' }} spin />}
                    size="large"
                />
            </div>
        }>
            <PaymentPageContent />
        </Suspense>
    );
};

export default PaymentPage;
