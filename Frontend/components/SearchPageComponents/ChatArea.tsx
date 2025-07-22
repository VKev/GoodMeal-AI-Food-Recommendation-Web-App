import React, { useState, useRef, useEffect } from "react";
import { Input, Button, message, Spin, Avatar, Tooltip} from "antd";
import {
  SendOutlined,
  UserOutlined,
  EnvironmentOutlined,
} from "@ant-design/icons";
import Image from "next/image";
import {
  processFoodRequestStream,
  ProcessFoodRequestPayload,
  getSessionMessages,
} from "../../services/PromptService";
import { useAuth } from "../../hooks/auths/authContext";
import { useRouter } from "next/navigation";
import { LocationData } from "../../hooks/useGeolocation";

const { TextArea } = Input;

// Add CSS animation styles
const animationStyles = `
        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        @keyframes slideInLeft {
            from {
                opacity: 0;
                transform: translateX(-20px);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }
        
        @keyframes slideInRight {
            from {
                opacity: 0;
                transform: translateX(20px);
            }
            to {
                opacity: 1;
                transform: translateX(0);
            }
        }
        
        @keyframes shimmer {
            0% {
                background-position: -468px 0;
            }
            100% {
                background-position: 468px 0;
            }
        }
        
        .image-loading {
            background: linear-gradient(90deg, #2a2a2a 25%, #3a3a3a 37%, #2a2a2a 63%);
            background-size: 400% 100%;
            animation: shimmer 1.5s ease-in-out infinite;
        }
        
        @media (max-width: 768px) {
            .food-grid {
                grid-template-columns: 1fr !important;
                gap: 16px !important;
            }
        }
        
        @media (max-width: 1024px) and (min-width: 769px) {
            .food-grid {
                grid-template-columns: repeat(2, 1fr) !important;
            }
        }
    `;

// Inject styles
if (
  typeof document !== "undefined" &&
  !document.getElementById("chat-animations")
) {
  const styleSheet = document.createElement("style");
  styleSheet.id = "chat-animations";
  styleSheet.textContent = animationStyles;
  document.head.appendChild(styleSheet);
}

interface Message {
  id: string;
  type: "user" | "bot";
  content: string;
  imageUrl?: string;
  imageUrls?: string[];
  foods?: any[];
  timestamp: Date;
  color?: string; // ADD THIS LINE - was missing from original interface
}

interface ChatAreaProps {
  sessionId?: string;
  userLocation?: LocationData | null;
  onRequestLocation?: () => void;
  onSessionName?: (name: string) => void;
  onFirstStreamTitle?: (title: string) => void;
}

