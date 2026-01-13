using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.UxmlAttributeDescription;


/// <summary>
/// GridRenderer is responsible for creating and updating
/// the *visual grid* of cells in the Unity UI.
///
/// Important responsibilities:
/// - Instantiate cell prefabs in an NxN layout
/// - Keep track of CellView objects by (row, col)
/// - Decide what to draw based on ViewMode
///
/// Important non-responsibilities:
/// - It does NOT generate puzzles
/// - It does NOT validate player input
/// - It does NOT store game rules
///
/// Think of this as a "view" or "presentation" layer.
///
public class GridRenderer : MonoBehaviour
{
    // --------------------------------------------------
    // Inspector-assigned references
    // --------------------------------------------------

    [Header("Prefabs / References")]

    /// 
    /// Parent RectTransform that will hold all cell instances.
    ///
    /// Typically this GameObject also has:
    /// - GridLayoutGroup (for automatic layout)
    /// - ContentSizeFitter (optional)
    ///
    /// Cells are instantiated as children of this transform.
    /// 
    [SerializeField] private RectTransform gridRoot;

    /// 
    /// Prefab for a single cell.
    ///
    /// This prefab must have a CellView component on it.
    /// GridRenderer instantiates this prefab N*N times.
    /// 
    [SerializeField] private CellView cellPrefab;

    // --------------------------------------------------
    // State / internal data
    // --------------------------------------------------

    ///
    /// Current display mode for the grid.
    /// Determines what is drawn when Redraw() is called.
    /// 
    public ViewMode Mode { get; private set; } = ViewMode.GroupsAndNumbers;

    /// 
    /// Size of the grid (NxN).
    /// Derived from the solution array when SetPuzzle() is called.
    /// 
    private int n;

    /// 
    /// 2D array of CellView references.
    /// cells[r, c] corresponds to the visual cell at row r, column c.
    /// 
    private CellView[,] cells;

    /// 
    /// The solved puzzle grid.
    /// Used only for display purposes (GroupsAndNumbers mode).
    /// 
    private int[,] solution;

    /// <summary>
    /// List of cages describing how cells are grouped.
    /// Used to draw cage borders / visuals.
    /// </summary>
    private List<Cage> cages;

    // --------------------------------------------------
    // Public API
    // --------------------------------------------------

    /// 
    /// Changes how the grid is displayed (Game / Groups / GroupsAndNumbers).
    ///
    /// This does NOT rebuild the grid — it only updates visuals.
    /// 
    public void SetMode(ViewMode mode)
    {
        Mode = mode;
        Redraw();
    }

    /// 
    /// Supplies the puzzle data to the renderer.
    ///
    /// This is typically called once when a new puzzle is loaded.
    ///
    public void SetPuzzle(int[,] solutionGrid, List<Cage> cages)
    {
        solution = solutionGrid;
        this.cages = cages;

        // Infer grid size from the solution array.
        n = solutionGrid.GetLength(0);

        // Ensure the grid exists.
        BuildGridIfNeeded();

        // Draw everything according to the current mode.
        Redraw();
    }

    // --------------------------------------------------
    // Grid construction
    // --------------------------------------------------

