# Smart Inventory System

A comprehensive web-based inventory management solution built with ASP.NET Core 6.0 and Entity Framework Core, designed to optimize business operations through efficient stock tracking and management.

![Smart Inventory System](https://via.placeholder.com/1200x600/4a90e2/ffffff?text=Smart+Inventory+System)


## 💼 Business Value

The Smart Inventory System addresses critical business challenges by:

- **Reducing Operating Costs**: Preventing overstock and stockouts through accurate tracking
- **Improving Decision Making**: Providing real-time insights into inventory status
- **Enhancing Efficiency**: Streamlining inventory operations with intuitive workflows
- **Preventing Revenue Loss**: Alerting when products reach reorder levels
- **Supporting Growth**: Scaling seamlessly with business expansion
- **Improving Accountability**: Tracking all stock movements with complete audit trail

## ✨ Key Features

### Product Management
- Create, view, edit, and delete products with detailed information
- Track product details including name, category, quantity, reorder levels and pricing
- View product history with complete audit trail of stock movements
- Low stock alerts for products requiring replenishment

### Supplier Management
- Maintain a comprehensive database of suppliers with contact information
- Associate suppliers with products for streamlined ordering
- Quick access to supplier details for efficient communication

### Inventory Control
- Record stock additions and removals with timestamp tracking
- View complete stock movement history with filtering capabilities
- Calculate inventory valuations automatically
- Track inventory turnover to identify slow-moving products

### User Management
- Role-based access control with Admin and Staff roles
- Admin role: Full access to all system features including user management
- Staff role: Limited to stock movement operations and viewing
- Extended employee information tracking (departments, job titles, etc.)
- Account security with password policies and lockout protection

### Dashboard
- Executive overview of key inventory metrics
- Low stock alerts highlighted for immediate attention
- Inventory valuation and turnover metrics
- Recent activity tracking with user attribution
- Quick access to common functions based on user role

## 🔧 Technology Stack

- **Framework**: ASP.NET Core 6.0
- **Architecture**: Razor Pages pattern for clean separation of concerns
- **ORM**: Entity Framework Core 6.0 with code-first approach
- **Database**: Microsoft SQL Server
- **Authentication**: ASP.NET Core Identity with extended user profiles
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Deployment**: Compatible with Azure App Service
- **Development Approach**: Clean code principles, DRY, and SOLID
- **Development Tools**: Visual Studio 2022, Git

## 🚦 Getting Started

### Prerequisites
- .NET 6.0 SDK or later
- SQL Server (Local DB, Express, or higher)
- Visual Studio 2022 (recommended) or Visual Studio Code

### Installation

1. Clone the repository
```
git clone https://github.com/SalemBamsaq/smart-inventory-system.git
```

2. Navigate to the project directory
```
cd smart-inventory-system
```

3. Restore dependencies
```
dotnet restore
```

4. Update the database with existing migrations
```
dotnet ef database update
```

5. Run the application
```
dotnet run
```

6. Navigate to `https://localhost:7280` in your web browser

### Default Accounts

The system seeds pre-configured accounts for immediate testing:

- **Admin User**
  - Email: admin@inventory.com
  - Password: Admin123!
  - Role: Admin (Full system access)

- **Staff User**
  - Email: staff@example.com  
  - Password: Staff@123
  - Role: Staff (Limited to operational tasks)

## 🏗️ Architecture

The application follows a modern, layered architecture:

- **Presentation Layer**: Razor Pages with Bootstrap 5
- **Business Logic Layer**: Page models and services
- **Data Access Layer**: Entity Framework Core with repositories
- **Database Layer**: SQL Server

## 📁 Project Structure

- `Areas/Identity`: Custom Identity pages for authentication and user management
- `Data`: Database context, migrations, and seeders
- `Models`: Domain entities with validation and business rules
- `Pages`: Razor Pages organized by feature
  - `Dashboard`: Main dashboard with KPIs and alerts
  - `Products`: Full product lifecycle management
  - `Suppliers`: Supplier relationship management
  - `StockMovements`: Inventory transaction tracking
  - `Users`: User administration and profile management
- `wwwroot`: Static assets and client-side libraries

## 🔒 Security Features

- **Authentication**: ASP.NET Core Identity with extended user profiles
- **Authorization**: Role-based access control (Admin, Staff)
- **Data Protection**: Input validation and sanitization
- **CSRF Protection**: Anti-forgery tokens on all forms
- **Password Policy**: Configurable password strength requirements
- **Account Lockout**: Protection against brute force attacks
- **Secure Configuration**: Environment-specific settings with user secrets

## 🔄 Deployment

The system can be deployed to various hosting environments:

- **Development**: Local IIS Express or Kestrel
- **Testing**: Docker containers
- **Production**: Azure App Service, IIS, or other ASP.NET Core compatible hosts

## 🧪 Testing

- **Unit Tests**: Testing individual components in isolation
- **Integration Tests**: Testing interactions between components
- **Manual Testing**: UI/UX testing for user workflows

## 🔮 Future Enhancements

- **Barcode/QR Code Integration**: Scan products for faster stock movements
- **Reporting Module**: Advanced reporting with export capabilities
- **Supplier Portal**: External access for suppliers
- **Mobile Application**: Native mobile experience for warehouse staff
- **API Integration**: Connect with accounting and e-commerce systems
- **Multi-location Support**: Manage inventory across multiple warehouses

## 📊 Performance Metrics

- Average page load: < 1.5 seconds
- Database query optimization for large inventories
- Responsive design for all device sizes
- Accessibility compliance with WCAG 2.1 guidelines

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Developer

Salem Bamsaq - Full Stack .NET Developer

- GitHub: [SalemBamsaq](https://github.com/SalemBamsaq)
- LinkedIn: [Salem Bamsaq](https://www.linkedin.com/in/salem-bamsaq-230565366/)

## 🙏 Acknowledgments

- [Microsoft ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Bootstrap Documentation](https://getbootstrap.com/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

---

## 📸 Screenshots

### Dashboard
![Dashboard](https://via.placeholder.com/800x400/4a90e2/ffffff?text=Executive+Dashboard)

### Product Management
![Products](https://via.placeholder.com/800x400/27ae60/ffffff?text=Product+Management)

### Stock Movement History
![Stock Movements](https://via.placeholder.com/800x400/8e44ad/ffffff?text=Stock+Movement+History)

### User Management
![User Management](https://via.placeholder.com/800x400/e74c3c/ffffff?text=User+Management)
