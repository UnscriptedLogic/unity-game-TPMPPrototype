using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Build_StretchHorizontalModifier : O_Build_ModifierBase
{
    protected override void ForEveryAttachedComponent(O_BuildComponentItem itemComponent)
    {
        itemComponent.StretchHorizontal();
    }
}
