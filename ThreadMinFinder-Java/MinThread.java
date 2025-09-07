public class MinThread extends Thread {
    private final int start;
    private final int end;
    private final ArrClass arrClass;
    private final int threadId;

    public MinThread(int start, int end, ArrClass arrClass, int threadId) {
        this.start = start;
        this.end = end;
        this.arrClass = arrClass;
        this.threadId = threadId;
    }

    @Override
    public void run() {
        int[] arr = arrClass.arr;
        int localMin = arr[start];
        int localMinIndex = start;

        for (int i = start + 1; i < end; i++) {
            if (arr[i] < localMin) {
                localMin = arr[i];
                localMinIndex = i;
            }
        }

        System.out.println("Thread #" + threadId + " processed range [" + start + ", " + end + "], local minimum: " + localMin + ", index: " + localMinIndex);

        arrClass.updateMin(localMin, localMinIndex);
    }
}