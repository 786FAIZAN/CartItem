# Cart Item Management System

## Overview
This web application allows users to manage a shopping cart. Users can register, log in, add items to their cart, apply coupons, and view their cart contents. It is built using ASP.NET Core MVC and connects to a SQL Server database.

## Approach and Thought Process
The Cart Item Management System was developed with the following considerations:
- **User-Centric Design**: The application focuses on providing a seamless user experience for managing shopping carts and coupons.
- **Modular Architecture**: We implemented a repository pattern for data access, promoting separation of concerns and making the codebase easier to maintain and extend.
- **Security**: User credentials are handled with consideration for security, though password hashing should be implemented in a production environment.
- **Logging**: Action logging for coupon usage allows for better tracking and analysis of user behavior.

## Features
- **User Registration**: Users can create an account.
- **User Login**: Users can log in to their account.
- **Cart Management**:
  - Add items to the cart.
  - Update item quantities.
  - Remove items from the cart.
  - Clear the entire cart.
- **Coupon Management**:
  - Apply discount coupons.
  - View active coupons.
- **Logging**: Keep track of coupon actions for users.

## Technologies Used
- **Backend**: ASP.NET Core MVC (C#)
- **Database**: SQL Server
- **Dependency Injection**: Using built-in DI in ASP.NET Core with the repository pattern
- **Middleware**: Using both built-in and custom middleware
- **ORM**: ADO.NET

## Getting Started

### Prerequisites
- .NET Core MVC (version 8.0)
- SQL Server
- An IDE like Visual Studio

### Instructions for Running the Solution Locally
1. **Clone the Repository**:
   ```bash
   git clone <repository-url>
   cd CartItem
