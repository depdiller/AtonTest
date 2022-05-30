public class Cher implements Runnable {
    private Duet duet;
    public Cher(Duet duet) {
        this.duet = duet;
    }

    @Override
    public void run() {
        duet.CherSing();
    }
}