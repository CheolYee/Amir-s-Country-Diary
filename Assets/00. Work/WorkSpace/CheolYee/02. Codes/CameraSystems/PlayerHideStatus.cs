using System;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public class PlayerHideStatus : MonoBehaviour, IAgentComponent, IAfterInitialize
    {
        public bool IsHidden { get; private set; }
        public event Action<bool> OnHiddenChanged;

        private Agent _agent;
        
        public void Initialize(Agent agent)
        {
            _agent = agent;
        }

        public void Afterinitialize()
        {
            SetHidden(false, force: true);
        }

        public void SetHidden(bool hidden) => SetHidden(hidden, force: false);

        public void Toggle() => SetHidden(!IsHidden);

        private void SetHidden(bool hidden, bool force)
        {
            if (!force && IsHidden == hidden) return;

            IsHidden = hidden;
            OnHiddenChanged?.Invoke(IsHidden);
        }
    }
}