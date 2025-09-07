import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        int defaultDim = 10_000_000;
        int defaultThreadNum = 10;

        Scanner scanner = new Scanner(System.in);

        System.out.print("Введіть розмір масиву (натисніть Enter для значення за замовчуванням " 
                          + defaultDim + "): ");
        String dimInput = scanner.nextLine();

        System.out.print("Введіть кількість потоків (натисніть Enter для значення за замовчуванням " 
                          + defaultThreadNum + "): ");
        String threadInput = scanner.nextLine();

        int dim = parseOrDefault(dimInput, defaultDim);
        int threadNum = parseOrDefault(threadInput, defaultThreadNum);


        ArrClass arrClass = new ArrClass(dim, threadNum);
        arrClass.findMinWithThreads();
    }

    private static int parseOrDefault(String input, int defaultValue) {
        try {
            int value = Integer.parseInt(input.trim());
            if (value > 0) {
                return value;
            } else {
                System.out.println("Некоректне значення (" + input + "), використано значення за замовчуванням " + defaultValue);
            }
        } catch (Exception e) {
            if (!input.isEmpty()) {
                System.out.println("Некоректне значення (" + input + "), використано значення за замовчуванням " + defaultValue);
            }
        }
        return defaultValue;
    }
}