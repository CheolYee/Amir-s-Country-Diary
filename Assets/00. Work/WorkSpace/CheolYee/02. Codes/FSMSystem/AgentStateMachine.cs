using System;
using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.Agents;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.FSMSystem
{
    public class AgentStateMachine
    {
        public AgentState CurrentState { get; private set; }
        
        private Dictionary<int, AgentState> _stateDict;

        public AgentStateMachine(Agent agent, StateSo[] stateList)
        {
            _stateDict = new Dictionary<int, AgentState>();

            foreach (StateSo stateData in stateList)
            {
                Type type = Type.GetType(stateData.className);
                Debug.Assert(type != null , $"Finding type is null {stateData.className}");//안전코드
                AgentState agentState = Activator.CreateInstance(type, agent, stateData.paramSo) as AgentState;
                
                _stateDict.Add(stateData.stateIndex, agentState);
            }
        }

        public void ChangeState(int newStateIndex)
        {
            CurrentState?.Exit();
            AgentState newAgentState = _stateDict.GetValueOrDefault(newStateIndex);
            Debug.Assert(newAgentState != null, $"State us null : {newStateIndex}");
            
            CurrentState = newAgentState;
            CurrentState.Enter();
        }

        public void UpdateMachine()
        {
            CurrentState?.Update();
        }
    }
}