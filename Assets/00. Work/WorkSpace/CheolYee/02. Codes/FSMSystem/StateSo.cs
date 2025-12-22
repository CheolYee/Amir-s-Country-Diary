using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem
{
    [CreateAssetMenu(fileName = "New State", menuName = "SO/FSM/State", order = 0)]
    public class StateSo : ScriptableObject
    {
        public string stateName;
        public string className;
        public int stateIndex;
        public AnimParamSo paramSo;
    }
}