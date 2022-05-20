using System.Diagnostics;

namespace MTPP3;

public class Program
{
    public static void Main()
    {
        var n = int.Parse(ReadNamed("N"));
        var k = int.Parse(ReadNamed("K"));
        var p = double.Parse(ReadNamed("P"));
        var mode = int.Parse(ReadNamed("Mode (1 - time; 2 - ticks)"));

        var crystal = new Crystal(n, k);
        var last = n - 1;
        var particlesPositions = Enumerable.Repeat(0, k).ToArray();

        Action<int, int, double, Crystal> func;

        if (mode == 1)
        {
            var time = int.Parse(ReadNamed("Time"));
            var delay = int.Parse(ReadNamed("Delay"));

            var ticks = time / delay;

            func =
                (initialPos, last, p, crystal) =>
                {
                    var curPos = initialPos;

                    for (var tick = 0; tick < ticks; ++tick)
                    {
                        curPos = Helper.ParticleTick(curPos, last, p, crystal);
                        Thread.Sleep(delay);
                    }
                };
        }
        else
        {
            var ticks = int.Parse(ReadNamed("Ticks"));

            func =
                (initialPos, last, p, crystal) =>
                {
                    var curPos = initialPos;

                    for (var tick = 0; tick < ticks; ++tick)
                    {
                        curPos = Helper.ParticleTick(curPos, last, p, crystal);
                    }
                };
        }

        Console.WriteLine(string.Join(' ', crystal.Data));

        var parallelOpts = new ParallelOptions { MaxDegreeOfParallelism = 4 };

        var sw = Stopwatch.StartNew();

        Parallel.ForEach
        (
            particlesPositions,
            parallelOpts,
            particlesPosition =>
            {
                func(particlesPosition, last, p, crystal);
            }
        );

        sw.Stop();

        Console.WriteLine($"{sw.ElapsedMilliseconds} ms");

        Console.WriteLine(string.Join(' ', crystal.Data));
    }

    public static string? ReadNamed(string name)
    {
        Console.Write($"{name}: ");
        return Console.ReadLine();
    }
}

public static class Helper
{
    public static readonly Random Random = new();

    public static void MoveParticle(this Crystal crystal, int from, int to)
    {
        lock (crystal.LockObj)
        {
            --crystal.Data[from];
            ++crystal.Data[to];
        }
    }

    public static int GetParticleNewPosition(int curPos, int last, double p)
    {
        if (curPos == 0) return 1;
        if (curPos == last) return curPos - 1;
        return p > Random.NextDouble() ? curPos + 1 : curPos - 1;
    }

    public static int ParticleTick(int curPos, int last, double p, Crystal crystal)
    {
        var newPos = GetParticleNewPosition(curPos, last, p);
        crystal.MoveParticle(curPos, newPos);
        return newPos;
    }
}

public class Crystal
{
    public object LockObj;

    public int[] Data;

    public Crystal(int n, int k)
    {
        LockObj = new object();
        Data = new int[n];
        Data[0] = k;
    }
}
