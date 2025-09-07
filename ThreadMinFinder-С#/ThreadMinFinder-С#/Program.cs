using System;
using System.Threading;

namespace ThreadMinFinder_CSharp
{
    class Program
    {
        private static int dim;
        private static int threadNum;

        private Thread[] threads;
        private int[] arr;

        private int globalMin = int.MaxValue;
        private int globalMinIndex = -1;

        private int threadCount = 0;
        private readonly object lockerForMin = new object();
        private readonly object lockerForCount = new object();

        static void Main(string[] args)
        {
            int defaultDim = 10_000_000;
            int defaultThreadNum = 10;

            Console.Write($"Введіть розмір масиву (Enter для {defaultDim}): ");
            string? dimInput = Console.ReadLine();

            Console.Write($"Введіть кількість потоків (Enter для {defaultThreadNum}): ");
            string? threadInput = Console.ReadLine();

            dim = ParseOrDefault(dimInput, defaultDim);
            threadNum = ParseOrDefault(threadInput, defaultThreadNum);

            Program main = new Program();
            main.InitArrays();
            main.ParallelMinSearch();

            Console.WriteLine($"Global min value: {main.globalMin} at index: {main.globalMinIndex}");
            Console.ReadKey();
        }

        private static int ParseOrDefault(string? input, int defaultValue)
        {
            if (string.IsNullOrWhiteSpace(input))
                return defaultValue;

            if (int.TryParse(input, out int value) && value > 0)
                return value;

            Console.WriteLine($"Некоректне значення ({input}), використано за замовчуванням: {defaultValue}");
            return defaultValue;
        }

        private void InitArrays()
        {
            arr = new int[dim];
            threads = new Thread[threadNum];

            Random rnd = new Random();
            for (int i = 0; i < dim; i++)
            {
                arr[i] = rnd.Next(100_000);
            }

            // Вставляем случайное отрицательное число
            int negativeIndex = rnd.Next(dim);
            arr[negativeIndex] = -rnd.Next(1, 100);
        }

        private void ParallelMinSearch()
        {
            int chunkSize = dim / threadNum;

            for (int i = 0; i < threadNum; i++)
            {
                int start = i * chunkSize;
                int end = (i == threadNum - 1) ? dim : start + chunkSize;

                // Передаем Bound и индекс потока
                threads[i] = new Thread(StarterThread);
                threads[i].Start(new Tuple<Bound, int>(new Bound(start, end), i));
            }

            lock (lockerForCount)
            {
                while (threadCount < threadNum)
                {
                    Monitor.Wait(lockerForCount);
                }
            }
        }

        private void StarterThread(object? param)
        {
            if (param is Tuple<Bound, int> tuple)
            {
                Bound bound = tuple.Item1;
                int threadIndex = tuple.Item2;

                int localMin = int.MaxValue;
                int localIndex = -1;

                for (int i = bound.StartIndex; i < bound.FinishIndex; i++)
                {
                    if (arr[i] < localMin)
                    {
                        localMin = arr[i];
                        localIndex = i;
                    }
                }

                Console.WriteLine($"[Thread {threadIndex}] Checked range [{bound.StartIndex}, {bound.FinishIndex}] " +
                                  $"Local min = {localMin} at index {localIndex}");

                lock (lockerForMin)
                {
                    if (localMin < globalMin)
                    {
                        globalMin = localMin;
                        globalMinIndex = localIndex;
                    }
                }

                IncrementThreadCount();
            }
        }

        private void IncrementThreadCount()
        {
            lock (lockerForCount)
            {
                threadCount++;
                Monitor.Pulse(lockerForCount);
            }
        }

        class Bound
        {
            public int StartIndex { get; }
            public int FinishIndex { get; }

            public Bound(int start, int finish)
            {
                StartIndex = start;
                FinishIndex = finish;
            }
        }
    }
}
