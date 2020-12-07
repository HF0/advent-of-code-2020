using QuikGraph;
using QuikGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode7
{
    class Program
    {
        static void Main(string[] args)
        {
            const string color = "shiny gold";
            IEnumerable<(string, IEnumerable<(int, string)>)> parsedRules = File
                .ReadAllLines("input.txt")
                .Select(e => ParseRule(e));

            // srcColor, colors that can contain srcColor
            Dictionary<string, List<string>> colorContainerRules = CreateColorContainerRules(parsedRules);
            int solution1a = CountCanContainColor(color, colorContainerRules);
            int solution1b = Problem1UsingGraphLibrary(color, parsedRules);
            Console.WriteLine($"Solution 1: {solution1a} {solution1b}");

            // srcColor, colors and quantity that canb e inside this color
            Dictionary<string, IEnumerable<(int quantity, string color)>> colorInsideRules = CreateColorInsideRules(parsedRules);
            int solution2a = BagsRequiredInsideRecursive(color, colorInsideRules);
            int solution2b = BagsRequiredInsideSequential(color, colorInsideRules);
            int solution2c = Problem2UsingGraphLibrary(color, parsedRules);
            Console.WriteLine($"Solution 2: {solution2a} {solution2b} {solution2c}");
        }

        private static int Problem2UsingGraphLibrary(string color, IEnumerable<(string, IEnumerable<(int, string)>)> parsedRules)
        {
            var rules = parsedRules.SelectMany(e => e.Item2.Select(v => (e.Item1, v.Item2, v.Item1)))
                .Select(e => (new ColorEdge(e.Item1, e.Item2), e.Item3));
            var graph = rules.Select(e => e.Item1).ToAdjacencyGraph<string, ColorEdge>();
            Dictionary<ColorEdge, int> costDictionary = rules.ToDictionary(k => k.Item1, k => k.Item2);
            Dictionary<string, int> bagsInside = new Dictionary<string, int>();
            DepthFirstSearchFinishVertexAction(color, graph, v => {
                IEnumerable<ColorEdge> children = graph.Edges.Where(e => e.Source.Equals(v));
                int bagsInsideV = children
                    .Select(edge => costDictionary[edge] * (bagsInside[edge.Target] + 1))
                    .Sum();
                bagsInside[v] = bagsInsideV;
            });
            return bagsInside[color];
        }

        private static int Problem1UsingGraphLibrary(string color, IEnumerable<(string, IEnumerable<(int, string)>)> parsedRules)
        {
            var edges = parsedRules
                .SelectMany(e => e.Item2.Select(v => (v.Item2, e.Item1)))
                .Select(e => new Edge<string>(e.Item1, e.Item2));
            AdjacencyGraph<string, Edge<string>> graph = edges.ToAdjacencyGraph<string, Edge<string>>();
            int nodesVisited = 0;
            DepthFirstSearchFinishVertexAction(color, graph, _ => nodesVisited++);
            return nodesVisited - 1;
        }

        private static void DepthFirstSearchFinishVertexAction<TVertex, TEdge>(TVertex rootNode, AdjacencyGraph<TVertex, TEdge> graph, VertexAction<TVertex> action)
            where TEdge : IEdge<TVertex>
        {
            var dfs = new DepthFirstSearchAlgorithm<TVertex, TEdge>(graph);
            dfs.SetRootVertex(rootNode);
            dfs.RootVertexChanged += (s, a) => dfs.Abort();
            dfs.FinishVertex += action;
            dfs.Compute();
        }

        // Recursive solution
        private static int BagsRequiredInsideRecursive(string color, Dictionary<string, IEnumerable<(int quantity, string color)>> rules)
        {
            var insideBags = rules[color];
            return insideBags
                .Select(e => e.quantity * (BagsRequiredInsideRecursive(e.color, rules) + 1))
                .Sum();
        }

        // Sequential solution
        private static int BagsRequiredInsideSequential(string color, Dictionary<string, IEnumerable<(int quantity, string color)>> rules)
        {
            Dictionary<string, int> bagInsideCalculated = new Dictionary<string, int>();
            Stack<string> toCalculate = new Stack<string>();
            toCalculate.Push(color);
            while (toCalculate.Count != 0)
            {
                string bag = toCalculate.Peek();
                if (bagInsideCalculated.ContainsKey(bag))
                {
                    toCalculate.Pop();
                    continue;
                }
                IEnumerable<(int quantity, string color)> r = rules[bag];
                var valueMissing = r.Where(e => !bagInsideCalculated.ContainsKey(e.color));
                if (valueMissing.Count() == 0)
                {
                    int solution = r.Select(e => e.quantity * (bagInsideCalculated[e.color] + 1)).Sum();
                    bagInsideCalculated.Add(bag, solution);
                    toCalculate.Pop();
                }
                else
                {
                    foreach (var v in valueMissing)
                    {
                        toCalculate.Push(v.color);
                    }
                }
            }
            return bagInsideCalculated[color];
        }

        private static Dictionary<string, IEnumerable<(int, string)>> CreateColorInsideRules(IEnumerable<(string, IEnumerable<(int, string)>)> rules)
            => rules.ToDictionary(e => e.Item1, e => e.Item2);

        private static int CountCanContainColor(string color, Dictionary<string, List<string>> rules)
        {
            ISet<string> colorVisited = new HashSet<string>();
            ISet<string> colorToVisit = new HashSet<string>();
            colorToVisit.Add(color);
            while (colorToVisit.Count() != 0)
            {
                var c = colorToVisit.First();
                colorToVisit.Remove(c);
                colorVisited.Add(c);

                if (rules.ContainsKey(c))
                {
                    rules[c].ForEach(e =>
                    {
                        colorToVisit.Add(e);
                    });
                }
            }

            colorVisited.Remove(color);
            return colorVisited.Count();
        }

        private static Dictionary<string, List<string>> CreateColorContainerRules(IEnumerable<(string, IEnumerable<(int, string)>)> rules)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();
            foreach ((string containerColor, IEnumerable<(int quantity, string color)> canBeInsideColors) in rules)
            {
                foreach (var insideColor in canBeInsideColors)
                {
                    string color = insideColor.color;
                    if (!result.ContainsKey(insideColor.color))
                    {
                        result.Add(color, new List<string>());
                    }
                    result[color].Add(containerColor);
                }
            }
            return result;
        }

        private static Dictionary<string, List<string>> CreateRulesContainer2(string[] lines)
        {
            var rulesAdvance = lines.Select(e => ParseRule(e))
                .SelectMany(e => Reverse(e))
                .GroupBy(e => e.Item3)
                .ToDictionary(e => e.Key, e => e.Select(e => e.Item1).ToList());
            return rulesAdvance;
        }

        private static IEnumerable<(string, int, string)> Reverse((string, IEnumerable<(int, string)>) e)
        {
            return e.Item2.Select(v => (e.Item1, v.Item1, v.Item2));
        }

        private static (string, IEnumerable<(int, string)>) ParseRule(string e)
        {
            var parts = e.Split(" bags contain ");
            string srcColor = parts[0];
            IEnumerable<(int, string)> rules = parts[1]
                .Split(new char[] { ',', '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !e.Equals("no other bags"))
                .Select(e => e[..e.LastIndexOf(" ")])
                .Select(e => ToTuple(e));
            return (srcColor, rules);
        }

        private static (int, string) ToTuple(string e)
        {
            int index = e.IndexOf(" ");
            return (int.Parse(e[0..index]), e[(index + 1)..]);
        }
    }
}
