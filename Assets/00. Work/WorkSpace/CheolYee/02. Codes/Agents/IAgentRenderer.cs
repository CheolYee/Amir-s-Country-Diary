using _00._Work.WorkSpace.CheolYee._02._Codes.AnimationSysytems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.Agents
{
    public interface IAgentRenderer
    {
        float FacingDirection { get; }
        Sprite CurrentSprite { get; }
        public void SetParam(AnimParamSo param, bool value);
        public void SetParam(AnimParamSo param, int value);
        public void SetParam(AnimParamSo param, float value);
        public void SetParam(AnimParamSo param);
    }
}