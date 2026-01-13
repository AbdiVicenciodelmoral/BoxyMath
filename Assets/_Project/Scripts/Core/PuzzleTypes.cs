// Scripts/Core/PuzzleTypes.cs
// This file defines "data types" used by your puzzle game.
// Think of it as the vocabulary for describing a puzzle:
// - ViewMode: how the board should be displayed
// - CageOp: what math operation a cage uses
// - CellPos: a row/column position on the grid
// - Cage: a group of cells + its operation + target number

using System;
using System.Collections.Generic;

/// <summary>
/// Controls how the puzzle board is displayed.
/// You would typically store this in a GameManager / UI controller,
/// and change it when the player presses a button (like "Toggle view").
/// </summary>
public enum ViewMode
{
    // Show the normal game view (likely just the grid / player inputs).
    Game,

    // Show "cages" / groups visually (maybe colored outlines or borders).
    Groups,

    // Show cages AND number hints/labels (like cage targets).
    GroupsAndNumbers
}

/// <summary>
/// Describes what operation a cage uses.
/// This is common in KenKen / Calcudoku style puzzles:
/// e.g. a cage might say "6+" meaning the cells sum to 6.
/// </summary>
public enum CageOp
{
    // No operation. Often used for 1-cell cages where the target is the number itself.
    None,

    // Target is the SUM of all cell values in the cage.
    Add,

    // Target is the RESULT of subtraction.
    // Usually subtraction cages have exactly 2 cells and order doesn't matter:
    // abs(a - b) == target
    Subtract,

    // Target is the PRODUCT of all cell values in the cage.
    Multiply,

    // Target is the RESULT of division.
    // Usually division cages have exactly 2 cells and order doesn't matter:
    // max(a, b) / min(a, b) == target (and must divide evenly)
    Divide
}

//Defines a named set of allowed directions
//Represents direction as:
//MergeDir.Down
//MergeDir.Right
//Makes future extensions obvious (Up, Left later)
public enum MergeDir { 
    Down, 
    Right 
}



/// <summary>
/// A position of a single cell on the grid.
/// "r" = row index, "c" = column index.
/// </summary>
/// <remarks>
/// [Serializable] is important in Unity:
/// - It allows Unity to save/load this data
/// - It allows Unity to show it in the Inspector (when inside another serializable type)
/// - It allows it to be included in ScriptableObjects, prefabs, JSON saves, etc.
/// </remarks>
[Serializable]
public struct CellPos
{
    // Row index (0-based is typical: top row is 0)
    public int r;

    // Column index (0-based is typical: left column is 0)
    public int c;

    // Constructor lets you create a position easily: new CellPos(2, 3)
    public CellPos(int r, int c)
    {
        this.r = r;
        this.c = c;
    }
}

/// <summary>
/// A "Cage" represents a group of one or more grid cells that share a constraint.
/// Example: In KenKen, a cage might be:
/// - cells: (0,0), (0,1)
/// - op: Add
/// - target: 5
/// meaning the values in those two cells must add up to 5.
/// </summary>
/// <remarks>
/// This is a CLASS (reference type), not a struct.
/// That usually makes sense here because:
/// - cages can contain lists and be edited/modified
/// - you often want to pass them around by reference
///
/// [Serializable] allows Unity to serialize this class and show it in the inspector
/// (when it is a field of a MonoBehaviour or ScriptableObject).
/// </remarks>
[Serializable]
public class Cage
{
    /// <summary>
    /// All the grid positions that belong to this cage.
    /// </summary>
    /// <remarks>
    /// "new()" here is C# shorthand for "new List<CellPos>()".
    /// It initializes the list so you can immediately add cells to it.
    /// </remarks>
    public List<CellPos> cells = new();

    /// <summary>
    /// The math rule for this cage (Add/Subtract/etc).
    /// Default is None (useful for 1-cell cages).
    /// </summary>
    public CageOp op = CageOp.None;

    /// <summary>
    /// The target result for the cage rule.
    /// Examples:
    /// - Add cage: target is sum
    /// - Multiply cage: target is product
    /// - None cage (single cell): target is the value that cell must be
    /// </summary>
    public int target = 0;
}
