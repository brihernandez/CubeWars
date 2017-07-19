using UnityEngine;

public class EffectsDeleter : MonoBehaviour
{
    ParticleSystem pSystem;

    [Tooltip("When true, will destroy the entire gameobject this is attached to.\n\nWhen false, destroys only the first particle system it sees.")]
    public bool destroyGameObject = true;

    bool hasStarted = false;

    public void Awake()
    {
        pSystem = GetComponentInChildren<ParticleSystem>();
    }

    public void Start()
    {
        // Account for a particle system that isn't playing on startup. Wait for them to have
        // started playing at least once before deleting them.
        if (pSystem != null && pSystem.isPlaying)
            hasStarted = true;
    }

    public void Update()
    {
        if (pSystem != null && hasStarted && !pSystem.IsAlive(true))
            Destroy(gameObject);
    }
}