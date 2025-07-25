import React, { useState, useEffect } from "react";
import {
  Layout,
  Button,
  theme,
  ConfigProvider,
  Spin,
  Typography,
  App,
} from "antd";
import { MenuOutlined } from "@ant-design/icons";
import { onAuthStateChanged } from "firebase/auth";
import { FirebaseAuth } from "../../firebase/firebase";

// Import components
import Sidebar from "./Sidebar";
import ChatArea from "./ChatArea";
import SearchHeader from "./SearchHeader";
import MainContent from "./MainContent";
import LocationPrompt from "./LocationPrompt";
import { useGeolocation, LocationData } from "../../hooks/useGeolocation";
import { locationPromptUtils } from "../../utils/locationPromptUtils";
import { checkAuthorization } from "../../services/Auth";

// Import services
import {
  getPromptSessions,
  createPromptSession,
  formatTime,
  deletePromptSession,
  PromptSession,
  deleteAllPromptSessions,
} from "../../services/PromptService";

// Import types
import { ChatItem } from "./types";

const { Text } = Typography;

interface SearchPageProps {
  initialSessionId?: string;
}

// Inner component that can use useApp hook
const SearchPageContent: React.FC<SearchPageProps> = ({ initialSessionId }) => {
  const { message: messageApi } = App.useApp();
  const [selectedChat, setSelectedChat] = useState<string | null>(null);
  const [collapsed, setCollapsed] = useState(false);
  const [chatHistory, setChatHistory] = useState<ChatItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [isCreatingSession, setIsCreatingSession] = useState(false);
  const [, setNewlyCreatedSessionId] = useState<string | null>(null);
  const [user, setUser] = useState<any>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [showLocationPermission, setShowLocationPermission] = useState(false);
  const [locationPromptKey, setLocationPromptKey] = useState(0); // Force re-render
  const [userLocation, setUserLocation] = useState<LocationData | null>(null);
  const [sessions, setSessions] = useState<PromptSession[]>([]);
  // Use geolocation hook
  const {
    location,
    permission,
    hasLocation,
    
  } = useGeolocation();

  const effectiveSessionName = sessions.find(s => s.id === selectedChat)?.sessionName;

  // Check authentication on mount
  useEffect(() => {
    const unsubscribe = onAuthStateChanged(FirebaseAuth, async (user) => {
             if (user) {
         try {
           const idToken = await user.getIdToken();
           const isAuthorized = await checkAuthorization(idToken);
           if (isAuthorized) {
             setUser(user);
             setUserId(user.uid);
           } else {
             setUser(null);
             setUserId(null);
           }
         } catch (error) {
           console.error("Error checking authorization:", error);
           messageApi.error("Failed to load user information");
           setUser(null);
           setUserId(null);
         }
      } else {
        setUser(null);
        setUserId(null);
      }
      setLoading(false);
    });

    return () => unsubscribe();
  }, [messageApi]);

  // Load sessions when user changes
  useEffect(() => {
    if (user && userId) {
      user.getIdToken().then((idToken: string) => {
        loadPromptSessions(idToken, userId);
      });
    } else {
      setChatHistory([]);
      setSessions([]);
    }
  }, [user, userId]);

  // Handle location permission on mount
  useEffect(() => {
    const locationPermission = localStorage.getItem('goodmeal_location_permission');
    if (locationPermission === 'granted' && hasLocation && location) {
      setUserLocation(location);
         } else if (user && locationPromptUtils.shouldShowPrompt(hasLocation, permission)) {
      setShowLocationPermission(true);
    }
  }, [user, hasLocation, location]);

  // Set initial session from URL parameter
  useEffect(() => {
    if (initialSessionId) {
      setSelectedChat(initialSessionId);
    }
  }, [initialSessionId]);

  // Load prompt sessions from API
  const loadPromptSessions = async (idToken: string, currentUserId: string, selectNewest: boolean = false) => {
    try {
      setLoading(true);
      const sessions = await getPromptSessions(idToken, currentUserId);
      
      // Sort sessions by createdAt descending (newest first)
      const sortedSessions = sessions.sort((a, b) => 
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      );
      setSessions(sortedSessions);
      // Transform sessions to chat history format
      const transformedHistory: ChatItem[] = sortedSessions.map((session) => {
        // const sessionDate = new Date(session.createdAt);
        // const formattedTitle = `Session ${sessionDate.toLocaleDateString('vi-VN')} ${sessionDate.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}`;
        const formattedSessionName = session.sessionName;
        return {
          id: session.id,
          title: formattedSessionName ?? 'New chat',
          time: formatTime(session.createdAt),
        };
      });

      setChatHistory(transformedHistory);
      
      // If requested, select the newest session
      if (selectNewest && transformedHistory.length > 0) {
        const newestSession = transformedHistory[0];
        setSelectedChat(newestSession.id);
        setNewlyCreatedSessionId(newestSession.id);
        window.history.pushState({}, '', `/c/${newestSession.id}`);
        
        setTimeout(() => {
          setNewlyCreatedSessionId(null);
        }, 2000);
      }
      
    } catch (error) {
      console.error("Error loading prompt sessions:", error);
      messageApi.error("Failed to load chat history");
    } finally {
      setLoading(false);
    }
  };

  // Create new session function
  const handleCreateSession = async () => {
    if (!user) {
      messageApi.error("Please log in to create a new session");
      return;
    }

    if (!userId) {
      messageApi.error("User ID not available");
      return;
    }

    try {
      setIsCreatingSession(true);
      const idToken = await user.getIdToken();
      
      console.log('About to create session with userId:', userId);
      const newSession = await createPromptSession(idToken, userId);
      console.log('Created session:', newSession);

      if (newSession && newSession.id) {
        messageApi.success("Đã tạo cuộc trò chuyện mới thành công");

        // Set the selected chat to the new session
        setSelectedChat(newSession.id);
        setNewlyCreatedSessionId(newSession.id);

        // Update URL to show the new session
        window.history.pushState({}, '', `/c/${newSession.id}`);

        // Refresh chat history to show the new session
        await loadPromptSessions(idToken, userId, false);

        // Clear newlyCreatedSessionId after animation
        setTimeout(() => {
          setNewlyCreatedSessionId(null);
        }, 2000);
      } else {
        console.error('No session returned or invalid session:', newSession);
        messageApi.error("Không thể tạo cuộc trò chuyện mới. Vui lòng thử lại.");
      }
    } catch (error) {
      console.error("Error creating new session:", error);
      messageApi.error("Không thể tạo cuộc trò chuyện mới. Vui lòng thử lại.");
    } finally {
      setIsCreatingSession(false);
    }
  };  // Delete session function
  const handleDeleteSession = async (sessionId: string) => {
    if (!user) {
      messageApi.error("Please log in to delete a session");
      return;
    }

    try {
      const idToken = await user.getIdToken();
      const success = await deletePromptSession(idToken, sessionId);

      if (success) {
        messageApi.success("Session deleted successfully");

        // If the deleted session was selected, clear selection
        if (selectedChat === sessionId) {
          setSelectedChat(null);
          window.history.pushState({}, '', '/c');
        }

        // Refresh chat history to remove the deleted session
        if (userId) {
          await loadPromptSessions(idToken, userId);
        }
      } else {
        messageApi.error("Failed to delete session");
      }
    } catch (error) {
      console.error("Error deleting session:", error);
      messageApi.error("Failed to delete session");
    }
  };
  
  // Delete all sessions function
  const handleDeleteAllSessions = async () => {
    if (!user) {
      messageApi.error("Vui lòng đăng nhập để xóa cuộc trò chuyện");
      return;
    }

    try {
      const idToken = await user.getIdToken();
      const success = await deleteAllPromptSessions(idToken);

      if (success) {
        messageApi.success("Đã xóa tất cả cuộc trò chuyện thành công");

        // Clear current selection since all sessions are deleted
        setSelectedChat(null);
        window.history.pushState({}, '', '/c');

        // Refresh chat history to show empty state
        if (userId) {
          await loadPromptSessions(idToken, userId);
        }
      } else {
        messageApi.error("Không thể xóa cuộc trò chuyện");
      }
    } catch (error) {
      console.error("Error deleting all sessions:", error);
      messageApi.error("Không thể xóa cuộc trò chuyện");
    }
  };

  // Location permission handlers
  const handleLocationGranted = (location: LocationData, rememberChoice: boolean) => {
    setUserLocation(location);
    setShowLocationPermission(false);
    
    if (rememberChoice) {
      locationPromptUtils.setStatus('always_granted');
    } else {
      locationPromptUtils.clearStatus(); // Will ask again after logout
    }
    
    // Always save the actual location permission
    localStorage.setItem('goodmeal_location_permission', 'granted');
    messageApi.success("Đã cập nhật vị trí của bạn để có gợi ý chính xác hơn!");
  };

  const handleLocationDenied = (rememberChoice: boolean) => {
    setShowLocationPermission(false);
    
    if (rememberChoice) {
      locationPromptUtils.setStatus('always_denied');
      localStorage.setItem('goodmeal_location_permission', 'denied');
      messageApi.info("Đã lưu lựa chọn. Bạn có thể thay đổi trong cài đặt trình duyệt.");
    } else {
      locationPromptUtils.setStatus('once_denied');
      messageApi.info("Sẽ hỏi lại sau 24 giờ. Bạn có thể bật quyền truy cập vị trí bất cứ lúc nào.");
    }
  };

  const handleShowLocationPermission = () => {
    // Reset prompt and show again
    locationPromptUtils.clearStatus();
    setShowLocationPermission(true);
    setLocationPromptKey(prev => prev + 1); // Force re-render
  };

  // Handle first stream title for new sessions
  const handleFirstStreamTitle = (title: string) => {
    if (selectedChat && title) {
      setSessions(prevSessions =>
        prevSessions.map(session =>
          session.id === selectedChat && (!session.sessionName || session.sessionName === "newchat")
            ? { ...session, sessionName: title }
            : session
        )
      );

      setChatHistory(prevHistory =>
        prevHistory.map(chat =>
          chat.id === selectedChat && (chat.title === "New chat" || chat.title === "newchat")
            ? { ...chat, title: title }
            : chat
        )
      );
    }
  };

  if (loading) {
    return (
      <ConfigProvider
        theme={{
          algorithm: theme.darkAlgorithm,
          token: {
            colorPrimary: "#ff7a00",
            colorBgContainer: "#1f1f23",
            colorBgElevated: "#262629",
            colorBorder: "#ff7a0033",
            colorText: "#ffffff",
            colorTextSecondary: "#b3b3b3",
          },
        }}
      >
        <div
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            height: "100vh",
            background:
              "linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)",
          }}
        >
          <Spin size="large" />
          <Text style={{ marginLeft: "16px", color: "#ffffff" }}>
            Đang tải...
          </Text>
        </div>
      </ConfigProvider>
    );
  }
  return (
      <Layout
        style={{
          height: "100vh",
          maxHeight: "100vh",
          overflow: "hidden",
          background:
            "linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)",
        }}
      >
        {/* Sidebar */}{" "}                <Sidebar
                    collapsed={collapsed}
                    setCollapsed={setCollapsed}
                    selectedChat={selectedChat}
                    setSelectedChat={setSelectedChat}
                    chatHistory={chatHistory}
                    onCreateSession={handleCreateSession}
                    isCreatingSession={isCreatingSession}
                    onDeleteSession={handleDeleteSession}
                    onDeleteAllSessions={handleDeleteAllSessions}
                />
        {/* Main Content */}
        <Layout
          style={{
            background: "linear-gradient(180deg, #1a1a1d 0%, #0f0f12 100%)",
            height: "100vh",
            maxHeight: "100vh",
            overflow: "hidden",
            display: "flex",
            flexDirection: "column",
          }}
        >
          {/* Floating Toggle Button when sidebar is collapsed - Updated styling */}
          {collapsed && (
            <Button
              type="text"
              shape="circle"
              icon={<MenuOutlined />}
              onClick={() => setCollapsed(false)}
              style={{
                position: "absolute",
                top: "16px",
                left: "16px",
                zIndex: 1000,
                backgroundColor: "transparent",
                border: "none",
                color: "#ffffff",
                fontSize: "18px",
                width: "40px",
                height: "40px",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                boxShadow: "none",
              }}
            />
          )}

          {/* Header */}
          <SearchHeader collapsed={collapsed} sessionName={effectiveSessionName} />

          {/* Content Area with Chat */}
          <div style={{ flex: 1, overflow: "hidden" }}>
            <ChatArea 
              sessionId={selectedChat || undefined} 
              userLocation={userLocation}
              onRequestLocation={handleShowLocationPermission}
              onFirstStreamTitle={handleFirstStreamTitle}
            />
        
          </div>
        </Layout>

        {/* Location Prompt */}
        {showLocationPermission && (
          <LocationPrompt
            key={locationPromptKey}
            onLocationGranted={handleLocationGranted}
            onLocationDenied={handleLocationDenied}
            onClose={() => setShowLocationPermission(false)}
          />
        )}
      </Layout>
  );
};

// Main component that provides App context
const SearchPage: React.FC<SearchPageProps> = ({ initialSessionId }) => {
  return (
    <ConfigProvider
      theme={{
        algorithm: theme.darkAlgorithm,
        token: {
          colorPrimary: "#ff7a00",
          colorBgContainer: "#1f1f23",
          colorBgElevated: "#262629",
          colorBorder: "#ff7a0033",
          colorText: "#ffffff",
          colorTextSecondary: "#b3b3b3",
        },
      }}
    >
      <App>
        <SearchPageContent initialSessionId={initialSessionId} />
      </App>
    </ConfigProvider>
  );
};

export default SearchPage;
