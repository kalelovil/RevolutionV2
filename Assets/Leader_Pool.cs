using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader_Pool : MonoBehaviour
{
    [SerializeField] List<BrigadeLeader> _leaderPool = new List<BrigadeLeader>();
    public List<BrigadeLeader> LeaderPool => _leaderPool;
}
