# Be.Stateless.BizTalk.ServiceModel

[![Build Status](https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.BizTalk.ServiceModel%20Manual%20Release?branchName=master)](https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=65&branchName=master)
[![GitHub Release](https://img.shields.io/github/v/release/icraftsoftware/Be.Stateless.BizTalk.ServiceModel?label=Release&logo=github)](https://github.com/icraftsoftware/Be.Stateless.BizTalk.ServiceModel/releases/latest)

BizTalk.Factory's WCF relay composition with BizTalk Server schema and transform artifacts for general purpose Biztalk Server development.

## NuGet Packages

[![NuGet Version](https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.svg?label=Be.Stateless.BizTalk.ServiceModel&style=flat&logo=nuget)](https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel/)

[![NuGet Version](https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.Unit.svg?label=Be.Stateless.BizTalk.ServiceModel.Unit&style=flat&logo=nuget)](https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel.Unit/)

[![NuGet Version](https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.NUnit.svg?label=Be.Stateless.BizTalk.ServiceModel.NUnit&style=flat&logo=nuget)](https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel.NUnit/)


## Prerequisites for Unit Tests

Execute the following commands in an elevated PowerShell session before running the tests:

```powershell
netsh http add urlacl url=http://+:8000/soap-stub/ user=$Env:USERDOMAIN\$Env:USERNAME
netsh http add urlacl url=http://+:8001/calculator/ user=$Env:USERDOMAIN\$Env:USERNAME
```