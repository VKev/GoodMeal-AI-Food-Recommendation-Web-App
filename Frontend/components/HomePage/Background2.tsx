"use client"
import React, { useState, useEffect, useRef } from 'react';
import en from '../../locales/en';

interface SpawnTextProps {
    text: string;
    delay?: number;
    className?: string;
}

const SpawnText: React.FC<SpawnTextProps> = ({ text, delay = 0, className = "" }) => {
    const [visibleWords, setVisibleWords] = useState(0);
    const words = text.split(' ');

    useEffect(() => {
        const timer = setTimeout(() => {
            const interval = setInterval(() => {
                setVisibleWords(prev => {
                    if (prev >= words.length) {
                        clearInterval(interval);
                        return prev;
                    }
                    return prev + 1;
                });
            }, 100);
            return () => clearInterval(interval);
        }, delay);
        return () => clearTimeout(timer);
    }, [words.length, delay]);    return (
        <span className={className}>
            {words.map((word: string, index: number) => (
                <span
                    key={index}
                    className={`inline-block transition-all duration-500 ${
                        index < visibleWords 
                            ? 'opacity-100 transform translate-y-0' 
                            : 'opacity-0 transform translate-y-4'
                    }`}                    style={{ 
                        transitionDelay: `${index * 30}ms`,
                        marginRight: '0.5rem'
                    }}
                >
                    {word}
                </span>
            ))}
        </span>
    );
};

const useInView = (): [React.RefObject<HTMLDivElement | null>, boolean] => {
    const [isInView, setIsInView] = useState(false);
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => {
                if (entry.isIntersecting) {
                    setIsInView(true);
                }
            },
            { threshold: 0.1 }
        );

        if (ref.current) {
            observer.observe(ref.current);
        }

        return () => observer.disconnect();
    }, []);

    return [ref, isInView];
};

