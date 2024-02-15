using System;
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

    [SerializeField] private GameObject buttonParent;
    [SerializeField] private GameObject clockInGO;
    [SerializeField] private GameObject finishProjectGO;

    private GM_TutorialGameMode tutorialGameMode;

    private UButtonComponent clockInComponent;
    private UButtonComponent finishProjectComponent;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        tutorialGameMode = GameMode.CastTo<GM_TutorialGameMode>();

        Bind<UButtonComponent>("pause", OnPause);
        Bind<UButtonComponent>("testfactory", tutorialGameMode.TestFactory);
        Bind("clockin", tutorialGameMode.ClockIn, out clockInComponent);
        Bind("finishproject", tutorialGameMode.FinishProject, out finishProjectComponent);

        clockInComponent.gameObject.SetActive(false);
        finishProjectComponent.gameObject.SetActive(false);

        LoadRequirementsUI();
    }

    private void OnPause()
    {
        GameMode.GetPlayerController().AttachUIWidget(pauseHUD);
    }

    private void LoadRequirementsUI()
    {
        for (int i = requirementsParent.childCount - 1; i >= 1; i--)
        {
            Destroy(requirementsParent.GetChild(i).gameObject);
        }

        Requirement tutorialRequirements = new Requirement();
        tutorialRequirements.SetDetails("Tutorial Requirement", "Keep feeding the deployer its required page dictated by the rectangle above it.", "Deployers represent a deployed instance of a website.");

        List<Requirement> requirements = new List<Requirement>()
        {
            tutorialRequirements
        };

        for (int i = 0; i < requirements.Count; i++)
        {
            RequirementTMP requirementTMP = Instantiate(requirementPrefab, requirementsParent);
            requirementTMP.Initialize(this, requirements[i].GameDescription, requirements[i].IsConditionMet);
        }
    }

    public void ShowClockIn()
    {
        buttonParent.SetActive(true);
        clockInGO.SetActive(true);
    }

    public void UseRateRequirement()
    {
        for (int i = requirementsParent.childCount - 1; i >= 1; i--)
        {
            Destroy(requirementsParent.GetChild(i).gameObject);
        }

        DeployersMeetRateRequirement rateTutorialRequirements = new DeployersMeetRateRequirement();
        rateTutorialRequirements.SetDetails("Tutorial Requirement", "Feed the deployer it's required rate", "Websites need to be able to cater to users around the world without slowing down the website");

        List<Requirement> requirements = new List<Requirement>()
        {
            rateTutorialRequirements
        };

        for (int i = 0; i < requirements.Count; i++)
        {
            RequirementTMP requirementTMP = Instantiate(requirementPrefab, requirementsParent);
            requirementTMP.Initialize(this, requirements[i].GameDescription, requirements[i].IsConditionMet);
        }
    }

    public void FinishProjectRequirement()
    {
        for (int i = requirementsParent.childCount - 1; i >= 1; i--)
        {
            Destroy(requirementsParent.GetChild(i).gameObject);
        }

        Requirement rateTutorialRequirements = new Requirement();
        rateTutorialRequirements.SetDetails("Tutorial Requirement", "Well done. Now click 'Finish Project'", "You've done well. Tutorial Completed");

        List<Requirement> requirements = new List<Requirement>()
        {
            rateTutorialRequirements
        };

        for (int i = 0; i < requirements.Count; i++)
        {
            RequirementTMP requirementTMP = Instantiate(requirementPrefab, requirementsParent);
            requirementTMP.Initialize(this, requirements[i].GameDescription, requirements[i].IsConditionMet);
        }
    }

    public void ShowFinishProject()
    {
        finishProjectGO.SetActive(true);
    }
}