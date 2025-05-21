"use client";
import Background from "@/components/Background";
import ReactFullpage from "@fullpage/react-fullpage";

export default function Home() {
  return (
    <>
      <ReactFullpage
        scrollingSpeed={1000}
        render={() => {
          return (
            <ReactFullpage.Wrapper>
              <div className="section">
                <div className="content-wrapper">
                  <Background />

                </div>
              </div>
              <div className="section">
                <div className="content-wrapper" style={{ width: "100vw", height: "100vh" }}>
                  <img
                    src="/banner.png"
                    alt="Food Banner"
                    className="w-full h-full object-cover rounded-lg shadow-md"
                  />
                </div>
              </div>
              <div className="section">
                <div className="content-wrapper" style={{ width: "100vw", height: "100vh" }}>
                  <img
                    src="/banner1.png"
                    alt="Food Banner"
                    className="w-full h-full object-cover rounded-lg shadow-md"
                  />
                </div>
              </div>
            </ReactFullpage.Wrapper>
          );
        }} credits={{
          enabled: undefined,
          label: "",
          position: undefined
        }} />
    </>
  );
}