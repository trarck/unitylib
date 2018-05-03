using UnityEngine;
using System.Collections;

namespace YH
{
    public class ParticleEvent : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            if (particleSystems != null)
            {
                float maxDuration = 0;
                for (int i = 0; i < particleSystems.Length; ++i)
                {
                    if (particleSystems[i].main.duration > maxDuration)
                    {
                        maxDuration = particleSystems[i].main.duration;
                    }
                }
                StartCoroutine(OnComplete(maxDuration));
            }
        }

        IEnumerator OnComplete(float wait)
        {
            yield return new WaitForSeconds(wait);

            AnimationEventSystem animationEventSystem = GetComponentInParent<AnimationEventSystem>();
            if (animationEventSystem != null)
            {
                animationEventSystem.TriggerCompleteEvent();
            }
        }
    }

    public class SimpleParticleEvent : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
            if (ps)
            {
                StartCoroutine(OnComplete(ps.main.duration));
            }
        }

        IEnumerator OnComplete(float wait)
        {
            yield return new WaitForSeconds(wait);

            AnimationEventSystem animationEventSystem = GetComponentInParent<AnimationEventSystem>();
            if (animationEventSystem != null)
            {
                animationEventSystem.TriggerCompleteEvent();
            }
        }
    }
}