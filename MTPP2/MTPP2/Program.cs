using System.Diagnostics;

var rnd = new Random();
var sw = new Stopwatch();
var threadsCounts = new[] { 2, 3, 4 };
var inputs = new[] { 10, 50, 100, 200 }.Select(x => Enumerable.Range(0, x).Select(_ => rnd.Next(-100, 101)).ToArray()).ToArray();

int F(int x)
{
    Thread.Sleep(10);
    return x + 5;
}

T WithDelay<T>(Func<T> func)
{
    Thread.Sleep(10);
    return func();
}

// init jit
var sample = Enumerable.Range(0, 10).ToArray();
var sampleResultSingle = sample.SelectFuncSingle(F);
var sampleResultParallel = sample.SelectFuncParallelForced(F, 3);
_ = sampleResultSingle.Select((x, i) => (x, i)).AggregateSingle((x, y) => WithDelay(() => x.Item1 < y.Item1 ? x : y));
_ = sampleResultParallel.Select((x, i) => (x, i)).AggregateParallelForced((x, y) => WithDelay(() => x.Item1 < y.Item1 ? x : y), 3);
//

foreach (var input in inputs)
{
    Console.WriteLine($"Input length: {input.Length}");
    Console.WriteLine();

    Console.WriteLine("Threads count: 1");

    sw.Restart();

    var resultSingle = input.SelectFuncSingle(F).ToArray();

    sw.Stop();

    Console.WriteLine("Applying func");
    Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
    Console.WriteLine();

    sw.Restart();

    var (resultSingleMinValue, resultSingleMinIndex) = resultSingle.Select((x, i) => (x, i)).AggregateSingle((x, y) => WithDelay(() => x.Item1 < y.Item1 ? x : y));
        
    sw.Stop();

    Console.WriteLine($"Min value: {resultSingleMinValue}, min index: {resultSingleMinIndex}");
    Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
    Console.WriteLine();

    foreach (var threadsCount in threadsCounts)
    {
        Console.WriteLine($"Threads count: {threadsCount}");

        sw.Restart();

        var resultParallel = input.SelectFuncParallelForced(F, threadsCount).ToArray();

        sw.Stop();

        Console.WriteLine("Applying func");
        Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
        Console.WriteLine();

        sw.Restart();

        var (resultParallelMinValue, resultParallelMinIndex) = resultParallel.Select((x, i) => (x, i)).AggregateParallelForced((x, y) => WithDelay(() => x.Item1 < y.Item1 ? x : y), threadsCount);
        
        sw.Stop();

        Console.WriteLine($"Min value: {resultParallelMinValue}, min index: {resultParallelMinIndex}");
        Console.WriteLine($"{sw.ElapsedMilliseconds} ms");
        Console.WriteLine();
    }
}

internal static class Helper
{
    public static IEnumerable<T> SelectFuncSingle<T>(this IEnumerable<T> source, Func<T, T> func) =>
        source.Select(func);

    public static ParallelQuery<T> SelectFuncParallelForced<T>(this IEnumerable<T> source, Func<T, T> func, int threadsCount) =>
        source
            .AsParallel()
            .WithDegreeOfParallelism(threadsCount)
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Select(func);

    public static T? AggregateSingle<T>(this IEnumerable<T> source, Func<T, T, T> func) =>
        source.Aggregate(func);

    public static T? AggregateParallelForced<T>(this IEnumerable<T> source, Func<T, T, T> func, int threadsCount) =>
        source
            .AsParallel()
            .WithDegreeOfParallelism(threadsCount)
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Aggregate(func);
}