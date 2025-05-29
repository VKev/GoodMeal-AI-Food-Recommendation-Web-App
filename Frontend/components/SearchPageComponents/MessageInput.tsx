import React from 'react';
import { Button, Input, Upload } from 'antd';
import { AudioOutlined, PictureOutlined } from '@ant-design/icons';

const { TextArea } = Input;

interface MessageInputProps {
    inputMessage: string;
    setInputMessage: (message: string) => void;
    textAreaRef: React.RefObject<HTMLTextAreaElement | null>;
    handlePaste: (e: React.ClipboardEvent<HTMLTextAreaElement>) => void;
    handleDrop: (e: React.DragEvent<HTMLTextAreaElement>) => void;
    handleDragOver: (e: React.DragEvent<HTMLTextAreaElement>) => void;
    handleUpload: (file: File) => boolean;
}

const MessageInput: React.FC<MessageInputProps> = ({
    inputMessage,
    setInputMessage,
    textAreaRef,
    handlePaste,
    handleDrop,
    handleDragOver,
    handleUpload
}) => {
    return (
        <div style={{ flex: 1, position: 'relative' }}>
            <TextArea
                value={inputMessage}
                onChange={(e) => setInputMessage(e.target.value)}
                onPaste={handlePaste}
                onDrop={handleDrop}
                onDragOver={handleDragOver}
                ref={textAreaRef}
                placeholder="Enter your question or drag & drop/paste images..."
                autoSize={{ minRows: 1, maxRows: 3 }}
                style={{
                    background: 'linear-gradient(135deg, rgba(255, 255, 255, 0.08) 0%, rgba(255, 255, 255, 0.04) 100%)',
                    border: '1.5px solid rgba(255, 122, 0, 0.2)',
                    borderRadius: '16px',
                    padding: '16px 80px 16px 20px',
                    fontSize: '15px',
                    color: '#ffffff',
                    resize: 'none',
                    fontWeight: '400',
                    lineHeight: '1.5',
                    transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
                    boxShadow: '0 4px 20px rgba(0, 0, 0, 0.1), inset 0 1px 0 rgba(255, 255, 255, 0.1)'
                }}
                onFocus={(e) => {
                    e.target.style.borderColor = 'rgba(255, 122, 0, 0.6)';
                    e.target.style.boxShadow = '0 4px 20px rgba(0, 0, 0, 0.1), 0 0 0 3px rgba(255, 122, 0, 0.1), inset 0 1px 0 rgba(255, 255, 255, 0.1)';
                }}
                onBlur={(e) => {
                    e.target.style.borderColor = 'rgba(255, 122, 0, 0.2)';
                    e.target.style.boxShadow = '0 4px 20px rgba(0, 0, 0, 0.1), inset 0 1px 0 rgba(255, 255, 255, 0.1)';
                }}
            />
            
            <Button
                type="text"
                icon={<AudioOutlined />}
                style={{
                    position: 'absolute',
                    right: '44px',
                    bottom: '10px',
                    color: 'rgba(255, 255, 255, 0.6)',
                    width: '32px',
                    height: '32px',
                    borderRadius: '8px',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    transition: 'all 0.2s ease',
                    fontSize: '16px'
                }}
                onMouseEnter={(e) => {
                    e.currentTarget.style.backgroundColor = 'rgba(255, 122, 0, 0.1)';
                    e.currentTarget.style.color = 'rgba(255, 122, 0, 0.8)';
                }}
                onMouseLeave={(e) => {
                    e.currentTarget.style.backgroundColor = 'transparent';
                    e.currentTarget.style.color = 'rgba(255, 255, 255, 0.6)';
                }}
            />
            
            <Upload
                beforeUpload={handleUpload}
                showUploadList={false}
                accept="image/*"
            >
                <Button
                    type="text"
                    icon={<PictureOutlined />}
                    style={{
                        position: 'absolute',
                        right: '10px',
                        bottom: '10px',
                        color: 'rgba(255, 255, 255, 0.6)',
                        width: '32px',
                        height: '32px',
                        borderRadius: '8px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        transition: 'all 0.2s ease',
                        fontSize: '16px'
                    }}
                    onMouseEnter={(e) => {
                        e.currentTarget.style.backgroundColor = 'rgba(255, 122, 0, 0.1)';
                        e.currentTarget.style.color = 'rgba(255, 122, 0, 0.8)';
                    }}
                    onMouseLeave={(e) => {
                        e.currentTarget.style.backgroundColor = 'transparent';
                        e.currentTarget.style.color = 'rgba(255, 255, 255, 0.6)';
                    }}
                />
            </Upload>
        </div>
    );
};

export default MessageInput;
