using UnityEngine;

public class EngineSounds : MonoBehaviour
{
    public AudioSource source;

    Ship ship;

    private float startVolume = 1.0f;
    private float startPitch = 1.0f;

    private void Awake()
    {
        ship = GetComponentInParent<Ship>();
    }

    private void Start()
    {
        if (source != null)
        {
            startVolume = source.volume;
            startPitch = source.pitch;
        }
    }

    private void Update()
    {
        if (ship != null && source != null)
        {
            source.volume = startVolume * Mathf.Lerp(0.5f, 1.0f, ship.Throttle);
            source.pitch = startPitch * Mathf.Lerp(0.75f, 1.0f, ship.Throttle);
        }
    }


}