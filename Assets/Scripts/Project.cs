using System.Collections.Generic;

[System.Serializable]
public class Project
{
    private string name;
    private string description;
    private bool isCompleted;

    private List<Requirement> requirements = new List<Requirement>();

    public string Name => name;
    public string Description => description;
    public bool IsCompleted => isCompleted;
    public List<Requirement> Requirements => requirements;

    public Project(string name, string description, bool isCompleted, List<Requirement> requirements)
    {
        this.name = name;
        this.description = description;
        this.isCompleted = isCompleted;
        this.requirements = requirements;
    }
}
