# Be.Stateless.BizTalk.ServiceModel

##### Build Pipelines

[![][pipeline.mr.badge]][pipeline.mr]

[![][pipeline.ci.badge]][pipeline.ci]

##### Latest Release

[![][nuget.badge]][nuget]

[![][nuget.unit.badge]][nuget.unit]

[![][nuget.nunit.badge]][nuget.nunit]

[![][release.badge]][release]

##### Release Preview

[![][nuget.preview.badge]][nuget.preview]

[![][nuget.unit.preview.badge]][nuget.unit.preview]

[![][nuget.nunit.preview.badge]][nuget.nunit.preview]

##### Documentation

[![][doc.main.badge]][doc.main]

[![][doc.this.badge]][doc.this]

[![][help.badge]][help]

[![][help.unit.badge]][help.unit]

[![][help.nunit.badge]][help.nunit]

## Overview

`Be.Stateless.BizTalk.ServiceModel` is part of the [BizTalk.Factory Runtime][biztalk.factory.runtime] Package. This component provides both `WCF` behavior extensions dedicated to Microsoft BizTalk Server® and utility classes allowing to compose `WCF` relays using Microsoft BizTalk Server® schema and map artifacts.

## Running the Unit Tests

You will first need to execute the following commands in an `PowerShell` elevated session in order to be able to run all the unit tests:

```powershell
netsh http add urlacl url=http://+:8000/soap-stub/ user=$env:USERDOMAIN\$env:USERNAME
netsh http add urlacl url=http://+:8001/calculator/ user=$env:USERDOMAIN\$env:USERNAME
```

<!-- badges -->

[doc.main.badge]: https://img.shields.io/static/v1?label=BizTalk.Factory%20SDK&message=User's%20Guide&color=8CA1AF&logo=readthedocs
[doc.main]: https://www.stateless.be/ "BizTalk.Factory SDK User's Guide"
[doc.this.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.ServiceModel&message=User's%20Guide&color=8CA1AF&logo=readthedocs
[doc.this]: https://www.stateless.be/BizTalk/ServiceModel "Be.Stateless.BizTalk.ServiceModel User's Guide"
[github.badge]: https://img.shields.io/static/v1?label=Repository&message=Be.Stateless.BizTalk.ServiceModel&logo=github
[github]: https://github.com/icraftsoftware/Be.Stateless.BizTalk.ServiceModel "Be.Stateless.BizTalk.ServiceModel GitHub Repository"
[help.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.ServiceModel&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/ServiceModel/README.md "Be.Stateless.BizTalk.ServiceModel Developer Help"
[help.nunit.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.ServiceModel.NUnit&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help.nunit]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/ServiceModel/NUnit/README.md "Be.Stateless.BizTalk.ServiceModel.NUnit Developer Help"
[help.unit.badge]: https://img.shields.io/static/v1?label=Be.Stateless.BizTalk.ServiceModel.Unit&message=Developer%20Help&color=8CA1AF&logo=microsoftacademic
[help.unit]: https://github.com/icraftsoftware/biztalk.factory.github.io/blob/master/Help/BizTalk/ServiceModel/Unit/README.md "Be.Stateless.BizTalk.ServiceModel.Unit Developer Help"
[nuget.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.svg?label=Be.Stateless.BizTalk.ServiceModel&style=flat&logo=nuget
[nuget]: https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel "Be.Stateless.BizTalk.ServiceModel NuGet Package"
[nuget.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.ServiceModel?logo=nuget
[nuget.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.ServiceModel&protocolType=NuGet "Be.Stateless.BizTalk.ServiceModel Preview NuGet Package"
[nuget.nunit.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.NUnit.svg?label=Be.Stateless.BizTalk.ServiceModel.NUnit&style=flat&logo=nuget
[nuget.nunit]: https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel.NUnit "Be.Stateless.BizTalk.ServiceModel.NUnit NuGet Package"
[nuget.nunit.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.ServiceModel.NUnit?logo=nuget
[nuget.nunit.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.ServiceModel.NUnit&protocolType=NuGet "Be.Stateless.BizTalk.ServiceModel.NUnit Preview NuGet Package"
[nuget.unit.badge]: https://img.shields.io/nuget/v/Be.Stateless.BizTalk.ServiceModel.Unit.svg?label=Be.Stateless.BizTalk.ServiceModel.Unit&style=flat&logo=nuget
[nuget.unit]: https://www.nuget.org/packages/Be.Stateless.BizTalk.ServiceModel.Unit "Be.Stateless.BizTalk.ServiceModel.Unit NuGet Package"
[nuget.unit.preview.badge]: https://badge-factory.azurewebsites.net/package/icraftsoftware/be.stateless/BizTalk.Factory.Preview/Be.Stateless.BizTalk.ServiceModel.Unit?logo=nuget
[nuget.unit.preview]: https://dev.azure.com/icraftsoftware/be.stateless/_packaging?_a=package&feed=BizTalk.Factory.Preview&package=Be.Stateless.BizTalk.ServiceModel.Unit&protocolType=NuGet "Be.Stateless.BizTalk.ServiceModel.Unit Preview NuGet Package"
[pipeline.ci.badge]: https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.BizTalk.ServiceModel%20Continuous%20Integration?branchName=master&label=Continuous%20Integration%20Build
[pipeline.ci]: https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=64&branchName=master "Be.Stateless.BizTalk.ServiceModel Continuous Integration Build Pipeline"
[pipeline.mr.badge]: https://dev.azure.com/icraftsoftware/be.stateless/_apis/build/status/Be.Stateless.BizTalk.ServiceModel%20Manual%20Release?branchName=master&label=Manual%20Release%20Build
[pipeline.mr]: https://dev.azure.com/icraftsoftware/be.stateless/_build/latest?definitionId=65&branchName=master "Be.Stateless.BizTalk.ServiceModel Manual Release Build Pipeline"
[release.badge]: https://img.shields.io/github/v/release/icraftsoftware/Be.Stateless.BizTalk.ServiceModel?label=Release&logo=github
[release]: https://github.com/icraftsoftware/Be.Stateless.BizTalk.ServiceModel/releases/latest "Be.Stateless.BizTalk.ServiceModel GitHub Release"

<!-- links -->

[biztalk.factory.runtime]: https://www.stateless.be/BizTalk/Factory/Runtime "BizTalk.Factory Runtime"

<!--
cSpell:ignore netsh urlacl
-->
