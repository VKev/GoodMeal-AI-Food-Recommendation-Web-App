import React from 'react';
import { Layout, Spin } from 'antd';
import { LoadingOutlined } from '@ant-design/icons';

interface LoadingComponentProps {
  message?: string;
  size?: 'small' | 'default' | 'large';
  fullScreen?: boolean;
}

const LoadingComponent: React.FC<LoadingComponentProps> = ({ 
  message = 'Đang tải...', 
  size = 'large',
  fullScreen = true 
}) => {
  const loadingIndicator = (
    <LoadingOutlined 
      style={{ 
        fontSize: size === 'large' ? 48 : size === 'default' ? 32 : 24, 
        color: '#ffa366' 
      }} 
      spin 
    />
  );

  if (fullScreen) {
    return (
      <Layout 
        style={{ 
          minHeight: "100vh", 
          background: "linear-gradient(135deg, #000000 0%, #1a1a1a 100%)" 
        }}
      >
        <div 
          style={{ 
            display: 'flex', 
            flexDirection: 'column',
            justifyContent: 'center', 
            alignItems: 'center', 
            minHeight: '100vh',
            gap: '16px'
          }}
        >
          <Spin size={size} indicator={loadingIndicator} />
          <div style={{ 
            color: 'rgba(255, 255, 255, 0.8)', 
            fontSize: '16px',
            fontWeight: '500',
            textAlign: 'center'
          }}>
            {message}
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'column',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '400px',
        gap: '16px'
      }}
    >
      <Spin size={size} indicator={loadingIndicator} />
      <div style={{ 
        color: 'rgba(255, 255, 255, 0.8)', 
        fontSize: '14px',
        fontWeight: '500',
        textAlign: 'center'
      }}>
        {message}
      </div>
    </div>
  );
};

export default LoadingComponent;
