using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class LeverRotate : MonoBehaviour
{
    public enum AxisOfRotate
    {
        X, Y, Z
    }

    public Transform rotateObj;

    public AxisOfRotate axisOfRotate;
    public float rotSpeed;

    bool isRotate;

    CubeControl ctrl;

    // Start is called before the first frame update
    void Start()
    {
        isRotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;

            if (Physics.Raycast(mouseRay, out mouseHit))
            {
                if (mouseHit.transform == transform)
                {
                    isRotate = true;
                }
                else
                {
                    // 자식들의 transform까지 검사
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        if (mouseHit.transform == transform.GetChild(i))
                        {
                            isRotate = true;

                            break;
                        }
                    }
                }
            }
        }

        // 마우스 클릭 중인 상태로 레버와 오브젝트 회전
        if (isRotate)
        {
            Vector2 rot = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            rot *= rotSpeed;

            // 회전할 레버
            transform.Rotate(
                ((axisOfRotate == AxisOfRotate.X) ? (rot.x) : (0)),
                ((axisOfRotate == AxisOfRotate.Y) ? (rot.x) : (0)),
                ((axisOfRotate == AxisOfRotate.Z) ? (rot.x) : (0)));

            // 회전할 블럭
            rotateObj.Rotate(
                ((axisOfRotate == AxisOfRotate.X) ? (rot.x) : (0)),
                ((axisOfRotate == AxisOfRotate.Y) ? (rot.x) : (0)),
                ((axisOfRotate == AxisOfRotate.Z) ? (rot.x) : (0)));
        }
        else
        {
            //블록 자동 조절
            rotateAngle();
        }

        // 마우스를 떼면 더 이상 움직이지 않음
        if (Input.GetMouseButtonUp(0))
        {   
            isRotate = false;
            SoundManager.instance.play("Rotate Sound_1", 0.5f);
        }    
    }

    private float getAngle()
    {
        float ret = 0;

        // axisOfRotate 확인해서 각축의 값일때 ret로 리턴
        switch (axisOfRotate)
        {
            case AxisOfRotate.X: ret = transform.eulerAngles.x; break;
            case AxisOfRotate.Y: ret = transform.eulerAngles.y; break;
            case AxisOfRotate.Z: ret = transform.eulerAngles.z; break;
        }

        return ret;
    }

    private void setAngle(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(
            (axisOfRotate == AxisOfRotate.X) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Y) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Z) ? angle : 0));

        rotateObj.rotation = Quaternion.Euler(new Vector3(
            (axisOfRotate == AxisOfRotate.X) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Y) ? angle : 0,
            (axisOfRotate == AxisOfRotate.Z) ? angle : 0));
    }

    private void rotateAngle()
    {
        if ((getAngle() < 360 && getAngle() > 340) || (getAngle() < 20 && getAngle() > 0))
        {
            setAngle(360);
        }
        if (getAngle() < 290 && getAngle() > 250)
        {
            setAngle(270);
        }
        if (getAngle() < 200 && getAngle() > 160)
        {
            setAngle(180);
        }
        if (getAngle() < 110 && getAngle() > 70)
        {
            setAngle(90);
        }
    }

    IEnumerator Rotate(float startAngle, float finalAngle)
    {
        float movetime = 0f;

        while (movetime < 1.0f)
        {
            float angle = Mathf.Lerp(startAngle, finalAngle, movetime);

            setAngle(angle);

            movetime += Time.deltaTime * rotSpeed;

            if (movetime >= 1.0f) setAngle(finalAngle);

            yield return null;
        }
        
        yield return null;
    }

    public void autoRotate(float targetAngle)
    {
        StartCoroutine(Rotate(getAngle(), targetAngle));
    }
}