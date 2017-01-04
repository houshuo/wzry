namespace AGE
{
    using System;
    using UnityEngine;

    public class LifeTimeHelper : MonoBehaviour
    {
        public bool checkParticleLife;
        public float startTime;

        private void Update()
        {
            if (((this.checkParticleLife && base.GetComponent<ParticleSystem>().playOnAwake) && (base.GetComponent<ParticleSystem>() != null)) && (base.GetComponent<ParticleSystem>().isStopped || !base.GetComponent<ParticleSystem>().IsAlive()))
            {
                ActionManager.DestroyGameObject(base.gameObject);
            }
        }
    }
}

