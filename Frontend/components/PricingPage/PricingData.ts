export interface PricingPlan {
    id: string;
    name: string;
    monthlyPrice: string;
    yearlyPrice: string;
    description: string;
    features: string[];
    popular: boolean;
    buttonText: string;
    buttonType: 'default' | 'primary';
    color: string;
    icon: string;
}

export const pricingPlans: PricingPlan[] = [
    {
        id: 'free',
        name: 'Free',
        monthlyPrice: '$0',
        yearlyPrice: '$0',
        description: 'Explore basic AI food recommendation features',
        features: [
            '3 AI Bot chats per day',
            'Food suggestions based on emotions & situations',
            'View food images from recommendations',
            'Find restaurants on Google Maps',
            'Save up to 5 favorite dishes',
            'Email support',
            'Basic web app access'
        ],
        popular: false,
        buttonText: 'Your current plan',
        buttonType: 'default',
        color: '#52c41a',
        icon: '🆓'
    },
    {
        id: 'pro',
        name: 'Pro',
        monthlyPrice: '$19',
        yearlyPrice: '$15',
        description: 'Unlimited AI food recommendation experience',
        features: [
            'Unlimited AI Bot chats',
            'Smart AI food suggestions using natural language',
            'Emotion analysis & personalized food recommendations',
            'View HD images of all dishes',
            'Google Maps integration with restaurant reviews',
            'Save unlimited favorite dishes',
            'Chat history & personalized suggestions',
            'Share dishes with friends',
            '24/7 support',
            'Mobile app access',
            'Export food lists to PDF'
        ],
        popular: true,
        buttonText: 'Upgrade to Pro',
        buttonType: 'primary',
        color: '#ff7a00',
        icon: '⭐'
    },
    {
        id: 'business',
        name: 'Business',
        monthlyPrice: '$49',
        yearlyPrice: '$39',
        description: 'Complete solution for restaurants & businesses',
        features: [
            'All Pro features',
            'Restaurant advertising registration',
            'Priority display in recommendation results',
            'Restaurant menu management system',
            'Customer data & trend analysis',
            'POS system API integration',
            'Brand customization in app',
            'Detailed view & interaction reports',
            'Multiple restaurant management',
            'Priority support & expert consultation',
            'Daily data backup'
        ],
        popular: false,
        buttonText: 'Contact Sales',
        buttonType: 'default',
        color: '#722ed1',
        icon: '🏢'
    }
];
