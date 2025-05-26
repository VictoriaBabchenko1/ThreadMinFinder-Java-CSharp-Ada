using System;
using System.Threading;

class MinFinder
{
    private static readonly object lockObj = new object();
    private static int globalMin = int.MaxValue;
    private static int globalIndex = -1;

    static void Main()
    {
        int arraySize = 1_000_000;
        int threadCount = 4;
        int[] array = GenerateArray(arraySize);

        Thread[] threads = new Thread[threadCount];
        int chunkSize = arraySize / threadCount;

        for (int i = 0; i < threadCount; i++)
        {
            int start = i * chunkSize;
            int end = (i == threadCount - 1) ? arraySize : start + chunkSize;
            threads[i] = new Thread(() =>
            {
                int localMin = int.MaxValue;
                int localIndex = -1;
                for (int j = start; j < end; j++)
                {
                    if (array[j] < localMin)
                    {
                        localMin = array[j];
                        localIndex = j;
                    }
                }
                lock (lockObj)
                {
                    if (localMin < globalMin)
                    {
                        globalMin = localMin;
                        globalIndex = localIndex;
                    }
                }
            });
            threads[i].Start();
        }

        // Очікування завершення потоків без використання Join()
        while (true)
        {
            bool allDone = true;
            foreach (var t in threads)
            {
                if (t.IsAlive)
                {
                    allDone = false;
                    break;
                }
            }
            if (allDone) break;
        }

        Console.WriteLine("Мінімальне значення: " + globalMin);
        Console.WriteLine("Індекс мінімального значення: " + globalIndex);
    }

    static int[] GenerateArray(int size)
    {
        int[] array = new int[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++)
        {
            array[i] = rand.Next(1000);
        }
        // Замінюємо випадковий елемент на від'ємне число
        array[rand.Next(size)] = -rand.Next(1000);
        return array;
    }
}
