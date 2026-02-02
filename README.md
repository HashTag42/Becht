# SauceDemo Playwright Test Suite

![Playwright Tests](https://github.com/HashTag42/Becht/actions/workflows/playwright-tests.yml/badge.svg)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![Playwright](https://img.shields.io/badge/Playwright-1.57-2EAD33)
![xUnit v3](https://img.shields.io/badge/xUnit-v3-5C2D91)

End-to-end test automation suite for [SauceDemo](https://www.saucedemo.com) using Playwright and xUnit v3.

* Test scenarios

  * [x] 1. Happy Path
  * [x] 2. Failed Login
  * [x] 3. Problem User
  * [x] 4. Glitchy User
  * [x] 5. Error User
  * [x] 6. Visual Error

* Bonus scenarios
  * [x] 1. Parameterized test
  * [x] 2. Run tests on Chrome and Edge or Firefox
  * [x] 3. Simulate a run on iPhone or Android
  * [x] 4. Performance/Load testing of 10 users
  * [x] 5. CI/CD gated check-in

## Features

* Page Object Model architecture
* Cross-browser testing (Chrome, Edge, Firefox)
* Mobile device emulation (iPhone, Android)
* Load testing with concurrent users
* CI/CD integration with GitHub Actions
* Screenshot capture on test failures
* Detailed test logging

## Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Quick Start

```bash
# Clone the repository
git clone https://github.com/HashTag42/Becht.git
cd Becht/SauceDemo

# Restore dependencies
dotnet restore

# Install Playwright browsers
pwsh bin/Debug/net8.0/playwright.ps1 install

# Run all tests
dotnet test

# Run a specific test class
dotnet run -- -method "*StandardUser_CanCompleteFullPurchaseFlow"
```

## Test Scenarios

| Scenario | Description | File |
| -------- | ----------- | ---- |
| Happy Path | Complete purchase flow with standard user | `HappyPathTests.cs` |
| Failed Login | Invalid credentials handling | `FailedLoginTests.cs` |
| Problem User | Tests with problem_user account | `ProblemUserTests.cs` |
| Glitchy User | Tests with performance_glitch_user | `GlitchyUserTests.cs` |
| Error User | Tests with error_user account | `ErrorUserTests.cs` |
| Visual Error | Tests with visual_user account | `VisualUserTests.cs` |

### Bonus Scenarios

| Scenario | Description | File |
| -------- | ----------- | ---- |
| Parameterized | Data-driven tests across multiple users | `ParameterizedTests.cs` |
| Multi-Browser | Chrome, Edge, and Firefox execution | `MultiBrowserTests.cs` |
| Mobile Emulation | iPhone and Android simulation | `MobileEmulationTests.cs` |
| Load Testing | 10 concurrent user simulation | `LoadTests.cs` |
| CI/CD | GitHub Actions gated check-in | `playwright-tests.yml` |

## Project Structure

```text
SauceDemo/
├── Pages/                    # Page Object Models
│   ├── LoginPage.cs
│   ├── InventoryPage.cs
│   ├── CartPage.cs
│   └── CheckoutPage.cs
├── Tests/                    # Test classes
│   ├── HappyPathTests.cs
│   ├── FailedLoginTests.cs
│   └── ...
├── TestBase.cs               # Base class with browser setup/teardown
├── TestData.cs               # Test constants and configuration
└── SauceDemo.csproj
```

## CI/CD

Tests run automatically on:

* Pull requests to `main`
* Pushes to `main`

Failed test screenshots are uploaded as artifacts for debugging.

## Test Output

After running tests:

* **Screenshots**: `bin/Debug/net8.0/screenshots/`
* **Logs**: `bin/Debug/net8.0/logs/test-log.txt`

---
