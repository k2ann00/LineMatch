using UnityEngine;
using UnityEngine.UI;

public class DiceSpriteRoll : MonoBehaviour
{
    public static DiceSpriteRoll Instance;
    public Sprite[] diceFaces;     // 1–6 yüzleri (manuel slice edip at)
    public Sprite[] rollAnimFrames; // En alttaki animasyon frame’leri
    public Image diceImage;

    public float rollAnimSpeed = 0.1f; // animasyon hýzý
    public float rollDuration = 1.2f;  // toplam süre

    private bool isRolling = false;

    public int diceRes;


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


    public void RollDice()
    {
        if (!isRolling)
            StartCoroutine(RollAnimation());
    }

    private System.Collections.IEnumerator RollAnimation()
    {
        isRolling = true;
        float timer = 0f;
        int animIndex = 0;

        while (timer < rollDuration)
        {
            // Zar dönme animasyonu (frame loop)
            diceImage.sprite = rollAnimFrames[animIndex];
            animIndex = (animIndex + 1) % rollAnimFrames.Length;

            timer += rollAnimSpeed;
            yield return new WaitForSeconds(rollAnimSpeed);
        }

        // Sonuç
        int tempResult = Random.Range(0, diceFaces.Length);
        diceImage.sprite = diceFaces[tempResult];

        Debug.Log("Zar sonucu: " + (tempResult + 1));
        DiceSetter(tempResult + 1);
        
        isRolling = false;
    }

    public void DiceSetter(int dice)
    {
        diceRes = dice + 1;
    }

    public int DiceGetter() {  return diceRes; }
}
