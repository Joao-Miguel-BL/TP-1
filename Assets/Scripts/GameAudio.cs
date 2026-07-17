using UnityEngine;

public class GameAudio : MonoBehaviour
{
    [Header("Fontes de áudio")]
    [SerializeField] private AudioSource crowdSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Áudios")]
    [SerializeField] private AudioClip crowdClip;
    [SerializeField] private AudioClip kickClip;
    [SerializeField] private AudioClip goalClip;

    [Header("Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float crowdVolume = 0.2f;

    [Range(0f, 1f)]
    [SerializeField] private float kickVolume = 0.8f;

    [Range(0f, 1f)]
    [SerializeField] private float goalVolume = 1f;

    private void Start()
    {
        PlayCrowd();
    }

    private void PlayCrowd()
    {
        if (crowdSource == null || crowdClip == null)
        {
            return;
        }

        crowdSource.clip = crowdClip;
        crowdSource.volume = crowdVolume;
        crowdSource.loop = true;
        crowdSource.Play();
    }

    public void PlayKick()
    {
        PlayEffect(kickClip, kickVolume);
    }

    public void PlayGoal()
    {
        PlayEffect(goalClip, goalVolume);
    }

    public void PlayVictory()
    {
        // Usa o mesmo áudio de gol na vitória.
        PlayEffect(goalClip, goalVolume);
    }

    private void PlayEffect(AudioClip clip, float volume)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }
}