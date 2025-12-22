using System.Collections.Generic;
using _00._Work.Resources._02._Codes.Utils;
using PBG_01_PUSE;
using UnityEngine;

namespace _00._Work.WorkSpace.Soso7194._01.Scripts.Interaction.Item
{
    public class GeneratorManager : MonoSingleton<GeneratorManager>
    {
        [Header("Door Settings")]
        [SerializeField] private GameObject mainDoor; // 열릴 문 오브젝트

        private List<Generator> allGenerators = new List<Generator>();
        private int activatedCount = 0;

        protected override void Awake()
        {
            base.Awake();
            // 씬에 있는 모든 발전기 자동 등록
            allGenerators.AddRange(FindObjectsByType<Generator>(FindObjectsSortMode.None));
        }

        public void ReportGeneratorActivated(Generator generator)
        {
            activatedCount++;
            Debug.Log($"발전기 가동 현황: {activatedCount} / {allGenerators.Count}");

            if (activatedCount >= allGenerators.Count)
            {
                OpenMainDoor();
            }
        }

        private void OpenMainDoor()
        {
            Debug.Log("모든 발전기가 가동되었습니다! 문이 열립니다.");
            if (mainDoor != null) mainDoor.SetActive(false); // 문 열기 (비활성화)
        }
    }
}