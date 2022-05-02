using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : NetworkBehaviour // this scripts purpose is to know which units can be targeted at and where exactly to aim when targeting at them
{
    [SerializeField] private Transform aimAtPoint = null;

    public Transform GetAimAtPoint()
    {
        return this.aimAtPoint;
    }
}
