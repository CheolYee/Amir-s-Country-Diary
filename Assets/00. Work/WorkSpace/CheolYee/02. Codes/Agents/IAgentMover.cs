using System;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public interface IAgentMover
    {
        bool IsGrounded { get; }
        bool CanManualMovement { get; set; }
        
        event Action<Vector2> OnVelocityChange;
        void AddForceToAgent(Vector2 force);
        void StopImmediately(bool xAxis, bool yAxis);
        void SetMovementX(float x);
    }
}