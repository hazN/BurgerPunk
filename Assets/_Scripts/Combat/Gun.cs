using UnityEngine;

namespace BurgerPunk.Combat
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private float range;
        [SerializeField] private float fireRate;

        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        private float nextFireTime;

        public void Fire()
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        private void Shoot()
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, range))
            {
                Health targetHealth = hit.transform.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage);
                }
            }

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.AddForce(firePoint.forward * 20f, ForceMode.Impulse);
            rb.gameObject.transform.rotation = Quaternion.LookRotation(rb.linearVelocity);

            Destroy(bullet, 0.3f);
        }
    }
}