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
  * [ ] 2. Run tests on Chrome and Edge or Firefox
  * [ ] 3. Simulate a run on iPhone or Android
  * [ ] 4. Performance/Load testing of 10 users
  * [ ] 5. CI/CD gated check-in

## Solution structure

* Repo: [`https://github.com/HashTag42/Becht`](https://github.com/HashTag42/Becht)
  * [`Playwright.sln`](./Playwright.sln) - Solution file
  * `./SauceDemo/`
    * [`SauceDemo.csproj`](./SauceDemo/SauceDemo.csproj) - Project file
    * [`TestBase.cs`](./SauceDemo/TestBase.cs) - Abstract base class for Playwright tests that handles browser setup and teardown using xUnit's lifecycle
    * [`TestData.cs`](./SauceDemo/TestData.cs) - Configuration file containging static constants for use across tests
    * `./Pages/` - Page Object Model (POM) classes for Playwright testing against the SauceDemo website
      * [`CartPage.cs`](./SauceDemo/Pages/CartPage.cs)
      * [`CheckoutPage.cs`](./SauceDemo/Pages/CheckoutPage.cs)
      * [`InventoryPage.cs`](./SauceDemo/Pages/InventoryPage.cs)
      * [`LoginPage.cs`](./SauceDemo/Pages/LoginPage.cs)
    * `./Tests/` - xUnit test classes to the test different scenarios
      * [`HappyPathTests.cs`](./SauceDemo/Tests/HappyPathTests.cs) - Test Scenario 1
      * [`FailedLoginTests.cs`](./SauceDemo/Tests/FailedLoginTests.cs) - Test Scenario 2
      * [`ProblemUserTests.cs`](./SauceDemo/Tests/ProblemUserTests.cs) - Test Scenario 3
      * [`GlitchyUserTests.cs`](./SauceDemo/Tests/GlitchyUserTests.cs) - Test Scenario 4
      * [`ErrorUserTests.cs`](./SauceDemo/Tests/ErrorUserTests.cs) - Test Scenario 5
      * [`VisualUserTests.cs`](./SauceDemo/Tests/VisualUserTests.cs) - Test Scenario 6
      * [`ParameterizedTests.cs`](./SauceDemo/Tests/ParameterizedTests.cs) - Bonus Scenario 1
    * `./bin/Debug/net8.0/screenshots/` - Upon test execution, screenshots generated during the tests will be stored here
    * `./bin/Debug/net8.0/logs/test-log.txt` - Upon test execution, a test log will be populated here

## Dependencies

Developed and tested using:

* `C#` (version 14)
* `.NET SDK 8.0` (x64 version 8.0.417)
* `Microsoft.Playwright` (version 1.57)
* `xUnit.v3` (mtp-v2 version 3.2.2)
* `Microsoft.Testing.Platform` (version 2.0.2)
* `VS Code` (version 1.108.2)
* `Windows 11 Pro` (64-bit version 26200.7623)
* `Chrome for Testing` (64-bit version 145.0.7632.26)
* `Chrome` (64-bit version 144.0.7559.110)
* `Edge` (64-bit version 144.0.3719.92)
* `Firefox` (64-bit version 147.0.2)

---
