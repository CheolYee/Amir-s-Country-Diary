using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems
{
    public struct NoiseEmittedEvent : IEvent
    {
        public readonly int SourceId; //같은 소리원 식별
        public readonly Vector2 Position; //소리 발생 위치
        public readonly NoisePresetSo Preset; // 프리셋
        public readonly float EmitTime; // 발생 시간

        public NoiseEmittedEvent(int sourceId, Vector2 position, NoisePresetSo preset, float emitTime)
        {
            SourceId = sourceId;
            Position = position;
            Preset = preset;
            EmitTime = emitTime;
        }
    }
}