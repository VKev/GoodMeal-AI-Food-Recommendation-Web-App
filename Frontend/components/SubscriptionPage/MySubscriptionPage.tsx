"use client"
import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import {
    Layout,
    Typography,
   
    Row,
    Col,
    Button,
    Tag,
    Modal,
    message,
    Tooltip
} from 'antd';
import {
    ArrowLeftOutlined,
    CrownOutlined,
    CheckCircleOutlined,
    HistoryOutlined,
    WarningOutlined,
    PlayCircleOutlined,
    StarFilled,
    CalendarOutlined,
    CreditCardOutlined,
    SettingOutlined,
    DownloadOutlined,
    QuestionCircleOutlined
} from '@ant-design/icons';

const {  } = Layout;
const {  } = Typography;

interface PaymentHistory {
    id: string;
    date: string;
    plan: string;
    amount: number;
    status: 'success' | 'failed' | 'pending';
    method: string;
}

interface Subscription {
    id: string;
    plan: string;
    status: 'active' | 'expired' | 'cancelled';
    startDate: string;
    endDate: string;
    price: number;
    features: string[];
    autoRenew: boolean;
    daysRemaining: number;
}

const MySubscriptionPage: React.FC = () => {
    const router = useRouter();
    const [cancelModalVisible, setCancelModalVisible] = useState(false);

    // Mock data
    const currentSubscription: Subscription = {
        id: 'SUB001',
        plan: 'Pro Plan',
        status: 'active',
        startDate: '2024-12-01',
        endDate: '2025-01-01',
        price: 299000,
        features: [
            'Gợi ý món ăn không giới hạn',
            'Tìm kiếm nhà hàng cao cấp',
            'Hỗ trợ AI cá nhân hóa',
            'Ưu đãi từ đối tác',
            'Hỗ trợ khách hàng 24/7'
        ],
        autoRenew: true,
        daysRemaining: 16
    };

    const paymentHistory: PaymentHistory[] = [
        {
            id: 'PAY001',
            date: '2024-12-01',
            plan: 'Pro Plan',
            amount: 299000,
            status: 'success',
            method: 'Visa **** 1234'
        },
        {
            id: 'PAY002',
            date: '2024-11-01',
            plan: 'Pro Plan',
            amount: 299000,
            status: 'success',
            method: 'Visa **** 1234'
        },
        {
            id: 'PAY003',
            date: '2024-10-01',
            plan: 'Basic Plan',
            amount: 99000,
            status: 'success',
            method: 'MasterCard **** 5678'
        }
    ];

    const handleBackClick = () => {
        router.back();
    };

    const handleCancelSubscription = () => {
        setCancelModalVisible(true);
    };

    const confirmCancelSubscription = () => {
        message.success('Gói đăng ký đã được hủy thành công');
        setCancelModalVisible(false);
    };



    const getStatusText = (status: string) => {
        switch (status) {
            case 'active': return 'Đang hoạt động';
            case 'expired': return 'Hết hạn';
            case 'cancelled': return 'Đã hủy';
            default: return 'Không xác định';
        }
    };

    

    const progressPercent = Math.round(((31 - currentSubscription.daysRemaining) / 31) * 100);    return (
        <div style={{
            minHeight: '100vh',
            background: '#000000',
            fontFamily: "'Netflix Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif"
        }}>
            {/* Netflix-style Header */}
            <div style={{
                position: 'fixed',
                top: 0,
                left: 0,
                right: 0,
                zIndex: 1000,
                background: 'linear-gradient(180deg, rgba(0,0,0,0.9) 0%, rgba(0,0,0,0.7) 70%, transparent 100%)',
                padding: '16px 0',
                backdropFilter: 'blur(10px)'
            }}>
                <div style={{
                    maxWidth: '1400px',
                    margin: '0 auto',
                    padding: '0 24px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'space-between'
                }}>
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <Button
                            type="text"
                            icon={<ArrowLeftOutlined />}
                            onClick={handleBackClick}
                            style={{
                                color: '#ff8c42',
                                fontSize: '18px',
                                marginRight: '20px',
                                border: 'none',
                                background: 'none'
                            }}
                        />
                        <h1 style={{
                            color: '#ffffff',
                            fontSize: '28px',
                            fontWeight: '700',
                            margin: 0,
                            letterSpacing: '0.5px'
                        }}>
                            Gói đăng ký của bạn
                        </h1>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                        <Tooltip title="Cài đặt tài khoản">
                            <Button
                                type="text"
                                icon={<SettingOutlined />}
                                style={{
                                    color: '#ffffff',
                                    fontSize: '18px'
                                }}
                            />
                        </Tooltip>
                        <Tooltip title="Hỗ trợ">
                            <Button
                                type="text"
                                icon={<QuestionCircleOutlined />}
                                style={{
                                    color: '#ffffff',
                                    fontSize: '18px'
                                }}
                            />
                        </Tooltip>
                    </div>
                </div>
            </div>

            {/* Main Content */}
            <div style={{
                paddingTop: '100px',
                padding: '100px 24px 40px',
                maxWidth: '1400px',
                margin: '0 auto'
            }}>
                {/* Hero Section - Netflix Style */}
                <div style={{
                    position: 'relative',
                    background: 'linear-gradient(135deg, #1a0a00 0%, #000000 50%, #0a0a0a 100%)',
                    borderRadius: '12px',
                    padding: '48px',
                    marginBottom: '40px',
                    overflow: 'hidden',
                    border: '1px solid rgba(255, 140, 66, 0.2)'
                }}>
                    {/* Background Pattern */}
                    <div style={{
                        position: 'absolute',
                        top: 0,
                        right: 0,
                        width: '60%',
                        height: '100%',
                        background: `radial-gradient(ellipse at top right, rgba(255, 140, 66, 0.1) 0%, transparent 70%)`,
                        pointerEvents: 'none'
                    }} />
                    
                    <Row gutter={[48, 32]} align="middle">
                        <Col xs={24} lg={8}>
                            <div style={{ textAlign: 'center' }}>
                                <div style={{
                                    width: '120px',
                                    height: '120px',
                                    background: 'linear-gradient(135deg, #ff8c42 0%, #ff6b1a 100%)',
                                    borderRadius: '50%',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    margin: '0 auto 24px',
                                    boxShadow: '0 8px 32px rgba(255, 140, 66, 0.3)',
                                    position: 'relative'
                                }}>
                                    <CrownOutlined style={{ 
                                        fontSize: '48px', 
                                        color: '#ffffff'
                                    }} />
                                    <div style={{
                                        position: 'absolute',
                                        top: '-8px',
                                        right: '-8px',
                                        background: '#ff4757',
                                        borderRadius: '50%',
                                        width: '32px',
                                        height: '32px',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center'
                                    }}>
                                        <StarFilled style={{ color: '#ffffff', fontSize: '16px' }} />
                                    </div>
                                </div>
                                
                                <h2 style={{
                                    color: '#ffffff',
                                    fontSize: '32px',
                                    fontWeight: '700',
                                    margin: '0 0 12px',
                                    textShadow: '0 2px 8px rgba(0,0,0,0.5)'
                                }}>
                                    {currentSubscription.plan}
                                </h2>
                                
                                <Tag
                                    style={{
                                        background: currentSubscription.status === 'active' 
                                            ? 'linear-gradient(90deg, #2ecc71 0%, #27ae60 100%)'
                                            : '#e74c3c',
                                        border: 'none',
                                        color: '#ffffff',
                                        padding: '6px 16px',
                                        borderRadius: '20px',
                                        fontSize: '14px',
                                        fontWeight: '600',
                                        textTransform: 'uppercase',
                                        letterSpacing: '0.5px'
                                    }}
                                >
                                    {getStatusText(currentSubscription.status)}
                                </Tag>
                            </div>
                        </Col>
                        
                        <Col xs={24} lg={8}>
                            <div style={{ textAlign: 'center' }}>
                                
                                
                                {/* Usage Progress */}
                                <div style={{ marginTop: '32px' }}>
                                    <div style={{
                                        display: 'flex',
                                        justifyContent: 'space-between',
                                        alignItems: 'center',
                                        marginBottom: '12px'
                                    }}>
                                        <span style={{ color: '#ffffff', fontSize: '16px', fontWeight: '600' }}>
                                            Thời gian sử dụng
                                        </span>
                                        <span style={{ color: '#ff8c42', fontSize: '16px', fontWeight: '600' }}>
                                            {currentSubscription.daysRemaining} ngày còn lại
                                        </span>
                                    </div>
                                    <div style={{
                                        background: 'rgba(255, 255, 255, 0.1)',
                                        borderRadius: '12px',
                                        height: '8px',
                                        overflow: 'hidden'
                                    }}>
                                        <div style={{
                                            background: 'linear-gradient(90deg, #ff8c42 0%, #ff6b1a 100%)',
                                            height: '100%',
                                            width: `${progressPercent}%`,
                                            borderRadius: '12px',
                                            boxShadow: '0 0 12px rgba(255, 140, 66, 0.5)',
                                            transition: 'all 0.3s ease'
                                        }} />
                                    </div>
                                    <div style={{
                                        display: 'flex',
                                        justifyContent: 'space-between',
                                        marginTop: '8px'
                                    }}>
                                        <span style={{ color: 'rgba(255, 255, 255, 0.5)', fontSize: '14px' }}>
                                            {new Date(currentSubscription.startDate).toLocaleDateString('vi-VN')}
                                        </span>
                                        <span style={{ color: 'rgba(255, 255, 255, 0.5)', fontSize: '14px' }}>
                                            {new Date(currentSubscription.endDate).toLocaleDateString('vi-VN')}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </Col>
                        
                        <Col xs={24} lg={8}>
                            <div>
                                <h3 style={{
                                    color: '#ffffff',
                                    fontSize: '20px',
                                    fontWeight: '600',
                                    marginBottom: '20px'
                                }}>
                                    Premium Features
                                </h3>
                                <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                                    {currentSubscription.features.slice(0, 4).map((feature, index) => (
                                        <div key={index} style={{
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '12px'
                                        }}>
                                            <CheckCircleOutlined style={{
                                                color: '#ff8c42',
                                                fontSize: '18px'
                                            }} />
                                            <span style={{
                                                color: 'rgba(255, 255, 255, 0.9)',
                                                fontSize: '16px',
                                                fontWeight: '500'
                                            }}>
                                                {feature}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </Col>
                    </Row>
                    
                    {/* Action Buttons */}
                    <div style={{
                        marginTop: '40px',
                        display: 'flex',
                        gap: '16px',
                        flexWrap: 'wrap',
                        justifyContent: 'center'
                    }}>
                        <Button
                            type="primary"
                            size="large"
                            icon={<PlayCircleOutlined />}
                            style={{
                                background: 'linear-gradient(135deg, #ff8c42 0%, #ff6b1a 100%)',
                                border: 'none',
                                borderRadius: '8px',
                                height: '48px',
                                fontSize: '16px',
                                fontWeight: '600',
                                padding: '0 32px',
                                boxShadow: '0 4px 16px rgba(255, 140, 66, 0.3)',
                                transition: 'all 0.3s ease'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.transform = 'translateY(-2px)';
                                e.currentTarget.style.boxShadow = '0 8px 24px rgba(255, 140, 66, 0.4)';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.transform = 'translateY(0)';
                                e.currentTarget.style.boxShadow = '0 4px 16px rgba(255, 140, 66, 0.3)';
                            }}
                        >
                            Gia hạn ngay
                        </Button>
                        
                        <Button
                            size="large"
                            icon={<CrownOutlined />}
                            style={{
                                background: 'transparent',
                                border: '2px solid #ff8c42',
                                color: '#ff8c42',
                                borderRadius: '8px',
                                height: '48px',
                                fontSize: '16px',
                                fontWeight: '600',
                                padding: '0 32px'
                            }}
                        >
                            Nâng cấp gói
                        </Button>
                        
                        <Button
                            size="large"
                            icon={<SettingOutlined />}
                            style={{
                                background: 'rgba(255, 255, 255, 0.1)',
                                border: '1px solid rgba(255, 255, 255, 0.2)',
                                color: '#ffffff',
                                borderRadius: '8px',
                                height: '48px',
                                fontSize: '16px',
                                fontWeight: '600',
                                padding: '0 32px'
                            }}
                        >
                            Quản lý
                        </Button>
                    </div>
                </div>

                {/* Stats Cards Row */}
                <Row gutter={[24, 24]} style={{ marginBottom: '40px' }}>
                    <Col xs={24} sm={8}>
                        <div style={{
                            background: 'linear-gradient(135deg, rgba(255, 140, 66, 0.1) 0%, rgba(0, 0, 0, 0.3) 100%)',
                            border: '1px solid rgba(255, 140, 66, 0.2)',
                            borderRadius: '12px',
                            padding: '24px',
                            textAlign: 'center'
                        }}>
                            <CalendarOutlined style={{ 
                                fontSize: '32px', 
                                color: '#ff8c42',
                                marginBottom: '12px'
                            }} />
                            <div style={{ color: '#ffffff', fontSize: '24px', fontWeight: '700' }}>
                                {currentSubscription.daysRemaining}
                            </div>
                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                Ngày còn lại
                            </div>
                        </div>
                    </Col>
                    
                    <Col xs={24} sm={8}>
                        <div style={{
                            background: 'linear-gradient(135deg, rgba(46, 204, 113, 0.1) 0%, rgba(0, 0, 0, 0.3) 100%)',
                            border: '1px solid rgba(46, 204, 113, 0.2)',
                            borderRadius: '12px',
                            padding: '24px',
                            textAlign: 'center'
                        }}>
                            <CreditCardOutlined style={{ 
                                fontSize: '32px', 
                                color: '#2ecc71',
                                marginBottom: '12px'
                            }} />
                            <div style={{ color: '#ffffff', fontSize: '24px', fontWeight: '700' }}>
                                {paymentHistory.filter(p => p.status === 'success').length}
                            </div>
                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                Thanh toán thành công
                            </div>
                        </div>
                    </Col>
                    
                    <Col xs={24} sm={8}>
                        <div style={{
                            background: 'linear-gradient(135deg, rgba(231, 76, 60, 0.1) 0%, rgba(0, 0, 0, 0.3) 100%)',
                            border: '1px solid rgba(231, 76, 60, 0.2)',
                            borderRadius: '12px',
                            padding: '24px',
                            textAlign: 'center'
                        }}>
                            <DownloadOutlined style={{ 
                                fontSize: '32px', 
                                color: '#e74c3c',
                                marginBottom: '12px'
                            }} />
                            <div style={{ color: '#ffffff', fontSize: '24px', fontWeight: '700' }}>
                                {currentSubscription.autoRenew ? 'ON' : 'OFF'}
                            </div>
                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                Tự động gia hạn
                            </div>
                        </div>
                    </Col>
                </Row>

                {/* Payment History Section */}
                <div style={{
                    background: 'rgba(255, 255, 255, 0.03)',
                    border: '1px solid rgba(255, 140, 66, 0.1)',
                    borderRadius: '12px',
                    padding: '32px',
                    marginBottom: '40px'
                }}>
                    <div style={{
                        display: 'flex',
                        alignItems: 'center',
                        marginBottom: '24px'
                    }}>
                        <HistoryOutlined style={{ 
                            color: '#ff8c42', 
                            fontSize: '24px',
                            marginRight: '12px'
                        }} />
                        <h2 style={{
                            color: '#ffffff',
                            fontSize: '24px',
                            fontWeight: '700',
                            margin: 0
                        }}>
                            Payment History
                        </h2>
                    </div>
                    
                    <div style={{ overflow: 'auto' }}>
                        {paymentHistory.map((payment, index) => (
                            <div key={payment.id} style={{
                                background: index % 2 === 0 ? 'rgba(255, 255, 255, 0.02)' : 'transparent',
                                padding: '16px',
                                borderRadius: '8px',
                                marginBottom: '8px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'space-between',
                                border: '1px solid rgba(255, 255, 255, 0.05)'
                            }}>
                                <div style={{ display: 'flex', alignItems: 'center', gap: '16px' }}>
                                    <div style={{
                                        width: '48px',
                                        height: '48px',
                                        background: payment.status === 'success' 
                                            ? 'rgba(46, 204, 113, 0.2)' 
                                            : payment.status === 'failed' 
                                                ? 'rgba(231, 76, 60, 0.2)' 
                                                : 'rgba(255, 193, 7, 0.2)',
                                        borderRadius: '50%',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center'
                                    }}>
                                        <CreditCardOutlined style={{
                                            color: payment.status === 'success' 
                                                ? '#2ecc71' 
                                                : payment.status === 'failed' 
                                                    ? '#e74c3c' 
                                                    : '#f39c12',
                                            fontSize: '20px'
                                        }} />
                                    </div>
                                    <div>
                                        <div style={{ color: '#ffffff', fontSize: '16px', fontWeight: '600' }}>
                                            {payment.plan}
                                        </div>
                                        <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                            {new Date(payment.date).toLocaleDateString('vi-VN')} • {payment.method}
                                        </div>
                                    </div>
                                </div>
                                
                                <div style={{ textAlign: 'right' }}>
                                    <div style={{ color: '#ff8c42', fontSize: '18px', fontWeight: '700' }}>
                                        {payment.amount.toLocaleString('vi-VN')}₫
                                    </div>
                                    <Tag
                                        style={{
                                            background: payment.status === 'success' 
                                                ? 'rgba(46, 204, 113, 0.2)' 
                                                : payment.status === 'failed' 
                                                    ? 'rgba(231, 76, 60, 0.2)' 
                                                    : 'rgba(255, 193, 7, 0.2)',
                                            color: payment.status === 'success' 
                                                ? '#2ecc71' 
                                                : payment.status === 'failed' 
                                                    ? '#e74c3c' 
                                                    : '#f39c12',
                                            border: 'none',
                                            borderRadius: '16px',
                                            fontSize: '12px',
                                            fontWeight: '600'
                                        }}
                                    >
                                        {payment.status === 'success' ? 'Thành công' : 
                                         payment.status === 'failed' ? 'Thất bại' : 'Đang xử lý'}
                                    </Tag>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Danger Zone */}
                <div style={{
                    background: 'linear-gradient(135deg, rgba(231, 76, 60, 0.05) 0%, rgba(0, 0, 0, 0.3) 100%)',
                    border: '1px solid rgba(231, 76, 60, 0.2)',
                    borderRadius: '12px',
                    padding: '24px'
                }}>
                    <h3 style={{
                        color: '#e74c3c',
                        fontSize: '18px',
                        fontWeight: '600',
                        marginBottom: '16px',
                        display: 'flex',
                        alignItems: 'center',
                        gap: '8px'
                    }}>
                        <WarningOutlined />
                        Danger Zone
                    </h3>
                    <p style={{
                        color: 'rgba(255, 255, 255, 0.7)',
                        marginBottom: '16px',
                        fontSize: '14px'
                    }}>
                        Hủy đăng ký sẽ ngừng tự động gia hạn. Bạn vẫn có thể sử dụng dịch vụ đến hết thời hạn hiện tại.
                    </p>
                    <Button
                        danger
                        size="large"
                        onClick={handleCancelSubscription}
                        style={{
                            borderRadius: '8px',
                            fontWeight: '600'
                        }}
                    >
                        Hủy đăng ký
                    </Button>
                </div>
            </div>

            {/* Cancel Subscription Modal */}
            <Modal
                title={
                    <div style={{ 
                        display: 'flex', 
                        alignItems: 'center', 
                        gap: '12px',
                        color: '#ffffff'
                    }}>
                        <WarningOutlined style={{ color: '#f39c12', fontSize: '24px' }} />
                        <span style={{ fontSize: '18px', fontWeight: '600' }}>
                            Xác nhận hủy đăng ký
                        </span>
                    </div>
                }
                open={cancelModalVisible}
                onOk={confirmCancelSubscription}
                onCancel={() => setCancelModalVisible(false)}
                okText="Xác nhận hủy"
                cancelText="Giữ lại"
                okButtonProps={{
                    danger: true,
                    size: 'large',
                    style: { fontWeight: '600' }
                }}
                cancelButtonProps={{
                    size: 'large',
                    style: { fontWeight: '600' }
                }}
                width={600}
                centered
                bodyStyle={{
                    background: '#1a1a1a',
                    color: '#ffffff',
                    padding: '32px'
                }}
                style={{
                    top: '20%'
                }}
            >
                <div style={{ padding: '16px 0' }}>
                    <p style={{ 
                        color: 'rgba(255, 255, 255, 0.9)',
                        fontSize: '16px',
                        lineHeight: '1.6',
                        marginBottom: '16px'
                    }}>
                        Bạn có chắc chắn muốn hủy đăng ký gói{' '}
                        <strong style={{ color: '#ff8c42' }}>{currentSubscription.plan}</strong>?
                    </p>
                    <p style={{ 
                        color: 'rgba(255, 255, 255, 0.7)',
                        fontSize: '14px',
                        lineHeight: '1.6'
                    }}>
                        Sau khi hủy, bạn sẽ vẫn có thể sử dụng dịch vụ đến ngày{' '}
                        <strong style={{ color: '#ff8c42' }}>
                            {new Date(currentSubscription.endDate).toLocaleDateString('vi-VN')}
                        </strong>{' '}
                        và không được gia hạn tự động.
                    </p>
                </div>
            </Modal>

            {/* Global Styles */}
            <style jsx global>{`
                .ant-modal-content {
                    background: #1a1a1a !important;
                    border-radius: 12px !important;
                    border: 1px solid rgba(255, 140, 66, 0.2) !important;
                }
                .ant-modal-header {
                    background: #1a1a1a !important;
                    border-bottom: 1px solid rgba(255, 140, 66, 0.2) !important;
                    border-radius: 12px 12px 0 0 !important;
                    padding: 24px 32px 16px !important;
                }
                .ant-modal-footer {
                    background: #1a1a1a !important;
                    border-top: 1px solid rgba(255, 140, 66, 0.2) !important;
                    border-radius: 0 0 12px 12px !important;
                    padding: 16px 32px 24px !important;
                }
                .ant-btn:hover {
                    transform: translateY(-2px) !important;
                    transition: all 0.3s ease !important;
                }
                
                /* Scrollbar Styling */
                ::-webkit-scrollbar {
                    width: 8px;
                }
                ::-webkit-scrollbar-track {
                    background: rgba(255, 255, 255, 0.1);
                    border-radius: 4px;
                }
                ::-webkit-scrollbar-thumb {
                    background: rgba(255, 140, 66, 0.6);
                    border-radius: 4px;
                }
                ::-webkit-scrollbar-thumb:hover {
                    background: rgba(255, 140, 66, 0.8);
                }
            `}</style>
        </div>
    );
};

export default MySubscriptionPage;
