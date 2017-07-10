# IntSharp

A rigorous interval arithmetic library for .NET 

# Quickstart for [Mono](http://www.mono-project.com/) (Linux)

* Clone or download IntSharp and change to it's root directory

```
git clone https://github.com/selmaohneh/IntSharp.git
cd /path/to/IntSharp
```

* Install the required packages [mono-complete](https://packages.debian.org/stretch/mono-complete)
and [nuget](https://packages.debian.org/stretch/nuget) from your distribution package manager.

```
apt-get install mono-complete nuget
```

* Use the [NuGet](https://www.nuget.org/) package mangager to obtain the remaining dependencies
[MathNet.Numerics](https://www.nuget.org/packages/MathNet.Numerics/3.19.0),
[NUnit](https://www.nuget.org/packages/NUnit/3.7.1), and
[NUnit.Console](https://www.nuget.org/packages/NUnit.Console/3.6.1):


```
mono /path/to/NuGet.exe update -self
mono /path/to/NuGet.exe install NUnit            -OutputDirectory packages -Version 3.7.1
mono /path/to/NuGet.exe install NUnit.Console    -OutputDirectory packages -Version 3.6.1
mono /path/to/NuGet.exe install MathNet.Numerics -OutputDirectory packages -Version 3.19.0
```

* Build the project:

```
xbuild IntSharp.sln
```

* Finally, run the tests:

```
mono packages/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe \
  IntSharpTests/bin/Debug/IntSharpTests.dll --noresult
```
```
NUnit Console Runner 3.6.1
Copyright (C) 2017 Charlie Poole

Runtime Environment
   OS Version: Linux 4.4.74.18
  CLR Version: 4.0.30319.42000

Test Files
    IntSharpTests/bin/Debug/IntSharpTests.dll


Run Settings
    DisposeRunners: True
    WorkDirectory: /workspace/IntSharp
    ImageRuntimeVersion: 4.0.30319
    ImageTargetFrameworkName: .NETFramework,Version=v4.6.1
    ImageRequiresX86: False
    ImageRequiresDefaultAppDomainAssemblyResolver: False
    NumberOfTestWorkers: 4

Test Run Summary
  Overall result: Passed
  Test Count: 93, Passed: 93, Failed: 0, Warnings: 0, Inconclusive: 0, Skipped: 0
  Start time: 2017-07-09 21:31:31Z
    End time: 2017-07-09 21:31:32Z
    Duration: 1.041 seconds
```
