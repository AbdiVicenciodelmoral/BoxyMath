using TMPro;
using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public int R { get; private set; }
    public int C { get; private set; }

    public void SetPos(int r, int c)
    {
        R = r;
        C = c;
    }

    public void SetText(string s)
    {
        if (label != null) label.text = s;
    }

    // Skeleton stub: later we’ll implement cage borders.
    // Example future API:
    // public void SetBorders(bool top, bool right, bool bottom, bool left) { ... }
}
