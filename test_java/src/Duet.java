import java.util.concurrent.locks.*;

public class Duet {

    private final ReentrantLock lock;
    private final Condition condition;

    private class Singer {
        private final String name;
        private boolean readyForDuet;

        public boolean isReady() {
            return readyForDuet;
        }

        public Singer(String name) {
            this.name = name;
            this.readyForDuet = false;
        }

        public void SingTogetherWith(Singer partner) {
            for (String[] lyric : lyrics) {
                try {
                    if (lyric[0].equals(name)) {
                        lock.lock();
                        System.out.printf("%s: %s\n\n", name, lyric[1]);
                        lock.unlock();
                        Thread.sleep(1);
                    }
                    else if (lyric[0].equals("Sonny, Cher")) {
                        lock.lock();
                        readyForDuet = true;
                        while (!partner.isReady())
                            condition.await();
                        System.out.printf("%s: %s\n\n", name, lyric[1]);
                        condition.signal();
                        lock.unlock();
                        Thread.sleep(1);
                    }
                    else {
                        Thread.sleep(1);
                    }
                } catch (InterruptedException e) {
                    System.out.println(e.getMessage());
                }
            }
        }
    }

    private static final String[][] lyrics = {
            {"Cher", "They say we're young and we don't know \nWe won't find out until we grow"},
            {"Sonny", "Well I don't know if all that's true \n'Cause you got me, and baby I got you"},
            {"Sonny", "Babe"},
            {"Sonny, Cher", "I got you babe \nI got you babe"},
            {"Cher", "They say our love won't pay the rent \nBefore it's earned, our money's all been spent"},
            {"Sonny", "I guess that's so, we don't have a pot \nBut at least I'm sure of all the things we got"},
            {"Sonny", "Babe"},
            {"Sonny, Cher", "I got you babe \nI got you babe"},
            {"Sonny", "I got flowers in the spring \nI got you to wear my ring"},
            {"Cher", "And when I'm sad, you're a clown \nAnd if I get scared, you're always around"},
            {"Cher", "So let them say your hair's too long \n'Cause I don't care, with you I can't go wrong"},
            {"Sonny", "Then put your little hand in mine \nThere ain't no hill or mountain we can't climb"},
            {"Sonny", "Babe"},
            {"Sonny, Cher", "I got you babe \nI got you babe"},
            {"Sonny", "I got you to hold my hand"},
            {"Cher", "I got you to understand"},
            {"Sonny", "I got you to walk with me"},
            {"Cher", "I got you to talk with me"},
            {"Sonny", "I got you to kiss goodnight"},
            {"Cher", "I got you to hold me tight"},
            {"Sonny", "I got you, I won't let go"},
            {"Cher", "I got you to love me so"},
            {"Sonny, Cher", "I got you babe \nI got you babe \nI got you babe \nI got you babe \nI got you babe"}
    };
    private final Singer cher;
    private final Singer sonny;

    public Duet() {
        lock = new ReentrantLock();
        condition = lock.newCondition();

        cher = new Singer("Cher");
        sonny = new Singer("Sonny");
    }

    public void CherSing() {
        cher.SingTogetherWith(sonny);
    }

    public void SonnySing() {
        sonny.SingTogetherWith(cher);
    }
}