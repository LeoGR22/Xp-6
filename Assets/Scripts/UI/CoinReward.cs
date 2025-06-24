using System.Collections;
using System.Collections.Generic;
using CandyCoded.HapticFeedback;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CoinReward : MonoBehaviour
{
    [SerializeField] private GameObject pileOfCoins;
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private TextMeshProUGUI movesCounter;
    [SerializeField] private Vector2[] initialPos;
    [SerializeField] private Quaternion[] initialRotation;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private FloatSO movesSO;
    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private Vector2 spawnPosition;
    [SerializeField] private Vector2 targetPosition;

    private int currentCoinCount = 0;
    private Vector3 initialCounterScale;
    private int currentMoves = 10;

    private Timer timer;

    public void Start()
    {
        timer = FindObjectOfType<Timer>();
    }

    public void InitializeCoins()
    {
        timer.PauseTimer();

        currentCoinCount = 10; 
        counter.text = "10";

        currentMoves = Mathf.FloorToInt(timer.GetMovesLeft());
        movesCounter.text = currentMoves.ToString();
        initialCounterScale = counter.transform.parent.GetChild(0).transform.localScale; 

        int coinsAmount = Mathf.FloorToInt(timer.GetMovesLeft());
        if (coinsAmount == 0)
            return;

        foreach (Transform child in pileOfCoins.transform)
        {
            Destroy(child.gameObject);
        }

        initialPos = new Vector2[coinsAmount];
        initialRotation = new Quaternion[coinsAmount];

        for (int i = 0; i < coinsAmount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector2 spawnPos = spawnPosition + randomOffset;

            GameObject coin = Instantiate(coinPrefab, pileOfCoins.transform);
            RectTransform coinRect = coin.GetComponent<RectTransform>();
            coinRect.anchoredPosition = spawnPos;
            coinRect.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            initialPos[i] = coinRect.anchoredPosition;
            initialRotation[i] = coinRect.rotation;
        }

        CountCoins();
    }

    private void CountCoins()
    {
        pileOfCoins.SetActive(true);
        var delay = 0f;

        for (int i = 0; i < pileOfCoins.transform.childCount; i++)
        {
            Transform coin = pileOfCoins.transform.GetChild(i);
            coin.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);

            coin.GetComponent<RectTransform>().DOAnchorPos(targetPosition, 0.8f)
                .SetDelay(delay + 0.5f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    IncrementCoinCounter();
                });

            coin.DORotate(Vector3.zero, 0.5f).SetDelay(delay + 0.5f)
                .SetEase(Ease.Flash);

            coin.DOScale(0f, 0.3f).SetDelay(delay + 1.5f).SetEase(Ease.OutBack);

            delay += 0.1f;
        }
    }

    private void IncrementCoinCounter()
    {
        HapticFeedback.LightFeedback();
        AudioManager.Instance.PlaySFX("Coin");
        currentCoinCount += 5; 
        counter.text = currentCoinCount.ToString();

        currentMoves -= 1;
        movesCounter.text = currentMoves.ToString();
    }

    private void AnimateCounter()
    {
        var counterTransform = counter.transform.parent.GetChild(0).transform;
        counterTransform.DOScale(initialCounterScale * 1.05f, 0.3f)
            .SetLoops(2, LoopType.Yoyo) 
            .SetEase(Ease.InOutQuad) 
            .OnComplete(() => counterTransform.localScale = initialCounterScale); 
    }
}