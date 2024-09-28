using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace NeonBots.Components
{
    public class BotTurret : MonoBehaviour
    {
        [SerializeField]
        private float visionRadius = 30;

        private Unit unit;

        private GameObject target;

        private Vector3 direction;

        private float distance;

        private void Start()
        {
            this.unit = this.GetComponent<Unit>();
        }

        private void Update()
        {
            if(!this.target)
                this.Scan();
            else
                this.Tracking();

            if(this.target)
            {
                this.Calculate();
                this.Rotate();
                this.Attack();
            }
        }

        private void Scan()
        {
            var targetObjects = this.ScopeCheck();

            foreach(var targetObject in targetObjects)
            {
                var target = targetObject.GetComponent<Obj>();
                if(target && target.fraction != this.unit.fraction) this.target = targetObject;
            }
        }

        private void Tracking()
        {
            var targetObjects = this.ScopeCheck();
            var inRange = targetObjects.Find(item => item == this.target);
            if(!inRange || !this.target.GetComponent<Unit>()) this.target = null;
        }

        private void Calculate()
        {
            this.distance = Vector3.Distance(this.transform.position, this.target.transform.position);

            if(this.unit.primarySockets.Count <= 0) return;
            
            var primaryItem = this.unit.primarySockets[0].item;

            if(primaryItem == default && primaryItem.GetType() != typeof(Gun)) return;

            var primaryGun = (Gun)primaryItem;
            var bulletVelocity = primaryGun.projectilePrefab.GetComponent<Bullet>().force / 10;
            var timeDistance = this.distance / bulletVelocity;
            var targetPosition = this.target.transform.position;
            var targetVelocity = this.target.GetComponent<Rigidbody>().velocity;
            var aimPoint = targetPosition + targetVelocity * timeDistance;

            this.direction = aimPoint - this.transform.position;
        }

        private void Rotate()
        {
            this.unit.Rotate(this.direction);
        }

        private void Attack()
        {
            if(this.distance < this.visionRadius && Vector3.Angle(this.transform.forward, this.direction) < 5f)
                this.unit.Shot();
        }

        private List<GameObject> ScopeCheck()
        {
            // Getting of all colliders that overlapping sphere
            return Physics.OverlapSphere(this.transform.position, this.visionRadius, 1 << 3)
                // Filtering out colliders that are related to current game object
                .Where(hitCollider => hitCollider.gameObject.transform.parent != this.transform)
                // Getting colliders game objects
                .Select(hitCollider => hitCollider.gameObject.transform.parent.gameObject)
                // Converting the result to List
                .ToList();
        }

        private void OnDrawGizmos()
        {
            if(this.target)
            {
                var bulletVelocity = 40f;
                var timeDistance = this.distance / bulletVelocity;
                var targetPosition = this.target.transform.position;
                var targetVelocity = this.target.GetComponent<Rigidbody>().velocity;
                var aimPoint = targetPosition + targetVelocity * timeDistance;

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(this.transform.position, this.target.transform.position - this.transform.position);
                Gizmos.DrawWireSphere(targetPosition, 1.5f);

                Gizmos.color = Color.gray;
                Gizmos.DrawRay(targetPosition, targetVelocity);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(aimPoint, 1.5f);
                Gizmos.DrawRay(this.transform.position, aimPoint - this.transform.position);
            }
        }
    }
}
