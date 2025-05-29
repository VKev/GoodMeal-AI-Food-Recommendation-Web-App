import React from 'react';
import {
    Layout,
    Typography,
    Flex
} from 'antd';

const { Header } = Layout;
const { Title, Text } = Typography;

interface SearchHeaderProps {
    collapsed: boolean;
}

const SearchHeader: React.FC<SearchHeaderProps> = ({ collapsed }) => {
    return (
        <Header
            style={{
                background: 'rgba(26, 26, 29, 0.5)',
                backdropFilter: 'blur(10px)',
                borderBottom: '1px solid #404040',
                padding: '0 24px',
                height: 'auto',
                maxHeight: '120px',
                lineHeight: 'normal',
                paddingTop: '24px',
                paddingBottom: '24px',
                flexShrink: 0
            }}
        >
            <Flex justify="space-between" align="center">
                <div style={{ marginLeft: collapsed ? '60px' : '0', transition: 'all 0.3s' }}>
                    <Title level={2} style={{ margin: 0, color: '#ffffff' }}>
                        Xin chào! Tôi có thể giúp gì?
                    </Title>
                    <Text type="secondary" style={{ fontSize: '14px' }}>
                        Hỏi bất cứ điều gì về lập trình, thiết kế, hoặc công nghệ
                    </Text>
                </div>
            </Flex>
        </Header>
    );
};

export default SearchHeader;
