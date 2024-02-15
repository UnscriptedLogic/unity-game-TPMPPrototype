using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class O_Build_ConveyorBelt : O_Build
{
    [SerializeField] private GameObject ui_hud;

    [SerializeField] private Transform startPointerAnchor;

    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject previewImage;
    
    private UIC_ConveyorBeltHUD hud;
    private bool isBuildingStart;
    private bool isInPreview;
    private Vector3 firstPos;
    private Collider2D startConstructCollider;

    public LineRenderer LineRenderer => lineRenderer;

    public override void OnBeginPreview()
    {
        base.OnBeginPreview();

        isBuildingStart = true;

        previewImage.SetActive(true);

        startPointerAnchor.GetComponent<BoxCollider2D>().enabled = false;

        hud = AttachUIWidget(ui_hud) as UIC_ConveyorBeltHUD;
        hud.transform.SetParent(null);

        isInPreview = true;
    }

    private void Update()
    {
        if (isInPreview) return;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                inventory.RemoveAt(i);
                continue;
            }

            if (!inventory[i].gameObject.activeInHierarchy)
            {
                inventory.RemoveAt(i);
                continue;
            }
        }

        if (levelBuildInterface.NodeTickSystem.HasTickedAfter(1))
        {
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(startPointerAnchor.position, 0.2f);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                O_BuildItem buildItem = collider2Ds[i].GetComponent<O_BuildItem>();
                if (buildItem != null)
                {
                    if (inventory.Contains(buildItem)) continue;

                    buildItem.transform.position = startPointerAnchor.position;

                    inventory.Add(buildItem);
                    buildItem.transform.SetParent(transform);
                    break;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isInPreview) return;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                inventory.RemoveAt(i);
                continue;
            }

            if (!inventory[i].gameObject.activeInHierarchy)
            {
                inventory.RemoveAt(i);
                continue;
            }

            if (DoesItemHaveSpaceToMove(i))
            {
                Vector3[] positions = new Vector3[lineRenderer.positionCount];
                lineRenderer.GetPositions(positions);

                //Which straight line the conveyor belt item is on
                (int start, int next) = FindTraversedSegment(inventory[i].transform.position, positions);

                Vector3 startPosition = lineRenderer.GetPosition(start) + transform.position;
                Vector3 nextPosition = lineRenderer.GetPosition(next) + transform.position;

                //This is how I calculate how far up the line it's travelled
                float calculatedLerp = Extensions.InverseLerp(startPosition, nextPosition, inventory[i].transform.position);

                if (next == lineRenderer.positionCount - 1 && calculatedLerp >= 1f)
                {
                    continue;
                }

                if (calculatedLerp >= 1f && next < lineRenderer.positionCount - 1)
                {
                    start = next;
                    next++;
                    calculatedLerp = 0f;

                    startPosition = lineRenderer.GetPosition(start) + transform.position;
                    nextPosition = lineRenderer.GetPosition(next) + transform.position;
                }

                float distance = (nextPosition - startPosition).magnitude;

                Vector3 lerpPosition = Vector3.Lerp(
                    startPosition, 
                    nextPosition, 
                    calculatedLerp + ((levelBuildInterface.GlobalBeltSpeed * Time.fixedDeltaTime) / distance)
                    );

                if (float.IsNaN(lerpPosition.x) || float.IsNaN(lerpPosition.y))
                {
                    //Probably out of bounds of the conveyor.
                    inventory.RemoveAt(i);
                    continue;
                }

                inventory[i].transform.position = lerpPosition;
            }
        }
    }

    public void SplitConveyorBelt(O_Build instigator)
    {
        ClearConveyorBelt();

        //Check where on the belt it is
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            Vector3 startPosition = lineRenderer.GetPosition(i) + transform.position;
            Vector3 nextPosition = lineRenderer.GetPosition(i + 1) + transform.position;

            if (!IsPointOnSegment(instigator.transform.position, startPosition, nextPosition)) continue;

            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            Vector3[] firstHalf = new Vector3[i];
            Vector3[] secondHalf = new Vector3[lineRenderer.positionCount - i];

            O_Build_ConveyorBelt newBelt = null;

            Extensions.SplitArray(positions, i + 1, out firstHalf, out secondHalf);

            Vector3 instigatorPos = instigator.transform.position;
            if (Vector3.Distance(secondHalf[secondHalf.Length - 1], instigator.transform.position) > 0.5f)
            {
                //Extending the end to the edge of the node
                Vector3 secondHalfStartPos = secondHalf[0] + transform.position;
                Vector3 origin = Vector3.zero;

                float offset = instigator.CellSize.x * 0.5f;
                if (instigatorPos.x == secondHalfStartPos.x)
                {
                    offset = instigator.CellSize.y * 0.5f;
                }

                float distance = Vector3.Distance(instigatorPos, secondHalfStartPos) - offset;
                Vector3 dir = (instigatorPos - secondHalfStartPos).normalized;

                origin = secondHalfStartPos + (dir * distance);

                Vector3[] secondHalfPoints = new Vector3[secondHalf.Length + 1];
                secondHalfPoints[0] = Vector3.zero;

                for (int j = 1; j < secondHalfPoints.Length; j++)
                {
                    secondHalfPoints[j] = secondHalf[j - 1] - (origin - transform.position);
                }

                //Create 2nd conveyor belt body
                newBelt = CreateNewConveyorBelt(secondHalfPoints, origin);
            }

            if (Vector3.Distance(firstHalf[firstHalf.Length - 1], instigator.transform.position) > 1f)
            {
                lineRenderer.positionCount = firstHalf.Length;
                lineRenderer.SetPositions(firstHalf);

                //Extending the end to the edge of the node
                Vector3 firstHalfEndPos = firstHalf[firstHalf.Length - 1];

                float offset = instigator.CellSize.x * 0.5f;
                if (instigatorPos.x == firstHalfEndPos.x)
                {
                    offset = instigator.CellSize.y * 0.5f;
                }

                float distance = Vector3.Distance(instigatorPos, firstHalfEndPos + transform.position) - offset;
                Vector3 dir = (instigatorPos - firstHalfEndPos - transform.position).normalized;

                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, firstHalfEndPos + (dir * distance));

                BuildToMesh();
            }
            else
            {
                Destroy(gameObject);
            }

            return;
        }
    }

    private void ClearConveyorBelt()
    {
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            inventory[i].DestroySelf();
            inventory[i].transform.SetParent(null);
        }

        inventory.Clear();
    }

    private bool DoesItemHaveSpaceToMove(int index)
    {
        if (index == 0)
        {
            return true;
        }

        //Old logic for avoiding the beeg screens
        //Vector3 difference = (inventory[index - 1].transform.position - inventory[index].transform.position);

        //bool isXFarEnough = Mathf.Abs(difference.x) > 2f;
        //bool isYFarEnough = Mathf.Abs(difference.y) > 1.25f;

        //return isXFarEnough || isYFarEnough;

        float distance = (inventory[index - 1].transform.position - inventory[index].transform.position).magnitude;
        return distance >= 0.35;
    }

    public (int, int) FindTraversedSegment(Vector3 currentPoint, Vector3[] positions)
    {
        for (int i = 0; i < positions.Length - 1; i++)
        {
            Vector3 startPoint = positions[i] + transform.position;
            Vector3 endPoint = positions[i + 1] + transform.position;

            // Check if the current point is on the inferred line segment
            if (IsPointOnSegment(currentPoint, startPoint, endPoint))
            {
                return (i, i + 1);
            }
        }

        return default; // Point is not on any inferred line segment
    }

    //ChatGPT
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
        Vector3 correctedPosition = position + new Vector3(0.5f, 0.5f, 0f) - firstPos;

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

    private Vector3 GetCorrectedPos(Vector3 position)
    {
        return position + new Vector3(0.5f, 0.5f, 0f);
    }

    public override bool CanBeBuilt()
    {
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(GetCorrectedPos(lineRenderer.GetPosition(0)), Vector2.one * 0.2f, 0f);



        return true;
    }

    public override void AlternateBuild(Vector3 position, int rotationOffset)
    {
        if (!isBuildingStart)
        {
            string failReason = "";

            bool isPointValid = true;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(GetCorrectedPos(position), Vector2.one * 0.2f, 0f);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                if (collider2Ds[i].CompareTag(START_CONSTRUCT_POINT) || collider2Ds[i].CompareTag(END_CONSTRUCT_POINT))
                {
                    isPointValid = false;
                    failReason = "Too close to critical points";
                    break;
                }

                if (collider2Ds[i].GetComponent<O_Build>() == null) continue;

                isPointValid = false;
            }

            RaycastHit2D[] hit2Ds = Physics2D.LinecastAll(GetCorrectedPos(position), transform.position + lineRenderer.GetPosition(lineRenderer.positionCount - 2));
            for (int i = 0; i < hit2Ds.Length; i++)
            {
                if (hit2Ds[i].collider == startConstructCollider)
                {
                    isPointValid = true;
                    break;
                }

                O_Build build = hit2Ds[i].collider.GetComponent<O_Build>();
                if (build != null)
                {
                    if (build is O_Build_ConveyorBelt)
                    {
                        continue;
                    }

                    isPointValid = false;
                    failReason = "Is obstructed";
                    break;
                }
            }

            for (int i = 0; i < lineRenderer.positionCount - 1; i++)
            {
                if (i == 0) continue;

                if (IsPointOnSegment(lineRenderer.GetPosition(lineRenderer.positionCount - 1), lineRenderer.GetPosition(i - 1), lineRenderer.GetPosition(i)))
                {
                    isPointValid = false;
                    failReason = "Trying to build into itself";
                    break;
                }
            }

            float distance = (lineRenderer.GetPosition(lineRenderer.positionCount - 1) - lineRenderer.GetPosition(lineRenderer.positionCount - 2)).magnitude;
            if (distance <= 0.5f)
            {
                isPointValid = false;
                failReason = "Points are too close";
            }

            if (!isPointValid)
            {
                Debug.Log(failReason);
                return;
            }

            //Add a point
            lineRenderer.positionCount++;
        }
    }

    public override void Build(Vector3 position, int rotationOffset, bool keepBuilding)
    {
        Vector3 correctedPosition = position + new Vector3(0.5f, 0.5f, 0f);

        if (isBuildingStart)
        {
            bool isInitialPointValid = true;
            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(correctedPosition, Vector2.one * 0.2f, 0f);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                if (collider2Ds[i].CompareTag(START_CONSTRUCT_POINT))
                {
                    startConstructCollider = collider2Ds[i];
                    isInitialPointValid = true;
                    break;
                }

                if (collider2Ds[i].GetComponent<O_Build>() == null) continue;

                isInitialPointValid = false;
            }

            if (!isInitialPointValid)
            {
                return;
            }

            isBuildingStart = false;

            //Create a start and end point

            firstPos = correctedPosition;

            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, correctedPosition - firstPos);

            previewImage.SetActive(false);
        }
        else
        {
            //Finish building

            bool isInitialPointValid = true;

            RaycastHit2D[] hit2Ds = Physics2D.LinecastAll(GetCorrectedPos(position), transform.position + lineRenderer.GetPosition(lineRenderer.positionCount - 2));
            for (int i = 0; i < hit2Ds.Length; i++)
            {
                if (hit2Ds[i].collider == startConstructCollider)
                {
                    break;
                }

                O_Build build = hit2Ds[i].collider.GetComponent<O_Build>();
                if (build != null)
                {
                    if (build is O_Build_ConveyorBelt)
                    {
                        continue;
                    }

                    isInitialPointValid = false;
                    break;
                }
            }

            Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(correctedPosition, Vector2.one * 0.2f, 0f);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                if (collider2Ds[i].CompareTag(END_CONSTRUCT_POINT))
                {
                    isInitialPointValid = true;
                    break;
                }

                if (collider2Ds[i].GetComponent<O_Build>() == null) continue;

                isInitialPointValid = false;
            }

            if (!isInitialPointValid)
            {
                return;
            }

            isBuildingStart = true;

            O_Build_ConveyorBelt newConveyor = Instantiate(gameObject).GetComponent<O_Build_ConveyorBelt>();
            newConveyor.startPointerAnchor.GetComponent<BoxCollider2D>().enabled = true;
            newConveyor.isInPreview = false;
            newConveyor.FireBuiltEvent();
            newConveyor.BuildToMesh();

            lineRenderer.positionCount = 0;

            if (!keepBuilding)
            {
                OnEndPreview();
            }
        }
    }

    /// <summary>
    /// Creates a new conveyor belt along with its points
    /// </summary>
    /// <param name="points"></param>
    private O_Build_ConveyorBelt CreateNewConveyorBelt(Vector3[] points, Vector3 origin)
    {
        O_Build_ConveyorBelt newConveyor = Instantiate(gameObject).GetComponent<O_Build_ConveyorBelt>();
        newConveyor.startPointerAnchor.GetComponent<BoxCollider2D>().enabled = true;
        newConveyor.isInPreview = false;

        //position alignment
        newConveyor.transform.position = origin;
        newConveyor.startPointerAnchor.transform.position = origin;

        //Initialize Points
        newConveyor.lineRenderer.positionCount = points.Length;
        newConveyor.lineRenderer.SetPositions(points);

        newConveyor.FireBuiltEvent();
        newConveyor.BuildToMesh();

        return newConveyor;
    }

    private void BuildToMesh()
    {
        EdgeCollider2D edgeCollider = GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = lineRenderer.AddComponent<EdgeCollider2D>();
        }

        edgeCollider.points = new Vector2[lineRenderer.positionCount];

        List<Vector2> edges = new List<Vector2>();
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 point = lineRenderer.GetPosition(i);
            edges.Add(new Vector2(point.x, point.y) - (Vector2)lineRenderer.GetPosition(0));
        }

        edgeCollider.SetPoints(edges);
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

        //source: https://stackoverflow.com/questions/1841246/c-sharp-splitting-an-array
        public static void SplitArray<T>(T[] array, int index, out T[] first, out T[] second)
        {
            first = array.Take(index).ToArray();
            second = array.Skip(index).ToArray();
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null) continue;

            Destroy(inventory[i].gameObject);
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