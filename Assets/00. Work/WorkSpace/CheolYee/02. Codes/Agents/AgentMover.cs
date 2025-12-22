using System;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public class AgentMover : MonoBehaviour, IAgentComponent, IAgentMover, ISpeedModifiable
    {
        [Header("Player values")] 
        [SerializeField]private float moveSpeed = 5f;
        [SerializeField] private AnimParamSo xSpeedParam;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Vector2 groundCheckSize;
        
        private Rigidbody2D _rigidbody;
        private Agent _agent;
        private AgentRenderer _renderer;
        private float _movementX;
        private float _moveSpeedMultiplier;
        private float _originalGravityScale;
        public bool IsGrounded { get; private set; }
        public bool CanManualMovement { get; set; }

        public event Action<Vector2> OnVelocityChange;
        public void Initialize(Agent agent)
        {
            _agent = agent;
            _renderer = agent.GetCompo<AgentRenderer>();
            _rigidbody = agent.GetComponent<Rigidbody2D>();
            _originalGravityScale = _rigidbody.gravityScale;
            _moveSpeedMultiplier = 1f;
        }

        
        public void SetMoveSpeedMultiplier(float value) => _moveSpeedMultiplier = value;
        public void SetGravityScale(float value) => _rigidbody.gravityScale = _originalGravityScale * value;
        

        public void AddForceToAgent(Vector2 force)
        {
            _rigidbody.AddForce(force, ForceMode2D.Impulse);
        }

        public void StopImmediately(bool xAxis, bool yAxis)
        {
            if (xAxis)
            {
                _rigidbody.linearVelocityX = 0;
                _movementX = 0;
            }

            if (yAxis)
            {
                _rigidbody.linearVelocityY = 0;
            }
        }

        public void SetMovementX(float x) => _movementX = x;
        
        private void FixedUpdate()
        {
            CheckGround();
            MoveCharacter();
        }

        private void CheckGround()
        {
            IsGrounded = Physics2D.OverlapBox(transform.position, groundCheckSize, 0, whatIsGround);
        }

        private void MoveCharacter()
        {
            if (CanManualMovement == false)
            {
                _renderer.FlipController(_movementX);
                _rigidbody.linearVelocityX = _movementX * moveSpeed * _moveSpeedMultiplier;
            }
            
            if (xSpeedParam != null)
                _renderer.SetParam(xSpeedParam, Mathf.Abs(_rigidbody.linearVelocityX));
            
            OnVelocityChange?.Invoke(_rigidbody.linearVelocity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, groundCheckSize);
        }
    }
}