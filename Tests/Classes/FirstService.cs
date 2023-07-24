using GraphDependency;

namespace Tests.Classes;

public class FirstService : IServiceVertex
{
    public bool IsStarted { get; private set; }
    public DateTime StartedTime { get; private set; }
    public DateTime StopTime { get; private set; }

    public void Start()
    {
        StartedTime = DateTime.UtcNow;
        IsStarted = true;
    }

    public void Stop()
    {
        IsStarted = false;
        StopTime = DateTime.UtcNow;
    }

    public Action<IServiceVertex> ServiceCrashed { get; set; }
}