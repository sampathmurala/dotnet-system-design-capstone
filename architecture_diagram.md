### **Food Delivery System \- Authentication Architecture**

This diagram illustrates the core components and the authentication flow for the Auth.Service. The primary goal is to offload authentication and user management to a dedicated Identity Provider (Keycloak), allowing the microservice to focus on its business logic.

#### **Component Roles:**

* **Client**: The frontend application (e.g., a web or mobile app) that a user interacts with. It initiates the authentication request.  
* **Keycloak (Identity Provider)**: The single source of truth for user identities and access management. It handles user login, token generation (JWTs), and user roles.  
* **Auth.Service (.NET Core)**: A microservice responsible for handling API requests. It does not manage user identities itself; instead, it validates the JWT tokens issued by Keycloak to protect its endpoints.

#### **Authentication Flow:**

1. The **Client** redirects the user to the **Keycloak** login page.  
2. The user logs in with their credentials on the **Keycloak** page.  
3. Upon successful login, **Keycloak** generates a JSON Web Token (JWT) and sends it back to the **Client**.  
4. The **Client** includes this JWT in the Authorization header of all subsequent API calls to the **Auth.Service**.  
5. The **Auth.Service** receives the request and uses the **Keycloak** public key to validate the JWT's signature and expiration.  
6. If the token is valid, the **Auth.Service** allows the request to proceed. If it is invalid or missing, the request is rejected with a 401 Unauthorized response.