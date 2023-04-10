using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public class AddOneBenchmark
{
    public static IEnumerable<Params.BenchData> GetCases() => Params.ValuesForAdd();
    public static IEnumerable<object[]> GetSizes() => Params.SizesSingle();

    [ParamsSource(nameof(GetCases))]    
    public Params.BenchData Case; 

    [Benchmark]
    [Arguments(1)]
    public void BenchAddOneEmpty(int leftSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddEmpty(left, right, res);
    }
    
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchAddOneBaseline(int leftSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddBaseline(left, right, res);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchAddOneNew(int leftSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        uint right = Case.Right[0];
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddNew(left, right, res);
    }

}

public class AddMultiBenchmark
{
    public static IEnumerable<Params.BenchData> GetCases() => Params.ValuesForAdd();
    public static IEnumerable<object[]> GetSizes() => Params.SizesTwin();

    [ParamsSource(nameof(GetCases))]    
    public Params.BenchData Case; 

    [Benchmark]
    [Arguments(1, 1)]
    public void BenchAddMultiEmpty(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize);
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddEmpty(left, right, res);
    }
    
    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchAddMultiBaseline(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize);
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddBaseline(left, right, res);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetSizes))]
    public void BenchAddMultiNew(int leftSize, int rightSize)
    {
        ReadOnlySpan<uint> left = Case.Left.AsSpan(0, leftSize);
        ReadOnlySpan<uint> right = Case.Right.AsSpan(0, rightSize);
        Span<uint> res = Case.Result.AsSpan(0, leftSize + 1);
        BigIntegerCalculator.AddNew(left, right, res);
    }

}
