using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace FuzzyLogicAndGeneticAlgorithm
{

    public class EnemyForFuzzyGenetic : MonoBehaviour
    {
        public MeshFilter groundPlane;

        [SerializeField] float speed = 3.2f;
        [SerializeField] float viewRadius = 3f;
        [SerializeField] float lookForTargetInterval = 3f;
        [SerializeField] float lookForTargetDuration = 0.5f;
        [SerializeField] int damage = 35;
        [SerializeField] float waitTimeAfterHit = 0.3f;

        [SerializeField] LayerMask targetPlayerLayer;

        GameObject currentTarget;

        bool hasTarget = false;
        bool lookingForTarget = false;
        bool waiting = false;
        float timer = 0f;
        float lookTimer = 0f;

        Vector3 patrolLocation;

        void Update()
        {
            if (waiting) return;

            if (timer < lookForTargetInterval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                hasTarget = false;
                lookingForTarget = true;
                if (lookTimer < lookForTargetDuration)
                {
                    lookTimer += Time.deltaTime;
                    LookForTarget();
                    if (hasTarget)
                    {
                        lookTimer = 0;
                        timer = 0f;
                        lookingForTarget = false;
                    }
                }
                else
                {
                    lookTimer = 0;
                    timer = 0f;
                    lookingForTarget = false;
                }
            }

            if (hasTarget)
            {
                MoveToTarget();
            }
            else
            {
                Wander();
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            PlayerAgent player = other.GetComponent<PlayerAgent>();

            player.TakeDamage(damage);
            if (player.Dead || player.gameObject == null)
                return;

            waiting = true;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player"))
                return;

            Invoke("SetWaitingFalse", waitTimeAfterHit);
        }

        void SetWaitingFalse()
        {
            print("done waiting");
            waiting = false;
        }


        void LookForTarget()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius, targetPlayerLayer);
            print(colliders.Length);
            if (colliders.Length == 0)
            {
                hasTarget = false;
                return;
            }
            hasTarget = true;
            colliders.OrderBy(collider => Vector3.Distance(transform.position, collider.transform.position)).ToArray();
            currentTarget = colliders[0].gameObject;
        }

        public void MoveToTarget()
        {
            if (currentTarget == null)
            {
                hasTarget = false;
                return;
            }

            Vector3 newPos = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
        }

        public void Wander()
        {
            if (Vector3.Distance(transform.position, patrolLocation) <= 0.1f)
            {
                //pick new random location
                GetRandomPatrolLocation();
            }

            transform.position = Vector3.MoveTowards(transform.position, patrolLocation, speed * Time.deltaTime);
        }

        private void GetRandomPatrolLocation()
        {
            Vector3 newlocation = groundPlane.transform.position;
            float minX = newlocation.x - groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float maxX = newlocation.x + groundPlane.mesh.bounds.extents.x * groundPlane.transform.localScale.x;
            float minZ = newlocation.z - groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;
            float maxZ = newlocation.z + groundPlane.mesh.bounds.extents.z * groundPlane.transform.localScale.z;

            patrolLocation = new Vector3(Random.Range(minX, maxX), transform.position.y, Random.Range(minZ, maxZ));
        }

        private void OnDrawGizmos()
        {

            if (hasTarget)
            {
                Gizmos.color = Color.green;
            }
            else if (lookingForTarget)
            {
                Gizmos.color = Color.blue;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
    }
}
