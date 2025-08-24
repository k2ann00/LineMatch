using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    public static SetupManager Instance;
    [Header("Grid Settings")]
    public TextMeshProUGUI textGridX;
    public TextMeshProUGUI textGridY;
    public Button buttonIncreaseX;
    public Button buttonDecreaseX;
    public Button buttonIncreaseY;
    public Button buttonDecreaseY;

    [Header("Player Settings")]
    public TextMeshProUGUI textPlayerCount;
    public Button buttonIncreasePlayer;
    public Button buttonDecreasePlayer;
    public Transform[] playerPanelPositions; // Sahnede yerleþtirilmiþ boþ objeler (max 4)


    [Header("Start Button")]
    public Button startButton;

    [Header("Game Scene Name")]
    public string gameSceneName = "GameScene";

    public int gridX = 5;
    public int gridY = 5;
    public int playerCount = 2;

    private const int minGrid = 4;
    private const int maxGrid = 16;
    private const int minPlayers = 1;
    private const int maxPlayers = 4;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        UpdateUI();

        buttonIncreaseX.onClick.AddListener(() => ChangeGridX(1));
        buttonDecreaseX.onClick.AddListener(() => ChangeGridX(-1));
        buttonIncreaseY.onClick.AddListener(() => ChangeGridY(1));
        buttonDecreaseY.onClick.AddListener(() => ChangeGridY(-1));

        buttonIncreasePlayer.onClick.AddListener(() => ChangePlayerCount(1));
        buttonDecreasePlayer.onClick.AddListener(() => ChangePlayerCount(-1));

        startButton.onClick.AddListener(StartGame);
    }

    private void ChangeGridX(int delta)
    {
        gridX = Mathf.Clamp(gridX + delta, minGrid, maxGrid);
        UpdateUI();
    }

    private void ChangeGridY(int delta)
    {
        gridY = Mathf.Clamp(gridY + delta, minGrid, maxGrid);
        UpdateUI();
    }

    private void ChangePlayerCount(int delta)
    {
        playerCount = Mathf.Clamp(playerCount + delta, minPlayers, maxPlayers);
        UpdateUI();
    }

    private void UpdateUI()
    {
        textGridX.text = gridX.ToString();
        textGridY.text = gridY.ToString();
        textPlayerCount.text = playerCount.ToString();
    }

    private void StartGame()
    {
        // Deðerleri GameManager veya statik deðiþkenler aracýlýðýyla aktar
        GameSetup.gridSizeX = gridX;
        GameSetup.gridSizeY = gridY;
        GameSetup.playerCount = playerCount;

        SceneManager.LoadScene(gameSceneName);
    }
}

// Statik aktarým için ayrý sýnýf
public static class GameSetup
{
    public static int gridSizeX = 5;
    public static int gridSizeY = 5;
    public static int playerCount = 2;
}
