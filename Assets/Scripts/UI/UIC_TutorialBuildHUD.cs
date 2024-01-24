using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_TutorialBuildHUD : UIC_BuildHUD
{
    [Header("Tutorial Extensions")]
    [SerializeField] private Transform newBuildPage;
    [SerializeField] private CanvasGroup notificationCanvasGroup;

    private GM_TutorialGameMode tutorialGameMode;
    private List<string> availableBuilds;
    private string lastSelectedFrameworkID;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        tutorialGameMode = GameMode.CastTo<GM_TutorialGameMode>();
        availableBuilds = new List<string>();

        tutorialGameMode.OnSectionStarted += TutorialGameMode_OnSectionCompleted;

        newBuildPage.gameObject.SetActive(false);
    }

    private void TutorialGameMode_OnSectionCompleted(object sender, GM_TutorialGameMode.OnSectionCompeletedEventArgs args)
    {
        availableBuilds.AddRange(args.buildsToAdd);

        if (args.buildsToAdd.Count > 0 && tutorialGameMode.CurrentSectionIndex > 0)
        {
            StartCoroutine(AnimateNotification());
        }

        if (!string.IsNullOrEmpty(lastSelectedFrameworkID))
        {
            ShowFrameworkBuilds(lastSelectedFrameworkID);
        }
    }

    protected override void ShowFrameworkButtons()
    {
        //for (int i = 0; i < builds.Frameworks.Count; i++)
        //{
        //    FrameworkSO framework = builds.Frameworks[i];
         
        //    //Framework button
        //    UButtonComponent frameworkBtn = Instantiate(frameworkBtnPrefab, buildBtnsParent).GetComponent<UButtonComponent>();
        //    frameworkBtn.GetComponentsInChildren<Image>()[1].sprite = framework.FrameworkIcon;
        //    frameworkBtn.SetID(framework.FrameworkName);
        //    frameworkBtn.GetComponentInChildren<TextMeshProUGUI>().text = framework.FrameworkName;
        //    frameworkBtn.InitializeUIComponent(this);

        //    Bind<UButtonComponent>(framework.FrameworkName, ShowFrameworkBuilds);
        //}
    }

    protected override void ShowFrameworkBuilds(string id)
    {        
        //buildFrameworkBtnsParent.gameObject.SetActive(!buildFrameworkBtnsParent.gameObject.activeInHierarchy);

        //lastSelectedFrameworkID = id;

        ////Clear the parent just in case
        //for (int i = buildFrameworkBtnsParent.childCount - 1; i >= 0; i--)
        //{
        //    Destroy(buildFrameworkBtnsParent.GetChild(i).gameObject);
        //}

        //buildFrameworkBtnsParent.gameObject.SetActive(true);

        //FrameworkSO framework = GetFrameworkByID(id);

        ////Individual Framework ah
        //for (int i = 0; i < framework.DataSet.Count; i++)
        //{
        //    if (!availableBuilds.Contains(framework.DataSet[i].ID)) continue;

        //    BuildBtn buildBtn = Instantiate(buildBtnPrefab, buildFrameworkBtnsParent).GetComponent<BuildBtn>();
        //    buildBtn.Initialize(this, framework.DataSet[i]);
        //}
    }

    private IEnumerator AnimateNotification()
    {
        notificationCanvasGroup.alpha = 1f;
        newBuildPage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        newBuildPage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        newBuildPage.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        newBuildPage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        newBuildPage.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        notificationCanvasGroup.DOFade(0f, 2f).OnComplete(() => newBuildPage.gameObject.SetActive(false));
    }
}