import React, { useState, useRef, useEffect } from 'react';
import { Input, Button, message, Spin, Avatar, Tooltip } from 'antd';
import { SendOutlined, UserOutlined, RobotOutlined, EnvironmentOutlined } from '@ant-design/icons';
import { processFoodRequest, ProcessFoodRequestPayload, getSessionMessages } from '../../services/PromptService';
import { useAuth } from '../../hooks/auths/authContext';
import { useRouter } from 'next/navigation';
import { LocationData } from '../../hooks/useGeolocation';

const { TextArea } = Input;

interface Message {
    id: string;
    type: 'user' | 'bot';
    content: string;
    imageUrl?: string;
    imageUrls?: string[]; // For multiple images
    foods?: any[]; // For food list display
    timestamp: Date;
}

interface ChatAreaProps {
    sessionId?: string;
    userLocation?: LocationData | null;
    onRequestLocation?: () => void;
}

export const ChatArea: React.FC<ChatAreaProps> = ({ 
    sessionId, 
    userLocation, 
    onRequestLocation 
}) => {
    const [messages, setMessages] = useState<Message[]>([]);
    const [inputValue, setInputValue] = useState('');
    const [loading, setLoading] = useState(false);
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const { currentUser, authUser } = useAuth();
    const router = useRouter();

    const handleFoodClick = (foodName: string, location?: string) => {
        console.log('=== FOOD CLICK DEBUG ===');
        console.log('foodName:', foodName);
        console.log('food location from API:', location);
        console.log('userLocation from props:', userLocation);
        console.log('========================');
        
        // Navigate to restaurants page with food name and location as search query
        let url = `/restaurants?search=${encodeURIComponent(foodName)}`;
        
        // Use API location first, then fallback to user location
        if (location && location !== 'null' && location.trim()) {
            url += `&location=${encodeURIComponent(location)}`;
            console.log('Using API location:', location);
        } else if (userLocation) {
            // If no API location, use user's coordinates
            url += `&lat=${userLocation.latitude}&lng=${userLocation.longitude}`;
            console.log('Using user coordinates:', userLocation.latitude, userLocation.longitude);
        }
        
        console.log('Final URL:', url);
        router.push(url);
    };

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages]);

    // Load messages when sessionId changes
    useEffect(() => {
        const loadMessages = async () => {
            if (!sessionId || !currentUser) return;

            try {
                const idToken = await currentUser.getIdToken();
                const sessionMessages = await getSessionMessages(idToken, sessionId);
                
                // Convert API messages to our Message format
                const convertedMessages: Message[] = [];
                
                for (const apiMsg of sessionMessages) {
                    // Add user message
                    convertedMessages.push({
                        id: `user-${apiMsg.id}`,
                        type: 'user',
                        content: apiMsg.promptMessage,
                        timestamp: new Date(apiMsg.createdAt)
                    });
                    
                    // Add bot response if exists
                    if (apiMsg.responseMessage) {
                        let responseText = apiMsg.responseMessage;
                        let imageUrl = undefined;
                        let foods: any[] = [];
                        
                        // Parse escaped JSON response
                        try {
                            if (responseText.startsWith('"') && responseText.endsWith('"')) {
                                responseText = JSON.parse(responseText);
                            }
                            
                            if (typeof responseText === 'string') {
                                const parsedResponse = JSON.parse(responseText);
                                
                                // Check for new format with multiple foods
                                if (parsedResponse.Foods && parsedResponse.Foods.length > 0) {
                                    if (parsedResponse.Foods.length === 1) {
                                        // Single food - old format
                                        const food = parsedResponse.Foods[0];
                                        responseText = `**${food.FoodName}** (${food.National})\n\n${food.Description}`;
                                        imageUrl = food.ImageUrl;
                                    } else {
                                        // Multiple foods - new format
                                        responseText = `**${parsedResponse.Title || 'Gợi ý món ăn'}**`;
                                        const location = parsedResponse.Location;
                                        foods = parsedResponse.Foods.map((food: any) => ({
                                            foodName: food.FoodName,
                                            national: food.National,
                                            description: food.Description,
                                            imageUrl: food.ImageUrl,
                                            location: location
                                        }));
                                    }
                                } else if (parsedResponse.Title) {
                                    responseText = parsedResponse.Title;
                                } else {
                                    responseText = 'No specific food information found.';
                                }
                            }
                        } catch (parseError) {
                            console.error('Error parsing historical message:', parseError);
                            // Keep original response if parsing fails
                        }
                        
                        convertedMessages.push({
                            id: `bot-${apiMsg.id}`,
                            type: 'bot',
                            content: responseText,
                            imageUrl,
                            foods: foods.length > 0 ? foods : undefined,
                            timestamp: new Date(apiMsg.createdAt)
                        });
                    }
                }
                
                // Sort by timestamp
                convertedMessages.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());
                setMessages(convertedMessages);
                
            } catch (error) {
                console.error('Error loading session messages:', error);
                // Don't show error message to user, just log it
            }
        };

        // Clear messages when sessionId changes
        setMessages([]);
        loadMessages();
    }, [sessionId, currentUser]);

    const handleSendMessage = async () => {
        if (!inputValue.trim() || !sessionId || !currentUser || !authUser) {
            message.error('Please enter a message and make sure you are logged in');
            return;
        }

        const userMessage: Message = {
            id: Date.now().toString(),
            type: 'user',
            content: inputValue.trim(),
            timestamp: new Date()
        };

        // Add user message immediately
        setMessages(prev => [...prev, userMessage]);
        const currentInput = inputValue.trim();
        setInputValue('');
        setLoading(true);

        try {
            // Get fresh token
            const idToken = await currentUser.getIdToken();
            
            const payload: ProcessFoodRequestPayload = {
                promptSessionId: sessionId,
                sender: authUser.userId || 'anonymous',
                promptMessage: currentInput,
                responseMessage: '', // This will be filled by the API
                // Include location data if available
                ...(userLocation && {
                    lat: userLocation.latitude,
                    lng: userLocation.longitude
                })
            };

            // Debug logs
            console.log('=== DEBUG LOCATION DATA ===');
            console.log('userLocation:', userLocation);
            console.log('payload with location:', payload);
            console.log('payload.lat:', payload.lat);
            console.log('payload.lng:', payload.lng);
            console.log('===========================');

            const response = await processFoodRequest(idToken, payload);
            
            console.log('Full API response:', response); // Debug log

            if (response) {
                // Extract response message with better fallback
                let responseText = '';
                let imageUrls: string[] = [];
                let foods: any[] = [];
                
                if (response.responseMessage) {
                    responseText = response.responseMessage;
                } else if (response.finalResponse) {
                    // Handle the new finalResponse format
                    const finalResp = response.finalResponse;
                    
                    if (finalResp.title) {
                        responseText = `**${finalResp.title}**`;
                    }
                    
                    if (finalResp.foods && finalResp.foods.length > 0) {
                        foods = finalResp.foods.map((food: any) => ({
                            ...food,
                            location: finalResp.location
                        }));
                        finalResp.foods.forEach((food: any) => {
                            if (food.imageUrl) {
                                imageUrls.push(food.imageUrl);
                            }
                        });
                    }
                } else {
                    // If no responseMessage or finalResponse, try to display the whole response nicely
                    responseText = `Response received: ${JSON.stringify(response, null, 2)}`;
                    console.warn('No responseMessage or finalResponse found in response, showing full response');
                }
                
                const botMessage: Message = {
                    id: (Date.now() + 1).toString(),
                    type: 'bot',
                    content: responseText,
                    imageUrl: response.imageUrl,
                    imageUrls: imageUrls.length > 0 ? imageUrls : undefined,
                    foods: foods.length > 0 ? foods : undefined,
                    timestamp: new Date()
                };

                setMessages(prev => [...prev, botMessage]);
            } else {
                throw new Error('Failed to get response from AI');
            }
        } catch (error) {
            console.error('Error sending message:', error);
            message.error('Failed to send message. Please try again.');
            
            // Add error message
            const errorMessage: Message = {
                id: (Date.now() + 1).toString(),
                type: 'bot',
                content: 'Sorry, I encountered an error processing your request. Please try again.',
                timestamp: new Date()
            };
            setMessages(prev => [...prev, errorMessage]);
        } finally {
            setLoading(false);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            handleSendMessage();
        }
    };

    if (!sessionId) {
        return (
            <div style={{ 
                display: 'flex', 
                alignItems: 'center', 
                justifyContent: 'center', 
                height: '100%',
                color: '#b3b3b3'
            }}>
                Hãy nhấn dấu vào + để tạo mới một đoạn chat
            </div>
        );
    }

    return (
        <div style={{ 
            display: 'flex', 
            flexDirection: 'column', 
            height: '100%',
            backgroundColor: 'transparent'
        }}>
            {/* Messages Area */}
            <div style={{ 
                flex: 1, 
                overflowY: 'auto', 
                padding: '20px',
                display: 'flex',
                flexDirection: 'column',
                gap: '16px'
            }}>
                {messages.map((msg) => (
                    <div key={msg.id} style={{
                        display: 'flex',
                        alignItems: 'flex-start',
                        gap: '12px',
                        flexDirection: msg.type === 'user' ? 'row-reverse' : 'row'
                    }}>
                        <Avatar 
                            icon={msg.type === 'user' ? <UserOutlined /> : <RobotOutlined />}
                            style={{ 
                                backgroundColor: msg.type === 'user' ? '#ff7a00' : '#52c41a',
                                flexShrink: 0
                            }}
                        />
                        <div style={{
                            maxWidth: '70%',
                            padding: '12px 16px',
                            borderRadius: '12px',
                            backgroundColor: msg.type === 'user' ? '#ff7a00' : '#262629',
                            color: '#ffffff',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.15)',
                            border: `1px solid ${msg.type === 'user' ? '#ff7a00' : 'rgba(255, 122, 0, 0.2)'}`,
                            textAlign: msg.type === 'user' ? 'right' : 'left'
                        }}>
                            <div style={{ marginBottom: (msg.imageUrl || msg.foods) ? '8px' : '0' }}>
                                {/* Render message with basic markdown-like formatting */}
                                {msg.content.split('\n').map((line, index) => {
                                    if (line.startsWith('**') && line.endsWith('**')) {
                                        // Bold text
                                        return (
                                            <div key={index} style={{ fontWeight: 'bold', marginBottom: '8px', fontSize: '16px' }}>
                                                {line.replace(/\*\*/g, '')}
                                            </div>
                                        );
                                    }
                                    return (
                                        <div key={index} style={{ marginBottom: line.trim() ? '4px' : '8px' }}>
                                            {line}
                                        </div>
                                    );
                                })}
                            </div>
                            
                            {/* Render food list if available */}
                            {msg.foods && msg.foods.length > 0 && (
                                <div style={{ marginTop: '12px' }}>
                                    {msg.foods.map((food: any, index: number) => (
                                        <div key={index} style={{
                                            marginBottom: '16px',
                                            padding: '12px',
                                            backgroundColor: 'rgba(255, 122, 0, 0.1)',
                                            borderRadius: '8px',
                                            border: '1px solid rgba(255, 122, 0, 0.2)'
                                        }}>
                                            <div style={{ display: 'flex', gap: '12px', alignItems: 'flex-start' }}>
                                                {food.imageUrl && (
                                                    <div 
                                                        style={{ 
                                                            flexShrink: 0,
                                                            cursor: 'pointer',
                                                            transition: 'transform 0.2s, box-shadow 0.2s'
                                                        }}
                                                        onClick={() => handleFoodClick(food.foodName, food.location)}
                                                        onMouseEnter={(e) => {
                                                            e.currentTarget.style.transform = 'scale(1.05)';
                                                            e.currentTarget.style.boxShadow = '0 4px 12px rgba(255, 122, 0, 0.3)';
                                                        }}
                                                        onMouseLeave={(e) => {
                                                            e.currentTarget.style.transform = 'scale(1)';
                                                            e.currentTarget.style.boxShadow = 'none';
                                                        }}
                                                        title={`Click to find restaurants serving ${food.foodName}${food.location && food.location !== 'null' ? ` in ${food.location}` : ''}`}
                                                    >
                                                        <img 
                                                            src={food.imageUrl}
                                                            alt={food.foodName}
                                                            style={{
                                                                width: '80px',
                                                                height: '80px',
                                                                objectFit: 'cover',
                                                                borderRadius: '6px',
                                                                border: '2px solid transparent'
                                                            }}
                                                            onError={(e) => {
                                                                console.error('Failed to load food image:', food.imageUrl);
                                                                e.currentTarget.style.display = 'none';
                                                            }}
                                                        />
                                                    </div>
                                                )}
                                                <div style={{ flex: 1 }}>
                                                    <div 
                                                        style={{ 
                                                            fontWeight: 'bold', 
                                                            color: '#ff7a00',
                                                            marginBottom: '4px',
                                                            fontSize: '14px',
                                                            cursor: 'pointer'
                                                        }}
                                                        onClick={() => handleFoodClick(food.foodName, food.location)}
                                                        title={`Click to find restaurants serving ${food.foodName}${food.location && food.location !== 'null' ? ` in ${food.location}` : ''}`}
                                                    >
                                                        {index + 1}. {food.foodName} ({food.national})
                                                    </div>
                                                    <div style={{ 
                                                        fontSize: '12px',
                                                        lineHeight: '1.4',
                                                        color: '#e6e6e6'
                                                    }}>
                                                        {food.description}
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            )}
                            
                            {/* Single image display (for backward compatibility) */}
                            {msg.imageUrl && !msg.foods && (
                                <div 
                                    style={{ 
                                        marginTop: '8px',
                                        cursor: 'pointer',
                                        transition: 'transform 0.2s'
                                    }}
                                    onClick={() => {
                                        // Try to extract food name from message content for single food
                                        const lines = msg.content.split('\n');
                                        const titleLine = lines.find(line => line.startsWith('**') && line.endsWith('**'));
                                        if (titleLine) {
                                            const foodName = titleLine.replace(/\*\*/g, '').split('(')[0].trim();
                                            handleFoodClick(foodName);
                                        }
                                    }}
                                    onMouseEnter={(e) => {
                                        e.currentTarget.style.transform = 'scale(1.02)';
                                    }}
                                    onMouseLeave={(e) => {
                                        e.currentTarget.style.transform = 'scale(1)';
                                    }}
                                    title="Click to find restaurants"
                                >
                                    <img 
                                        src={msg.imageUrl} 
                                        alt="Generated food" 
                                        style={{ 
                                            maxWidth: '100%', 
                                            borderRadius: '8px',
                                            display: 'block'
                                        }}
                                        onError={(e) => {
                                            console.error('Failed to load image:', msg.imageUrl);
                                            e.currentTarget.style.display = 'none';
                                        }}
                                    />
                                </div>
                            )}
                            <div style={{
                                fontSize: '11px',
                                opacity: 0.7,
                                marginTop: '4px'
                            }}>
                                {msg.timestamp.toLocaleTimeString()}
                            </div>
                        </div>
                    </div>
                ))}
                {loading && (
                    <div style={{ display: 'flex', alignItems: 'center', gap: '12px' }}>
                        <Avatar 
                            icon={<RobotOutlined />}
                            style={{ backgroundColor: '#52c41a' }}
                        />
                        <div style={{ 
                            padding: '12px 16px',
                            backgroundColor: '#262629',
                            border: '1px solid rgba(255, 122, 0, 0.2)',
                            borderRadius: '12px',
                            boxShadow: '0 2px 8px rgba(0,0,0,0.15)'
                        }}>
                            <Spin size="small" style={{ color: '#ffffff' }} /> 
                            <span style={{ color: '#ffffff', marginLeft: '8px' }}>Thinking...</span>
                        </div>
                    </div>
                )}
                <div ref={messagesEndRef} />
            </div>

            {/* Input Area */}
            <div style={{ 
                padding: '20px',
                borderTop: '1px solid rgba(255, 122, 0, 0.2)',
                backgroundColor: '#1f1f23'
            }}>
                {/* Location Status */}
                <div style={{ 
                    marginBottom: '12px',
                    display: 'flex',
                    alignItems: 'center',
                    gap: '8px'
                }}>
                    <Tooltip title={userLocation ? 
                        `Vị trí hiện tại: ${userLocation.latitude.toFixed(4)}, ${userLocation.longitude.toFixed(4)}` : 
                        'Chưa có quyền truy cập vị trí - Click để cấp quyền'
                    }>
                        <Button
                            size="small"
                            type={userLocation ? 'primary' : 'default'}
                            icon={<EnvironmentOutlined />}
                            onClick={userLocation ? undefined : onRequestLocation}
                            style={{
                                backgroundColor: userLocation ? '#52c41a' : 'transparent',
                                borderColor: userLocation ? '#52c41a' : 'rgba(255, 122, 0, 0.3)',
                                color: userLocation ? '#ffffff' : '#b3b3b3',
                                fontSize: '12px',
                                height: '24px',
                                cursor: userLocation ? 'default' : 'pointer'
                            }}
                        >
                            {userLocation ? 'Vị trí đã bật' : 'Bật vị trí'}
                        </Button>
                    </Tooltip>
                    {userLocation && (
                        <Button
                            size="small"
                            type="text"
                            onClick={() => {
                                // Clear location and ask again
                                localStorage.removeItem('goodmeal_location_permission');
                                localStorage.removeItem('goodmeal_location_data');
                                window.location.reload();
                            }}
                            style={{
                                fontSize: '12px',
                                height: '24px',
                                color: '#b3b3b3',
                                padding: '0 8px'
                            }}
                        >
                            Đặt lại
                        </Button>
                    )}
                    {userLocation && (
                        <span style={{ 
                            fontSize: '12px', 
                            color: '#b3b3b3' 
                        }}>
                            Kết quả sẽ tập trung vào khu vực gần bạn
                        </span>
                    )}
                </div>

                <div style={{ display: 'flex', gap: '8px', alignItems: 'flex-end' }}>
                    <TextArea
                        value={inputValue}
                        onChange={(e) => setInputValue(e.target.value)}
                        onKeyPress={handleKeyPress}
                        placeholder="Nhập yêu cầu món ăn của bạn ở dây"
                        autoSize={{ minRows: 1, maxRows: 4 }}
                        style={{ 
                            flex: 1,
                            backgroundColor: '#262629',
                            borderColor: 'rgba(255, 122, 0, 0.3)',
                            color: '#ffffff'
                        }}
                        disabled={loading}
                    />
                    <Button
                        type="primary"
                        icon={<SendOutlined />}
                        onClick={handleSendMessage}
                        loading={loading}
                        disabled={!inputValue.trim()}
                        style={{ 
                            alignSelf: 'flex-end',
                            backgroundColor: '#ff7a00',
                            borderColor: '#ff7a00'
                        }}
                    >
                        Gửi
                    </Button>
                </div>
            </div>
        </div>
    );
};

export default ChatArea;
