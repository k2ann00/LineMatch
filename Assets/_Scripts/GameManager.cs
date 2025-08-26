using System.Collections;
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

    [Header("Impluse Variables")]
    public float minScaleVar = 0.9f;
    public float maxScaleVar = 1.2f;
    public float impluseSpeed = 1.5f;

    private Color[] playerColors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow };
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex = 0;

    [Header("Gameplay")]
    public int playerCount = 2;

    private Coroutine pulseCoroutine;

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
        playerCount = Mathf.Clamp(playerCount, 2, 4);

        for (int i = 0; i < playerCount; i++)
        {
            Player player = new Player();
            player.id = i + 1;
            player.name = "Player " + player.id;
            player.color = playerColors[i % playerColors.Length];

            if (i >= playerPanelPositions.Length)
            {
                Debug.LogWarning($"Player slot yok! index: {i}, panelPositions.Length: {playerPanelPositions.Length}. Panel otomatik olarak sahnede oluþturulacak.");
                GameObject tempSlot = new GameObject("PlayerSlot_" + i, typeof(RectTransform));
                tempSlot.transform.SetParent(canvasTransform);
                tempSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(150 * i, -50);
                playerPanelPositions = playerPanelPositions.Append(tempSlot.transform).ToArray();
            }

            Transform slot = playerPanelPositions[i];

            player.panel = Instantiate(playerPanelPrefab, slot);
            RectTransform rt = player.panel.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;

            player.nameText = player.panel.transform.Find("Player_Name").GetComponent<TextMeshProUGUI>();
            player.scoreText = player.panel.transform.Find("Score").GetComponent<TextMeshProUGUI>();
            player.lineText = player.panel.transform.Find("Remaning_Lines").GetComponent<TextMeshProUGUI>();

            player.nameText.text = player.name;
            player.scoreText.text = "Score" + "0";
            player.lineText.text = "Remaining Lines: " + "0";

            players.Add(player);
        }
    }

    private void RollDice()
    {
        Player current = players[currentPlayerIndex];
        DiceSpriteRoll.Instance.RollDice();
        int dice = DiceSpriteRoll.Instance.DiceGetter();
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
        current.lineText.text = "Remaining Lines: " + current.remainingLines.ToString();

        if (current.remainingLines <= 0)
        {
            NextPlayerTurn();
        }
    }

    void NextPlayerTurn()
    {
        if (players.Count == 0) return;

        // Eski oyuncunun panel scale'ini sýfýrla
        StopPulse();
        players[currentPlayerIndex].panel.transform.localScale = Vector3.one;

        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        UpdateCurrentPlayerUI();
        rollDiceButton.interactable = true;
    }

    void UpdateCurrentPlayerUI()
    {
        if (players.Count == 0 || currentPlayerIndex >= players.Count) return;

        Player current = players[currentPlayerIndex];
        diceResultText.text = current.name + " sýrasý!";

        // Yeni oyuncunun paneline animasyon baþlat
        StartPulse(current.panel.transform);
    }

    public void AddScore(Player player, int amount)
    {
        player.score += amount;
        player.scoreText.text = "Score: "  + player.score.ToString();
    }

    // --- Panel Animasyonu ---
    void StartPulse(Transform panel)
    {
        StopPulse();
        pulseCoroutine = StartCoroutine(PulseEffect(panel));
    }

    void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
    }

    IEnumerator PulseEffect(Transform target)
    {
        Vector3 minScale = Vector3.one * minScaleVar;
        Vector3 maxScale = Vector3.one * maxScaleVar;
        float impluseSpeed = 1f;

        while (true)
        {
            // büyüme
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * impluseSpeed;
                target.localScale = Vector3.Lerp(minScale, maxScale, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }

            // küçülme
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * impluseSpeed;
                target.localScale = Vector3.Lerp(maxScale, minScale, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }
    }
}
