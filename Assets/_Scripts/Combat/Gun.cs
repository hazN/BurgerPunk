using Assets._Scripts.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BurgerPunk.Combat
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private float damage = 5;
        [SerializeField] private float range = 20;
        [SerializeField] private float fireRate = 1;
        [SerializeField] private float accuracy = 1;

        private float nextFireTime;

        [SerializeField] private Transform rightHandIKTarget;
        [SerializeField] private Transform leftHandIKTarget;

        [SerializeField] private Transform rightHandWeaponPosition;
        [SerializeField] private Transform leftHandWeaponPosition;

        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private Transform muzzleFlashTransform;

        [SerializeField] private GameObject trailPrefab;

        [SerializeField] private bool isFireable = true;
        private void Update()
        {
            if (rightHandIKTarget == null || leftHandIKTarget == null)
            {
                Debug.LogError("IK targets not set");
                return;
            }
            rightHandIKTarget.SetPositionAndRotation(rightHandWeaponPosition.position, rightHandWeaponPosition.rotation);
            leftHandIKTarget.SetPositionAndRotation(leftHandWeaponPosition.position, leftHandWeaponPosition.rotation);
        }
        public void Fire()
        {
            if (!isFireable) return;
            if (Time.time >= nextFireTime)
            {
                if (muzzleFlash != null)
                {
                    muzzleFlash.gameObject.transform.position = muzzleFlashTransform.transform.position;
                    muzzleFlash.gameObject.transform.rotation = muzzleFlashTransform.transform.rotation;
                    Debug.Log("Muzzle flash played");
                    muzzleFlash.Stop();
                    muzzleFlash.Play();
                }
                animator.Play("Fire", 0, 0f);
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
            Vector3 direction = Camera.main.transform.forward;

            float maxSpreadAngle = Mathf.Lerp(0f, 10f, 1f - accuracy);

            Quaternion spread = Quaternion.Euler(
                Random.Range(-maxSpreadAngle, maxSpreadAngle),
                Random.Range(-maxSpreadAngle, maxSpreadAngle),
                0
            );

            direction = spread * direction;
            direction.Normalize();


            Vector3 start = muzzleFlashTransform.position;
            Vector3 hitPoint = Camera.main.transform.position + direction * range;

            int enemyLayer = LayerMask.NameToLayer("Enemy");
            int layerMask = 1 << enemyLayer; 

            if (Physics.Raycast(Camera.main.transform.position, direction, out RaycastHit hit, range, layerMask))
            {
                hitPoint = hit.point;

                EnemyHitbox enemyHitbox = hit.collider.GetComponent<EnemyHitbox>();
                if (enemyHitbox != null)
                {
                    Actor actor = enemyHitbox.GetActor();
                    if (actor != null)
                    {
                        actor.TakeDamage(damage);
                        Debug.Log("Hit " + actor.name + " for " + damage + " damage.");
                    }
                    else
                    {
                        Debug.Log("Hit " + hit.collider.name);
                    }
                }
                else
                {
                    Debug.Log("Hit " + hit.collider.name);
                }

                //Actor actor = hit.collider.GetComponent<Actor>();
                //if (actor != null && actor.tag != "Enemy")
                //{
                //    actor.TakeDamage(damage);
                //    Debug.Log("Hit " + actor.name + " for " + damage + " damage.");
                //}
                //else
                //{
                //    Debug.Log("Hit " + hit.collider.name);
                //}
            }
            else
            {
                Debug.Log("Missed.");
            }

            // Always spawn the trail
            GameObject trail = Instantiate(trailPrefab, start, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail.GetComponent<TrailRenderer>(), start, hitPoint));
        }

        private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 start, Vector3 end)
        {
            float distance = Vector3.Distance(start, end);
            float speed = 100f; // the higher, the faster

            float time = 0f;
            while (time < 1f)
            {
                time += (Time.deltaTime * speed) / distance;
                trail.transform.position = Vector3.Lerp(start, end, time);
                yield return null;
            }

            trail.transform.position = end;
            Destroy(trail.gameObject, trail.time);
        }

        public void AddDamage(float multiplier)
        {
            damage *= multiplier;
            Debug.Log("Damage increased to: " + damage);
        }
        public void AddFireRate(float multiplier)
        {
            fireRate *= multiplier;
            Debug.Log("Fire rate increased to: " + fireRate);
        }
        public void AddAccuracy(float multiplier)
        {
            accuracy *= multiplier;
            Debug.Log("Accuracy increased to: " + accuracy);
        }
        public bool IsFireable()
        {
            return isFireable;
        }
    }
}