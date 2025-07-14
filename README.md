# ProjectPrn222 - E-commerce Website

## Mô tả dự án

ProjectPrn222 là một website thương mại điện tử bán hoa quả tươi được phát triển bằng ASP.NET Core 8.0 MVC. Hệ thống cho phép khách hàng mua sắm trực tuyến, quản lý giỏ hàng, thanh toán và theo dõi đơn hàng. Đồng thời cung cấp các tính năng quản lý cho admin và nhân viên.

## Tính năng chính

### 🛒 Khách hàng (Customer)
- **Duyệt sản phẩm**: Xem danh sách sản phẩm với phân trang và lọc theo danh mục
- **Tìm kiếm**: Tìm kiếm sản phẩm theo tên
- **Chi tiết sản phẩm**: Xem thông tin chi tiết, hình ảnh, giá cả
- **Giỏ hàng**: Thêm/xóa/cập nhật số lượng sản phẩm
- **Mã giảm giá**: Áp dụng voucher để được giảm giá
- **Thanh toán**: Thanh toán qua VNPay
- **Lịch sử đơn hàng**: Theo dõi trạng thái và chi tiết các đơn hàng đã mua

### 👨‍💼 Nhân viên (Staff)
- **Quản lý danh mục**: Thêm/sửa/xóa danh mục sản phẩm
- **Quản lý sản phẩm**: CRUD sản phẩm với upload hình ảnh
- **Quản lý voucher**: Tạo và quản lý mã giảm giá
- **Quản lý giá**: Lịch sử thay đổi giá sản phẩm

### 👑 Admin
- **Quản lý người dùng**: CRUD tài khoản người dùng
- **Phân quyền**: Gán role cho người dùng (Admin, Staff, Customer)
- **Báo cáo**: Xem thống kê doanh thu theo tháng

### 🔐 Xác thực & Phân quyền
- **Đăng ký/Đăng nhập**: Hệ thống authentication với ASP.NET Identity
- **Xác thực email**: Gửi email xác nhận khi đăng ký
- **Quên mật khẩu**: Đặt lại mật khẩu qua email
- **Phân quyền**: Role-based authorization (Admin, Staff, Customer)

## Công nghệ sử dụng

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0.13
- **Database**: SQL Server
- **Authentication**: ASP.NET Core Identity
- **Real-time**: SignalR

### Frontend
- **Template Engine**: Razor Pages
- **CSS Framework**: Bootstrap, Custom SCSS
- **JavaScript**: jQuery, AJAX
- **Icons**: Bootstrap Icons, Font Awesome

### Dịch vụ bên thứ 3
- **Cloudinary**: Lưu trữ và quản lý hình ảnh
- **VNPay**: Cổng thanh toán trực tuyến
- **SMTP Gmail**: Gửi email xác thực

### Kiến trúc
- **Pattern**: Repository Pattern, Dependency Injection
- **Layered Architecture**: Controllers, Services, Models
- **Clean Code**: Separation of Concerns

## Cấu trúc dự án

```
ProjectPrn222/
├── Controllers/          # MVC Controllers
│   ├── AdminController.cs       # Quản lý admin
│   ├── AuthController.cs        # Xác thực
│   ├── CustomerController.cs    # Chức năng khách hàng
│   ├── HomeController.cs        # Trang chủ
│   ├── PaymentController.cs     # Thanh toán
│   └── StaffController.cs       # Chức năng nhân viên
├── Models/               # Data Models
│   ├── ApplicationUser.cs       # User model
│   ├── Product.cs              # Sản phẩm
│   ├── Category.cs             # Danh mục
│   ├── Cart.cs                 # Giỏ hàng
│   ├── Order.cs                # Đơn hàng
│   ├── Vourcher.cs             # Voucher
│   └── DTO/                    # Data Transfer Objects
├── Views/                # Razor Views
│   ├── Admin/                  # Views cho Admin
│   ├── Auth/                   # Views xác thực
│   ├── Customer/               # Views khách hàng
│   ├── Home/                   # Views trang chủ
│   ├── Staff/                  # Views nhân viên
│   └── Shared/                 # Views chung
├── Service/              # Business Logic
│   ├── Interface/              # Service Interfaces
│   └── Implement/              # Service Implementations
├── Helpers/              # Helper Classes
├── Hubs/                 # SignalR Hubs
├── lib/                  # Third-party Libraries
│   ├── signalr/               # SignalR client
│   └── vnpay/                 # VNPay library
├── Migrations/           # Database Migrations
└── wwwroot/             # Static Files
    ├── css/                   # Stylesheets
    ├── js/                    # JavaScript files
    └── lib/                   # Client-side libraries
```

## Cài đặt và chạy dự án

### Yêu cầu hệ thống
- .NET 8.0 SDK
- SQL Server (LocalDB hoặc SQL Server Express)
- Visual Studio 2022 hoặc VS Code
- Node.js (nếu muốn build SCSS)

### Bước 1: Clone repository
```bash
git clone <repository-url>
cd ProjectPrn222
```

