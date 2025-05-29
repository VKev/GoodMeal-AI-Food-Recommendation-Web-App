import React from 'react';
import { Flex, Typography } from 'antd';

const { Text } = Typography;

interface InputAreaFooterProps {
    inputMessageLength: number;
    maxLength?: number;
}

const InputAreaFooter: React.FC<InputAreaFooterProps> = ({
    inputMessageLength,
    maxLength = 2000
}) => {
    return (
        <Flex justify="space-between" style={{ marginTop: '12px' }}>
            <Text type="secondary" style={{ 
                fontSize: '11px',
                color: 'rgba(255, 255, 255, 0.5)',
                fontWeight: '400'
            }}>
                AI Chat Pro can make mistakes. Please verify important information.
            </Text>
            <Text type="secondary" style={{ 
                fontSize: '11px',
                color: 'rgba(255, 122, 0, 0.7)',
                fontWeight: '500'
            }}>
                {inputMessageLength}/{maxLength}
            </Text>
        </Flex>
    );
};

export default InputAreaFooter;
