using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyState
{
    Patrol, Chase, Shocked, Hit, Stun, Dead
}

public interface IEnemyState
{
    EnemyState StateId { get; }
    void Enter();
    void Exit();
}