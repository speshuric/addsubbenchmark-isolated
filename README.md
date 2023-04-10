
Build:

```
dotnet build -c Release
```

Launch:

```
dotnet run -c Release -- -f "*" -h Toolchain --coreRun $path_to_corerun --iterationTime 300 --minIterationCount 6 --maxIterationCount 12 --minWarmupCount 6 --maxWarmupCount 12
```

Used options:
- `-f "*"` - all tests
- `-h Toolchain` - hide "Toolchain" column which contains long path. Use "Job" column to determine toolchain.
- `--coreRun $path_to_corerun` - point to baseline and testing corerun from local CLR build. The first has to point artifacts from `main` branch and used as baseline. The second points to testing bbranch.
- `--iterationTime 300 --minIterationCount 6 --maxIterationCount 12 --minWarmupCount 6 --maxWarmupCount 12` example of tunes for optimal speed/accuracy ()

**NOTE:** Test are quite long. Test cases can be adjusted by edit `Common.cs`
