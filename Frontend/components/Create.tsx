import React from "react";

const Create: React.FC = () => {
  return (
    <div style={{ minHeight: "100vh", background: "#fff" }}>
      <div style={{ padding: "24px 0 0 32px", fontWeight: 600, fontSize: 28, fontFamily: "sans-serif" }}>
        GoodMeal
      </div>
      <div style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        marginTop: 80
      }}>
        <h1 style={{ fontSize: 36, fontWeight: 500, marginBottom: 32 }}>Create an account</h1>
        <input
          type="email"
          placeholder="Email address"
          style={{
            width: 400,
            padding: "16px 12px",
            fontSize: 18,
            borderRadius: 8,
            border: "1.5px solid #16a085",
            marginBottom: 20,
            outline: "none"
          }}
        />
        <button
          style={{
            width: 400,
            padding: "14px 0",
            background: "#16a085",
            color: "#fff",
            fontSize: 20,
            border: "none",
            borderRadius: 8,
            fontWeight: 500,
            marginBottom: 18,
            cursor: "pointer"
          }}
        >
          Continue
        </button>
        <div style={{ marginBottom: 18, color: "#222", fontSize: 16 }}>
          Already have an account?{" "}
          <a href="/sign-in" style={{ color: "#16a085", textDecoration: "none", fontWeight: 500 }}>
            Log in
          </a>
        </div>
        <div style={{
          display: "flex",
          alignItems: "center",
          width: 400,
          margin: "16px 0"
        }}>
          <div style={{ flex: 1, height: 1, background: "#e0e0e0" }} />
          <span style={{ margin: "0 16px", color: "#888" }}>OR</span>
          <div style={{ flex: 1, height: 1, background: "#e0e0e0" }} />
        </div>
        <button style={{
          width: 400,
          display: "flex",
          alignItems: "center",
          padding: "12px 0",
          border: "1px solid #e0e0e0",
          borderRadius: 8,
          background: "#fff",
          fontSize: 18,
          marginBottom: 12,
          cursor: "pointer"
        }}>
          <img src="https://www.svgrepo.com/show/475656/google-color.svg" alt="Google" style={{ width: 24, marginRight: 16 }} />
          Continue with Google
        </button>
        
        <div style={{ display: "flex", justifyContent: "center", width: 400, marginTop: 32, color: "#16a085", fontSize: 15 }}>
          <a href="#" style={{ color: "#16a085", textDecoration: "none", marginRight: 12 }}>
            Terms of Use
          </a>
          <span style={{ color: "#bdbdbd" }}>|</span>
          <a href="#" style={{ color: "#16a085", textDecoration: "none", marginLeft: 12 }}>
            Privacy Policy
          </a>
        </div>
      </div>
    </div>
  );
};

export default Create;
