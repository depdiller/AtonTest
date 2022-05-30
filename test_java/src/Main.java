
public class Main {
    public static void main(String[] args) {
        Duet duet = new Duet();
        Cher cher = new Cher(duet);
        Sonny sonny = new Sonny(duet);

        new Thread(cher).start();
        new Thread(sonny).start();
    }
}