using System.Collections.Generic;
using UnityEngine;

public class O_Build_ConveyorBelt : O_Build
{
    [SerializeField] private GameObject ui_hud;

    [SerializeField] private Transform startPointerAnchor;
    [SerializeField] private Transform endPointerAnchor;

    [SerializeField] private LineRenderer lineRenderer;
    
    private List<O_BuildItem> conveyorItems = new List<O_BuildItem>();
    private UIC_ConveyorBeltHUD hud;
    private bool isBuildingStart;
    private bool isInPreview;

    public override void OnBeginPreview()
    {
        isBuildingStart = true;

        startPointerAnchor.GetComponent<BoxCollider2D>().enabled = false;
        endPointerAnchor.GetComponent<BoxCollider2D>().enabled = false;

        hud = AttachUIWidget(ui_hud) as UIC_ConveyorBeltHUD;
        hud.transform.SetParent(null);

        endPointerAnchor.gameObject.SetActive(false);
        isInPreview = true;
    }

    private void FixedUpdate()
    {
        if (isInPreview) return;

        if (levelBuildInterface.NodeTickSystem.HasTickedAfter(2))
        {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(startPointerAnchor.position, 0.2f);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                O_BuildItem buildItem = collider2Ds[i].GetComponent<O_BuildItem>();
                if (buildItem != null)
                {
                    if (conveyorItems.Contains(buildItem)) continue;

                    conveyorItems.Add(buildItem);
                    break;
                }
            }
        }

