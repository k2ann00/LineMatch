using UnityEngine;
using UnityEngine.EventSystems;

public class LineCandidate : MonoBehaviour, IPointerClickHandler
{
    private SpriteRenderer sr;
    private bool isActive = false;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.gray; // pasif
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isActive) return;

        ActivateLine();
    }

    private void ActivateLine()
    {
        isActive = true;
        sr.color = Color.black; // aktif çizgi
        Debug.Log("Çizgi aktifleþtirildi: " + gameObject.name);
    }
}
