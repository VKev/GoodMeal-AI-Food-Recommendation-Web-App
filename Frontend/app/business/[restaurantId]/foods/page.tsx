"use client";
import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import { Table, Button, Modal, Form, Input, InputNumber, Space, Typography, Tag, Popconfirm, notification } from "antd";
import { PlusOutlined, EditOutlined, DeleteOutlined, CheckCircleOutlined, StopOutlined } from "@ant-design/icons";
import { getFoods, createFood, updateFood, deleteFood, Food, getFoodById } from "@/services/RestaurantService";
import { FirebaseAuth } from "@/firebase/firebase";
import { onAuthStateChanged } from "firebase/auth";

const { Title } = Typography;

export default function FoodsPage() {
  const params = useParams();
  const restaurantId = params.restaurantId as string;
  const [foods, setFoods] = useState<Food[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingFood, setEditingFood] = useState<Food | null>(null);
  const [form] = Form.useForm();
  const [api, contextHolder] = notification.useNotification();
  const [authLoading, setAuthLoading] = useState(true);

  const fetchFoods = async () => {
    setLoading(true);
    try {
      const user = FirebaseAuth.currentUser;
      if (!user) {
        setLoading(false);
        return;
      }
      const idToken = await user.getIdToken();
      // Gọi API getFoodById với restaurantId
      const food = await getFoodById(restaurantId, idToken);
      setFoods(Array.isArray(food) ? food : [food]);
    } catch (error: any) {
      api.error({ message: "Lỗi", description: error?.message || "Không thể tải danh sách món ăn" });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(FirebaseAuth, (user) => {
      setAuthLoading(false);
      if (user && restaurantId) {
        fetchFoods();
      }
    });
    return () => unsubscribe();
  }, [restaurantId]);

  const handleSubmit = async (values: any) => {
    try {
      setLoading(true);
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : undefined;
      if (!idToken) {
        api.error({ message: "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn!" });
        setLoading(false);
        return;
      }
      if (editingFood) {
        await updateFood({ ...editingFood, ...values, restaurantId }, idToken);
        api.success({ message: "Cập nhật món ăn thành công" });
      } else {
        await createFood({ ...values, restaurantId, isAvailable: true }, idToken);
        api.success({ message: "Tạo món ăn mới thành công" });
      }
      setShowModal(false);
      setEditingFood(null);
      form.resetFields();
      fetchFoods();
    } catch (error: any) {
      api.error({ message: "Lỗi", description: error?.message || "Không thể lưu thông tin món ăn" });
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    try {
      setLoading(true);
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : undefined;
      if (!idToken) {
        api.error({ message: "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn!" });
        setLoading(false);
        return;
      }
      await deleteFood(id, idToken);
      api.success({ message: "Xóa món ăn thành công" });
      fetchFoods();
    } catch (error: any) {
      api.error({ message: "Lỗi", description: error?.message || "Không thể xóa món ăn" });
    } finally {
      setLoading(false);
    }
  };

  const columns = [
    {
      title: "Tên món",
      dataIndex: "name",
      key: "name",
    },
    {
      title: "Mô tả",
      dataIndex: "description",
      key: "description",
    },
    {
      title: "Giá",
      dataIndex: "price",
      key: "price",
      render: (value: number) => value?.toLocaleString("vi-VN") + " đ",
    },
    {
      title: "Trạng thái",
      dataIndex: "isAvailable",
      key: "isAvailable",
      render: (val: boolean) => val ? <Tag icon={<CheckCircleOutlined />} color="success">Có sẵn</Tag> : <Tag icon={<StopOutlined />} color="error">Hết món</Tag>,
    },
    {
      title: "Thao tác",
      key: "actions",
      render: (record: Food) => (
        <Space>
          <Button icon={<EditOutlined />} onClick={() => { setEditingFood(record); form.setFieldsValue(record); setShowModal(true); }}>Sửa</Button>
          <Popconfirm title="Xóa món ăn" description="Bạn có chắc chắn muốn xóa món ăn này?" onConfirm={() => handleDelete(record.id)} okText="Có" cancelText="Không">
            <Button danger icon={<DeleteOutlined />}>Xóa</Button>
          </Popconfirm>
        </Space>
      ),
    },
  ];

  if (authLoading) return <div>Đang xác thực đăng nhập...</div>;

  return (
    <div style={{ background: "#fff", padding: 24, minHeight: 600 }}>
      {contextHolder}
      <Title level={2}>Quản lý món ăn</Title>
      <Button type="primary" icon={<PlusOutlined />} style={{ marginBottom: 16 }} onClick={() => { setEditingFood(null); form.resetFields(); setShowModal(true); }}>Thêm món ăn mới</Button>
      <Table columns={columns} dataSource={foods} loading={loading} rowKey="id" pagination={{ pageSize: 10 }} />
      <Modal
        title={editingFood ? "Chỉnh sửa món ăn" : "Thêm món ăn mới"}
        open={showModal}
        onCancel={() => { setShowModal(false); setEditingFood(null); form.resetFields(); }}
        footer={null}
        width={600}
      >
        <Form form={form} layout="vertical" onFinish={handleSubmit} initialValues={{ isAvailable: true }}>
          <Form.Item label="Tên món ăn" name="name" rules={[{ required: true, message: "Vui lòng nhập tên món ăn!" }]}>
            <Input placeholder="Nhập tên món ăn" />
          </Form.Item>
          <Form.Item label="Mô tả" name="description" rules={[{ required: true, message: "Vui lòng nhập mô tả!" }]}>
            <Input placeholder="Mô tả về món ăn" />
          </Form.Item>
          <Form.Item label="Giá (VND)" name="price" rules={[{ required: true, message: "Vui lòng nhập giá!" }]}>
            <InputNumber placeholder="50000" style={{ width: '100%' }} min={0} step={1000} />
          </Form.Item>
          <Form.Item label="Link ảnh món ăn (imageUrl)" name="imageUrl" rules={[{ required: true, message: "Vui lòng nhập link ảnh!" }]}>
            <Input placeholder="https://..." />
          </Form.Item>
          <Form.Item label="Trạng thái" name="isAvailable">
            <Input type="checkbox" /> Có sẵn
          </Form.Item>
          <Button type="primary" htmlType="submit" loading={loading} style={{ marginTop: 16 }}>{editingFood ? "Cập nhật" : "Tạo mới"}</Button>
        </Form>
      </Modal>
    </div>
  );
} 