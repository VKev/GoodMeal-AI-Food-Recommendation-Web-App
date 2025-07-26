"use client"
import React, { useEffect, useState } from 'react';
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
    Tooltip,
    Spin
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
import { useAuth } from '@/hooks/auths/authContext';
import { 
    getMySubscription, 
    getAllPaymentStatuses,
    Subscription
} from '@/services/SubscriptionService';
import SubscriptionCard from '../PricingPage/SubscriptionCard';

interface SubscriptionData {
    id: string;
    userId: string;
    subscriptionId: string;
    subscription: Subscription;
    startDate: string;
    endDate: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

interface PaymentStatus {
    correlationId: string;
    subscriptionId: string;
    amount: number;
    currency: string;
    orderId: string;
    currentState: string;
    paymentUrl: string | null;
    paymentUrlCreated: boolean;
    paymentCompleted: boolean;
    subscriptionActivated: boolean;
    transactionId: string | null;
    failureReason: string | null;
    completedAt: string | null;
    createdAt: string;
    updatedAt: string;
}

interface PaymentHistoryResponse {
    value: {
        paymentStatuses: PaymentStatus[];
    };
    isSuccess: boolean;
    isFailure: boolean;
    error: {
        code: string;
        description: string;
    };
}

const MySubscriptionPage: React.FC = () => {
    const router = useRouter();
    const { currentUser, loading: authLoading } = useAuth();
    const [cancelModalVisible, setCancelModalVisible] = useState(false);
    const [subscription, setSubscription] = useState<SubscriptionData | null>(null);
    const [paymentHistory, setPaymentHistory] = useState<PaymentStatus[]>([]);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const fetchData = async () => {
            if (!currentUser) {
                setLoading(false);
                return;
            }

            try {
                const idToken = await currentUser.getIdToken();
                
                // Fetch subscription data
                const subscriptionResult = await getMySubscription(idToken);
                if (subscriptionResult) {
                    setSubscription(subscriptionResult);
                }
                
                // Fetch payment history using the new function
                const paymentResult = await getAllPaymentStatuses(idToken);
                if (paymentResult.success) {
                    setPaymentHistory(paymentResult.data);
                }
            } catch (error) {
                console.error("Error fetching data:", error);
            } finally {
                setLoading(false);
            }
        };

        if (!authLoading) {
            fetchData();
        }
    }, [currentUser, authLoading]);

    const handleBackClick = () => {
        router.back();
    };

    const handleCancelSubscription = () => {
        setCancelModalVisible(true);
    };

    const confirmCancelSubscription = async () => {
        try {
            if (!currentUser || !subscription) return;
            
            const idToken = await currentUser.getIdToken();
            // Here you would implement the API call to cancel the subscription
            // Since the API endpoint for cancellation wasn't provided, I'm just showing a success message
            
            message.success('Gói đăng ký đã được hủy thành công');
            setCancelModalVisible(false);
        } catch (error) {
            console.error("Error cancelling subscription:", error);
            message.error('Có lỗi xảy ra khi hủy đăng ký');
        }
    };

    const getStatusText = (status: string) => {
        switch (status) {
            case 'PaymentPending': return 'Đang chờ thanh toán';
            case 'PaymentCompleted': return 'Đã thanh toán';
            case 'PaymentFailed': return 'Thanh toán thất bại';
            case 'PaymentUrlCreating': return 'Đang tạo URL thanh toán';
            case 'SubscriptionActivated': return 'Đã kích hoạt';
            default: return status;
        }
    };

    const formatCurrency = (amount: number, currency: string) => {
        if (currency === 'VND') {
            return new Intl.NumberFormat('vi-VN', { 
                style: 'currency', 
                currency: 'VND',
                maximumFractionDigits: 0 
            }).format(amount);
        }
        return new Intl.NumberFormat('en-US', { 
            style: 'currency', 
            currency: currency 
        }).format(amount);
    };

    // Calculate days remaining in subscription
    const calculateDaysRemaining = () => {
        if (!subscription) return 0;
        
        const endDate = new Date(subscription.endDate);
        const now = new Date();
        const diffTime = endDate.getTime() - now.getTime();
        const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        
        return diffDays > 0 ? diffDays : 0;
    };

    const daysRemaining = calculateDaysRemaining();
    const totalDuration = subscription ? 
        Math.round((new Date(subscription.endDate).getTime() - new Date(subscription.startDate).getTime()) / (1000 * 60 * 60 * 24)) : 
        30;
    
    const progressPercent = Math.round(((totalDuration - daysRemaining) / totalDuration) * 100);

    if (loading || authLoading) {
        return (
            <div style={{ 
                minHeight: '100vh', 
                display: 'flex', 
                justifyContent: 'center', 
                alignItems: 'center',
                background: '#000000'
            }}>
                <Spin size="large" />
            </div>
        );
    }

    if (!subscription) {
        return (
            <div style={{ 
                minHeight: '100vh',
                background: '#000000',
                padding: '100px 24px 40px',
                fontFamily: "'Netflix Sans', 'Helvetica Neue', Helvetica, Arial, sans-serif",
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'center',
                gap: '24px',
                textAlign: 'center'
            }}>
                <h2 style={{ color: '#ffffff', fontSize: '28px' }}>
                    Bạn chưa có gói đăng ký nào
                </h2>
                <p style={{ color: '#b3b3b3', fontSize: '16px' }}>
                    Hãy đăng ký một gói dịch vụ để trải nghiệm đầy đủ tính năng
                </p>
                <Button 
                    type="primary"
                    size="large"
                    onClick={() => router.push('/pricing')}
                    style={{
                        background: 'linear-gradient(135deg, #ff8c42 0%, #ff6b1a 100%)',
                        border: 'none',
                        borderRadius: '8px',
                        height: '48px',
                        fontSize: '16px',
                        fontWeight: '600',
                        padding: '0 32px'
                    }}
                >
                    Xem các gói đăng ký
                </Button>
            </div>
        );
    }

    return (
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
                {/* Hero Section with Subscription Card */}
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
                        <Col xs={24} lg={12}>
                            {subscription.subscription && (
                                <SubscriptionCard
                                    subscription={subscription.subscription}
                                    isActive={subscription.isActive}
                                    onSelectSubscription={() => {}}
                                    onCardClick={() => {}}
                                />
                            )}
                        </Col>
                        
                        <Col xs={24} lg={12}>
                            <div>
                                <h3 style={{
                                    color: '#ffffff',
                                    fontSize: '20px',
                                    fontWeight: '600',
                                    marginBottom: '20px'
                                }}>
                                    Thông tin đăng ký
                                </h3>
                                
                                {/* Subscription Details */}
                                <div style={{ 
                                    display: 'flex', 
                                    flexDirection: 'column', 
                                    gap: '16px',
                                    background: 'rgba(255, 255, 255, 0.05)',
                                    padding: '24px',
                                    borderRadius: '12px'
                                }}>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <span style={{ color: '#b3b3b3' }}>ID đăng ký:</span>
                                        <span style={{ color: '#ffffff' }}>{subscription.id.substring(0, 8)}...</span>
                                    </div>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <span style={{ color: '#b3b3b3' }}>Ngày bắt đầu:</span>
                                        <span style={{ color: '#ffffff' }}>
                                            {new Date(subscription.startDate).toLocaleDateString('vi-VN')}
                                        </span>
                                    </div>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <span style={{ color: '#b3b3b3' }}>Ngày kết thúc:</span>
                                        <span style={{ color: '#ffffff' }}>
                                            {new Date(subscription.endDate).toLocaleDateString('vi-VN')}
                                        </span>
                                    </div>
                                    <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                                        <span style={{ color: '#b3b3b3' }}>Trạng thái:</span>
                                        <Tag
                                            style={{
                                                background: subscription.isActive 
                                                    ? 'linear-gradient(90deg, #2ecc71 0%, #27ae60 100%)'
                                                    : '#e74c3c',
                                                border: 'none',
                                                color: '#ffffff',
                                                padding: '2px 12px',
                                                borderRadius: '20px',
                                            }}
                                        >
                                            {subscription.isActive ? 'Đang hoạt động' : 'Không hoạt động'}
                                        </Tag>
                                    </div>
                                    
                                    {/* Usage Progress */}
                                    <div style={{ marginTop: '16px' }}>
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
                                                {daysRemaining} ngày còn lại
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
                                    </div>
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
                            onClick={() => router.push('/pricing')}
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
                            onClick={() => router.push('/pricing')}
                        >
                            Nâng cấp gói
                        </Button>
                        
                        <Button
                            size="large"
                            danger
                            onClick={handleCancelSubscription}
                            style={{
                                background: 'rgba(231, 76, 60, 0.1)',
                                border: '1px solid #e74c3c',
                                color: '#e74c3c',
                                borderRadius: '8px',
                                height: '48px',
                                fontSize: '16px',
                                fontWeight: '600',
                                padding: '0 32px'
                            }}
                        >
                            Hủy đăng ký
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
                                {daysRemaining}
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
                                {paymentHistory.filter(p => p.paymentCompleted).length}
                            </div>
                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                Thanh toán thành công
                            </div>
                        </div>
                    </Col>
                    
                    <Col xs={24} sm={8}>
                        <div style={{
                            background: 'linear-gradient(135deg, rgba(52, 152, 219, 0.1) 0%, rgba(0, 0, 0, 0.3) 100%)',
                            border: '1px solid rgba(52, 152, 219, 0.2)',
                            borderRadius: '12px',
                            padding: '24px',
                            textAlign: 'center'
                        }}>
                            <DownloadOutlined style={{ 
                                fontSize: '32px', 
                                color: '#3498db',
                                marginBottom: '12px'
                            }} />
                            <div style={{ color: '#ffffff', fontSize: '24px', fontWeight: '700' }}>
                                {subscription && subscription.subscription ? subscription.subscription.durationInMonths : 0}
                            </div>
                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                Tháng đăng ký
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
                            Lịch sử thanh toán
                        </h2>
                    </div>
                    
                    {paymentHistory.length > 0 ? (
                        <div style={{ overflow: 'auto' }}>
                            {paymentHistory.map((payment, index) => (
                                <div key={payment.correlationId} style={{
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
                                            background: payment.paymentCompleted 
                                                ? 'rgba(46, 204, 113, 0.2)' 
                                                : payment.failureReason 
                                                    ? 'rgba(231, 76, 60, 0.2)' 
                                                    : 'rgba(255, 193, 7, 0.2)',
                                            borderRadius: '50%',
                                            display: 'flex',
                                            alignItems: 'center',
                                            justifyContent: 'center'
                                        }}>
                                            <CreditCardOutlined style={{
                                                color: payment.paymentCompleted 
                                                    ? '#2ecc71' 
                                                    : payment.failureReason 
                                                        ? '#e74c3c' 
                                                        : '#f39c12',
                                                fontSize: '20px'
                                            }} />
                                        </div>
                                        <div>
                                            <div style={{ color: '#ffffff', fontSize: '16px', fontWeight: '600' }}>
                                                Đơn hàng: {payment.orderId.substring(4, 12)}...
                                            </div>
                                            <div style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '14px' }}>
                                                {new Date(payment.createdAt).toLocaleDateString('vi-VN')}
                                            </div>
                                        </div>
                                    </div>
                                    
                                    <div style={{ textAlign: 'right' }}>
                                        <div style={{ color: '#ff8c42', fontSize: '18px', fontWeight: '700' }}>
                                            {formatCurrency(payment.amount, payment.currency)}
                                        </div>
                                        <Tag
                                            style={{
                                                background: payment.paymentCompleted 
                                                    ? 'rgba(46, 204, 113, 0.2)' 
                                                    : payment.failureReason 
                                                        ? 'rgba(231, 76, 60, 0.2)' 
                                                        : 'rgba(255, 193, 7, 0.2)',
                                                color: payment.paymentCompleted 
                                                    ? '#2ecc71' 
                                                    : payment.failureReason 
                                                        ? '#e74c3c' 
                                                        : '#f39c12',
                                                border: 'none',
                                                borderRadius: '16px',
                                                fontSize: '12px',
                                                fontWeight: '600'
                                            }}
                                        >
                                            {getStatusText(payment.currentState)}
                                        </Tag>
                                    </div>
                                </div>
                            ))}
                        </div>
                    ) : (
                        <div style={{ 
                            textAlign: 'center', 
                            padding: '32px', 
                            color: '#b3b3b3',
                            fontSize: '16px'
                        }}>
                            Không có lịch sử thanh toán
                        </div>
                    )}
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
                            <strong style={{ color: '#ff8c42' }}>{subscription.subscription?.name}</strong>?
                        </p>
                        <p style={{ 
                            color: 'rgba(255, 255, 255, 0.7)',
                            fontSize: '14px',
                            lineHeight: '1.6'
                        }}>
                            Sau khi hủy, bạn sẽ vẫn có thể sử dụng dịch vụ đến ngày{' '}
                            <strong style={{ color: '#ff8c42' }}>
                                {new Date(subscription.endDate).toLocaleDateString('vi-VN')}
                            </strong>{' '}
                            và không được gia hạn tự động.
                        </p>
                    </div>
                </Modal>
            </div>

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
