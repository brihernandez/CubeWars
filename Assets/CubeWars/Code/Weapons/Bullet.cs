using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timeToLive = 2.0f;
    public float damage = 10.0f;

    public ParticleSystem hitEffect;

    Ship firedFrom;

    Vector3 velocity = Vector3.zero;
    float fireTime = 0.0f;
    bool fired = false;

    public bool Fired { get { return fired; } }
    public Ship FiredFrom { get { return (firedFrom) ? firedFrom : null; } }

    const float VELOCITY_MULT = 2.0f;

    private void Update()
    {
        if (fired)
        {
            if (Time.time - fireTime > timeToLive)
            {
                DestroyBullet(true);
            }
            else
            {
                MoveBullet();
            }
        }
    }

    private void MoveBullet()
    {
        transform.position += velocity * Time.deltaTime;

        // Perform a raycast for hit detection. Note this is no longer the forward, instead it's velocity.
        // Bullets can now travel in directions other than forwards because of inherited velocity.
        RaycastHit rayHit;
        Ray velocityRay = new Ray(transform.position, velocity.normalized);

        // Perform the raycast. Shoots a ray forwards of the bullet that covers all the distance
        // that it will cover in the next frame. This guarantees a hit in all but the most extenuating
        // circumstances (against other extremely fast and small moving targets it may miss) and works
        // at practically any bullet speed.
        bool rayHasHit = Physics.Raycast(velocityRay, out rayHit, velocity.magnitude * VELOCITY_MULT * Time.deltaTime, HitMasks.NO_PROJECTILES);

        Debug.DrawRay(transform.position, velocity.normalized * velocity.magnitude * VELOCITY_MULT * Time.deltaTime);

        // When a hit is detected, apply damage if destructible and apply splash damage if needed.
        if (rayHasHit)
        {
            GameObject rayHitGameObject = rayHit.transform.gameObject;

            // Target stays null if this didn't hit a ship. Should only happen if something is
            // on the ship layer that isn't a ship.
            Ship target = rayHitGameObject.GetComponent<Ship>();
            bool hitSelf = false;

            // Apply damage to what we hit, but ignore the firing ship.
            if (target)
            {
                // If we didn't hit ourselves, then apply damage then destroy the bullet.
                if (target != firedFrom)
                {
                    // DEBUG
                    //print("Hit " + target.shipName + " with " + target.shipHP + " HP left.");

                    target.ApplyDamage(damage);
                    //DestroyBullet(rayHit.point, true);
                }
                else
                {
                    hitSelf = true;
                }
            }

            // If we hit ourselves, just keep going and don't destroy the bullet.
            if (!hitSelf)
            {
                DestroyBullet(rayHit.point, false);
            }
        }
    }

    public void Fire(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        fireTime = Time.time;
        transform.position = position;
        transform.rotation = rotation;
        this.velocity = velocity;

        fired = true;
    }

    public void Fire(Vector3 position, Quaternion rotation, Vector3 velocity, Ship firedFrom)
    {
        this.firedFrom = firedFrom;
        Fire(position, rotation, velocity);
    }

    public void DestroyBullet(bool diedFromTimeout = true)
    {
        DestroyBullet(transform.position, diedFromTimeout);
    }

    public void DestroyBullet(Vector3 pointOfImpact, bool diedFromTimeout = true)
    {
        if (hitEffect != null && !diedFromTimeout)
            Instantiate(hitEffect, pointOfImpact, transform.rotation);

        Destroy(gameObject);
    }
}