using System;
using System.Collections;
using _00._Work.WorkSpace.CheolYee._02._Codes.CameraSystems;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;


namespace PBG_01_LockPick
{
    public class LockPick : MonoBehaviour
    {
        [Header("오브젝트")]
        [SerializeField] private Transform pick;      // 락픽 오브젝트
        [SerializeField] private Transform lockCore;  // 실린더(락 코어)

        [Header("계산")]
        [SerializeField] private float tolerance = 15f;    // 허용 오차
        [SerializeField] private float unlockAngle = 180f;  // 완전히 열리는 각도
        [SerializeField] private float rotateSpeed = 5f;   // 실린더 반응 속도

        [Header("기회")]
        [SerializeField] private float pickDurability = 100f;
        [SerializeField] private float damagePerSecond = 20f;

        [SerializeField] private float targetAngle;     // 정답 각도
        private float pickAngle;       // 현재 락픽 각도
        private float currentRotate;   // 현재 실린더 회전값
        private bool isUnlocked;
        private NoiseEmitter noiseEmitter;
        [SerializeField] private NoiseEmitter _noiseEmitter;

        [SerializeField] private Camera cam;

        public Action OnUnlocked;

        public GameObject parent;
        Quaternion originRot;

        void Awake()
        {
            noiseEmitter = GetComponentInChildren<NoiseEmitter>();
        }


        void Update()
        {
            if (isUnlocked)
                return;

            RotatePick();

            if (Input.GetKey(KeyCode.D))
            {
                TryRotateLock();
            }
            else
            {
                // 키를 안 누르면 실린더 원위치
                currentRotate = Mathf.Lerp(currentRotate, 0f, Time.deltaTime * rotateSpeed);
                lockCore.localRotation = Quaternion.Euler(0, 0, -currentRotate);
            }
        }

        private void OnEnable()
        {
            parent.SetActive(true);
            pickDurability = 100f;
            targetAngle = Random.Range(180f, 0f);
            originRot = cam.transform.localRotation;
        }

        private void OnDisable()
        {
            isUnlocked = false;
        }

        // 락픽 회전
        void RotatePick()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = mousePos - (Vector2)pick.position;

            pickAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            pick.rotation = Quaternion.Euler(0, 0, pickAngle);
        }

        // 실린더 회전 시도
        void TryRotateLock()
        {
            float angleDiff = Mathf.Abs(pickAngle - targetAngle);

            float rotateRatio = Mathf.Clamp01(1 - angleDiff / tolerance);

            float maxRotate = rotateRatio * unlockAngle;

            currentRotate = Mathf.Lerp(currentRotate, maxRotate, Time.deltaTime * rotateSpeed);
            lockCore.localRotation = Quaternion.Euler(0, 0, -currentRotate);

            if (rotateRatio > 0.95f)
            {
                Unlock();
                return;
            }

            if (rotateRatio < 0.2f)
            {
                DamagePick();
            }
        }

        void DamagePick()
        {
            pickDurability -= damagePerSecond * Time.deltaTime;
            pickDurability = Mathf.Clamp(pickDurability, 0f, 100f);

            if (pickDurability <= 0f)
            {
                enabled = false; // 미니게임 종료
                _noiseEmitter.Begin();
                parent.SetActive(false);
            }
            else if (pickDurability <= 50 /*&& pickDurability >= 49 || pickDurability < 10 && pickDurability > 9*/)
            {
                cam.transform
                .DOShakeRotation(1f, 3f, 10)
                .OnComplete(() =>
                {
                    cam.transform.DOLocalRotateQuaternion(originRot, 0.1f);
                });
            }
        }

        void Unlock()
        {
            if (isUnlocked) return;
            noiseEmitter.Begin();
            isUnlocked = true;
            StartCoroutine(RotateToUnlock());
            OnUnlocked?.Invoke();
        }

        public void ShowLockPick()
        {
            this.gameObject.SetActive(true);
            enabled = true;
        }

        private IEnumerator RotateToUnlock()
        {
            float duration = 1f; // 몇 초 동안 회전할지
            float elapsed = 0f;

            float startRotate = currentRotate;
            float endRotate = unlockAngle;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                t = Mathf.SmoothStep(0f, 1f, t); // 부드럽게

                currentRotate = Mathf.Lerp(startRotate, endRotate, t);
                lockCore.localRotation = Quaternion.Euler(0, 0, -currentRotate);

                yield return null;
            }

            // 정확히 목표값으로
            currentRotate = endRotate;
            lockCore.localRotation = Quaternion.Euler(0, 0, -currentRotate);

            parent.SetActive(false);
        }
    }
}
