"use client";
import React from 'react';
import { Button } from 'antd';
import { DownloadOutlined, RocketOutlined } from '@ant-design/icons';
import SplitText from '@/Reactbits/SplitText/SplitText';
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

function Content() {
    const settings = {
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 4,
        slidesToScroll: 2,
        autoplay: true,
        autoplaySpeed: 2000,
        pauseOnHover: true,
        arrows: false,
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-[8vh] bg-gradient-to-br from-green-100 via-white to-yellow-100 rounded-xl shadow-lg p-10">
            <SplitText
                text="Hello, I'm GoodMeal — your smart food assistant."
                className="text-4xl font-semibold text-center"
                delay={50}
                animationFrom={{ opacity: 0, transform: 'translate3d(0,50px,0)' }}
                animationTo={{ opacity: 1, transform: 'translate3d(0,0,0)' }}
                threshold={0.2}
                rootMargin="-50px"
            />

            <div className="w-full max-w-7xl mt-6">
                <Slider {...settings}>
                    <div>
                        <img
                            src="/ga.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/tom1.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/thit.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/mi.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                </Slider>
            </div>
            <div className="w-full max-w-7xl mt-3">
                <Slider {...settings}>
                    <div>
                        <img
                            src="/pho.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/lau.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/sushi.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                    <div>
                        <img
                            src="/mi.jpg"
                            alt="Food Banner"
                            className="w-full h-64 object-cover rounded-lg shadow-md"
                        />
                    </div>
                </Slider>
            </div>

            <div className="flex gap-4 mt-4">
                <Button
                    type="default"
                    size="large"
                    icon={<RocketOutlined />}
                    className="rounded-full px-6 h-10 font-semibold bg-yellow-200 text-black hover:bg-yellow-300 transition-all duration-200 flex items-center"
                >
                    Start now
                </Button>
                <Button
                    type="text"
                    size="large"
                    icon={<DownloadOutlined />}
                    className="rounded-full px-6 h-10 font-semibold text-black hover:text-yellow-600 transition-all duration-200 flex items-center"
                >
                    Download the app
                </Button>
            </div>
        </div>
    );
}

export default Content;