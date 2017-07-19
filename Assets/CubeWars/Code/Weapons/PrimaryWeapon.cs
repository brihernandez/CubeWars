using UnityEngine;
using System.Collections.Generic;

public class PrimaryWeapon : MonoBehaviour
{
    [Tooltip("Prefab used for firing.")]
    public Bullet bulletPrefab;

    [Space]
    [Tooltip("Time between each shot.")]
    public float fireDelay = 0.33f;
    [Tooltip("Speed of the bullet (m/s) when fired.")]
    public float muzzleVelocity = 500.0f;
    [Tooltip("Fire from all fire points at the same time, or cycle between them.")]
    public bool linkFire = true;
    [HideInInspector]
    public bool fire = false;

    [Space]
    public Transform[] firePoints;

    [Space]
    [Tooltip("This audio source will be played when the gun is fired.")]
    public AudioSource fireAudioSource;

    private Ship ship;
    private Queue<Transform> fireQueue;
    private float cooldown = 0.0f;
    private float startPitch = 1.0f;

    private void Awake()
    {
        fireQueue = new Queue<Transform>(firePoints.Length);
        foreach (Transform point in firePoints)
            fireQueue.Enqueue(point);

        ship = GetComponent<Ship>();
    }

    private void Start()
    {
        if (fireAudioSource != null)
        {
            startPitch = fireAudioSource.pitch;
        }
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;

        if (fire && cooldown <= 0.0f)
        {
            if (linkFire)
            {
                // Fire guns by just firing from all points at once.
                foreach (Transform point in firePoints)
                {
                    SpawnAndFireBullet(point.position, point.rotation);
                }
            }
            else
            {
                // Fire the guns by cycling through fire points.
                Transform point = fireQueue.Dequeue();
                SpawnAndFireBullet(point.position, point.rotation);
                fireQueue.Enqueue(point);
            }

            cooldown = fireDelay;
        }
    }

    private void SpawnAndFireBullet(Vector3 position, Quaternion rotation)
    {
        if (bulletPrefab != null)
        {
            Bullet bullet = Instantiate(bulletPrefab);
            bullet.Fire(position, rotation, transform.forward * muzzleVelocity, ship);

            if (fireAudioSource != null)
            {
                fireAudioSource.pitch = startPitch * Random.Range(0.95f, 1.05f);
                fireAudioSource.Play();
            }
        }
    }
}
