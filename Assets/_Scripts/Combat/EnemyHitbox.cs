using System.Collections;
using UnityEngine;

namespace Assets._Scripts.Combat
{
    public class EnemyHitbox : MonoBehaviour
    {
        [SerializeField] private Actor actor;

        public Actor GetActor()
        {
            return actor;
        }
    }
}