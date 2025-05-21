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

            {/* Background xám mờ dần từ phải sang trái */}
            <div
                style={{
                    position: "absolute",
                    top: 0,
                    right: 0,
                    width: "50%",
                    height: "100%",
                    background: "linear-gradient(to left, rgba(128, 128, 128, 0.5), rgba(128, 128, 128, 0))",
                    zIndex: 0,
                }}
            ></div>
           

        </div>
    );
}