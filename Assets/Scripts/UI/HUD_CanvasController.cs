using System;
using TMPro;
using UnityEngine;
using UnscriptedEngine;

public class HUD_CanvasController : UCanvasController
{
    [Header("Pages")]
    [SerializeField] private GameObject defaultPage;
    [SerializeField] private GameObject buildPage;
    [SerializeField] private GameObject deletePage;

    [Header("Components")]
    [SerializeField] private SO_Builds builds;
    [SerializeField] private GameObject buildBtnPrefab;
    [SerializeField] private Transform buildBtnsParent;

    public event EventHandler<string> OnRequestingToBuild;
    public event EventHandler OnCloseBuildMenu;
    public event EventHandler<bool> OnDeleteBuildToggled;

    private GM_LevelManager levelManager;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        levelManager = GameMode.CastTo<GM_LevelManager>();

        Bind<UButtonComponent>("buildBtn", BuildBtnClicked);
        Bind<UButtonComponent>("closeBtn", DefaultBtnClicked);

        Bind<UButtonComponent>("deleteBtn", DeleteBtnClicked);
        Bind<UButtonComponent>("closeDeleteBtn", CloseDeletePageClicked);

        Bind<UButtonComponent>("clockin", levelManager.ClockIn);
        Bind<UButtonComponent>("finishproject", levelManager.FinishProject);
        Bind<UButtonComponent>("testfactory", levelManager.TestFactory);

        BindUI(ref GameMode.CastTo<GM_LevelManager>().daysLeft, "time", value => $"{value} days left to the deadline");
        BindUI(ref levelManager.energy, "energy", value => $"Test Factory - {value}/{levelManager.ResetEnergyAmount}");

        for (int i = 0; i < builds.DataSet.Count; i++)
        {
            UButtonComponent buttonComponent = Instantiate(buildBtnPrefab, buildBtnsParent).GetComponent<UButtonComponent>();
            buttonComponent.SetID(builds.DataSet[i].ID);
            buttonComponent.GetComponentInChildren<TextMeshProUGUI>().text = builds.DataSet[i].DisplayName;

            Bind<UButtonComponent>(builds.DataSet[i].ID, OnBuildableClicked);
        }

        buildPage.SetActive(false);
        deletePage.SetActive(false);

        levelManager.OnProjectCompleted += LevelManager_OnProjectCompleted;
    }

    private void LevelManager_OnProjectCompleted(object sender, EventArgs e)
    {
        defaultPage.gameObject.SetActive(false);
        buildPage.SetActive(false);
        deletePage.SetActive(false);

        GetUIComponent<UButtonComponent>("clockin").gameObject.SetActive(false);
        GetUIComponent<UButtonComponent>("finishproject").gameObject.SetActive(false);
        GetUIComponent<UButtonComponent>("testfactory").gameObject.SetActive(false);

        UnBindUI(ref GameMode.CastTo<GM_LevelManager>().daysLeft, "time");
        UnBindUI(ref levelManager.energy, "energy");
    }

    public void CloseDeletePageClicked()
    {
        defaultPage.SetActive(true);
        buildPage.SetActive(false);
        deletePage.SetActive(false);

        OnDeleteBuildToggled?.Invoke(this, false);
    }

    public void DeleteBtnClicked()
    {
        defaultPage.SetActive(false);
        buildPage.SetActive(false);
        deletePage.SetActive(true);

        OnDeleteBuildToggled?.Invoke(this, true);
    }

    public void BuildBtnClicked()
    {
        defaultPage.SetActive(false);
        buildPage.SetActive(true);
    }

    public void DefaultBtnClicked()
    {
        defaultPage.SetActive(true);
        buildPage.SetActive(false);

        OnCloseBuildMenu?.Invoke(this, EventArgs.Empty);
    }

    public void OnBuildableClicked(string id)
    {
        OnRequestingToBuild?.Invoke(this, id);
    }

    public override void OnWidgetDetached(ULevelObject context)
    {
        levelManager.OnProjectCompleted -= LevelManager_OnProjectCompleted;

        base.OnWidgetDetached(context);
    }
}
