using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.U2D;

public class RayerScript : MonoBehaviour
{
    public float range;
    public int countReflect;
    public int damage;
    LayerMask layermask;
    LineRenderer lineRenderer;
    public List<Vector3> posLine;
    private void Start()
    {
        posLine = new List<Vector3>();
        lineRenderer = GetComponent<LineRenderer>();
        layermask = (1 << LayerMask.NameToLayer("Wall")) | (1 << LayerMask.NameToLayer("Enemy"));
        posLine.Add(transform.position);
        ThrowRay();
        ShowLine();
        Destroy(gameObject, 1f);
    }

    void ThrowRay()
    {
        Ray2D ray = new Ray2D(transform.position, transform.right);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin + (Vector2)(transform.right * 0.3f), ray.direction, range, layermask);
        Debug.DrawRay(ray.origin + (Vector2)(transform.right * 0.3f), ray.direction * range, Color.blue, 3f);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<EnemyScript>() != null)
            {
                hit.collider.GetComponent<EnemyScript>().RemoveHp(damage);
            }
            if (hit.collider.tag == "Wall")
            {
                transform.position = hit.point;
                if (countReflect > 0)
                {
                    countReflect -= 1;
                    range -= hit.distance;
                    Reflect(hit);
                }
                else
                {
                    posLine.Add(transform.position);
                }

            }

        }
        else
        {
            transform.Translate(Vector2.right * range);
            posLine.Add(transform.position);
        }
    }

    void ShowLine()
    {
        lineRenderer.SetPositions(posLine.ToArray());
        for (int i = 0; i < posLine.Count; i++)
        {
            lineRenderer.positionCount += 1;
            lineRenderer.SetPosition(i, posLine[i]);
        }
    }

    void Reflect(RaycastHit2D hit)
    {
        //transform.position = hit.point;
        posLine.Add(hit.point);
        Vector2 pos = Vector2.Reflect(transform.right, hit.normal);
        float angle = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        Debug.DrawRay(hit.point, pos, Color.red, 2f);
        ThrowRay();
    }
}
