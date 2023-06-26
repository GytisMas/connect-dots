using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LevelPoint : MonoBehaviour, IPointerClickHandler
{
    public UnityAction<LevelPoint> onClick;

    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI levelNumber;
    [SerializeField] private float fadeOutTimeLength = 2f;
    [SerializeField] private Sprite pointInactive;
    [SerializeField] private Sprite pointActive;
    [SerializeField] Animator animator;

    private bool fadeOutIsOver;
    private float fadeOutStartTime;

    public string levelNum {
        get {
            return levelNumber.text;
        }
    }

    public void SetNumber(int num) {
        levelNumber.text = num.ToString();
    }

    public void SetAsClickedCorrect() {
        ChangeColorToActive();
        FadeOutText();
    }

    private void ClickPoint() {
        onClick?.Invoke(this);
    }

    private void ChangeColorToActive() {
        image.sprite = pointActive;
    }

    private void FadeOutText() {  
        animator.SetBool("FadeOutText", true);

        // Script version
        // StartCoroutine(FadeOutTextAnimation());
    }

    IEnumerator FadeOutTextAnimation() {
        fadeOutStartTime = Time.time;
        while (!fadeOutIsOver) {
            float timeDiff = Time.time - fadeOutStartTime;
            float timeDiffRatio = timeDiff / fadeOutTimeLength;
            Color newColor = levelNumber.color;
            newColor.a = 1f - timeDiffRatio;
            if (newColor.a <= 0f) {
                fadeOutIsOver = true;
                newColor.a = 0f;
            }
            levelNumber.color = newColor;
            yield return null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickPoint();
    }
}
