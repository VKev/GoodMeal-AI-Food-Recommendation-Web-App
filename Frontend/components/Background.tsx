"use client";
import { useState, useEffect } from 'react';
import Image from 'next/image';

export default function Background() {
    const [isLoaded, setIsLoaded] = useState(true);
    const [scrollY, setScrollY] = useState(0);

    useEffect(() => {
        const handleScroll = () => setScrollY(window.scrollY);
        window.addEventListener('scroll', handleScroll);
        
        // Đảm bảo animation luôn chạy khi component mount
        const timer = setTimeout(() => {
            setIsLoaded(true);
        }, 100);

        return () => {
            window.removeEventListener('scroll', handleScroll);
            clearTimeout(timer);
        };
    }, []);

    return (
        <div className="relative h-screen overflow-hidden bg-black">
            {/* Video Background */}
            <video
                autoPlay
                loop
                muted
                playsInline
                onLoadedData={() => setIsLoaded(true)}
                className="absolute inset-0 w-full h-full object-cover scale-105"
                style={{ transform: `translateY(${scrollY * 0.5}px) scale(1.05)` }}
            >
                <source src="https://s3.ap-southeast-1.wasabisys.com/khangstorage/public/banner_food.mp4" type="video/mp4" />
                Your browser does not support the video tag.
            </video>

            {/* Sophisticated Overlay */}
            <div className="absolute inset-0 bg-gradient-to-b from-black/20 via-black/40 to-black/80" />
            <div className="absolute inset-0 bg-gradient-to-r from-black/30 via-transparent to-black/30" />

            {/* Animated particles effect */}
            <div className="absolute inset-0 overflow-hidden pointer-events-none">
                {[...Array(6)].map((_, i) => (
                    <div
                        key={i}
                        className="absolute w-2 h-2 bg-white/20 rounded-full animate-pulse"
                        style={{
                            left: `${20 + i * 15}%`,
                            top: `${30 + i * 10}%`,
                            animationDelay: `${i * 0.5}s`,
                            animationDuration: `${3 + i}s`
                        }}
                    />
                ))}
            </div>

            {/* Main Content */}
            <div className={`absolute inset-0 flex items-center justify-center transition-all duration-1000 ${isLoaded ? 'opacity-100 translate-y-0' : 'opacity-0 translate-y-8'}`}>
                <div className="text-center px-6 max-w-4xl mx-auto">

                    {/* Main Heading */}
                    <h1 className={`text-6xl md:text-8xl lg:text-9xl font-black mb-6 ${isLoaded ? 'animate-fadeInUp animation-delay-200' : 'opacity-0'}`}>
                        <span className="bg-gradient-to-r from-white via-white to-white/80 bg-clip-text text-transparent drop-shadow-2xl">
                            Good
                        </span>
                        <span className="bg-gradient-to-r from-orange-200 via-orange-300 to-orange-400 bg-clip-text text-transparent">
                            Meal
                        </span>
                    </h1>

                    {/* Subtitle */}
                    <p className={`text-xl md:text-2xl lg:text-3xl text-white/90 font-light mb-12 max-w-2xl mx-auto leading-relaxed ${isLoaded ? 'animate-fadeInUp animation-delay-400' : 'opacity-0'}`}>
                        Discover your perfect meal with intelligent AI recommendations tailored just for you
                    </p>

                    {/* CTA Button */}
                    <div className={`${isLoaded ? 'animate-fadeInUp animation-delay-600' : 'opacity-0'}`}>
                        <a
                            href="/sign-in"
                            className="group relative inline-flex items-center gap-3 bg-white text-black font-semibold text-lg px-8 py-4 rounded-full transition-all duration-300 hover:bg-orange-400 hover:text-black hover:scale-105 shadow-2xl hover:shadow-orange-400/25"
                        >
                            <span>Start now</span>
                            <svg
                                className="w-5 h-5 transition-transform duration-300 group-hover:translate-x-1 group-hover:-translate-y-1"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke="currentColor"
                            >
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 17L17 7M17 7H7M17 7V17" />
                            </svg>

                            {/* Animated border */}
                            <div className="absolute inset-0 rounded-full bg-gradient-to-r from-orange-400 via-orange-300 to-orange-400 opacity-0 group-hover:opacity-100 transition-opacity duration-300 -z-10 blur-xl" />
                        </a>
                    </div>

                    {/* App Store Badges */}
                    <div className={`flex flex-col sm:flex-row items-center justify-center gap-4 mt-12 ${isLoaded ? 'animate-fadeInUp animation-delay-800' : 'opacity-0'}`}>                        <div className="flex items-center gap-4">
                            <a
                                href="https://play.google.com/store"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="transform transition-all duration-300 hover:scale-110 hover:brightness-110"
                            >
                                <Image
                                    src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                                    alt="Get it on Google Play"
                                    width={144}
                                    height={48}
                                    className="h-12 filter drop-shadow-lg"
                                />
                            </a>
                            <a
                                href="https://www.apple.com/app-store/"
                                target="_blank"
                                rel="noopener noreferrer"
                                className="transform transition-all duration-300 hover:scale-110 hover:brightness-110"
                            >
                                <Image
                                    src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                                    alt="Download on the App Store"
                                    width={144}
                                    height={48}
                                    className="h-12 filter drop-shadow-lg"
                                />
                            </a>
                        </div>
                    </div>
                </div>
            </div>

            {/* Scroll Indicator */}
            <div className="absolute bottom-8 left-1/2 transform -translate-x-1/2 flex flex-col items-center text-white/80 animate-bounce">
                <span className="text-sm font-medium tracking-widest mb-2 opacity-75">EXPLORE</span>
                <div className="w-6 h-10 border-2 border-white/40 rounded-full flex justify-center">
                    <div className="w-1 h-3 bg-white/60 rounded-full mt-2 animate-pulse" />
                </div>
            </div>

            {/* Custom Animations */}
            <style jsx>{`
                @keyframes fadeInUp {
                    from {
                        opacity: 0;
                        transform: translateY(30px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
                
                .animate-fadeInUp {
                    animation: fadeInUp 0.8s ease-out forwards;
                }
                
                .animation-delay-200 {
                    animation-delay: 0.2s;
                }
                
                .animation-delay-400 {
                    animation-delay: 0.4s;
                }
                
                .animation-delay-600 {
                    animation-delay: 0.6s;
                }
                
                .animation-delay-800 {
                    animation-delay: 0.8s;
                }
            `}</style>
        </div>
    );
}