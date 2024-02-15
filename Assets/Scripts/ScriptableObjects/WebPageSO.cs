using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New WebPageScriptable", menuName = "ScriptableObjects/Create New WebPageScriptable")]
public class WebPageSO : ScriptableObject
{
    [System.Serializable]
    public class WebPageComponentData
    {
        [SerializeField] private string componentID;
        [SerializeField] private string[] modificationIds;

        public string ComponentID => componentID;
        public string[] ModificationIDs => modificationIds;
    }

    [System.Serializable]
    public class PageData
    {
        [SerializeField] private GameObject webpage;
        [SerializeField] private int requiredMinPage;
        [SerializeField] private WebPageComponentData[] components;

        public GameObject WebPage => webpage;
        public int RequiredMinPage => requiredMinPage;
        public WebPageComponentData[] RequiredComponents => components;
    }

    [SerializeField] private List<PageData> webPageData = new List<PageData>();

    public List<PageData> WebPageDataSet => webPageData;

    public bool IsComponentRequirementsMet(O_BuildPage page, PageData data)
    {
        //Component count
        if (page.AttachedComponents.Count != data.RequiredComponents.Length) return false;

        //Check if the components are indeed the required components
        for (int i = 0; i < page.AttachedComponents.Count; i++)
        {
            bool isComponentValid = false;
            if (page.AttachedComponents[i].id == data.RequiredComponents[i].ComponentID)
            {
                isComponentValid = AreAllModificationsTheSame(data.RequiredComponents[i].ModificationIDs, page.AttachedComponents[i].ModificationsID);
            }

            if (!isComponentValid)
            {
                return false;
            }
        }

        return true;
    }

    private bool AreAllModificationsTheSame(string[] requiredMods, string componentModID)
    {
        if (requiredMods.Length > 0 && string.IsNullOrEmpty(componentModID)) return false;

        for (int i = 0; i < requiredMods.Length; i++)
        {
            if (!componentModID.Contains(requiredMods[i]))
            {
                return false;
            }
        }

        return true;
    }
}
