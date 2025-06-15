"use client"
import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import {
    Layout,
    Typography,
    Card,
    Row,
    Col,
    Button,
    Tag,
    Progress,
    Table,
    Space,
    Divider,
    Avatar,
    Modal,
    message
} from 'antd';
import {
    ArrowLeftOutlined,
    CrownOutlined,
    CheckCircleOutlined,
    HistoryOutlined,
    WarningOutlined,
} from '@ant-design/icons';

const { Content, Header } = Layout;
const { Title, Text, Paragraph } = Typography;

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

    const getStatusColor = (status: string) => {
        switch (status) {
            case 'active': return '#52c41a';
            case 'expired': return '#f5222d';
            case 'cancelled': return '#faad14';
            default: return '#d9d9d9';
        }
    };

    const getStatusText = (status: string) => {
        switch (status) {
            case 'active': return 'Đang hoạt động';
            case 'expired': return 'Hết hạn';
            case 'cancelled': return 'Đã hủy';
            default: return 'Không xác định';
        }
    };

    const paymentColumns = [
        {
            title: 'Ngày',
            dataIndex: 'date',
            key: 'date',
            render: (date: string) => (
                <Text style={{ color: '#ffffff' }}>
                    {new Date(date).toLocaleDateString('vi-VN')}
                </Text>
            )
        },
        {
            title: 'Gói dịch vụ',
            dataIndex: 'plan',
            key: 'plan',
            render: (plan: string) => (
                <Text style={{ color: '#ff7a00', fontWeight: 'bold' }}>
                    {plan}
                </Text>
            )
        },
        {
            title: 'Số tiền',
            dataIndex: 'amount',
            key: 'amount',
            render: (amount: number) => (
                <Text style={{ color: '#ffffff', fontWeight: 'bold' }}>
                    {amount.toLocaleString('vi-VN')}đ
                </Text>
            )
        },
        {
            title: 'Phương thức',
            dataIndex: 'method',
            key: 'method',
            render: (method: string) => (
                <Text style={{ color: 'rgba(255, 255, 255, 0.8)' }}>
                    {method}
                </Text>
            )
        },
        {
            title: 'Trạng thái',
            dataIndex: 'status',
            key: 'status',
            render: (status: string) => (
                <Tag
                    color={status === 'success' ? 'green' : status === 'failed' ? 'red' : 'orange'}
                    style={{ borderRadius: '6px' }}
                >
                    {status === 'success' ? 'Thành công' : status === 'failed' ? 'Thất bại' : 'Đang xử lý'}
                </Tag>
            )
        }
    ];

    const progressPercent = Math.round(((31 - currentSubscription.daysRemaining) / 31) * 100);

    return (
        <Layout style={{
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%)'
        }}>
            <Header style={{
                background: 'rgba(0, 0, 0, 0.8)',
                padding: '0 24px',
                borderBottom: '1px solid rgba(255, 122, 0, 0.2)'
            }}>
                <div style={{
                    display: 'flex',
                    alignItems: 'center',
                    height: '100%'
                }}>
                    <Button
                        type="text"
                        icon={<ArrowLeftOutlined />}
                        onClick={handleBackClick}
                        style={{
                            color: '#ff7a00',
                            marginRight: '16px'
                        }}
                    />
                    <Title level={3} style={{
                        color: '#ffffff',
                        margin: 0,
                        flex: 1
                    }}>
                        My Subscription
                    </Title>
                </div>
            </Header>

            <Content style={{
                padding: '32px',
                overflow: 'auto'
            }}>
                <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                    {/* Current Subscription Card */}
                    <Card
                        style={{
                            background: 'rgba(255, 255, 255, 0.08)',
                            border: '1px solid rgba(255, 122, 0, 0.3)',
                            borderRadius: '16px',
                            marginBottom: '32px'
                        }}
                        bodyStyle={{ padding: '32px' }}
                    >
                        <Row gutter={[24, 24]}>
                            <Col xs={24} lg={12}>
                                <div style={{ textAlign: 'center', marginBottom: '24px' }}>
                                    <Avatar
                                        size={80}
                                        style={{
                                            background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                            marginBottom: '16px'
                                        }}
                                        icon={<CrownOutlined style={{ fontSize: '40px' }} />}
                                    />
                                    <Title level={2} style={{ color: '#ffffff', margin: 0 }}>
                                        {currentSubscription.plan}
                                    </Title>
                                    <Tag
                                        color={getStatusColor(currentSubscription.status)}
                                        style={{
                                            marginTop: '8px',
                                            padding: '4px 12px',
                                            borderRadius: '12px',
                                            fontSize: '14px'
                                        }}
                                    >
                                        {getStatusText(currentSubscription.status)}
                                    </Tag>
                                </div>

                                <div style={{ textAlign: 'center' }}>
                                    <Text style={{ color: '#ff7a00', fontSize: '36px', fontWeight: 'bold' }}>
                                        {currentSubscription.price.toLocaleString('vi-VN')}đ
                                    </Text>
                                    <Text style={{ color: 'rgba(255, 255, 255, 0.8)', display: 'block' }}>
                                        /tháng
                                    </Text>
                                </div>
                            </Col>

                            <Col xs={24} lg={12}>
                                <Space direction="vertical" size="large" style={{ width: '100%' }}>
                                    <div>
                                        <Title level={4} style={{ color: '#ffffff', marginBottom: '16px' }}>
                                            Thời gian sử dụng
                                        </Title>
                                        <div style={{ marginBottom: '16px' }}>
                                            <div style={{
                                                display: 'flex',
                                                justifyContent: 'space-between',
                                                marginBottom: '8px'
                                            }}>
                                                <Text style={{ color: 'rgba(255, 255, 255, 0.8)' }}>
                                                    Còn lại {currentSubscription.daysRemaining} ngày
                                                </Text>
                                                <Text style={{ color: '#ff7a00' }}>
                                                    {progressPercent}%
                                                </Text>
                                            </div>
                                            <Progress
                                                percent={progressPercent}
                                                strokeColor={{
                                                    '0%': '#ff7a00',
                                                    '100%': '#ff9500',
                                                }}
                                                trailColor="rgba(255, 255, 255, 0.1)"
                                                showInfo={false}
                                            />
                                        </div>
                                        <div style={{
                                            display: 'flex',
                                            justifyContent: 'space-between'
                                        }}>
                                            <div>
                                                <Text style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '12px' }}>
                                                    Bắt đầu
                                                </Text>
                                                <br />
                                                <Text style={{ color: '#ffffff' }}>
                                                    {new Date(currentSubscription.startDate).toLocaleDateString('vi-VN')}
                                                </Text>
                                            </div>
                                            <div style={{ textAlign: 'right' }}>
                                                <Text style={{ color: 'rgba(255, 255, 255, 0.6)', fontSize: '12px' }}>
                                                    Hết hạn
                                                </Text>
                                                <br />
                                                <Text style={{ color: '#ff7a00' }}>
                                                    {new Date(currentSubscription.endDate).toLocaleDateString('vi-VN')}
                                                </Text>
                                            </div>
                                        </div>
                                    </div>

                                    <div>
                                        <Title level={4} style={{ color: '#ffffff', marginBottom: '16px' }}>
                                            Tính năng bao gồm
                                        </Title>
                                        <Space direction="vertical" size="small">
                                            {currentSubscription.features.map((feature, index) => (
                                                <div key={index} style={{ display: 'flex', alignItems: 'center' }}>
                                                    <CheckCircleOutlined style={{ color: '#52c41a', marginRight: '8px' }} />
                                                    <Text style={{ color: 'rgba(255, 255, 255, 0.8)' }}>
                                                        {feature}
                                                    </Text>
                                                </div>
                                            ))}
                                        </Space>
                                    </div>
                                </Space>
                            </Col>
                        </Row>

                        <Divider style={{ borderColor: 'rgba(255, 122, 0, 0.2)', margin: '32px 0' }} />

                        <Row gutter={[16, 16]}>
                            <Col xs={24} sm={8}>
                                <Button
                                    type="primary"
                                    size="large"
                                    block
                                    style={{
                                        background: 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)',
                                        border: 'none',
                                        borderRadius: '8px',
                                        height: '48px'
                                    }}
                                >
                                    Gia hạn ngay
                                </Button>
                            </Col>
                            <Col xs={24} sm={8}>
                                <Button
                                    size="large"
                                    block
                                    style={{
                                        background: 'rgba(255, 122, 0, 0.1)',
                                        border: '1px solid rgba(255, 122, 0, 0.4)',
                                        color: '#ff7a00',
                                        borderRadius: '8px',
                                        height: '48px'
                                    }}
                                >
                                    Nâng cấp gói
                                </Button>
                            </Col>
                            <Col xs={24} sm={8}>
                                <Button
                                    size="large"
                                    block
                                    danger
                                    onClick={handleCancelSubscription}
                                    style={{
                                        borderRadius: '8px',
                                        height: '48px'
                                    }}
                                >
                                    Hủy đăng ký
                                </Button>
                            </Col>
                        </Row>
                    </Card>

                    {/* Payment History */}
                    <Card
                        title={
                            <div style={{ display: 'flex', alignItems: 'center' }}>
                                <HistoryOutlined style={{ color: '#ff7a00', marginRight: '8px' }} />
                                <Text style={{ color: '#ffffff', fontSize: '18px', fontWeight: 'bold' }}>
                                    Lịch sử thanh toán
                                </Text>
                            </div>
                        }
                        style={{
                            background: 'rgba(255, 255, 255, 0.08)',
                            border: '1px solid rgba(255, 122, 0, 0.2)',
                            borderRadius: '16px'
                        }}
                        bodyStyle={{ padding: '24px' }}
                    >
                        <Table
                            columns={paymentColumns}
                            dataSource={paymentHistory}
                            rowKey="id"
                            pagination={false}
                            style={{
                                background: 'transparent'
                            }}
                            className="custom-table"
                        />
                    </Card>
                </div>
            </Content>

            {/* Cancel Subscription Modal */}
            <Modal
                title={
                    <div style={{ display: 'flex', alignItems: 'center' }}>
                        <WarningOutlined style={{ color: '#faad14', marginRight: '8px' }} />
                        <Text style={{ color: '#ffffff' }}>Xác nhận hủy đăng ký</Text>
                    </div>
                }
                open={cancelModalVisible}
                onOk={confirmCancelSubscription}
                onCancel={() => setCancelModalVisible(false)}
                okText="Xác nhận hủy"
                cancelText="Giữ lại"
                okButtonProps={{
                    danger: true
                }}
                style={{
                    top: '20%'
                }}
                bodyStyle={{
                    background: '#2d2d2d',
                    color: '#ffffff'
                }}
            >
                <Paragraph style={{ color: 'rgba(255, 255, 255, 0.8)' }}>
                    Bạn có chắc chắn muốn hủy đăng ký gói <strong style={{ color: '#ff7a00' }}>{currentSubscription.plan}</strong>?
                </Paragraph>
                <Paragraph style={{ color: 'rgba(255, 255, 255, 0.8)' }}>
                    Sau khi hủy, bạn sẽ vẫn có thể sử dụng dịch vụ đến ngày <strong style={{ color: '#ff7a00' }}>
                        {new Date(currentSubscription.endDate).toLocaleDateString('vi-VN')}
                    </strong> và không được gia hạn tự động.
                </Paragraph>
            </Modal>

            <style jsx global>{`
                .custom-table .ant-table {
                    background: transparent !important;
                }
                .custom-table .ant-table-thead > tr > th {
                    background: rgba(255, 122, 0, 0.1) !important;
                    border-bottom: 1px solid rgba(255, 122, 0, 0.2) !important;
                    color: #ff7a00 !important;
                    font-weight: bold !important;
                }
                .custom-table .ant-table-tbody > tr > td {
                    border-bottom: 1px solid rgba(255, 255, 255, 0.1) !important;
                    background: transparent !important;
                }
                .custom-table .ant-table-tbody > tr:hover > td {
                    background: rgba(255, 122, 0, 0.05) !important;
                }
                .ant-modal-content {
                    background: #2d2d2d !important;
                }
                .ant-modal-header {
                    background: #2d2d2d !important;
                    border-bottom: 1px solid rgba(255, 122, 0, 0.2) !important;
                }
            `}</style>
        </Layout>
    );
};

export default MySubscriptionPage;
