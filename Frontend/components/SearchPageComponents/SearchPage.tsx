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

// Import services
import {
  getPromptSessions,
  formatTime,
  createPromptSession,
  deletePromptSession,
} from "../../services/PromptService";
import { checkAuthorization } from "../../services/Auth";
import { ChatItem } from "./types";

const SearchPage = () => {
  const [selectedChat, setSelectedChat] = useState<string | null>(null);
  const [collapsed, setCollapsed] = useState(false);
  const [chatHistory, setChatHistory] = useState<ChatItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<any>(null);
  const [userId, setUserId] = useState<string | null>(null);

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
          setLoading(false);
        }
      }
    );

    return () => unsubscribe();
  }, []);

  // Load prompt sessions from API
  const loadPromptSessions = async (idToken: string, currentUserId: string) => {
    try {
      setLoading(true);
      const sessions = await getPromptSessions(idToken, currentUserId);
      
      // Sort sessions by createdAt descending (newest first)
      const sortedSessions = sessions.sort((a, b) => 
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      );
      
      // Transform sessions to chat history format
      const transformedHistory: ChatItem[] = sortedSessions.map((session) => {
        const sessionDate = new Date(session.createdAt);
        const formattedTitle = `Session ${sessionDate.toLocaleDateString('vi-VN')} ${sessionDate.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' })}`;
        
        return {
          id: session.id,
          title: formattedTitle,
          time: formatTime(session.createdAt),
        };
      });

      setChatHistory(transformedHistory);
    } catch (error) {
      console.error("Error loading prompt sessions:", error);
      message.error("Failed to load chat history");
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
        console.error("Error refreshing chat history:", error);
        message.error("Failed to refresh chat history");
      }
    }
  }; // Create new session function
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
      const idToken = await user.getIdToken();
      console.log("About to create session with userId:", userId);
      const newSession = await createPromptSession(idToken, userId);

      if (newSession) {
        message.success("New session created successfully");
        // Set the new session as selected
        setSelectedChat(newSession.id);
        // Refresh chat history to include the new session
        await loadPromptSessions(idToken, userId);
      } else {
        message.error("Failed to create new session");
      }
    } catch (error) {
      console.error("Error creating session:", error);
      message.error("Failed to create new session");
    }
  }; // Delete session function
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
  }; // Show loading spinner while fetching data
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
            Loading your chat history...
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
          <SearchHeader collapsed={collapsed} />

          {/* Content Area with Chat */}
          <div style={{ flex: 1, overflow: "hidden" }}>
            <ChatArea sessionId={selectedChat || undefined} />
          </div>
        </Layout>
      </Layout>
    </ConfigProvider>
  );
};

export default SearchPage;
