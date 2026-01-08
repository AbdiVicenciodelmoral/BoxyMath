using TMPro;
using UnityEngine;

public class GridSmokeTest : MonoBehaviour
{
    [SerializeField] private Transform gridRoot;     // drag GridPanel here
    [SerializeField] private GameObject cellPrefab;  // drag Cell.prefab here

    void Start()
    {
        int n = 9;
        int v = 1;

        for (int i = 0; i < n * n; i++)
        {
            var go = Instantiate(cellPrefab, gridRoot);

            // find the TMP label inside the cell and set text
            var tmp = go.GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = v.ToString();

            v = (v % n) + 1;
        }
    }
}
