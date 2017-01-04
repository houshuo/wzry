namespace AGE
{
    using System;
    using UnityEngine;

    public class RootMotionHelper : MonoBehaviour
    {
        private Vector3 posOffset = new Vector3();
        public Transform rootTransform;

        public void ForceLateUpdate()
        {
            this.rootTransform.localPosition = this.posOffset;
        }

        public void ForceStart()
        {
            this.posOffset = this.rootTransform.localPosition;
        }

        private void LateUpdate()
        {
            this.rootTransform.localPosition = this.posOffset;
        }

        private void Start()
        {
            this.posOffset = this.rootTransform.localPosition;
        }
    }
}

