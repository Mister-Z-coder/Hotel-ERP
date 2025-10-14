# Hotel-ERP 🏨

Imagine a special computer program, like a super helper, made just for hotels! That's Hotel-ERP. It's designed to manage everything a hotel needs to run smoothly. This project is built using C#, a popular programming language.

Think of it as having different tools to help with:

*   **Room Management:** Keeping track of all the rooms, which ones are free, and which ones are occupied.
*   **Reservations:** Handling bookings from guests, so everyone gets the room they want.
*   **Billing:** Creating bills for guests and making sure everything is paid for.
*   **Stock Management:** Managing supplies, like towels and food, so the hotel never runs out.
*   **Reporting:** Giving reports to the hotel manager so they can see how the hotel is doing.

Basically, it's an all-in-one system to make running a hotel easier!

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![C#](https://img.shields.io/badge/C%23-code-blue)]()



## 🌟 Table of Contents
1.  [Description](#description)
2.  [Features](#features)
3.  [Tech Stack](#tech-stack)
4.  [Installation](#installation)
5.  [Usage](#usage)
6.  [Project Structure](#project-structure)
7.  [Contributing](#contributing)
8.  [License](#license)
9.  [Important Links](#important-links)
10. [Footer](#footer)



## 📝 Description
The Hotel-ERP project is a C# based application designed to streamline hotel operations. It includes modules for managing various aspects of a hotel, such as agents, aliments, beverages, room bookings, client details, declassifications, requests, currency, suppliers, inventory, payments, point of sale, work stations, regulations, reports, reservations, services, work shifts, stock control and user management. The project leverages .NET framework and potentially utilizes Entity Framework for database interactions as suggested by the presence of `ApplicationDbContext` and Migrations folders. The numerous controllers and views indicate a web-based interface for user interaction.



## ✨ Features
Based on the code analysis, the key features of the Hotel-ERP system include:

*   **Agent Management:** Managing agent information (creation, update, deletion).
*   **Aliment & Beverage Management:** Managing food and beverage items, categories, and related information.
*   **Room Management:** Handling room details, maintenance, and availability.
*   **Reservation Management:** Managing hotel reservations, including creation, modification, and billing.
*   **Client Management:** Handling client information and related operations.
*   **Supplier Management:** Managing supplier information, deliveries and supplies.
*   **Inventory Management:** Managing Stock levels through requests, deliveries, and stock takes.
*   **Point of Sale (POS):** Managing sales transactions, orders and table management.
*   **User Management:** User authentication, roles and permissions management.
*   **Report Generation:** Generating reports for various hotel operations, including sales, stock, and client data.
*   **Payment and Billing:** Managing payment settlements and invoices.
*   **Currency Management:** Managing currency exchange rates.
*   **Shift Management:** Managing work shifts for employees.
*   **Maintenance Management:** Managing room maintenance requests.



## 🛠️ Tech Stack
*   **Language:** C#
*   **Framework:** .NET
*   **Configuration:** JSON
*   **Frontend:** CSS, JavaScript
*   **Libraries/Frameworks:** TypeScript, Python (potentially for tooling, not core functionality).
*   **Database:** Entity Framework (inferred from the presence of `ApplicationDbContext` and Migrations).



## ⚙️ Installation
1.  **Clone the Repository:**

    ```bash
    git clone https://github.com/Mister-Z-coder/Hotel-ERP.git
    cd Hotel-ERP
    ```

2.  **Install .NET SDK:**

    Ensure you have the .NET SDK installed. You can download it from [Microsoft .NET Downloads](https://dotnet.microsoft.com/download).

3.  **Configuration:**

    Configure the database connection string in `Hotel.Web/appsettings.json`.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "Your_Connection_String_Here"
      },
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
      },
      "AllowedHosts": "*"
    }
    ```

4.  **Apply Migrations:**

    Navigate to the `Hotel.Web` directory and apply the database migrations.

    ```bash
    cd Hotel.Web
    dotnet ef database update
    ```

5.  **Build the Project:**

    Build the solution using the .NET CLI.

    ```bash
    dotnet build
    ```

6.  **Run the Application:**

    Run the application.

    ```bash
    dotnet run
    ```



## 🚀 Usage
1.  **Access the Application:**

    Open your web browser and navigate to the URL where the application is running (e.g., `http://localhost:5000`).

2.  **Login:**

    Use the provided credentials or create a new user account to log in.

3.  **Navigate the Modules:**

    Use the navigation menu to access different modules such as Room Management, Reservation Management, POS, and Reports.

4.  **Perform Hotel Operations:**

    Utilize the forms and views provided to manage hotel operations such as creating reservations, managing inventory, processing payments, and generating reports.



## 📂 Project Structure
The project structure is organized as follows:

```
Hotel-ERP/
├── Hotel.Application/         # Application Layer
├── Hotel.Domain/              # Domain Layer
├── Hotel.Infrastructure/      # Infrastructure Layer
├── Hotel.Web/                # ASP.NET Core Web Application
│   ├── Controllers/         # MVC Controllers
│   ├── Models/              # Data Models and View Models
│   ├── Views/               # Razor Views
│   ├── Data/                # Database Context and Migrations
│   ├── wwwroot/             # Static Files (CSS, JavaScript, Images)
│   ├── appsettings.json     # Application Configuration
│   ├── Program.cs           # Application Entry Point
│   └── Startup.cs           # Application Startup Configuration
├── RDLCDesign/              # RDLC Report Design Project
├── Hotel.sln                 # Solution File
└── ...
```



## 🤝 Contributing
Contributions are welcome! Here's how to contribute:

1.  Fork the repository.
2.  Create a new branch for your feature or bug fix.
3.  Make your changes and commit them with clear, descriptive messages.
4.  Submit a pull request.



## 📜 License
This project is licensed under the MIT License - see the [LICENSE](https://opensource.org/licenses/MIT) file for details.



## 🔗 Important Links
*   **GitHub Repository:** [https://github.com/Mister-Z-coder/Hotel-ERP](https://github.com/Mister-Z-coder/Hotel-ERP)



## <footer>
<p align="center">
  <a href="https://github.com/Mister-Z-coder/Hotel-ERP">Hotel-ERP on GitHub</a> | Made with ❤️ by Mister-Z-coder<br>
  ⭐ Fork the project and give it a star! ⭐
</p>

</footer>
