// Assets/_Project/Scripts/Core/PuzzleGenerator.cs
//
// This class is a "pure C# generator" (NOT a MonoBehaviour).
// That means you do not attach it to a GameObject.
//
// Instead, some Unity script (like PuzzleManager) will create an instance like:
//    var gen = new PuzzleGenerator(n, seed);
//    gen.GenerateSolution();
//    int[,] solvedGrid = gen.Solution;
//    List<Cage> cages = gen.GroupingStub();
//
// This generator currently does two main things:
// 1) Creates a Latin square solution grid (each row/col contains 1..N once)
// 2) Creates a temporary "cage grouping" (stub) that groups cells into cages of size 2-3

using System;
using System.Collections.Generic;
using Random = System.Random;
using UnityEngine;

public class PuzzleGenerator
{
    /// 
    /// The size of the puzzle grid (NxN).
    /// Example: N=4 => 4x4 puzzle with numbers 1..4.
    /// 
    public int N { get; private set; }

    /// 
    /// The generated solved grid.
    /// This is an N by N array of integers.
    ///
    /// In Unity you might later copy this into:
    /// - a board model (for validation),
    /// - cage calculations,
    /// - or a "solution reveal" feature.
    /// 
    public int[,] Solution { get; private set; }

    /// 
    /// Random number generator used for shuffling rows/cols and grouping.
    /// Making this a field means the generator stays consistent for a given seed.
    /// 
    private readonly Random rng;

