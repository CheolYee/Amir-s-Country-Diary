using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Players.FSMStates
{
    public class PlayerFootstepNoise : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Player player;                 // 비워두면 부모에서 자동 탐색
        [SerializeField] private NoiseEmitter walkEmitter;
        [SerializeField] private NoiseEmitter runEmitter;

        [Header("Filter")]
        [SerializeField] private float minMoveInput = 0.05f;    // 거의 멈췄을 때 이벤트 무시

        private void Reset()
        {
            player = GetComponentInParent<Player>();
        }

        public void Footstep()
        {
            if (player == null) player = GetComponentInParent<Player>();
            if (player == null || player.PlayerInput == null) return;

            float x = player.PlayerInput.MoveInput.x;
            if (Mathf.Abs(x) < minMoveInput) return;

            bool isRunning = player.CurrentState is PlayerRunState;
            var emitter = isRunning ? runEmitter : walkEmitter;
            if (emitter == null) return;

            emitter.EmitOnce();
        }

        [ContextMenu("TEST/Footstep")]
        private void TEST_Footstep() => Footstep();
    }
}