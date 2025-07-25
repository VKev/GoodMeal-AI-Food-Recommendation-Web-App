"use client";
import { useEffect, useState } from "react";
import { useCallback } from "react";
import { useParams } from "next/navigation";
import { Card, Button, Modal, Form, Input, InputNumber, Space, Typography, Tag, Popconfirm, notification, Row, Col } from "antd";
import { PlusOutlined, EditOutlined, DeleteOutlined, CheckCircleOutlined, StopOutlined } from "@ant-design/icons";
import { getFoods, createFood, updateFood, deleteFood, Food, getFoodsByRestaurantId } from "@/services/RestaurantService";
import { FirebaseAuth } from "@/firebase/firebase";
import { onAuthStateChanged } from "firebase/auth";
import Image from "next/image";

const { Title } = Typography;

export default function FoodsPage() {
  const params = useParams();
  const id = params.id as string; // lấy id từ url
  const [foods, setFoods] = useState<Food[]>([]);
  const [loading, setLoading] = useState(false);
  const [showModal, setShowModal] = useState(false);
  const [editingFood, setEditingFood] = useState<Food | null>(null);
  const [form] = Form.useForm();
  const [api, contextHolder] = notification.useNotification();

  const fetchFoods = useCallback(async () => {
    setLoading(true);
    try {
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : undefined;
      if (!idToken) {
        api.error({ message: "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn!" });
        setLoading(false);
        return;
      }
      // Gọi API lấy danh sách món ăn theo nhà hàng
      const foods = await getFoodsByRestaurantId(id, idToken);
      setFoods(foods);
    } catch (error: any) {
      api.error({ message: "Lỗi", description: error?.message || "Không thể tải danh sách món ăn" });
    } finally {
      setLoading(false);
    }
  }, [id, api]);

  useEffect(() => {
    const unsubscribe = onAuthStateChanged(FirebaseAuth, (user) => {
      if (user && id) {
        fetchFoods();
      }
    });
    return () => unsubscribe();
  }, [id, fetchFoods]);

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
        await updateFood({ ...editingFood, ...values, restaurantId: id }, idToken);
        api.success({ message: "Cập nhật món ăn thành công" });
      } else {
        await createFood({ ...values, restaurantId: id, isAvailable: true }, idToken);
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

  const handleDelete = async (foodId: string) => {
    try {
      setLoading(true);
      const user = FirebaseAuth.currentUser;
      const idToken = user ? await user.getIdToken() : undefined;
      if (!idToken) {
        api.error({ message: "Bạn chưa đăng nhập hoặc phiên đăng nhập đã hết hạn!" });
        setLoading(false);
        return;
      }
      await deleteFood(foodId, idToken);
      api.success({ message: "Xóa món ăn thành công" });
      fetchFoods();
    } catch (error: any) {
      api.error({ message: "Lỗi", description: error?.message || "Không thể xóa món ăn" });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ background: "#fff", padding: 24, minHeight: 600 }}>
      {contextHolder}
      <Title level={2}>Quản lý món ăn</Title>
      <Button type="primary" icon={<PlusOutlined />} style={{ marginBottom: 16 }} onClick={() => { setEditingFood(null); form.resetFields(); setShowModal(true); }}>Thêm món ăn mới</Button>
      <Row gutter={[16, 16]}>
        {foods.length === 0 && !loading ? (
          <Col span={24} style={{ textAlign: 'center', color: '#888', fontSize: 18, padding: 40 }}>
            Chưa có món ăn nào
          </Col>
        ) : (
          foods.map(food => (
            <Col xs={24} sm={12} md={8} lg={6} key={food.id}>
              <Card
                hoverable
                cover={
                  <Image
                    alt={food.name}
                    src={food.imageUrl || 'https://via.placeholder.com/300x200?text=No+Image'}
                    width={300}
                    height={200}
                    style={{ height: 200, objectFit: 'cover', width: '100%' }}
                  />
                }
                actions={[
                  <Button icon={<EditOutlined />} onClick={() => { setEditingFood(food); form.setFieldsValue(food); setShowModal(true); }} key="edit">Sửa</Button>,
                  <Popconfirm title="Xóa món ăn" description="Bạn có chắc chắn muốn xóa món ăn này?" onConfirm={() => handleDelete(food.id)} okText="Có" cancelText="Không" key="delete">
                    <Button danger icon={<DeleteOutlined />}>Xóa</Button>
                  </Popconfirm>
                ]}
              >
                <Card.Meta
                  title={<span style={{ fontWeight: 600 }}>{food.name}</span>}
                  description={
                    <>
                      <div style={{ marginBottom: 8 }}>{food.description}</div>
                      <div style={{ fontWeight: 600, color: '#fa541c', marginBottom: 8 }}>{food.price?.toLocaleString('vi-VN')} đ</div>
                      <Tag icon={food.isAvailable ? <CheckCircleOutlined /> : <StopOutlined />} color={food.isAvailable ? 'success' : 'error'}>
                        {food.isAvailable ? 'Có sẵn' : 'Hết món'}
                      </Tag>
                    </>
                  }
                />
              </Card>
            </Col>
          ))
        )}
      </Row>
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