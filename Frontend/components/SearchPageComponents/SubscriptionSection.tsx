import React, { useEffect, useState } from "react";
import { useAuth } from "@/hooks/auths/authContext";
import { getMySubscription } from "@/services/SubscriptionService";
import UpgradeSection from "./UpgradeSection";
import SubscriptionInfo from "./SubscriptionInfo";
import { Spin } from "antd";

interface SubscriptionData {
  id: string;
  userId: string;
  subscriptionId: string;
  subscriptionName?: string;
  subscriptionPrice?: number;
  durationInMonths?: number;
  currency?: string;
  startDate: string;
  endDate: string;
  isActive: boolean;
  createdAt: string;
}

const SubscriptionSection: React.FC = () => {
  const { currentUser, loading: authLoading } = useAuth();
  const [subscription, setSubscription] = useState<SubscriptionData | null>(
    null
  );
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const fetchSubscription = async () => {
      if (!currentUser) {
        setLoading(false);
        return;
      }

      try {
        const idToken = await currentUser.getIdToken();
        const result = await getMySubscription(idToken);

        if (result) {
          setSubscription(result as unknown as SubscriptionData);
        }
      } catch (error) {
        console.error("Error fetching subscription:", error);
      } finally {
        setLoading(false);
      }
    };

    if (!authLoading) {
      fetchSubscription();
    }
  }, [currentUser, authLoading]);

  if (loading || authLoading) {
    return (
      <div
        style={{
          padding: "16px",
          borderTop: "1px solid #404040",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          flexShrink: 0,
          marginTop: "auto",
        }}
      >
        <Spin size="small" />
      </div>
    );
  }

  // Check if user has an active subscription
  const hasActiveSubscription = subscription && subscription.isActive;

  return hasActiveSubscription ? (
    <SubscriptionInfo subscription={subscription} />
  ) : (
    <UpgradeSection />
  );
};

export default SubscriptionSection;
