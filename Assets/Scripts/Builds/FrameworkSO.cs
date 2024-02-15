using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public enum FrameworkCategory
{
    VANILLA,
    NONE
}

public enum BuildMachineCategory
{
    DISPENSER,
    BUILDER,
    PRODUCER,
    MODIFIER,
    UTILITY,
    NONE,
}

[CreateAssetMenu(fileName = "New Buildable Dataset", menuName = "ScriptableObjects/Create New Buildable Dataset")]
public class FrameworkSO : ScriptableObject
{
    [SerializeField] private FrameworkDetailsSO detailsSO;
    [SerializeField] private List<BuildSO> buildableDataSet;

    public Sprite FrameworkIcon => detailsSO.FrameworkIcon;
    public string FrameworkName => detailsSO.FrameworkName;
    public List<BuildSO> DataSet => buildableDataSet;

    public BuildSO GetBuildableWithID(string id)
    {
        for (int i = 0; i < buildableDataSet.Count; i++)
        {
            if (buildableDataSet[i].ID.Equals(id))
            {
                return buildableDataSet[i];
            }
        }

        return default;
    }
}
