using UnityEngine;
using UnityEngine.UI;

namespace PBG_01_Breath
{
    public class BreathBar : MonoBehaviour
    {
        public Transform blackBar; // 움직이는 검은색 바
        public float swingSpeed = 100f; // 회전 속도
        public float maxAngle = 90f; // 반구 최대 각도

        private bool goingRight = true;
        private Transform clone;

        void Awake()
        {
            clone = GetComponentInChildren<Transform>();
        }

        void Update()
        {
            float delta = swingSpeed * Time.deltaTime;
            float angle = blackBar.localEulerAngles.z;

            if (angle > 180) angle -= 360; // -180~180 범위

            if (goingRight)
            {
                angle += delta;
                if (angle >= maxAngle)
                {
                    angle = maxAngle;
                    goingRight = false;
                }
            }
            else
            {
                angle -= delta;
                if (angle <= -maxAngle)
                {
                    angle = -maxAngle;
                    goingRight = true;
                }
            }

            blackBar.localEulerAngles = new Vector3(0, 0, angle);
        }

        void OnEnable()
        {
            swingSpeed = 100;
        }

        void OnDisable()
        {
            var child = this.transform.GetChild(0).gameObject;
            if (child.CompareTag("Judgment"))
                Destroy(child);
        }
    }

}
