using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SuperFight;

public class AudioManager : Singleton<AudioManager>
{
    public static int MusicSetting
    {
        get { return PlayerPrefs.GetInt("music_setting", 1); }
        set { PlayerPrefs.SetInt("music_setting", value); }
    }
    public static int SoundSetting
    {
        get { return PlayerPrefs.GetInt("sound_setting", 1); }
        set { PlayerPrefs.SetInt("sound_setting", value); }
    }
    [SerializeField] AudioContainerSO commonSound;
    [SerializeField] AudioContainerSO musics;
    [SerializeField] AudioSource soundPlayer;
    [SerializeField] AudioSource musicPlayer;
    private List<AudioSource> activeAudioSources = new List<AudioSource>();
    private List<AudioSource> deactiveAudioSources = new List<AudioSource>();
    protected override void Awake()
    {
        base.Awake();
        deactiveAudioSources.Add(musicPlayer);
        for (int i = 0; i < 2; i++)
        {
            AudioSource audioSource = musicPlayer.gameObject.AddComponent<AudioSource>();
            deactiveAudioSources.Add(audioSource);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void PlayOneShot(AudioClip audioClip, float volume, float delay = 0, Transform target = null)
    {
        if (SoundSetting != 1) return;
        if (audioClip == null) return;
        StartCoroutine(IEDeplayPlayOneShot(audioClip, volume, delay, target));
    }
    public void PlayOneShot(string clipName, float volume, float delay = 0, Transform target = null)
    {
        if (SoundSetting != 1) return;
        AudioClip clip = commonSound.GetClip(clipName);
        if (clip != null)
        {
            StartCoroutine(IEDeplayPlayOneShot(clip, volume, delay, target));
        }
    }
    IEnumerator IEDeplayPlayOneShot(AudioClip audioClip, float volume, float delay = 0, Transform target = null)
    {
        // if (target != null)
        // {
        //     if (!CameraController.Instance.CheckVisibleOnCamera(target.position))
        //     {
        //         yield break;
        //     }
        // }
        float timer = delay;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        float newVolume = volume;
        soundPlayer.PlayOneShot(audioClip, newVolume);
    }
    public void PlayMusic(string clipName, float volume, bool isLoop)
    {
        if (MusicSetting != 1)
        {
            volume = 0;
        }
        AudioClip clip = musics.GetClip(clipName);
        if (clip == null) return;
        if (IsPlaying(clip.name)) return;
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.loop = isLoop;
        source.volume = volume;
        source.Play();
    }
    public void PlayMusic(AudioClip clip, float volume, bool isLoop)
    {
        if (MusicSetting != 1)
        {
            volume = 0;
        }
        if (clip == null) return;
        if (IsPlaying(clip.name)) return;
        AudioSource source = GetAudioSource();
        source.clip = clip;
        source.loop = isLoop;
        source.volume = volume;
        source.Play();
    }
    // public void PlayMusic(string clipName, float volume, bool isLoop, float fadeInTime)
    // {
    //     AudioClip clip = musics.GetClip(clipName);
    //     PlayMusic(clip, volume, isLoop, fadeInTime);
    // }
    // public void PlayMusic(AudioClip clip, float volume, bool isLoop, float fadeInTime)
    // {
    //     if (MusicSetting != 1)
    //     {
    //         volume = 0;
    //     }
    //     if (clip == null) return;
    //     if (IsPlaying(clip.name)) return;
    //     AudioSource source = GetAudioSource();
    //     StartCoroutine(IEFadeIn());
    //     IEnumerator IEFadeIn()
    //     {
    //         float timer = 0;
    //         float v = 0;
    //         while (timer < fadeInTime)
    //         {
    //             timer += Time.deltaTime;
    //             v = Mathf.Lerp(0, volume, timer / fadeInTime);
    //             source.volume = v;
    //             yield return null;
    //         }
    //     }
    //     source.clip = clip;
    //     source.loop = isLoop;
    //     source.Play();
    // }
    private AudioSource GetAudioSource()
    {
        AudioSource audioSource = null;
        if (deactiveAudioSources.Count > 0)
        {
            audioSource = deactiveAudioSources[0];
            deactiveAudioSources.RemoveAt(0);
        }
        else
        {
            audioSource = musicPlayer.gameObject.AddComponent<AudioSource>();
        }
        activeAudioSources.Add(audioSource);
        return audioSource;
    }

    private bool IsPlaying(string clipName)
    {
        for (int i = 0; i < activeAudioSources.Count; i++)
        {
            if (activeAudioSources[i].clip.name == clipName)
            {
                return activeAudioSources[i].isPlaying;
            }
        }
        return false;
    }

    public void StopAll()
    {
        StopAllMusic();
        StopSound();
    }
    public void StopSound()
    {
        soundPlayer.Stop();
    }
    public void StopAllMusic()
    {
        for (int i = 0; i < activeAudioSources.Count; i++)
        {
            activeAudioSources[i].Stop();
        }
        deactiveAudioSources.AddRange(activeAudioSources);
        activeAudioSources.Clear();
    }
    public void StopMusic(string musicName)
    {
        for (int i = 0; i < activeAudioSources.Count; i++)
        {
            if (activeAudioSources[i].clip.name == musicName)
            {
                activeAudioSources[i].Stop();
                deactiveAudioSources.Add(activeAudioSources[i]);
                activeAudioSources.RemoveAt(i);
                break;
            }
        }
    }
    public void StopMusic(string musicName, float fadeOutTime)
    {
        AudioSource audioSource = null;
        for (int i = 0; i < activeAudioSources.Count; i++)
        {
            if (activeAudioSources[i].clip.name == musicName)
            {
                audioSource = activeAudioSources[i];
                break;
            }
        }
        if (audioSource)
        {
            StartCoroutine(IEFadeOut());
            IEnumerator IEFadeOut()
            {
                float timer = 0;
                float v = audioSource.volume;
                while (timer < fadeOutTime)
                {
                    timer += Time.deltaTime;
                    v = Mathf.Lerp(v, 0, timer / fadeOutTime);
                    audioSource.volume = v;
                    yield return null;
                }
                activeAudioSources.Remove(audioSource);
            }
        }
    }
    public void ResumeMusic()
    {
        musicPlayer.Play();
    }
    public void EnableMusic(bool status)
    {
        MusicSetting = status ? 1 : 0;
        if (MusicSetting != 1)
        {
            for (int i = 0; i < activeAudioSources.Count; i++)
            {
                activeAudioSources[i].volume = 0;
            }
        }
        else
        {
            for (int i = 0; i < activeAudioSources.Count; i++)
            {
                activeAudioSources[i].volume = 1;
            }
        }
    }
    public void EnableSound(bool status)
    {
        SoundSetting = status ? 1 : 0;
    }
}
