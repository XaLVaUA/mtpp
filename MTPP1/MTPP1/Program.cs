using System.Diagnostics;

namespace MTPP1;

internal static class Program
{
    private static void Main()
    {
        var data = Enumerable.Range(0, 100).ToArray();
        var dataSourcesPaths = new[] {"data3_1.txt", "data3_2.txt", "data_3_3.txt"};

        var degreesOfParallelism = new[] {2, 3, 4};
        var stopwatch = new Stopwatch();

        Console.WriteLine("Single thread");
        Console.WriteLine();
        Console.WriteLine("CPU bound");

        stopwatch =
            Execute
            (
                () =>
                {
                    _ = data.Select(CpuBoundCalculate).ToArray();
                },
                stopwatch
            );

        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine();

        Console.WriteLine("Memory bound");

        stopwatch =
            Execute
            (
                () =>
                {
                    _ = data.Select(ReadMemoryBoundData).Select(Calculate).ToArray();
                },
                stopwatch
            );

        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine();

        Console.WriteLine("IO bound");

        stopwatch =
            Execute
            (
                () =>
                {
                    _ = dataSourcesPaths.SelectMany(x => ReadIOBoundDataChunk(x, int.Parse)).Select(Calculate).ToArray();
                },
                stopwatch
            );

        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
        Console.WriteLine();

        void TestParallel(int degreeOfParallelism)
        {
            Console.WriteLine($"Degree of parallelism = {degreeOfParallelism}");
            Console.WriteLine();
            Console.WriteLine("CPU bound");

            stopwatch =
                Execute
                (
                    () =>
                    {
                        _ = data.GetForceParallelQuery(degreeOfParallelism).Select(CpuBoundCalculate).ToArray();
                    },
                    stopwatch
                );

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            Console.WriteLine("Memory bound");

            stopwatch =
                Execute
                (
                    () =>
                    {
                        _ = data.GetForceParallelQuery(degreeOfParallelism).Select(ReadMemoryBoundData).AsEnumerable().Select(Calculate).ToArray();
                    },
                    stopwatch
                );

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine();

            Console.WriteLine("IO bound");

            stopwatch =
                Execute
                (
                    () =>
                    {
                        _ = dataSourcesPaths.GetForceParallelQuery(degreeOfParallelism).SelectMany(x => ReadIOBoundDataChunk(x, int.Parse)).AsEnumerable().Select(Calculate).ToArray();
                    },
                    stopwatch
                );

            Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine();
        }

        foreach (var degreeOfParallelism in degreesOfParallelism)
        {
            TestParallel(degreeOfParallelism);
        }
    }

    private static ParallelQuery<T> GetForceParallelQuery<T>(this IEnumerable<T> source, int degreeOfParallelism) =>
        source
            .AsParallel()
            .WithDegreeOfParallelism(degreeOfParallelism)
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism);

    private static void GenerateDataSources<T>(IEnumerable<T> data, IEnumerable<string> dataSourcesPaths, int dataLength, int dataSourcesPathsLength)
    {
        var size = dataLength / dataSourcesPathsLength;

        foreach (var (dataChunk, dataSourcePath) in data.Chunk(size).Zip(dataSourcesPaths))
        {
            File.WriteAllText(dataSourcePath, string.Join(' ', dataChunk));
        }
    }

    private static int Calculate(int x) => x + 1;

    // simulate heavy calculation
    private static int CpuBoundCalculate(int x)
    {
        Thread.Sleep(10);
        return Calculate(x);
    }

    // simulate slow memory
    private static T ReadMemoryBoundData<T>(T data)
    {
        Thread.Sleep(10);
        return data;
    }

    private static IEnumerable<T> ReadIOBoundDataChunk<T>(string dataSourcePath, Func<string, T> parseFunc) => 
        File.ReadAllText(dataSourcePath).Split().Select(parseFunc);

    private static Stopwatch Execute(Action action, Stopwatch stopwatch)
    {
        stopwatch.Restart();
        action();
        stopwatch.Stop();

        return stopwatch;
    }
}