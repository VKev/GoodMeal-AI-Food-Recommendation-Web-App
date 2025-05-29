"use client";
import Background from "@/components/HomePage/Background";
import Background1 from "@/components/HomePage/Background1";
import Background2 from "@/components/HomePage/Background2";
import Footer from "@/components/HomePage/Footer";

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
                <div className="content-wrapper">
                  <Background1 />

                </div>
              </div>
              <div className="section">
                <div className="content-wrapper">
                  <Background2 />

                </div>
              </div>
              <div className="section">
                <div className="content-wrapper">
                  <Footer />

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