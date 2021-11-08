using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerControl : MonoBehaviour
{
    [Header("Cube Info")]
    // ���� ��ġ�� ť��
    public Transform currentCube;
    // ���콺 Ŭ���� ť��
    public Transform clickedCube;

    // �÷��̾� �̵����
    public List<Transform> finalPath = new List<Transform>();

    CubeControl pastCube;
    CubeControl nextCube;
    float timing = 0;

    public float moveSpeed;


    // Start is called before the first frame update
    void Start()
    {
        RayCurrentCube();

        moveSpeed = 2.5f;
    }

    // Update is called once per frame
    void Update()
    {
        RayCurrentCube();

        // ���� ��� �ִ� ť�갡 �����̴� ���
        if (currentCube.GetComponent<CubeControl>().movingGround)
        {
            // �÷��̾ �� �ڽ����� �ִ´�
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }

        // ���콺 Ŭ���� ���ӸŴ��� �÷��� üũ
        if (Input.GetMouseButtonDown(0) && GameManager.instance.Ready)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            // ���� �߻�!!
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                // Ŭ���� ���� Path�� ���
                if (mouseHit.transform.GetComponent<CubeControl>() != null)
                {
                    SoundManager.instance.play("Navi", 1f);

                    clickedCube = mouseHit.transform;

                    // ��� �ʱ�ȭ
                    finalPath.Clear();

                    // ��ã��
                    FindPath();
                }
            }
        }

        // �̵�
        FollowPath();
    }

    public void RayCurrentCube()
    {
        Vector3 rayPos = transform.position;
        rayPos.y += transform.localScale.y * 0.5f;

        // ���� ����, ������ �Ʒ�
        Ray playerRay = new Ray(rayPos, -transform.up);
        RaycastHit playerHit;

        // ���� �߻�!!
        if (Physics.Raycast(playerRay, out playerHit))
        {
            // ������ ��� �ִٸ�
            if (playerHit.transform.GetComponent<CubeControl>() != null)
            {
                currentCube = playerHit.transform;
            }
        }
    }

    // ��ã��
    private void FindPath()
    {
        // �̵��� ť�� ���� ����Ʈ
        List<Transform> nextCubes = new List<Transform>();
        // ���� ť�� ���� ����Ʈ
        List<Transform> pastCubes = new List<Transform>();

        for (int i = 0; i < currentCube.GetComponent<CubeControl>().possiblePaths.Count; i++)
        {
            // ť�긶�� possiblePaths ����Ʈ ���
            WalkPath walkPath = currentCube.GetComponent<CubeControl>().possiblePaths[i];

            if (walkPath.active)    // true
            {
                //���� ť���� ��ġ���� �߰�
                nextCubes.Add(walkPath.target);

                // ���� ť����� ���� ť��� ����.
                walkPath.target.GetComponent<CubeControl>().previousBlock = currentCube;
            }
        }
        
        pastCubes.Add(currentCube);

        ExploreCube(nextCubes, pastCubes);
        BuildPath();
    }

    void FollowPath()
    {
        if (finalPath.Count == 0)
        {
            return;
        }

        // Ÿ���� üũ�Ͽ� �÷��̾� ���̾� ����
        LayerCheck(nextCube.transform);

        // ���� ����
        transform.position = Vector3.Lerp(pastCube.GetWalkPoint(), nextCube.GetWalkPoint(), timing);

        // ���� ��� ����
        if (timing >= 1.0f)
        {
            timing = 0;

            pastCube = finalPath.Last().GetComponent<CubeControl>();

            finalPath.Last().GetComponent<CubeControl>().previousBlock = null;
            finalPath.RemoveAt(finalPath.Count - 1);

            if (finalPath.Count > 0)
            {
                nextCube = finalPath.Last().GetComponent<CubeControl>();

                return;
            }
        }
        else
        {
            LayerCheck(nextCube.transform);

            if (currentCube.Equals(GameManager.instance.clearCube))
            {
                GameManager.instance.Clear = true;
            }
        }

        timing += Time.deltaTime * moveSpeed;
    }

    private void ExploreCube(List<Transform> nextCubes, List<Transform> visitedCubes)
    {
        Transform current = nextCubes.First();
        nextCubes.Remove(current);

        // Ŭ���� ť��� ���� ť�갡 ������
        // ��ǥ ��ǥ�� ������ ��
        if (current == clickedCube)
        {
            return;
        }

        // ���� ť���� �̵� ������ ť�길ŭ �ݺ�
        for (int i = 0; i < current.GetComponent<CubeControl>().possiblePaths.Count; i++)
        {
            WalkPath walkPath = current.GetComponent<CubeControl>().possiblePaths[i];

            // �̹� ������ ���� �ƴϰ� ���� ����Ǿ� �ִٸ�
            if (!visitedCubes.Contains(walkPath.target) && walkPath.active)
            {
                // ���� �˻� ť���
                nextCubes.Add(walkPath.target);

                // �̵��� ��ο� �߰�
                walkPath.target.GetComponent<CubeControl>().previousBlock = current;
            }

        }

        // �湮�� ť�� ����Ʈ�� ���� ť�� �߰�
        visitedCubes.Add(current);

        // ����Ʈ�� �ϳ��� �ִٸ�
        if (nextCubes.Count > 0)
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    // ��� ����
    private void BuildPath()
    {
        Transform cube = clickedCube;

        // Ŭ���� ť�갡 ����ť��� ���� ���� ������
        while (cube != currentCube)
        {
            // ���� �̵��� ��ο� ����
            finalPath.Add(cube);

            // Ŭ���� ť���� ����ť�갡 None�� ��
            if (cube.GetComponent<CubeControl>().previousBlock != null)
            {
                cube = cube.GetComponent<CubeControl>().previousBlock;
            }
            else
            {
                break;
            }
        }

        if (finalPath.Count > 0)
        {
            bool walk = false;

            for (int i = 0; i < currentCube.GetComponent<CubeControl>().possiblePaths.Count; i++)
            {
                WalkPath walkCube = currentCube.GetComponent<CubeControl>().possiblePaths[i];

                if (walkCube.target.Equals(finalPath[finalPath.Count - 1]) && walkCube.active)
                {
                    walk = true;
                    break;
                }
            }

            if (!walk)
            {
                finalPath.Clear();
            }
            else
            {
                pastCube = currentCube.GetComponent<CubeControl>();
                nextCube = finalPath[finalPath.Count - 1].GetComponent<CubeControl>();

                timing = 0;
            }
        }
    }

    void LayerCheck(Transform cube)
    {
        bool isTop = false;

        if (cube.childCount > 0)
        {
            isTop = true;
        }

        if (!isTop)
        {
            for (int i = 0; i < cube.GetComponent<CubeControl>().possiblePaths.Count; i++)
            {
                if (cube.GetComponent<CubeControl>().possiblePaths[i].target.childCount > 0)
                {
                    isTop = true;
                    break;
                }
            }
        }

        if (isTop)
        {
            SetLayerObject(transform, "Top");
        }
        else
        {
            SetLayerObject(transform, "Default");
        }
    }

    // �ڽ� ������Ʈ���� ��� ���̾� ����
    void SetLayerObject(Transform tr, string layerName)
    {
        tr.gameObject.layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < tr.childCount; i++)
        {
            // �ڽ��� �ڽĵ���� ����
            SetLayerObject(tr.GetChild(i), layerName);
        }
    }
}
