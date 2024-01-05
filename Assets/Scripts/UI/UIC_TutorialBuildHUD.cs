using System.Collections.Generic;
using TMPro;
using UnscriptedEngine;

public class UIC_TutorialBuildHUD : UIC_BuildHUD
{
    private GM_TutorialGameMode tutorialGameMode;
    private List<string> availableBuilds;
    private string lastSelectedFrameworkID;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        tutorialGameMode = GameMode.CastTo<GM_TutorialGameMode>();
        availableBuilds = new List<string>();

        tutorialGameMode.OnSectionStarted += TutorialGameMode_OnSectionCompleted;
    }

    private void TutorialGameMode_OnSectionCompleted(object sender, GM_TutorialGameMode.OnSectionCompeletedEventArgs args)
    {
        availableBuilds.AddRange(args.buildsToAdd);

        if (!string.IsNullOrEmpty(lastSelectedFrameworkID))
        {
            ShowFrameworkBuilds(lastSelectedFrameworkID);
        }
    }

    protected override void ShowFrameworkButtons()
    {
        for (int i = 0; i < builds.Frameworks.Count; i++)
        {
            SO_Builds framework = builds.Frameworks[i];
         
            //Framework button
            UButtonComponent frameworkBtn = Instantiate(buildBtnPrefab, buildBtnsParent).GetComponent<UButtonComponent>();
            frameworkBtn.SetID(framework.FrameworkName);
            frameworkBtn.GetComponentInChildren<TextMeshProUGUI>().text = framework.FrameworkName;
            frameworkBtn.InitializeUIComponent(this);

            Bind<UButtonComponent>(framework.FrameworkName, ShowFrameworkBuilds);
        }
    }

    protected override void ShowFrameworkBuilds(string id)
    {        
        buildFrameworkBtnsParent.gameObject.SetActive(!buildFrameworkBtnsParent.gameObject.activeInHierarchy);

        lastSelectedFrameworkID = id;

        //Clear the parent just in case
        for (int i = buildFrameworkBtnsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buildFrameworkBtnsParent.GetChild(i).gameObject);
        }

        buildFrameworkBtnsParent.gameObject.SetActive(true);

        SO_Builds framework = GetFrameworkByID(id);

        //Individual Framework ah
        for (int i = 0; i < framework.DataSet.Count; i++)
        {
            if (!availableBuilds.Contains(framework.DataSet[i].ID)) continue;

            BuildBtn buildBtn = Instantiate(buildBtnPrefab, buildFrameworkBtnsParent).GetComponent<BuildBtn>();
            buildBtn.Initialize(this, framework.DataSet[i].DisplayName, framework.DataSet[i].ID);
        }
    }
}