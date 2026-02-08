# 🔐 Locker Room Management System

## 📖 About

The **Locker Room Management System** is a comprehensive web-based application designed to manage locker bookings, payments, and Point of Sale (POS) integration for facilities such as gyms, spas, hotels, and recreational centers. The system provides an efficient way to track locker availability, manage customer bookings with bracelet identification, handle multiple currencies, and seamlessly integrate with existing POS systems for automated billing.

This project is developed using **ASP.NET Core MVC** architecture, following the **Repository Pattern** and **Unit of Work** pattern to ensure clean separation of concerns, maintainability, and scalability.

---

## 🔧 Key Features

### 🔑 Locker Management
- **320 Lockers**: Support for 160 male lockers (M-1 to M-160) and 160 female lockers (F-1 to F-160)
- **Real-time Status Tracking**: Monitor occupied and available lockers in real-time
- **Locker Groups**: Organize lockers by sections for better facility management
- **Visual Interface**: Interactive locker grid showing availability status

### 👤 Booking System
- **Bracelet-Based Identification**: Customers identified via unique bracelet codes using barcode scanners
- **Quick Booking Process**: Streamlined workflow from locker selection to booking confirmation
- **Booking History**: Complete audit trail of all locker bookings with timestamps
- **Active Booking Management**: Track current active bookings with expiration times
- **Manual Unlock**: Staff can manually unlock lockers when needed

### 💰 Payment & Currency Management
- **Multi-Currency Support**: Handle bookings in multiple currencies (USD, EUR, EGP, etc.)
- **Dynamic Exchange Rates**: Update currency rates as needed
- **Payment Methods**: Support for Cash, Credit Card, and other payment types
- **Flexible Pricing**: Configure locker prices based on facility requirements

### 🏪 POS Integration
- **Automated Billing**: Seamless integration with existing POS systems (tested with Opera POS)
- **Check Creation**: Automatically create POS checks for each locker booking
- **Order Management**: Send order details including locker information and customer bracelet codes
- **Fallback Mode**: Continue operations even if POS system is temporarily unavailable
- **Comprehensive Logging**: Detailed logs of all POS transactions for troubleshooting

### 📊 Reporting & Logs
- **Booking Logs**: Complete history of all locker activities
- **POS Transaction Logs**: Track all POS integration attempts and responses
- **User Activity Tracking**: Monitor which staff members performed which actions
- **Audit Trail**: Full accountability with timestamps and user information

### 🔐 Security & Access Control
- **Session-Based Authentication**: Secure login system for staff members
- **User Management**: Track actions by username for accountability
- **Role-Based Operations**: Different access levels for reception staff and administrators

---

## 💻 Technologies Used

### Backend
- **ASP.NET Core MVC** (.NET 9.0)
- **Entity Framework Core** for data access
- **SQL Server** for database management
- **HttpClient** for POS API integration
- **System.Text.Json** for JSON serialization

### Frontend
- **HTML5, CSS3, JavaScript**
- **Bootstrap 5** for responsive UI
- **jQuery** for DOM manipulation
- **AJAX** for asynchronous operations

### Architecture & Patterns
- **Repository Pattern** for data access abstraction
- **Unit of Work Pattern** for transaction management
- **Dependency Injection** for service management
- **Layered Architecture** for separation of concerns

---

## 🏗 Architecture

This project follows a **layered architecture** with clear separation of concerns:

### 1. **Core Layer** (`LockerRoom.Core`)
- **Entities**: Domain models (Locker, Booking, CurrencyRate, LockerLog, etc.)
- **Interfaces**: Repository and service interfaces (IRepository, IUnitOfWork, IPOSService)
- **Business Logic**: Core domain rules and validations

### 2. **Infrastructure Layer** (`LockerRoom.Infrastructure`)
- **Data Context**: Entity Framework DbContext configuration
- **Repositories**: Implementation of repository interfaces
- **Unit of Work**: Transaction management implementation
- **Database Migrations**: Schema version control

### 3. **Application/Service Layer** (`LockerRoom.Core.Interfaces`)
- **POS Service**: External POS system integration logic
- **Business Services**: Application-specific business rules
- **DTOs**: Data transfer objects for API communication

