# Becht

Solutions to the problems listed in [`Playwright-Candidate-Lab 3.docx`](./Playwright-Candidate-Lab%203.docx)

## Summary

* Create Playwright scenarios to test the functionality of a fictitious shopping site at [https://www.saucedemo.com](https://www.saucedemo.com)

  * [ ] Happy Path
  * [ ] Failed Login
  * [ ] Problem User
  * [ ] Glitchy User
  * [ ] Error User
  * [ ] Visual Error

* Bonus scenarios:
  * [ ] Parameterized test
  * [ ] Run tests on Chrome and Edge or Firefox
  * [ ] Simulate a run on iPhone or Android
  * [ ] Performance/Load testing of 10 users
  * [ ] CI/CD gated check-in

## Solution

* Repo: [`https://github.com/HashTag42/Becht`](https://github.com/HashTag42/Becht)
  * `Becht.sln`
  * `SauceDemo/`
    * `SauceDemo.csproj`
    * `TestBase.cs`: Abstract base class for Playwright tests that handles browser setup and teardown using xUnit's lifecycle.
    * `Pages/`
      * `LoginPage.cs`: Page Object Model (POM) class for Playwright testing against the SauceDemo website.
    * `Tests/`
      * `HappyPathTests.cs`

## Dependencies

* [`C#`](https://learn.microsoft.com/en-us/dotnet/csharp/)
* [`.NET 8.0`](https://dotnet.microsoft.com/en-us/learn)
* [`Microsoft.Playwright 1.57`](https://playwright.dev/dotnet/docs/intro)
* [`xUnit.v3.mtp-v2 3.2.2`](https://xunit.net/docs/getting-started/v3/getting-started)
* [`Microsoft.Testing.Platform 2.0.2`](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-intro?tabs=dotnetcli)
* [`VS Code`](https://code.visualstudio.com/)

---
