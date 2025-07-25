export interface PromptSession {
  id: string;
  userId: string;
  createdAt: string;
  updatedAt: string | null;
  deletedAt: string | null;
  isDeleted: boolean;
  sessionName: string;
}

interface PromptSessionResponse {
  value: {
    results: Array<{
      name: string;
      isSuccess: boolean;
      data: PromptSession[];
    }>;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export const getPromptSessions = async (
  idToken: string,
  userId: string
): Promise<PromptSession[]> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return [];
    }

    const baseUrl =
      process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/prompt/PromptSession/read`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: PromptSessionResponse = await response.json();
    if (data.isSuccess && data.value.results.length > 0) {
      // Lọc sessions theo userId và chỉ lấy những session chưa bị xóa
      const allSessions = data.value.results[0].data || [];
      return allSessions.filter(
        (session) => session.userId === userId && !session.isDeleted
      );
    }

    return [];
  } catch (error) {
    console.error("Error fetching prompt sessions:", error);
    return [];
  }
};

// Helper function để format thời gian
export const formatTime = (dateString: string): string => {
  const date = new Date(dateString);
  const now = new Date();

  // Chuyển đổi về múi giờ địa phương và chỉ so sánh ngày
  const dateLocal = new Date(
    date.getFullYear(),
    date.getMonth(),
    date.getDate()
  );
  const nowLocal = new Date(now.getFullYear(), now.getMonth(), now.getDate());

  const diffInMs = nowLocal.getTime() - dateLocal.getTime();
  const diffInDays = Math.floor(diffInMs / (1000 * 60 * 60 * 24));

  if (diffInDays === 0) {
    return "Hôm nay";
  } else if (diffInDays === 1) {
    return "Hôm qua";
  } else if (diffInDays < 7) {
    return `${diffInDays} ngày trước`;
  } else if (diffInDays < 14) {
    return "1 tuần trước";
  } else if (diffInDays < 21) {
    return "2 tuần trước";
  } else {
    return date.toLocaleDateString("vi-VN");
  }
};

interface CreateSessionResponse {
  value: {
    results: Array<{
      name: string;
      isSuccess: boolean;
      data: PromptSession;
    }>;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export const createPromptSession = async (
  idToken: string,
  userId: string
): Promise<PromptSession | null> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return null;
    }

    if (!userId) {
      console.warn("No User ID provided");
      return null;
    }
    console.log("Creating session with userId:", userId);

    const baseUrl =
      process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";

    // API only needs userId - it will generate a new session ID automatically
    const requestBody = {
      userId: userId,
    };

    console.log("Request body:", requestBody);
    const response = await fetch(`${baseUrl}api/prompt/PromptSession/create`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(requestBody),
    });

    console.log("Response status:", response.status);
    console.log("Response headers:", response.headers);

    if (!response.ok) {
      let errorText = "";
      try {
        errorText = await response.text();
        console.log("Error response body:", errorText);
      } catch (e) {
        console.log("Could not read error response body");
      }
      throw new Error(
        `HTTP error! status: ${response.status}, body: ${errorText}`
      );
    }

    const responseText = await response.text();
    console.log("Raw response:", responseText);

    // Parse JSON
    const data: CreateSessionResponse = JSON.parse(responseText);
    console.log("Create session response:", data);
    console.log("Response structure:", JSON.stringify(data, null, 2));

    if (data.isSuccess && data.value.results.length > 0) {
      const sessionResult = data.value.results[0];
      console.log("Session result:", sessionResult);
      console.log("Session result data:", sessionResult.data);
      console.log(
        "Session result data keys:",
        Object.keys(sessionResult.data || {})
      );

      if (sessionResult.isSuccess && sessionResult.data) {
        console.log("Session created successfully:", sessionResult.data);

        // Log all properties of the session data
        for (const [key, value] of Object.entries(sessionResult.data)) {
          console.log(`${key}:`, value);
        }

        return sessionResult.data;
      }
    }

    console.log("Session creation failed or no data returned");
    return null;
  } catch (error) {
    console.error("Error creating prompt session:", error);
    return null;
  }
};

export interface ProcessFoodRequestPayload {
  promptSessionId: string;
  sender: string;
  promptMessage: string;
  responseMessage: string;
  // Optional location parameters
  lat?: number;
  lng?: number;
}

export interface ProcessFoodResponse {
  responseMessage?: string;
  imageUrl?: string;
  [key: string]: any; // Allow any additional fields
}

export const processFoodRequest = async (
  idToken: string,
  payload: ProcessFoodRequestPayload
): Promise<ProcessFoodResponse | null> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return null;
    }

    console.log("Sending request with payload:", payload); // Debug log
    console.log("=== PROMPT SERVICE DEBUG ===");
    console.log("payload.lat:", payload.lat);
    console.log("payload.lng:", payload.lng);
    console.log("Full payload:", JSON.stringify(payload, null, 2));
    console.log("=============================");

    const baseUrl =
      process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(
      `${baseUrl}api/prompt/Gemini/process-food-request/stream`,
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${idToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify(payload),
      }
    );

    console.log("Process food request response status:", response.status);

    if (!response.ok) {
      let errorText = "";
      try {
        errorText = await response.text();
        console.log("Error response body:", errorText);
      } catch (e) {
        console.log("Could not read error response body");
      }
      throw new Error(
        `HTTP error! status: ${response.status}, body: ${errorText}`
      );
    }

    const data = await response.json();
    console.log("Raw API response data:", data);

    // Handle the specific response format from your API
    if (
      data &&
      data.value &&
      data.value.results &&
      data.value.results.length > 0
    ) {
      const result = data.value.results[0];
      if (result.isSuccess && result.data && result.data.length > 0) {
        // Get the latest message (last item in array)
        const latestMessage = result.data[result.data.length - 1];

        let responseMessage = latestMessage.responseMessage;
        let imageUrl = null;

        // Parse the escaped JSON string
        try {
          if (
            responseMessage &&
            responseMessage.startsWith('"') &&
            responseMessage.endsWith('"')
          ) {
            // Remove outer quotes and unescape
            responseMessage = JSON.parse(responseMessage);
          }

          // If it's still a JSON string, parse again
          if (typeof responseMessage === "string") {
            const parsedResponse = JSON.parse(responseMessage);

            // Extract meaningful information
            if (parsedResponse.Foods && parsedResponse.Foods.length > 0) {
              const food = parsedResponse.Foods[0];
              responseMessage = `**${food.FoodName}** (${food.National})\n\n${food.Description}`;
              imageUrl = food.ImageUrl;
            } else if (parsedResponse.Title) {
              responseMessage = parsedResponse.Title;
            } else {
              responseMessage =
                "I received your message but couldn't find specific food information.";
            }
          }
        } catch (parseError) {
          console.error("Error parsing responseMessage:", parseError);
          responseMessage = latestMessage.responseMessage; // Use original if parsing fails
        }

        return {
          responseMessage,
          imageUrl,
        };
      }
    }

    // Fallback for direct response
    return data;
  } catch (error) {
    console.error("Error processing food request:", error);
    return null;
  }
};
export const processFoodRequestStream = async (
  idToken: string,
  payload: ProcessFoodRequestPayload,
  onMessage: (data: ProcessFoodResponse) => void,
  onDone?: () => void
): Promise<void> => {
  if (!idToken) {
    console.warn("No ID token provided");
    return;
  }

  const baseUrl =
    process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";

  const response = await fetch(
    `${baseUrl}api/prompt/Gemini/process-food-request/stream`,
    {
      method: "POST",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    }
  );

  if (!response.ok || !response.body) {
    const errorText = await response.text();
    throw new Error(
      `HTTP error! status: ${response.status}, body: ${errorText}`
    );
  }

  const reader = response.body.getReader();
  const decoder = new TextDecoder();
  let buffer = "";

  while (true) {
    const { value, done } = await reader.read();
    if (done) break;

    buffer += decoder.decode(value, { stream: true });

    // Split messages by double linebreaks
    const parts = buffer.split("\n\n");

    for (let i = 0; i < parts.length - 1; i++) {
      const raw = parts[i].trim();

      if (raw.startsWith("event: done")) {
        if (onDone) onDone();
        continue;
      }

      if (raw.startsWith("data: ")) {
        const jsonStr = raw.slice(6); // Remove 'data: '
        try {
          const message = JSON.parse(jsonStr);
          onMessage(message);
        } catch (err) {
          console.error("❌ Failed to parse stream message:", err);
        }
      } else if (raw.startsWith("event: error")) {
        console.error("❌ Stream error:", raw);
      }
    }

    // Keep any incomplete part for the next read
    buffer = parts[parts.length - 1];
  }
};
export interface MessageResponse {
  id: string;
  promptSessionId: string;
  sender: string;
  createdAt: string;
  responseMessage: string;
  promptMessage: string;
  isDeleted: boolean;
}

export interface MessagesApiResponse {
  value: {
    results: Array<{
      name: string;
      isSuccess: boolean;
      data: MessageResponse[];
    }>;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export const getSessionMessages = async (
  idToken: string,
  promptSessionId: string
): Promise<MessageResponse[]> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return [];
    }

    const baseUrl =
      process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(
      `${baseUrl}api/prompt/Message/read-active/${promptSessionId}`,
      {
        method: "GET",
        headers: {
          Authorization: `Bearer ${idToken}`,
          "Content-Type": "application/json",
        },
      }
    );

    if (!response.ok) {
      let errorText = "";
      try {
        errorText = await response.text();
        console.log("Error response body:", errorText);
      } catch (e) {
        console.log("Could not read error response body");
      }
      throw new Error(
        `HTTP error! status: ${response.status}, body: ${errorText}`
      );
    }

    const data: MessagesApiResponse = await response.json();

    if (data.isSuccess && data.value.results.length > 0) {
      const result = data.value.results[0];
      if (result.isSuccess && result.data) {
        return result.data.filter((msg) => !msg.isDeleted);
      }
    }

    return [];
  } catch (error) {
    console.error("Error fetching session messages:", error);
    return [];
  }
};

export const deletePromptSession = async (
  idToken: string,
  sessionId: string
): Promise<boolean> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return false;
    }

    if (!sessionId) {
      console.warn("No session ID provided");
      return false;
    }

    console.log("Deleting session with ID:", sessionId);

    const baseUrl =
      process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(
      `${baseUrl}api/prompt/PromptSession/soft-delete`,
      {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${idToken}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ id: sessionId }),
      }
    );

    console.log("Delete session response status:", response.status);

    if (!response.ok) {
      let errorText = "";
      try {
        errorText = await response.text();
        console.log("Error response body:", errorText);
      } catch (e) {
        console.log("Could not read error response body");
      }
      throw new Error(
        `HTTP error! status: ${response.status}, body: ${errorText}`
      );
    }

    const data = await response.json();
    console.log("Delete session response:", data);

    // Check if deletion was successful
    if (data.isSuccess) {
      return true;
    }

    return false;
  } catch (error) {
    console.error("Error deleting prompt session:", error);
    return false;
  }
};

export const deleteAllPromptSessions = async (
    idToken: string
): Promise<boolean> => {
    try {
        if (!idToken) {
            console.warn('No ID token provided');
            return false;
        }

        console.log('Deleting all sessions for current user');

        const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || 'http://localhost:2406/';
        const response = await fetch(`${baseUrl}api/prompt/PromptSession/delete-all`, {
            method: 'DELETE',
            headers: {
                'Authorization': `Bearer ${idToken}`,
                'Content-Type': 'application/json',
            }
        });

        console.log('Delete all sessions response status:', response.status);

        if (!response.ok) {
            let errorText = '';
            try {
                errorText = await response.text();
                console.log('Error response body:', errorText);
            } catch (e) {
                console.log('Could not read error response body');
            }
            throw new Error(`HTTP error! status: ${response.status}, body: ${errorText}`);
        }

        const data = await response.json();
        console.log('Delete all sessions response:', data);

        // Check if deletion was successful
        if (data.isSuccess) {
            console.log('Successfully deleted all sessions');
            return true;
        }

        return false;
    } catch (error) {
        console.error('Error deleting all prompt sessions:', error);
        return false;
    }
};
