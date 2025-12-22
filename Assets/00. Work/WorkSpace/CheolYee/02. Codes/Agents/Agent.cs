using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public class Agent : MonoBehaviour
    {
        private Dictionary<Type, IAgentComponent> _componentDict;
        
        public AgentHealth Health { get; private set; }
        public AgentMover AgentMover { get; protected set; }
        public AgentRenderer AgentRenderer { get; protected set; }
        protected CapsuleCollider2D CapsuleCollider { get; private set; }
    
        public UnityEvent onAgentDead;

        protected virtual void Awake()
        {
            _componentDict = GetComponentsInChildren<IAgentComponent>(true).ToDictionary(compo => compo.GetType());

            InitComponents();
            AfterInitializeComponent();
        }

        protected virtual void InitComponents()
        {
            foreach (IAgentComponent compo in _componentDict.Values)
            {
                compo.Initialize(this);
            }
            Health = GetCompo<AgentHealth>();
            AgentMover = GetCompo<AgentMover>();
            AgentRenderer = GetCompo<AgentRenderer>();
            CapsuleCollider = GetComponent<CapsuleCollider2D>();
        }
        protected virtual void AfterInitializeComponent()
        {
            _componentDict.Values.OfType<IAfterInitialize>()
                .ToList().ForEach(compo => compo.Afterinitialize());

            Health.OnDead += HandleAgentDead;
        }

        protected virtual void OnDestroy()
        {
            Health.OnDead -= HandleAgentDead;
        }

        protected virtual void HandleAgentDead()
        {
            onAgentDead?.Invoke();
        }

        public T GetCompo<T>()
        {
            if (_componentDict.TryGetValue(typeof(T), out IAgentComponent component) && component is T compo)
            {
                return compo;
            }

            IAgentComponent findComponent = _componentDict.Values.FirstOrDefault(c => c is T);
            if (findComponent is T findCompo)
                return findCompo;

            return default(T);
        }
    }
}
