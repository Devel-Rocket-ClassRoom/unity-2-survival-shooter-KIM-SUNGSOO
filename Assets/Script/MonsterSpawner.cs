using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterSpawner : MonoBehaviour
{
    public Monster[] monsterPrefabs;          // 생성할 몬스터
    public MonsterData[] monsterDatas;     // 몬스터 데이터들
    

    private List<Monster> monsters = new List<Monster>();

    public int wave = 0;

    private void Update()
    {
        if (monsters.Count == 0)
        {
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        wave++;

        int count = Mathf.RoundToInt(wave * 1.5f);

        for (int i = 0; i < count; i++)
        {
            SpawnMonster();
        }

        Debug.Log($"웨이브 {wave} 시작 / 몬스터 수: {monsters.Count}");
    }

    private void SpawnMonster()
    {
        Vector3 spawnPos = GetRandomPointOnNavMesh(transform.position, 20f);

        //프리팹 랜덤 선택
        int index = Random.Range(0, monsterPrefabs.Length);

        var monster = Instantiate(monsterPrefabs[index], spawnPos, Quaternion.identity);

        monster.gameObject.SetActive(false);

        // 데이터도 같은 인덱스로 맞추기
        var data = monsterDatas[index];
        monster.Setup(data);

        monsters.Add(monster);

        monster.gameObject.SetActive(true);

        monster.OnDead.AddListener(() => monsters.Remove(monster));
    }

    Vector3 GetRandomPointOnNavMesh(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++) // 30번 시도
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return center; // 실패하면 중심 반환
    }
}