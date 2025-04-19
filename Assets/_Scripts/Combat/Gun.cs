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
        private void Awake()
        {
            muzzleFlash.gameObject.transform.position = muzzleFlashTransform.transform.position;
            muzzleFlash.gameObject.transform.rotation = muzzleFlashTransform.transform.rotation;
        }
        private void Update()
        {
            rightHandIKTarget.SetPositionAndRotation(rightHandWeaponPosition.position, rightHandWeaponPosition.rotation);
            leftHandIKTarget.SetPositionAndRotation(leftHandWeaponPosition.position, leftHandWeaponPosition.rotation);
        }
        public void Fire()
        {
            if (Time.time >= nextFireTime)
            {
                if (muzzleFlash != null)
                {
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
            float inaccuracy = 1f - accuracy;
            Vector3 direction = Camera.main.transform.forward;

            direction += new Vector3(
                Random.Range(-inaccuracy, inaccuracy),
                Random.Range(-inaccuracy, inaccuracy),
                Random.Range(-inaccuracy, inaccuracy)
            );

            direction.Normalize();

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, range))
            {
                Actor actor = hit.collider.GetComponent<Actor>();
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
                Debug.Log("Missed.");
            }
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
    }
}