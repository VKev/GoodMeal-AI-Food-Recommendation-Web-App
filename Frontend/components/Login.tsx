// import React, { useState } from "react";
// import { Form, Input, Button, Tabs, Select, Checkbox, message } from "antd";
// import { GoogleOutlined } from "@ant-design/icons";

// const { TabPane } = Tabs;
// const { Option } = Select;

// const Login: React.FC = () => {
//   const [isRegister, setIsRegister] = useState(false);

//   const onFinishLogin = (values: any) => {
//     console.log("Login Success:", values);
//     message.success("Login successful!");
//   };

//   const onFinishRegister = (values: any) => {
//     console.log("Register Success:", values);
//     message.success("Registration successful!");
//   };

//   const handleGoogleLogin = () => {
//     console.log("Google Login");
//     message.info("Redirecting to Google Login...");
//   };

//   return (
//     <div
//       style={{
//         width: "100%",
//         height: "100%", // Chiếm toàn bộ chiều cao của phần nền trắng
//         padding: "40px 20px", // Tăng padding để giống Instagram
//         backgroundColor: "#fff",
//         display: "flex",
//         flexDirection: "column",
//         justifyContent: "center",
//         alignItems: "center", // Căn giữa nội dung
//       }}
//     >
//       <div
//         style={{
//           width: "100%",
//           maxWidth: 350,
//           minHeight: 400, // Cố định chiều cao tối thiểu cho nội dung
//           display: "flex",
//           flexDirection: "column",
//           justifyContent: "space-between",
//         }}
//       >
//         <Tabs
//           defaultActiveKey="1"
//           onChange={(key) => setIsRegister(key === "2")}
//           centered
//           style={{ marginBottom: 20 }}
//         >
//           <TabPane tab="Login" key="1">
//             <div style={{ minHeight: 300 }}> {/* Cố định chiều cao nội dung tab */}
//               <Form name="login" onFinish={onFinishLogin} layout="vertical">
//                 <Form.Item
//                   name="email"
//                   label="Email"
//                   rules={[
//                     { required: true, message: "Please input your email!" },
//                     { type: "email", message: "Invalid email!" },
//                   ]}
//                 >
//                   <Input placeholder="Enter your email" />
//                 </Form.Item>
//                 <Form.Item
//                   name="password"
//                   label="Password"
//                   rules={[{ required: true, message: "Please input your password!" }]}
//                 >
//                   <Input.Password placeholder="Enter your password" />
//                 </Form.Item>
//                 <Form.Item>
//                   <Button type="primary" htmlType="submit" block>
//                     Login
//                   </Button>
//                 </Form.Item>
//                 <Form.Item>
//                   <Button icon={<GoogleOutlined />} block onClick={handleGoogleLogin}>
//                     Login with Google
//                   </Button>
//                 </Form.Item>
//               </Form>
//             </div>
//           </TabPane>
//           <TabPane tab="Register" key="2">
//             <div style={{ minHeight: 300 }}> {/* Cố định chiều cao nội dung tab */}
//               <Form name="register" onFinish={onFinishRegister} layout="vertical">
//                 <Form.Item
//                   name="email"
//                   label="Email"
//                   rules={[
//                     { required: true, message: "Please input your email!" },
//                     { type: "email", message: "Invalid email!" },
//                   ]}
//                 >
//                   <Input placeholder="Enter your email" />
//                 </Form.Item>
//                 <Form.Item
//                   name="password"
//                   label="Password"
//                   rules={[{ required: true, message: "Please input your password!" }]}
//                 >
//                   <Input.Password placeholder="Enter your password" />
//                 </Form.Item>
//                 <Form.Item
//                   name="role"
//                   label="Role"
//                   rules={[{ required: true, message: "Please select your role!" }]}
//                 >
//                   <Select placeholder="Select your role">
//                     <Option value="user">User</Option>
//                     <Option value="business">Business</Option>
//                   </Select>
//                 </Form.Item>
//                 <Form.Item
//                   name="terms"
//                   valuePropName="checked"
//                   rules={[
//                     {
//                       validator: (_, value) =>
//                         value
//                           ? Promise.resolve()
//                           : Promise.reject("You must accept the terms and conditions!"),
//                     },
//                   ]}
//                 >
//                   <Checkbox>
//                     I accept the <a href="#">terms and conditions</a>
//                   </Checkbox>
//                 </Form.Item>
//                 <Form.Item>
//                   <Button type="primary" htmlType="submit" block>
//                     Register
//                   </Button>
//                 </Form.Item>
//               </Form>
//             </div>
//           </TabPane>
//         </Tabs>
//       </div>
//     </div>
//   );
// };

// export default Login;