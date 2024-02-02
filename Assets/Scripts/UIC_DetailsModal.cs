using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnscriptedEngine;

public class UIC_DetailsModal : UCanvasController
{
    [SerializeField] private RectTransform slideInTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Volume globalVolume;
    [SerializeField] private List<GameObject> pages;

    [SerializeField] private float fadeTime = 0.5f;
    [SerializeField] private float blurTime = 1f;

    private int currentPage;
    private DepthOfField depth;

    protected override void Awake()
    {
        canvasGroup.alpha = 0f;
        globalVolume.profile.TryGet(out depth);
        depth.focusDistance.value = 10f;
        slideInTransform.anchoredPosition3D = new Vector3(0f, -600, 0f);

        base.Awake();
    }

    public override void OnWidgetAttached(ULevelObject context)
    {
        GameMode.PauseGame();

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(true);
        }

        base.OnWidgetAttached(context);

        Bind<UButtonComponent>("dismiss", () => DettachUIWidget(this));
        Bind<UButtonComponent>("next", () =>
        {
            currentPage++;

            if (currentPage >= pages.Count)
            {
                DettachUIWidget(this);
                return;
            }

            ActivateCurrentPage();
        });

        ActivateCurrentPage();

        DOTween.To(() => depth.focusDistance.value, x => depth.focusDistance.value = x, 1f, blurTime).SetEase(Ease.OutExpo).SetUpdate(true);
        slideInTransform.DOAnchorPos3DY(0f, 1f).SetEase(Ease.OutExpo).SetUpdate(true);
        canvasGroup.DOFade(1f, fadeTime).SetEase(Ease.OutExpo).SetUpdate(true);
    }

    private void ActivateCurrentPage()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].gameObject.SetActive(false);
        }

        RectTransform rectTransform = pages[currentPage].GetComponent<RectTransform>();
        rectTransform.anchoredPosition3D -= Vector3.up * 50f;
        rectTransform.DOAnchorPos3DY(0f, 0.25f).SetUpdate(true);

        pages[currentPage].SetActive(true);

        CanvasGroup pageCanvasGroup = pages[currentPage].GetComponent<CanvasGroup>();
        if (pageCanvasGroup == null)
        {
            pageCanvasGroup = pages[currentPage].AddComponent<CanvasGroup>();
        }

        pageCanvasGroup.alpha = 0f;
        pageCanvasGroup.DOFade(1f, 0.25f).SetUpdate(true);
    }

    public override void DettachUIWidget<T>(T widget)
    {
        T uCanvasController = widget.GetComponent<T>();
        uCanvasController.OnWidgetDetached(this);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        DOTween.To(() => depth.focusDistance.value, x => depth.focusDistance.value = x, 10f, blurTime).SetUpdate(true).SetEase(Ease.InExpo);

        slideInTransform.DOAnchorPos3DY(-600f, 0.5f).SetEase(Ease.InExpo).SetUpdate(true);
        canvasGroup.DOFade(0f, fadeTime).SetUpdate(true).OnComplete(() =>
        {
            GameMode.ResumeGame();

            base.OnWidgetDetached(context);

            Destroy(gameObject);
        });
    }
}