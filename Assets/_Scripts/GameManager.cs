using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int currentPlayer = 1; // 1 veya 2
    private int remainingLines;

    public void RollDice()
    {
        remainingLines = Random.Range(1, 7);
        Debug.Log($"Oyuncu {currentPlayer} {remainingLines} çizgi çekebilir");
    }

    public bool CanDrawLine()
    {
        return remainingLines > 0;
    }

    public void LineDrawn()
    {
        remainingLines--;
        if (remainingLines <= 0)
        {
            SwitchPlayer();
        }
    }

    private void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        RollDice();
    }
}

