# Becht

Solutions to the problems listed in [`Playwright-Candidate-Lab 3.docx`](./Playwright-Candidate-Lab%203.docx)

## Summary

* Create Playwright scenarios to test the functionality of a fictitious shopping site at [https://www.saucedemo.com](https://www.saucedemo.com)

  * [x] 1. Happy Path
  * [x] 2. Failed Login
  * [x] 3. Problem User
  * [x] 4. Glitchy User
  * [x] 5. Error User
  * [x] 6. Visual Error

* Bonus scenarios:
  * [x] 1. Parameterized test
  * [x] 2. Run tests on Chrome and Edge or Firefox
  * [ ] 3. Simulate a run on iPhone or Android
  * [ ] 4. Performance/Load testing of 10 users
  * [ ] 5. CI/CD gated check-in

## Structure

* Repo: [`https://github.com/HashTag42/Becht`](https://github.com/HashTag42/Becht)
  * [`Playwright.sln`](./Playwright.sln) - Solution file
  * `./SauceDemo/`
    * [`SauceDemo.csproj`](./SauceDemo/SauceDemo.csproj) - Project file
    * [`TestBase.cs`](./SauceDemo/TestBase.cs) - Abstract base class for Playwright tests that handles browser setup and teardown using xUnit's lifecycle
    * [`TestData.cs`](./SauceDemo/TestData.cs) - Configuration file containging static constants for use across tests
    * `./Pages/` - Page Object Model (POM) classes for Playwright testing against the SauceDemo website
      * [`LoginPage.cs`](./SauceDemo/Pages/LoginPage.cs) - Login page POM
      * [`InventoryPage.cs`](./SauceDemo/Pages/InventoryPage.cs) - Inventory page POM
      * [`CartPage.cs`](./SauceDemo/Pages/CartPage.cs) - Shopping cart POM
      * [`CheckoutPage.cs`](./SauceDemo/Pages/CheckoutPage.cs) - Checkout pages POM
    * `./Tests/` - xUnit test classes to the test different scenarios
      * [`HappyPathTests.cs`](./SauceDemo/Tests/HappyPathTests.cs) - Test Scenario 1
      * [`FailedLoginTests.cs`](./SauceDemo/Tests/FailedLoginTests.cs) - Test Scenario 2
      * [`ProblemUserTests.cs`](./SauceDemo/Tests/ProblemUserTests.cs) - Test Scenario 3
      * [`GlitchyUserTests.cs`](./SauceDemo/Tests/GlitchyUserTests.cs) - Test Scenario 4
      * [`ErrorUserTests.cs`](./SauceDemo/Tests/ErrorUserTests.cs) - Test Scenario 5
      * [`VisualUserTests.cs`](./SauceDemo/Tests/VisualUserTests.cs) - Test Scenario 6
      * [`ParameterizedTests.cs`](./SauceDemo/Tests/ParameterizedTests.cs) - Bonus Scenario 1
      * [`MultiBrowserTests.cs`](./SauceDemo/Tests/MultiBrowserTests.cs) - Bonus Scenario 2
    * `./bin/Debug/net8.0/screenshots/` - Upon test execution, screenshots generated during the tests will be stored here
    * `./bin/Debug/net8.0/logs/test-log.txt` - Upon test execution, a test log will be populated here

## Developed and tested using:

* ![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white) version `14`
* ![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4) `x64` version `8.0.417`
* ![Playwright](https://img.shields.io/badge/Playwright-2EAD33) version `1.57`
* ![xUnit v3](https://img.shields.io/badge/xUnit-v3-5C2D91) `mtp-v2` version `3.2.2`
* ![Microsoft Testing Platform](https://img.shields.io/badge/Microsoft.Testing.Platform-0078D4?logo=microsoft&logoColor=white) version `2.0.2`
* ![VS Code](https://img.shields.io/badge/VS%20Code-007ACC?logo=visualstudiocode&logoColor=white) version `1.108.2`
* ![Windows 11 Pro](https://img.shields.io/badge/Windows%2011%20Pro-0078D4) `64-bit` version `26200.7623`
* ![Chrome for Testing](https://img.shields.io/badge/Chrome%20for%20Testing-4285F4) `64-bit` version `145.0.7632.26`
* ![Chrome](https://img.shields.io/badge/Chrome-4285F4) `64-bit` version `144.0.7559.110`
* ![Edge](https://img.shields.io/badge/Edge-0078D7?logo=microsoftedge&logoColor=white) `64-bit` version `144.0.3719.92`
* ![Firefox](https://img.shields.io/badge/Firefox-FF7139) `64-bit` version `147.0.2`

---
