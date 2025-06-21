"use client"
import React, { useState } from "react";
import { EyeInvisibleOutlined, EyeOutlined } from '@ant-design/icons';
import Link from "next/link";
import { useRouter } from "next/navigation";
import { signUpWithEmail, signInWithGoogle } from "../../firebase/firebase";
import { useAuth } from "@/hooks/auths/authContext";
import { updateProfile } from "firebase/auth";
import { registerUser } from "../../services/Create";

const CreateAccount: React.FC = () => {
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    password: "",
    confirmPassword: ""
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [acceptTerms, setAcceptTerms] = useState(false);

  const { authenticated } = useAuth();
  const router = useRouter();
  // Redirect if already authenticated
  React.useEffect(() => {
    if (authenticated) {
      router.push("/");
    }
  }, [authenticated, router]);

  // Log environment variable on component mount
  React.useEffect(() => {
    console.log('Environment check - NEXT_PUBLIC_BACKEND_BASE_URL:', process.env.NEXT_PUBLIC_BACKEND_BASE_URL);
  }, []);

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };  const validateForm = () => {
    if (!formData.name.trim()) {
      setError("Vui lòng nhập họ tên đầy đủ");
      return false;
    }
    if (!formData.email.trim()) {
      setError("Vui lòng nhập email");
      return false;
    }
    if (!formData.password) {
      setError("Vui lòng nhập mật khẩu");
      return false;
    }
    if (formData.password.length < 6) {
      setError("Mật khẩu phải có ít nhất 6 ký tự");
      return false;
    }
    if (formData.password !== formData.confirmPassword) {
      setError("Mật khẩu không khớp");
      return false;
    }
    if (!acceptTerms) {
      setError("Vui lòng chấp nhận điều khoản và điều kiện");
      return false;
    }
    return true;
  };  const handleCreateAccount = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) return;

    setLoading(true);
    setError("");    try {
      // Only call API registration
      const registrationData = {
        email: formData.email,
        password: formData.password,
        name: formData.name
      };

      console.log('Attempting to register with API:', registrationData);
      const apiResponse = await registerUser(registrationData);
      console.log('API registration successful:', apiResponse);

      // Redirect to login page or home after successful registration
      router.push("/sign-in"); // Or "/" if you want auto-login
    } catch (error: any) {
      console.error('Registration error:', error);
      setError(error.message || "Tạo tài khoản thất bại");
    } finally {
      setLoading(false);
    }
  };

  const handleGoogleSignUp = async () => {
    setLoading(true);
    setError("");

    try {
      await signInWithGoogle();      router.push("/");
    } catch (error: any) {
      setError(error.message || "Đăng ký bằng Google thất bại");
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
        <div className="absolute top-1/2 left-1/4 w-60 h-60 bg-orange-400/5 rounded-full blur-3xl animate-pulse delay-500"></div>
      </div>

      <div className="relative w-full max-w-md">
        {/* Glassmorphism Card */}
        <div className="backdrop-blur-xl p-8">
          {/* Welcome Section */}          <div className="text-center mb-8">
            <h2 className="text-2xl font-bold text-white mb-2">Tạo tài khoản</h2>
            <p className="text-gray-400 text-sm">Tham gia với chúng tôi và bắt đầu hành trình ẩm thực</p>
          </div>

          {/* Error Message */}
          {error && (
            <div className="mb-4 p-3 bg-red-500/20 border border-red-500/50 rounded-lg text-red-400 text-sm">
              {error}
            </div>
          )}

          <form onSubmit={handleCreateAccount}>
      

            {/* Form Inputs */}
            <div className="space-y-4 mb-6">
              {/* Full Name */}
              <div className="relative">                <input
                  type="text"
                  placeholder="Họ và tên"                  value={formData.name}
                  onChange={(e) => handleInputChange("name", e.target.value)}
                  disabled={loading}
                  className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>

              {/* Email */}
              <div className="relative">                <input
                  type="email"
                  placeholder="Địa chỉ email"
                  value={formData.email}
                  onChange={(e) => handleInputChange("email", e.target.value)}
                  disabled={loading}
                  className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>

              {/* Password */}
              <div className="relative">                <input
                  type={showPassword ? "text" : "password"}
                  placeholder="Mật khẩu"
                  value={formData.password}
                  onChange={(e) => handleInputChange("password", e.target.value)}
                  disabled={loading}
                  className="w-full p-4 pr-12 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  disabled={loading}
                  className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-orange-500 transition-colors duration-200 disabled:opacity-50"
                >
                  {showPassword ? <EyeOutlined /> : <EyeInvisibleOutlined />}
                </button>
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>

              {/* Confirm Password */}
              <div className="relative">                <input
                  type={showConfirmPassword ? "text" : "password"}
                  placeholder="Xác nhận mật khẩu"
                  value={formData.confirmPassword}
                  onChange={(e) => handleInputChange("confirmPassword", e.target.value)}
                  disabled={loading}
                  className="w-full p-4 pr-12 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300 disabled:opacity-50"
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  disabled={loading}
                  className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-orange-500 transition-colors duration-200 disabled:opacity-50"
                >
                  {showConfirmPassword ? <EyeOutlined /> : <EyeInvisibleOutlined />}
                </button>
                <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
              </div>
            </div>

            {/* Terms & Conditions */}
            <div className="mb-6">
              <label className="flex items-start space-x-3">
                <input
                  type="checkbox"
                  checked={acceptTerms}
                  onChange={(e) => setAcceptTerms(e.target.checked)}
                  disabled={loading}
                  className="mt-1 w-4 h-4 rounded border-white/20 bg-white/5 text-orange-500 focus:ring-orange-500 focus:ring-2 disabled:opacity-50"
                />                <span className="text-gray-400 text-sm leading-relaxed">
                  Tôi đồng ý với{" "}
                  <button type="button" className="text-orange-500 hover:text-orange-400 transition-colors duration-200 underline">
                    Điều khoản dịch vụ
                  </button>
                  {" "}và{" "}
                  <button type="button" className="text-orange-500 hover:text-orange-400 transition-colors duration-200 underline">
                    Chính sách bảo mật
                  </button>
                </span>
              </label>
            </div>

            {/* Create Account Button */}
            <button 
              type="submit"
              disabled={loading}
              className="w-full p-4 bg-gradient-to-r from-orange-300 to-orange-400 text-white font-bold rounded-2xl shadow-lg shadow-orange-500/30 hover:from-orange-600 hover:to-orange-700 hover:shadow-orange-500/40 transition-all duration-300 transform hover:scale-[1.02] mb-6 disabled:opacity-50 disabled:cursor-not-allowed disabled:transform-none"
            >
              {loading ? "Đang tạo tài khoản..." : "Tạo tài khoản"}
            </button>
          </form>

          {/* Divider */}
          <div className="flex items-center mb-6">
            <div className="flex-1 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent"></div>
            <span className="px-4 text-gray-400 text-sm font-medium">HOẶC</span>
            <div className="flex-1 h-px bg-gradient-to-r from-transparent via-white/20 to-transparent"></div>
          </div>

          {/* Google Sign Up */}
          <button 
            type="button"
            onClick={handleGoogleSignUp}
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
              {loading ? "Đang đăng ký..." : "Tiếp tục với Google"}
            </div>
          </button>

          {/* Footer */}          <div className="text-center">
            <span className="text-gray-400 text-sm">Đã có tài khoản? </span>
            <Link
              href="/sign-in"
              className="text-orange-500 font-semibold text-sm hover:text-orange-400 transition-colors duration-300">
              Đăng nhập
            </Link>
          </div>
        </div>

        {/* Floating Elements */}
        <div className="absolute -top-2 -right-2 w-4 h-4 bg-orange-500 rounded-full opacity-60 animate-bounce"></div>
        <div className="absolute -bottom-2 -left-2 w-3 h-3 bg-orange-400 rounded-full opacity-40 animate-bounce delay-500"></div>
        <div className="absolute top-1/3 -right-4 w-2 h-2 bg-orange-300 rounded-full opacity-50 animate-bounce delay-1000"></div>
      </div>
    </div>
  );
};

export default CreateAccount;