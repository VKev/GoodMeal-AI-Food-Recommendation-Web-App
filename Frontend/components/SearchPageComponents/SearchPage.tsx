import React, { useState, useEffect } from "react";
import {
  Layout,
  Button,
  theme,
  ConfigProvider,
  Spin,
  message,
  Typography,
} from "antd";
import { MenuOutlined } from "@ant-design/icons";
import { onAuthStateChanged } from "firebase/auth";
import { FirebaseAuth } from "../../firebase/firebase";

// Import components
import Sidebar from "./Sidebar";
import SearchHeader from "./SearchHeader";
import ChatArea from "./ChatArea";
import LocationPrompt from "./LocationPrompt";

// Import hooks
import { useGeolocation, LocationData } from "../../hooks/useGeolocation";

// Import services
import {
  getPromptSessions,
  formatTime,
  createPromptSession,
  deletePromptSession,
  PromptSession,
} from "../../services/PromptService";
import { checkAuthorization } from "../../services/Auth";
import { ChatItem } from "./types";
import locationPromptUtils from "../../utils/locationPromptUtils";

interface SearchPageProps {
  initialSessionId?: string;
}

const SearchPage: React.FC<SearchPageProps> = ({ initialSessionId }) => {
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

  // Listen to auth state changes
  useEffect(() => {
    const unsubscribe = onAuthStateChanged(
      FirebaseAuth,
      async (firebaseUser) => {
        if (firebaseUser) {
          setUser(firebaseUser);
          try {
            const idToken = await firebaseUser.getIdToken();
            const authResponse = await checkAuthorization(idToken);
            console.log("Auth response:", authResponse);
            if (authResponse?.user) {
              console.log("User from auth:", authResponse.user);
              console.log("UserId from auth:", authResponse.user.userId);
              setUserId(authResponse.user.userId);
              // Load prompt sessions after getting userId
              await loadPromptSessions(idToken, authResponse.user.userId);
            }
          } catch (error) {
            console.error("Error getting user info:", error);
            message.error("Failed to load user information");
          } finally {
            setLoading(false);
          }
        } else {
          setUser(null);
          setUserId(null);
          setChatHistory([]);
          setUserLocation(null);
          setShowLocationPermission(false);
          
          // Handle logout - reset "once" choices but keep "always" choices
          locationPromptUtils.handleLogout();
          
          setLoading(false);
        }
      }
    );

    return () => unsubscribe();
  }, []); // Remove permission and hasLocation dependencies to prevent unnecessary re-runs

  // Update user location when geolocation changes
  useEffect(() => {
    if (location) {
      setUserLocation(location);
    }
  }, [location]);

  // Handle location permission modal display
  useEffect(() => {
    if (user && userId && permission !== 'unavailable') {
      console.log('=== LOCATION PROMPT CHECK ===');
      console.log('Has location:', hasLocation);
      console.log('Permission:', permission);
      console.log('Should show prompt:', locationPromptUtils.shouldShowPrompt(hasLocation, permission));
      console.log('=============================');
      
      if (locationPromptUtils.shouldShowPrompt(hasLocation, permission)) {
        const timer = setTimeout(() => {
          setShowLocationPermission(true);
        }, 2000); // Delay to let user see the interface first
        
        return () => clearTimeout(timer);
      }
    }
  }, [user, userId, permission, hasLocation]);

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
      message.error("Failed to load chat history");
    } finally {
      setLoading(false);
    }
  };

 // Create new session function
  const handleCreateSession = async () => {
    console.log("handleCreateSession called");
    console.log("Current user:", user);
    console.log("Current userId:", userId);

    if (!user) {
      message.error("Please log in to create a new session");
      return;
    }

    if (!userId) {
      message.error("User ID not available");
      return;
    }

    try {
      setIsCreatingSession(true);
      const idToken = await user.getIdToken();
      console.log("About to create session with userId:", userId);
      const newSession = await createPromptSession(idToken, userId);
      
      console.log("New session response:", newSession);
      console.log("New session ID:", newSession?.id);
      console.log("All properties of newSession:", Object.keys(newSession || {}));
      
      // Try different possible ID field names
      const sessionData = newSession as any;
      const sessionId = sessionData?.id || sessionData?.Id || sessionData?.sessionId || sessionData?.promptSessionId;
      console.log("Extracted session ID:", sessionId);

      if (newSession && sessionId) {
        message.success("New session created successfully");
        
        // Set the new session as selected immediately (smooth transition)
        setSelectedChat(sessionId);
        setNewlyCreatedSessionId(sessionId);
        
        // Update URL without navigation (no page reload)
        window.history.pushState({}, '', `/c/${sessionId}`);
        
        // Refresh chat history to include the new session
        await loadPromptSessions(idToken, userId);
        
        // Clear the newly created indicator after animation
        setTimeout(() => {
          setNewlyCreatedSessionId(null);
        }, 2000);
      } else if (newSession && (newSession as any).isSuccess === true) {
        // API returned success but no session data - refresh list and select newest
        console.log("Session created but no ID returned, refreshing list...");
        message.success("New session created successfully");
        
        // Create a temporary session ID to avoid /c/undefined
        const tempSessionId = 'temp-' + Date.now();
        setSelectedChat(tempSessionId);
        window.history.pushState({}, '', `/c/${tempSessionId}`);
        
        // Refresh chat history and auto-select newest session
        await loadPromptSessions(idToken, userId, true);
      } else {
        console.error("Failed to create session - no ID returned:", newSession);
        message.error("Failed to create new session - no session ID returned");
      }
    } catch (error) {
      console.error("Error creating session:", error);
      message.error("Failed to create new session");
    } finally {
      setIsCreatingSession(false);
    }
  };  // Delete session function
  const handleDeleteSession = async (sessionId: string) => {
    if (!user) {
      message.error("Please log in to delete a session");
      return;
    }

    try {
      const idToken = await user.getIdToken();
      const success = await deletePromptSession(idToken, sessionId);

      if (success) {
        message.success("Session deleted successfully");

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
        message.error("Failed to delete session");
      }
    } catch (error) {
      console.error("Error deleting session:", error);
      message.error("Failed to delete session");
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
    message.success("Đã cập nhật vị trí của bạn để có gợi ý chính xác hơn!");
  };

  const handleLocationDenied = (rememberChoice: boolean) => {
    setShowLocationPermission(false);
    
    if (rememberChoice) {
      locationPromptUtils.setStatus('always_denied');
      localStorage.setItem('goodmeal_location_permission', 'denied');
      message.info("Đã lưu lựa chọn. Bạn có thể thay đổi trong cài đặt trình duyệt.");
    } else {
      locationPromptUtils.setStatus('once_denied');
      message.info("Sẽ hỏi lại sau 24 giờ. Bạn có thể bật quyền truy cập vị trí bất cứ lúc nào.");
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
          chat.id === selectedChat
            ? { ...chat, title }
            : chat
        )
      );
    }
  };

  // Use only currentSession.sessionName for the header
  const currentSession = sessions.find(s => s.id === selectedChat);
  const effectiveSessionName = currentSession?.sessionName;
  
  console.log("📋 Session Debug:", {
    selectedChat,
    currentSession: currentSession?.sessionName,
    effectiveSessionName
  });

  // Show loading spinner while fetching data
  if (loading) {
    return (
      <ConfigProvider
        theme={{
          algorithm: theme.darkAlgorithm,
          token: {
            colorPrimary: "#ff7a00",
          },
        }}
      >
        <div
          style={{
            display: "flex",
            flexDirection: "column",
            justifyContent: "center",
            alignItems: "center",
            height: "100vh",
            background:
              "linear-gradient(135deg, #1a1a1d 0%, #16161a 50%, #0f0f12 100%)",
            color: "#ffffff",
          }}
        >
          <Spin size="large" style={{ marginBottom: "16px" }} />
          <Typography.Text style={{ color: "#ffffff", fontSize: "16px" }}>
            Đang tải lịch sử trò chuyện...
          </Typography.Text>
        </div>
      </ConfigProvider>
    );
  }
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
    </ConfigProvider>
  );
};

export default SearchPage;