    /// 
    /// Builds the visual grid if needed.
    ///
    /// Current behavior:
    /// - Always destroys and rebuilds the grid
    ///
    /// This is simple and safe, but not optimal.
    /// Later you could:
    /// - Reuse cells
    /// - Resize instead of rebuild
    ///
    private void BuildGridIfNeeded()
    {
        // Safety check: if references are missing, do nothing.
        if (gridRoot == null || cellPrefab == null)
            return;


       // What this line means(plain language)
       // gridRoot -> the UI object that holds the cells
       // GetComponent<GridLayoutGroup>() -> “give me the layout controller on this object”
        gridRoot.GetComponent<GridLayoutGroup>().constraintCount = n;

        // Destroy all existing cell GameObjects.
        // We iterate backwards because we're removing children.
        for (int i = gridRoot.childCount - 1; i >= 0; i--)
            Destroy(gridRoot.GetChild(i).gameObject);

        // Allocate the 2D array to track cell views.
        cells = new CellView[n, n];

        // Instantiate new cells row by row.
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                // Instantiate prefab as a child of gridRoot.
                var cell = Instantiate(cellPrefab, gridRoot);


                /// THESE TESTS Remove Borders manually...
                // Will Remove during code Clean up.
                //if (r == 0) cell.SetTopBorderVisible(false);
                //if (c == 0) cell.SetLeftBorderVisible(false);
                //if (c == n - 1) cell.SetRightBorderVisible(false);
                //if (r == n - 1) cell.SetBottomBorderVisible(false);

                // Tell the cell which logical position it represents.
                cell.SetPos(r, c);

                // Clear text initially.
                cell.SetText("");

                // Store reference for later access.
                cells[r, c] = cell;

            }
        }
    }

    // --------------------------------------------------
    // Rendering logic
    // --------------------------------------------------

    /// 
    /// Updates the visual state of the grid based on:
    /// - current Mode
    /// - solution
    /// - cages
    ///
    /// This method can be called many times safely.
    ///
    private void Redraw()
    {
        // If the grid hasn't been built yet, do nothing.
        if (cells == null)
            return;

        // Step 1: clear all text.
        // This prevents leftover text when switching modes.
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
                cells[r, c].SetText("");

        // Step 2: handle each view mode.
        if (Mode == ViewMode.Game)
        {
            // In Game mode:
            // - we will later draw player-entered values
            // - no cage numbers, no solution
            return;
        }

        if (Mode == ViewMode.Groups || Mode == ViewMode.GroupsAndNumbers)
        {
            // Apply cage visuals (currently a stub).
            ApplyCageVisualsStub();
        }

        if (Mode == ViewMode.GroupsAndNumbers && solution != null)
        {
            // Display the solution numbers in each cell.
            // This is useful for debugging or editor previews.
            for (int r = 0; r < n; r++)
                for (int c = 0; c < n; c++)
                    cells[r, c].SetText(solution[r, c].ToString());
        }
    }

    // --------------------------------------------------
    // Cage visuals (stub)
    // --------------------------------------------------

    /// 
    /// Temporary placeholder for cage rendering logic.
    ///
    /// Eventually this method will:
    /// - determine which edges of each cell are cage borders
    /// - enable thick borders or outlines accordingly
    ///
    /// Right now, it does nothing.
    ///
    private void ApplyCageVisualsStub()
    {
        // If there are no cages defined, there is nothing to map.
        // Early exit prevents null reference errors later.
        if (cages == null) return;

        // 1) Build a lookup table that tells us:
        //    "Given a cell (r,c), which cage does it belong to?"
        //
        // We flatten (r,c) into a single integer key: key = r*n + c
        // This lets us store the mapping in a dictionary for fast O(1) access.
        //
        // Invariant after this loop:
        //   - Every cell that belongs to a cage has exactly one entry in cellToCage.
        //   - cellToCage[key] == index of the cage that owns that cell.
        var cellToCage = new Dictionary<int, int>(n * n);

        // Loop over all cages by index so we can store the cage index as the value.
        for (int cageIndex = 0; cageIndex < cages.Count; cageIndex++)
        {
            // Each cage contains a list of its cells (with row/column coordinates).
            foreach (var p in cages[cageIndex].cells)
            {
                // Convert 2D grid coordinates (r,c) into a unique 1D key.
                int key = p.r * n + p.c;

                // Record that this cell belongs to cageIndex.
                // If cages are well-formed, each cell should appear exactly once.
                cellToCage[key] = cageIndex;
            }
        }

        // 2) Decide which borders to draw for every cell.
        //
        // Goal: show cage outlines.
        // Rule: a border is visible when it separates *different cages*,
        // or when the cell is on the outer edge of the board.
        //
        // We have a mapping cellToCage[] that tells us which cage each cell belongs to.
        // cellToCage is 1D, so we convert (r,c) -> index = r*n + c.
        // cells[,] is 2D and holds the UI cell objects we can set borders on.
        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
            {
                // Cage id for the current cell
                int myCage = cellToCage[r * n + c];

                // TOP border:
                // - visible if we're on the first row (no neighbor above),
                //   OR if the cell above belongs to a different cage.
                bool top = (r == 0) ||
                           (cellToCage[(r - 1) * n + c] != myCage);

                // BOTTOM border:
                // - visible if we're on the last row,
                //   OR if the cell below belongs to a different cage.
                bool bottom = (r == n - 1) ||
                              (cellToCage[(r + 1) * n + c] != myCage);

                // LEFT border:
                // - visible if we're in the first column,
                //   OR if the cell to the left belongs to a different cage.
                bool left = (c == 0) ||
                            (cellToCage[r * n + (c - 1)] != myCage);

                // RIGHT border:
                // - visible if we're in the last column,
                //   OR if the cell to the right belongs to a different cage.
                bool right = (c == n - 1) ||
                             (cellToCage[r * n + (c + 1)] != myCage);

                // Apply the visibility decisions to the UI cell borders.
                cells[r, c].SetTopBorderVisible(top);
                cells[r, c].SetBottomBorderVisible(bottom);
                cells[r, c].SetLeftBorderVisible(left);
                cells[r, c].SetRightBorderVisible(right);

                // Debug helper: print a couple of specific cells to verify logic.
                // (Remove once confirmed.)
                if ((r == 0 && c == 0) || (r == 1 && c == 0))
                    Debug.Log($"Cell({r},{c}) myCage={myCage} top={top} bottom={bottom}");
            }

    }



}
