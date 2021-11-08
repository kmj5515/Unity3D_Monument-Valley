using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // sound는 list를 이용해 관리
    public List<Sound> sounds = new List<Sound>();

    AudioSource bgm;
    AudioSource effect;

    private void Awake()
    {      
        if (instance == null)
        {
            instance = this;

            // 씬을 전환해도 가져올수 있게 DontDestroyOnLoad 설정
            DontDestroyOnLoad(gameObject);

            bgm = gameObject.AddComponent<AudioSource>();
            effect = gameObject.AddComponent<AudioSource>();

            // 첫 실행때 재생 방지
            bgm.playOnAwake = false;
            effect.playOnAwake = false;

            play("IntroBGM");
        }
    }

    public void play(string audioName, float volume = 1.0f)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            // soundName을 찾아서 
            if (audioName == sounds[i].audio.name)
            {
                if (sounds[i].isBGM)
                {
                    bgm.Stop();

                    bgm.clip = sounds[i].audio;
                    bgm.loop = true;
                    bgm.volume = volume;

                    bgm.Play();
                }
                else
                {
                    //버튼 사운드 중복 해결을 위해 PlayOneShot함수 이용
                    effect.PlayOneShot(sounds[i].audio, volume);
                }

                break;
            }
        }
    }

    public void stop(string audioName)
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (audioName == sounds[i].audio.name)
            {
                if (sounds[i].isBGM)
                {
                    bgm.Stop();
                }
                else
                {
                    effect.Stop();
                }

                break;
            }
        }
    }
}

[System.Serializable]
public class Sound
{
    public AudioClip audio;
    public bool isBGM = false;
}