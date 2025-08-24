using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSize = 5;
    public float spacing = 1.0f;
    public GameObject dotPrefab;
    public GameObject horizontalLinePrefab;
    public GameObject verticalLinePrefab;
    public GameManager gameManager;

    private Dot[,] dots;
    private Dot selectedDot;
    private List<Dot> neighborDots = new List<Dot>();

    void Start()
    {
        GenerateGrid();
        
    }

    void GenerateGrid()
    {
        dots = new Dot[gridSize, gridSize];
        float offset = (gridSize - 1) * spacing * 0.5f;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                Vector3 dotPos = new Vector3(x * spacing - offset, y * spacing - offset, 0);
                GameObject dotObj = Instantiate(dotPrefab, dotPos, Quaternion.identity, transform);

                Dot dot = dotObj.GetComponent<Dot>();

                int id = 0;
                if (y == gridSize - 1 || y == 0 || x == 0 || x == gridSize - 1)
                    id = 1;

                dot.Init(x, y, id, this);
                dots[x, y] = dot;
            }
        }
    }

    public void DotSelected(Dot dot)
    {
        // Eğer seçili dot varsa ve tıklanan dot neighbor ise → çizgi oluştur
        if (selectedDot != null && neighborDots.Contains(dot))
        {
            DrawLine(selectedDot, dot);
            ResetSelection();
            return;
        }

        // Normal seçim: önceki highlight’ları resetle
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

        // Yukarı
        if (y + 1 < gridSize)
        {
            neighborDots.Add(dots[x, y + 1]);
            dots[x, y + 1].SetHighlight(true);
        }

        // Aşağı
        if (y - 1 >= 0)
        {
            neighborDots.Add(dots[x, y - 1]);
            dots[x, y - 1].SetHighlight(true);
        }

        // Sağ
        if (x + 1 < gridSize)
        {
            neighborDots.Add(dots[x + 1, y]);
            dots[x + 1, y].SetHighlight(true);
        }

        // Sol
        if (x - 1 >= 0)
        {
            neighborDots.Add(dots[x - 1, y]);
            dots[x - 1, y].SetHighlight(true);
        }
    }

    private void ResetSelection()
    {
        if (selectedDot != null)
            selectedDot.SetSelected(false);

        foreach (Dot d in neighborDots)
            d.SetHighlight(false);

        neighborDots.Clear();
        selectedDot = null;
    }

    private void DrawLine(Dot a, Dot b)
    {
        Vector3 pos = (a.transform.position + b.transform.position) / 2f;

        Quaternion rotation = Quaternion.identity;


        GameObject line;


        gameManager.LineDrawn();

        if (!gameManager.CanDrawLine()) return;

        if (a.x == b.x) // Dikey çizgi
        {
            line = Instantiate(horizontalLinePrefab, pos, rotation, transform);
            Debug.Log("Dikey Çizgi");
            line.transform.localScale = new Vector3(0.1f, spacing, 1);
            line.transform.rotation = rotation; // veya 90 dereceye çevirme, prefab modeline göre
            if (a.y < b.y)
            {
                a.UpLineActive = true;
                b.DownLineActive = true;
            }
            else
            {
                a.DownLineActive = true;
                b.UpLineActive = true;
            }
        }
        else if (a.y == b.y) // Yatay çizgi
        {
            Debug.Log("Yatay Çizgi");
            line = Instantiate(verticalLinePrefab, pos, rotation, transform);
            line.transform.localScale = new Vector3(spacing, 0.1f, 1);
            line.transform.rotation = rotation; // yatay prefab için default
            if (a.x < b.x)
            {
                a.RightLineActive = true;
                b.LeftLineActive = true;
            }
            else
            {
                a.LeftLineActive = true;
                b.RightLineActive = true;
            }
        }
        CheckForSquares();
    }



    private void CheckForSquares()
    {
        for (int y = 1; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize - 1; x++)
            {
                Dot A = dots[x, y];       // üst sol
                Dot B = dots[x + 1, y];   // üst sağ
                Dot C = dots[x, y - 1];   // alt sol
                Dot D = dots[x + 1, y - 1]; // alt sağ

                bool top = A.RightLineActive && B.LeftLineActive;
                bool bottom = C.RightLineActive && D.LeftLineActive;
                bool left = A.DownLineActive && C.UpLineActive;
                bool right = B.DownLineActive && D.UpLineActive;

                if (top && bottom && left && right)
                {
                    CreateSquareVisual(x, y - 1);
                    // Burada GameManager’a puan eklemeyi de çağırabilirsin
                    // gameManager.AddScore(gameManager.currentPlayer);
                }
            }
        }
    }



    private void CreateSquareVisual(int x, int y)
    {
        // Kare prefab varsa instantiate edebilirsin, yoksa bir Quad oluştur
        Vector3 pos = (dots[x, y].transform.position + dots[x + 1, y + 1].transform.position) / 2f;
        GameObject square = GameObject.CreatePrimitive(PrimitiveType.Quad);
        square.transform.position = pos;
        square.transform.localScale = new Vector3(spacing, spacing, 1);
        square.GetComponent<Renderer>().material.color = Color.green;
        square.transform.SetParent(transform);
    }

}
