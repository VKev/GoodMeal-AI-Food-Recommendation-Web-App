import React, { useRef, useState } from 'react';
import { Flex } from 'antd';
import ImageUploadArea from './ImageUploadArea';
import MessageInput from './MessageInput';
import SendButton from './SendButton';
import { useImageUpload } from './hooks/useImageUpload';
import { InputAreaProps, UploadedImage } from './types';

const InputArea: React.FC<InputAreaProps> = ({ inputMessage, setInputMessage }) => {
    const [uploadedImages, setUploadedImages] = useState<UploadedImage[]>([]);
    const textAreaRef = useRef<HTMLTextAreaElement>(null);

    const {
        handleUpload,
        handlePaste,
        handleDrop,
        handleDragOver,
        removeImage
    } = useImageUpload(uploadedImages, setUploadedImages);

    const handleSend = () => {
        // Add send logic here
        console.log('Sending message:', inputMessage);
        console.log('Uploaded images:', uploadedImages);
    };

    return (
        <div
            style={{
                borderTop: '1px solid rgba(255, 122, 0, 0.2)',
                padding: '20px 24px',
                background: 'linear-gradient(135deg, rgba(26, 26, 29, 0.95) 0%, rgba(20, 20, 23, 0.98) 100%)',
                backdropFilter: 'blur(20px)',
                minHeight: uploadedImages.length > 0 ? '180px' : '120px',
                maxHeight: uploadedImages.length > 0 ? '300px' : '180px',
                flexShrink: 0,
                transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
                borderRadius: '0',
                boxShadow: '0 -4px 32px rgba(0, 0, 0, 0.3), 0 -1px 0 rgba(255, 122, 0, 0.1)'
            }}
        >
            <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
                <ImageUploadArea
                    uploadedImages={uploadedImages}
                    removeImage={removeImage}
                />

                <Flex gap={16} align="flex-end">
                    <MessageInput
                        inputMessage={inputMessage}
                        setInputMessage={setInputMessage}
                        textAreaRef={textAreaRef}
                        handlePaste={handlePaste}
                        handleDrop={handleDrop}
                        handleDragOver={handleDragOver}
                        handleUpload={handleUpload}
                    />
                    
                    <SendButton
                        inputMessage={inputMessage}
                        uploadedImages={uploadedImages}
                        onSend={handleSend}
                    />
                </Flex>
                
            </div>
        </div>
    );
};

export default InputArea;
