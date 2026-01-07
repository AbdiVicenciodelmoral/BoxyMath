// Assets/_Project/Scripts/Core/GameController.cs
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GridRenderer gridRenderer;
    public ViewMode startMode = ViewMode.GroupsAndNumbers;
    [Range(2, 12)] public int n = 9;

    void Start()
    {
        var gen = new PuzzleGenerator(n);
        gen.GenerateSolution();

        bool ok = gen.CheckLatinValidity(gen.Solution);
        Debug.Log($"Generated {n}x{n}. Latin-valid: {ok}");

        List<Cage> cages = gen.GroupingStub();
        Debug.Log($"Cages created (stub): {cages.Count}");

        // Optional: log first row to confirm values look right
        string row0 = "";
        for (int c = 0; c < n; c++) row0 += gen.Solution[0, c] + " ";
        Debug.Log("Row0: " + row0);
    }
}
