# IntSharp

A rigorous interval arithmetic library for .NET

# Sample usage

This section will give some examples on how to use IntSharp. For further, advanced functions of IntSharp, it is
recommended to study the corresponding unit tests. They cover almost every aspect of
the implementation and give insights on how to use specific features.

* Create intervals with one of the several factory methods and calculate with them:

```
var i1 = Interval.FromInfSup(3.2,7); // [3.2 , 7]
var i2 = Interval.FromMidRad(4.8,2.2); // [2.6 , 7]
var i3 = Interval.FromDoublePrecise(9.34); // [9.34 , 9.34]

var res = (i1 * i2 + i3) / 2;
```

* Make verified calculations with included error propagation. No approximations, no derivations.

```
// Init interval with MidRad factory
var height = Interval.FromMidRad(3.8, 0.2);

// Init interval with InfSup factory
var accelerationOfGravity = Interval.FromInfSup(9.8, 9.82);

// Calculate the verified result
var velocity = Math.Sqrt(2 * height * accelerationOfGravity);
// = [8.4 , 8.863]
```

* Solve determined or overdetermined linear equation systems and obtain verified results:

```
// Init items for the matrix A.
var aItems = new[,]
{
    {Interval.FromMidRad(2,0.2), Interval.FromDoublePrecise(1)},
    {Interval.FromMidRad(7,0.2), Interval.FromDoublePrecise(1)}
};

// Init items for the rhs vector b.
var bItems = new[]
{
    Interval.FromMidRad(4,0.2),
    Interval.FromMidRad(1,0.2)
};

// Init matrix A and vector b.
var a = new IntervalMatrix(aItems);
var b = new IntervalVector(bItems);

// Solve the system with or withought error weighing:
var result = LinearEquationSystem.Solve(a, b);
```

Find verified enclosures of a function's root:

```
private static Interval MyTestFunction(Interval x)
{
    return 2 * Math.Pown(x, 3) - 9;
}

private static Interval MyTestDerivative(Interval x)
{
    return 6 * Math.Pown(x, 2);
}

public void FindTestRoot()
{
    var myTestRange = Interval.FromInfSup(0, 100);
    var root = RootFinding.FindRoot(
        MyTestFunction, 
        MyTestDerivative,
        myTestRange);
}
```

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
