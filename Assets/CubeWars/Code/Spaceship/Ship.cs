using UnityEngine;

[RequireComponent(typeof(ShipInput))]
public class Ship : MonoBehaviour
{
    [Header("Health")]
    [Tooltip("Amount of damage the ship can take before being destroyed.")]
    public float hitpoints = 100.0f;
    [Tooltip("Amount of damage the shield can take before the shield goes down.")]
    public float shield = 0.0f;
    [Tooltip("Units per second the shield regerates over time.")]
    public float shieldRegeneration = 10.0f;
    [Tooltip("How long after the shield is fully depleted before it starts recharging again.")]
    public float shieldDownTime = 10.0f;

    [Header("Effects")]
    [Tooltip("Particle effect to play when shield goes down.")]
    public ParticleSystem shieldDownEffect;
    [Tooltip("Particle effect to play when the ship dies.")]
    public ParticleSystem explosionEffect;

    [Header("Flight")]
    [Tooltip("Max speed in m/s")]
    public float maxSpeed = 100.0f;
    [Tooltip("Turn rate in deg/s")]
    public float turnSpeed = 30.0f;
    [Tooltip("How quickly the ship can change speed.")]
    public float acceleration = 20.0f;
    [Range(0.1f, 10.0f)]
    public float responsiveness = 3.0f;

    public float Throttle { get { return input.throttle; } }
    public bool IsPlayer { get { return input.isPlayer; } }

    private float speed = 0.0f;
    private Vector3 rotationSpeed = Vector3.zero;
    private Vector3 targetRotSpeed = Vector3.zero;

    private float hp;
    private float shieldHp;
    private float shieldCooldown = 0.0f;

    ShipInput input;
    PrimaryWeapon weapon;

    private void Awake()
    {
        input = GetComponent<ShipInput>();
        weapon = GetComponent<PrimaryWeapon>();
    }

    private void Start()
    {
        hp = hitpoints;
        shieldHp = shield;
    }

    private void Update()
    {
        // Movement
        speed = Mathf.MoveTowards(speed, input.throttle * maxSpeed, acceleration * Time.deltaTime);
        UpdateShipPosition();

        // Weapons
        if (weapon != null)
            weapon.fire = input.fire;

        // Recharge shield if it's not on cooldown.
        shieldCooldown -= Time.deltaTime;
        if (shieldCooldown <= 0.0f)
            shieldHp = Mathf.MoveTowards(shieldHp, shield, shieldRegeneration * Time.deltaTime);

    }

    private void UpdateShipPosition()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        transform.Rotate(input.stickAndRudder * turnSpeed * Time.deltaTime, Space.Self);
    }

    public void ApplyDamage(float damage)
    {
        // Apply damage to shield first, but only if there was a shield at all.
        if (shield > 0.0f && shieldHp > 0.0f)
        {
            shieldHp -= damage;
            shieldHp = Mathf.Max(shieldHp, 0.0f);

            // Shield depleted
            if (shieldHp <= 0.0f)
            {
                shieldCooldown = shieldDownTime;

                if (shieldDownEffect != null)
                    Instantiate(shieldDownEffect, transform.position, transform.rotation);
            }
        }
        else
        {
            hitpoints -= damage;
            hitpoints = Mathf.Max(hitpoints, 0.0f);
        }

        
        // Ship was destroyed.
        if (hitpoints <= 0.0f)
        {
            DestroyShip();
        }
    }

    public void DestroyShip()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}