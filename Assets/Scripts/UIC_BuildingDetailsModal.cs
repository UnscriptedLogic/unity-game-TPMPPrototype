using UnityEngine;
using UnityEngine.Video;
using UnscriptedEngine;
using DG.Tweening;

public class UIC_BuildingDetailsModal : UCanvasController
{
    [SerializeField] private BuildListSO builds;
    [SerializeField] private VideoPlayer functionalityVideoPlayer;

    [SerializeField] private Transform activeTransform;
    [SerializeField] private Transform deactiveTransform;
    [SerializeField] private Transform modalParent;

    [SerializeField] private Ease ease;

    private UTextComponent buildName;
    private UTextComponent buildDesc;

    private UButtonComponent showBtn;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        buildName = GetUIComponent<UTextComponent>("name");
        buildDesc = GetUIComponent<UTextComponent>("desc");

        Bind<UButtonComponent>("hide", OnHide);
        Bind<UButtonComponent>("show", OnShow);

        showBtn = GetUIComponent<UButtonComponent>("show");

        showBtn.gameObject.SetActive(false);
    }

    private void OnShow()
    {
        modalParent.DOMoveX(activeTransform.position.x, 0.5f).SetEase(ease).OnComplete(() => showBtn.gameObject.SetActive(false));
    }

    private void OnHide()
    {
        modalParent.DOMoveX(deactiveTransform.position.x, 0.5f).SetEase(ease).OnComplete(() => showBtn.gameObject.SetActive(true));
    }

    public void InitializeModal(string id)
    {
        (int frameworkIndex, int buildIndex) = builds.GetBuildableFromID(id);
        SO_Builds.BuildableData build = builds.Frameworks[frameworkIndex].DataSet[buildIndex];

        buildName.TMP.text = build.DisplayName;
        buildDesc.TMP.text = build.Description;
        functionalityVideoPlayer.clip = build.FunctionalityVideo;
    }
}