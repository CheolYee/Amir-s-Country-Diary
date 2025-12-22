using System;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public class AgentHealth : MonoBehaviour, IAgentComponent, IDamageable
    {
        [field: SerializeField] public bool IsDead { get; private set; }
            
        private Agent _owner;
        public event Action OnDead;

        public void Initialize(Agent agent)
        {
            _owner = agent;
            IsDead = false;
        }

        public void ApplyDamage()
        {
            if (IsDead) return;
            
            IsDead = true;
            OnDead?.Invoke();
        }
    }
}