    /// 
    /// Create a generator for an NxN puzzle.
    ///
    /// seed is optional:
    /// - If you pass a seed, results are repeatable (great for debugging / sharing puzzles).
    /// - If seed is null, it uses time-based randomness (different every run).
    /// 
    public PuzzleGenerator(int n, int? seed = null)
    {
        N = n;

        // Allocate the solution array right away.
        // It will be filled in GenerateSolution().
        Solution = new int[n, n];

        // If a seed is provided, use it. Otherwise randomize normally.
        rng = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    ///
    /// Generates a Latin square solution.
    /// A Latin square means:
    /// - Each row has 1..N exactly once
    /// - Each column has 1..N exactly once
    ///
    /// This is a common "base" solution for KenKen-like puzzles.
    ///
    public void GenerateSolution()
    {
        // STEP 1: Build a simple "base" Latin square.
        //
        // latin[r,c] = ((r+c) % N) + 1
        //
        // For N=4, this produces:
        // 1 2 3 4
        // 2 3 4 1
        // 3 4 1 2
        // 4 1 2 3
        //
        // This is valid but always looks the same for a given N.
        int[,] latin = new int[N, N];
        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
                latin[r, c] = ((r + c) % N) + 1;

        // STEP 2: Create random permutations (shuffle orders)
        // - rowIdx: rearranges which rows appear where
        // - colIdx: rearranges which columns appear where
        // - mapIdx: remaps the symbols 1..N (ex: swap all 1s with 4s, etc.)
        //
        // These preserve Latin validity while giving many different-looking solutions.
        int[] rowIdx = ShuffledIndex(N);
        int[] colIdx = ShuffledIndex(N);
        int[] mapIdx = ShuffledIndex(N); // relabel 1..N

        // STEP 3: Apply row permutation
        //
        // We take original row r from latin, and place it into "newR".
        // Example: if rowIdx[0] = 2, then original row 0 moves to row 2.
        //
        // This keeps each row a valid set of 1..N, just reordered in the grid.
        int[,] rowPerm = new int[N, N];
        for (int r = 0; r < N; r++)
        {
            int newR = rowIdx[r];
            for (int c = 0; c < N; c++)
                rowPerm[newR, c] = latin[r, c];
        }

        // STEP 4: Apply column permutation
        //
        // Similar idea: original column c moves to newC.
        // Example: if colIdx[0] = 3, original col 0 moves to col 3.
        int[,] colPerm = new int[N, N];
        for (int r = 0; r < N; r++)
        {
            for (int c = 0; c < N; c++)
            {
                int newC = colIdx[c];
                colPerm[r, newC] = rowPerm[r, c];
            }
        }

        // STEP 5: Relabel numbers (optional, but adds variety)
        //
        // Even after shuffling rows/cols, the *pattern* might still feel similar.
        // Relabeling changes the actual symbols: all 1s become something else, etc.
        //
        // mapIdx is a shuffled list of [0..N-1].
        // If mapIdx = [2,0,3,1] for N=4, then:
        // value 1 -> 3
        // value 2 -> 1
        // value 3 -> 4
        // value 4 -> 2
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

        // Final: store the generated solution.
        Solution = relabeled;
    }

    /// 
    /// Returns an array [0..n-1] in random order.
    ///
    /// This uses Fisher-Yates shuffle:
    /// - O(n) time
    /// - produces an unbiased shuffle (every permutation equally likely)
    ///
    /// Used for shuffling rows, columns, and number labels.
    ///
    private int[] ShuffledIndex(int n)
    {
        // Start with identity: [0, 1, 2, ..., n-1]
        int[] a = new int[n];
        for (int i = 0; i < n; i++) a[i] = i;

        // Fisher-Yates:
        // for i from n-1 down to 1:
        // pick j in [0..i], swap a[i] and a[j]
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);
            (a[i], a[j]) = (a[j], a[i]);
        }
        return a;
    }

    /// 
    /// Checks whether a given grid is a valid Latin square of size N.
    ///
    /// Latin validity means:
    /// - each row contains every number 1..N exactly once
    /// - each column contains every number 1..N exactly once
    ///
    /// This is useful for debugging your generator:
    /// after GenerateSolution(), you can confirm it produced a valid result.
    /// 
    public bool CheckLatinValidity(int[,] grid)
    {
        // Check each row
        for (int r = 0; r < N; r++)
        {
            // seen[v] tells us whether we've already encountered value v in this row.
            // We use N+1 so we can index directly by value (1..N).
            bool[] seen = new bool[N + 1];

            for (int c = 0; c < N; c++)
            {
                int v = grid[r, c];

                // Fail if:
                // - value out of range
                // - value already appeared in this row
                if (v < 1 || v > N || seen[v]) return false;

                seen[v] = true;
            }
        }

        // Check each column
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

        // If we got here, every row and every column was valid.
        return true;
    }

    /// 
    /// TEMPORARY / PLACEHOLDER cage grouping.
    ///
    /// This does NOT guarantee the puzzle is solvable, unique, or "nice".
    /// It simply covers the board with random cages of size 2-3 (and possibly 1 at the end).
    ///
    /// Why it exists:
    /// - useful for testing cage rendering in Unity (borders, colors, labels)
    /// - useful to test the rest of your pipeline before real cage logic exists
    ///
    /// Next step (later):
    /// - create cages using the Solution grid
    /// - choose an operation + target that matches the solution values
    /// - ensure constraints lead to a unique solution
    ///
    public List<Cage> MakeSingleCellCages()
    {
        Debug.Log("MakeSingleCellCages() called");
        MergeDir dir = MergeDir.Down;
        int mergeR = 0, mergeC = 0; // anchor cell to merge from

        //If we are merging downward, check whether the cell below exists.
        //Otherwise, check whether the cell to the right exists.
        //condition? valueIfTrue : valueIfFalse
        //Current code: condition? (return boolean -> true or false) : (return boolean -> true or false)
        bool mergeInBounds = dir == MergeDir.Down ? (mergeR + 1 < N) : (mergeC + 1 < N);

        var cages = new List<Cage>(N * N);
        Cage anchorCage = null;
        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
            {
                if (anchorCage != null && mergeInBounds &&
                    ((dir == MergeDir.Down && r == mergeR + 1 && c == mergeC) ||
                     (dir == MergeDir.Right && r == mergeR && c == mergeC + 1)))
                {
                    anchorCage.cells.Add(new CellPos(r, c));
                    continue;
                }

                var cage = new Cage();
                if (r == mergeR && c == mergeC) anchorCage = cage;

                cage.cells.Add(new CellPos(r, c));
                cages.Add(cage);
                

            }
        return cages;
    }

}
