using UnityEngine;

public class EngineSounds : MonoBehaviour
{
    public AudioSource internalAudioSource;
    public AudioSource externalAudioSource;

    Ship ship;

    EngineSoundSource internalEngine;
    EngineSoundSource externalEngine;

    private void Awake()
    {
        ship = GetComponentInParent<Ship>();
    }

    private void Start()
    {
        if (externalAudioSource != null)
        {
            externalEngine = new EngineSoundSource(externalAudioSource, 0.75f, 0.5f);
        }

        if (internalAudioSource != null)
        {
            internalEngine = new EngineSoundSource(internalAudioSource, 0.75f, 0.5f);
        }
    }

    private void Update()
    {
        if (ship != null)
        {
            // Play internal audio only if this is the player.
            if (internalEngine != null)
            {
                internalEngine.PlayAudio(ship.IsPlayer);
                internalEngine.Update(ship.Throttle);
            }

            // Play external audio only if this isn't the player.
            if (externalEngine != null)
            {
                externalEngine.PlayAudio(!ship.IsPlayer);
                externalEngine.Update(ship.Throttle);
            }
        }
    }
}

/// <summary>
/// Simple wrapper for audio source to handle engine sounds.
/// </summary>
public class EngineSoundSource
{
    AudioSource source;

    float startVol;
    float startPitch;

    float minPitch;
    float minVolume;

    /// <summary>
    /// Simple wrapper for audio source to handle engine sounds.
    /// </summary>
    /// <param name="audioSource">Audio Source on which the pitch and volume will be manipulated</param>
    /// <param name="minPitch">Multiplier for the start pitch when at minimum throttle</param>
    /// <param name="minVolume">Multiplier for the start volume when at minimum throttle</param>
    public EngineSoundSource(AudioSource audioSource, float minPitch, float minVolume)
    {
        source = audioSource;
        source.playOnAwake = false;

        startVol = source.volume;
        startPitch = source.pitch;

        this.minPitch = minPitch;
        this.minVolume = minVolume;
    }

    /// <summary>
    /// Updates the volume and pitch of the audio source based on throttle.
    /// </summary>
    /// <param name="throttle">Ship's throttle</param>
    public void Update(float throttle)
    {
        source.volume = startVol * Mathf.Lerp(minVolume, 1.0f, throttle);
        source.pitch = startPitch * Mathf.Lerp(minPitch, 1.0f, throttle);
    }

    /// <summary>
    /// Should audio source play
    /// </summary>
    public void PlayAudio(bool play)
    {
        if (play)
            Enable();
        else
            Disable();
    }

    /// <summary>
    /// Enable playing of the audio source
    /// </summary>
    public void Enable()
    {
        if (!source.isPlaying)
            source.Play();
    }

    /// <summary>
    /// Disable playing of the audio souce
    /// </summary>
    public void Disable()
    {
        if (source.isPlaying)
            source.Stop();
    }
}