using System.Collections.Generic;
using _00._Work.WorkSpace.CheolYee._02._Codes.EventSysyems;
using UnityEngine;

namespace _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems.CameraFlashs
{
    public class ShutterCameraStageManager : MonoBehaviour
    {
        [Header("Stage Rule")]
        [Tooltip("true: floorNumber(1~3)를 stage로 사용 / false: 발전기 켤 때마다 stage++")]
        [SerializeField] private bool useFloorAsStage = true;

        [Range(0, 3)]
        [SerializeField] private int startStage = 0;

        [Header("Camera Groups (Enable on Stage)")]
        [SerializeField] private List<GameObject> stage1Cameras = new();
        [SerializeField] private List<GameObject> stage2Cameras = new();
        [SerializeField] private List<GameObject> stage3Cameras = new();

        private int _stage;
        private readonly HashSet<int> _activatedFloors = new();

        private void Awake()
        {
            _stage = Mathf.Clamp(startStage, 0, 3);
            ApplyStage(_stage);
        }

        private void OnEnable()
        {
            Bus<GeneratorActivatedEvent>.OnEvent += OnGeneratorActivated;
        }

        private void OnDisable()
        {
            Bus<GeneratorActivatedEvent>.OnEvent -= OnGeneratorActivated;
        }

        private void OnGeneratorActivated(GeneratorActivatedEvent evt)
        {
            // 발전기 코드상 isActivated로 1회만 오겠지만, 안전장치로 중복 방지
            if (!_activatedFloors.Add(evt.Floor))
                return;

            int newStage;
            if (useFloorAsStage)
            {
                newStage = Mathf.Clamp(evt.Floor, 0, 3); // 1층=>1, 2층=>2, 3층=>3
                _stage = Mathf.Max(_stage, newStage);
            }
            else
            {
                _stage = Mathf.Clamp(_stage + 1, 0, 3); // 켤 때마다 +1
            }

            ApplyStage(_stage);
        }

        private void ApplyStage(int stage)
        {
            // stage 이하 그룹은 켜고, stage 초과 그룹은 끔(원치 않으면 아래 Disable 부분 삭제)
            SetGroupActive(stage1Cameras, stage >= 1);
            SetGroupActive(stage2Cameras, stage >= 2);
            SetGroupActive(stage3Cameras, stage >= 3);
        }

        private static void SetGroupActive(List<GameObject> group, bool active)
        {
            if (group == null) return;

            for (int i = 0; i < group.Count; i++)
            {
                var go = group[i];
                if (go == null) continue;
                if (go.activeSelf == active) continue;
                go.SetActive(active);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("DEBUG/Stage 0")]
        private void DEBUG_Stage0() { _stage = 0; ApplyStage(_stage); }

        [ContextMenu("DEBUG/Stage 1")]
        private void DEBUG_Stage1() { _stage = 1; ApplyStage(_stage); }

        [ContextMenu("DEBUG/Stage 2")]
        private void DEBUG_Stage2() { _stage = 2; ApplyStage(_stage); }

        [ContextMenu("DEBUG/Stage 3")]
        private void DEBUG_Stage3() { _stage = 3; ApplyStage(_stage); }
#endif
    }
}
