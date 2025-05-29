import React from 'react';
import {
    Button,
    Typography,
    Flex,
    Input
} from 'antd';
import {
    AudioOutlined,
    ArrowUpOutlined
} from '@ant-design/icons';

const { Text } = Typography;
const { TextArea } = Input;

interface InputAreaProps {
    message: string;
    setMessage: (message: string) => void;
}

const InputArea: React.FC<InputAreaProps> = ({ message, setMessage }) => {
    return (
        <div
            style={{
                borderTop: '1px solid #404040',
                padding: '24px',
                background: 'rgba(26, 26, 29, 0.8)',
                backdropFilter: 'blur(10px)',
                maxHeight: '160px',
                flexShrink: 0
            }}
        >
            <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                <Flex gap={16} align="flex-end">
                    <div style={{ flex: 1, position: 'relative' }}>
                        <TextArea
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            placeholder="Nhập câu hỏi của bạn..."
                            autoSize={{ minRows: 1, maxRows: 4 }}
                            style={{
                                background: 'rgba(128, 128, 128, 0.1)',
                                border: '1px solid rgba(128, 128, 128, 0.2)',
                                borderRadius: '12px',
                                padding: '16px 60px 16px 16px',
                                fontSize: '16px',
                                color: '#ffffff',
                                resize: 'none'
                            }}
                        />
                        <Button
                            type="text"
                            icon={<AudioOutlined />}
                            style={{
                                position: 'absolute',
                                right: '8px',
                                bottom: '8px',
                                color: '#b3b3b3'
                            }}
                        />
                    </div>
                    <Button
                        type="primary"
                        shape="round"
                        icon={<ArrowUpOutlined />}
                        disabled={!message.trim()}
                        style={{
                            background: message.trim()
                                ? 'linear-gradient(45deg, #ff7a00 0%, #ff9500 100%)'
                                : '#404040',
                            border: 'none',
                            width: '48px',
                            height: '48px',
                            boxShadow: message.trim()
                                ? '0 4px 15px rgba(255, 122, 0, 0.25)'
                                : 'none'
                        }}
                    />
                </Flex>
                <Flex justify="space-between" style={{ marginTop: '12px' }}>
                    <Text type="secondary" style={{ fontSize: '12px' }}>
                        AI Chat Pro có thể mắc lỗi. Hãy kiểm tra thông tin quan trọng.
                    </Text>
                    <Text type="secondary" style={{ fontSize: '12px' }}>
                        {message.length}/2000
                    </Text>
                </Flex>
            </div>
        </div>
    );
};

export default InputArea;
