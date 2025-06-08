using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private GameObject tutorialGO;
    [SerializeField] private Animator gameAnim;
    [SerializeField] private BooleanSO isTuto;
    [SerializeField] private string tuto;
    [SerializeField] private string tuto2;
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
                StartCoroutine(TutorialSequence());
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
        tutoPart.value = 2;
    }

    public void OnNextButtonClicked()
    {
        if (canClick)
        {
            currentStep++;
            nextButton.interactable = false; 
        }
    }

    public void LoadMatch3()
    {
        if (isTuto.value)
        {
            gameAnim.SetTrigger("Tuto");
        }
        else
        {
            gameAnim.Play("OpenSelectLevel");
        }
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