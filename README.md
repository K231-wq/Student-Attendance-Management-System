# Student Attendance Management System

## Overview
Student Attendance Management System ("Easy Attend") is a cross-platform application built using Microsoft .NET MAUI. It implements a **client-server architecture** for robust management of student attendance, supporting Android, iOS, Mac Catalyst, and Windows targets.

## System Architecture

### Client-Server Design

- **Client**: The frontend is developed using .NET MAUI and XAML, providing a responsive UI for teachers and students. It runs natively on Android, iOS, Mac, and Windows.
- **Server**: Backend services (implied by the client-server design message and typical for such systems) handle authentication, data persistence, and business logic. Communication likely happens via HTTP APIs or web services.

### Components and Modules

#### 1. **User Interface (View)**
- Built in XAML, with screens for login, registration, and dashboard.
- Responsive layouts using `<Grid>` and `<StackLayout>`.
- Includes theming (resources for colors, icons, splash screen).
- Example files: [LoginPage.xaml](https://github.com/K231-wq/Student-Attendance-Management-System/blob/main/Student%20Attendance%20Management%20System/View/login/LoginPage.xaml), [RegisterPage.xaml](https://github.com/K231-wq/Student-Attendance-Management-System/blob/main/Student%20Attendance%20Management%20System/View/login/RegisterPage.xaml)

#### 2. **ViewModels**
- Implements MVVM pattern, binding UI elements to view models for state and logic.
- Handles user input (email, password, username, major selection) and error messaging.
- Example binding: `Text="{Binding Email}"` in LoginPage, showing separation of UI and logic.

#### 3. **Model Layer**
- Defines domain entities such as Teacher, Student, AttendanceRecord, Majors.
- Provides validation and conversion utilities (e.g., IntToBoolConverter for checkbox states).

#### 4. **Backend Service Integration**
- Although explicit backend code isn't visible in the current search, the client-server note and MVVM pattern imply use of backend APIs:
    - **Authentication**: User credentials are sent to backend for validation.
    - **Registration**: New accounts are created via backend service.
    - **Attendance Management**: Attendance data is recorded/stored through server calls.
- Backend likely exposes RESTful APIs, with client-side code making HTTP requests for CRUD operations.

#### 5. **Data Persistence**
- Possible use of local storage (for cached data) and remote storage (via web APIs).
- Backend server/database interaction ensures centralization and consistency.

#### 6. **Platform Support**
- Target frameworks (from .csproj): 
    - Android: `net10.0-android`
    - iOS: `net10.0-ios`
    - Mac Catalyst: `net10.0-maccatalyst`
    - Windows: `net10.0-windows10.0.19041.0`

#### 7. **Resources & Assets**
- Custom icons, splash screens, and images for branding.
- Resource dictionaries for theme and converters.

## Component Interconnection

- **MVVM Pattern**: UI (View) binds to ViewModel, ViewModel interacts with Model, Model represents data.
- **Service Layer**: ViewModels call backend services via HTTP requests or abstractions, handling results and errors.
- **UI Updates**: Data changes in ViewModel update the UI automatically using data binding.
- **Authentication Flow**: 
    1. User submits login/register form.
    2. ViewModel collects input.
    3. Sends to backend service.
    4. Receives authentication/registration result.
    5. Shows error or proceeds to main dashboard.

## Sample Code Snippet (Login)
```xml
<!-- XAML Login Panel -->
<Entry Placeholder="Enter your email" Text="{Binding Email}" />
<Entry Placeholder="Enter your password" Text="{Binding Password}" IsPassword="True" />
<ActivityIndicator IsVisible="{Binding IsBusy}" IsRunning="{Binding IsBusy}" />
```
ViewModel handles logic, and backend service validates credentials.

## How Backend Services Are Used

- Backend APIs are called for:
    - User authentication and registration
    - Attendance marking and retrieval
    - Data validation/processing
- The client communicates with backend over network using HTTP requests (actual details found in service code, not visible here).
- Backend maintains central data consistency and security.

## Building & Running

- Requires [.NET MAUI](https://docs.microsoft.com/en-us/dotnet/maui/).
- Multi-platform support: Android, iOS, Mac, Windows.
- Use `dotnet build` and `dotnet run` for local development.

## Additional Information

- For full code and architecture exploration, visit the [GitHub repository](https://github.com/K231-wq/Student-Attendance-Management-System).
- Some code and details may not be included here due to search limitations. To view more results, use GitHub's code search.

