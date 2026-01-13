// Assets/_Project/Scripts/UI/CellView.cs
//
// This script represents the *visual* side of a single cell on the puzzle grid.
//
// Important separation of concerns:
// - CellView does NOT know puzzle rules
// - CellView does NOT know the solution
// - CellView does NOT generate anything
//
// It only:
// - knows its (row, column) position
// - displays text (numbers, hints, etc.)
// - will eventually draw cage borders

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    /// <summary>
    /// Reference to a TextMeshPro text component.
    ///
    /// This is assigned in the Inspector, usually by dragging
    /// a child TextMeshProUGUI object into this field.
    ///
    /// [SerializeField] keeps the field private
    /// but still visible/editable in Unity.
    /// </summary>
    [SerializeField] private TMP_Text label;

    [SerializeField] private Image borderTop;
    [SerializeField] private Image borderRight;
    [SerializeField] private Image borderBottom;
    [SerializeField] private Image borderLeft;


    /// <summary>
    /// Row index of this cell in the puzzle grid.
    /// These are set by a GridRenderer or similar manager
    /// when the grid is created.
    /// </summary>
    public int R { get; private set; }

    /// <summary>
    /// Column index of this cell in the puzzle grid.
    /// </summary>
    public int C { get; private set; }

    /// <summary>
    /// Assigns the logical grid position to this cell.
    ///
    /// This is usually called once when the grid is built:
    /// - Instantiate cell prefab
    /// - Set its (r, c)
    /// - Store it in a 2D array or list
    /// </summary>
    public void SetPos(int r, int c)
    {
        R = r;
        C = c;
    }

    /// <summary>
    /// Sets the visible text inside the cell.
    ///
    /// Examples of usage:
    /// - Show a player-entered number
    /// - Show a given number
    /// - Clear the cell ("")
    ///
    /// The null check protects against missing Inspector references
    /// and prevents a NullReferenceException.
    /// </summary>
    public void SetText(string s)
    {
        if (label != null)
            label.text = s;
    }

    //method that can hide/show specific borders on a cell.
    public void SetTopBorderVisible(bool visible)
    {
        if (borderTop != null)
            borderTop.enabled = visible;
    }

    public void SetRightBorderVisible(bool visible)
    {
        if (borderRight != null)
            borderRight.enabled = visible;
    }

    public void SetBottomBorderVisible(bool visible)
    {
        if (borderBottom != null)
            borderBottom.enabled = visible;
    }

    public void SetLeftBorderVisible(bool visible)
    {
        if (borderLeft != null)
            borderLeft.enabled = visible;
    }

    // ---------------------------------------------------------
    // FUTURE EXTENSION POINT
    // ---------------------------------------------------------
    //
    // This class is intentionally minimal right now.
    // Later, this is where you'll add code to draw cage borders.
    //
    // Example future API:
    //
    // public void SetBorders(bool top, bool right, bool bottom, bool left)
    // {
    //     // Enable/disable border GameObjects or UI Images
    // }
    //
    // This keeps all visual logic for a cell in one place.
}
