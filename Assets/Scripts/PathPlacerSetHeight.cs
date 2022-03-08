using PathCreation;
using UnityEngine;

namespace PathCreation.Examples
{
    public class PathPlacerSetHeight : PathPlacer
    {

        public LayerMask groundLayer;
        public float heightAboveGround;

        const float minSpacing = .1f;

        protected override void Generate()
        {
            if (pathCreator != null && prefab != null && holder != null)
            {
                DestroyObjects();

                VertexPath path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = 0;

                while (dst < path.length)
                {
                    Vector3 point = path.GetPointAtDistance(dst);
                    RaycastHit hit;
                    if(Physics.Raycast(point, Vector3.down, out hit, 6f, groundLayer, QueryTriggerInteraction.Ignore))
                        point = hit.point + (Vector3.up * heightAboveGround);

                    Quaternion rot = path.GetRotationAtDistance(dst);
                    Instantiate(prefab, point, rot, holder.transform);
                    dst += spacing;
                }
            }
        }

        protected override void DestroyObjects()
        {
            int numChildren = holder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
            }
        }

        protected override void PathUpdated()
        {
            base.PathUpdated();
        }
    }
}
