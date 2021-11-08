using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour
{
    // �ش� ��Ͽ��� �̵��� �� �ִ� ��ǥ
    public List<WalkPath> possiblePaths = new List<WalkPath>();

    [Space]
    // ��ã�� �Ҷ� ������ ��
    public Transform previousBlock;
    [Space]
    public float moveSpeed = 0.5f;
    public bool movingGround = false;

    private void OnDrawGizmos()
    {
        //ť�� ���� �� �׸���
        Gizmos.color = Color.white;
        Gizmos.DrawCube(GetWalkPoint(), new Vector3(0.1f, 0.1f, 0.1f));

        for (int i = 0; i < possiblePaths.Count; i++)
        {
            if (possiblePaths[i].active)
            {
                //ť�� ���� ���� �׸���
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

    //�ȴ� ��ǥ ����Ʈ ��ȯ
    public Vector3 GetWalkPoint()
    {
        return (transform.position + transform.up * moveSpeed);
    }
}

// �� ť�긶�� ��ġ���� �ൿ
[System.Serializable]
public class WalkPath
{
    public Transform target;
    public bool active = true;
}