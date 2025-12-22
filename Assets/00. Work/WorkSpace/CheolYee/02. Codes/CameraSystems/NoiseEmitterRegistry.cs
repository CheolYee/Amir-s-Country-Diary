using System.Collections.Generic;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public static class NoiseEmitterRegistry
    {
        private static readonly Dictionary<int, NoiseEmitter> Emitters = new();

        public static void Register(NoiseEmitter emitter)
        {
            if (emitter == null) return;
            Emitters[emitter.SourceId] = emitter;
        }

        public static void Unregister(NoiseEmitter emitter)
        {
            if (emitter == null) return;
            if (Emitters.TryGetValue(emitter.SourceId, out var cur) && cur == emitter)
                Emitters.Remove(emitter.SourceId);
        }

        public static bool TryGet(int sourceId, out NoiseEmitter emitter)
            => Emitters.TryGetValue(sourceId, out emitter);

        public static IEnumerable<NoiseEmitter> All => Emitters.Values;
    }
}