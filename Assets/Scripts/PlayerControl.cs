using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerControl : MonoBehaviour
{
    [Header("Cube Info")]
    // 현재 위치한 큐브
    public Transform currentCube;
    // 마우스 클릭한 큐브
    public Transform clickedCube;

    // 플레이어 이동경로
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

        // 현재 밟고 있는 큐브가 움직이는 경우
        if (currentCube.GetComponent<CubeControl>().movingGround)
        {
            // 플레이어를 그 자식으로 넣는다
            transform.parent = currentCube.parent;
        }
        else
        {
            transform.parent = null;
        }

        // 마우스 클릭과 게임매니저 플래그 체크
        if (Input.GetMouseButtonDown(0) && GameManager.instance.Ready)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            // 레이 발사!!
            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                // 클릭한 곳이 Path인 경우
                if (mouseHit.transform.GetComponent<CubeControl>() != null)
                {
                    SoundManager.instance.play("Navi", 1f);

                    clickedCube = mouseHit.transform;

                    // 경로 초기화
                    finalPath.Clear();

                    // 길찾기
                    FindPath();
                }
            }
        }

        // 이동
        FollowPath();
    }

    public void RayCurrentCube()
    {
        Vector3 rayPos = transform.position;
        rayPos.y += transform.localScale.y * 0.5f;

        // 레이 생성, 방향은 아래
        Ray playerRay = new Ray(rayPos, -transform.up);
        RaycastHit playerHit;

        // 레이 발사!!
        if (Physics.Raycast(playerRay, out playerHit))
        {
            // 발판을 밟고 있다면
            if (playerHit.transform.GetComponent<CubeControl>() != null)
            {
                currentCube = playerHit.transform;
            }
        }
    }

    // 길찾기
    private void FindPath()
    {
        // 이동할 큐브 담을 리스트
        List<Transform> nextCubes = new List<Transform>();
        // 이전 큐브 담을 리스트
        List<Transform> pastCubes = new List<Transform>();

        for (int i = 0; i < currentCube.GetComponent<CubeControl>().possiblePaths.Count; i++)
        {
            // 큐브마다 possiblePaths 리스트 목록
            WalkPath walkPath = currentCube.GetComponent<CubeControl>().possiblePaths[i];

            if (walkPath.active)    // true
            {
                //다음 큐브의 위치값을 추가
                nextCubes.Add(walkPath.target);

                // 현재 큐브들이 지난 큐브로 들어간다.
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

        // 타일을 체크하여 플레이어 레이어 설정
        LayerCheck(nextCube.transform);

        // 보간 적용
        transform.position = Vector3.Lerp(pastCube.GetWalkPoint(), nextCube.GetWalkPoint(), timing);

        // 다음 경로 설정
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

        // 클릭한 큐브와 현재 큐브가 같으면
        // 목표 좌표에 도착한 것
        if (current == clickedCube)
        {
            return;
        }

        // 현재 큐브의 이동 가능한 큐브만큼 반복
        for (int i = 0; i < current.GetComponent<CubeControl>().possiblePaths.Count; i++)
        {
            WalkPath walkPath = current.GetComponent<CubeControl>().possiblePaths[i];

            // 이미 지나온 길이 아니고 길이 연결되어 있다면
            if (!visitedCubes.Contains(walkPath.target) && walkPath.active)
            {
                // 다음 검색 큐브로
                nextCubes.Add(walkPath.target);

                // 이동할 경로에 추가
                walkPath.target.GetComponent<CubeControl>().previousBlock = current;
            }

        }

        // 방문한 큐브 리스트에 현재 큐브 추가
        visitedCubes.Add(current);

        // 리스트가 하나라도 있다면
        if (nextCubes.Count > 0)
        {
            ExploreCube(nextCubes, visitedCubes);
        }
    }

    // 경로 생성
    private void BuildPath()
    {
        Transform cube = clickedCube;

        // 클릭한 큐브가 현재큐브와 같지 않을 때까지
        while (cube != currentCube)
        {
            // 실제 이동할 경로에 삽입
            finalPath.Add(cube);

            // 클릭한 큐브의 이전큐브가 None일 때
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

    // 자식 오브젝트까지 모두 레이어 설정
    void SetLayerObject(Transform tr, string layerName)
    {
        tr.gameObject.layer = LayerMask.NameToLayer(layerName);

        for (int i = 0; i < tr.childCount; i++)
        {
            // 자식의 자식들까지 설정
            SetLayerObject(tr.GetChild(i), layerName);
        }
    }
}
