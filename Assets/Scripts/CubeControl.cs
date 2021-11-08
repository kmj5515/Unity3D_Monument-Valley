using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour
{
    // 해당 블록에서 이동할 수 있는 좌표
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    [Space]
    // 길찾기 할때 지나온 길
    public Transform previousBlock;
    [Space]
    public float moveSpeed = 0.5f;
    public bool movingGround = false;

    private void OnDrawGizmos()
    {
        //큐브 위에 선 그리기
        Gizmos.color = Color.white;
        Gizmos.DrawCube(GetWalkPoint(), new Vector3(0.1f, 0.1f, 0.1f));

        for (int i = 0; i < possiblePaths.Count; i++)
        {
            if (possiblePaths[i].active)
            {
                //큐브 위에 라인 그리기
                Gizmos.color = Color.green;
                Gizmos.DrawLine(GetWalkPoint(), possiblePaths[i].target.GetComponent<CubeControl>().GetWalkPoint());
            }
            else if (!possiblePaths[i].active)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(GetWalkPoint(), possiblePaths[i].target.GetComponent<CubeControl>().GetWalkPoint());
            }
        }
    }

    //걷는 좌표 포인트 반환
    public Vector3 GetWalkPoint()
    {
        return (transform.position + transform.up * moveSpeed);
    }
}

// 각 큐브마다 위치값과 행동
[System.Serializable]
public class WalkPath
{
    public Transform target;
    public bool active = true;
}