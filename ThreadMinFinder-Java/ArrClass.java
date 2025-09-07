import java.util.Random;

public class ArrClass {
    private final int dim;
    private final int threadNum;
    public final int[] arr;
    private int globalMin;
    private int globalMinIndex;
    private int threadCount = 0;

    public ArrClass(int dim, int threadNum) {
        this.dim = dim;
        this.threadNum = threadNum;
        this.arr = new int[dim];
        generateArray();
    }

    private void generateArray() {
        Random rand = new Random();
        for (int i = 0; i < dim; i++) {
            arr[i] = rand.nextInt(10000);
        }
        int randomNegativeIndex = rand.nextInt(dim);
        arr[randomNegativeIndex] = -(rand.nextInt(99) + 1);
    }

    synchronized public void updateMin(int value, int index) {
        if (threadCount == 0 || value < globalMin) {
            globalMin = value;
            globalMinIndex = index;
        }
        threadCount++;
        if (threadCount == threadNum) {
            notify();
        }
    }

    synchronized private void waitForThreads() {
        while (threadCount < threadNum) {
            try {
                wait();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    public void findMinWithThreads() {
        int partSize = dim / threadNum;
        MinThread[] threads = new MinThread[threadNum];

        for (int i = 0; i < threadNum; i++) {
            int start = i * partSize;
            int end = (i == threadNum - 1) ? dim : start + partSize;
            threads[i] = new MinThread(start, end, this, i);
            threads[i].start();
        }

        waitForThreads();
        System.out.println("Global min: " + globalMin + ", index: " + globalMinIndex);
    }
}