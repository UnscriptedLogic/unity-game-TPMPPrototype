using UnityEngine;

public class O_Build_ModifierBase : O_Build
{
    public enum Side
    {
        Width,
        Height,
    }

    [SerializeField] protected InputNode inputNode;
    [SerializeField] protected OutputNode outputNode;
    [SerializeField] protected int processTickDelay = 8;
    [SerializeField] protected int creationIteration;

    protected int _creationIteration;

    protected O_BuildComponent buildComponent
    {
        get
        {
            if (inputNode.isInventoryEmpty)
            {
                return null;
            }

            return inputNode.Inventory[0] as O_BuildComponent;
        }

        set
        {
            inputNode.Inventory.Add(value);
        }
    } 

    protected override void NodeTickSystem_OnTick(object sender, TickSystem.OnTickEventArgs e)
    {
        if (inPreview) return;

        for (int i = inputNode.Inventory.Count - 1; i >= 0; i--)
        {
            if (inputNode.Inventory[i] == null)
            {
                inputNode.Inventory.RemoveAt(i);
            }
        }

        if (inputNode.isInventoryEmpty)
        {
            if (inputNode.TryGetBuildComponent(out O_BuildComponent buildItem))
            {
                if (buildItem as O_BuildPage) return;

                BuildBehaviours.ConsumeItem(this, buildItem, inputNode);

                OnComponentRecieved(buildItem);

                for (int i = 0; i < buildItem.AttachedComponents.Count; i++)
                {
                    ForEveryAttachedComponent(buildItem.AttachedComponents[i]);
                }
            }
        }

        if (!inputNode.isInventoryEmpty)
        {
            if (!outputNode.IsConnected) return;

            if (levelBuildInterface.NodeTickSystem.HasTickedAfter(processTickDelay))
            {
                _creationIteration++;

                if (_creationIteration < creationIteration) return;

                if (!outputNode.IsSpawnAreaEmpty) return;

                OnComponentToDispense(inputNode.Inventory[0] as O_BuildComponent);

                BuildBehaviours.TryDispenseItemFromInventory(outputNode, inputNode);

                _creationIteration = 0;
            }
        }
    }

    public override bool IsAreaEmpty()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position + (Vector3)offset, cellSize, 0);
        for (int i = 0; i < colliders.Length; i++)
        {
            O_Build build = colliders[i].GetComponent<O_Build>();
            if (build == null) continue;
            if (colliders[i].gameObject == gameObject) continue;

            O_Build_ConveyorBelt conveyorBelt = build as O_Build_ConveyorBelt;
            if (conveyorBelt != null)
            {
                conveyorBelt.SplitConveyorBelt(this);
            }

            return conveyorBelt != null;
        }

        return true;
    }

    protected virtual void OnComponentRecieved(O_BuildComponent component) { }
    protected virtual void ForEveryAttachedComponent(O_BuildComponentItem itemComponent) { }
    protected virtual void OnComponentToDispense(O_BuildComponent component) { }
}