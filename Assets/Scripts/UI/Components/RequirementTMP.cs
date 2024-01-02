using UnityEngine;
using UnscriptedEngine;
using static UnscriptedEngine.UObject;

public class RequirementTMP : UTextComponent
{
    [Header("Extensions")]
    [SerializeField] private Color requirementMetColor;

    public void Initialize(UCanvasController context, string name, Bindable<bool> requirementMet)
    {
        InitializeUIComponent(context);

        TMP.text = $"- {name}";
        SetID(name);

        OnRequirementChanged(requirementMet.Value);
        requirementMet.OnValueChanged += OnRequirementChanged;
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
}
