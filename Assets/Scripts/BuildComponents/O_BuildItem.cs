using UnityEngine;
using UnityEngine.Splines;
using UnscriptedEngine;

public class O_BuildItem : ULevelObject
{
    [SerializeField] private FrameworkCategory category = FrameworkCategory.VANILLA;
    [SerializeField] private string id;

    public string ID => id;
    public FrameworkCategory Category => category;
}