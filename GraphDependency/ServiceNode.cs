namespace GraphDependency;

/// <summary>
///   Represents a node in the graph for a service.
/// </summary>
public class ServiceNode
{
    /// <summary>
    ///   The service associated with this node.
    /// </summary>
    public IServiceVertex Service { get; }

    /// <summary>
    ///   Indicates whether the service is currently started.
    /// </summary>
    public bool IsStarted { get; private set; }

    /// <summary>
    ///   Creates a node for a service.
    /// </summary>
    public ServiceNode(IServiceVertex service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    ///   Starts the service associated with this node.
    /// </summary>
    public void Start()
    {
        Service.Start();
        IsStarted = true;
    }

    /// <summary>
    ///   Stops the service associated with this node.
    /// </summary>
    public void Stop()
    {
        Service.Stop();
        IsStarted = false;
    }
}