### 4. **Presentation Layer** (`LockerRoom.Web`)
- **Controllers**: MVC controllers handling HTTP requests
- **Views**: Razor views for UI rendering
- **Static Files**: CSS, JavaScript, images
- **Configuration**: appsettings.json for application settings

---

## 📦 Project Structure
```
LockerRoom/
├── LockerRoom.Core/
│   ├── Entities/
│   │   ├── Locker.cs
│   │   ├── Booking.cs
│   │   ├── LockerLog.cs
│   │   └── CurrencyRate.cs
│   └── Interfaces/
│       ├── IRepository.cs
│       ├── IUnitOfWork.cs
│       └── IPOSService.cs
├── LockerRoom.Infrastructure/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Repositories/
│   │   ├── Repository.cs
│   │   └── UnitOfWork.cs
│   └── Migrations/
├── LockerRoom.Web/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   └── appsettings.json
└── README.md
```

---

## ⚙️ Configuration

### Database Connection
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LockerRoom_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

### POS Integration Settings
```json
{
  "POSSettings": {
    "BaseUrl": "http://YOUR_POS_SERVER/APP",
    "CheckOutletID": 12,
    "OrderOutletID": 13,
    "DefaultCashierID": 55,
    "DefaultServerID": 55,
    "LockerItemID": 430,
    "DefaultMenuID": 13,
    "DefaultCover": 1
  }
}
```

---

## 🚀 Getting Started

### Prerequisites
- **.NET 9.0 SDK**
- **SQL Server** (2017 or later)
- **Visual Studio 2022** or **VS Code**
- **Git**

### Installation Steps

1. **Clone the repository**
```bash
git clone https://github.com/ahmedbitar1/Lockers-Room-Management-System.git
cd Lockers-Room-Management-System
```

2. **Update database connection string** in `appsettings.json`

3. **Run database migrations**
```bash
dotnet ef database update
```

4. **Build the project**
```bash
dotnet build
```

5. **Run the application**
```bash
dotnet run --project LockerRoom.Web
```

6. **Access the application**
```
https://localhost:5001
```

---

## 🔌 POS Integration

The system integrates with external POS systems using RESTful APIs:

### Supported Features:
- ✅ Automatic check creation for locker bookings
- ✅ Order synchronization with locker details
- ✅ Bracelet code tracking in POS orders
- ✅ Fallback mode when POS is unavailable
- ✅ Detailed logging for troubleshooting

### POS Mapping:
- **Male Lockers**: M-1 to M-160 → POS Table Numbers 1-160
- **Female Lockers**: F-1 to F-160 → POS Table Numbers 161-320

### Integration Logs:
All POS transactions are logged to `C:\Temp\POS_Logs.txt` for debugging purposes.

---

## 📝 Usage Workflow

### 1. Customer Arrives
- Reception staff scans customer's bracelet
- System validates bracelet code

### 2. Select Locker
- View available lockers (male/female sections)
- Click on desired locker
- System shows locker details

### 3. Complete Booking
- Select currency and enter amount
- Choose payment method (Cash/Card)
- Confirm booking

### 4. POS Integration
- System automatically creates POS check
- Order sent with locker and bracelet details
- POS Check Number stored in booking record

### 5. Check-Out
- Scan bracelet to retrieve booking
- Manual unlock option available
- Booking closed and locker freed

---

## 🛠 Troubleshooting

### Common Issues:

**POS Integration Fails**
- Check `C:\Temp\POS_Logs.txt` for detailed error messages
- Verify POS server is accessible from application server
- Confirm POSSettings in appsettings.json are correct

**Lockers Not Showing**
- Ensure database is seeded with locker data
- Check browser console for JavaScript errors

**Booking Fails**
- Verify currency rates are configured
- Check database connection
- Review application logs

---

## 📬 Contact

**Project Owner**: Ahmed Essam  
**Email**: ahmedesamo778@gmail.com  

---

## 📄 License

This project is proprietary software developed for internal use.

---

## 🙏 Acknowledgments

- ASP.NET Core Team for the excellent framework
- Entity Framework Core for simplified data access
- Bootstrap for responsive UI components

--
README.md
