namespace Assets.Scripts.GameLogic
{
    using System;
    using UnityEngine;

    public class Waypoint : MonoBehaviour
    {
        [HideInInspector]
        public int AccessIndex = -1;
        public Color color = new Color(1f, 0f, 0f);
        public float radius = 0.25f;

        public void OnDrawGizmos()
        {
            Gizmos.color = this.color;
            Gizmos.DrawSphere(base.transform.position, this.radius);
        }
    }
}

