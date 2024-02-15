using TMPro;
using UnscriptedEngine;

public class UDetailsTextComponent : UTextComponent
{
    private UCustomTheme customTheme;

    public override void InitializeUIComponent(UCanvasController context)
    {
        this.context = context;

        if (ID != "")
        {
            context.AddUIComponent(this);
        }

        if (context.CanvasTheme != null)
        {
            theme = context.CanvasTheme;
            customTheme = theme as UCustomTheme;
        }

        tmp = GetComponent<TextMeshProUGUI>();
        tmp.font = customTheme.ExplanationModalFont;
    }
}
