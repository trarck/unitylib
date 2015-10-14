using UnityEngine;
using System.Collections;

namespace YH
{
    public class ParticleEvent : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps)
            {
                StartCoroutine(OnComplete(ps.duration));
            }
        }

        IEnumerator OnComplete(float wait)
        {
            yield return new WaitForSeconds(wait);

            AnimationEventSystem animationEventSystem = GetComponent<AnimationEventSystem>();
            if (animationEventSystem != null)
            {
                animationEventSystem.TriggerCompleteEvent();
            }
        }
    }
}