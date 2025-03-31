using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Target
{
    public override Team Team => Team.Enemy;
}
