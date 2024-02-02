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

    public static void ConsumeItem(O_Build build, O_BuildItem item, InputNode inputNode)
    {
        item.gameObject.SetActive(false);
        item.transform.position = build.transform.position;
        item.transform.SetParent(build.transform);

        inputNode.Inventory.Add(item);
    }

    public static void ConsumeItem(O_Build build, O_BuildItem item)
    {
        item.gameObject.SetActive(false);
        item.transform.position = build.transform.position;
        item.transform.SetParent(build.transform);

        build.Inventory.Add(item);
    }

    public static bool TryDispenseItemFromInventory(OutputNode outputNode, InputNode inputNode)
    {
        O_BuildItem item = inputNode.Inventory[0];
        item.transform.position = outputNode.Transform.position;
        item.gameObject.SetActive(true);

        inputNode.Inventory.Remove(item);
        return true;
    }

    public static void CreateBuildItem(O_BuildItem buildItem, OutputNode outputNode)
    {
        if (!outputNode.IsConnected) return;

        O_BuildItem item = buildItem;
        item.transform.position = outputNode.Transform.position;
        item.gameObject.SetActive(true);
    }
}
