using UnityEngine;
[CreateAssetMenu(fileName = "MonsterData", menuName = "Game/Monster Data")]
public class MonsterData : ScriptableObject
{
    public string monsterName;

    public float maxHealth;
    public float moveSpeed;
    public float damage;
}
