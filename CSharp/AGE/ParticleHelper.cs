namespace AGE
{
    using System;
    using UnityEngine;

    public class ParticleHelper
    {
        private static int _particleActiveNumber;

        public static void DecParticleActiveNumber()
        {
            _particleActiveNumber--;
            if (_particleActiveNumber < 0)
            {
            }
        }

        public static int GetParticleActiveNumber()
        {
            return _particleActiveNumber;
        }

        public static void IncParticleActiveNumber()
        {
            _particleActiveNumber++;
        }

        public static ParticleSystem[] Init(GameObject gameObj, Vector3 scaling)
        {
            ParticleSystem[] componentsInChildren = gameObj.GetComponentsInChildren<ParticleSystem>();
            if ((componentsInChildren == null) || (componentsInChildren.Length == 0))
            {
                return null;
            }
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                ParticleSystem system = componentsInChildren[i];
                system.startSize *= scaling.x;
                system.startLifetime *= scaling.y;
                system.startSpeed *= scaling.z;
                Transform transform = system.transform;
                transform.localScale = (Vector3) (transform.localScale * scaling.x);
                if (!system.playOnAwake)
                {
                    system.playOnAwake = true;
                    system.Play();
                }
            }
            return componentsInChildren;
        }
    }
}

