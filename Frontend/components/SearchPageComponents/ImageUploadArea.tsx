import React from 'react';
import { Flex, Image, Button, Typography } from 'antd';
import { CloseOutlined } from '@ant-design/icons';
import { UploadedImage } from './types';

const { Text } = Typography;

interface ImageUploadAreaProps {
    uploadedImages: UploadedImage[];
    removeImage: (uid: string) => void;
}

const ImageUploadArea: React.FC<ImageUploadAreaProps> = ({
    uploadedImages,
    removeImage
}) => {
    if (uploadedImages.length === 0) return null;

    return (
        <div style={{ marginBottom: '16px' }}>
            <Flex gap={12} wrap="wrap" style={{ marginBottom: '8px' }}>
                {uploadedImages.map((img) => (
                    <div
                        key={img.uid}
                        style={{
                            position: 'relative',
                            width: '80px',
                            height: '80px',
                            borderRadius: '12px',
                            overflow: 'hidden',
                            border: '2px solid rgba(255, 122, 0, 0.3)',
                            background: 'rgba(255, 122, 0, 0.05)',
                            boxShadow: '0 4px 12px rgba(0, 0, 0, 0.3)',
                            transition: 'all 0.2s ease'
                        }}
                    >
                        <Image
                            src={img.url}
                            alt={img.name}
                            width={80}
                            height={80}
                            style={{ objectFit: 'cover' }}
                            preview={true}
                        />
                        <Button
                            type="text"
                            size="small"
                            icon={<CloseOutlined />}
                            onClick={() => removeImage(img.uid)}
                            style={{
                                position: 'absolute',
                                top: '4px',
                                right: '4px',
                                width: '20px',
                                height: '20px',
                                backgroundColor: 'rgba(0, 0, 0, 0.8)',
                                color: '#ffffff',
                                fontSize: '10px',
                                borderRadius: '50%',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                border: '1px solid rgba(255, 255, 255, 0.2)',
                                boxShadow: '0 2px 8px rgba(0, 0, 0, 0.3)',
                                transition: 'all 0.2s ease'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.backgroundColor = 'rgba(255, 59, 48, 0.9)';
                                e.currentTarget.style.transform = 'scale(1.1)';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.backgroundColor = 'rgba(0, 0, 0, 0.8)';
                                e.currentTarget.style.transform = 'scale(1)';
                            }}
                        />
                    </div>
                ))}
            </Flex>
            <Text 
                type="secondary" 
                style={{ 
                    fontSize: '11px', 
                    color: 'rgba(255, 122, 0, 0.7)',
                    fontWeight: '500'
                }}
            >
                {uploadedImages.length} images uploaded
            </Text>
        </div>
    );
};

export default ImageUploadArea;
