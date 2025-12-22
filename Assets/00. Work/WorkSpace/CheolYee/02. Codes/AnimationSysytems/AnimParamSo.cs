using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems
{
    [CreateAssetMenu(fileName = "Animator Param", menuName = "SO/Animator/Param", order = 0)]
    public class AnimParamSo : ScriptableObject
    {
        [field: SerializeField] public string ParamName { get; private set; }
        [field: SerializeField] public int HashValue { get; private set; }

        private void OnValidate()
        {
            HashValue = Animator.StringToHash(ParamName);
        }
    }
}