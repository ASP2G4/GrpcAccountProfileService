# GrpcAccountProfileService

// Har använt Ai - assistans för texter, dummydata och få upp diagramen i github

A .NET 9 gRPC service for managing user account profiles and addresses, using Entity Framework Core and SQL Server.

## Features

- Create, retrieve, update, and delete user account profiles
- Each profile includes personal information and an address
- Strongly-typed, efficient communication via gRPC
- Integration with SQL Server using Entity Framework Core

## Technologies & Packages

- **.NET 9**
- **gRPC**
  - Grpc.AspNetCore
  - Grpc.Tools
  - Google.Protobuf
- **Entity Framework Core**
  - Microsoft.EntityFrameworkCore
  - Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.Data.SqlClient
- **Testing**
  - xUnit
  - Microsoft.EntityFrameworkCore.InMemory

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server instance (local or Azure)
- (Optional) gRPC testing tools:
  - Postman (with gRPC support)

### Setup

1. Clone the repository:
   ```sh
   git clone <your-repo-url>
   cd GrpcAccountProfileService
   ```
---
### gRPC Endpoints
All endpoints are defined in Protos/profile.proto.
 ```sh
1. GetProfileByUserId
RPC:
rpc GetProfileByUserId(GetProfileByUserIdRequest) returns (GetProfileByUserIdReply);

Request Example:
{
  "user_id": "12345"
}
Response Example:
{
  "profile": {
    "id": "1",
    "user_id": "12345",
    "first_name": "John",
    "last_name": "Doe",
    "phone_number": "555-1234",
    "address": {
      "street_name": "Main St 1",
      "postal_code": "12345",
      "city": "Stockholm"
    }
  },
  "status_code": 200
}

---
2. CreateProfile
RPC:
rpc CreateProfile(CreateProfileRequest) returns (CreateProfileReply);
Request Example:
{
  "profile": {
    "user_id": "12345",
    "first_name": "John",
    "last_name": "Doe",
    "phone_number": "555-1234",
    "address": {
      "street_name": "Main St 1",
      "postal_code": "12345",
      "city": "Stockholm"
    }
  }
}

Response Example:
{
  "success": true,
  "message": "Account succesfully created"
}

3. UpdateProfile
RPC:
rpc UpdateProfile(UpdateProfileRequest) returns (UpdateProfileReply);
Request Example:
{
  "profile": {
    "user_id": "12345",
    "first_name": "Jane",
    "last_name": "Doe",
    "phone_number": "555-5678",
    "address": {
      "street_name": "Updated St 2",
      "postal_code": "54321",
      "city": "Gothenburg"
    }
  }
}
Response Example:
{
  "success": true
}
---
4. DeleteProfileByUserId
RPC:
rpc DeleteProfileByUserId(DeleteProfileByUserIdRequest) returns (DeleteProfileByUserIdReply);
Request Example:
{
  "user_id": "12345"
}
Response Example:
{
  "success": true
}
```
---
  ### Project Structure
  - GrpcAccountProfileService/Services/ProfileService.cs – gRPC service implementation
  - GrpcAccountProfileService/Contexts/AccountProfileContext.cs – EF Core DbContext
  - GrpcAccountProfileService/Entities/ – Entity models
  - GrpcAccountProfileService/Protos/profile.proto – gRPC service and message definitions
  - AccountProfileService.Tests/ – Integration and unit tests

### Testing
  - Integration tests are located in the AccountProfileService.Tests project.

### Notes
  - The service requires HTTP/2 for gRPC endpoints.
  - For browser-based clients, consider adding gRPC-Web support.
  - For Azure App Service deployment, use a Linux plan for native gRPC support.


