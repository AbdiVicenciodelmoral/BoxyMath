// Assets/_Project/Scripts/Core/GameController.cs
//
// This script is a MonoBehaviour, meaning:
// - It is attached to a GameObject in a Unity scene
// - Unity controls its lifetime
// - Unity automatically calls Start(), Update(), etc.
//
// Think of this as the "orchestrator":
// it kicks off puzzle generation and (eventually) tells other systems
// like the grid renderer what to display.

using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    /// <summary>
    /// Reference to a GridRenderer component in the scene.
    ///
    /// This is expected to be assigned in the Unity Inspector.
    /// GameController itself does NOT draw anything;
    /// it just passes data to systems that do.
    /// </summary>
    public GridRenderer gridRenderer;

    /// <summary>
    /// Determines how the puzzle is initially displayed.
    /// Example:
    /// - Game: normal play mode
    /// - Groups: show cage outlines
    /// - GroupsAndNumbers: show outlines + cage targets
    ///
    /// Because this is public, Unity shows it in the Inspector.
    /// </summary>
    public ViewMode startMode = ViewMode.GroupsAndNumbers;

    /// <summary>
    /// Size of the puzzle grid (NxN).
    ///
    /// [Range(2,12)] makes this a slider in the Inspector,
    /// which is extremely handy for quick testing.
    /// </summary>
    [Range(2, 12)] public int n = 9;

    /// <summary>
    /// Unity automatically calls Start() once,
    /// right before the first frame is rendered.
    ///
    /// This is where we:
    /// - generate the puzzle
    /// - verify it
    /// - (eventually) pass it to rendering / gameplay systems
    /// </summary>
    void Start()
    {
        // Create a new puzzle generator for an NxN puzzle.
        //
        // IMPORTANT:
        // PuzzleGenerator is NOT a MonoBehaviour.
        // We create it with 'new' like a normal C# object.
        var gen = new PuzzleGenerator(n, 12345);

        // Generate the solved Latin square grid.
        // After this call, gen.Solution is filled in.
        gen.GenerateSolution();

        // Sanity check: verify the generated grid is Latin-valid.
        // This is mostly for debugging during development.
        bool ok = gen.CheckLatinValidity(gen.Solution);

        // Log the result to the Unity Console.
        Debug.Log($"Generated {n}x{n}. Latin-valid: {ok}");

        // Generate cage groupings.
        //
        // IMPORTANT:
        // This currently uses GroupingStub(), which:
        // - covers the board
        // - does NOT enforce adjacency
        // - does NOT assign operations or targets
        //
        // This is temporary scaffolding so other systems can be tested.
        List<Cage> cages = gen.MakeSingleCellCages();
        Debug.Log($"Cages created (stub): {cages.Count}");


        // Hand the generated data to the GridRenderer so it can build/draw the UI grid.
        if (gridRenderer != null)
        {
            gridRenderer.SetPuzzle(gen.Solution, cages);
            gridRenderer.SetMode(startMode);
        }
        else
        {
            Debug.LogError("GameController: gridRenderer is not assigned in the Inspector!");
        }




        // Optional debugging output:
        // Print the first row of the solution grid to the console
        // so we can visually confirm that values look reasonable.
        //
        // Example output:
        // Row0: 3 1 9 4 7 6 2 5 8
        string row0 = "";
        for (int c = 0; c < n; c++)
            row0 += gen.Solution[0, c] + " ";

        Debug.Log("Row0: " + row0);

        // FUTURE (not implemented yet, but implied):
        //
        // gridRenderer.Initialize(n, cages, startMode);
        // gridRenderer.SetSolution(gen.Solution);
        //
        // This is typically where GameController hands off
        // data to rendering and gameplay systems.
    }
}
