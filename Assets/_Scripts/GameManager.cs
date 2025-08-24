using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button rollDiceButton;
    public TextMeshProUGUI diceResultText;
    public GameObject playerPanelPrefab;
    public Transform canvasTransform;

    public Transform[] playerPanelPositions;

    [Header("Gameplay")]
    public int playerCount = 2; // SetupManager'dan al
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    void Start()
    {
        SetupPlayers();
        UpdateCurrentPlayerUI();

        rollDiceButton.onClick.AddListener(RollDice);
        rollDiceButton.interactable = true;
    }

    void SetupPlayers()
    {
        for (int i = 0; i < playerCount; i++)
        {
            Player player = new Player();
            player.id = i + 1;
            player.name = "Oyuncu " + player.id;

            // Panel oluþtur, parent olarak Canvas yerine placeholder kullan
            Transform slot = playerPanelPositions[i]; // Placeholder
            RectTransform rect = slot.GetComponent<RectTransform>();
            player.panel = Instantiate(playerPanelPrefab, slot.position, Quaternion.identity, rect);

            // UI referanslarýný al
            player.nameText = player.panel.transform.Find("Player_Name").GetComponent<TextMeshProUGUI>();
            player.scoreText = player.panel.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            player.lineText = player.panel.transform.Find("Remaning_Lines").GetComponent<TextMeshProUGUI>();
            player.glow = player.panel.transform.Find("Glow").gameObject;

            // Deðerleri ata
            player.nameText.text = player.name;
            player.scoreText.text = "0";
            player.lineText.text = "0";
            player.glow.SetActive(false);

            // Paneli placeholder pozisyonuna hizala
            RectTransform rt = player.panel.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero; // parent placeholder'ýn pozisyonuna göre sýfýrla

            players.Add(player);
        }
    }


    private void RollDice()
    {
        Player current = players[currentPlayerIndex];
        int dice = Random.Range(1, 7);
        current.remainingLines = dice;

        current.lineText.text = dice.ToString();
        diceResultText.text = current.name + " Zar: " + dice;

        rollDiceButton.interactable = false;
    }

    public bool CanDrawLine()
    {
        return players[currentPlayerIndex].remainingLines > 0;
    }

    public void LineDrawn()
    {
        Player current = players[currentPlayerIndex];

        if (current.remainingLines > 0)
        {
            current.remainingLines--;
            current.lineText.text = current.remainingLines.ToString();

            if (current.remainingLines <= 0)
                NextPlayerTurn();
        }
    }

    void NextPlayerTurn()
    {
        players[currentPlayerIndex].glow.SetActive(false);

        currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;

        UpdateCurrentPlayerUI();
        rollDiceButton.interactable = true;
    }

    void UpdateCurrentPlayerUI()
    {
        Player current = players[currentPlayerIndex];
        current.glow.SetActive(true);
        diceResultText.text = current.name + " sýrasý!";
    }
}
