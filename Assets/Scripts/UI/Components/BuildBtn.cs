using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnscriptedEngine;

public class BuildBtn : UButtonComponent, IPointerEnterHandler, IPointerExitHandler
{
    private UIC_BuildHUD hudCanvas;
    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private Image iconImg;

    private float originalY;
    private float offsetY => originalY + 25f;

    public void Initialize(UCanvasController context, SO_Builds.BuildableData data)
    {
        InitializeUIComponent(context);

        SetID(data.ID);

        nameTMP.text = data.DisplayName;

        if (!context.CastTo<UIC_BuildHUD>()) return;
        hudCanvas = context.CastTo<UIC_BuildHUD>();

        TMPButton.onClick.AddListener(OnClick);

        originalY = -152;
        nameTMP.transform.localScale = Vector3.zero;

        iconImg.sprite = data.Icon;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOLocalMoveY(offsetY, 0.15f).SetEase(Ease.OutSine);
        nameTMP.transform.DOLocalMoveY(-64, 0.15f).SetEase(Ease.OutSine);
        nameTMP.transform.DOScale(1f, 0.15f).SetEase(Ease.OutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOLocalMoveY(originalY, 0.15f).SetEase(Ease.OutSine);
        nameTMP.transform.DOLocalMoveY(-20, 0.15f).SetEase(Ease.OutSine);
        nameTMP.transform.DOScale(0f, 0.15f).SetEase(Ease.OutSine);
    }

    private void OnClick()
    {
        hudCanvas.OnBuildableClicked(ID);
    }

    private void OnDestroy()
    {
        TMPButton.onClick.RemoveListener(OnClick);
    }
}
