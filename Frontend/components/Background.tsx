"use client";

import Login from "./Login";

export default function Background() {
    return (
        <div style={{ position: "relative", height: "100vh", overflow: "hidden" }}>
            <video
                autoPlay
                loop
                muted
                style={{
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%",
                    height: "100%",
                    objectFit: "cover",
                    zIndex: -1,
                }}
            >
                <source src="https://s3.ap-southeast-1.wasabisys.com/khangstorage/public/banner_food.mp4" type="video/mp4" />
                Your browser does not support the video tag.
            </video>

            {/* Background xám mờ dần từ phải sang trái */}
            <div
                style={{
                    position: "absolute",
                    top: 0,
                    right: 0,
                    width: "30%",
                    height: "100%",
                    background: "white",
                    zIndex: 0,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                }}
            >
                <div style={{ width: "100%", padding: "20px" }}>
                    <Login />
                </div>
            </div>
        </div>
    );
}