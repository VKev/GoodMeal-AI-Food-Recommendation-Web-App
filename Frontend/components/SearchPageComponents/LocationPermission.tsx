import React from 'react';
import { Button, Card, Typography, Space, Alert } from 'antd';
import { EnvironmentOutlined, CloseOutlined } from '@ant-design/icons';
import { useGeolocation, LocationData } from '../../hooks/useGeolocation';

const { Title, Text } = Typography;

export interface LocationPermissionProps {
  onLocationGranted?: (location: LocationData) => void;
  onLocationDenied?: () => void;
  onClose?: () => void;
  showCloseButton?: boolean;
  title?: string;
  description?: string;
}

export const LocationPermission: React.FC<LocationPermissionProps> = ({
  onLocationGranted,
  onLocationDenied,
  onClose,
  showCloseButton = true,
  title = "Cho phép truy cập vị trí",
  description = "Để có thể đưa ra gợi ý nhà hàng gần bạn nhất, chúng tôi cần quyền truy cập vị trí của bạn."
}) => {
  const { 
    location, 
    loading, 
    error, 
    permission, 
    requestLocation,
    hasLocation 
  } = useGeolocation();

  React.useEffect(() => {
    if (location && onLocationGranted) {
      onLocationGranted(location);
    }
  }, [location, onLocationGranted]);

  React.useEffect(() => {
    if (permission === 'denied' && onLocationDenied) {
      // Save the denied status to localStorage so we don't ask again
      localStorage.setItem('goodmeal_location_permission', 'denied');
      onLocationDenied();
    }
  }, [permission, onLocationDenied]);

  const handleRequestLocation = () => {
    requestLocation();
  };

  const handleSkip = () => {
    // Save the denied status to localStorage so we don't ask again
    localStorage.setItem('goodmeal_location_permission', 'denied');
    
    if (onLocationDenied) {
      onLocationDenied();
    }
    if (onClose) {
      onClose();
    }
  };

  // Don't show if location is already granted and available
  if (hasLocation) {
    return null;
  }

  return (
    <Card
      style={{
        maxWidth: 400,
        margin: '20px auto',
        backgroundColor: '#262629',
        border: '1px solid rgba(255, 122, 0, 0.2)',
        borderRadius: '12px'
      }}
      bodyStyle={{ padding: '24px' }}
    >
      {showCloseButton && (
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
        />
      )}

      <Space direction="vertical" size="large" style={{ width: '100%', textAlign: 'center' }}>
        <div>
          <EnvironmentOutlined 
            style={{ 
              fontSize: '48px', 
              color: '#ff7a00',
              marginBottom: '16px'
            }} 
          />
          <Title level={4} style={{ color: '#ffffff', margin: 0 }}>
            {title}
          </Title>
        </div>

        <Text style={{ color: '#b3b3b3', textAlign: 'center', display: 'block' }}>
          {description}
        </Text>

        {error && (
          <Alert
            message="Lỗi truy cập vị trí"
            description={error}
            type="error"
            showIcon
            style={{
              backgroundColor: 'rgba(255, 77, 79, 0.1)',
              border: '1px solid rgba(255, 77, 79, 0.2)',
              color: '#ff4d4f'
            }}
          />
        )}

        {permission === 'unavailable' && (
          <Alert
            message="Không hỗ trợ"
            description="Trình duyệt của bạn không hỗ trợ tính năng định vị."
            type="warning"
            showIcon
            style={{
              backgroundColor: 'rgba(250, 173, 20, 0.1)',
              border: '1px solid rgba(250, 173, 20, 0.2)',
              color: '#faad14'
            }}
          />
        )}

        <Space direction="vertical" size="middle" style={{ width: '100%' }}>
          {permission !== 'unavailable' && (
            <Button
              type="primary"
              size="large"
              loading={loading}
              onClick={handleRequestLocation}
              icon={<EnvironmentOutlined />}
              style={{
                width: '100%',
                backgroundColor: '#ff7a00',
                borderColor: '#ff7a00',
                height: '48px',
                fontSize: '16px'
              }}
            >
              {loading ? 'Đang lấy vị trí...' : 'Cho phép truy cập vị trí'}
            </Button>
          )}

          <Button
            type="text"
            size="large"
            onClick={handleSkip}
            style={{
              width: '100%',
              color: '#b3b3b3',
              height: '40px'
            }}
          >
            Bỏ qua (không sử dụng vị trí)
          </Button>
        </Space>

        {permission === 'granted' && location && (
          <Alert
            message="Đã lấy được vị trí"
            description={`Vĩ độ: ${location.latitude.toFixed(6)}, Kinh độ: ${location.longitude.toFixed(6)}`}
            type="success"
            showIcon
            style={{
              backgroundColor: 'rgba(82, 196, 26, 0.1)',
              border: '1px solid rgba(82, 196, 26, 0.2)',
              color: '#52c41a'
            }}
          />
        )}
      </Space>
    </Card>
  );
};

export default LocationPermission;
