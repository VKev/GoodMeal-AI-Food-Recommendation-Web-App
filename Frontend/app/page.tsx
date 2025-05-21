"use client";
import Background from "@/components/Background";
import Content from "@/components/Content";
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
                  <Content />

                </div>
              </div>
            </ReactFullpage.Wrapper>
          );
        }} credits={{
          enabled: undefined,
          label: undefined,
          position: undefined
        }} />
    </>
  );
}