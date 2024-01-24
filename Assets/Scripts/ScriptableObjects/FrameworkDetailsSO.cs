using UnityEngine;

[CreateAssetMenu(fileName = "New Framework Details", menuName = "ScriptableObjects/Create New Framework Details")]
public class FrameworkDetailsSO : ScriptableObject
{
    [SerializeField] private Sprite frameworkIcon;
    [SerializeField] private string frameworkName;

    public string FrameworkName => frameworkName;
    public Sprite FrameworkIcon => frameworkIcon;
}