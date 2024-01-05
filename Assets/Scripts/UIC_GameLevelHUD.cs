using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class UIC_GameLevelHUD : UCanvasController
{
    [Header("Canvases")]
    [SerializeField] private UIC_PauseMenu pauseHUD;

    [Header("Requirements")]
    [SerializeField] private Transform requirementsParent;
    [SerializeField] private RequirementTMP requirementPrefab;

    private GM_LevelManager levelManager;
    private GI_CustomGameInstance customGameInstance;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        levelManager = GameMode.CastTo<GM_LevelManager>();
        customGameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        Bind<UButtonComponent>("pause", OnPause);

        Bind<UButtonComponent>("clockin", levelManager.ClockIn);
        Bind<UButtonComponent>("finishproject", levelManager.FinishProject);
        Bind<UButtonComponent>("testfactory", levelManager.TestFactory);

        BindUI(ref levelManager.daysLeft, "time", value => $"{value} days left to the deadline");
        BindUI(ref levelManager.energy, "energy", value => $"Test Factory - {value}/{levelManager.ResetEnergyAmount}");

        levelManager.OnProjectCompleted += FactoryValidationInterface_OnProjectCompleted;

        LoadRequirementsUI();
    }

    private void FactoryValidationInterface_OnProjectCompleted(object sender, System.EventArgs e)
    {
        GetUIComponent<UButtonComponent>("clockin").gameObject.SetActive(false);
        GetUIComponent<UButtonComponent>("finishproject").gameObject.SetActive(false);
        GetUIComponent<UButtonComponent>("testfactory").gameObject.SetActive(false);

        UnBindUI(ref levelManager.daysLeft, "time");
        UnBindUI(ref levelManager.energy, "energy");
    }

    private void LoadRequirementsUI()
    {
        List<Requirement> requirements = new List<Requirement>(customGameInstance.Project.Requirements);
        for (int i = 0; i < requirements.Count; i++)
        {
            RequirementTMP requirementTMP = Instantiate(requirementPrefab, requirementsParent);
            requirementTMP.Initialize(this, requirements[i].GameDescription, requirements[i].IsConditionMet);
        }
    }

    private void OnPause()
    {
        GameMode.GetPlayerController().AttachUIWidget(pauseHUD);
    }

    protected override void OnDestroy()
    {
        levelManager.OnProjectCompleted -= FactoryValidationInterface_OnProjectCompleted;

        base.OnDestroy();
    }
}