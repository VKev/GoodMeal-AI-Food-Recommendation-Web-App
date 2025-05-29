import React from 'react';
import { message } from 'antd';
import { UploadedImage } from '../types';

export interface ImageUploadHookReturn {
    handleUpload: (file: File) => boolean;
    handlePaste: (e: React.ClipboardEvent<HTMLTextAreaElement>) => void;
    handleDrop: (e: React.DragEvent<HTMLTextAreaElement>) => void;
    handleDragOver: (e: React.DragEvent<HTMLTextAreaElement>) => void;
    removeImage: (uid: string) => void;
}

export const useImageUpload = (
    uploadedImages: UploadedImage[],
    setUploadedImages: React.Dispatch<React.SetStateAction<UploadedImage[]>>
): ImageUploadHookReturn => {
    // Handle file upload
    const handleUpload = (file: File) => {
        // Check file type
        const isImage = file.type.startsWith('image/');
        if (!isImage) {
            message.error('You can only upload image files!');
            return false;
        }

        // Check file size (5MB limit)
        const isLt5M = file.size / 1024 / 1024 < 5;
        if (!isLt5M) {
            message.error('Image must be smaller than 5MB!');
            return false;
        }

        // Create preview URL
        const reader = new FileReader();
        reader.onload = (e) => {
            const newImage: UploadedImage = {
                uid: Math.random().toString(36).substring(7),
                name: file.name,
                url: e.target?.result as string
            };
            setUploadedImages(prev => [...prev, newImage]);
        };
        reader.readAsDataURL(file);

        return false; // Prevent default upload
    };

    // Handle paste event
    const handlePaste = (e: React.ClipboardEvent<HTMLTextAreaElement>) => {
        const items = e.clipboardData?.items;
        if (!items) return;

        for (let i = 0; i < items.length; i++) {
            const item = items[i];
            if (item.type.startsWith('image/')) {
                e.preventDefault();
                const file = item.getAsFile();
                if (file) {
                    handleUpload(file);
                }
                return;
            }
        }
    };

    // Handle drag and drop
    const handleDrop = (e: React.DragEvent<HTMLTextAreaElement>) => {
        e.preventDefault();
        const files = e.dataTransfer.files;
        
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            if (file.type.startsWith('image/')) {
                handleUpload(file);
            }
        }
    };

    const handleDragOver = (e: React.DragEvent<HTMLTextAreaElement>) => {
        e.preventDefault();
    };

    // Remove uploaded image
    const removeImage = (uid: string) => {
        setUploadedImages(prev => prev.filter(img => img.uid !== uid));
    };

    return {
        handleUpload,
        handlePaste,
        handleDrop,
        handleDragOver,
        removeImage
    };
};
