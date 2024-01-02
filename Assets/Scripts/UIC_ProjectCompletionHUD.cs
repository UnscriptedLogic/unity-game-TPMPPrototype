using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;

public class UIC_ProjectCompletionHUD : UCanvasController
{
    [SerializeField] private Transform requirementParent;
    [SerializeField] private GameObject requirementPrefab;

    [SerializeField] private Color conditionMetColour;
    [SerializeField] private Color conditionNotMetColour;

    [SerializeField] private float requirementDisplayInterval = 1f;

    private C_PlayerController playerController;
    private GI_CustomGameInstance customGameInstance;

    public override void OnWidgetAttached(ULevelObject context)
    {
        base.OnWidgetAttached(context);

        customGameInstance = GameMode.GameInstance.CastTo<GI_CustomGameInstance>();

        Bind<UButtonComponent>("mainmenu", OnMainMenuPressed);
        Bind<UButtonComponent>("dismiss", OnDismissPressed);

        playerController = context.CastTo<C_PlayerController>();

        StartCoroutine(DisplayRequirements());
    }

    private IEnumerator DisplayRequirements()
    {
        //Clear the children under the requirement parent
        for (int i = requirementParent.childCount - 1; i >= 0; i--)
        {
            Destroy(requirementParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < customGameInstance.Project.Requirements.Count; i++)
        {
            yield return new WaitForSeconds(requirementDisplayInterval);

            Requirement requirement = customGameInstance.Project.Requirements[i];

            GameObject requirementObject = Instantiate(requirementPrefab, requirementParent);

            TextMeshProUGUI tmp = requirementObject.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = requirement.GameDescription;
            tmp.color = requirement.IsConditionMet.Value ? conditionMetColour : conditionNotMetColour;
            tmp.GetComponent<UTextComponent>().InitializeUIComponent(this);

            LayoutRebuilder.ForceRebuildLayoutImmediate(requirementParent.GetComponent<RectTransform>());
        }
    }

    private void OnMainMenuPressed()
    {
        playerController.ReturnToMainMenu();
    }

    private void OnDismissPressed()
    {
        DettachUIWidget(this);
        Destroy(gameObject);
    }
}