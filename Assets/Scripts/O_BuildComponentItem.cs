using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_BuildComponentItem : ULevelObject
{
    public string id;
    private string modificationsID;

    [SerializeField] private RectTransform uiComponent;

    public string ModificationsID => modificationsID;

    public virtual void StretchVertical()
    {
        uiComponent.offsetMin = new Vector2(uiComponent.offsetMin.x, 0);
        uiComponent.offsetMax = new Vector2(uiComponent.offsetMax.x, 0);

        AppendModification("stretchvertical");
    }

    public virtual void StretchHorizontal()
    {
        uiComponent.offsetMin = new Vector2(0, uiComponent.offsetMin.y);
        uiComponent.offsetMax = new Vector2(0, uiComponent.offsetMax.y);

        AppendModification("stretchhorizontal");
    }

    private void AppendModification(string modificationName)
    {
        if (modificationsID != "")
        {
            modificationsID += "_";
        }

        modificationsID += modificationName;
    }
}
