using System.Collections.Generic;
using NeonBots.Managers;
using UnityEngine;

namespace NeonBots.Components
{
    public class Sleeper : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private List<MonoBehaviour> components;

        [SerializeField]
        private List<GameObject> gameObjects;

        private RigidbodyConstraints constraints;

        private void Start() => this.Init();

        private void Init()
        {
            var gameManager = MainManager.GetManager<GameManager>();
            if(!gameManager.IsReady) return;
            this.Sleep();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.TryGetComponent<Waker>(out _)) this.Wakeup();
        }

        private void OnTriggerExit(Collider collider)
        {
            if(collider.TryGetComponent<Waker>(out _)) this.Sleep();
        }

        public void Sleep()
        {
            this.constraints = this.rigidBody.constraints;
            this.rigidBody.constraints = RigidbodyConstraints.FreezeAll;

            foreach(var component in this.components)
                component.enabled = false;

            foreach(var go in this.gameObjects)
                go.SetActive(false);
        }

        public void Wakeup()
        {
            this.rigidBody.constraints = this.constraints;

            foreach(var component in this.components)
                component.enabled = true;

            foreach(var go in this.gameObjects)
                go.SetActive(true);
        }
    }
}
