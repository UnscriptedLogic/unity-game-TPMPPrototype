using System.Collections.Generic;
using UnityEngine;
using UnscriptedEngine;

[System.Serializable]
public class Framework
{
    [SerializeField] private FrameworkDetailsSO details;
    [SerializeField] private List<BuildSO> builds;

    public FrameworkDetailsSO Details => details;
    public List<BuildSO> Builds => builds;
}

[System.Serializable]
public class Project
{
    [SerializeField] private Sprite thumbnail;
    [SerializeField] private string name;
    [SerializeField][TextArea(3, 5)] private string description;
    [SerializeField] private WebPageSO webpageSO;
    [SerializeField] private List<Framework> projectFrameworks;

    private bool isCompleted;

    //This is the defining factor of the randomness in the project. 
    //Everything random is defined by this seed for ease.
    private int seed;

    //A project is defined as initialized when the player has opened it and viewed the pages
    private bool initialized;

    private List<Requirement> requirements;

    private List<int> pageIndexes = new List<int>();

    public string Name => name;
    public string Description => description;
    public Sprite Thumbnail => thumbnail;
    public bool IsCompleted => isCompleted;
    public List<Requirement> Requirements => requirements;
    public List<int> PageIndexes => pageIndexes;
    public bool IsInitialized => initialized;
    public int Seed => seed;
    public WebPageSO WebPageSO => webpageSO;
    public List<Framework> Frameworks => projectFrameworks;

    public Project(string name, string description, bool isCompleted, List<Requirement> requirements, List<int> pageIndexes, bool initialized, int seed)
    {
        this.name = name;
        this.description = description;
        this.isCompleted = isCompleted;
        this.requirements = requirements;
        this.pageIndexes = pageIndexes;
        this.initialized = initialized;
        this.seed = seed;
    }   

    public void EvaluateRequirements(UGameModeBase context)
    {
        for (int i = 0; i < requirements.Count; i++)
        {
            if (requirements[i].IsConditionMet.Value) continue;

            requirements[i].EvaluateCondition(context);
        }
    }

    public bool AreAllRequirementsMet
    {
        get
        {
            for (int i = 0; i < requirements.Count; i++)
            {
                if (requirements[i].IsConditionMet.Value) continue;

                return false;
            }

            return true;
        }
    }

    public void Complete() => isCompleted = true;

    public void SetRequirements(List<Requirement> requirements)
    {
        this.requirements = new List<Requirement>(requirements);
    }
}

public static class ProjectExtensions
{
    public static bool DoesPlayerHaveProject(this List<Project> projects, string name)
    {
        for (int i = 0; i < projects.Count; i++)
        {
            if (projects[i].Name != name) continue;

            return true;
        }

        return false;
    }

    public static Project GetPlayerProjectByName(this List<Project> projects, string name)
    {
        for (int i = 0; i < projects.Count; i++)
        {
            if (projects[i].Name != name) continue;
            return projects[i];
        }

        return null;
    }

    public static int GenerateRandomProjectSeed()
    {
        return UnityEngine.Random.Range(-1000000, 1000000) * System.DateTime.Now.Millisecond * System.DateTime.Now.Second * System.DateTime.Now.Day;
    }
}