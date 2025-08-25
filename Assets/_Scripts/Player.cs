using TMPro;
using UnityEngine;

[System.Serializable]
public class Player
{
    public int id;
    public string name;
    public int score;
    public int remainingLines;

    public Color color;
    public GameObject panel;           // Panel prefab instance
    public TextMeshProUGUI nameText;   // Player_Name
    public TextMeshProUGUI scoreText;  // Score
    public TextMeshProUGUI lineText;   // Kalan hak
    public GameObject glow;            // Glow image
}
