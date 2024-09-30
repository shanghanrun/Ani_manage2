using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] PlayerScript PS;
    public AudioSource audioSource;       // 공용 AudioSource
    public AudioClip[] soundEffects;      // 다양한 소리를 저장할 배열

    // 사운드 효과의 인덱스를 미리 정의해놓음 (이름으로도 쉽게 관리 가능)
    public enum SoundEffect
    {
        TitleMusic = 0,
        SparkEffect1 = 1,   // 레이저
        SparkEffect2 = 2,   // 얼음 변화
        OpenBox = 3,        // 박스 열기
        GetItem = 4,        // 아이템 획득
        Steps = 5,          // 착지 소리
        RunSound = 6,       // 달리기 소리
        MoveLever = 7,      // 레버 당기기
        Panting = 8,        // 헉헉숨소리
        TransferSound = 9,  // 순간이동
        NatureSound = 10    // 자연 소리
    }

    public float fadeDuration = 8f; // 음악 페이드아웃 시간 (초)
    public float initialWait = 100f; // 주제 음악 재생 시간 (초)
    public float natureVolume = 0.3f; // 자연 소리 볼륨

    private void Start()
    {
        // MainCamera에서 CameraController를 찾아 PS를 가져옴
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            PS = cameraController.PS; // PS를 CameraController에서 가져옵니다.
        }
        else
        {
            Debug.LogError("CameraController를 찾을 수 없습니다.");
        }
    }

    // 특정 효과음을 인덱스로 재생하는 함수
    public void PlaySoundEffect(SoundEffect effect)
    {
        int index = (int)effect; // Enum을 인덱스로 변환

        // 유효한 인덱스인지 체크
        if (index >= 0 && index < soundEffects.Length && soundEffects[index] != null)
        {
            audioSource.PlayOneShot(soundEffects[index]); // 해당 인덱스의 사운드 재생
        }
        else
        {
            Debug.LogWarning("해당 인덱스에 음원이 없거나 유효하지 않습니다.");
        }
    }

    // 자연 소리처럼 지속적인 배경음악을 재생할 때 사용 (반복 재생)
    public void PlayLoopingSoundEffect(SoundEffect effect)
    {
        int index = (int)effect;

        if (index >= 0 && index < soundEffects.Length && soundEffects[index] != null)
        {
            audioSource.clip = soundEffects[index];
            audioSource.loop = true;   // 반복 재생 설정
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("해당 인덱스에 음원이 없거나 유효하지 않습니다.");
        }
    }

    // 특정 효과음 정지
    public void StopLoopingSoundEffect()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }


    // 주제 음악을 재생하는 메서드
    public void PlayTitleMusic()
    {
        PlaySoundEffect(SoundEffect.TitleMusic);
    }


    public IEnumerator FadeOutMusicAfterDelay()
    {
        yield return new WaitForSeconds(initialWait);

        float currentTime = 0f;
        float startVolume = audioSource.volume;

        // 주제음악 페이드아웃
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);

            // 자연소리 볼륨 점차 증가
            // if (soundEffects[(int)SoundEffect.NatureSound] != null)
            // {
            //     // 자연 소리 클립을 새 AudioSource에 재생 중인지 확인
            //     AudioSource natureSoundSource = gameObject.AddComponent<AudioSource>();
            //     natureSoundSource.clip = soundEffects[(int)SoundEffect.NatureSound];
            //     natureSoundSource.volume = Mathf.Lerp(0f, natureVolume, currentTime / fadeDuration);
            //     natureSoundSource.loop = true; // 자연 소리 반복 재생
                // if (!natureSoundSource.isPlaying)
                // {
                //     natureSoundSource.Play();
                // }
            // }
            yield return null;
        }
        // 페이드아웃이 완료되면 주제음악 중지
        audioSource.Stop();
        audioSource.volume = startVolume; // 볼륨 초기화(다시 사용할 경우 대비)

    }
    void Update(){  // MoveLever 한번 플레이
        if(PS !=null){
            if (PS.leverActivated && ! audioSource.isPlaying)
            {
                // MoveLever 한번 플레이
                // PlaySoundEffect(SoundEffect.MoveLever);
                PlaySoundEffect(SoundEffect.SparkEffect1);
                // PS.leverActivated = false;  이것은 CameraController에서 spot에 도달했을 때 false로 만든다.
                
            }
            else if (PS.openBox)
            {
                PlaySoundEffect(SoundEffect.OpenBox); // OpenBox 사운드 재생
                PS.openBox = false;
            }
            else if (PS.gotKey)
            {
                print("아이템 사운드 플레이");
                PlaySoundEffect(SoundEffect.GetItem);
                PS.gotKey = false; 
                PS.canOpenDoor = true;
            }
            else if (PS.touchDoor)
            {
                PS.touchDoor = false;
                PlaySoundEffect(SoundEffect.TransferSound);

            }
        }
        
        
    }
}