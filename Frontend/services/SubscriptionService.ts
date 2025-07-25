export interface Subscription {
  id: string;
  name: string;
  description: string;
  price: number;
  durationInMonths: number;
  currency: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface SubscriptionsResponse {
  value: {
    subscriptions: Subscription[];
    totalCount: number;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export const getAllSubscriptions = async (idToken: string): Promise<Subscription[]> => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return [];
    }

    const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/subscription`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: SubscriptionsResponse = await response.json();
    
    if (data.isSuccess && data.value?.subscriptions) {
      return data.value.subscriptions;
    }

    return [];
  } catch (error) {
    console.error("Error fetching subscriptions:", error);
    return [];
  }
};

export interface MySubscriptionResponse {
  value: {
    id: string;
    userId: string;
    subscriptionId: string;
    subscription: Subscription;
    startDate: string;
    endDate: string;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    description: string;
  };
}

export const getMySubscription = async (idToken: string) => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return null;
    }

    const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/subscription/my-subscription`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: MySubscriptionResponse = await response.json();
    
    if (data.isSuccess && data.value) {
      return data.value;
    }

    return null;
  } catch (error) {
    console.error("Error fetching my subscription:", error);
    return null;
  }
};

export interface SubscriptionRegistrationResponse {
  results: [{
    name: string;
    isSuccess: boolean;
    data: {
      correlationId: string;
      userId: string;
      subscriptionId: string;
      amount: {
        source: string;
        parsedValue: number;
      };
      currency: string;
      orderId: string;
      message: string;
    }
  }];
}

export const registerSubscription = async (idToken: string, subscriptionId: string) => {
  try {
    if (!idToken) {
      console.warn("No ID token provided");
      return { success: false, data: null };
    }

    const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/subscription/register`, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ subscriptionId }),
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: SubscriptionRegistrationResponse = await response.json();
    
    return {
      success: data.results[0].isSuccess,
      data: data.results[0].data
    };
  } catch (error) {
    console.error("Error registering subscription:", error);
    return { success: false, data: null };
  }
};

export interface PaymentStatusResponse {
  value: {
    correlationId: string;
    currentState: string;
    paymentUrl: string;
    paymentUrlCreated: boolean;
    paymentCompleted: boolean;
    subscriptionActivated: boolean;
    failureReason: string | null;
    createdAt: string;
    completedAt: string | null;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error?: {
    code: string;
    description: string;
  };
}

export const getSubscriptionPaymentStatus = async (idToken: string, correlationId: string) => {
  try {
    if (!idToken || !correlationId) {
      console.warn("No ID token or correlationId provided");
      return null;
    }

    const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/subscription/payment-status/${correlationId}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: PaymentStatusResponse = await response.json();
    
    return data;
  } catch (error) {
    console.error("Error checking payment status:", error);
    return null;
  }
}; 

export interface PaymentUrlResponse {
  value: {
    correlationId: string;
    paymentUrl: string;
    currentState: string;
    paymentUrlCreated: boolean;
    paymentCompleted: boolean;
    subscriptionActivated: boolean;
    failureReason: string | null;
    createdAt: string;
    completedAt: string | null;
  };
  isSuccess: boolean;
  isFailure: boolean;
  error?: {
    code: string;
    description: string;
  };
}

export const getPaymentUrl = async (idToken: string, correlationId: string) => {
  try {
    if (!idToken || !correlationId) {
      console.warn("No ID token or correlationId provided");
      return null;
    }

    const baseUrl = process.env.NEXT_PUBLIC_BACKEND_BASE_URL || "http://localhost:2406/";
    const response = await fetch(`${baseUrl}api/subscription/payment-url/${correlationId}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${idToken}`,
        "Content-Type": "application/json",
      },
    });

    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }

    const data: PaymentUrlResponse = await response.json();
    
    return data;
  } catch (error) {
    console.error("Error fetching payment URL:", error);
    return null;
  }
}; 