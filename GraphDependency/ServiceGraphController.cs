namespace GraphDependency
{
    /// <summary>
    ///   Represents a controller for managing a service dependency graph.
    /// </summary>
    public class ServiceGraphController
    {
        private readonly Graph<ServiceNode> graph;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ServiceGraphController"/> class.
        /// </summary>
        /// <param name="graph">The graph to be controlled.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided graph is null.</exception>
        public ServiceGraphController(Graph<ServiceNode> graph)
        {
            if (graph == null)
                throw new ArgumentNullException(nameof(graph));

            this.graph = graph;
            graph.Nodes.ForEach(x => x.Service.ServiceCrashed = OnCrashed);
        }

        /// <summary>
        ///   Starts a set of services.
        /// </summary>
        /// <param name="services">The collection of services to start.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided services collection is null.</exception>
        public void StartService(IEnumerable<IServiceVertex> services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var service in services)
            {
                StartService(service);
            }
        }

        /// <summary>
        ///   Stops a set of services.
        /// </summary>
        /// <param name="services">The collection of services to stop.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided services collection is null.</exception>
        public void StopService(IEnumerable<IServiceVertex> services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var service in services)
            {
                StopService(service);
            }
        }

        /// <summary>
        ///   Starts a service and its dependencies.
        /// </summary>
        /// <param name="service">The service to start.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided service is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the service wasn't added to the graph.</exception>
        public void StartService(IServiceVertex service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (graph.Nodes.All(x => x.Service != service))
                throw new ArgumentException("Service wasn't added to the graph", nameof(service));

            var node = graph.Nodes.Find(x => x.Service == service);

            StartServiceRecursively(node);
        }

        /// <summary>
        ///   Stops a service and its dependencies.
        /// </summary>
        /// <param name="service">The service to stop.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided service is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the service wasn't added to the graph.</exception>
        public void StopService(IServiceVertex service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            if (graph.Nodes.All(x => x.Service != service))
                throw new ArgumentException("Service wasn't added to the graph", nameof(service));

            var node = graph.Nodes.Find(x => x.Service == service);

            StopServiceRecursively(node);
        }

        private void StopServiceRecursively(ServiceNode node)
        {
            var necessaryNodes = graph.NodeEdges
                                      .Where(x => x.Value.Contains(node) && x.Key.IsStarted)
                                      .Select(x => x.Key);

            foreach (var necessaryNode in necessaryNodes)
            {
                StopServiceRecursively(necessaryNode);
            }

            node.Stop();
        }

        private void StartServiceRecursively(ServiceNode node)
        {
            if (node.IsStarted) return;

            foreach (var necessaryNode in graph.NodeEdges.GetValueOrDefault(node, new List<ServiceNode>()))
            {
                StartServiceRecursively(necessaryNode);
            }

            node.Start();
        }

        private void OnCrashed(IServiceVertex service)
        {
            StopService(service);
        }
    }
}
