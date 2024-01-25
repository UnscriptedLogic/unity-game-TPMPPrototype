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
    [SerializeField] private Transform videoTransform;

    [SerializeField] private Ease ease;

    [SerializeField] private GameObject gnomeObject;
    [SerializeField] private AudioSource gnomeAudioSource;

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

        O_Build.OnObjectBuilt += O_Build_OnBuildCreated;
    }

    private void OnShow()
    {
        modalParent.DOMoveX(activeTransform.position.x, 0.5f).SetEase(ease).OnComplete(() => showBtn.gameObject.SetActive(false));

        gnomeObject.SetActive(false);
        if (UnityEngine.Random.Range(0f, 100f) >= 69)
        {
            gnomeObject.SetActive(true);
            gnomeAudioSource.Play();
        }
    }

    private void OnHide()
    {
        modalParent.DOMoveX(deactiveTransform.position.x, 0.5f).SetEase(ease).OnComplete(() => showBtn.gameObject.SetActive(true));
    }

    public void InitializeModal(string id)
    {
        (int frameworkIndex, int buildIndex) = builds.GetBuildableFromID(id);
        BuildSO build = builds.Frameworks[frameworkIndex].DataSet[buildIndex];

        buildName.TMP.text = build.DisplayName;
        buildDesc.TMP.text = build.Description;
        functionalityVideoPlayer.clip = build.FunctionalityVideo;

        OnShow();
    }

    private void O_Build_OnBuildCreated(object sender, System.EventArgs e)
    {
        OnHide();
    }

    protected override void OnDestroy()
    {
        O_Build.OnObjectBuilt -= O_Build_OnBuildCreated;

        base.OnDestroy();
    }
}