using UnityEngine;
using UnscriptedEngine;

public class O_BuildItem : ULevelObject
{
    [SerializeField] private FrameworkCategory category = FrameworkCategory.VANILLA;
    [SerializeField] private string id;

    public string ID => id;
    public FrameworkCategory Category => category;

    protected IUsesPageObjects pageObjectInterface;

    protected virtual void Start()
    {
        pageObjectInterface = GameMode as IUsesPageObjects;
        if (pageObjectInterface == null)
        {
            Debug.LogWarning("The current GameMode doesn't use IUsesPageObjects");
            return;
        }

        pageObjectInterface.OnClearAllObjects += PageObjectInterface_OnClearAllObjects;
    }

    private void PageObjectInterface_OnClearAllObjects(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }

    protected override void OnDestroy()
    {
        pageObjectInterface.OnClearAllObjects -= PageObjectInterface_OnClearAllObjects;

        base.OnDestroy();
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}