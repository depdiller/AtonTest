public class Sonny implements Runnable {
    private Duet duet;
    public Sonny(Duet duet) {
        this.duet = duet;
    }


    @Override
    public void run() {
        duet.SonnySing();
    }
}