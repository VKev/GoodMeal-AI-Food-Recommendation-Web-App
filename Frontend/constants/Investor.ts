export interface Investor {
    name: string;
    logo: string;
    darkLogo?: string;
}

export const investors: Investor[] = [
    { 
        name: "OpenAI", 
        logo: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/openai/openai-original.svg",
        darkLogo: "https://upload.wikimedia.org/wikipedia/commons/0/04/ChatGPT_logo.svg"
    },
    { 
        name: "Google Ventures", 
        logo: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/google/google-original.svg",
        darkLogo: "https://www.google.com/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png"
    },
    { 
        name: "Microsoft", 
        logo: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/microsoft/microsoft-original.svg",
        darkLogo: "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE1Mu3b?ver=5c31"
    },
    { 
        name: "Amazon", 
        logo: "https://cdn.jsdelivr.net/gh/devicons/devicon/icons/amazonwebservices/amazonwebservices-plain-wordmark.svg",
        darkLogo: "https://d1.awsstatic.com/logos/aws-logo-lockups/poweredbyaws/PB_AWS_logo_RGB_REV_SQ.8c88ac215fe4e441dc42865dd6962ed4f444a90d.png"
    },
];