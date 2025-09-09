OK, let's start with the first project: the **Identity Provider**.

## Project 1: Identity Provider (Auth Service)

**The Goal:** Build a secure and scalable authentication service that handles user registration and login. We'll use a modern approach by integrating with a robust, open-source identity and access management (IAM) solution: **Keycloak**. This demonstrates your expertise in leveraging powerful existing tools rather than building from scratch.

### Architectural Overview

Instead of building a full-fledged authentication system ourselves, our .NET service will act as an **OAuth 2.0 client** to Keycloak.

1.  **User Interaction:** A user interacts with the frontend application (which you'll build in the capstone).
2.  **Redirect to Keycloak:** The frontend redirects the user to the Keycloak login page.
3.  **Authentication:** Keycloak authenticates the user and, upon success, returns an **authorization code**.
4.  **Token Exchange:** Our .NET Auth Service receives this code and exchanges it with Keycloak for a set of **access tokens** and **refresh tokens**.
5.  **Secure API Calls:** The client application uses the access token to make secure, authenticated calls to our other microservices.

### Step-by-Step Implementation

#### 1\. Set Up Keycloak

  * **Installation:** The easiest way to get Keycloak running is with Docker.
    ```bash
    docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin quay.io/keycloak/keycloak:22.0.0 start-dev
	docker run -p 8080:8080 -e KEYCLOAK_ADMIN=admin -e KEYCLOAK_ADMIN_PASSWORD=admin -e KC_HOSTNAME=localhost -v keycloak-data:/opt/keycloak/data quay.io/keycloak/keycloak:22.0.0 start --http-enabled=true --http-port=8080
    ```
  * **Configuration:** Go to `http://localhost:8080` to access the admin console.
      * Create a new realm (e.g., "FoodDelivery").
      * Create a new client (e.g., "food-delivery-app") and configure it for **OpenID Connect**.
      * Set the **valid redirect URI** (e.g., `http://localhost:5000/signin-oidc`). This is crucial for the callback after login.
      * Note down the `Client ID` and `Client Secret`.

#### 2\. Create the .NET Auth Service

  * **New Project:** Create a new ASP.NET Core Web API project.
    ```bash
    dotnet new webapi -n Auth.Service
    cd Auth.Service
    ```
  * **Install Packages:** Add the necessary NuGet packages for OpenID Connect integration.
    ```bash
    dotnet add package Microsoft.AspNetCore.Authentication.OpenIdConnect
    dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    ```
  * **Configure `Program.cs`:** Configure the OpenID Connect middleware. This middleware will handle the token exchange and user authentication.
    ```csharp
    // Add authentication and authorization services
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "Cookies"; // Use cookies to store the token
        options.DefaultChallengeScheme = "oidc";
    })
    .AddCookie("Cookies")
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "http://localhost:8080/realms/FoodDelivery";
        options.ClientId = "food-delivery-app";
        options.ClientSecret = "YOUR_KEYCLOAK_CLIENT_SECRET"; // Get this from Keycloak
        options.ResponseType = "code";
        options.SaveTokens = true;
        options.RequireHttpsMetadata = false; // For development only
    });

    builder.Services.AddAuthorization();
    // ...
    app.UseAuthentication();
    app.UseAuthorization();
    ```
  * **Create an API Endpoint:** Add a simple protected endpoint to the `WeatherForecastController` (or a new controller) to demonstrate that authentication is working.
    ```csharp
    [HttpGet]
    [Authorize] // This attribute protects the endpoint
    public IEnumerable<WeatherForecast> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
    ```

### What to Showcase

  * **Repository Structure:** Organize your code with clear folders (`Services`, `Controllers`, `Models`).
  * **README.md:** This is your sales pitch.
      * **Architecture Diagram:** A simple diagram showing the client, the .NET Auth Service, and Keycloak.
      * **Design Choices:** Explain why you chose Keycloak over building a custom solution (security, standards-compliance, scalability, etc.). Mention the use of **OpenID Connect** and **OAuth 2.0**.
      * **Usage:** Provide clear instructions on how to set up Keycloak and run the .NET service. Show an example of how a client would interact with the service and receive an access token.
	  

### Step-by-Step Testing Guide (for PowerShell)

1.  **Launch Your Application:**
    Open your terminal and run the two lines above. This will start your application on port `5000`.

2.  **Test the Public Endpoint:**
    Open a **new** PowerShell terminal and run the test command.

    ```powershell
    curl http://localhost:5000/public
    ```

    You should see a successful response.

3.  **Get a JWT Token from Keycloak:**
    Run this command to get your authentication token.

    ```powershell
    curl -X POST -H 'Content-Type: application/x-www-form-urlencoded' -d 'grant_type=password&username=admin&password=admin&client_id=food-delivery-app&client_secret=YOUR_KEYCLOAK_CLIENT_SECRET http://localhost:8080/realms/FoodDelivery/protocol/openid-connect/token
	
	curl -X POST -H "Content-Type: application/x-www-form-urlencoded" -d "grant_type=password&username=user001&password=user001&client_id=food-delivery-app&client_secret=ze5EbKhq5vdIXy3SUiLA0pSA1QQE9N6L" http://localhost:8080/realms/FoodDelivery/protocol/openid-connect/token
	curl -X POST -H "Content-Type: application/x-www-form-urlencoded" -d "grant_type=password&username=user001&password=user001&client_id=food-delivery-app&client_secret=ze5EbKhq5vdIXy3SUiLA0pSA1QQE9N6L" http://localhost:8080/realms/FoodDelivery/protocol/openid-connect/token                                                                                                                                                              
    ```

      * **Note:** In PowerShell, the backtick `` ` `` is used to continue a command on the next line.

4.  **Test the Protected Endpoint (Success Case):**
    Use the `access_token` you received from the previous step.

    ```dos
     curl http://localhost:5000/weatherforecast -H "Authorization: Bearer eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJ0bjlMUmJ0cWRnaVNpSnZjOHNvQ0t6WE5qclh5ODNpU0xfTWp0clFqazd3In0.eyJleHAiOjE3NTczMzY4MTEsImlhdCI6MTc1NzMzNjUxMSwianRpIjoiMmU4NzViYjAtNWVmOC00MjY1LWEzM2ItZTFiZjJmZTZjNjNhIiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo4MDgwL3JlYWxtcy9Gb29kRGVsaXZlcnkiLCJhdWQiOlsiZm9vZC1kZWxpdmVyeS1hcHAiLCJhY2NvdW50Il0sInN1YiI6ImE4OWQyYTQ4LTk0YjEtNDRlMS1iYjJjLThlMDE3ZmU2NjUzZSIsInR5cCI6IkJlYXJlciIsImF6cCI6ImZvb2QtZGVsaXZlcnktYXBwIiwic2Vzc2lvbl9zdGF0ZSI6IjI0ZWI1MjllLTQ0OWMtNGE4Yy04MjE4LTI0YzIzOTM5MGExMiIsImFjciI6IjEiLCJhbGxvd2VkLW9yaWdpbnMiOlsiaHR0cDovL2xvY2FsaG9zdDo1MDAwIl0sInJlYWxtX2FjY2VzcyI6eyJyb2xlcyI6WyJvZmZsaW5lX2FjY2VzcyIsImRlZmF1bHQtcm9sZXMtZm9vZGRlbGl2ZXJ5IiwidW1hX2F1dGhvcml6YXRpb24iLCJjdXN0b21lciJdfSwicmVzb3VyY2VfYWNjZXNzIjp7ImFjY291bnQiOnsicm9sZXMiOlsibWFuYWdlLWFjY291bnQiLCJtYW5hZ2UtYWNjb3VudC1saW5rcyIsInZpZXctcHJvZmlsZSJdfX0sInNjb3BlIjoiZm9vZC1kZWxpdmVyeS1hcGktYXVkaWVuY2UgZW1haWwgcHJvZmlsZSIsInNpZCI6IjI0ZWI1MjllLTQ0OWMtNGE4Yy04MjE4LTI0YzIzOTM5MGExMiIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJwcmVmZXJyZWRfdXNlcm5hbWUiOiJ1c2VyMDAxIiwiZ2l2ZW5fbmFtZSI6IiIsImZhbWlseV9uYW1lIjoiIiwiZW1haWwiOiJ1c2VyMDAxX3Rlc3RAeW9wbWFpbC5jb20ifQ.u_AED4VXwCxQTY1kZxtqdGuUJuYYxbG1BEUCth1XQixQcprxxu8CUjxZCTffdpO3fwSKmrRDT6DxTHpUA6iJzPPvgL0yIkB70U5UKu-gQEYz1CyxYpYnUHx8Z3oQoPdjSuiDxX1Qs1vEQsm0c1CdHjDdMQLZ8ywJrmN0AN4P10Ol0Rq2NIn0Gu5Z_Vs5j31h7oJnCsceXywC1K0q4hv6USyrSE32eKX_6i3J2vDekGfx1Ki8dSjl3NMgyK4ZZU1TtRDlcTeXBtAcdFdQnqK4yR_87Y-LP0hDPZ7Npe2T43oCsXDIkKk0qZNOxkFlEb7cT0UQBTCIye9PAuSAANuJ2Q"
    ```

This sequence of commands, tailored for PowerShell, should resolve the parsing error and allow you to test your authentication flow successfully.