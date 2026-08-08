using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public bool[] occupied;
    public int radius;
    public player player;
    public int entities = 1;
    public int turn = 0;
    public int PlayerPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPos = player.TilePos;   
    }

    public void NextTurn()
    {
        turn++;
        turn = turn % entities;
    }
}
