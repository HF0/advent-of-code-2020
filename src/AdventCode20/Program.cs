using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventCode20
{
    class Program
    {
        private const char Mark = '#';
        private delegate (int i, int j, int length) Transformer((int i, int j, int length) coordinates);
        private static readonly Transformer Identity = x => x;
        private static readonly Transformer HorizontalFlip = p => (p.i, p.length - p.j - 1, p.length);
        private static readonly Transformer VerticalFlip = p => (p.length - p.i - 1, p.j, p.length);
        private static readonly Transformer Rot1 = p => (p.j, p.length - p.i - 1, p.length);
        private static readonly Transformer Rot2 = p => (p.length - p.i - 1, p.length - p.j - 1, p.length);
        private static readonly Transformer Rot3 = p => (p.length - p.j - 1, p.i, p.length);
        private static readonly Transformer[] Rotations = new[] { Identity, Rot1, Rot2, Rot3 };

        private const int Up = 0;
        private const int Right = 1;
        private const int Down = 2;
        private const int Left = 3;

        static void Main(string[] args)
        {
            var tiles = File.ReadAllText("input.txt")
                .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(e => ParseTile(e))
                .ToDictionary(k => k.id, k => k);

            IEnumerable<int> cornerTilesId = tiles.Values
                .Where(e => IsCornerTile(e, tiles))
                .Select(e => e.id);
            long solution1 = cornerTilesId
                .Select(e => (long)e)
                .Aggregate((a, b) => a * b);

            var tilesInSide = (int)Math.Sqrt(tiles.Count());
            Dictionary<(int, int), int> assignedTiles = FindGridTiles(tiles, tilesInSide);
            long solution1b = (long)
                assignedTiles[(0, 0)] *
                assignedTiles[(0, tilesInSide - 1)] *
                assignedTiles[(tilesInSide - 1, 0)] *
                assignedTiles[(tilesInSide - 1, tilesInSide - 1)];
            Console.WriteLine($"Solution 1: {solution1} {solution1b}");

            char[][] image = AssembleImage(assignedTiles, tiles, tilesInSide);
            ISet<(int, int)> originalPattern = File.ReadAllLines("seamonster.txt")
                .SelectMany((e, i) => ParsePatternRow(e, i))
                .ToHashSet();

            var solutionGrid = GetMarkGrid(originalPattern, image);
            Show(solutionGrid);

            int solution2 = solutionGrid
                .SelectMany(e => e)
                .Where(c => c.Equals(Mark))
                .Count();
            Console.WriteLine($"Solution 2: {solution2}");
        }

        private static char[][] GetMarkGrid(ISet<(int, int)> originalPattern, char[][] image)
        {
            var firstMatch = GetImageTransformations()
                .Select(t => TransformTile(t, image))
                .Select(grid => MarkPattern(grid, originalPattern))
                .Where(s => s.matches != 0)
                .FirstOrDefault();
            return firstMatch.grid;
        }

        private static IEnumerable<Transformer> GetImageTransformations()
        {
            foreach (var rotationTransformation in Rotations)
            {
                yield return rotationTransformation;
                Transformer flippedRotationTransformation =
                    x => rotationTransformation(HorizontalFlip(x));
                yield return flippedRotationTransformation;
            }
        }

        private static ISet<(int, int)> ApplyTransformationPattern(Transformer t, ISet<(int, int)> pattern)
        {
            ISet<(int, int)> result = new HashSet<(int, int)>();
            var maxX = pattern.Max(e => e.Item1);
            foreach (var p in pattern)
            {
                var (i, j, length) = t((p.Item1, p.Item2, maxX));
                result.Add((i, j));
            }
            return result;
        }

        private static (int matches, char[][] grid) MarkPattern(char[][] image, ISet<(int, int)> pattern)
        {
            int patternsFound = 0;
            var maxX = pattern.Max(e => e.Item1) + 1;
            var maxY = pattern.Max(e => e.Item2) + 1;
            for (int i = 0; i <= image.Length - maxX; i++)
            {
                for (int j = 0; j <= image[0].Length - maxY; j++)
                {
                    bool patternFound = pattern.All(p => image[i + p.Item1][j + p.Item2].Equals(Mark));
                    if (patternFound)
                    {
                        patternsFound++;
                        foreach (var p in pattern)
                        {
                            image[i + p.Item1][j + p.Item2] = 'O';
                        }
                    }
                }
            }
            return (patternsFound, image);
        }

        private static IEnumerable<(int, int)> ParsePatternRow(string e, int i)
        {
            return e.Select((c, j) => (c, j))
                .Where(e => e.c.Equals(Mark))
                .Select(e => (i, e.j));
        }

        private static char[][] AssembleImage(Dictionary<(int i, int j), int> assignedTiles, Dictionary<int, (int, (string[], string[]), char[][])> tiles, int tilesInSide)
        {
            int imageTileLength = tiles.First().Value.Item3.Length - 2;
            int imageLength = tilesInSide * imageTileLength;
            char[][] image = Enumerable.Range(0, imageLength)
                            .Select(_ => new char[imageLength]).ToArray();
            foreach (var k in assignedTiles)
            {
                var p = k.Key;
                int offsetX = p.i * imageTileLength;
                int offsetY = p.j * imageTileLength;
                var tile = tiles[k.Value];
                for (int i = 0; i < imageTileLength; i++)
                {
                    for (int j = 0; j < imageTileLength; j++)
                    {
                        image[i + offsetX][j + offsetY] = tile.Item3[i + 1][j + 1];
                    }
                }
            }
            return image;
        }

        private static Dictionary<(int, int), int> FindGridTiles(Dictionary<int, (int, (string[], string[]), char[][])> tiles, int tilesInSide)
        {
            Dictionary<(int, int), int> assignedTiles = new Dictionary<(int, int), int>();

            // First corner
            var oneCorner = tiles.Values.First(t => IsCornerTile(t, tiles));
            Transformer t = GetAlignTopLeftTransformation(tiles, oneCorner);
            tiles[oneCorner.Item1] = ApplyTransformationTile(t, oneCorner);
            assignedTiles[(0, 0)] = oneCorner.Item1;

            // Fill first row
            for (int j = 1; j < tilesInSide; j++)
            {
                var previousTile = tiles[assignedTiles[(0, j - 1)]];
                var nextTile = GetUnassignedTiles(tiles, assignedTiles)
                    .First(currentTile => MatchBorder(previousTile, currentTile, Right));
                var transformation = GetNeededTransformation(previousTile, nextTile, Right);
                tiles[nextTile.Item1] = ApplyTransformationTile(transformation, nextTile);
                assignedTiles[(0, j)] = nextTile.Item1;
            }
            // Fill rest
            for (int j = 0; j < tilesInSide; j++)
            {
                for (int i = 1; i < tilesInSide; i++)
                {
                    var previousTile = tiles[assignedTiles[(i - 1, j)]];
                    var nextTile = GetUnassignedTiles(tiles, assignedTiles)
                        .First(t => MatchBorder(previousTile, t, Down));
                    var transformation = GetNeededTransformation(previousTile, nextTile, Down);
                    tiles[nextTile.Item1] = ApplyTransformationTile(transformation, nextTile);
                    assignedTiles[(i, j)] = nextTile.Item1;
                }
            }
            return assignedTiles;
        }

        private static IEnumerable<(int, (string[], string[]), char[][])> GetUnassignedTiles(
            Dictionary<int, (int tileId, (string[], string[]), char[][])> tiles,
            Dictionary<(int, int), int> tileGrid)
            => tiles.Values.Where(e => !tileGrid.Values.Contains(e.tileId)).ToList();

        private static void Show(char[][] tile) =>
            Console.WriteLine(string.Join("", tile.Select(e => new string(e) + Environment.NewLine)));

        private static (int, (string[], string[]), char[][]) ApplyTransformationTile(Transformer transform, (int tileId, (string[], string[]), char[][] tiles) tileInfo)
        {
            char[][] newTile = TransformTile(transform, tileInfo.tiles);
            return (tileInfo.tileId, CreateBorders(newTile), newTile);
        }

        private static char[][] TransformTile(Transformer transform, char[][] image)
        {
            var imageLength = image.Length;
            char[][] newTile = Enumerable.Range(0, imageLength)
                            .Select(_ => new char[imageLength]).ToArray();
            for (int i = 0; i < imageLength; i++)
            {
                for (int j = 0; j < imageLength; j++)
                {
                    var c = transform((i, j, image.Length));
                    newTile[c.i][c.j] = image[i][j];
                }
            }
            return newTile;
        }

        private static Transformer GetAlignTopLeftTransformation(Dictionary<int, (int, (string[], string[]), char[][])> tiles,
            (int, (string[], string[]), char[][]) oneCorner)
        {
            Transformer t;
            if (HasEdge(Left, oneCorner, tiles) && HasEdge(Down, oneCorner, tiles))
            {
                t = Rot1;
            }
            else if (HasEdge(Down, oneCorner, tiles) && HasEdge(Right, oneCorner, tiles))
            {
                t = Rot2;
            }
            else if (HasEdge(Up, oneCorner, tiles) && HasEdge(Right, oneCorner, tiles))
            {
                t = Rot3;
            }
            else
            {
                t = Identity;
            }
            return t;
        }

        private static (string[], string[]) CreateBorders(char[][] tiles)
        {
            var up = new string(tiles[0]);
            var down = new string(tiles[^1]);
            var left = new string(tiles.Select(e => e[0]).ToArray());
            var right = new string(tiles.Select(e => e[^1]).ToArray());
            return (
                // No flip 
                new[] { up, right, down, left },
                // Flip
                new[] { Reverse(up), Reverse(right), Reverse(down), Reverse(left) });
        }

        private static bool IsCornerTile((int, (string[], string[]), char[][]) tile,
            Dictionary<int, (int, (string[], string[]), char[][])> tiles)
        {
            var otherTiles = tiles.Values.Where(e => !e.Item1.Equals(tile.Item1));
            return (HasEdge(Up, tile, tiles) && HasEdge(Left, tile, tiles)) ||
                (HasEdge(Left, tile, tiles) && HasEdge(Down, tile, tiles)) ||
                (HasEdge(Down, tile, tiles) && HasEdge(Right, tile, tiles)) ||
                (HasEdge(Right, tile, tiles) && HasEdge(Up, tile, tiles));
        }

        private static bool HasEdge(int side, (int, (string[], string[]), char[][]) tile,
            Dictionary<int, (int, (string[], string[]), char[][])> tiles)
        {
            var otherTiles = tiles.Values.Where(e => !e.Item1.Equals(tile.Item1));
            return otherTiles.All(t => !MatchBorder(tile, t, side));
        }

        private static Transformer GetNeededTransformation((int, (string[], string[]), char[][]) src,
            (int, (string[], string[]), char[][]) dst, int side)
        {
            var normalBorders = dst.Item2.Item1;
            var flippedBorders = dst.Item2.Item2;
            var sourceSide = src.Item2.Item1[side];

            Transformer initialTransformer;
            int oppositeSide = (side + 2) % 4;
            int foundSide = Array.IndexOf(normalBorders, sourceSide);
            bool mustFlip = false;
            if (foundSide == -1)
            {
                mustFlip = true;
                foundSide = Array.IndexOf(flippedBorders, sourceSide);
                if (foundSide == -1)
                {
                    throw new ArgumentException();
                }
            }
            // flip occurs in down and left
            bool flipPresent = (oppositeSide >= 2 && foundSide < 2) ||
                (oppositeSide < 2 && foundSide >= 2);
            bool flipNeeded = mustFlip != flipPresent;
            if (flipNeeded)
            {
                initialTransformer = foundSide == Up || foundSide == Down ? VerticalFlip : HorizontalFlip;
            }
            else
            {
                initialTransformer = Identity;
            }
            int flipRotations = flipNeeded ? 2 : 0;
            int numRotations = (oppositeSide - foundSide + flipRotations + 4) % 4;
            Transformer result = x => Rotations[numRotations](initialTransformer(x));
            return result;
        }

        private static bool MatchBorder((int, (string[], string[]), char[][]) src, (int, (string[], string[]), char[][]) dst, int side)
        {
            var sourceSide = src.Item2.Item1[side];
            var normalBorders = dst.Item2.Item1;
            var flippedBorders = dst.Item2.Item2;
            return normalBorders.Contains(sourceSide) || flippedBorders.Contains(sourceSide);
        }
        private static (int id, (string[], string[]), char[][]) ParseTile(string e)
        {
            var lines = e.Split("\n", StringSplitOptions.RemoveEmptyEntries);
            var name = int.Parse(lines.First()[5..^1]);
            var tiles = lines.Skip(1)
                .Select(e => e.ToArray())
                .ToArray();
            return (name, CreateBorders(tiles), tiles);
        }

        private static string Reverse(string str) => new string(str.ToCharArray().Reverse().ToArray());
    }
}
