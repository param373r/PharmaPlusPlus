# PharmaPlusPlus - Serving drugs at your doorstep

## Introduction
This was an old project I built about last year, made a few changes to polish with new features such as addition of user secrets and the use of EF Core, upgrade to .NET 8, and a major upgrade in the auth handling.

PharmaPlusPlus is an online application that offers a door-to-door delivery service for medicines by reducing the effort to go upto a local pharmacy. This application offers easy management for any local medical store out there to be able to upload and configure medicines in the API and let the consumers discover nearby shops offering the medicine they require. As of now, the design is pretty simple but will upgrade soon.

## Getting Started

### Prerequisites
- .NET Core SDK
- IDE such as Visual Studio Code

### Installation
1. `git clone`.
2. Navigate to the project directory.
3. `dotnet restore` to install all the required dependencies.
4. `dotnet run` to start the application.

## Features

### Current Features
- **API Versioning:**: API Versioning is made flexible using `Asp.Versioning` package.
- **Authentication and Authorization**: Jwt based authentication, Policy based access for Admin and User roles, uses built-in password hashing
- **Entity Framework Core**: Been used extensively for managing entities and relations.

### Future Enhancements
- **Cart and Ordering:**: For the current part it doesn't support adding to cart 
- **Switch to Identity Framework**: Switch completely to identity framework enabling new features like Email verification and TOTP integrations.
- **Use `FluentValidations`**: Better validations using a popular framework.
- **Add membership features and payment options**: Thinking of enabling a subscription based service for loyalty program and advanced discount tiring.
