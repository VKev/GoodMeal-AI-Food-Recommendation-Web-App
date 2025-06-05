import React from 'react';
import { Card, Button, Typography, Radio, Space, Divider } from 'antd';
import { CheckOutlined } from '@ant-design/icons';
import { PaymentFormProps } from './types';

const { Title, Text } = Typography;

const PaymentForm: React.FC<PaymentFormProps> = ({
    selectedPayment,
    setSelectedPayment,
    isProcessing,
    onPayment,
    currentPrice,
    period,
    paymentMethods
}) => {
    return (
        <Card style={{
            background: 'rgba(255, 255, 255, 0.05)',
            border: '1px solid rgba(255, 255, 255, 0.1)',
            borderRadius: '16px'
        }}>
            <Title level={3} style={{ color: '#ffffff', marginBottom: '24px' }}>
                Payment Method
            </Title>
            
            <Radio.Group 
                value={selectedPayment} 
                onChange={(e) => setSelectedPayment(e.target.value)}
                style={{ width: '100%' }}
            >
                <Space direction="vertical" size={16} style={{ width: '100%' }}>
                    {paymentMethods.map((method) => (
                        <Radio.Button
                            key={method.id}
                            value={method.id}
                            style={{
                                width: '100%',
                                height: 'auto',
                                padding: '16px',
                                background: selectedPayment === method.id 
                                    ? 'rgba(255, 122, 0, 0.1)' 
                                    : 'rgba(255, 255, 255, 0.02)',
                                border: selectedPayment === method.id 
                                    ? '2px solid #ff7a00' 
                                    : '1px solid rgba(255, 255, 255, 0.1)',
                                borderRadius: '12px',
                                color: '#ffffff'
                            }}
                        >
                            <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                                {method.icon}
                                <div>
                                    <div style={{ 
                                        fontWeight: 'medium', 
                                        marginBottom: '4px',
                                        color: '#ffffff'
                                    }}>
                                        {method.name}
                                    </div>
                                    <div style={{ 
                                        fontSize: '12px', 
                                        color: '#b3b3b3' 
                                    }}>
                                        {method.description}
                                    </div>
                                </div>
                            </div>
                        </Radio.Button>
                    ))}
                </Space>
            </Radio.Group>

            <Divider style={{ 
                borderColor: 'rgba(255, 255, 255, 0.1)',
                margin: '32px 0'
            }} />

            {/* Security Notice */}
            <div style={{
                background: 'rgba(82, 196, 26, 0.1)',
                border: '1px solid rgba(82, 196, 26, 0.3)',
                borderRadius: '8px',
                padding: '16px',
                marginBottom: '24px'
            }}>
                <div style={{ display: 'flex', alignItems: 'flex-start', gap: '12px' }}>
                    <CheckOutlined style={{ color: '#52c41a', marginTop: '2px' }} />
                    <div>
                        <Text style={{ 
                            color: '#ffffff', 
                            fontWeight: 'medium',
                            display: 'block',
                            marginBottom: '4px'
                        }}>
                            Secure Payment
                        </Text>
                        <Text style={{ color: '#b3b3b3', fontSize: '14px' }}>
                            Your payment information is encrypted and secure. We use industry-standard SSL encryption.
                        </Text>
                    </div>
                </div>
            </div>

            {/* Payment Button */}
            <Button
                type="primary"
                size="large"
                block
                loading={isProcessing}
                onClick={onPayment}
                disabled={!selectedPayment}
                style={{
                    height: '56px',
                    borderRadius: '12px',
                    background: !selectedPayment 
                        ? 'rgba(255, 255, 255, 0.1)'
                        : 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                    border: 'none',
                    fontSize: '18px',
                    fontWeight: 'bold',
                    boxShadow: selectedPayment ? '0 4px 15px rgba(255, 122, 0, 0.3)' : 'none',
                    opacity: !selectedPayment ? 0.6 : 1
                }}
            >
                {isProcessing 
                    ? 'Processing Payment...' 
                    : `Pay ${currentPrice}${period} with ${paymentMethods.find(p => p.id === selectedPayment)?.name || 'Selected Method'}`
                }
            </Button>

            <div style={{ 
                textAlign: 'center', 
                marginTop: '16px',
                color: '#b3b3b3',
                fontSize: '12px'
            }}>
                By proceeding, you agree to our Terms of Service and Privacy Policy.
                <br />
                You can cancel your subscription at any time.
            </div>
        </Card>
    );
};

export default PaymentForm;
