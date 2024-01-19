using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_BuildHUD : UCanvasController, IBuildHUD
{
    [Header("Canvases")]
    [SerializeField] private UIC_BuildingDetailsModal detailsModalPrefab;

    private UIC_BuildingDetailsModal detailsModal;

    [Header("Pages")]
    [SerializeField] protected GameObject buildPage;
    [SerializeField] protected GameObject deletePage;

    [Header("Components")]
    [SerializeField] protected BuildListSO builds;
    [SerializeField] protected GameObject buildBtnPrefab;
    [SerializeField] protected GameObject frameworkBtnPrefab;
    [SerializeField] protected Transform buildBtnsParent;
    [SerializeField] protected Transform buildFrameworkBtnsParent;

    private string lastFramework;

    public event EventHandler<string> OnRequestingToBuild;
    public event EventHandler<bool> OnDeleteBuildToggled;

    protected IFactoryValidation factoryValidationInterface;

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

        ShowFrameworkButtons();

        deletePage.SetActive(false);

        factoryValidationInterface.OnProjectCompleted += LevelManager_OnProjectCompleted;

        OnObjectCreated += ULevelObject_OnObjectCreated;
        OnObjectToBeDestroyed += HUD_CanvasController_OnObjectToBeDestroyed;
    }

    protected virtual void ShowFrameworkButtons()
    {
        for (int i = 0; i < builds.Frameworks.Count; i++)
        {
            SO_Builds framework = builds.Frameworks[i];

            //Framework button
            UButtonComponent frameworkBtn = Instantiate(frameworkBtnPrefab, buildBtnsParent).GetComponent<UButtonComponent>();
            frameworkBtn.GetComponentsInChildren<Image>()[1].sprite = framework.FrameworkIcon;
            frameworkBtn.SetID(framework.FrameworkName);
            frameworkBtn.GetComponentInChildren<TextMeshProUGUI>().text = framework.FrameworkName;
            frameworkBtn.InitializeUIComponent(this);

            Bind<UButtonComponent>(framework.FrameworkName, ShowFrameworkBuilds);
        }
    }

    protected virtual void ShowFrameworkBuilds(string id)
    {
        //Toggles the panel
        buildFrameworkBtnsParent.gameObject.SetActive(true);
        if (lastFramework == id)
        {
            if (buildFrameworkBtnsParent.gameObject.activeInHierarchy)
            {
                buildFrameworkBtnsParent.gameObject.SetActive(false);
                lastFramework = "";
                return;
            }
        }

        //Clear the parent just in case
        for (int i = buildFrameworkBtnsParent.childCount - 1; i >= 0; i--)
        {
            Destroy(buildFrameworkBtnsParent.GetChild(i).gameObject);
        }

        SO_Builds framework = GetFrameworkByID(id);

        //Individual Framework
        for (int i = 0; i < framework.DataSet.Count; i++)
        {
            BuildBtn buildBtn = Instantiate(buildBtnPrefab, buildFrameworkBtnsParent).GetComponent<BuildBtn>();
            buildBtn.Initialize(this, framework.DataSet[i]);
        }

        lastFramework = id;

    }

    protected SO_Builds GetFrameworkByID(string frameworkName)
    {
        for (int i = 0; i < builds.Frameworks.Count; i++)
        {
            if (builds.Frameworks[i].FrameworkName != frameworkName) continue;

            return builds.Frameworks[i];
        }

        Debug.LogWarning($"Something hella sus went wrong here. Tried to get id of {frameworkName}");
        return null;
    }

    protected void ULevelObject_OnObjectCreated(object sender, EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(false);
    }

    protected void HUD_CanvasController_OnObjectToBeDestroyed(object sender, EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(true);
    }

    protected void LevelManager_OnProjectCompleted(object sender, EventArgs e)
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
        if (detailsModal == null)
        {
            detailsModal = GameMode.GetPlayerController().AttachUIWidget(detailsModalPrefab);
        }

        detailsModal.InitializeModal(id);

        OnRequestingToBuild?.Invoke(this, id);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        if (detailsModal != null)
        {
            DettachUIWidget(detailsModal);
            Destroy(detailsModal.gameObject);
        }

        factoryValidationInterface.OnProjectCompleted -= LevelManager_OnProjectCompleted;

        OnObjectCreated -= ULevelObject_OnObjectCreated;
        OnObjectToBeDestroyed -= HUD_CanvasController_OnObjectToBeDestroyed;

        base.OnWidgetDetached(context);
    }
}
