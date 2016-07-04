//Exit Placement Exercise, C# (.NET 4)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace CodeForLife_EvacuationProblem{
    class EvacuationProblem_Challenge2{
        static void Main(string[] args){
            System.IO.StreamReader sr;
            string path = args[0];
            int[] nodeWeigths;
            int[,] routeConnectionAndCapacity;
            int totalNumberOfNodes;
            int maximumNumberOfIterations = 0;
            string newExitPointLocation = \"\";
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
                maximumNumberOfIterations = int.Parse(lines[totalNumberOfNodes + 2]);
                /* YOUR  CODE HERE */
                var indexOfMaxWeight = Array.IndexOf(nodeWeigths, nodeWeigths.Max()) + 1;
                newExitPointLocation = string.Format(\"{0},0\", indexOfMaxWeight);
                Console.WriteLine(\"{0}\", newExitPointLocation);
            }
            catch (Exception e){
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }
    }
}