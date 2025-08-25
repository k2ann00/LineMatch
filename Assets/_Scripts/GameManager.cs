using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button rollDiceButton;
    public TextMeshProUGUI diceResultText;
    public GameObject playerPanelPrefab;
    public Transform[] playerPanelPositions;
    public Transform canvasTransform;

    private Color[] playerColors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    [Header("Gameplay")]
    public int playerCount = 2;

    void Start()
    {
        if (canvasTransform == null)
            canvasTransform = GameObject.Find("Canvas").transform;

        playerCount = SetupManager.Instance.playerCount;
        SetupPlayers();
        UpdateCurrentPlayerUI();

        rollDiceButton.onClick.AddListener(RollDice);
        rollDiceButton.interactable = true;
    }

    void SetupPlayers()
    {
        players.Clear();

        // Player sayýsýný max 4 ile sýnýrla
        playerCount = Mathf.Clamp(playerCount, 2, 4);

        for (int i = 0; i < playerCount; i++)
        {
            Player player = new Player();
            player.id = i + 1;
            player.name = "Oyuncu " + player.id;
            player.color = playerColors[i % playerColors.Length];

            // Panel slot kontrolü
            if (i >= playerPanelPositions.Length)
            {
                Debug.LogWarning($"Player slot yok! index: {i}, panelPositions.Length: {playerPanelPositions.Length}. Panel otomatik olarak sahnede oluþturulacak.");
                GameObject tempSlot = new GameObject("PlayerSlot_" + i, typeof(RectTransform));
                tempSlot.transform.SetParent(canvasTransform);
                tempSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 * i, -50); // örnek pozisyon
                playerPanelPositions = playerPanelPositions.Append(tempSlot.transform).ToArray();
            }

            Transform slot = playerPanelPositions[i];

            // Panel oluþtur
            player.panel = Instantiate(playerPanelPrefab, slot);
            RectTransform rt = player.panel.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            // UI referanslarý al
            player.nameText = player.panel.transform.Find("Player_Name").GetComponent<TextMeshProUGUI>();
            player.scoreText = player.panel.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            player.lineText = player.panel.transform.Find("Remaning_Lines").GetComponent<TextMeshProUGUI>();
            player.glow = player.panel.transform.Find("Glow").gameObject;

            // Deðerleri ata
            player.nameText.text = player.name;
            player.scoreText.text = "0";
            player.lineText.text = "0";
            player.glow.SetActive(false);

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

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public bool CanDrawLine()
    {
        return players[currentPlayerIndex].remainingLines > 0;
    }

    public void LineDrawn()
    {
        Player current = players[currentPlayerIndex];
        current.remainingLines--;
        current.lineText.text = current.remainingLines.ToString();

        if (current.remainingLines <= 0)
        {
            NextPlayerTurn();
        }
    }

    void NextPlayerTurn()
    {
        if (players.Count == 0) return;

        players[currentPlayerIndex].glow.SetActive(false);
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdateCurrentPlayerUI();
        rollDiceButton.interactable = true;
    }

    void UpdateCurrentPlayerUI()
    {
        if (players.Count == 0 || currentPlayerIndex >= players.Count) return;

        Player current = players[currentPlayerIndex];
        current.glow.SetActive(true);
        diceResultText.text = current.name + " sýrasý!";
    }

    public void AddScore(Player player, int amount)
    {
        player.score += amount;
        player.scoreText.text = player.score.ToString();
    }
}