export const ChatArea: React.FC<ChatAreaProps> = ({
  sessionId,
  userLocation,
  onRequestLocation,
  onFirstStreamTitle,
}) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [inputValue, setInputValue] = useState("");
  const [loading, setLoading] = useState(false);
  const [isTransitioning, setIsTransitioning] = useState(false);
  const [visibleFoodCounts, setVisibleFoodCounts] = useState<{
    [key: string]: number;
  }>({});
  const messagesEndRef = useRef<HTMLDivElement>(null);
  const { currentUser, authUser } = useAuth();
  const router = useRouter();
  const [prevSessionId, setPrevSessionId] = useState<string | undefined>(
    sessionId
  );

  const hasSetStreamTitle = useRef(false);

  const handleLoadMore = (messageId: string) => {
    setVisibleFoodCounts((prev) => ({
      ...prev,
      [messageId]: (prev[messageId] || 6) + 6,
    }));
  };

  const handleFoodClick = (foodName: string, location?: string) => {
    // console.log('=== FOOD CLICK DEBUG ===');
    // console.log('foodName:', foodName);
    // console.log('food location from API:', location);
    // console.log('userLocation from props:', userLocation);
    // console.log('========================');

    // Navigate to restaurants page with food name and location as search query
    let url = `/restaurants?search=${encodeURIComponent(foodName)}`;

    // Use API location first, then fallback to user location
    if (location && location !== "null" && location.trim()) {
      url += `&location=${encodeURIComponent(location)}`;
    } else if (userLocation) {
      // If no API location, use user's coordinates
      url += `&lat=${userLocation.latitude}&lng=${userLocation.longitude}`;
      console.log(
        "Using user coordinates:",
        userLocation.latitude,
        userLocation.longitude
      );
    }

    router.push(url);
  };

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // Load messages when sessionId changes
  useEffect(() => {
    const loadMessages = async () => {
      // Handle session transition with smooth effect
      if (prevSessionId !== sessionId) {
        setIsTransitioning(true);

        // Clear messages immediately for new session or when switching
        setMessages([]);
        
        // Reset stream title flag for new session
        hasSetStreamTitle.current = false;

        // Small delay for smooth transition
        await new Promise((resolve) => setTimeout(resolve, 150));

        setPrevSessionId(sessionId);
        setIsTransitioning(false);
      }

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
            type: "user",
            content: apiMsg.promptMessage,
            timestamp: new Date(apiMsg.createdAt),
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

              if (typeof responseText === "string") {
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
                    responseText = `**${
                      parsedResponse.Title || "Gợi ý món ăn"
                    }**`;
                    const location = parsedResponse.Location;
                    foods = parsedResponse.Foods.map((food: any) => ({
                      foodName: food.FoodName,
                      national: food.National,
                      description: food.Description,
                      imageUrl: food.ImageUrl,
                      location: location,
                    }));
                  }
                } else if (parsedResponse.Title) {
                  responseText = parsedResponse.Title;
                } else {
                  responseText = "No specific food information found.";
                }
              }
            } catch (parseError) {
              console.error("Error parsing historical message:", parseError);
              // Keep original response if parsing fails
            }

            convertedMessages.push({
              id: `bot-${apiMsg.id}`,
              type: "bot",
              content: responseText,
              imageUrl,
              foods: foods.length > 0 ? foods : undefined,
              timestamp: new Date(apiMsg.createdAt),
            });
          }
        }

        // Sort by timestamp
        convertedMessages.sort(
          (a, b) => a.timestamp.getTime() - b.timestamp.getTime()
        );
        setMessages(convertedMessages);
      } catch (error) {
        console.error("Error loading session messages:", error);
        // Don't show error message to user, just log it
      }
    };

    // Clear messages when sessionId changes
    setMessages([]);
    loadMessages();
  }, [sessionId, currentUser, prevSessionId]);

  const handleSendMessage = async () => {
    if (!inputValue.trim() || !sessionId || !currentUser || !authUser) {
      message.error("Please enter a message and make sure you are logged in");
      return;
    }

    const userMessage: Message = {
      id: Date.now().toString(),
      type: "user",
      content: inputValue.trim(),
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, userMessage]);

    const currentInput = inputValue.trim();
    setInputValue("");
    setLoading(true);

    try {
      const idToken = await currentUser.getIdToken();

      const payload: ProcessFoodRequestPayload = {
        promptSessionId: sessionId,
        sender: authUser.userId || "anonymous",
        promptMessage: currentInput,
        responseMessage: "",
        ...(userLocation && {
          lat: userLocation.latitude,
          lng: userLocation.longitude,
        }),
      };

      console.log("=== DEBUG LOCATION DATA ===");
      console.log("userLocation:", userLocation);
      console.log("payload with location:", payload);
      console.log("===========================");

      let hasError = false;
      let isFirstMessage = true;

      let currentBotMessage: Message | null = null;
      const streamedFoods: any[] = [];
      let streamTitle = "";
      let streamLocation = "";

      await processFoodRequestStream(
        idToken,
        payload,
        (data) => {
          if (hasError) return;

          console.log("📥 Stream data:", data);

          if (isFirstMessage) {
            isFirstMessage = false;

            if (data.Error && data.Error !== "null") {
              const errorMessage: Message = {
                id: Date.now().toString(),
                type: "bot",
                content: data.Error,
                timestamp: new Date(),
              };
              setMessages((prev) => [...prev, errorMessage]);
              hasError = true;
            }
            return;
          }

          // Handle different data types from stream
          if (data.Title) {
            streamTitle = data.Title;
            console.log("🎯 Stream Title received:", data.Title);
            console.log("🎯 hasSetStreamTitle.current:", hasSetStreamTitle.current);
            console.log("🎯 onFirstStreamTitle exists:", !!onFirstStreamTitle);
            
            // Call onFirstStreamTitle immediately when we get the first title
            if (!hasSetStreamTitle.current && onFirstStreamTitle) {
              console.log("🎯 Calling onFirstStreamTitle with:", data.Title);
              hasSetStreamTitle.current = true;
              onFirstStreamTitle(data.Title);
            }
          }

          if (data.Location) {
            streamLocation = data.Location;
          }

          if (data.FoodName) {
            // Add food to accumulated list
            streamedFoods.push({
              foodName: data.FoodName,
              national: data.National,
              description: data.Description,
              imageUrl: data.ImageUrl,
              location: streamLocation,
            });
          }

          // Create or update the current bot message
          if (!currentBotMessage) {
            currentBotMessage = {
              id: Date.now().toString(),
              type: "bot",
              content: streamTitle ? `**${streamTitle}**` : "",
              timestamp: new Date(),
            };
            setMessages((prev) => [...prev, currentBotMessage!]);
          }

          // Update the message with accumulated foods
          setMessages((prev) =>
            prev.map((msg) =>
              msg.id === currentBotMessage?.id
                ? {
                    ...msg,
                    content: streamTitle ? `**${streamTitle}**` : "",
                    foods:
                      streamedFoods.length > 0 ? [...streamedFoods] : undefined,
                  }
                : msg
            )
          );
        },
        () => {
          console.log("✅ Stream completed");
          setLoading(false);
        }
      );
    } catch (error) {
      console.error("❌ Error sending message:", error);
      message.error("Failed to send message. Please try again.");

      const errorMessage: Message = {
        id: (Date.now() + 1).toString(),
        type: "bot",
        content:
          "Sorry, I encountered an error processing your request. Please try again.",
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, errorMessage]);

      setLoading(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  if (!sessionId) {
    return (
      <div
        style={{
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          height: "100%",
          color: "#b3b3b3",
        }}
      >
        Hãy nhấn dấu vào + để tạo mới một đoạn chat
      </div>
    );
  }

  return (
    <div
      className={`chat-area ${isTransitioning ? "transitioning" : ""}`}
      style={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        backgroundColor: "transparent",
      }}
    >
      {/* Messages Area */}
      <div
        style={{
          flex: 1,
          overflowY: "auto",
          padding: "20px",
          display: "flex",
          flexDirection: "column",
          gap: "16px",
          opacity: isTransitioning ? 0 : 1,
          transform: isTransitioning ? "translateY(10px)" : "translateY(0)",
          transition: "opacity 0.3s ease-in-out, transform 0.3s ease-in-out",
        }}
      >
        {messages.map((msg, index) => (
          <div key={msg.id} style={{ marginBottom: "16px" }}>
            {msg.type === "user" ? (
              // User message - keep original format
              <div
                style={{
                  display: "flex",
                  alignItems: "flex-start",
                  gap: "12px",
                  flexDirection: "row-reverse",
                  animationDelay: `${index * 0.1}s`,
                }}
              >
                <Avatar
                  icon={<UserOutlined />}
                  style={{
                    backgroundColor: "#ff7a00",
                    flexShrink: 0,
                  }}
                />
                <div
                  style={{
                    maxWidth: "70%",
                    padding: "12px 16px",
                    borderRadius: "12px",
                    backgroundColor: "#ff7a00",
                    color: "#ffffff",
                    boxShadow: "0 2px 8px rgba(0,0,0,0.15)",
                    border: "1px solid #ff7a00",
                    textAlign: "right",
                  }}
                >
                  <div>{msg.content}</div>
                  <div
                    style={{
                      fontSize: "11px",
                      opacity: 0.7,
                      marginTop: "4px",
                    }}
                  >
                    {msg.timestamp.toLocaleTimeString()}
                  </div>
                </div>
              </div>
            ) : (
              // Bot message - show foods directly without message box
              <div>


                {/* Food Grid - Direct display like in the image */}
                {msg.foods && msg.foods.length > 0 && (
                  <div>
                    <div
                      style={{
                        display: "grid",
                        gridTemplateColumns:
                          "repeat(auto-fit, minmax(280px, 1fr))",
                        gap: "20px",
                        maxWidth: "100%",
                      }}
                      className="food-grid"
                    >
                      {msg.foods
                        .slice(0, visibleFoodCounts[msg.id] || 6)
                        .map((food: any, foodIndex: number) => (
                          <div
                            key={foodIndex}
                            style={{
                              background:
                                "linear-gradient(145deg, rgba(40,40,40,0.9) 0%, rgba(20,20,20,0.95) 100%)",
                              borderRadius: "16px",
                              border: "1px solid rgba(80, 80, 80, 0.3)",
                              overflow: "hidden",
                              transition: "all 0.3s ease",
                              cursor: "pointer",
                              position: "relative",
                              minHeight: "280px",
                              boxShadow: "0 4px 12px rgba(0,0,0,0.3)",
                            }}
                            onMouseEnter={(e) => {
                              e.currentTarget.style.background =
                                "linear-gradient(145deg, rgba(60,60,60,0.95) 0%, rgba(30,30,30,1) 100%)";
                              e.currentTarget.style.transform =
                                "translateY(-6px)";
                              e.currentTarget.style.boxShadow =
                                "0 20px 40px rgba(0,0,0,0.5)";
                              e.currentTarget.style.borderColor =
                                "rgba(120, 120, 120, 0.5)";
                            }}
                            onMouseLeave={(e) => {
                              e.currentTarget.style.background =
                                "linear-gradient(145deg, rgba(40,40,40,0.9) 0%, rgba(20,20,20,0.95) 100%)";
                              e.currentTarget.style.transform = "translateY(0)";
                              e.currentTarget.style.boxShadow =
                                "0 4px 12px rgba(0,0,0,0.3)";
                              e.currentTarget.style.borderColor =
                                "rgba(80, 80, 80, 0.3)";
                            }}
                            onClick={() =>
                              handleFoodClick(food.foodName, food.location)
                            }
                            title={`Click to find restaurants serving ${
                              food.foodName
                            }${
                              food.location && food.location !== "null"
                                ? ` in ${food.location}`
                                : ""
                            }`}
                          >
                            {/* Food Image */}
                            <div
                              style={{
                                position: "relative",
                                width: "100%",
                                height: "200px",
                                overflow: "hidden",
                                backgroundColor: "#2a2a2a",
                              }}
                            >
                              {food.imageUrl ? (
                                <Image
                                  src={food.imageUrl}
                                  alt={food.foodName}
                                  fill
                                  sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
                                  quality={95}
                                  priority={foodIndex < 3}
                                  style={{
                                    objectFit: "cover",
                                    objectPosition: "center",
                                  }}
                                  onError={(e) => {
                                    console.error(
                                      "Failed to load food image:",
                                      food.imageUrl
                                    );
                                    // Replace with fallback image instead of hiding
                                    const target =
                                      e.currentTarget as HTMLImageElement;
                                    target.src = "/placeholder-food.svg";
                                    target.onerror = null; // Prevent infinite loop
                                  }}
                                  onLoad={(e) => {
                                    // Fade in effect when image loads
                                    const target =
                                      e.currentTarget as HTMLImageElement;
                                    target.style.opacity = "1";
                                  }}
                                  onLoadStart={(e) => {
                                    // Start with opacity 0 for smooth loading
                                    const target =
                                      e.currentTarget as HTMLImageElement;
                                    target.style.opacity = "0";
                                    target.style.transition =
                                      "opacity 0.3s ease-in-out";
                                  }}
                                />
                              ) : (
                                // Fallback when no image URL
                                <div
                                  style={{
                                    width: "100%",
                                    height: "100%",
                                    display: "flex",
                                    alignItems: "center",
                                    justifyContent: "center",
                                    backgroundColor: "#3a3a3a",
                                    color: "#888",
                                    fontSize: "14px",
                                  }}
                                >
                                  <div style={{ textAlign: "center" }}>
                                    <div
                                      style={{
                                        fontSize: "24px",
                                        marginBottom: "8px",
                                      }}
                                    >
                                      🍽️
                                    </div>
                                    <div>Không có hình ảnh</div>
                                  </div>
                                </div>
                              )}
                              {/* Gradient overlay */}
                              <div
                                style={{
                                  position: "absolute",
                                  bottom: 0,
                                  left: 0,
                                  right: 0,
                                  height: "60%",
                                  background:
                                    "linear-gradient(to top, rgba(0,0,0,0.8) 0%, rgba(0,0,0,0.4) 50%, transparent 100%)",
                                  pointerEvents: "none",
                                }}
                              />

                              {/* Food name overlay on image */}
                              <div
                                style={{
                                  position: "absolute",
                                  bottom: "12px",
                                  left: "12px",
                                  right: "12px",
                                  color: "#ffffff",
                                  fontSize: "18px",
                                  fontWeight: "bold",
                                  textShadow: "0 2px 4px rgba(0,0,0,0.8)",
                                  overflow: "hidden",
                                  textOverflow: "ellipsis",
                                  whiteSpace: "nowrap",
                                }}
                              >
                                {food.foodName}
                              </div>
                            </div>

                            {/* Food Info */}
                            <div style={{ padding: "16px" }}>
                              <div
                                style={{
                                  fontSize: "15px",
                                  lineHeight: "1.5",
                                  color: "#ffffff",
                                  display: "-webkit-box",
                                  WebkitLineClamp: 3,
                                  WebkitBoxOrient: "vertical",
                                  overflow: "hidden",
                                  marginBottom: "16px",
                                  minHeight: "65px",
                                  fontWeight: "400",
                                }}
                              >
                                {food.description}
                              </div>
                            </div>
                          </div>
                        ))}
                    </div>

                    {/* Load More Button */}
                    {msg.foods.length > (visibleFoodCounts[msg.id] || 6) && (
                      <div
                        style={{
                          display: "flex",
                          justifyContent: "center",
                          marginTop: "24px",
                        }}
                      >
                        <Button
                          type="default"
                          size="large"
                          onClick={() => handleLoadMore(msg.id)}
                          style={{
                            backgroundColor: "transparent",
                            borderColor: "rgba(255, 122, 0, 0.5)",
                            color: "#ff7a00",
                            borderRadius: "12px",
                            fontSize: "14px",
                            height: "40px",
                            padding: "0 24px",
                            fontWeight: "600",
                            transition: "all 0.3s ease",
                          }}
                          onMouseEnter={(e) => {
                            e.currentTarget.style.backgroundColor =
                              "rgba(255, 122, 0, 0.1)";
                            e.currentTarget.style.borderColor = "#ff7a00";
                            e.currentTarget.style.transform =
                              "translateY(-2px)";
                          }}
                          onMouseLeave={(e) => {
                            e.currentTarget.style.backgroundColor =
                              "transparent";
                            e.currentTarget.style.borderColor =
                              "rgba(255, 122, 0, 0.5)";
                            e.currentTarget.style.transform = "translateY(0)";
                          }}
                        >
                          Xem thêm món ăn (
                          {msg.foods.length - (visibleFoodCounts[msg.id] || 6)}{" "}
                          món còn lại)
                        </Button>
                      </div>
                    )}
                  </div>
                )}

                {/* Single image display (for backward compatibility) */}
                {msg.imageUrl && !msg.foods && (
                  <div
                    style={{
                      cursor: "pointer",
                      transition: "transform 0.2s",
                      borderRadius: "12px",
                      overflow: "hidden",
                      maxWidth: "500px",
                      backgroundColor: "#2a2a2a",
                    }}
                    onClick={() => {
                      const lines = msg.content.split("\n");
                      const titleLine = lines.find(
                        (line) => line.startsWith("**") && line.endsWith("**")
                      );
                      if (titleLine) {
                        const foodName = titleLine
                          .replace(/\*\*/g, "")
                          .split("(")[0]
                          .trim();
                        handleFoodClick(foodName);
                      }
                    }}
                    onMouseEnter={(e) => {
                      e.currentTarget.style.transform = "scale(1.02)";
                    }}
                    onMouseLeave={(e) => {
                      e.currentTarget.style.transform = "scale(1)";
                    }}
                    title="Click to find restaurants"
                  >
                    <div
                      style={{
                        position: "relative",
                        width: "100%",
                        height: "300px",
                      }}
                    >
                      <Image
                        src={msg.imageUrl}
                        alt="Generated food"
                        fill
                        sizes="(max-width: 768px) 100vw, 500px"
                        quality={95}
                        style={{
                          objectFit: "cover",
                          objectPosition: "center",
                        }}
                        onError={(e) => {
                          console.error("Failed to load image:", msg.imageUrl);
                          const target = e.currentTarget as HTMLImageElement;
                          target.src = "/placeholder-food.svg";
                          target.onerror = null;
                        }}
                        onLoad={(e) => {
                          const target = e.currentTarget as HTMLImageElement;
                          target.style.opacity = "1";
                        }}
                        onLoadStart={(e) => {
                          const target = e.currentTarget as HTMLImageElement;
                          target.style.opacity = "0";
                          target.style.transition = "opacity 0.3s ease-in-out";
                        }}
                      />
                    </div>
                  </div>
                )}
              </div>
            )}
          </div>
        ))}
        {loading && (
          <div
            style={{
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              padding: "20px",
            }}
          >
            <div
              style={{
                padding: "12px 20px",
                backgroundColor: "#262629",
                border: "1px solid rgba(255, 122, 0, 0.2)",
                borderRadius: "12px",
                boxShadow: "0 2px 8px rgba(0,0,0,0.15)",
                display: "flex",
                alignItems: "center",
                gap: "10px",
              }}
            >
              <Spin size="small" style={{ color: "#ffffff" }} />
              <span style={{ color: "#ffffff", fontSize: "14px" }}>
                Đang tìm kiếm món ăn phù hợp...
              </span>
            </div>
          </div>
        )}
        <div ref={messagesEndRef} />
      </div>

      {/* Input Area */}
      <div
        style={{
          padding: "20px",
          borderTop: "1px solid rgba(255, 122, 0, 0.2)",
          backgroundColor: "#1f1f23",
        }}
      >
        {/* Location Status */}
        <div
          style={{
            marginBottom: "12px",
            display: "flex",
            alignItems: "center",
            gap: "8px",
          }}
        >
          <Tooltip
            title={
              userLocation
                ? `Vị trí hiện tại: ${userLocation.latitude.toFixed(
                    4
                  )}, ${userLocation.longitude.toFixed(4)}`
                : "Chưa có quyền truy cập vị trí - Click để cấp quyền"
            }
          >
            <Button
              size="small"
              type={userLocation ? "primary" : "default"}
              icon={<EnvironmentOutlined />}
              onClick={userLocation ? undefined : onRequestLocation}
              style={{
                backgroundColor: userLocation ? "#52c41a" : "transparent",
                borderColor: userLocation
                  ? "#52c41a"
                  : "rgba(255, 122, 0, 0.3)",
                color: userLocation ? "#ffffff" : "#b3b3b3",
                fontSize: "12px",
                height: "24px",
                cursor: userLocation ? "default" : "pointer",
              }}
            >
              {userLocation ? "Vị trí đã bật" : "Bật vị trí"}
            </Button>
          </Tooltip>
          {userLocation && (
            <span
              style={{
                fontSize: "12px",
                color: "#b3b3b3",
              }}
            >
              Kết quả sẽ tập trung vào khu vực gần bạn
            </span>
          )}
        </div>

        <div style={{ display: "flex", gap: "8px", alignItems: "flex-end" }}>
          <TextArea
            value={inputValue}
            onChange={(e) => setInputValue(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Nhập yêu cầu món ăn của bạn ở dây"
            autoSize={{ minRows: 1, maxRows: 4 }}
            style={{
              flex: 1,
              backgroundColor: "#262629",
              borderColor: "rgba(255, 122, 0, 0.3)",
              color: "#ffffff",
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
              alignSelf: "flex-end",
              backgroundColor: "#ff7a00",
              borderColor: "#ff7a00",
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
