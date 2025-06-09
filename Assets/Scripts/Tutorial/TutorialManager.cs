using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialGO;
    [SerializeField] private GameObject coinMenu;
    [SerializeField] private Animator gameAnim;
    [SerializeField] private BooleanSO isTuto;
    [SerializeField] private BooleanSO canMove;
    [SerializeField] private string tuto;
    [SerializeField] private string tuto2;
    [SerializeField] private string tuto3;
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private Image handSprite;
    [SerializeField] private Sprite[] spriteArray;
    [SerializeField] private Button nextButton;
    [SerializeField] private FloatSO tutoPart;

    private int currentStep = 0;
    private bool canClick = true;

    private void Start()
    {
        nextButton.onClick.AddListener(OnNextButtonClicked);

        if(coinMenu != null)
            coinMenu.SetActive(false);

        if (isTuto.value)
        {
            tutorialGO.SetActive(true);

            if (tutoPart.value == 1)
            {
                gameAnim.Play(tuto);
                StartCoroutine(TutorialSequence());
            }
            if (tutoPart.value == 2)
            {
                gameAnim.Play(tuto2);
                StartCoroutine(TutorialSequence2());
            }
            if (tutoPart.value == 3)
            {
                gameAnim.Play(tuto3);
                StartCoroutine(TutorialSequence3());
            }
        }
        else { tutorialGO.SetActive(false); }


    }

    IEnumerator TutorialSequence()
    {
        textMesh.text = "";
        ChangeSprite(0);

        yield return new WaitForSeconds(1.35f);
        ChangeText("Hey!");
        ChangeSprite(0);
        currentStep++;
        canClick = true; 

        yield return new WaitUntil(() => currentStep > 1 && canClick);
        canClick = false; 
        ChangeText("I see it's your first time here...");
        canClick = true; 

        yield return new WaitUntil(() => currentStep > 2 && canClick);
        canClick = false;
        ChangeText("So I came to give you a hand!");
        ChangeSprite(1);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 3 && canClick);
        canClick = false;
        ChangeText("");
        ChangeSprite(2);

        yield return new WaitForSeconds(0.3f);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(1.5f);
        ChangeText("Oh..");
        ChangeSprite(0);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 4 && canClick);
        canClick = false;
        ChangeText("Looks like you're out of money...");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 5 && canClick);
        canClick = false;
        ChangeText("But that's okay! Let's play!");
        ChangeSprite(3);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 6 && canClick);
        canClick = false;
        ChangeText("");
        ChangeSprite(2);

        yield return new WaitForSeconds(0.3f);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(0.7f);
        ChangeText("Click here!");
        canClick = true;
        nextButton.gameObject.SetActive(false);

        yield return new WaitUntil(() => currentStep > 7 && canClick);
        canClick = false;
        ChangeText("");

        yield return new WaitForSeconds(0.3f);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(0.7f);
        ChangeText("Now here!");
    }

    IEnumerator TutorialSequence2()
    {
        canMove.value = false;
        textMesh.text = "";
        ChangeSprite(0);

        yield return new WaitForSeconds(1.35f);
        ChangeText("Hey again!");
        ChangeSprite(0);
        currentStep++;
        canClick = true;

        yield return new WaitUntil(() => currentStep > 1 && canClick);

        canClick = false;
        ChangeText("This might all look a bit complicated...");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 2 && canClick);

        canClick = false;
        ChangeText("But don't worry! It's actually super simple!");
        ChangeSprite(1);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 3 && canClick);

        canClick = false;
        ChangeText("Here, you can organize some objects and earn coins for it!");
        ChangeSprite(3);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 4 && canClick);
        canClick = false;
        ChangeText("");
        ChangeSprite(2);

        yield return new WaitForSeconds(0.3f);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(0.9f);
        ChangeText("Up here, you can see which objects you need to collect.");

        canClick = true;

        yield return new WaitUntil(() => currentStep > 5 && canClick);
         canClick = false;
        ChangeText("To collect them, just match them in groups of 3 or more!");
        ChangeSprite(3);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 6 && canClick);
        canClick = false;
        ChangeText("");

        yield return new WaitForSeconds(0.3f);
        ChangeSprite(0);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(0.9f);
        ChangeText("And to do that, all you need is a swipe!");

        yield return new WaitForSeconds(2f);

        ChangeText("");
        yield return new WaitForSeconds(.3f);
        ChangeSprite(5);
        gameAnim.SetTrigger("Tuto");
        nextButton.gameObject.SetActive(false);
        canMove.value = true;
        AudioManager.Instance.PlaySFX("Swipe");

        yield return new WaitForSeconds(1f);
        ChangeText("Like this!");
        ChangeSprite(0);

        yield return new WaitUntil(() => currentStep > 7);
        ChangeText("");

        yield return new WaitForSeconds(1.5f);
        ChangeSprite(3);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(1f);
        ChangeText("Now lets go Back to the setup!");
        tutoPart.value = 3;
    }

    IEnumerator TutorialSequence3()
    {
        textMesh.text = "";
        ChangeSprite(0);

        yield return new WaitForSeconds(1.35f);
        ChangeText("We're back!");
        ChangeSprite(0);
        currentStep++;
        canClick = true;

        yield return new WaitUntil(() => currentStep > 1 && canClick);
        canClick = false;
        ChangeText("Look! You have 20 coins now!");
        ChangeSprite(2);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 2 && canClick);
        canClick = false;
        ChangeText("Let's buy something with them!");
        ChangeSprite(1);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 3 && canClick);
        canClick = false;
        ChangeText("");

        yield return new WaitForSeconds(0.3f);
        ChangeSprite(2);
        gameAnim.SetTrigger("Tuto");

        yield return new WaitForSeconds(1f);
        ChangeText("Click here to open the Shop!");
        nextButton.gameObject.SetActive(false);
        canClick = true;

        yield return new WaitUntil(() => currentStep > 4 && canClick);

        yield return new WaitForSeconds(0.3f);
        ChangeText("");
        ChangeSprite(0);

        yield return new WaitForSeconds(1f);
        canClick = true;
        nextButton.gameObject.SetActive(true);
        ChangeText("Look at all the possibilities!");

        yield return new WaitUntil(() => currentStep > 5 && canClick);
        canClick = false;
        ChangeSprite(2);
        ChangeText("You can choose from different categories of items that fit your space.");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 6 && canClick);
        canClick = false;
        ChangeSprite(0);
        ChangeText("Pick something that matches your style!");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 7 && canClick);
        canClick = false;
        ChangeSprite(1);
        ChangeText("I think that's all I can teach you...");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 8 && canClick);
        canClick = false;
        ChangeText("Now I leave it in your hands! See you later!");
        canClick = true;

        yield return new WaitUntil(() => currentStep > 9 && canClick);
        ChangeSprite(0);
        ChangeText("");

        yield return new WaitForSeconds(0.3f);
        gameAnim.SetTrigger("Tuto");
        isTuto.value = false;
        nextButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
        tutorialGO.SetActive(false);
    }


    public void OnNextButtonClicked()
    {
        if (canClick)
        {
            currentStep++;
            nextButton.interactable = false; 
        }
    }

    public void Next()
    {
        currentStep++;
    }

    public void LoadMatch3()
    {
        if (isTuto.value)
        {
            gameAnim.SetTrigger("Tuto");
            tutoPart.value = 2;
        }
        else
        {
            gameAnim.Play("OpenSelectLevel");
        }
    }

    public void OpenShop()
    {
        if (isTuto.value)
        {
            Next();
        }
        else
        {
            gameAnim.Play("OpenShop");
        }
    }
    public void CloseShop()
    {
        if (isTuto.value)
        {
            return;
        }
        else
        {
            gameAnim.Play("CloseShop");
        }
    }
    public void OpenCoinShop()
    {
        if (!isTuto.value)
        {
            coinMenu.SetActive(true);
            coinMenu.transform.localScale = Vector3.zero;
            coinMenu.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void CloseCoinShop()
    {
        coinMenu.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            coinMenu.SetActive(false);
        });
    }

    private void ChangeText(string newText)
    {
        Vector3 originalScale = textMesh.transform.localScale;
        LeanTween.scale(textMesh.gameObject, new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z), 0.15f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                textMesh.text = newText;
                LeanTween.scale(textMesh.gameObject, originalScale, 0.15f)
                    .setEase(LeanTweenType.easeOutBounce)
                    .setOnComplete(() => nextButton.interactable = true); 
            });
    }

    private void ChangeSprite(int spriteIndex)
    {
        if (spriteIndex < 0 || spriteIndex >= spriteArray.Length || spriteArray[spriteIndex] == null)
            return;

        Vector3 originalScale = handSprite.transform.localScale;
        LeanTween.scale(handSprite.gameObject, new Vector3(originalScale.x * 1.2f, originalScale.y * 0.8f, originalScale.z), 0.15f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                handSprite.sprite = spriteArray[spriteIndex];
                LeanTween.scale(handSprite.gameObject, originalScale, 0.15f)
                    .setEase(LeanTweenType.easeOutBounce)
                    .setOnComplete(() => nextButton.interactable = true); 
            });
    }
}