Activity Diagram
Create Profile
flowchart TD
    Start([Start])
    Request[Client sends CreateProfile request]
    Validate[Validate input]
    Exists{Does profile with UserId exist?}
    Create[Create AccountProfile entity]
    Save[Save to database]
    Success[Return: Success=true]
    Fail[Return: Success=false]
    End([End])

    Start --> Request --> Validate --> Exists
    Exists -- Yes --> Fail
    Exists -- No --> Create --> Save --> Success --> End
    Fail --> End

Get Profile By UserId
flowchart TD
    Start([Start])
    Request[Client sends GetProfileByUserId request]
    Lookup[Find profile by UserId in database]
    Found{Profile found?}
    ReturnProfile[Return profile and status 200]
    NotFound[Return null and status 404]
    End([End])

    Start --> Request --> Lookup --> Found
    Found -- Yes --> ReturnProfile --> End
    Found -- No --> NotFound --> End

Update Profile
flowchart TD
    Start([Start])
    Request[Client sends UpdateProfile request]
    Validate[Validate input]
    Lookup[Find profile by UserId in database]
    Found{Profile found?}
    Update[Update profile fields]
    Save[Save changes to database]
    Success[Return: Success=true]
    Fail[Return: Success=false]
    End([End])

    Start --> Request --> Validate --> Lookup --> Found
    Found -- Yes --> Update --> Save --> Success --> End
    Found -- No --> Fail --> End

Delete Profile By UserId
flowchart TD
    Start([Start])
    Request[Client sends DeleteProfileByUserId request]
    Lookup[Find profile by UserId in database]
    Found{Profile found?}
    Delete[Delete profile from database]
    Save[Save changes]
    Success[Return: Success=true]
    Fail[Return: Success=false]
    End([End])

    Start --> Request --> Lookup --> Found
    Found -- Yes --> Delete --> Save --> Success --> End
    Found -- No --> Fail --> End

    
Sequence Diagram 
Create Profile
sequenceDiagram
    participant Client
    participant Service as ProfileService
    participant DB as Database

    Client->>Service: CreateProfile(request)
    Service->>Service: Validate request
    Service->>DB: Check if UserId exists
    alt UserId exists
        Service-->>Client: CreateProfileReply(Success=false, Message="A profile with the given UserId already exists.")
    else UserId does not exist
        Service->>DB: Save AccountProfile
        DB-->>Service: Confirmation
        Service-->>Client: CreateProfileReply(Success=true, Message="Account succesfully created")
    end
    
Get Profile By UserId
sequenceDiagram
    participant Client
    participant Service as ProfileService
    participant DB as Database

    Client->>Service: GetProfileByUserId(request)
    Service->>DB: Find profile by UserId
    alt Profile found
        DB-->>Service: Profile entity
        Service-->>Client: GetProfileByUserIdReply(Profile, StatusCode=200)
    else Not found
        DB-->>Service: null
        Service-->>Client: GetProfileByUserIdReply(Profile=null, StatusCode=404)
    end

Update Profile
sequenceDiagram
    participant Client
    participant Service as ProfileService
    participant DB as Database

    Client->>Service: UpdateProfile(request)
    Service->>Service: Validate request
    Service->>DB: Find profile by UserId
    alt Profile found
        DB-->>Service: Profile entity
        Service->>DB: Update and save profile
        DB-->>Service: Confirmation
        Service-->>Client: UpdateProfileReply(Success=true)
    else Not found
        DB-->>Service: null
        Service-->>Client: UpdateProfileReply(Success=false)
    end

Delete Profile By UserId
sequenceDiagram
    participant Client
    participant Service as ProfileService
    participant DB as Database

    Client->>Service: DeleteProfileByUserId(request)
    Service->>DB: Find profile by UserId
    alt Profile found
        DB-->>Service: Profile entity
        Service->>DB: Delete profile
        DB-->>Service: Confirmation
        Service-->>Client: DeleteProfileByUserIdReply(Success=true)
    else Not found
        DB-->>Service: null
        Service-->>Client: DeleteProfileByUserIdReply(Success=false)
    end
