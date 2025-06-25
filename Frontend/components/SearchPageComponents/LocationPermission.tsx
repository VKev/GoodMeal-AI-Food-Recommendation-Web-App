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

  const [isRequesting, setIsRequesting] = React.useState(false);
  const [showHelp, setShowHelp] = React.useState(false);

  React.useEffect(() => {
    if (location && onLocationGranted && !hasLocation) {
      // Save granted status before calling callback
      localStorage.setItem('goodmeal_location_permission', 'granted');
      onLocationGranted(location);
    }
  }, [location, onLocationGranted, hasLocation]);

  React.useEffect(() => {
    if (permission === 'denied' && onLocationDenied && isRequesting) {
      // Add a small delay to prevent immediate close
      setTimeout(() => {
        // Save the denied status to localStorage so we don't ask again
        localStorage.setItem('goodmeal_location_permission', 'denied');
        setShowHelp(true); // Show help instead of immediately closing
      }, 500);
    }
  }, [permission, onLocationDenied, isRequesting]);

  const handleRequestLocation = () => {
    console.log('=== LOCATION REQUEST DEBUG ===');
    console.log('Current permission:', permission);
    console.log('Navigator geolocation available:', !!navigator.geolocation);
    console.log('Is HTTPS:', window.location.protocol === 'https:');
    console.log('Current URL:', window.location.href);
    
    // Test direct geolocation API
    if (navigator.geolocation) {
      console.log('Testing direct geolocation call...');
      navigator.geolocation.getCurrentPosition(
        (position) => {
          console.log('Direct geolocation SUCCESS:', position);
        },
        (error) => {
          console.log('Direct geolocation ERROR:', error);
        },
        { timeout: 5000 }
      );
    }
    console.log('===============================');
    
    setIsRequesting(true);
    setShowHelp(false);
    requestLocation();
    
    // Show help message after 3 seconds if no response
    setTimeout(() => {
      if (!location && !error && isRequesting) {
        setShowHelp(true);
      }
    }, 3000);
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

  const handleCloseHelp = () => {
    setShowHelp(false);
    if (onLocationDenied) {
      onLocationDenied();
    }
    if (onClose) {
      onClose();
    }
  };

  // Don't show if location is already granted and available
  if (hasLocation && !showHelp) {
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

        {showHelp && (
          <Alert
            message="Cần bật quyền truy cập vị trí"
            description={
              <div>
                <p>Vui lòng:</p>
                <ol style={{ paddingLeft: '16px', margin: '8px 0' }}>
                  <li>Nhấn vào biểu tượng khóa 🔒 bên cạnh URL</li>
                  <li>Chọn "Cho phép" cho vị trí</li>
                  <li>Tải lại trang và thử lại</li>
                </ol>
                <p>Hoặc kiểm tra cài đặt trình duyệt → Quyền riêng tư → Vị trí</p>
              </div>
            }
            type="info"
            showIcon
            closable
            onClose={handleCloseHelp}
            style={{
              backgroundColor: 'rgba(24, 144, 255, 0.1)',
              border: '1px solid rgba(24, 144, 255, 0.2)',
              color: '#1890ff'
            }}
          />
        )}

        {isRequesting && !error && !location && (
          <Alert
            message="Đang chờ phản hồi..."
            description="Vui lòng kiểm tra popup xin phép truy cập vị trí từ trình duyệt (thường ở góc trên bên trái hoặc trong thanh địa chỉ)."
            type="warning"
            showIcon
            style={{
              backgroundColor: 'rgba(250, 173, 20, 0.1)',
              border: '1px solid rgba(250, 173, 20, 0.2)',
              color: '#faad14'
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
          {permission !== 'unavailable' && !showHelp && (
            <Button
              type="primary"
              size="large"
              loading={loading || isRequesting}
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
              {loading || isRequesting 
                ? 'Đang lấy vị trí...' 
                : 'Cho phép truy cập vị trí'}
            </Button>
          )}

          {showHelp && (
            <Button
              type="primary"
              size="large"
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
              Thử lại
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
