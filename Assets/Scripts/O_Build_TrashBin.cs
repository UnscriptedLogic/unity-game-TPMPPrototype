using UnityEngine;
using UnscriptedEngine;

public class O_Build_TrashBin : O_Build
{
    [SerializeField] private InputNode inputNode;

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inputNode.TryGetBuildItem(out O_BuildItem buildItem))
        {
            BuildBehaviours.ConsumeItem(this, buildItem);
            Destroy(buildItem.gameObject);
        };
    }
}