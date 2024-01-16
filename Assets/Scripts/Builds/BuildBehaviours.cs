using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static O_Build;

public static class BuildBehaviours
{
    public class Timer
    {
        public float Time;
        public float TimeRemaining;

        public bool IsLooping;

        private bool called;

        public event Action OnTimeFinished;

        public Timer(float time, bool looping = true)
        {
            Time = time;
            IsLooping = looping;
        }

        public void Update()
        {
            if (TimeRemaining <= 0f)
            {
                if (IsLooping)
                {
                    OnTimeFinished?.Invoke();
                    TimeRemaining = Time;
                }
                else
                {
                    if (!called)
                    {
                        OnTimeFinished?.Invoke();
                        called = true;
                    }
                }
            }
            else
            {
                TimeRemaining -= UnityEngine.Time.deltaTime;
            }
        }

        public void Reset()
        {
            TimeRemaining = Time;
            called = false;
        }
    }

    [System.Serializable]
    public class InventorySlot
    {
        public string id;
        public int amount = 1;
        public int storageCap = 20;

        [HideInInspector] public List<O_BuildItem> items = new List<O_BuildItem>();

        public bool IsInventoryFull
        {
            get
            {
                return items.Count >= storageCap;
            }
        }
    }

    public static void ConsumeItem(O_Build build, O_BuildItem item, ref List<O_BuildItem> buildItems)
    {
        ConsumeItem(build, item);

        buildItems.Add(item);
    }

    public static void ConsumeItem(O_Build build, O_BuildItem item)
    {
        item.gameObject.SetActive(false);
        item.transform.position = build.transform.position;
    }

    public static void DispenseItemFromInventory(OutputNode outputNode, ref List<O_BuildItem> inventory)
    {
        if (!outputNode.IsConnected) return;

        O_BuildItem item = inventory[0];
        item.gameObject.SetActive(true);
        item.transform.position = outputNode.Transform.position;

        outputNode.GetBuildInfront().GiveItem(inventory[0]);

        inventory.RemoveAt(0);
    }

    public static void DispenseItemFromInventory(OutputNode outputNode, O_BuildItem item)
    {
        if (!outputNode.IsConnected) return;

        item.gameObject.SetActive(true);
        item.transform.position = outputNode.Transform.position;

        outputNode.GetBuildInfront().GiveItem(item);
    }

    public static void CreateBuildItem(O_BuildItem buildItem, OutputNode outputNode)
    {
        if (!outputNode.IsConnected) return;

        O_BuildItem item = buildItem;
        item.transform.position = outputNode.Transform.position;
        item.gameObject.SetActive(true);

        outputNode.GetBuildInfront().GiveItem(item);
    }
}
