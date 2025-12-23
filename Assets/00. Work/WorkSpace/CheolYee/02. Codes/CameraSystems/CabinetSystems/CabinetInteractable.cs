using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;
using UnityEngine.Serialization;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CabinetSystems
{
    [DisallowMultipleComponent]
    public class CabinetInteractable : MonoBehaviour
    {
        [Header("Interaction Trigger")]
        [SerializeField] private Collider2D trigger;
        
        [Header("Anchors")]
        [SerializeField] private Transform enterPoint;
        [SerializeField] private Transform insidePoint;
        [SerializeField] private Transform exitPoint;
        
        [Header("Animator")]
        [SerializeField] private List<Animator> animators;
        [SerializeField] private AnimParamSo openTrigger;
        [SerializeField] private AnimParamSo closeTrigger;
        
        public bool IsOccupied => _occupant != null;
        public Transform EnterPoint => enterPoint != null ? enterPoint : transform;
        public Transform InsidePoint => insidePoint != null ? insidePoint : transform;
        public Transform ExitPoint => exitPoint != null ? exitPoint : transform;
        
        private PlayerCabinetController _occupant;
        
        private void Awake()
        {
            if (trigger == null) trigger = GetComponent<Collider2D>();
            if (trigger != null) trigger.isTrigger = true;
        }

        public bool TryReserve(PlayerCabinetController controller)
        {
            if (controller == null) return false;
            if (_occupant != null && _occupant != controller) return false;

            _occupant = controller;
            return true;
        }

        public void Release(PlayerCabinetController controller)
        {
            if (_occupant == controller) _occupant = null;
        }

        public void PlayOpen()
        {
            if (animators == null || openTrigger == null) return;

            foreach (var animator in animators)
            {
                if (closeTrigger != null) animator.ResetTrigger(closeTrigger.HashValue);
                animator.ResetTrigger(openTrigger.HashValue);
                animator.SetTrigger(openTrigger.HashValue);
            }
        }

        public void PlayClose()
        {
            if (animators == null || closeTrigger == null) return;

            foreach (var animator in animators)
            {
                animator.ResetTrigger(openTrigger.HashValue);
                animator.ResetTrigger(closeTrigger.HashValue);
                animator.SetTrigger(closeTrigger.HashValue);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var controller = other.GetComponentInChildren<PlayerCabinetController>();
            if (controller == null) return;
            controller.SetCandidate(this);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var controller = other.GetComponentInChildren<PlayerCabinetController>();
            if (controller == null) return;
            controller.ClearCandidate(this);
        }
        
        private void OnTriggerStay2D(Collider2D other)
        {
            var controller = other.GetComponentInChildren<PlayerCabinetController>();
            if (controller == null) return;

            controller.SetCandidate(this);
        }
    }
}