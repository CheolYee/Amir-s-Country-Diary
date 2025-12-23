using System;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public class AgentRenderer : MonoBehaviour, IAgentComponent, IAgentRenderer, IAnimationTrigger
    {
        private Agent _agent;
        private Animator _animator; 
        private SpriteRenderer _spriteRenderer;

        public event Action OnAnimationEnd;
        [field: SerializeField] public float FacingDirection { get; private set; } = 1f;//1f가 오른쪽 보는거다.
        public Sprite CurrentSprite => _spriteRenderer.sprite;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        public void Initialize(Agent agent)
        {
            _agent = agent;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetParam(AnimParamSo param, bool value) => _animator.SetBool(param.HashValue, value);
        public void SetParam(AnimParamSo param, int value) => _animator.SetInteger(param.HashValue, value);
        public void SetParam(AnimParamSo param, float value) => _animator.SetFloat(param.HashValue, value);
        public void SetParam(AnimParamSo param) => _animator.SetTrigger(param.HashValue);
        
        public void FlipController(float xMove)
        {
            if (Mathf.Abs(FacingDirection + xMove) < 0.5f)
                Flip();
        }

        private void Flip()
        {
            FacingDirection *= -1;
            float targetYRotation = FacingDirection > 0 ? 0 : 180f;
            _agent.transform.rotation = Quaternion.Euler(0, targetYRotation, 0);
        }
        
        private void AnimationEndTrigger() => OnAnimationEnd?.Invoke();
    
    }
}