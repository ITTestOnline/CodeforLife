//Evacuation Time Exercise, C# (.NET 4)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CodeForLife_EvacuationProblem{
    class EvacuationProblem_Challenge1{
        static int numberOfPersonsToBeRescued = 0;
        static void Main(string[] args){
            System.IO.StreamReader sr;
            string path = args[0];
            int[] nodeWeigths;
            int[,] routeConnectionAndCapacity;
            int totalNumberOfNodes;
            int finalResult = 0;
            try{
                sr = new System.IO.StreamReader(path);
                List<string> lines = new List<string>();
                String line = null;
                while ((line = sr.ReadLine()) != null)lines.Add(line);
                nodeWeigths = lines[0].Split(',').Select(x => int.Parse(x)).ToArray();
                totalNumberOfNodes = nodeWeigths.Length;
                routeConnectionAndCapacity = new int[totalNumberOfNodes + 1, totalNumberOfNodes + 1];
                for (int row = 0; row < totalNumberOfNodes + 1; row++){
                    var weightsFromLine = lines[row + 1].Split(',').Select(x => int.Parse(x)).ToArray();
                    for (int column = 0; column < weightsFromLine.Length; column++)routeConnectionAndCapacity[row, column] = weightsFromLine[column];
                }
                /* YOUR  CODE HERE */
                finalResult = ResolveEvacuationProblem(nodeWeigths, routeConnectionAndCapacity);
                Console.WriteLine(\"{0}\", finalResult);
            }
            catch (Exception e){
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
        private static int ResolveEvacuationProblem(int[] nodeWeigths, int[,] routeConnectionAndCapacity)
        {
            List<Node> nodes = InitializeNodeStructure(nodeWeigths, routeConnectionAndCapacity);
            var baseNode = MapNodesToTreeStructure(nodes);
            int numberOfIterations = 0;

            ComputeNumberOfPersonsToBeSaved(baseNode);

            while (AreAnyUnsavedPersons())
            {
                numberOfIterations++;
                TransferPersonsBetweenNodes(baseNode);
                numberOfPersonsToBeRescued = 0;
                ComputeNumberOfPersonsToBeSaved(baseNode);
            }

            return numberOfIterations;
        }

        private static bool AreAnyUnsavedPersons()
        {
            return numberOfPersonsToBeRescued > 0;
        }

        private static List<Node> InitializeNodeStructure(int[] nodeWeigths, int[,] routeConnectionAndCapacity)
        {
            List<Node> nodes = new List<Node>(); ;

            nodes.Add(new Node { Capacity = int.MaxValue, PlacesOccupied = 0 });
            for (int i = 0; i < nodeWeigths.Count(); i++)
                nodes.Add(new Node { Capacity = nodeWeigths[i], Index = i + 1, PlacesOccupied = nodeWeigths[i] });

            for (int i = 0; i < nodeWeigths.Count() + 1; i++)
                for (int j = 0; j < nodeWeigths.Count() + 1; j++)
                    if (routeConnectionAndCapacity[i, j] > 0)
                    {
                        var currentNode = nodes.Where(x => x.Index == i).FirstOrDefault();
                        var childNode = nodes.Where(x => x.Index == j).FirstOrDefault();

                        if (childNodeWasNotAssigned(currentNode, childNode))
                            currentNode.ChildNodes.Add(new Tuple<int, Node>(routeConnectionAndCapacity[i, j], childNode));

                    }
            return nodes;
        }

        private static bool childNodeWasNotAssigned(Node currentNode, Node childNode)
        {
            return !currentNode.ChildNodes.Where(x => x.Item2.Index == childNode.Index).Any();
        }

        private static Node MapNodesToTreeStructure(List<Node> nodes)
        {
            var result = new List<Node>();
            var baseNode = nodes.Where(x => x.Index == 0).FirstOrDefault();
            baseNode.StructureLevelNumber = 0;
            BuildStructure(baseNode);
            AssignStructureLevelForNodes(baseNode);

            return baseNode;
        }

        private static void BuildStructure(Node baseNode)
        {
            foreach (var node in baseNode.ChildNodes)
                if (node.Item2.StructureLevelNumber > baseNode.StructureLevelNumber + 1)
                {
                    node.Item2.StructureLevelNumber =
                        node.Item2.StructureLevelNumber > baseNode.StructureLevelNumber + 1 ? baseNode.StructureLevelNumber + 1 : node.Item2.StructureLevelNumber;
                    if (node.Item2.StructureLevelNumber < baseNode.StructureLevelNumber)
                        baseNode.ChildNodes.RemoveAll(x => x.Item2.Index == node.Item2.Index);
                    BuildStructure(node.Item2);
                }
        }

        private static void AssignStructureLevelForNodes(Node baseNode)
        {
            baseNode.ChildNodes.RemoveAll(x => x.Item2.StructureLevelNumber < baseNode.StructureLevelNumber);

            foreach (var node in baseNode.ChildNodes)
            {
                node.Item2.ChildNodes.RemoveAll(x => x.Item2.Index == baseNode.Index);
                node.Item2.MinimumRoutePathToOutside = node.Item1;
                if (baseNode.MinimumRoutePathToOutside < node.Item2.MinimumRoutePathToOutside)
                    node.Item2.MinimumRoutePathToOutside = baseNode.MinimumRoutePathToOutside;

                if (node.Item2.StructureLevelNumber != baseNode.StructureLevelNumber)
                    AssignStructureLevelForNodes(node.Item2);
            }
        }

        private static void TransferPersonsBetweenNodes(Node baseNode)
        {
            foreach (var node in baseNode.ChildNodes)
            {
                var personsToBeTransfered = 0;
                if ((baseNode.Capacity > baseNode.PlacesOccupied) &&
                    ((node.Item2.StructureLevelNumber != baseNode.StructureLevelNumber) ||
                    ((node.Item2.MinimumRoutePathToOutside < baseNode.MinimumRoutePathToOutside) && node.Item2.MinimumRoutePathToOutside < node.Item1)))
                {
                    personsToBeTransfered = baseNode.PlacesFree > node.Item2.Capacity ? node.Item2.Capacity : baseNode.PlacesFree;
                    personsToBeTransfered = personsToBeTransfered <= node.Item1 ? personsToBeTransfered : node.Item1;
                    personsToBeTransfered = personsToBeTransfered < node.Item2.PlacesOccupied ? personsToBeTransfered : node.Item2.PlacesOccupied;

                    baseNode.PlacesOccupied += personsToBeTransfered;
                    node.Item2.PlacesOccupied -= personsToBeTransfered;
                }
                TransferPersonsBetweenNodes(node.Item2);
            }
        }

        private static void ComputeNumberOfPersonsToBeSaved(Node baseNode)
        {
            foreach (var node in baseNode.ChildNodes)
            {
                numberOfPersonsToBeRescued += node.Item2.PlacesOccupied;
                ComputeNumberOfPersonsToBeSaved(node.Item2);
            }
        }
    }
    public class Node
    {
        public int Index { get; set; }
        public int Capacity { get; set; }
        public List<Tuple<int, Node>> ChildNodes { get; set; }
        public int MinimumRoutePathToOutside { get; set; }
        public int StructureLevelNumber { get; set; }
        public int PlacesOccupied { get; set; }
        public int PlacesFree
        {
            get { return Capacity - PlacesOccupied; }
        }

        public int NumberOfIterationsToOutside
        {
            get
            {
                return StructureLevelNumber + PlacesOccupied / MinimumRoutePathToOutside;
            }
        }

        public Node()
        {
            Index = 0;
            Capacity = 0;
            ChildNodes = new List<Tuple<int, Node>>();
            StructureLevelNumber = int.MaxValue;
            PlacesOccupied = Capacity;
            MinimumRoutePathToOutside = int.MaxValue;
        }
    }
}