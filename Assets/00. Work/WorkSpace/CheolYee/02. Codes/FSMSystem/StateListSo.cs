using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem
{
    [CreateAssetMenu(fileName = "Fsm State Manager", menuName = "SO/FSM/ListManager", order = 5)]
    public class StateListSo : ScriptableObject
    {
        public string enumName;
        public StateSo[] states;
    }
}