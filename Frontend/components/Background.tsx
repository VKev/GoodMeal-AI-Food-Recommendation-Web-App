"use client";

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

            {/* Overlay gradient background */}
            <div
                style={{
                    position: "absolute",
                    top: 0,
                    left: 0,
                    width: "100%",
                    height: "100%",
                    zIndex: 0,
                    pointerEvents: "none",
                    background: "linear-gradient(to bottom, rgba(0, 0, 0, 0.1) 0%, rgba(0, 0, 0, 0.62) 100%)",
                }}
            />

            {/* Text content */}
            <div
                style={{
                    position: "absolute",
                    top: "60%",
                    left: "50%",
                    transform: "translate(-50%, -50%)",
                    textAlign: "center",
                    zIndex: 1,
                }}
            >
                <h1
                    style={{
                        fontSize: "5rem", // Chữ lớn hơn
                        fontWeight: "bold",
                        margin: 0,
                        background: "linear-gradient(to right, #ffffff, #e0e0e0)", // Gradient màu trắng
                        WebkitBackgroundClip: "text",
                        WebkitTextFillColor: "transparent", // Hiệu ứng gradient
                    }}
                >
                    GoodMeal
                </h1>
                <p
                    style={{
                        fontSize: "2rem", // Chữ lớn hơn
                        margin: 0,
                        background: "linear-gradient(to right, #ffffff, #e0e0e0)", // Gradient màu trắng
                        WebkitBackgroundClip: "text",
                        WebkitTextFillColor: "transparent", // Hiệu ứng gradient
                    }}
                >
                    Your AI Food Recommendation App
                </p>
                <a
                    href="/sign-in"
                    style={{
                        display: "inline-flex",
                        alignItems: "center",
                        background: "#fff",
                        color: "#111",
                        fontWeight: "500",
                        fontSize: "1rem",
                        border: "none",
                        borderRadius: "999px",
                        padding: "12px 36px",
                        margin: "32px 0 0 0",
                        cursor: "pointer",
                        boxShadow: "0 2px 8px rgba(0,0,0,0.10)",
                        textDecoration: "none",
                        transition: "background 0.2s",
                        gap: "10px",
                    }}
                >
                    Start now
                    <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        fill="none"
                        viewBox="0 0 24 24"
                        style={{ display: "inline", verticalAlign: "middle" }}
                    >
                        <path
                            d="M7 17L17 7M17 7H7M17 7V17"
                            stroke="#111"
                            strokeWidth="2"
                            strokeLinecap="round"
                            strokeLinejoin="round"
                        />
                    </svg>
                </a>
                <div style={{ display: "flex", justifyContent: "center", gap: "20px", marginTop: "20px" }}>
                    <a href="https://play.google.com/store" target="_blank" rel="noopener noreferrer">
                        <img
                            src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                            alt="Get it on Google Play"
                            style={{ height: "60px" }}
                        />
                    </a>
                    <a href="https://www.apple.com/app-store/" target="_blank" rel="noopener noreferrer">
                        <img
                            src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                            alt="Download on the App Store"
                            style={{ height: "60px" }}
                        />
                    </a>
                    
                </div>
            </div>
            {/* Scroll down indicator */}
            <div
                style={{
                    position: "absolute",
                    left: "50%",
                    bottom: "32px",
                    transform: "translateX(-50%)",
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    zIndex: 2,
                    pointerEvents: "none",
                    userSelect: "none",
                }}
            >
                <span
                    style={{
                        color: "#fff",
                        fontSize: "1.2rem",
                        marginBottom: "8px",
                        textShadow: "0 2px 8px rgba(0,0,0,0.4)",
                        letterSpacing: "1px",
                        fontWeight: 500,
                        animation: "scrollTextBounce 2s infinite",
                    }}
                >
                    Scroll down
                </span>
                <svg
                    width="32"
                    height="32"
                    viewBox="0 0 24 24"
                    fill="none"
                    style={{
                        animation: "arrowBounce 2s infinite",
                        filter: "drop-shadow(0 2px 8px rgba(0,0,0,0.4))",
                    }}
                >
                    <path d="M12 5v14M12 19l-6-6M12 19l6-6" stroke="#fff" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                </svg>
            </div>
            <style>
                {`
                @keyframes arrowBounce {
                    0%, 100% { transform: translateY(0);}
                    50% { transform: translateY(12px);}
                }
                @keyframes scrollTextBounce {
                    0%, 100% { transform: translateY(0);}
                    50% { transform: translateY(4px);}
                }
                `}
            </style>
        </div>
    );
}