using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRecievedItemGlint : MonoBehaviour
{
    [SerializeField] private Image glintImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color acceptedGlint;
    [SerializeField] private Color rejectedGlint;

    public void FlashSuccess() => Flash(acceptedGlint);
    public void FlashError() => Flash(rejectedGlint);

    private void Flash(Color color)
    {
        glintImage.color = color;
        canvasGroup.alpha = 1;
        transform.localScale = Vector3.one * 0.001f;
        canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.InSine);
        transform.DOScale(0.0011f, 0.35f).SetEase(Ease.OutExpo);
    }
}
