import React, { useState } from 'react';
import { Button, Card, Typography, Space, Alert, Checkbox } from 'antd';
import { EnvironmentOutlined, CloseOutlined } from '@ant-design/icons';
import { useGeolocation, LocationData } from '../../hooks/useGeolocation';

const { Text } = Typography;

export interface LocationPromptProps {
  onLocationGranted?: (location: LocationData, rememberChoice: boolean) => void;
  onLocationDenied?: (rememberChoice: boolean) => void;
  onClose?: () => void;
}

export const LocationPrompt: React.FC<LocationPromptProps> = ({
  onLocationGranted,
  onLocationDenied,
  onClose
}) => {
  const { 
    location, 
    loading, 
    error, 
    permission, 
    requestLocation,
    hasLocation 
  } = useGeolocation();

  const [isRequesting, setIsRequesting] = useState(false);
  const [rememberChoice, setRememberChoice] = useState(false);

  React.useEffect(() => {
    if (location && onLocationGranted) {
      onLocationGranted(location, rememberChoice);
    }
  }, [location, onLocationGranted, rememberChoice]);

  React.useEffect(() => {
    if (permission === 'denied' && onLocationDenied && isRequesting) {
      setTimeout(() => {
        onLocationDenied(rememberChoice);
      }, 500);
    }
  }, [permission, onLocationDenied, isRequesting, rememberChoice]);

  const handleAllow = () => {
    console.log('=== LOCATION REQUEST ===');
    console.log('Remember choice:', rememberChoice);
    console.log('========================');
    
    setIsRequesting(true);
    requestLocation();
  };

  const handleDeny = () => {
    if (onLocationDenied) {
      onLocationDenied(rememberChoice);
    }
  };

  // Don't show if location is already granted and available
  if (hasLocation) {
    return null;
  }

  return (
    <Card
      style={{
        position: 'fixed',
        top: '20px',
        right: '20px',
        width: '350px',
        backgroundColor: '#262629',
        border: '1px solid rgba(255, 122, 0, 0.3)',
        borderRadius: '12px',
        boxShadow: '0 8px 32px rgba(0, 0, 0, 0.3)',
        zIndex: 1000
      }}
      bodyStyle={{ padding: '16px' }}
    >
      <Button
        type="text"
        icon={<CloseOutlined />}
        onClick={onClose}
        style={{
          position: 'absolute',
          top: 8,
          right: 8,
          color: '#b3b3b3'
        }}
        size="small"
      />

      <Space direction="vertical" size="small" style={{ width: '100%' }}>
        <div style={{ display: 'flex', alignItems: 'center', marginBottom: '8px' }}>
          <EnvironmentOutlined 
            style={{ 
              fontSize: '20px', 
              color: '#ff7a00',
              marginRight: '8px'
            }} 
          />
          <Text style={{ color: '#ffffff', fontWeight: 'bold' }}>
            Bật vị trí để tìm nhà hàng dễ dàng hơn?
          </Text>
        </div>

        <Text style={{ color: '#b3b3b3', fontSize: '13px', display: 'block' }}>
          Chúng tôi sẽ gợi ý các nhà hàng gần bạn nhất
        </Text>

        {error && (
          <Alert
            message={error}
            type="error"
            showIcon
            style={{
              backgroundColor: 'rgba(255, 77, 79, 0.1)',
              border: '1px solid rgba(255, 77, 79, 0.2)',
              color: '#ff4d4f',
              fontSize: '12px'
            }}
          />
        )}

        {isRequesting && !error && !location && (
          <Alert
            message="Vui lòng cho phép truy cập vị trí trong popup của trình duyệt"
            type="warning"
            showIcon
            style={{
              backgroundColor: 'rgba(250, 173, 20, 0.1)',
              border: '1px solid rgba(250, 173, 20, 0.2)',
              color: '#faad14',
              fontSize: '12px'
            }}
          />
        )}

        <div style={{ marginTop: '12px' }}>
          <Checkbox
            checked={rememberChoice}
            onChange={(e) => setRememberChoice(e.target.checked)}
            style={{ color: '#b3b3b3', fontSize: '12px' }}
          >
            Ghi nhớ lựa chọn (không hỏi lại)
          </Checkbox>
        </div>

        <div style={{ display: 'flex', gap: '8px', marginTop: '12px' }}>
          <Button
            type="primary"
            size="small"
            loading={loading || isRequesting}
            onClick={handleAllow}
            icon={<EnvironmentOutlined />}
            style={{
              backgroundColor: '#ff7a00',
              borderColor: '#ff7a00',
              fontSize: '12px',
              height: '32px'
            }}
          >
            {loading || isRequesting ? 'Đang lấy...' : 'Cho phép'}
          </Button>

          <Button
            type="text"
            size="small"
            onClick={handleDeny}
            style={{
              color: '#b3b3b3',
              fontSize: '12px',
              height: '32px'
            }}
          >
            Không
          </Button>
        </div>
      </Space>
    </Card>
  );
};

export default LocationPrompt;
