using System.Collections.Generic;
using UnityEngine;

namespace Replay.Utils
{
    public class ParticleCleanup : MonoBehaviour
    {
        private readonly HashSet<ParticleSystem> _particleSystems = new();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void OnEnable()
        {
            FindParticleSystems();
        }

        private void OnDisable()
        {
            ClearParticleSystems();
        }

        void FindParticleSystems()
        {
            var particleSystems = GetComponentsInChildren<ParticleSystem>();
            if (particleSystems != null && !particleSystems.IsEmpty())
                _particleSystems.AddRange(particleSystems);
            
        }

        public void ClearParticleSystems()
        {
            foreach (var ps in _particleSystems)
            {
                if (ps != null && ps.IsAlive())
                    ps.Clear();
            }
        }
    }
}
