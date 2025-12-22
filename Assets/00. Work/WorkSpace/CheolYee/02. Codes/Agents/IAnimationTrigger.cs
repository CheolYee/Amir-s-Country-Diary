using System;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public interface IAnimationTrigger
    {
        event Action OnAnimationEnd;
    }
}