        for (int i = 0; i < conveyorItems.Count; i++)
        {
            if (conveyorItems[i] == null)
            {
                conveyorItems.RemoveAt(i);
                continue;
            }

            if (!conveyorItems[i].gameObject.activeInHierarchy)
            {
                conveyorItems.RemoveAt(i);
                continue;
            }

            if (DoesItemHaveSpaceToMove(i))
            {
                Vector3[] positions = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(positions);

                (int start, int next) = FindTraversedSegment(conveyorItems[i].transform.position, positions);

                float calculatedLerp = Extensions.InverseLerp(lineRenderer.GetPosition(start), lineRenderer.GetPosition(next), conveyorItems[i].transform.position);

                if (next == lineRenderer.positionCount - 1 && calculatedLerp >= 1f)
                {
                    continue;
                }

                if (calculatedLerp >= 1f && next < lineRenderer.positionCount - 1)
                {
                    start = next;
                    next++;
                    calculatedLerp = 0f;
                }

                float distance = Vector3.Distance(lineRenderer.GetPosition(start), lineRenderer.GetPosition(next));

                Vector3 lerpPosition = Vector3.Lerp(lineRenderer.GetPosition(start), lineRenderer.GetPosition(next), calculatedLerp + ((levelBuildInterface.GlobalBeltSpeed * Time.fixedDeltaTime) / distance));
                if (float.IsNaN(lerpPosition.x) || float.IsNaN(lerpPosition.y))
                {
                    //Probably out of bounds of the conveyor.

                    conveyorItems.RemoveAt(i);
                    continue;
                }

                conveyorItems[i].transform.position = lerpPosition;
            }
        }
    }

    private bool DoesItemHaveSpaceToMove(int index)
    {
        if (index == 0)
        {
            return true;
        }

        Vector3 difference = (conveyorItems[index - 1].transform.position - conveyorItems[index].transform.position);

        bool isXFarEnough = Mathf.Abs(difference.x) > 2f;
        bool isYFarEnough = Mathf.Abs(difference.y) > 1.25f;

        return isXFarEnough || isYFarEnough;
    }

    public (int, int) FindTraversedSegment(Vector3 currentPoint, Vector3[] positions)
    {
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector3 startPoint = positions[i];
            Vector3 endPoint = positions[i + 1];

            // Check if the current point is on the inferred line segment
            if (IsPointOnSegment(currentPoint, startPoint, endPoint))
            {
                return (i, i + 1);
            }
        }

        return default; // Point is not on any inferred line segment
    }

    private bool IsPointOnSegment(Vector3 point, Vector3 start, Vector3 end)
    {
        float epsilon = 0.0001f; // A small value to account for floating-point errors

        // Check if the point is within the bounding box of the inferred line segment
        bool xInRange = (point.x >= Mathf.Min(start.x, end.x) - epsilon) &&
                        (point.x <= Mathf.Max(start.x, end.x) + epsilon);

        bool yInRange = (point.y >= Mathf.Min(start.y, end.y) - epsilon) &&
                        (point.y <= Mathf.Max(start.y, end.y) + epsilon);

        if (xInRange && yInRange)
        {
            // Check if the point is collinear with the inferred line segment
            float crossProduct = (point.y - start.y) * (end.x - start.x) - (point.x - start.x) * (end.y - start.y);

            return Mathf.Abs(crossProduct) < epsilon;
        }

        return false;
    }

    public override void OnUpdatePreview(Vector3 position, int rotationOffset)
    {
        Vector3 correctedPosition = position + new Vector3(0.5f, 0.5f, 0f);

        if (isBuildingStart)
        {
            transform.position = correctedPosition;
        }
        else
        {
            //Logic used to confining the conveyor belts to straight lines
            float deltaX = Mathf.Abs(correctedPosition.x - lineRenderer.GetPosition(lineRenderer.positionCount - 2).x);
            float deltaY = Mathf.Abs(correctedPosition.y - lineRenderer.GetPosition(lineRenderer.positionCount - 2).y);

            if (deltaX > deltaY)
            {
                correctedPosition.y = lineRenderer.GetPosition(lineRenderer.positionCount - 2).y;
            }
            else
            {
                correctedPosition.x = lineRenderer.GetPosition(lineRenderer.positionCount - 2).x;
            }

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, correctedPosition);
            endPointerAnchor.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
        }
    }

    public override void OnEndPreview()
    {
        if (hud != null)
        {
            DettachUIWidget(hud.gameObject);
        }

        Destroy(gameObject);
    }

    public override bool CanBeBuilt()
    {
        return true;
    }

    public override void AlternateBuild(Vector3 position, int rotationOffset)
    {
        if (!isBuildingStart)
        {
            //Add a point
            lineRenderer.positionCount++;
        }
    }

    public override void Build(Vector3 position, int rotationOffset, bool keepBuilding)
    {
        Vector3 correctedPosition = position + new Vector3(0.5f, 0.5f, 0f);

        if (isBuildingStart)
        {
            isBuildingStart = false;

            //Create a start and end point

            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, correctedPosition);
            lineRenderer.SetPosition(1, correctedPosition);

            endPointerAnchor.gameObject.SetActive(true);
        }
        else
        {
            //Finish building

            isBuildingStart = true;

            O_Build_ConveyorBelt newConveyor = Instantiate(gameObject).GetComponent<O_Build_ConveyorBelt>();
            newConveyor.startPointerAnchor.GetComponent<BoxCollider2D>().enabled = true;
            newConveyor.endPointerAnchor.GetComponent<BoxCollider2D>().enabled = true;
            newConveyor.isInPreview = false;
            newConveyor.FireBuiltEvent();

            lineRenderer.positionCount = 0;
            endPointerAnchor.gameObject.SetActive(false);

            if (!keepBuilding)
            {
                OnEndPreview();
            }
        }
    } 

    public static class Extensions
    {
        //source: https://discussions.unity.com/t/inverselerp-for-vector3/177038/2
        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < conveyorItems.Count; i++)
        {
            if (conveyorItems[i] == null) continue;

            Destroy(conveyorItems[i].gameObject);
        }
    }
}

public static class ConveyorBeltExtensions
{
    public static void AnimateConveyorMaterial(this Material globalConveyorMaterial, float beltSpeed)
    {
        globalConveyorMaterial.mainTextureOffset -= new Vector2(beltSpeed * 2.115f * Time.deltaTime, 0);

        if (globalConveyorMaterial.mainTextureOffset.x <= -10)
        {
            globalConveyorMaterial.mainTextureOffset = Vector2.zero;
        }
    }

}