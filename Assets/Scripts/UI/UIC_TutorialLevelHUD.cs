using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class UIC_TutorialLevelHUD : UCanvasController
{
    [Header("Canvases")]
    [SerializeField] private UIC_PauseMenu pauseHUD;

    [Header("Requirements")]
    [SerializeField] private Transform requirementsParent;
    [SerializeField] private RequirementTMP requirementPrefab;

    private GM_TutorialGameMode tutorialGameMode;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        tutorialGameMode = GameMode.CastTo<GM_TutorialGameMode>();

        Bind<UButtonComponent>("pause", OnPause);
        Bind<UButtonComponent>("testfactory", tutorialGameMode.TestFactory);

        LoadRequirementsUI();
    }

    private void OnPause()
    {
        GameMode.GetPlayerController().AttachUIWidget(pauseHUD);
    }

    private void LoadRequirementsUI()
    {
        List<Requirement> requirements = new List<Requirement>();

        for (int i = 0; i < requirements.Count; i++)
        {
            RequirementTMP requirementTMP = Instantiate(requirementPrefab, requirementsParent);
            requirementTMP.Initialize(this, requirements[i].GameDescription, requirements[i].IsConditionMet);
        }
    }
}