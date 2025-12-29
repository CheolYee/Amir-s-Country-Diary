using System;
using System.Collections.Generic;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems
{
    public enum ClipSelectMode
    {
        Sequential, // 순서대로
        Random      // 랜덤(선택)
    }
    
    [CreateAssetMenu(menuName = "SO/SHUTTER/Noise Preset", fileName = "New NoisePreset")]
    public class NoisePresetSo : ScriptableObject
    {
        [Header("Audio (Variants)")]
        public List<AudioClip> clips = new();
        public ClipSelectMode clipSelectMode = ClipSelectMode.Sequential;

        [Tooltip("랜덤일 때 직전과 같은 클립을 피함(가능하면).")]
        public bool avoidImmediateRepeat = true;
        
        public bool loop;

        [Tooltip("Loop / Player 타입일 때 펄스 이벤트 주기")]
        [Min(0.05f)] public float pulseInterval = 0.35f;

        [Header("Gameplay Hearing")]
        [Tooltip("기본 소리 크기 0 ~ 1 권장")]
        [Range(0f, 1f)] public float loudness = 0.3f;

        [Tooltip("최대 청취 거리(R). 이 밖에서는 0으로 처리.")]
        [Min(0f)] public float maxRange = 12f;

        [Tooltip("반감 거리(H). d=H일 때 절반으로 들림.")]
        [Min(0.01f)] public float halfDistance = 5f;

        [Header("Type")]
        public NoiseType type = NoiseType.Impulse;
        public NoiseTag tag = NoiseTag.None;

        [Header("Optional Multipliers")]
        [Min(0f)] public float uiGainMultiplier = 1f;
        [Min(0f)] public float cameraGainMultiplier = 1f;
        
        [Header("UI Noise Meter")]
        public bool affectUiNoiseMeter = true;
        
        
        
        public int ClipCount => clips?.Count ?? 0;
        public AudioClip GetClip(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
            return clips is { Count: > 0 } ? clips[index % clips.Count] : null;
        }
    }
}