using UnityEngine;

public class BreathCheck : MonoBehaviour
{
    public Transform halfCircleBar;      // 반원 중심
    public GameObject hitZonePrefab;     // 판정 칸 Prefab


    public void SpawnHitZone()
    {
        // 반원의 SpriteRenderer 기준 반지름 계산
        SpriteRenderer sr = halfCircleBar.GetComponent<SpriteRenderer>();
        float radius = sr.bounds.size.x / 2f;

        // 반원의 각도를 랜덤 선택 (-90 ~ 90)
        float angle = Random.Range(180f, 0f);
        float rad = angle * Mathf.Deg2Rad;

        // 반원의 중심 기준 테두리 좌표 계산
        Vector3 spawnPos = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0) * radius;
        spawnPos += halfCircleBar.position;

        // HitZone 생성
        GameObject hitZone = Instantiate(hitZonePrefab, spawnPos, Quaternion.identity);
        hitZone.transform.parent = halfCircleBar;

        Debug.Log("HitZone spawned at: " + spawnPos + " | Angle: " + angle);
    }
}
