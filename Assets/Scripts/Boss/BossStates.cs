using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossStates: MonoBehaviour
{
    /// <summary>
    /// 1. State 0: Idle - Remain stationary while shooting.
    /// 2. State Pattern 1: Circle around the player while shooting projectiles. Then Charge Towards the player.
    /// 3. State Pattern 2: Shoot projectiles while stationary. Then Summon minions.
    /// </summary>
    public enum State
    {
        IDLING,
        FIRSTPATTERN,
        SECONDPATTERN,
        DEAD
    }
}
