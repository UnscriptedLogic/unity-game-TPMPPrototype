using UnityEngine;
using UnscriptedEngine;

public class O_TestBuild : O_Build
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public override void OnUpdatePreview(Vector3 position, int rotationOffset)
    {
        base.OnUpdatePreview(position, rotationOffset);

        if (!IsOverlapping())
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    public override bool CanBeBuilt()
    {
        if (!IsOverlapping())
        {
            return false;
        }

        return true;
    }
}