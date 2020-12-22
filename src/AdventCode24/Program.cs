using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode24
{
    class Program
    {
        static void Main(string[] args)
        {
            var tiles = File.ReadAllLines("input.txt")
                .Select(e => ParseInstructions(e))
                .ToList();
            var tileStatus = tiles
                .GroupBy(e => e)
                .Select(g => (tile: g.Key, isBlack: g.Count() % 2 == 1));
            int solution1 = GetNumBlackTiles(tileStatus);
            Console.WriteLine($"Solution 1: {solution1}");

            Dictionary<(int x, int y), bool> updatedTiles = tileStatus
                .Where(e => e.isBlack)
                .ToDictionary(k => k.tile, k => k.isBlack);
            for (int i = 0; i < 100; i++)
            {
                UpdateTiles(updatedTiles);
            }
            int solution2 = GetNumBlackTiles(updatedTiles);
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static void UpdateTiles(Dictionary<(int x, int y), bool> tiles)
        {
            Dictionary<(int x, int y), bool> previousTiles = new Dictionary<(int x, int y), bool>(tiles);
            var blackTiles = previousTiles.Where(e => e.Value).Select(e => e.Key);
            var blackToWhite = blackTiles.Where(e =>
                {
                    var blacks = AdjacentBlackTiles(e, previousTiles);
                    return blacks == 0 || blacks > 2;
                });
            var possibleWhiteTiles = blackTiles
                .SelectMany(e => Adjacent(e))
                .Where(e => !IsBlack(e, previousTiles));
            var whiteToBlack = possibleWhiteTiles
                .Where(e => AdjacentBlackTiles(e, previousTiles) == 2);

            foreach (var t in blackToWhite)
            {
                tiles.Remove(t);
            }
            foreach (var t in whiteToBlack)
            {
                tiles[t] = true;
            }
        }

        private static int GetNumBlackTiles(Dictionary<(int x, int y), bool> tiles)
            => tiles.Where(e => e.Value).Count();

        private static int GetNumBlackTiles(IEnumerable<((int x, int y) tile, bool isBlack)> tiles)
            => tiles.Count(t => t.isBlack);

        private static int AdjacentBlackTiles((int x, int y) tile, Dictionary<(int x, int y), bool> d)
            => Adjacent(tile).Where(t => IsBlack(t, d)).Count();

        private static IEnumerable<(int, int)> Adjacent((int x, int y) tile)
        {
            yield return (tile.x + 2, tile.y);
            yield return (tile.x - 2, tile.y);
            yield return (tile.x + 1, tile.y + 1);
            yield return (tile.x + 1, tile.y - 1);
            yield return (tile.x - 1, tile.y + 1);
            yield return (tile.x - 1, tile.y - 1);
        }

        private static bool IsBlack((int x, int y) tile, Dictionary<(int x, int y), bool> d)
            => d.ContainsKey(tile) && d[tile];

        private static (int x, int y) ParseInstructions(string e)
        {
            int y = 0;
            int x = 0;
            bool previousNorthSouth = false;
            foreach (var c in e)
            {
                switch (c)
                {
                    case 'n':
                        y++;
                        previousNorthSouth = true;
                        break;
                    case 's':
                        y--;
                        previousNorthSouth = true;
                        break;
                    case 'e':
                        x++;
                        if (!previousNorthSouth)
                        {
                            x++;
                        }
                        previousNorthSouth = false;
                        break;
                    case 'w':
                        x--;
                        if (!previousNorthSouth)
                        {
                            x--;
                        }
                        previousNorthSouth = false;
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            return (x, y);
        }
    }
}
