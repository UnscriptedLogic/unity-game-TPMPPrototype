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
    public class RequiredItem
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

    public static void ConsumeItem(O_Build build, O_BuildItem item, InputNode inputNode, ref List<O_BuildItem> buildItems)
    {
        item.gameObject.SetActive(false);

        item.SplineAnimator.Pause();
        item.SplineAnimator.ElapsedTime = 0;

        item.transform.position = build.transform.position;

        buildItems.Add(item);
    }

    public static void DispenseItemFromInventory(OutputNode outputNode, ref List<O_BuildItem> buildItems)
    {
        O_BuildItem item = buildItems[0];
        item.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
        item.gameObject.SetActive(true);

        buildItems.RemoveAt(0);
    }

    public static void CreateBuildItem(O_BuildItem buildItem, OutputNode outputNode)
    {
        O_BuildItem item = buildItem;
        item.SetSpline(outputNode.ConveyorBelt.ConveyorSplineContainer);
        item.gameObject.SetActive(true);
    }
}
