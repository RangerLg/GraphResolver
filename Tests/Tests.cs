using GraphDependency;

using Tests.Classes;

namespace Tests;

public class Tests
{
    [Test]
    public void TestExampleUsage()
    {
        // Create graph
        var graph = new Graph<ServiceNode>();

        // Init services.
        var firstService = new FirstService();
        var secondService = new SecondService();

        // Add services to graph
        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));

        graph.AddEdge(firstNode, secondNode);

        var graphController = new ServiceGraphController(graph);
        // Start service.
        graphController.StartService(graph.FindNode(x => x.Service == firstService).Service);

        // Stop service.
        graphController.StopService(secondService);
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |                       |
    //         v                       v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void FirstGraph_ServiceStart()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(thirdService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(!fourthService.IsStarted);
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |                       |
    //         v                       v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void FirstGraph_CheckOrder()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.StartedTime, Is.GreaterThan(secondService.StartedTime));
            Assert.That(thirdService.StartedTime, Is.GreaterThan(firstService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(thirdService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(secondService.StartedTime));
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |                       |
    //         v                       v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void StartSetServices_CheckOrder()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode});

        var graphController = new ServiceGraphController(graph);
        var setServices = new List<IServiceVertex> { fourthService, secondService };
        graphController.StartService(setServices);

        Assert.Multiple(() => {
            Assert.That(firstService.StartedTime, Is.GreaterThan(secondService.StartedTime));
            Assert.That(thirdService.StartedTime, Is.GreaterThan(firstService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(thirdService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(secondService.StartedTime));
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |        | --------     |
    //         |                 |     |
    //         v                 v    v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void SecondGraph_StartService()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode, firstNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(fourthService.IsStarted);
        });

        Assert.Multiple(() => {
            Assert.That(firstService.StartedTime, Is.GreaterThan(secondService.StartedTime));
            Assert.That(thirdService.StartedTime, Is.GreaterThan(firstService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(thirdService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(secondService.StartedTime));
            Assert.That(fourthService.StartedTime, Is.GreaterThan(firstService.StartedTime));
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |        | --------     |
    //         |                 |     |
    //         v                 v    v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void SecondGraph_StopService()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode, firstNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(fourthService.IsStarted);
        });

        graphController.StopService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(!fourthService.IsStarted);
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |        | --------     |
    //         |                 |     |
    //         v                 v    v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void SecondGraph_StopNecessaryServices()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode, firstNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(fourthService.IsStarted);
        });

        graphController.StopService(secondService);

        Assert.Multiple(() => {
            Assert.That(!firstService.IsStarted);
            Assert.That(!secondService.IsStarted);
            Assert.That(!thirdService.IsStarted);
            Assert.That(!fourthService.IsStarted);
        });

        Assert.Multiple(() => {
            Assert.That(firstService.StartedTime, Is.LessThan(secondService.StopTime));
            Assert.That(thirdService.StartedTime, Is.LessThan(firstService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(thirdService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(secondService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(firstService.StopTime));
        });
    }

    
    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |        | --------     |
    //         |                 |     |
    //         v                 v    v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void StopSetOfNecessaryServices()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode, firstNode});

        var graphController = new ServiceGraphController(graph);
        var setServices = new List<IServiceVertex> { fourthService, secondService };
        graphController.StartService(setServices);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(fourthService.IsStarted);
        });

        graphController.StopService(setServices);

        Assert.Multiple(() => {
            Assert.That(!firstService.IsStarted);
            Assert.That(!secondService.IsStarted);
            Assert.That(!thirdService.IsStarted);
            Assert.That(!fourthService.IsStarted);
        });

        Assert.Multiple(() => {
            Assert.That(firstService.StartedTime, Is.LessThan(secondService.StopTime));
            Assert.That(thirdService.StartedTime, Is.LessThan(firstService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(thirdService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(secondService.StopTime));
            Assert.That(fourthService.StartedTime, Is.LessThan(firstService.StopTime));
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +--------|--------+
    //         |        | --------     |
    //         |                 |     |
    //         v                 v     v
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void SecondGraph_ServiceIsCrushed()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(thirdNode, firstNode);
        graph.AddEdge(fourthNode, new List<ServiceNode>{secondNode, thirdNode, firstNode});

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(fourthService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(thirdService.IsStarted);
            Assert.That(fourthService.IsStarted);
        });

        secondService.ProcessCrashedService();

        Assert.Multiple(() => {
            Assert.That(!firstService.IsStarted);
            Assert.That(!secondService.IsStarted);
            Assert.That(!thirdService.IsStarted);
            Assert.That(!fourthService.IsStarted);
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |---->|  SecondService  |
    //+--------|--------+     +-----------------+
    //         |                       ^
    //         |                       |
    //         v                       |
    //+-----------------+              |
    //| SecondServiceV2 |---------------
    //+-----------------+
    [Test]
    public void ThirdGraph_AddedSameGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var secondServiceV2 = new SecondService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(secondServiceV2));

        graph.AddEdge(secondNode, firstNode);
        graph.AddEdge(thirdNode, secondNode);

        var graphController = new ServiceGraphController(graph);
        graphController.StartService(secondService);

        Assert.Multiple(() => {
            Assert.That(firstService.IsStarted);
            Assert.That(secondService.IsStarted);
            Assert.That(!secondServiceV2.IsStarted);
        });
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +-----------------+
    //         |        ^   
    //         |        |- --------
    //         v                 |
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void StopService_CircleReference()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, new List<ServiceNode>{secondNode, fourthNode});
        graph.AddEdge(thirdNode, firstNode);

        var ex = Assert.Throws<Exception>(() => graph.AddEdge(fourthNode, thirdNode));

        Assert.That(ex?.Message, Is.EqualTo("Circular reference found. Cannot add the edge."));
    }

    //+-----------------+     +-----------------+
    //|   FirstService  |<----|  SecondService  |
    //+--------|--------+     +-----------------+
    //         |                       ^
    //         v                       |
    //+-----------------+      +-----------------+
    //|   ThirdService  |----->|  FourthService  |
    //+-----------------+      +-----------------+
    [Test]
    public void ThirdGraph_CircleReference()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var thirdService = new ThirdService();
        var fourthService = new FourthService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));
        var secondNode = graph.AddNode(new ServiceNode(secondService));
        var thirdNode = graph.AddNode(new ServiceNode(thirdService));
        var fourthNode = graph.AddNode(new ServiceNode(fourthService));

        graph.AddEdge(firstNode, secondNode);
        graph.AddEdge(secondNode, fourthNode);
        graph.AddEdge(thirdNode, firstNode);

        var ex = Assert.Throws<Exception>(() => graph.AddEdge(fourthNode, new List<ServiceNode>{thirdNode, firstNode}));

        Assert.That(ex?.Message, Is.EqualTo("Circular reference found. Cannot add the edge."));
    }

    //+-----------------+     
    //|   FirstService  |<----
    //+--------|--------+    |
    //         |-------------|

    [Test]
    public void ThirdGraph_CircleReferenceOneGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));

        var ex = Assert.Throws<Exception>(() => graph.AddEdge(firstNode, firstNode));

        Assert.That(ex?.Message, Is.EqualTo("Circular reference found. Cannot add the edge."));
    }

    //+-----------------+     
    //|   FirstService  |<----
    //+--------|--------+    |
    //         |-------------|
    [Test]
    public void ThirdGraph_StopServiceCircleReferenceOneGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();

        var firstNode = graph.AddNode(new ServiceNode(firstService));

        var ex = Assert.Throws<Exception>(() => graph.AddEdge(firstNode, firstNode));

        Assert.That(ex?.Message, Is.EqualTo("Circular reference found. Cannot add the edge."));
    }

    [Test]
    public void AlreadyAdded()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var fourthService = new FourthService();

        var node = new ServiceNode(secondService);
        graph.AddNode(node);

        var ex = Assert.Throws<ArgumentException>(() => graph.AddNode(node));

        Assert.That(ex?.Message, Contains.Substring("Node already exists in the graph."));
    }

    [Test]
    public void ServiceDoesntAddedToGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var fourthService = new FourthService();

        graph.AddNode(new ServiceNode(firstService));
        graph.AddNode(new ServiceNode(secondService));


        var graphController = new ServiceGraphController(graph);
        var ex = Assert.Throws<ArgumentException>(() => graphController.StartService(fourthService));

        var type = fourthService.GetType();

        Assert.That(ex.Message, Contains.Substring("Service wasn't added to the graph"));
    }

    [Test]
    public void StopServiceDoesntAddedToGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var secondService = new SecondService();
        var fourthService = new FourthService();

        graph.AddNode(new ServiceNode(firstService));
        graph.AddNode(new ServiceNode(secondService));

        var graphController = new ServiceGraphController(graph);
        var ex = Assert.Throws<ArgumentException>(() => graphController.StopService(fourthService));

        Assert.That(ex.Message, Contains.Substring("Service wasn't added to the graph"));
    }

    [Test]
    public void NullService()
    {
        var graph = new Graph<ServiceNode>();

        var ex = Assert.Throws<ArgumentNullException>(() => graph.AddNode(null));

        Assert.That(ex?.ParamName, Is.EqualTo("node"));
    }

    [Test]
    public void NullNecessaryService()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();

        graph.AddNode(new ServiceNode(firstService));


        var ex = Assert.Throws<ArgumentNullException>(() => graph.AddNode(null));

        Assert.That(ex?.ParamName, Is.EqualTo("node"));
    }

    [Test]
    public void StopServiceNull()
    {
        var graph = new Graph<ServiceNode>();

        var graphController = new ServiceGraphController(graph);
        IServiceVertex nullService = null;
        var ex = Assert.Throws<ArgumentNullException>(() => graphController.StopService(nullService));

        Assert.That(ex?.ParamName, Is.EqualTo("service"));
    }

    [Test]
    public void StartServiceNull()
    {
        var graph = new Graph<ServiceNode>();

        var graphController = new ServiceGraphController(graph);
        IServiceVertex nullService = null;
        var ex = Assert.Throws<ArgumentNullException>(() => graphController.StartService(nullService));

        Assert.That(ex?.ParamName, Is.EqualTo("service"));
    }

    [Test]
    public void StopSetOfServiceNull()
    {
        var graph = new Graph<ServiceNode>();

        var graphController = new ServiceGraphController(graph);
        List<IServiceVertex> nullService = null;
        var ex = Assert.Throws<ArgumentNullException>(() => graphController.StopService(nullService));

        Assert.That(ex?.ParamName, Is.EqualTo("services"));
    }

    [Test]
    public void StartSetOfServiceNull()
    {
        var graph = new Graph<ServiceNode>();

        var graphController = new ServiceGraphController(graph);
        List<IServiceVertex> nullService = null;
        var ex = Assert.Throws<ArgumentNullException>(() => graphController.StartService(nullService));

        Assert.That(ex?.ParamName, Is.EqualTo("services"));
    }

    [Test]
    public void NullGraph()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ServiceGraphController(null));

        Assert.That(ex?.ParamName, Is.EqualTo("graph"));
    }

    [Test]
    public void NullServiceNode()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => new ServiceNode(null));

        Assert.That(ex?.ParamName, Is.EqualTo("service"));
    }

    [Test]
    public void NodeWasntAddedToGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var firstNode = new ServiceNode(firstService);
        var secondNode = new ServiceNode(firstService);
        graph.AddNode(firstNode);

        var ex = Assert.Throws<ArgumentException>(() => graph.AddEdge(firstNode, secondNode));

        Assert.That(ex?.Message, Is.EqualTo("Both nodes must be added to the graph before adding an edge."));
    }

    [Test]
    public void NodesWasntAddedToGraph()
    {
        var graph = new Graph<ServiceNode>();
        var firstService = new FirstService();
        var firstNode = new ServiceNode(firstService);
        var secondNode = new ServiceNode(firstService);
        var thirdNode = new ServiceNode(firstService);
        graph.AddNode(firstNode);

        var ex = Assert.Throws<ArgumentException>(() => graph.AddEdge(firstNode, new List<ServiceNode>{secondNode, thirdNode}));

        Assert.That(ex?.Message, Is.EqualTo("Both nodes must be added to the graph before adding an edge."));
    }
}