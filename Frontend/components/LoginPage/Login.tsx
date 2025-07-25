"use client"
import React, { useState } from "react";
import Link from "next/link";
import { signInWithGoogle, signInWithEmail } from "../../firebase/firebase";
import { useAuth } from "@/hooks/auths/authContext";

const Login: React.FC = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const { authenticated, loading: authLoading } = useAuth();

  // Remove the useEffect that calls navigateByRole since it's now handled in AuthContext
  React.useEffect(() => {
    // If user is already authenticated when component mounts, redirect
    if (authenticated && !authLoading) {
      console.log('User already authenticated, redirecting...');
      // The navigation will be handled by AuthContext
    }
  }, [authenticated, authLoading]);
  const handleEmailLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email || !password) {
      setError("Vui lòng điền đầy đủ thông tin");
      return;
    }

    setLoading(true);
    setError("");

    try {
      await signInWithEmail(email, password);
      console.log('Email login successful');
      // Navigation will be handled by AuthContext useEffect
    } catch (error: any) {
      setError(error.message || "Đăng nhập thất bại");
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleLogin = async () => {
    setLoading(true);
    setError("");

    try {
      await signInWithGoogle();
      console.log('Google login successful');
      // Navigation will be handled by AuthContext useEffect
    } catch (error: any) {
      setError(error.message || "Đăng nhập bằng Google thất bại");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 via-black to-gray-800 flex items-center justify-center p-4 relative overflow-hidden">
      {/* Animated Background Elements */}
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute -top-40 -right-40 w-80 h-80 bg-orange-500/10 rounded-full blur-3xl animate-pulse"></div>
        <div className="absolute -bottom-40 -left-40 w-80 h-80 bg-orange-600/10 rounded-full blur-3xl animate-pulse delay-1000"></div>
      </div>

      <div className="relative w-full max-w-md">
        {/* Glassmorphism Card */}
        <div className="backdrop-blur-xl p-8">
          {/* Welcome Section */}          <div className="text-center mb-8">
            <h2 className="text-2xl font-bold text-white mb-2">Chào mừng trở lại</h2>
            <p className="text-gray-400 text-sm">Tiếp tục hành trình ẩm thực của bạn</p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg text-red-400 text-sm">
              {error}
            </div>
          )}

          {/* Form */}
          <form onSubmit={handleEmailLogin}>
            {/* Form Inputs */}
            <div className="space-y-4 mb-6">
              <div className="relative">                <input
                  type="email"
                  placeholder="Địa chỉ email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  disabled={loading}
                  className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>

              <div className="relative">                <input
                  type="password"
                  placeholder="Mật khẩu"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  disabled={loading}
                  className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>
            </div>

            {/* Sign In Button */}
            <button
              type="submit"
              disabled={loading}
              className="w-full p-4 bg-gradient-to-r from-orange-300 to-orange-400 text-white font-bold rounded-2xl shadow-lg shadow-orange-500/30 hover:from-orange-600 hover:to-orange-700 hover:shadow-orange-500/40 transition-all duration-300 transform hover:scale-[1.02] mb-6 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none"
            >
              {loading ? "Đang đăng nhập..." : "Đăng nhập"}
            </button>
          </form>

          {/* Divider */}
          <div className="flex items-center mb-6">
            <div className="flex-1 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent"></div>
            <span className="px-4 text-gray-400 text-sm font-medium">HOẶC</span>
            <div className="flex-1 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent"></div>
          </div>

          {/* Google Sign In */}
          <button
            type="button"
            onClick={handleGoogleLogin}
            disabled={loading}
            className="w-full p-4 rounded-2xl font-semibold mb-6 transition-all duration-300 transform hover:scale-[1.02] bg-gradient-to-r from-orange-300 to-orange-400 text-white shadow-lg shadow-orange-500/30 hover:from-orange-600 hover:to-orange-700 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none"
          >
            <div className="flex items-center justify-center">
              <svg className="w-5 h-5 mr-3" viewBox="0 0 24 24">
                <path fill="currentColor" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" />
                <path fill="currentColor" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" />
                <path fill="currentColor" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" />
                <path fill="currentColor" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" />
              </svg>
              {loading ? "Đang đăng nhập..." : "Tiếp tục với Google"}
            </div>
          </button>

          {/* Footer */}          <div className="text-center">
            <span className="text-gray-400 text-sm">Chưa có tài khoản? </span>
            <Link
              href="/create-account"
              className="text-orange-500 font-semibold text-sm hover:text-orange-400 transition-colors duration-300"
            >
              Đăng ký
            </Link>
          </div>
        </div>

        {/* Floating Elements */}
        <div className="absolute -top-2 -right-2 w-4 h-4 bg-orange-500 rounded-full opacity-60 animate-bounce"></div>
        <div className="absolute -bottom-2 -left-2 w-3 h-3 bg-orange-400 rounded-full opacity-40 animate-bounce delay-500"></div>
      </div>
    </div>
  );
};

export default Login;