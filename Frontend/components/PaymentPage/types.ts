import React from 'react';

export interface PaymentMethod {
    id: string;
    name: string;
    icon: React.ReactNode;
    description: string;
}

export interface OrderSummaryProps {
    selectedPlan: any;
    isYearly: boolean;
    currentPrice: string;
    period: string;
    savings: string;
}

export interface PaymentFormProps {
    selectedPayment: string;
    setSelectedPayment: (value: string) => void;
    isProcessing: boolean;
    onPayment: () => void;
    currentPrice: string;
    period: string;
    paymentMethods: PaymentMethod[];
}
