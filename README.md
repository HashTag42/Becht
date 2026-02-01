# Becht

Solutions to the problems listed in [`Playwright-Candidate-Lab 3.docx`](./Playwright-Candidate-Lab%203.docx)

## Summary

* Create Playwright scenarios to test the functionality of a fictitious shopping site at [https://www.saucedemo.com](https://www.saucedemo.com)

  * [x] Happy Path
  * [x] Failed Login
  * [x] Problem User
  * [x] Glitchy User
  * [x] Error User
  * [x] Visual Error

* Bonus scenarios:
  * [ ] Parameterized test
  * [ ] Run tests on Chrome and Edge or Firefox
  * [ ] Simulate a run on iPhone or Android
  * [ ] Performance/Load testing of 10 users
  * [ ] CI/CD gated check-in

## Solution structure

* Repo: [`https://github.com/HashTag42/Becht`](https://github.com/HashTag42/Becht)
  * [`Becht.sln`](./Becht.sln) - Solution file
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
      * [`ErrorUserTests.cs`](./SauceDemo/Tests/ErrorUserTests.cs) - Scenario 5
      * [`FailedLoginTests.cs`](./SauceDemo/Tests/FailedLoginTests.cs) - Scenario 2
      * [`GlitchyUserTests.cs`](./SauceDemo/Tests/GlitchyUserTests.cs) - Scenario 4
      * [`HappyPathTests.cs`](./SauceDemo/Tests/HappyPathTests.cs) - Scenario 1
      * [`ProblemUserTests.cs`](./SauceDemo/Tests/ProblemUserTests.cs) - Scenario 3
    * `./bin/Debug/net8.0/screenshots/` - Upon test execution, screenshots generated during the tests will be stored here

## Dependencies

* [`C#`](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [`.NET 8.0`](https://dotnet.microsoft.com/en-us/learn)
* [`Microsoft.Playwright 1.57`](https://playwright.dev/dotnet/docs/intro)
* [`xUnit.v3.mtp-v2 3.2.2`](https://xunit.net/docs/getting-started/v3/getting-started)
* [`Microsoft.Testing.Platform 2.0.2`](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro?tabs=dotnetcli)
* [`VS Code`](https://code.visualstudio.com/)
* Solution developed and tested on `Windows 11`

---
