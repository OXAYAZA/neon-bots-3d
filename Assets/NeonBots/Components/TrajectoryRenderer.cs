using UnityEngine;

namespace NeonBots.Components
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryRenderer : MonoBehaviour
    {
        [SerializeField]
        private int pointsNumber = 50;

        [SerializeField]
        private float duration = 3f;

        private LineRenderer lineRenderer;

        private void Awake()
        {
            this.lineRenderer = this.GetComponent<LineRenderer>();
        }

        public void SetTrajectory(Vector3 origin, Vector3 velocity)
        {
            var points = new Vector3[this.pointsNumber];
            var timeCut = this.duration / this.pointsNumber;

            for(var i = 0; i < points.Length; i++)
            {
                var time = timeCut * i;
                points[i] = origin + velocity * time + Physics.gravity * (time * time) / 2f;
            }

            this.lineRenderer.positionCount = points.Length;
            this.lineRenderer.SetPositions(points);
        }
    }
}