export default function BusinessSection() {
    const [sectionRef, isVisible] = useInView();
    const [activeCard, setActiveCard] = useState(0);
    const t = en.businessSection;

    // AI-powered business benefits for natural language suggestion platform
    const businessBenefits = t.benefits;    useEffect(() => {
        const interval = setInterval(() => {
            setActiveCard(prev => (prev + 1) % businessBenefits.length);
        }, 3000);
        return () => clearInterval(interval);
    }, [businessBenefits.length]);

    return (
        <div ref={sectionRef} className="max-h-screen py-24 bg-gradient-to-b from-gray-900 via-black to-gray-900 relative overflow-hidden">
            {/* Dynamic floating elements */}
            <div className="absolute inset-0">
                {/* Neural network pattern */}
                <div className="absolute top-10 left-10 w-1 h-1 bg-orange-400 rounded-full animate-ping opacity-60"></div>
                <div className="absolute top-20 left-32 w-1 h-1 bg-orange-500 rounded-full animate-ping opacity-40" style={{animationDelay: '1s'}}></div>
                <div className="absolute top-32 left-20 w-1 h-1 bg-orange-300 rounded-full animate-ping opacity-80" style={{animationDelay: '2s'}}></div>
                
                {/* Connecting lines */}
                <svg className="absolute inset-0 w-full h-full opacity-10">
                    <defs>
                        <linearGradient id="line-gradient" x1="0%" y1="0%" x2="100%" y2="100%">
                            <stop offset="0%" stopColor="#f97316" stopOpacity="0.5"/>
                            <stop offset="100%" stopColor="#ea580c" stopOpacity="0.1"/>
                        </linearGradient>
                    </defs>
                    <path d="M50 80 Q200 120 350 100 T650 90" stroke="url(#line-gradient)" strokeWidth="2" fill="none"/>
                    <path d="M100 200 Q300 180 500 220 T800 200" stroke="url(#line-gradient)" strokeWidth="1" fill="none"/>
                </svg>

                {/* Glowing orb */}
                <div className="absolute top-1/2 right-20 w-48 h-48 bg-orange-500/5 rounded-full blur-3xl animate-pulse"></div>
            </div>

            <div className="relative z-10 max-w-7xl mx-auto px-6">
                {/* Main Content Grid */}
                <div className="grid lg:grid-cols-2 gap-16 items-center">
                    
                    {/* Left: Content */}
                    <div className="space-y-8">
                        <div className={`transition-all duration-1000 ${isVisible ? 'opacity-100 translate-x-0' : 'opacity-0 -translate-x-12'}`}>
                            <div className="inline-flex items-center gap-2 bg-orange-500/10 border border-orange-500/20 rounded-full px-4 py-2 mb-6">
                                <span className="w-2 h-2 bg-orange-400 rounded-full animate-pulse"></span>
                                <span className="text-orange-300 text-sm font-medium">{t.aiPlatform}</span>
                            </div>                            <h2 className="text-5xl lg:text-6xl font-bold mb-6 leading-tight">
                                <span className="bg-gradient-to-r from-white to-gray-300 bg-clip-text text-transparent text-white">
                                    <SpawnText text={t.header} delay={isVisible ? 200 : 0} />
                                </span>
                                <br />
                                <span className="bg-gradient-to-r from-orange-400 to-orange-600 bg-clip-text text-transparent text-white">
                                    <SpawnText text={t.subHeader} delay={isVisible ? 1000 : 0} />
                                </span>
                            </h2>
                            
                            <p className="text-xl text-gray-300 leading-relaxed mb-8">
                                <SpawnText text={t.description} delay={isVisible ? 2000 : 0} />
                            </p>
                        </div>

                        {/* Stats */}
                        <div className={`grid grid-cols-3 gap-6 transition-all duration-1000 delay-500 ${isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                            {t.stats.map((stat, idx) => (
                                <div className="text-center" key={idx}>
                                    <div className="text-3xl font-bold text-orange-300 mb-1">{stat.value}</div>
                                    <div className="text-sm text-gray-400">{stat.label}</div>
                                </div>
                            ))}
                        </div>

                        {/* CTA */}
                        <div className={`flex flex-col sm:flex-row gap-4 transition-all duration-1000 delay-1000 ${isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                            <button className="group relative bg-gradient-to-r from-orange-300 to-orange-400 text-white px-8 py-4 rounded-xl font-bold text-lg overflow-hidden transition-all duration-300 hover:scale-105 shadow-lg shadow-orange-500/25">
                                <div className="absolute inset-0 bg-gradient-to-r from-orange-200 to-orange-300 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                <span className="relative flex items-center justify-center gap-3">
                                    {t.ctaRegister}
                                </span>
                            </button>
                        </div>
                    </div>

                    {/* Right: Interactive Benefits */}
                    <div className="relative">
                        <div className={`transition-all duration-1000 delay-300 ${isVisible ? 'opacity-100 translate-x-0' : 'opacity-0 translate-x-12'}`}>
                            {/* Main feature card */}
                            <div className="relative bg-gradient-to-br from-gray-800/60 to-gray-900/60 backdrop-blur-lg rounded-2xl p-8 border border-orange-500/20 mb-6">
                                <div className="absolute inset-0 bg-gradient-to-br from-orange-500/5 to-transparent rounded-2xl"></div>
                                <div className="relative">
                                    <div className="flex items-center gap-4 mb-4">
                                        <div className="text-4xl">{businessBenefits[activeCard].icon}</div>
                                        <div>
                                            <div className="text-xs text-orange-400 font-medium mb-1">{businessBenefits[activeCard].feature}</div>
                                            <h3 className="text-xl font-bold text-white">{businessBenefits[activeCard].title}</h3>
                                        </div>
                                    </div>
                                    <p className="text-gray-300 leading-relaxed">{businessBenefits[activeCard].desc}</p>
                                </div>
                            </div>

                            {/* Benefits indicators */}
                            <div className="grid grid-cols-4 gap-3">
                                {businessBenefits.map((benefit, index) => (
                                    <button
                                        key={index}
                                        onClick={() => setActiveCard(index)}
                                        className={`p-4 rounded-xl transition-all duration-300 border ${
                                            index === activeCard 
                                                ? 'bg-orange-500/20 border-orange-500/40 scale-105' 
                                                : 'bg-gray-800/40 border-gray-700/40 hover:border-orange-500/20'
                                        }`}
                                    >
                                        <div className="text-2xl mb-2">{benefit.icon}</div>
                                        <div className="text-xs text-gray-400 font-medium">{benefit.feature}</div>
                                    </button>
                                ))}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Bottom trust bar */}
                <div className={`mt-16 pt-8 border-t border-gray-800 transition-all duration-1000 delay-1200 ${isVisible ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-4'}`}>
                    <div className="flex flex-wrap justify-center items-center gap-8 text-sm text-gray-500">
                        {t.trust.map((item, idx) => (
                            <span className="flex items-center gap-2" key={idx}>
                                <span className={`w-2 h-2 ${item.color} rounded-full`}></span>
                                {item.text}
                            </span>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
}