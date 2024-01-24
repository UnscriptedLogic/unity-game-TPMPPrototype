using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

public class O_Build_ManualDispenser : O_Build_GenericDispenser
{
    [System.Serializable]
    public class SpawnSets
    {
        [SerializeField] private O_BuildItem screenPrefab;
        [SerializeField] private int delayTick;

        public O_BuildItem ScreenPrefab => screenPrefab;
        public int DelayTick => delayTick;
    }

    [SerializeField] private List<SpawnSets> screenPrefabs;

    private int index;

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (index >= screenPrefabs.Count) return;

        if (buildSystemInterface.NodeTickSystem.HasTickedAfter(screenPrefabs[index].DelayTick))
        {
            BuildBehaviours.DispenseItemFromInventory(outputNode, Instantiate(screenPrefabs[index].ScreenPrefab, outputNode.Transform.position, Quaternion.identity));
            index++;
        }
    }
}