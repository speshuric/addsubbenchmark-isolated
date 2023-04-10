using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class SubOneBenchmark
{
    public static IEnumerable<Params.BenchData> GetCases() => Params.ValuesForSub();
    public static IEnumerable<object[]> GetSizes() => Params.SizesSingle();

    [ParamsSource(nameof(GetCases))]    
    public Params.BenchData Case; 

    [Benchmark]
    [Arguments(1)]
    public void BenchSubOneEmpty(int leftSize)
    {
        uint tmp = Case.Left[leftSize-1];
        Case.Left[leftSize-1] = uint.MaxValue;
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractEmpty(left, right, res);
        Case.Left[leftSize-1] = tmp;
    }
    
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchSubOneBaseline(int leftSize)
    {
        uint tmp = Case.Left[leftSize-1];
        Case.Left[leftSize-1] = uint.MaxValue;
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractBaseline(left, right, res);
        Case.Left[leftSize-1] = tmp;
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchSubOneNew(int leftSize)
    {
        uint tmp = Case.Left[leftSize-1];
        Case.Left[leftSize-1] = uint.MaxValue;
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractNew(left, right, res);
        Case.Left[leftSize-1] = tmp;
    }

}

public class SubMultiBenchmark
{
    public static IEnumerable<Params.BenchData> GetCases() => Params.ValuesForSub();
    public static IEnumerable<object[]> GetSizes() => Params.SizesTwin();

    [ParamsSource(nameof(GetCases))]    
    public Params.BenchData Case; 

    [Benchmark]
    [Arguments(1, 1)]
    public void BenchSubMultiEmpty(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize - 1);
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractEmpty(left, right, res);
    }
    
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchSubMultiBaseline(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize - 1);
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractBaseline(left, right, res);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchSubMultiNew(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize - 1);
        Span<uint> res = Case.Result.AsSpan(0, leftSize);
        BigIntegerCalculator.SubtractNew(left, right, res);
    }

}
