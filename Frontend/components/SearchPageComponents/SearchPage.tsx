import React, { useState, useEffect } from 'react';
import {
    Layout,
    Button,
    theme,
    ConfigProvider,
    Spin,
    message,
    Typography
} from 'antd';
import {
    MenuOutlined
} from '@ant-design/icons';
import { onAuthStateChanged } from 'firebase/auth';
import { FirebaseAuth } from '../../firebase/firebase';

// Import components
import Sidebar from './Sidebar';
import SearchHeader from './SearchHeader';
import MainContent from './MainContent';
import InputArea from './InputArea';

// Import services
import { getPromptSessions, formatTime, createPromptSession } from '../../services/PromptService';
import { checkAuthorization } from '../../services/Auth';
import { ChatItem } from './types';


const SearchPage = () => {    const [selectedChat, setSelectedChat] = useState<string | null>(null);
    const [inputMessage, setInputMessage] = useState('');
    const [collapsed, setCollapsed] = useState(false);
    const [chatHistory, setChatHistory] = useState<ChatItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [user, setUser] = useState<any>(null);
    const [userId, setUserId] = useState<string | null>(null);

    // Listen to auth state changes
    useEffect(() => {
        const unsubscribe = onAuthStateChanged(FirebaseAuth, async (firebaseUser) => {            if (firebaseUser) {
                setUser(firebaseUser);
                try {
                    const idToken = await firebaseUser.getIdToken();
                    const authResponse = await checkAuthorization(idToken);
                    console.log('Auth response:', authResponse);
                    if (authResponse?.user) {
                        console.log('User from auth:', authResponse.user);
                        console.log('UserId from auth:', authResponse.user.userId);
                        setUserId(authResponse.user.userId);
                        // Load prompt sessions after getting userId
                        await loadPromptSessions(idToken, authResponse.user.userId);
                    }
                } catch (error) {
                    console.error('Error getting user info:', error);
                    message.error('Failed to load user information');
                } finally {
                    setLoading(false);
                }
            } else {
                setUser(null);
                setUserId(null);
                setChatHistory([]);
                setLoading(false);
            }
        });

        return () => unsubscribe();
    }, []);

    // Load prompt sessions from API
    const loadPromptSessions = async (idToken: string, currentUserId: string) => {
        try {
            setLoading(true);
            const sessions = await getPromptSessions(idToken, currentUserId);
              // Transform sessions to chat history format
            const transformedHistory: ChatItem[] = sessions.map(session => ({
                id: session.id,
                title: `Session ${session.id.substring(0, 8)}`, // Use first 8 chars of ID as title
                preview: 'AI Chat Session...', // Default preview
                time: formatTime(session.createdAt)
            }));

            setChatHistory(transformedHistory);
        } catch (error) {
            console.error('Error loading prompt sessions:', error);
            message.error('Failed to load chat history');
        } finally {
            setLoading(false);
        }
    };

    // Refresh function to reload data
    const refreshChatHistory = async () => {
        if (user && userId) {
            try {
                const idToken = await user.getIdToken();
                await loadPromptSessions(idToken, userId);
            } catch (error) {
                console.error('Error refreshing chat history:', error);
                message.error('Failed to refresh chat history');
            }
        }
    };    // Create new session function
    const handleCreateSession = async () => {
        console.log('handleCreateSession called');
        console.log('Current user:', user);
        console.log('Current userId:', userId);
        
        if (!user) {
            message.error('Please log in to create a new session');
            return;
        }

        if (!userId) {
            message.error('User ID not available');
            return;
        }

        try {
            const idToken = await user.getIdToken();
            console.log('About to create session with userId:', userId);
            const newSession = await createPromptSession(idToken, userId);
            
            if (newSession) {
                message.success('New session created successfully');
                // Set the new session as selected
                setSelectedChat(newSession.id);
                // Refresh chat history to include the new session
                await loadPromptSessions(idToken, userId);
            } else {
                message.error('Failed to create new session');
            }
        } catch (error) {
            console.error('Error creating session:', error);
            message.error('Failed to create new session');
        }
    };// Show loading spinner while fetching data
    if (loading) {
        return (
            <ConfigProvider
                theme={{
                    algorithm: theme.darkAlgorithm,
                    token: {
                        colorPrimary: '#ff7a00',
                    },
                }}
            >
                <div style={{ 
                    display: 'flex', 
                    flexDirection: 'column',
                    justifyContent: 'center', 
                    alignItems: 'center', 
                    height: '100vh',
                    background: 'linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)',
                    color: '#ffffff'
                }}>
                    <Spin size="large" style={{ marginBottom: '16px' }} />
                    <Typography.Text style={{ color: '#ffffff', fontSize: '16px' }}>
                        Loading your chat history...
                    </Typography.Text>
                </div>
            </ConfigProvider>
        );
    }return (
        <ConfigProvider
            theme={{
                algorithm: theme.darkAlgorithm,
                token: {
                    colorPrimary: '#ff7a00',
                    colorBgContainer: '#1f1f23',
                    colorBgElevated: '#262629',
                    colorBorder: '#ff7a0033',
                    colorText: '#ffffff',
                    colorTextSecondary: '#b3b3b3',
                },
            }}
        >
            <Layout style={{ height: '100vh', maxHeight: '100vh', overflow: 'hidden', background: 'linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)' }}>
                {/* Sidebar */}                <Sidebar
                    collapsed={collapsed}
                    setCollapsed={setCollapsed}
                    selectedChat={selectedChat}
                    setSelectedChat={setSelectedChat}
                    chatHistory={chatHistory}
                    onCreateSession={handleCreateSession}
                />
                
                {/* Main Content */}
                <Layout style={{ background: 'linear-gradient(180deg, #1a1a1d 0%, #0f0f12 100%)', height: '100vh', maxHeight: '100vh', overflow: 'hidden', display: 'flex', flexDirection: 'column' }}>
                    {/* Floating Toggle Button when sidebar is collapsed - Updated styling */}
                    {collapsed && (
                        <Button
                            type="text"
                            shape="circle"
                            icon={<MenuOutlined />}
                            onClick={() => setCollapsed(false)}
                            style={{
                                position: 'absolute',
                                top: '16px',
                                left: '16px',
                                zIndex: 1000,
                                backgroundColor: 'transparent',
                                border: 'none',
                                color: '#ffffff',
                                fontSize: '18px',
                                width: '40px',
                                height: '40px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                boxShadow: 'none'
                            }}
                        />
                    )}
                    
                    {/* Header */}
                    <SearchHeader collapsed={collapsed} />
                    
                    {/* Content Area */}
                    <MainContent selectedChat={selectedChat} />                      {/* Input Area */}
                        <InputArea inputMessage={inputMessage} setInputMessage={setInputMessage} />
                </Layout>
            </Layout>
        </ConfigProvider>
    );
};

export default SearchPage;