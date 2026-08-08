using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public float health;
    public float maxHealth;
    public GameObject PlayerObj;
    public Slider HealthBar;
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
        HealthBar.value = health / maxHealth;
        PlayerPos = player.TilePos;   
    }

    public void NextTurn()
    {
        turn++;
        turn = turn % entities;
    }
}
