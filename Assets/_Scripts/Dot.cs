using UnityEngine;
using UnityEngine.EventSystems;

public class Dot : MonoBehaviour, IPointerClickHandler
{
    public int x, y;
    public int id;
    private GridManager gridManager;
    private SpriteRenderer sr;
    private Color defaultColor = Color.white;
    private Color selectedColor = Color.yellow;
    private Color highlightColor = Color.green;
    private Color edgeColor = new Color(0.612f, 0.608f, 0.592f);
    private Color innerColor = new Color(0.659f, 0.659f, 0.643f);



    public bool UpLineActive = false;
    public bool DownLineActive = false;
    public bool LeftLineActive = false;
    public bool RightLineActive = false;

    public void Init(int x, int y, int id, GridManager manager)
    {
        this.x = x;
        this.y = y;
        this.id = id;
        this.gridManager = manager;

        sr = GetComponent<SpriteRenderer>();
        if (id == 1)
            sr.color = edgeColor;
        else
            sr.color = innerColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gridManager.DotSelected(this);
    }


    public void SetHighlight(bool highlight)
    {
        sr.color = highlight ? highlightColor : (id == 1 ? edgeColor : innerColor);
    }

    public void SetSelected(bool selected)
    {
        sr.color = selected ? selectedColor : (id == 1 ? edgeColor : innerColor);
    }

}
