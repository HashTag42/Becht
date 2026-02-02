# SauceDemo Playwright Test Suite

![Playwright Tests](https://github.com/HashTag42/Becht/actions/workflows/playwright-tests.yml/badge.svg)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4)
![Playwright](https://img.shields.io/badge/Playwright-1.57-2EAD33)
![xUnit v3](https://img.shields.io/badge/xUnit-v3-5C2D91)

End-to-end test automation suite for [SauceDemo](https://www.saucedemo.com) using Playwright and xUnit v3.

## Features

- Page Object Model architecture
- Cross-browser testing (Chrome, Edge, Firefox)
- Mobile device emulation (iPhone, Android)
- Load testing with concurrent users
- CI/CD integration with GitHub Actions
- Screenshot capture on test failures
- Detailed test logging

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

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
```

## Running Tests

```bash
# Run all tests
dotnet test

# Run a specific test class
dotnet test --filter "HappyPathTests"

# Run with detailed output
dotnet test --verbosity normal
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

- Pull requests to `main`
- Pushes to `main`

Failed test screenshots are uploaded as artifacts for debugging.

## Test Output

After running tests:

- **Screenshots**: `bin/Debug/net8.0/screenshots/`
- **Logs**: `bin/Debug/net8.0/logs/test-log.txt`

## Developed and Tested using

- ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
- ![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4) `x64` version `8.0.417`
- ![Playwright](https://img.shields.io/badge/Playwright-2EAD33) version `1.57`
- ![xUnit v3](https://img.shields.io/badge/xUnit-v3-5C2D91) `mtp-v2` version `3.2.2`
- ![Microsoft Testing Platform](https://img.shields.io/badge/Microsoft.Testing.Platform-0078D4?logo=microsoft&logoColor=white) version `2.0.2`
- ![VS Code](https://img.shields.io/badge/VS%20Code-007ACC?logo=visualstudiocode&logoColor=white) version `1.108.2`
- ![Windows 11 Pro](https://img.shields.io/badge/Windows%2011%20Pro-0078D4) `64-bit` version `26200.7623`
- ![Chrome for Testing](https://img.shields.io/badge/Chrome%20for%20Testing-4285F4) `64-bit` version `145.0.7632.26`
- ![Chrome](https://img.shields.io/badge/Chrome-4285F4) `64-bit` version `144.0.7559.110`
- ![Edge](https://img.shields.io/badge/Edge-0078D7?logo=microsoftedge&logoColor=white) `64-bit` version `144.0.3719.92`
- ![Firefox](https://img.shields.io/badge/Firefox-FF7139) `64-bit` version `147.0.2`

---
