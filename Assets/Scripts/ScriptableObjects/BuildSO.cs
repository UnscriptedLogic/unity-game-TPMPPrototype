using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "New Build", menuName = "ScriptableObjects/Create New Build")]
public class BuildSO : ScriptableObject
{
    [SerializeField] private Sprite iconSpr;
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField][TextArea(3, 5)] private string description;
    [SerializeField] private FrameworkCategory frameworkCategory;
    [SerializeField] private BuildMachineCategory buildMachineCategory;
    [SerializeField] private O_Build build;
    [SerializeField] private VideoClip functionalityVideo;

    public Sprite Icon => iconSpr;
    public string ID => id;
    public string DisplayName => displayName;
    public string Description => description;
    public FrameworkCategory FrameworkCategory => frameworkCategory;
    public BuildMachineCategory BuildMachineCategory => buildMachineCategory;
    public O_Build Build => build;
    public VideoClip FunctionalityVideo => functionalityVideo;
}
