# Food Delivery System: A .NET Core Microservices Capstone

📝 Project Overview
This repository contains a series of microservices and infrastructure components for a scalable food delivery application. The project is a capstone that demonstrates expertise in modern system design concepts, including microservices architecture, API Gateways, service discovery, and event-driven communication.

Each major component is built as a separate project to showcase a specific system design pattern. The entire system is containerized with Docker to ensure a consistent and portable development environment.

### 🛠️ Technologies Used

| Category | Technology | Why We Chose It |
| :--- | :--- | :--- |
| **Backend** | .NET Core | A high-performance, cross-platform framework for building robust microservices. |
| **Identity Management** | Keycloak | An open-source Identity and Access Management (IAM) solution. We chose this over a custom solution to leverage its enterprise-grade security features, OIDC/OAuth 2.0 standards compliance, and scalability. This allows us to focus on the application's business logic. |
| **Containerization** | Docker | Ensures a consistent development environment for all microservices and dependencies (like Keycloak), simplifying setup and deployment. |



## 🚀 Getting Started
Follow these steps to set up and run the Auth.Service project.

### Prerequisites
* .NET SDK 8.0
* Docker Desktop

### 1. Start the Keycloak Identity Provider
Open your terminal and run the following Docker command. This will pull the Keycloak image and start the server.

docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:22.0.0 start-dev

### 2. Keycloak Configuration
Once Keycloak is running, open a web browser and navigate to http://localhost:8080 to log in to the admin console with the username admin and password admin.

**Follow these steps to set up the client for our service:**

* Create a new realm named FoodDelivery.
* Inside the FoodDelivery realm, create a new OpenID Connect client named food-delivery-app.
* In the client's configuration, set the Valid Redirect URIs to http://localhost:5000/signin-oidc.
* Navigate to the Credentials tab for your client and copy the Client Secret.
