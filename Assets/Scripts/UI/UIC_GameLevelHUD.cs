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

        Bind<UButtonComponent>("finishproject", levelManager.FinishProject);

        levelManager.OnProjectCompleted += FactoryValidationInterface_OnProjectCompleted;

        LoadRequirementsUI();

        OnObjectCreated += UIC_GameLevelHUD_OnObjectCreated;
        OnObjectToBeDestroyed += UIC_GameLevelHUD_OnObjectToBeDestroyed;
    }

    private void UIC_GameLevelHUD_OnObjectCreated(object sender, System.EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(false);
    }

    private void UIC_GameLevelHUD_OnObjectToBeDestroyed(object sender, System.EventArgs e)
    {
        if (!(sender as UIC_ProjectCompletionHUD)) return;

        gameObject.SetActive(true);
    }

    private void FactoryValidationInterface_OnProjectCompleted(object sender, System.EventArgs e)
    {
        GetUIComponent<UButtonComponent>("finishproject").gameObject.SetActive(false);
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