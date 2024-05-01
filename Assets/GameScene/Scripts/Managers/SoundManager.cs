using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

[Serializable]
public class AudioDetail
{
    public AudioClip Clip;
    public double LastPlayedTime;
    public double SequenceInterval;
    public int CurrentSequence;
    public int CompleteCount;
    public float PitchMin;
    public float PitchIncrease;

}

public class SoundManager : MonoSingleton<SoundManager>
{
    public event Action OnSequenceCompleted;

    public AudioSource efxSource;
    public AudioSource musicSource;
    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

    private bool isDisabled;
    public bool IsDisabled
    {
        get
        {
            return isDisabled;
        }
        set
        {
            isDisabled = value;
            if (isDisabled)
            {
                musicSource.Pause();
            }
            else
            {
                musicSource.UnPause();
            }
        }
    }

    public override void Init()
    {
        IsDisabled = false;
        musicSource.Play();
    }

    public void PlayMusic(AudioDetail clip)
    {
        if (IsDisabled) return;

        musicSource.pitch = 1;
        musicSource.volume = .3f;
        musicSource.clip = clip.Clip;
        musicSource.Play();
    }

    public void PlaySingle(AudioDetail clip)
    {
        if (IsDisabled) return;


        efxSource.pitch = 1;
        efxSource.clip = clip.Clip;
        efxSource.Play();
    }

    public void RandomizeSfx(params AudioDetail[] clips)
    {
        if (IsDisabled) return;

        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex].Clip;

        efxSource.Play();
    }

    public void PlaySequencial(AudioDetail clip)
    {
        if (IsDisabled) return;

        var currentTime = Time.time * 1000;
        if (currentTime > clip.LastPlayedTime + clip.SequenceInterval)
        {
            clip.CurrentSequence = 1;
        }
        else
        {
            clip.CurrentSequence++;
        }
        clip.LastPlayedTime = currentTime;

        efxSource.pitch = clip.PitchMin + (clip.CurrentSequence - 1) * clip.PitchIncrease;
        efxSource.clip = clip.Clip;
        efxSource.Play();

        if (clip.CurrentSequence >= clip.CompleteCount)
        {
            if (OnSequenceCompleted != null) OnSequenceCompleted();
        }

    }

}