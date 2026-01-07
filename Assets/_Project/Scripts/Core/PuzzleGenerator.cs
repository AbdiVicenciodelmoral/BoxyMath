// Assets/_Project/Scripts/Core/PuzzleGenerator.cs
using System;
using System.Collections.Generic;

public class PuzzleGenerator
{
    public int N { get; private set; }
    public int[,] Solution { get; private set; }

    private readonly Random rng;

    public PuzzleGenerator(int n, int? seed = null)
    {
        N = n;
        Solution = new int[n, n];
        rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public void GenerateSolution()
    {
        // latin[r,c] = ((r+c)%N)+1
        int[,] latin = new int[N, N];
        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
                latin[r, c] = ((r + c) % N) + 1;

        int[] rowIdx = ShuffledIndex(N);
        int[] colIdx = ShuffledIndex(N);
        int[] mapIdx = ShuffledIndex(N); // relabel 1..N

        // Row permutation
        int[,] rowPerm = new int[N, N];
        for (int r = 0; r < N; r++)
        {
            int newR = rowIdx[r];
            for (int c = 0; c < N; c++)
                rowPerm[newR, c] = latin[r, c];
        }

        // Col permutation
        int[,] colPerm = new int[N, N];
        for (int r = 0; r < N; r++)
        {
            for (int c = 0; c < N; c++)
            {
                int newC = colIdx[c];
                colPerm[r, newC] = rowPerm[r, c];
            }
        }

        // Relabel numbers (optional but nice for variety)
        int[,] relabeled = new int[N, N];
        for (int r = 0; r < N; r++)
        {
            for (int c = 0; c < N; c++)
            {
                int v = colPerm[r, c];          // 1..N
                int mapped = mapIdx[v - 1] + 1; // still 1..N
                relabeled[r, c] = mapped;
            }
        }

        Solution = relabeled;
    }

    private int[] ShuffledIndex(int n)
    {
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;

        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }

    // Latin validity: each row/col contains 1..N exactly once
    public bool CheckLatinValidity(int[,] grid)
    {
        for (int r = 0; r < N; r++)
        {
            bool[] seen = new bool[N + 1];
            for (int c = 0; c < N; c++)
            {
                int v = grid[r, c];
                if (v < 1 || v > N || seen[v]) return false;
                seen[v] = true;
            }
        }

        for (int c = 0; c < N; c++)
        {
            bool[] seen = new bool[N + 1];
            for (int r = 0; r < N; r++)
            {
                int v = grid[r, c];
                if (v < 1 || v > N || seen[v]) return false;
                seen[v] = true;
            }
        }
        return true;
    }

    // Temporary grouping stub — covers the board with cages of size 2-3
    public List<Cage> GroupingStub()
    {
        var cells = new List<CellPos>(N * N);
        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
                cells.Add(new CellPos(r, c));

        // shuffle cells
        for (int i = cells.Count - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (cells[i], cells[j]) = (cells[j], cells[i]);
        }

        var cages = new List<Cage>();
        int idx = 0;
        while (idx < cells.Count)
        {
            int size = rng.Next(2, 4); // 2..3
            int remaining = cells.Count - idx;
            if (remaining < 2) size = remaining;

            var cage = new Cage();
            for (int k = 0; k < size; k++)
                cage.cells.Add(cells[idx++]);

            cages.Add(cage);
        }

        return cages;
    }
}
