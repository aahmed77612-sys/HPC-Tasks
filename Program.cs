using System.Diagnostics;

namespace Version1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Why not Parallel ?
            // 1. Overhead of managing multiple threads
            // 2. Not enough work to justify parallelism



            int[] nums = Enumerable.Range(1, 10000000).ToArray();
            long sum = 0;
            var processorCount = Environment.ProcessorCount;
            var sw = Stopwatch.StartNew();

            // Sequential Sum
            for (int i = 0; i < nums.Length; i++)
            {
                sum += nums[i];
            }
            sw.Stop();

            Console.WriteLine($"Sequential Sum: {sum}, Sequential Time taken: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("-----------------------------------------------------------------------");
            var sequentialTime = sw.ElapsedMilliseconds;

            //---------------------------------------------------------------

            // Parallel Sum
            sw.Restart();
            sum = 0;
            Parallel.For(0, nums.Length,
                        () => 0L,   // initial local sum لكل thread
                        (i, loopState, localSum) =>
                        {
                            localSum += nums[i];

                            return localSum;
                        },
                        localSum => Interlocked.Add(ref sum, localSum)  
            );
            sw.Stop();
            Console.WriteLine($"Parallel Sum: {sum}, Parallel Time taken: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("-----------------------------------------------------------------------");
            var parallelTime = sw.ElapsedMilliseconds;

            //----------------------------------------------------------------

            // Parallel Sum with MaxDegreeOfParallelism

            sum = 0;
            var optionsAll = new ParallelOptions { MaxDegreeOfParallelism = processorCount };

            sw.Restart();
            Parallel.For(0, nums.Length, optionsAll,
                () => 0L,
                (i, loopState, localSum) =>
                {
                    localSum += nums[i];
                    return localSum;
                },
                localSum => Interlocked.Add(ref sum, localSum)
            );
            sw.Stop();
            var parallelAllTime = sw.ElapsedMilliseconds;
            Console.WriteLine($"Parallel Sum On Many Cores: {sum}, Parallel Time taken On Many Cores: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine("-----------------------------------------------------------------------");


            //----------------------------------------------------------------

            // Speedup and Efficiency
            Console.WriteLine($"Speedup With Parallel on 1 Core: {(double)sequentialTime / parallelTime:F2}x");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine($"Speedup With Parallel on Many Cores: {(double)sequentialTime / parallelAllTime:F2}x");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine($"Relative Speedup: {(double)parallelTime / parallelAllTime:F2}x");
            Console.WriteLine("-----------------------------------------------------------------------------");
            Console.WriteLine($"Efficiency: {((double)sequentialTime / parallelAllTime) / processorCount:F2}");

        }
    }
}
