using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button rollDiceButton;
    public TextMeshProUGUI diceResultText;
    public TextMeshProUGUI remainingLinesText;

    public int currentPlayer = 1; // 1 veya 2
    private int remainingLines = 0;


    void Start()
    {
        rollDiceButton.onClick.AddListener(RollDice);
        UpdateUI();
        rollDiceButton.interactable = true;
    }

    private void RollDice()
    {
        int dice = Random.Range(1, 7); // 1�6 aras�
        remainingLines = dice;
        diceResultText.text = "Zar: " + dice;
        UpdateUI();

        // Zar at�ld� -> tekrar bas�lmamal�
        rollDiceButton.interactable = false;
    }

    public bool CanDrawLine()
    {
        return remainingLines > 0;
    }

    public void LineDrawn()
    {
        if (remainingLines > 0)
        {
            remainingLines--;
            UpdateUI();

            // Ancak t�m �izgiler bitince tekrar zar at�labilir
            if (remainingLines <= 0)
            {
                rollDiceButton.interactable = true;
            }
        }
    }

    private void UpdateUI()
    {
        remainingLinesText.text = "Kalan �izgi: " + remainingLines;
    }

    private void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        // Burada otomatik zar att�rmak istemiyorsan RollDice() �a��rma
        rollDiceButton.interactable = true;
        diceResultText.text = "Oyuncu " + currentPlayer + " s�ras�!";
    }
}
