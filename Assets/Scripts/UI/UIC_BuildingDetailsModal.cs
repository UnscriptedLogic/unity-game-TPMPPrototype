using UnityEngine;
using UnityEngine.Video;
using UnscriptedEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class UIC_BuildingDetailsModal : UCanvasController
{
    [SerializeField] private BuildListSO builds;
    [SerializeField] private VideoPlayer functionalityVideoPlayer;

    [SerializeField] private Transform activeTransform;
    [SerializeField] private Transform deactiveTransform;
    [SerializeField] private Transform modalParent;
    [SerializeField] private Transform videoTransform;

    [SerializeField] private Ease ease;

    [SerializeField] private GameObject memeObject;
    [SerializeField] private TextMeshProUGUI memeTMP;
    [SerializeField] private Image memeImg;
    [SerializeField] private List<string> memeTexts;
    [SerializeField] private List<Sprite> memeImages;

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

        memeObject.SetActive(false);

        if (UnityEngine.Random.Range(0f, 100f) <= 2.3f)
        {
            memeObject.SetActive(true);
            memeTMP.gameObject.SetActive(false);
            memeImg.gameObject.SetActive(false);
            if (UnityEngine.Random.Range(0f, 100f) <= 50f)
            {
                memeTMP.text = memeTexts.GetRandomElement();
                memeTMP.gameObject.SetActive(true);
            }
            else
            {
                memeImg.sprite = memeImages.GetRandomElement();
                memeImg.gameObject.SetActive(true);
            }
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