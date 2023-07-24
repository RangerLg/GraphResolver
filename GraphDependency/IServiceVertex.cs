namespace GraphDependency;

/// <summary>
///   Represents services that can be used as vertices in a graph.
/// </summary>
public interface IServiceVertex
{
    /// <summary>
    ///   Starts the service.
    /// </summary>
    void Start();

    /// <summary>
    ///   Stops the service.
    /// </summary>
    void Stop();

    /// <summary>
    ///   Event handler for notifying about a service crash.
    /// </summary>
    Action<IServiceVertex> ServiceCrashed { set; }
}