using UnityEngine;

namespace BurgerPunk.Combat
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private float range;
        [SerializeField] private float fireRate;

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
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
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
        }
    }
}