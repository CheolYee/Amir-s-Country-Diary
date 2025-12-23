using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CabinetSystems
{
    public class DoorSortLayerChanger : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private NoiseEmitter openEmitter;
        [SerializeField] private NoiseEmitter closeEmitter;
        
        private int _sortingOrder;

        private void Awake()
        {
            _sortingOrder = spriteRenderer.sortingOrder;
        }

        public void StartOpenSound() => openEmitter.EmitOnce();
        public void StartCloseSound() => closeEmitter.EmitOnce();

        public void ChangeEnter()
        {
            spriteRenderer.sortingOrder = 1;
        }

        public void ChangeExit()
        {
            spriteRenderer.sortingOrder = _sortingOrder;
        }
    }
}