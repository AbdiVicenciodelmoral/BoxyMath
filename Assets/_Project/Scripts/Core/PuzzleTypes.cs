// Scripts/Core/PuzzleTypes.cs
using System;
using System.Collections.Generic;

public enum ViewMode
{
    Game,
    Groups,
    GroupsAndNumbers
}

public enum CageOp
{
    None, Add, Subtract, Multiply, Divide
}

[Serializable]
public struct CellPos
{
    public int r;
    public int c;
    public CellPos(int r, int c) { this.r = r; this.c = c; }
}

[Serializable]
public class Cage
{
    public List<CellPos> cells = new();
    public CageOp op = CageOp.None;
    public int target = 0;
}
