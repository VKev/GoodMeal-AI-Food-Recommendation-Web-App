"use client";
import { useState, useEffect } from 'react';
import Image from 'next/image';
import { investors } from '@/constants/Investor';

export default function Footer() {
    const [isVisible, setIsVisible] = useState(false);

    useEffect(() => {
        setIsVisible(true);
    }, []);    const footerLinks = {
        product: {
            title: "Sản phẩm",
            links: [
                { name: "Gợi ý AI", href: "/features" },
                { name: "Lập kế hoạch bữa ăn", href: "/meal-planning" },
                { name: "Đề xuất thông minh", href: "/smart-suggestions" },
                { name: "Theo dõi dinh dưỡng", href: "/nutrition" }
            ]
        },
        company: {
            title: "Công ty",
            links: [
                { name: "Về chúng tôi", href: "/about" },
                { name: "Tuyển dụng", href: "/careers" },
                { name: "Báo chí", href: "/press" },
                { name: "Liên hệ", href: "/contact" }
            ]
        },
        support: {
            title: "Hỗ trợ",
            links: [
                { name: "Trung tâm trợ giúp", href: "/help" },
                { name: "Cộng đồng", href: "/community" },
                { name: "Chính sách bảo mật", href: "/privacy" },
                { name: "Điều khoản dịch vụ", href: "/terms" }
            ]
        }
    };

    const socialLinks = [
        { name: "Twitter", icon: "𝕏", href: "#" },
        { name: "Instagram", icon: "📷", href: "#" },
        { name: "Facebook", icon: "📘", href: "#" },
        { name: "LinkedIn", icon: "💼", href: "#" }
    ];

    return (
        <footer className="relative min-h-screen bg-gradient-to-b from-black via-gray-900 to-black overflow-hidden">
            {/* Animated background elements */}
            <div className="absolute inset-0">
                {/* Floating orbs */}
                <div className="absolute top-10 left-10 w-64 h-64 bg-orange-500/5 rounded-full blur-3xl animate-pulse"></div>
                <div className="absolute bottom-10 right-10 w-80 h-80 bg-orange-600/3 rounded-full blur-3xl animate-pulse" style={{ animationDelay: '1s' }}></div>
                <div className="absolute top-1/3 left-1/2 w-40 h-40 bg-orange-400/4 rounded-full blur-2xl animate-pulse" style={{ animationDelay: '2s' }}></div>

                {/* Grid pattern */}
                <div className="absolute inset-0 opacity-5">
                    <div className="w-full h-full" style={{
                        backgroundImage: `radial-gradient(circle at 1px 1px, rgba(255,165,0,0.2) 1px, transparent 0)`,
                        backgroundSize: '40px 40px'
                    }}></div>
                </div>

                {/* Neural network lines */}
                <svg className="absolute inset-0 w-full h-full opacity-10">
                    <defs>
                        <linearGradient id="footer-gradient" x1="0%" y1="0%" x2="100%" y2="100%">
                            <stop offset="0%" stopColor="#f97316" stopOpacity="0.3"/>
                            <stop offset="100%" stopColor="#ea580c" stopOpacity="0.1"/>
                        </linearGradient>
                    </defs>
                    <path d="M0 50 Q200 80 400 60 T800 70" stroke="url(#footer-gradient)" strokeWidth="1" fill="none"/>
                    <path d="M100 150 Q300 130 500 160 T900 150" stroke="url(#footer-gradient)" strokeWidth="1" fill="none"/>
                </svg>
            </div>

            <div className="relative z-10 max-w-7xl mx-auto px-6 pt-16 pb-8 flex items-center min-h-screen">
                {/* Main Footer Content */}
                <div className={`w-full transition-all duration-1000 ${isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                    
                    {/* Investors Section - Auto Scroll */}
                    <div className="mb-16 space-y-6">
                        <div className="text-center">                            <h4 className="text-gray-400 text-sm font-medium tracking-wider uppercase mb-8">
                                Được tin tưởng bởi các nhà đầu tư hàng đầu trên thế giới
                            </h4>
                        </div>
                        
                        <div className="relative overflow-hidden">
                            <div className="flex animate-scroll">                                {/* First set of investors */}
                                {investors.map((investor, index) => (
                                    <div
                                        key={`first-${index}`}
                                        className="flex-none mx-6 flex items-center gap-4 px-8 py-6 bg-gray-800/30 border border-gray-700/30 rounded-xl backdrop-blur-sm hover:bg-gray-800/50 hover:border-orange-500/30 transition-all duration-300 group"
                                    >
                                        <div className="w-8 h-8 flex items-center justify-center bg-white rounded-md p-1">
                                            <Image 
                                                src={investor.darkLogo || investor.logo}
                                                alt={`${investor.name} logo`}
                                                width={32}
                                                height={32}
                                                className="w-full h-full object-contain filter brightness-0"
                                            />
                                        </div>
                                        <span className="text-gray-300 font-medium whitespace-nowrap group-hover:text-white transition-colors duration-300">
                                            {investor.name}
                                        </span>
                                    </div>
                                ))}                                {/* Duplicate set for seamless loop */}
                                {investors.map((investor, index) => (
                                    <div
                                        key={`second-${index}`}
                                        className="flex-none mx-6 flex items-center gap-4 px-8 py-6 bg-gray-800/30 border border-gray-700/30 rounded-xl backdrop-blur-sm hover:bg-gray-800/50 hover:border-orange-500/30 transition-all duration-300 group"
                                    >
                                        <div className="w-8 h-8 flex items-center justify-center bg-white rounded-md p-1">
                                            <Image 
                                                src={investor.darkLogo || investor.logo}
                                                alt={`${investor.name} logo`}
                                                width={32}
                                                height={32}
                                                className="w-full h-full object-contain filter brightness-0"
                                            />
                                        </div>
                                        <span className="text-gray-300 font-medium whitespace-nowrap group-hover:text-white transition-colors duration-300">
                                            {investor.name}
                                        </span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    </div>

                    {/* Top Section */}
                    <div className="grid lg:grid-cols-5 gap-12 mb-12">
                        {/* Brand Section */}
                        <div className="lg:col-span-2 space-y-6">
                            <div className="flex items-center gap-3">
                                <div className="w-10 h-10 bg-gradient-to-r from-orange-400 to-orange-600 rounded-xl flex items-center justify-center">
                                    <span className="text-white font-bold text-xl">🍽️</span>
                                </div>
                                <div>
                                    <h3 className="text-2xl font-bold">
                                        <span className="text-white">Good</span>
                                        <span className="bg-gradient-to-r from-orange-300 to-orange-400 bg-clip-text text-transparent">Meal</span>
                                    </h3>
                                </div>
                            </div>
                              <p className="text-gray-300 leading-relaxed max-w-md">
                                Khám phá bữa ăn hoàn hảo với các gợi ý AI thông minh được cá nhân hóa dành riêng cho bạn. 
                                Biến đổi trải nghiệm ẩm thực của bạn với những đề xuất cá nhân hóa.
                            </p>

                            {/* Newsletter */}                            <div className="space-y-3">
                                <h4 className="text-white font-semibold">Cập nhật thông tin</h4>
                                <div className="flex gap-3">
                                    <input 
                                        type="email" 
                                        placeholder="Nhập email của bạn"
                                        className="flex-1 bg-gray-800/60 border border-gray-700/50 rounded-xl px-4 py-3 text-white placeholder-gray-400 focus:outline-none focus:border-orange-500/50 transition-colors"
                                    />
                                    <button className="bg-gradient-to-r from-orange-400 to-orange-500 text-white px-6 py-3 rounded-xl font-semibold hover:from-orange-500 hover:to-orange-600 transition-all duration-300 hover:scale-105">
                                        Đăng ký
                                    </button>
                                </div>
                            </div>
                        </div>

                        {/* Links Sections */}
                        {Object.entries(footerLinks).map(([key, section]) => (
                            <div key={key} className="space-y-4">
                                <h4 className="text-white font-semibold text-lg">{section.title}</h4>
                                <ul className="space-y-3">
                                    {section.links.map((link, index) => (
                                        <li key={index}>
                                            <a 
                                                href={link.href}
                                                className="text-gray-400 hover:text-orange-300 transition-colors duration-300 flex items-center gap-2 group"
                                            >
                                                <span className="group-hover:translate-x-1 transition-transform duration-300">
                                                    {link.name}
                                                </span>
                                            </a>
                                        </li>
                                    ))}
                                </ul>
                            </div>
                        ))}
                    </div>

                    {/* Divider */}
                    <div className="border-t border-gray-800/50 mb-8"></div>

                    {/* Bottom Section */}
                    <div className="flex flex-col md:flex-row justify-between items-center gap-6">
                        {/* Copyright */}                        <div className="text-gray-400 text-sm">
                            © 2025 GoodMeal. Bảo lưu mọi quyền. Được hỗ trợ bởi AI.
                        </div>

                        {/* Social Links */}                        <div className="flex items-center gap-4">
                            <span className="text-gray-400 text-sm mr-2">Theo dõi chúng tôi:</span>
                            {socialLinks.map((social, index) => (
                                <a
                                    key={index}
                                    href={social.href}
                                    className="w-10 h-10 bg-gray-800/60 hover:bg-orange-500/20 border border-gray-700/50 hover:border-orange-500/50 rounded-xl flex items-center justify-center transition-all duration-300 hover:scale-110 group"
                                    aria-label={social.name}
                                >
                                    <span className="text-gray-400 group-hover:text-orange-300 transition-colors duration-300">
                                        {social.icon}
                                    </span>
                                </a>
                            ))}
                        </div>                        {/* App Store Badges */}
                        <div className="flex items-center gap-3">
                            <a
                                href="https://play.google.com/store"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="transform transition-all duration-300 hover:scale-110 hover:brightness-110"
                            >                                <Image
                                    src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                                    alt="Tải về trên Google Play"
                                    width={120}
                                    height={40}
                                    className="h-10 filter drop-shadow-lg"
                                />
                            </a>
                            <a
                                href="https://www.apple.com/app-store/"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="transform transition-all duration-300 hover:scale-110 hover:brightness-110"
                            >                                <Image
                                    src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                                    alt="Tải về trên App Store"
                                    width={120}
                                    height={40}
                                    className="h-10 filter drop-shadow-lg"
                                />
                            </a>
                        </div>
                    </div>
                </div>

                {/* Animated particles */}
                <div className="absolute inset-0 overflow-hidden pointer-events-none">
                    {[...Array(4)].map((_, i) => (
                        <div
                            key={i}
                            className="absolute w-1 h-1 bg-orange-400/30 rounded-full animate-pulse"
                            style={{
                                left: `${20 + i * 20}%`,
                                top: `${30 + i * 15}%`,
                                animationDelay: `${i * 0.8}s`,
                                animationDuration: `${4 + i}s`
                            }}
                        />
                    ))}
                </div>
            </div>

            {/* Bottom gradient */}
            <div className="absolute bottom-0 left-0 right-0 h-20 bg-gradient-to-t from-orange-900/5 to-transparent"></div>

            {/* Custom Scroll Animation */}
            <style jsx>{`
                @keyframes scroll {
                    0% {
                        transform: translateX(0);
                    }
                    100% {
                        transform: translateX(-50%);
                    }
                }
                
                .animate-scroll {
                    animation: scroll 30s linear infinite;
                }
                
                .animate-scroll:hover {
                    animation-play-state: paused;
                }
            `}</style>
        </footer>
    );
}