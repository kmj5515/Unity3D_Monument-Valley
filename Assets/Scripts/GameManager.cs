using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Space]
    public Transform clearCube;

    public Image fadeImg;
    public Image fadeReload;
    public Image fadeIntro;
    public List<Text> fadeText = new List<Text>();

    [Space]
    private bool isClear;
    private bool isReady;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        fadeImg.gameObject.SetActive(true);

        fadeImg.color = new Color(0, 0, 0, 1.0f);

        for (int i = 0; i < fadeText.Count; i++)
        {
            // ���� ȭ�鿡 ���ڸ� ���̵� ��
            fadeText[i].color = new Color(1.0f, 1.0f, 1.0f, 0);
            StartCoroutine(FadeIn(fadeText[i], 1.0f, 0.6f));
        }

        isClear = isReady = false;
    }

    private void Update()
    {
        if (!isReady)
        {
            // ���� ���̵� ���� �Ϸ�Ǿ��ٸ�
            if (fadeText[0].color.a == 1.0f && fadeImg.color.a == 1.0f)
            {
                // ���� ȭ���� ���̵� �ƿ�
                StartCoroutine(FadeOut(fadeImg, 0.35f, 0.4f));
            }

            // ���ڿ� ���� ȭ���� ��� ���õǾ� �ִٸ�
            if (fadeText[0].color.a == 1.0f && fadeImg.color.a == 0.35f)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    for (int i = 0; i < fadeText.Count; i++)
                    {
                        // ���� ���̵� �ƿ�
                        StartCoroutine(FadeOut(fadeText[i], 0, 0.65f));
                    }

                    // UI ���̵� ��
                    StartCoroutine(FadeIn(fadeReload, 1.0f, 0.65f));
                    StartCoroutine(FadeIn(fadeIntro, 1.0f, 0.65f));

                    // ���� ȭ�� ���̵� �ƿ�
                    StartCoroutine(FadeOut(fadeImg, 0, 0.65f));

                    SoundManager.instance.play("BackClick");
                }
            }

            // ���ڿ� ���� ȭ���� ��� ������ �ʴ´ٸ�
            if (fadeText[0].color.a == 0 && fadeImg.color.a == 0)
            {
                // ���� ��Ʈ�� ����
                isReady = true;
            }
        }

        // Ŭ���� 
        if (isClear && isReady)
        {
            StartCoroutine(FadeIn(fadeImg, 1.0f, 0.4f));

            if (fadeImg.color.a == 1.0f)
            {
                SoundManager.instance.stop(SceneManager.GetActiveScene().name + "BGM");

                SceneManager.LoadScene("Intro");
            }
        }
    }

    // ���� -> ������
    IEnumerator FadeIn(Image target, float endAlpha, float speed)
    {
        Color fadeColor = target.color;
        Color originColor = target.color;

        float timing = 0;

        while (target.color.a < endAlpha)
        {
            //�̹���a
            fadeColor.a = Mathf.Lerp(originColor.a, endAlpha, timing);

            target.color = fadeColor;

            timing += Time.deltaTime * speed;

            yield return null;
        }

        fadeColor.a = endAlpha;
        target.color = fadeColor;

        yield return null;
    }

    // ������ -> ����
    IEnumerator FadeOut(Image target, float endAlpha, float speed)
    {
        Color fadeColor = target.color;
        Color originColor = target.color;

        float timing = 0;

        while (target.color.a > endAlpha)
        {
            //�̹���a
            fadeColor.a = Mathf.Lerp(originColor.a, endAlpha, timing);

            target.color = fadeColor;

            timing += Time.deltaTime * speed;

            yield return null;
        }

        fadeColor.a = endAlpha;
        target.color = fadeColor;

        yield return null;
    }

    // ���� >> ������
    IEnumerator FadeIn(Text target, float endAlpha, float speed)
    {
        Color fadeColor = target.color;
        Color originColor = target.color;

        float timing = 0;

        while (target.color.a < endAlpha)
        {
            fadeColor.a = Mathf.Lerp(originColor.a, endAlpha, timing);

            target.color = fadeColor;

            timing += Time.deltaTime * speed;

            yield return null;
        }

        fadeColor.a = endAlpha;
        target.color = fadeColor;

        yield return null;
    }

    // ������ >> ����
    IEnumerator FadeOut(Text target, float endAlpha, float speed)
    {
        Color fadeColor = target.color;
        Color originColor = target.color;

        float timing = 0;

        while (target.color.a > endAlpha)
        {
            fadeColor.a = Mathf.Lerp(originColor.a, endAlpha, timing);

            target.color = fadeColor;

            timing += Time.deltaTime * speed;

            yield return null;
        }

        fadeColor.a = endAlpha;
        target.color = fadeColor;

        yield return null;
    }


    public bool Clear
    {
        get { return isClear; }
        set { isClear = value; }
    }

    public bool Ready
    {
        get { return isReady; }
        set { isReady = value; }
    }
}
