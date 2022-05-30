namespace Train;

static class Program
{
    public static void Main(string[] args)
    {
        for (int i = 0; i < 100; i++)
        {
            var train = new RandomTrain();
            Console.Out.WriteLine($"Result of algorithm: real length = " +
                                  $"{train.RealLength}, found = {train.FindLength()}");
        }
    }
}

public class Wagon
{
    private bool _light;

    public bool Light
    {
        get => _light;
    }

    public Wagon(bool turnOnLight)
    {
        _light = turnOnLight;
    }

    public void TurnOnLight()
    {
        _light = true;
    }

    public void TurnOffLight()
    {
        _light = false;
    }
}

public class RandomTrain
{
    private readonly LinkedList<Wagon> _train;
    private readonly int _realLength;
    private LinkedListNode<Wagon> _currentWagon;

    public LinkedList<Wagon> TrainWagons
    {
        get => _train;
    }

    public LinkedListNode<Wagon> CurrentWagon
    {
        get => _currentWagon;
    }

    public int RealLength
    {
        get => _realLength;
    }

    public RandomTrain()
    {
        Random random = new Random();
        int length = random.Next(2, 10000);
        _realLength = length;
        _train = new LinkedList<Wagon>();
        for (var i = 0; i < length; ++i)
        {
            _train.AddLast(new Wagon(random.Next(100) > 20));
        }

        _currentWagon = _train.First ?? throw new InvalidOperationException();
    }

    public void TurnOnLightInCurrentWagon()
    {
        _currentWagon.Value.TurnOnLight();
    }

    public void TurnOffLightInCurrentWagon()
    {
        _currentWagon.Value.TurnOffLight();
    }

    public bool IsLightInWagon()
    {
        return _currentWagon.Value.Light;
    }

    public void GoNextWagon()
    {
        _currentWagon = _currentWagon.Next ?? _currentWagon.List.First;
    }

    public void GoToPreviousWagon()
    {
        _currentWagon = _currentWagon.Previous ?? _currentWagon.List.Last;
    }

    public int FindLength()
    {
        int count;
        do
        {
            count = 0;
            TurnOnLightInCurrentWagon();
            GoNextWagon();
            while (!IsLightInWagon())
            {
                ++count;
                GoNextWagon();
            }

            TurnOffLightInCurrentWagon();
            for (var i = 0; i <= count; ++i)
                GoToPreviousWagon();
        } while (IsLightInWagon());

        return count + 1;
    }
}