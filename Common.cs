namespace Benchmarks;

public class Params 
{
    public const int MaxSize = 65536*16;

    public struct BenchData {
        public string Name;
        public uint[] Left;
        public uint[] Right;
        public uint[] Result;
        public int Delta;
        public override string ToString()
        {
            return Name;
        }
    }

    public static IEnumerable<Params.BenchData> ValuesForAdd()
    {
        BenchData good = new() {
            Name = "01-good(add)",
            Left = new uint[MaxSize],
            Right = new uint[MaxSize],
            Result = new uint[MaxSize+1],
            Delta = 1
        };
        Array.Fill<uint>(good.Left, 1);
        Array.Fill<uint>(good.Right, 1);
        yield return good;

        BenchData bad = new() {
            Name = "02-bad(add)",
            Left = new uint[MaxSize],
            Right = new uint[MaxSize],
            Result = new uint[MaxSize+1],
            Delta = -1
        };
        Array.Fill<uint>(bad.Left, uint.MaxValue);
        Array.Fill<uint>(bad.Right, uint.MaxValue);
        yield return bad;
    }

    public static IEnumerable<Params.BenchData> ValuesForSub()
    {
        BenchData good = new() {
            Name = "03-good(sub)",
            Left = new uint[MaxSize],
            Right = new uint[MaxSize],
            Result = new uint[MaxSize],
            Delta = 1
        };
        Array.Fill<uint>(good.Left, uint.MaxValue);
        Array.Fill<uint>(good.Right, uint.MaxValue);
        yield return good;

        BenchData bad = new() {
            Name = "04-bad(sub)",
            Left = new uint[MaxSize],
            Right = new uint[MaxSize],
            Result = new uint[MaxSize],
            Delta = -1
        };
        Array.Fill<uint>(bad.Left, 0);
        Array.Fill<uint>(bad.Right, uint.MaxValue);

        yield return bad;
    }    

    public static IEnumerable<object[]> SizesSingle()
    {
        var p = new int[] {1, 2, 4, 8, 16, 64, 128, 256, 1024, 4096, 16384, 65536};

        foreach (int i in p)
        {
            yield return new object[] {i};
        }
    }

    public static IEnumerable<object[]> SizesTwin()
    {
        var p = new int[] {1, 2, 4, 8, 16, 64, 128, 256, 1024, 4096, 16384, 65536};

        foreach (int i in p)
        {
            if (i >= 1) yield return new object[] {i, 1};
            if (i >= 2) yield return new object[] {i, 2};
            if (i >= 4) yield return new object[] {i, 4};

            if (i/2 > 4) yield return new object[] {i, i/2};

            if (i > 4) yield return new object[] {i, i-1};
            if (i > 4) yield return new object[] {i, i};

        }

    }



}
