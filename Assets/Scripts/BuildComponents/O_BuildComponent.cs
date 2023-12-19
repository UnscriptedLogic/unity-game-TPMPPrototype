using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_BuildComponent : O_BuildItem
{
    [Header("Build Components")]
    [SerializeField] private Transform canvasRoot;

    [SerializeField] private List<O_BuildComponentItem> attachedComponents = new List<O_BuildComponentItem>();

    public List<O_BuildComponentItem> AttachedComponents => attachedComponents;

    public void AttachComponent(O_BuildComponentItem buildComponent)
    {
        AttachedComponents.Add(buildComponent);

        buildComponent.transform.SetParent(canvasRoot);
        buildComponent.transform.localPosition = Vector3.zero;
    }

    public bool HasComponent(O_BuildComponentItem componentItem)
    {
        //for (int i = 0; i < attachedComponents.Count; i++)
        //{
        //    if (attachedComponents[i].id == componentItem.id && attachedComponents[i].ModificationsID == componentItem.ModificationsID)
        //    {
        //        return true;
        //    }
        //}

        return false;
    }
}