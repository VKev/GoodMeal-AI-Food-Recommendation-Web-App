"use client"
import React, { useState, useEffect } from 'react';
import Image from 'next/image';
import en from '../../locales/en';

type SpawnTextProps = {
    text: string;
    delay?: number;
    className?: string;
};

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
            }, 60); // Nhanh hơn: mỗi từ xuất hiện sau 60ms

            return () => clearInterval(interval);
        }, delay);

        return () => clearTimeout(timer);
    }, [words.length, delay]);    return (
        <span className={className}>
            {words.map((word: string, index: number) => (
                <span
                    key={index}
                    className={`inline-block transition-all duration-500 ${index < visibleWords
                        ? 'opacity-100 transform translate-y-0'
                        : 'opacity-0 transform translate-y-4'
                        }`}
                    style={{
                        transitionDelay: `${index * 20}ms`,
                        marginRight: index === words.length - 1 ? 0 : '0.8rem' // Tăng khoảng cách giữa các chữ
                    }}
                >
                    {word}
                </span>
            ))}
        </span>
    );
};

export default function Background() {
    const [isLoaded, setIsLoaded] = useState(false);
    const [isImageLoaded, setIsImageLoaded] = useState(false); // Thêm state mới
    const t = en.businessSection;

    useEffect(() => {
        setIsLoaded(true);
        const timer = setTimeout(() => setIsImageLoaded(true), 1500); // Delay xuất hiện hình
        return () => clearTimeout(timer);
    }, []);

    return (
        <div className="max-h-screen bg-gradient-to-br from-black via-black to-gray-700 relative overflow-hidden">
            {/* Animated background elements */}
            <div className="absolute inset-0">
                {/* Floating orbs */}
                <div className="absolute top-20 left-10 w-72 h-72 bg-orange-500/10 rounded-full blur-3xl animate-pulse"></div>
                <div className="absolute bottom-20 right-10 w-96 h-96 bg-orange-600/5 rounded-full blur-3xl animate-pulse" style={{ animationDelay: '1s' }}></div>
                <div className="absolute top-1/2 left-1/3 w-48 h-48 bg-orange-400/8 rounded-full blur-2xl animate-pulse" style={{ animationDelay: '2s' }}></div>

                {/* Grid pattern */}
                <div className="absolute inset-0 opacity-5">
                    <div className="w-full h-full" style={{
                        backgroundImage: `radial-gradient(circle at 1px 1px, rgba(255,165,0,0.3) 1px, transparent 0)`,
                        backgroundSize: '50px 50px'
                    }}></div>
                </div>
            </div>

            <div className="relative z-10 grid lg:grid-cols-2 min-h-screen items-center">
                {/* Left Side - Content */}
                <div className="flex flex-col justify-center px-8 md:px-16 lg:px-20 py-12">
                    <div className="max-w-2xl space-y-4 ml-20">
                        {/* Main Heading with spawn effect */}
                        <div className="inline-flex items-center gap-2 bg-orange-500/10 border border-orange-500/20 rounded-full px-4 py-2 mb-6">
                            <span className="w-2 h-2 bg-orange-400 rounded-full animate-pulse"></span>
                            <span className="text-orange-300 text-sm font-medium">{t.aiPlatform}</span>
                        </div>
                        <div className="space-y-6">
                            <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold leading-tight text-white">
                                <SpawnText text={en.helpTitle} />
                            </h1>

                            <div className={`transition-all duration-1000 delay-1000 ${isLoaded ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                                <p className="text-lg md:text-xl text-gray-300 leading-relaxed font-medium">
                                    <SpawnText text={en.helpDescription} delay={2000} />
                                </p>
                            </div>
                        </div>



                        <div className={`flex flex-col sm:flex-row gap-4 pt-4 transition-all duration-1000 delay-[3000ms] ${isLoaded ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                            <button className="group relative bg-gradient-to-r from-orange-400 to-orange-300 text-white px-8 py-4 rounded-2xl font-semibold text-lg transition-all duration-300 hover:from-orange-400 hover:to-orange-500 hover:scale-105 shadow-lg shadow-orange-500/25 overflow-hidden">
                                <div className="absolute inset-0 bg-gradient-to-r from-orange-300 to-orange-200 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                <span className="relative flex items-center justify-center gap-3">
                                    Get Started
                                    <span className="transition-transform duration-300 group-hover:translate-x-1">→</span>
                                </span>
                            </button>

                            <button className="group relative px-8 py-4 rounded-2xl font-semibold text-lg text-orange-300 border-2 border-orange-600/50 hover:border-orange-500 hover:text-orange-200 transition-all duration-300 hover:scale-105 overflow-hidden">
                                <div className="absolute inset-0 bg-gradient-to-r from-orange-500/10 to-orange-600/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                                <span className="relative flex items-center justify-center gap-3">
                                    Learn More
                                    <span className="transition-transform duration-300 group-hover:rotate-12">↗</span>
                                </span>
                            </button>
                        </div>



                    </div>
                </div>

                {/* Right Side - Visual Elements */}
                <div className="flex items-center justify-center p-8 lg:p-16">
                    <div className={`relative transition-all duration-1000 delay-1000 ${isImageLoaded ? 'opacity-100 scale-100' : 'opacity-0 scale-95'}`}>
                        {/* Glowing background */}
                        <div className="absolute -inset-4 rounded-3xl blur-2xl"></div>                        {/* Main image container */}
                        <div className="relative  rounded-2xl p-1 ">
                            <Image
                                src="/app.png"
                                alt="App Screenshot"
                                width={800}
                                height={600}
                                className="rounded-xl shadow-2xl  w-full max-w-3xl mx-auto"
                            />
                        </div>
                    </div>
                </div>
            </div>

            {/* Bottom gradient */}
            <div className="absolute bottom-0 left-0 right-0 h-32 bg-gradient-to-t from-orange-900/10 to-transparent"></div>
        </div>
    );
}