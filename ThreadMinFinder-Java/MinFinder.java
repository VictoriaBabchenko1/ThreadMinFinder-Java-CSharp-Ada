import java.util.Random;

public class MinFinder {
    private static final Object lock = new Object();
    private static int globalMin = Integer.MAX_VALUE;
    private static int globalIndex = -1;

    public static void main(String[] args) {
        int arraySize = 1_000_000;
        int threadCount = 4;
        int[] array = generateArray(arraySize);

        Thread[] threads = new Thread[threadCount];
        int chunkSize = arraySize / threadCount;

        for (int i = 0; i < threadCount; i++) {
            final int start = i * chunkSize;
            final int end = (i == threadCount - 1) ? arraySize : start + chunkSize;
            threads[i] = new Thread(() -> {
                int localMin = Integer.MAX_VALUE;
                int localIndex = -1;
                for (int j = start; j < end; j++) {
                    if (array[j] < localMin) {
                        localMin = array[j];
                        localIndex = j;
                    }
                }
                synchronized (lock) {
                    if (localMin < globalMin) {
                        globalMin = localMin;
                        globalIndex = localIndex;
                    }
                }
            });
            threads[i].start();
        }

        // Очікування завершення потоків без використання join()
        while (true) {
            boolean allDone = true;
            for (Thread t : threads) {
                if (t.isAlive()) {
                    allDone = false;
                    break;
                }
            }
            if (allDone) break;
        }

        System.out.println("Мінімальне значення: " + globalMin);
        System.out.println("Індекс мінімального значення: " + globalIndex);
    }

    private static int[] generateArray(int size) {
        int[] array = new int[size];
        Random rand = new Random();
        for (int i = 0; i < size; i++) {
            array[i] = rand.nextInt(1000);
        }
        // Замінюємо випадковий елемент на від'ємне число
        array[rand.nextInt(size)] = -rand.nextInt(1000);
        return array;
    }
}
