# Authentication Flow Guide

## Tổng quan Flow Authentication

### 1. Frontend Login
```javascript
// Frontend đăng nhập với Firebase
const user = await signInWithEmailAndPassword(auth, email, password);
const idToken = await user.getIdToken(); // Lấy IdToken từ Firebase
```

### 2. Frontend gửi IdToken lên Backend
```javascript
// Gửi request lên Authentication.Microservice
const response = await fetch('/api/auth/login-token', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    identityToken: idToken // IdToken từ Firebase
  })
});

const loginData = await response.json();
```

### 3. Backend Response (Authentication.Microservice)
```json
{
  "isSuccess": true,
  "value": {
    "userId": "firebase_uid_12345",
    "email": "user@example.com", 
    "name": "John Doe",
    "idToken": "original_firebase_token", // Token gốc từ Firebase
    "roles": ["User", "Admin"],
    "primaryRole": "User",
    "expiresIn": 3600
  }
}
```

### 4. Frontend lưu thông tin và sử dụng
```javascript
// Lưu thông tin user
localStorage.setItem('user', JSON.stringify(loginData.value));
localStorage.setItem('idToken', loginData.value.idToken);

// Khi gọi API, sử dụng IdToken gốc
const apiResponse = await fetch('/api/some-endpoint', {
  headers: {
    'Authorization': `Bearer ${loginData.value.idToken}` // Sử dụng IdToken gốc
  }
});
```

### 5. API Gateway xử lý requests
```
1. API Gateway nhận request với Authorization header
2. Extract IdToken từ header
3. Verify IdToken với Firebase
4. Gọi User.Microservice để lấy roles theo userId
5. Forward request với headers:
   - X-Is-Authenticated: true
   - X-User-Id: firebase_uid
   - X-User-Email: user@example.com
   - X-User-Role: User (primary role)
   - X-User-Roles: User,Admin (all roles)
```

### 6. Microservices nhận thông tin đã xác thực
```csharp
[ApiGatewayUser] // Chỉ cho phép user đã xác thực
public IActionResult SomeAction()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var email = User.FindFirst(ClaimTypes.Email)?.Value;
    var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    
    return Ok(new { userId, email, roles });
}

[ApiGatewayUser(Roles = "Admin")] // Chỉ cho phép Admin
public IActionResult AdminOnlyAction()
{
    return Ok("Admin access granted");
}
```

## Ưu điểm của Flow này

1. **Đơn giản**: Sử dụng IdToken gốc từ Firebase, không cần tạo custom token
2. **Bảo mật**: Firebase tự verify token, không cần implement JWT validation
3. **Linh hoạt**: Roles được lấy từ database, có thể thay đổi động
4. **Hiệu quả**: API Gateway cache roles hoặc có thể optimize bằng Redis
5. **Tương thích**: Hoạt động với mọi Firebase auth provider

## Cấu hình cần thiết

### Environment Variables
```bash
# API Gateway
USER_SERVICE_URL=http://localhost:5004

# User.Microservice  
DATABASE_HOST=localhost
DATABASE_PORT=5432
# ... other database configs
```

### Firebase Credentials
- Đặt file `credentials.json` trong thư mục gốc của ApiGateway
- File này chứa Firebase service account key

## Test Flow

### 1. Test login endpoint
```bash
POST /api/auth/login-token
{
  "identityToken": "firebase_id_token_here"
}
```

### 2. Test protected endpoint
```bash
GET /api/auth/check-authorization
Authorization: Bearer firebase_id_token_here
```

### 3. Test role-based endpoint
```bash
GET /api/auth/check-admin
Authorization: Bearer firebase_id_token_here
```

## Troubleshooting

### Lỗi "Firebase credentials not found"
- Đảm bảo file `credentials.json` có trong thư mục gốc
- Kiểm tra permissions của file

### Lỗi "Failed to get user roles"
- Kiểm tra USER_SERVICE_URL environment variable
- Đảm bảo User.Microservice đang chạy
- Kiểm tra user có tồn tại trong database với đúng IdentityId

### Token verification failed
- Kiểm tra IdToken có đúng format không
- Đảm bảo token chưa hết hạn
- Kiểm tra Firebase project configuration 