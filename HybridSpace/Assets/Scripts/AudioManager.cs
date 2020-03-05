using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public enum ClipType { Effect, Music, UI, Ambience }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource musicSource2;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource effectSource;

    [Space]

    public AudioClip DefaultMusicClip;
    public AudioClip DistortedMusicClip;
    public AudioClip DefaultAmbienceClip;

    [Space]

    public AudioClip LightsToggleClip;
    public AudioClip PowerCharge;
    public AudioClip PowerGood;
    public AudioClip PowerOff;
    public AudioClip PowerBackOn;
    public AudioClip GoreClip;
    public AudioClip Jumpscare;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        musicSource.loop = true;
        musicSource.ignoreListenerPause = true;

        ambienceSource.loop = true;
        ambienceSource.ignoreListenerPause = true;

        PlayMusic(DefaultMusicClip);
        //PlayAmbience(DefaultAmbienceClip);
    }

    public void PlayClip(AudioClip _clip, ClipType _type = ClipType.Effect)
    {
        switch (_type)
        {
            default:
            case ClipType.Effect:
                effectSource.PlayOneShot(_clip);
                break;
            case ClipType.Music:
                PlayMusic(_clip);
                break;
        }
    }

    public void PlayRandomClip(AudioClip[] _clips, ClipType _type = ClipType.Effect)
    {
        if (_clips == null || _clips.Length == 0) return;
        AudioClip _rClip = _clips[Random.Range(0, _clips.Length)];

        switch (_type)
        {
            default:
            case ClipType.Effect:
                effectSource.PlayOneShot(_rClip);
                break;
            case ClipType.Music:
                PlayMusic(_rClip);
                break;
        }
    }
    public void PlayMusic(AudioClip _clip)
    {
        musicSource.clip = _clip;
        musicSource.Play();
    }

    public void ChangeToCreepyMusic(AudioClip _clip, float _duration)
    {
        StartCoroutine(IEChangeToCreepyMusic(_clip, _duration));
    }

    private IEnumerator IEChangeToCreepyMusic(AudioClip _clip, float _duration)
    {
        musicSource2.clip = _clip;
        musicSource2.volume = 0;
        musicSource2.Play();

        ambienceSource.clip = DefaultAmbienceClip;
        ambienceSource.volume = 0;
        ambienceSource.Play();

        float _timer = 0;

        while(_timer < _duration)
        {
            _timer += Time.deltaTime;

            musicSource.volume = 1f - (1f / _duration * _timer);

            musicSource2.volume = 1f / _duration * _timer;
            ambienceSource.volume = (1f / _duration * _timer) /2;
            yield return null;
        }
        musicSource.Stop();

        yield return null;
    }

    public void PlayAmbience(AudioClip _clip)
    {
        ambienceSource.clip = _clip;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}

