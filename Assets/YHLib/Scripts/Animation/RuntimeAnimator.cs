using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YH.Animation
{
    public class RuntimeAnimator
    {
        enum UpdateState
        {
            None,
            Start,
            Update
        }

        UpdateState m_State = UpdateState.None;

        GameObject m_Target;
        string m_OriginalAnimationName = null;
        string m_StateName = null;
        //Key:state name,Value:State Hash
        Dictionary<string, int> m_OverrideMap=null;

        Animator m_Animator;

        AnimatorOverrideController m_AnimatorOverrideController;
        AnimationClip m_CurrentAnimationClip = null;
        int m_CurrentStateHash;
        float m_Elapsed = 0;
        float m_StateDuration = 0;
        private bool m_NeedCompleteEvent = true;

        public delegate void LoadAnimationClipHandle(string path,System.Action<AnimationClip> callback);

        public LoadAnimationClipHandle loadAnimationClipHandle=null;

        public event System.Action onComplete;

        public RuntimeAnimator(GameObject target)
        {
            m_Target = target;
        }

        public virtual void Init()
        {
            m_OverrideMap = new Dictionary<string, int>();

            m_Animator = m_Target.GetComponent<Animator>();

            m_AnimatorOverrideController = new AnimatorOverrideController(m_Animator.runtimeAnimatorController);
            m_Animator.runtimeAnimatorController = m_AnimatorOverrideController;
        }

        public void Update(float delta)
        {
            if (m_NeedCompleteEvent)
            {
                switch (m_State)
                {
                    case UpdateState.Start:
                        //检查当前播放的State是否为设置的State
                        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                        if (stateInfo.shortNameHash == m_CurrentStateHash|| stateInfo.fullPathHash == m_CurrentStateHash)
                        {
                            m_StateDuration = stateInfo.length / (stateInfo.speed * stateInfo.speedMultiplier);
                            m_Elapsed = 0;
                            m_State = UpdateState.Update;
                        }
                        break;
                    case UpdateState.Update:
                        {
                            m_Elapsed += delta;
                            if (m_Elapsed >= m_StateDuration)
                            {
                                if (onComplete != null)
                                {
                                    onComplete.Invoke();
                                }
                                m_State = UpdateState.None;
                            }
                        }
                        break;
                }
            }
        }

        public void AddOverride(string originalAnimation,string stateName)
        {
            m_OverrideMap[originalAnimation] = Animator.StringToHash(stateName);
        }

        public void SetTrigger(string name)
        {
            if (m_Animator)
            {
                m_Animator.SetTrigger(name);
            }
        }

        public void SetTrigger(int id)
        {
            if (m_Animator)
            {
                m_Animator.SetTrigger(id);
            }
        }

        public void SetBool(string name, bool value)
        {
            if (m_Animator)
            {
                m_Animator.SetBool(name,value);
            }
        }

        public void SetBool(int id, bool value)
        {
            if (m_Animator)
            {
                m_Animator.SetBool(id, value);
            }
        }
        public void SetFloat(int id, float value)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(id, value);
            }
        }

        public void SetFloat(string name, float value, float dampTime, float deltaTime)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(name, value,dampTime,deltaTime);
            }
        }

        public void SetFloat(string name, float value)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(name, value);
            }
        }

        public void SetFloat(int id, float value, float dampTime, float deltaTime)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(id, value, dampTime, deltaTime);
            }
        }

        public void SetInteger(int id, int value)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(id, value);
            }
        }

        public void SetInteger(string name, int value)
        {
            if (m_Animator)
            {
                m_Animator.SetFloat(name, value);
            }
        }

        public void Play(string stateName)
        {
            int stateNameHash = Animator.StringToHash(stateName);
            Play(stateNameHash);
        }

        public void Play(int stateNameHash)
        {
            if (m_Animator)
            {
                m_Animator.Play(stateNameHash,-1,0);
                m_State = UpdateState.Start;
                m_CurrentStateHash = stateNameHash;
            }
        }

        public void PlayOverride(string originalAnimation,string animationClip)
        {
            int state;
            if (m_OverrideMap.TryGetValue(originalAnimation, out state))
            {
                if (loadAnimationClipHandle != null)
                {
                    loadAnimationClipHandle(animationClip, (clip) =>
                    {
                        if (clip)
                        {
                            if (m_CurrentAnimationClip != clip)
                            {
                                m_CurrentAnimationClip = clip;
                                m_AnimatorOverrideController[originalAnimation] = clip;
                                Play(state);
                            }
                            else
                            {
                                Play(state);
                            }
                        }
                    });
                }
            }
        }

        public void PlayOverride(string originalAnimation, AnimationClip animationClip)
        {
            int state;
            if (m_OverrideMap.TryGetValue(originalAnimation, out state))
            {
                if (animationClip)
                {
                    if (m_CurrentAnimationClip != animationClip)
                    {
                        m_CurrentAnimationClip = animationClip;
                        m_AnimatorOverrideController[originalAnimation] = animationClip;
                        Play(state);
                    }
                    else
                    {
                        Play(state);
                    }
                }
            }
        }

        public void Dispose()
        {
            
        }

        public bool needCompleteEvent
        {
            get
            {
                return m_NeedCompleteEvent;
            }
            set
            {
                m_NeedCompleteEvent = value;
            }
        }
    }
}