### Bước 2: Cấu hình cơ sở dữ liệu
1. Mở file `appsettings.json`
2. Cập nhật connection string phù hợp với SQL Server của bạn:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.; Database=PRN222; Trusted_Connection=True; TrustServerCertificate=True"
}
```

### Bước 3: Cấu hình dịch vụ bên thứ 3
1. **Cloudinary** (cho upload hình ảnh):
```json
"Cloudinary": {
  "CloudName": "your-cloud-name",
  "ApiKey": "your-api-key",
  "ApiSecret": "your-api-secret"
}
```

2. **Email Settings** (cho gửi email):
```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "SmtpPort": 587,
  "SenderEmail": "your-email@gmail.com",
  "SenderPassword": "your-app-password"
}
```

3. **VNPay** (cho thanh toán):
```json
"Vnpay": {
  "TmnCode": "your-tmn-code",
  "HashSecret": "your-hash-secret",
  "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html"
}
```

### Bước 4: Migrate cơ sở dữ liệu
```bash
dotnet ef database update
```

### Bước 5: Chạy ứng dụng
```bash
dotnet run
```

Ứng dụng sẽ chạy tại: `https://localhost:7xxx`

## Tài khoản mặc định

Khi chạy lần đầu, hệ thống sẽ tự động tạo tài khoản admin:
- **Email**: admin@example.com
- **Password**: Admin@123
- **Role**: Admin

## Database Schema

### Bảng chính
- **AspNetUsers**: Thông tin người dùng (Identity)
- **AspNetRoles**: Vai trò người dùng
- **Categories**: Danh mục sản phẩm
- **Products**: Sản phẩm
- **ProductPrices**: Lịch sử giá sản phẩm
- **Carts**: Giỏ hàng
- **Orders**: Đơn hàng
- **OrderDetails**: Chi tiết đơn hàng
- **Vourchers**: Mã giảm giá

### Quan hệ
- User 1-n Cart
- User 1-n Order
- Product 1-n Cart
- Product 1-n OrderDetail
- Category 1-n Product
- Order 1-n OrderDetail
- Vourcher 1-n Order

## API Endpoints

### Authentication
- `GET /Auth/Login` - Trang đăng nhập
- `POST /Auth/Login` - Xử lý đăng nhập
- `GET /Auth/Register` - Trang đăng ký
- `POST /Auth/Register` - Xử lý đăng ký
- `POST /Auth/Logout` - Đăng xuất

### Customer
- `GET /Customer/Cart` - Giỏ hàng
- `POST /Customer/AddToCart` - Thêm vào giỏ hàng
- `POST /Customer/UpdateCart` - Cập nhật giỏ hàng
- `GET /Customer/Checkout` - Trang thanh toán
- `GET /Customer/HistoryPayment` - Lịch sử đơn hàng

### Staff
- `GET /Staff/ManageProduct` - Quản lý sản phẩm
- `GET /Staff/CreateProduct` - Tạo sản phẩm
- `GET /Staff/ListCategories` - Quản lý danh mục
- `GET /Staff/ManageVourcher` - Quản lý voucher

### Admin
- `GET /Admin/ManageUser` - Quản lý người dùng
- `POST /Admin/Create` - Tạo người dùng
- `POST /Admin/Edit` - Sửa người dùng

## Tính năng nổi bật

### 1. Upload hình ảnh với Cloudinary
- Tự động resize và optimize hình ảnh
- CDN delivery để tăng tốc độ tải
- Quản lý storage hiệu quả

### 2. Thanh toán VNPay
- Tích hợp sandbox VNPay
- Xử lý callback an toàn
- Lưu trữ thông tin giao dịch

### 3. Email service
- Gửi email xác thực đăng ký
- Đặt lại mật khẩu qua email
- Template email đẹp mắt

### 4. Real-time với SignalR
- Cơ sở hạ tầng sẵn sàng cho tính năng real-time
- Hub service đã được cấu hình

### 5. Session management
- Quản lý giỏ hàng qua session
- Lưu trữ thông tin checkout
- Xử lý voucher

## Bảo mật

### Authentication & Authorization
- ASP.NET Core Identity với role-based access
- Password policy tùy chỉnh
- Email confirmation bắt buộc

### Data Protection
- HTTPS enforcement
- CSRF protection với ValidateAntiForgeryToken
- SQL Injection prevention với Entity Framework

### Input Validation
- Model validation với Data Annotations
- XSS prevention với HTML encoding
- File upload validation

## Performance

### Caching
- Output caching cho static content
- Session-based cart management

### Database
- Entity Framework với query optimization
- Pagination để giảm tải database
- Indexes trên các trường thường search

### Frontend
- SCSS compilation
- JavaScript minification
- Image optimization qua Cloudinary

## Testing

Để test các chức năng:

1. **Đăng ký tài khoản mới**
2. **Duyệt sản phẩm và thêm vào giỏ hàng**
3. **Áp dụng mã giảm giá** (tạo voucher từ staff panel)
4. **Thanh toán qua VNPay sandbox**
5. **Kiểm tra lịch sử đơn hàng**

## Giấy phép

Dự án này thuộc về FPT University - PRN222 Course Project.

## Liên hệ

- **Tác giả**: Học viên FPT University
- **Email**: hoapmhe173343@fpt.edu.vn
- **Khóa học**: PRN222 - Season 7 SP25

## Ghi chú kỹ thuật

### Package Dependencies
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.13)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.13)
- CloudinaryDotNet (1.27.4)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)

---

**Happy Coding! 🚀**
