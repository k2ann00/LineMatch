using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public float spacing = 1.0f;
    public GameObject dotPrefab;
    public GameObject horizontalLinePrefab;
    public GameObject verticalLinePrefab;
    public GameManager gameManager;

    private int gridSizeX;
    private int gridSizeY;

    private Dot[,] dots;
    private Dot selectedDot;
    private List<Dot> neighborDots = new List<Dot>();
    private HashSet<Vector2Int> completedSquares = new HashSet<Vector2Int>();

    void Start()
    {
        gridSizeX = SetupManager.Instance.gridX;
        gridSizeY = SetupManager.Instance.gridY;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        dots = new Dot[gridSizeX, gridSizeY];
        float offsetX = (gridSizeX - 1) * spacing * 0.5f;
        float offsetY = (gridSizeY - 1) * spacing * 0.5f;

        for (int y = 0; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                Vector3 dotPos = new Vector3(x * spacing - offsetX, y * spacing - offsetY, 0);
                GameObject dotObj = Instantiate(dotPrefab, dotPos, Quaternion.identity, transform);

                Dot dot = dotObj.GetComponent<Dot>();
                dot.Init(x, y, 0, this); // id 0 standart, sınır için farklı verilebilir
                dots[x, y] = dot;
            }
        }
    }

    public void DotSelected(Dot dot)
    {
        if (selectedDot != null && neighborDots.Contains(dot))
        {
            DrawLine(selectedDot, dot);
            ResetSelection();
            return;
        }

        ResetSelection();
        selectedDot = dot;
        selectedDot.SetSelected(true);
        HighlightNeighbors(dot);
    }

    private void HighlightNeighbors(Dot dot)
    {
        neighborDots.Clear();
        int x = dot.x;
        int y = dot.y;

        if (y + 1 < gridSizeY) { neighborDots.Add(dots[x, y + 1]); dots[x, y + 1].SetHighlight(true); }
        if (y - 1 >= 0) { neighborDots.Add(dots[x, y - 1]); dots[x, y - 1].SetHighlight(true); }
        if (x + 1 < gridSizeX) { neighborDots.Add(dots[x + 1, y]); dots[x + 1, y].SetHighlight(true); }
        if (x - 1 >= 0) { neighborDots.Add(dots[x - 1, y]); dots[x - 1, y].SetHighlight(true); }
    }

    private void ResetSelection()
    {
        if (selectedDot != null) selectedDot.SetSelected(false);
        foreach (Dot d in neighborDots) d.SetHighlight(false);
        neighborDots.Clear();
        selectedDot = null;
    }

    private void DrawLine(Dot a, Dot b)
    {
        if (!gameManager.CanDrawLine()) return;

        Vector3 pos = (a.transform.position + b.transform.position) / 2f;
        GameObject line;

        if (a.x == b.x) // Dikey çizgi
        {
            line = Instantiate(verticalLinePrefab, pos, Quaternion.identity, transform);
            line.transform.localScale = new Vector3(0.1f, spacing, 1);
            if (a.y < b.y) { a.UpLineActive = true; b.DownLineActive = true; }
            else { a.DownLineActive = true; b.UpLineActive = true; }
        }
        else if (a.y == b.y) // Yatay çizgi
        {
            line = Instantiate(horizontalLinePrefab, pos, Quaternion.identity, transform);
            line.transform.localScale = new Vector3(spacing, 0.1f, 1);
            if (a.x < b.x) { a.RightLineActive = true; b.LeftLineActive = true; }
            else { a.LeftLineActive = true; b.RightLineActive = true; }
        }

        Debug.Log($"LineDrawn -> ({a.x},{a.y}) <-> ({b.x},{b.y})");

        // Kare kontrolü: çizgiyi çeken oyuncuyu gönderiyoruz
        Player currentPlayer = gameManager.GetCurrentPlayer();
        CheckForSquares(currentPlayer);

        // Çizgi hakkını azalt
        gameManager.LineDrawn();
    }

    private void CheckForSquares(Player player)
    {
        for (int y = 1; y < gridSizeY; y++)
        {
            for (int x = 0; x < gridSizeX - 1; x++)
            {
                Dot A = dots[x, y];
                Dot B = dots[x + 1, y];
                Dot C = dots[x, y - 1];
                Dot D = dots[x + 1, y - 1];

                bool top = A.RightLineActive && B.LeftLineActive;
                bool bottom = C.RightLineActive && D.LeftLineActive;
                bool left = A.DownLineActive && C.UpLineActive;
                bool right = B.DownLineActive && D.UpLineActive;

                if (top && bottom && left && right)
                {
                    Vector2Int squareCoord = new Vector2Int(x, y - 1);

                    if (completedSquares.Contains(squareCoord))
                        continue;

                    completedSquares.Add(squareCoord);

                    Debug.Log($"✅ Yeni Kare! Koordinatlar: {squareCoord} - Oyuncu: {player.name}");
                    CreateSquareVisual(x, y - 1, player);
                }
            }
        }
    }

    private void CreateSquareVisual(int x, int y, Player player)
    {
        Vector3 pos = (dots[x, y].transform.position + dots[x + 1, y + 1].transform.position) / 2f;
        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
        square.transform.position = pos;
        square.transform.localScale = new Vector3(spacing, spacing, 1);
        square.transform.SetParent(transform);
        square.GetComponent<Renderer>().material.color = player.color;

        Debug.Log($"🎯 Skor eklendi -> Oyuncu: {player.name}, Önceki Skor: {player.score}");
        gameManager.AddScore(player, 1);
        Debug.Log($"📊 Yeni Skor -> Oyuncu: {player.name}, Skor: {player.score}");
    }
}
