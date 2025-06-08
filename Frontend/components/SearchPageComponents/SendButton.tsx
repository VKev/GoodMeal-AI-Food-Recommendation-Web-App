import React from 'react';
import { Button } from 'antd';
import { ArrowUpOutlined } from '@ant-design/icons';
import { UploadedImage } from './types';

interface SendButtonProps {
    inputMessage: string;
    uploadedImages: UploadedImage[];
    onSend?: () => void;
}

const SendButton: React.FC<SendButtonProps> = ({
    inputMessage,
    uploadedImages,
    onSend
}) => {
    const isDisabled = !inputMessage.trim() && uploadedImages.length === 0;
    const hasContent = inputMessage.trim() || uploadedImages.length > 0;

    return (
        <Button
            type="primary"
            shape="round"
            icon={<ArrowUpOutlined />}
            disabled={isDisabled}
            onClick={onSend}
            style={{
                background: hasContent
                    ? 'linear-gradient(135deg, #ff7a00 0%, #ff9500 50%, #ffb347 100%)'
                    : 'rgba(64, 64, 64, 0.8)',
                border: 'none',
                width: '52px',
                height: '52px',
                boxShadow: hasContent
                    ? '0 8px 32px rgba(255, 122, 0, 0.4), 0 4px 16px rgba(255, 122, 0, 0.2)'
                    : '0 4px 12px rgba(0, 0, 0, 0.2)',
                transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
                fontSize: '18px',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center'
            }}
            onMouseEnter={(e) => {
                if (hasContent) {
                    e.currentTarget.style.transform = 'scale(1.05)';
                    e.currentTarget.style.boxShadow = '0 12px 40px rgba(255, 122, 0, 0.5), 0 8px 20px rgba(255, 122, 0, 0.3)';
                }
            }}
            onMouseLeave={(e) => {
                e.currentTarget.style.transform = 'scale(1)';
                e.currentTarget.style.boxShadow = hasContent
                    ? '0 8px 32px rgba(255, 122, 0, 0.4), 0 4px 16px rgba(255, 122, 0, 0.2)'
                    : '0 4px 12px rgba(0, 0, 0, 0.2)';
            }}
        />
    );
};

export default SendButton;
