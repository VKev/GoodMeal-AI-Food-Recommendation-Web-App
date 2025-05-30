"use client"
import React, { useState } from "react";
import { EyeInvisibleOutlined, EyeOutlined, UserOutlined, ShopOutlined } from '@ant-design/icons';
import Link from "next/link";

const CreateAccount: React.FC = () => {
  const [selectedRole, setSelectedRole] = useState<"US" | "BU" | null>(null);
  const [formData, setFormData] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: ""
  });
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const handleInputChange = (field: string, value: string) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
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
        <div className="backdrop-blur-xl p-8">          {/* Welcome Section */}
          <div className="text-center mb-8">
            <h2 className="text-2xl font-bold text-white mb-2">Create Account</h2>
            <p className="text-gray-400 text-sm">Join us and start your culinary journey</p>
          </div>



          {/* Form Inputs */}
          <div className="space-y-4 mb-6">
            {/* Full Name */}
            <div className="relative">
              <input
                type="text"
                placeholder="Full name"
                value={formData.fullName}
                onChange={(e) => handleInputChange("fullName", e.target.value)}
                className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300"
              />
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
            </div>

            {/* Email */}
            <div className="relative">
              <input
                type="email"
                placeholder="Email address"
                value={formData.email}
                onChange={(e) => handleInputChange("email", e.target.value)}
                className="w-full p-4 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300"
              />
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
            </div>

            {/* Password */}
            <div className="relative">
              <input
                type={showPassword ? "text" : "password"}
                placeholder="Password"
                value={formData.password}
                onChange={(e) => handleInputChange("password", e.target.value)}
                className="w-full p-4 pr-12 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-orange-500 transition-colors duration-200"
              >
                {showPassword ? <EyeOutlined /> : <EyeInvisibleOutlined />}
              </button>
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
            </div>

            {/* Confirm Password */}
            <div className="relative">
              <input
                type={showConfirmPassword ? "text" : "password"}
                placeholder="Confirm password"
                value={formData.confirmPassword}
                onChange={(e) => handleInputChange("confirmPassword", e.target.value)}
                className="w-full p-4 pr-12 bg-white/5 border border-white/10 rounded-2xl text-white placeholder-gray-400 focus:outline-none focus:border-orange-500 focus:bg-white/10 transition-all duration-300"
              />
              <button
                type="button"
                onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-orange-500 transition-colors duration-200"
              >
                {showConfirmPassword ? <EyeOutlined /> : <EyeInvisibleOutlined />}
              </button>
              <div className="absolute inset-0 rounded-2xl bg-gradient-to-r from-orange-500/0 via-orange-500/0 to-orange-500/0 focus-within:from-orange-500/10 focus-within:to-orange-600/10 pointer-events-none transition-all duration-300"></div>
            </div>          </div>

          {/* Terms & Conditions */}
          <div className="mb-6">
            <label className="flex items-start space-x-3">
              <input
                type="checkbox"
                className="mt-1 w-4 h-4 rounded border-white/20 bg-white/5 text-orange-500 focus:ring-orange-500 focus:ring-2"
              />
              <span className="text-gray-400 text-sm leading-relaxed">
                I agree to the{" "}
                <button className="text-orange-500 hover:text-orange-400 transition-colors duration-200 underline">
                  Terms of Service
                </button>
                {" "}and{" "}
                <button className="text-orange-500 hover:text-orange-400 transition-colors duration-200 underline">
                  Privacy Policy
                </button>
              </span>
            </label>
          </div>

          {/* Create Account Button */}
          <button className="w-full p-4 bg-gradient-to-r from-orange-300 to-orange-400 text-white font-bold rounded-2xl shadow-lg shadow-orange-500/30 hover:from-orange-600 hover:to-orange-700 hover:shadow-orange-500/40 transition-all duration-300 transform hover:scale-[1.02] mb-6">
            Create Account
          </button>

          {/* Footer */}
          <div className="text-center">
            <span className="text-gray-400 text-sm">Already have an account? </span>
            <Link
              href="/sign-in"
              className="text-orange-500 font-semibold text-sm hover:text-orange-400 transition-colors duration-300">
              Sign in
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