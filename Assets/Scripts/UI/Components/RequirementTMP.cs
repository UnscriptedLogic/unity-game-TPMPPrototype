using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnscriptedEngine;
using static UnscriptedEngine.UObject;

public class RequirementTMP : UTextComponent
{
    [Header("Extensions")]
    [SerializeField] private Color requirementMetColor;
    [SerializeField] private CanvasGroup notifyCanvasGroup;

    public void Initialize(UCanvasController context, string name, Bindable<bool> requirementMet)
    {
        InitializeUIComponent(context);

        TMP.text = $"- {name}";
        SetID(name);

        OnRequirementChanged(requirementMet.Value);
        requirementMet.OnValueChanged += OnRequirementChanged;

        StartCoroutine(NotifyOfRequirement());

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
    }

    private void OnRequirementChanged(bool value)
    {
        if (value)
        {
            TMP.color = requirementMetColor;
        }
        else
        {
            TMP.color = Color.white;
        }
    }

    public IEnumerator NotifyOfRequirement()
    {
        notifyCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(0.1f);

        notifyCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);

        notifyCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(0.1f);

        notifyCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);

        notifyCanvasGroup.alpha = 1;
        yield return new WaitForSeconds(0.1f);

        notifyCanvasGroup.alpha = 0;
        yield return new WaitForSeconds(0.1f);
    }
}
