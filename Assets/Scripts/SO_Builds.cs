using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class SO_Builds : ScriptableObject
{
    [System.Serializable]
    public struct BuildableData
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private FrameworkCategory frameworkCategory;
        [SerializeField] private BuildMachineCategory buildMachineCategory;
        [SerializeField] private O_Build build;

        public string ID => id;
        public string DisplayName => displayName;
        public FrameworkCategory FrameworkCategory => frameworkCategory;
        public BuildMachineCategory BuildMachineCategory => buildMachineCategory;
        public O_Build Build => build;

        public BuildableData(string id, string displayName, FrameworkCategory frameworkCategory, BuildMachineCategory buildMachineCategory, O_Build build)
        {
            this.id = id;
            this.displayName = displayName;
            this.frameworkCategory = frameworkCategory;
            this.buildMachineCategory = buildMachineCategory;
            this.build = build;
        }
    }

    [SerializeField] private List<BuildableData> buildableDataSet;

    public List<BuildableData> DataSet => buildableDataSet;

    public BuildableData GetBuildableWithID(string id)
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
