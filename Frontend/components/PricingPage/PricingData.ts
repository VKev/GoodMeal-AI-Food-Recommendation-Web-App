export interface PricingPlan {
  id: string;
  name: string;
  monthlyPrice: string;
  yearlyPrice: string;
  description: string;
  features: string[];
  popular: boolean;
  buttonText: string;
  buttonType: "default" | "primary";
  color: string;
  icon: string;
}

export const pricingPlans: PricingPlan[] = [
  {
    id: "free",
    name: "Miễn phí",
    monthlyPrice: "$0",
    yearlyPrice: "$0",
    description: "Khám phá các tính năng gợi ý món ăn AI cơ bản",
    features: [
      "3 cuộc trò chuyện AI Bot mỗi ngày",
      "Gợi ý món ăn dựa trên cảm xúc & tình huống",
      "Xem hình ảnh món ăn từ gợi ý",
      "Tìm nhà hàng trên Google Maps",
      "Lưu tối đa 5 món ăn yêu thích",
      "Hỗ trợ qua email",
      "Truy cập ứng dụng web cơ bản",
    ],
    popular: false,
    buttonText: "Gói hiện tại",
    buttonType: "default",
    color: "#52c41a",
    icon: "🆓",
  },
  {
    id: "pro",
    name: "Pro",
    monthlyPrice: "$19",
    yearlyPrice: "$182", // $19 * 12 * 0.8 = $182.4 ≈ $182
    description: "Trải nghiệm gợi ý món ăn AI không giới hạn",
    features: [
      "Trò chuyện AI Bot không giới hạn",
      "Gợi ý món ăn AI thông minh sử dụng ngôn ngữ tự nhiên",
      "Phân tích cảm xúc & gợi ý món ăn cá nhân hóa",
      "Xem hình ảnh HD của tất cả món ăn",
      "Tích hợp Google Maps với đánh giá nhà hàng",
      "Lưu món ăn yêu thích không giới hạn",
      "Lịch sử trò chuyện & gợi ý cá nhân hóa",
      "Chia sẻ món ăn với bạn bè",
      "Hỗ trợ 24/7",
      "Truy cập ứng dụng di động",
      "Xuất danh sách món ăn ra PDF",
    ],
    popular: true,
    buttonText: "Nâng cấp lên Pro",
    buttonType: "primary",
    color: "#ff7a00",
    icon: "⭐",
  },
  {
    id: "business",
    name: "Doanh nghiệp",
    monthlyPrice: "$49",
    yearlyPrice: "$470", // $49 * 12 * 0.8 = $470.4 ≈ $470
    description: "Giải pháp hoàn chỉnh cho nhà hàng & doanh nghiệp",
    features: [
      "Tất cả tính năng Pro",
      "Đăng ký quảng cáo nhà hàng",
      "Hiển thị ưu tiên trong kết quả gợi ý",
      "Hệ thống quản lý thực đơn nhà hàng",
      "Phân tích dữ liệu khách hàng & xu hướng",
      "Tích hợp API hệ thống POS",
      "Tùy chỉnh thương hiệu trong ứng dụng",
      "Báo cáo lượt xem & tương tác chi tiết",
      "Quản lý nhiều nhà hàng",
      "Hỗ trợ ưu tiên & tư vấn chuyên gia",
      "Sao lưu dữ liệu hàng ngày",
    ],
    popular: false,
    buttonText: "Liên hệ bán hàng",
    buttonType: "default",
    color: "#722ed1",
    icon: "🏢",
  },
];
