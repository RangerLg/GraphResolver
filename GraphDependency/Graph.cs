namespace GraphDependency
{
    /// <summary>
    ///   Represents a dependency graph that consists of nodes and edges (dependencies) between them.
    /// </summary>
    /// <typeparam name="T">The type of the nodes in the graph.</typeparam>
    public class Graph<T> where T : notnull
    {
        /// <summary>
        /// Gets the list of nodes in the graph.
        /// </summary>
        public List<T> Nodes { get; }= new();

        /// <summary>
        /// Gets the dictionary of node edges in the graph.
        /// </summary>
        public Dictionary<T, List<T>> NodeEdges { get; }= new();

        /// <summary>
        ///   Adds a node to the graph.
        /// </summary>
        /// <param name="node">The node to be added to the graph.</param>
        /// <exception cref="ArgumentNullException">Thrown when the provided node is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the node is already added to the graph.</exception>
        public T AddNode(T node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (Nodes.Contains(node))
                throw new ArgumentException("Node already exists in the graph.", nameof(node));

            Nodes.Add(node);

            return node;
        }

        /// <summary>
        ///   Adds an edge between two nodes in the graph.
        /// </summary>
        /// <param name="from">The source node from which the edge will be directed.</param>
        /// <param name="to">The target node to which the edge will be directed.</param>
        /// <exception cref="ArgumentException">Thrown when either of the nodes is not added to the graph.</exception>
        public void AddEdge(T from, T to)
        {
            if (!Nodes.Contains(from) || !Nodes.Contains(to))
                throw new ArgumentException("Both nodes must be added to the graph before adding an edge.");

            var edges = new List<T>
            {
                to
            };
            NodeEdges.Add(from, edges);

            var visited = new HashSet<T>();
            var path = new Stack<T>();

            if (ValidateCircularReference(from, visited, path))
                throw new Exception("Circular reference found. Cannot add the edge.");
        }

        /// <summary>
        /// Adds an edge between a node and a list of nodes in the graph.
        /// </summary>
        /// <param name="from">The source node from which the edge will be directed.</param>
        /// <param name="to">The list of target nodes to which the edge will be directed.</param>
        /// <exception cref="ArgumentException">Thrown when either of the nodes is not added to the graph.</exception>
        public void AddEdge(T from, List<T> to)
        {
            if (!Nodes.Contains(from) || to.Any(x => !Nodes.Contains(x)))
                throw new ArgumentException("Both nodes must be added to the graph before adding an edge.");

            NodeEdges.Add(from, to);

            var visited = new HashSet<T>();
            var path = new Stack<T>();

            if (ValidateCircularReference(from, visited, path))
                throw new Exception("Circular reference found. Cannot add the edge.");
        }

        /// <summary>
        /// Finds a node in the graph that matches the specified condition.
        /// </summary>
        /// <param name="condition">The condition to match the node.</param>
        /// <returns>The node in the graph that matches the condition.</returns>
        /// <exception cref="Exception">Thrown when the node cannot be found.</exception>
        public T FindNode(Predicate<T> condition)
        {
            return Nodes.Find(condition) ?? throw new Exception("Node cannot be found.");
        }

        private bool ValidateCircularReference(T node, ISet<T> visited, Stack<T> path)
        {
            visited.Add(node);
            path.Push(node);

            foreach (var dependentVertex in NodeEdges.GetValueOrDefault(node, new List<T>()))
            {
                if (!visited.Contains(dependentVertex))
                {
                    if (ValidateCircularReference(dependentVertex, visited, path)) return true;
                }
                else if (path.Contains(dependentVertex)) return true;
            }

            path.Pop();
            return false;
        }
    }
}
