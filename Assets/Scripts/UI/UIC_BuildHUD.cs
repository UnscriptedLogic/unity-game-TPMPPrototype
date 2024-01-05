using System;
using TMPro;
using UnityEngine;
using UnscriptedEngine;

public class UIC_BuildHUD : UCanvasController
{
    [Header("Pages")]
    [SerializeField] private GameObject buildPage;
    [SerializeField] private GameObject deletePage;

    [Header("Components")]
    [SerializeField] private BuildListSO builds;
    [SerializeField] private GameObject buildBtnPrefab;
    [SerializeField] private Transform buildBtnsParent;
    [SerializeField] private Transform buildFrameworkBtnsParent;

    public event EventHandler<string> OnRequestingToBuild;
    public event EventHandler<bool> OnDeleteBuildToggled;

    private IFactoryValidation factoryValidationInterface;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        factoryValidationInterface = GameMode as IFactoryValidation;
        if (factoryValidationInterface == null)
        {
            Debug.Log("GameMode doesn't use IFactoryValidation");
            return;
        }

        Bind<UButtonComponent>("deleteBtn", DeleteBtnClicked);
        Bind<UButtonComponent>("closeDeleteBtn", CloseDeletePageClicked);

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

        deletePage.SetActive(false);

        factoryValidationInterface.OnProjectCompleted += LevelManager_OnProjectCompleted;

        OnObjectCreated += ULevelObject_OnObjectCreated;
        OnObjectToBeDestroyed += HUD_CanvasController_OnObjectToBeDestroyed;
    }

    private void ShowFrameworkBuilds(string id)
    {
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
            BuildBtn buildBtn = Instantiate(buildBtnPrefab, buildFrameworkBtnsParent).GetComponent<BuildBtn>();
            buildBtn.Initialize(this, framework.DataSet[i].DisplayName, framework.DataSet[i].ID);
        }
    }

    private SO_Builds GetFrameworkByID(string frameworkName)
    {
        for (int i = 0; i < builds.Frameworks.Count; i++)
        {
            if (builds.Frameworks[i].FrameworkName != frameworkName) continue;

            return builds.Frameworks[i];
        }

        Debug.LogWarning($"Something hella sus went wrong here. Tried to get id of {frameworkName}");
        return null;
    }

    private void ULevelObject_OnObjectCreated(object sender, EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(false);
    }

    private void HUD_CanvasController_OnObjectToBeDestroyed(object sender, EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(true);
    }

    private void LevelManager_OnProjectCompleted(object sender, EventArgs e)
    {
        buildPage.SetActive(false);
        deletePage.SetActive(false);
    }

    public void CloseDeletePageClicked()
    {
        buildPage.SetActive(true);
        deletePage.SetActive(false);

        OnDeleteBuildToggled?.Invoke(this, false);
    }

    public void DeleteBtnClicked()
    {
        buildPage.SetActive(false);
        deletePage.SetActive(true);

        OnDeleteBuildToggled?.Invoke(this, true);
    }

    public void OnBuildableClicked(string id)
    {
        OnRequestingToBuild?.Invoke(this, id);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        factoryValidationInterface.OnProjectCompleted -= LevelManager_OnProjectCompleted;

        OnObjectCreated -= ULevelObject_OnObjectCreated;
        OnObjectToBeDestroyed -= HUD_CanvasController_OnObjectToBeDestroyed;

        base.OnWidgetDetached(context);
    }
}
