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
public class SO_Builds : ScriptableObject
{
    [System.Serializable]
    public struct BuildableData
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField][TextArea(3, 5)] private string description;
        [SerializeField] private FrameworkCategory frameworkCategory;
        [SerializeField] private BuildMachineCategory buildMachineCategory;
        [SerializeField] private O_Build build;
        [SerializeField] private VideoClip functionalityVideo;

        public string ID => id;
        public string DisplayName => displayName;
        public string Description => description;
        public FrameworkCategory FrameworkCategory => frameworkCategory;
        public BuildMachineCategory BuildMachineCategory => buildMachineCategory;
        public O_Build Build => build;
        public VideoClip FunctionalityVideo => functionalityVideo;

        public BuildableData(string id, string displayName, string description, FrameworkCategory frameworkCategory, BuildMachineCategory buildMachineCategory, O_Build build, VideoClip functionalityVideo)
        {
            this.id = id;
            this.displayName = displayName;
            this.description = description;
            this.frameworkCategory = frameworkCategory;
            this.buildMachineCategory = buildMachineCategory;
            this.build = build;
            this.functionalityVideo = functionalityVideo;
        }
    }

    [SerializeField] private Sprite frameworkIcon;
    [SerializeField] private string frameworkName;
    [SerializeField] private List<BuildableData> buildableDataSet;

    public Sprite FrameworkIcon => frameworkIcon;
    public string FrameworkName => frameworkName;
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
