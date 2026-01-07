using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [Header("Prefabs / References")]
    [SerializeField] private RectTransform gridRoot; // parent for cells (GridLayoutGroup later)
    [SerializeField] private CellView cellPrefab;

    public ViewMode Mode { get; private set; } = ViewMode.GroupsAndNumbers;

    private int n;
    private CellView[,] cells;

    private int[,] solution;
    private List<Cage> cages;

    public void SetMode(ViewMode mode)
    {
        Mode = mode;
        Redraw();
    }

    public void SetPuzzle(int[,] solutionGrid, List<Cage> cages)
    {
        solution = solutionGrid;
        this.cages = cages;
        n = solutionGrid.GetLength(0);

        BuildGridIfNeeded();
        Redraw();
    }

    private void BuildGridIfNeeded()
    {
        // Skeleton behavior: rebuild every time.
        // Later we can optimize / reuse.
        if (gridRoot == null || cellPrefab == null) return;

        // Destroy old children
        for (int i = gridRoot.childCount - 1; i >= 0; i--)
            Destroy(gridRoot.GetChild(i).gameObject);

        cells = new CellView[n, n];

        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.SetPos(r, c);
                cell.SetText("");
                cells[r, c] = cell;
            }
        }
    }

    private void Redraw()
    {
        if (cells == null) return;

        // Clear all text first
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                cells[r, c].SetText("");

        if (Mode == ViewMode.Game)
        {
            // later: render player grid
            return;
        }

        if (Mode == ViewMode.Groups || Mode == ViewMode.GroupsAndNumbers)
        {
            ApplyCageVisualsStub();
        }

        if (Mode == ViewMode.GroupsAndNumbers && solution != null)
        {
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    cells[r, c].SetText(solution[r, c].ToString());
        }
    }

    private void ApplyCageVisualsStub()
    {
        // Skeleton stub like your PyQt _apply_group_styles.
        // Next step later: compute thick borders per cell based on cages adjacency.
        if (cages == null) return;
    }
}
