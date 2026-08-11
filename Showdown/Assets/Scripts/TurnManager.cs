using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public GameObject[] FireTiles;
    public float PoisonDamage;
    public float FireDamage;
    public bool needUpdateTurns;
    GameObject newEnemy;
    int currentWave = -1;
    public int waveCount;
    public int[] WavesInfo;
    public GameObject EnemyPrefab;
    public float health;
    public float maxHealth;
    public GameObject PlayerObj;
    public Slider HealthBar;
    public bool[] occupied;
    public bool[] onFire;
    public int[] TurnsLeftOnFire;
    public int radius;
    public player player;
    public int entities = 1;
    public int deadEnemies;
    public int turn = 0;
    public int PlayerPos;
    public int rounds;
    int roundsLastFrame;
    // Start is called before the first frame update
    void Start()
    {
        NextWave();
    }

    // Update is called once per frame
    void Update()
    {
        turn = turn % entities;
            for (int i = 0; i < onFire.Length; i++)
            {
                if (onFire[i])
                {
                    FireTiles[i].SetActive(true);
                }
                else
                {
                    FireTiles[i].SetActive(false);
                }


            }

        StartCoroutine(resetTurnUpdate());
        HealthBar.value = health / maxHealth;
        PlayerPos = player.TilePos;

        if (entities == 1 && player.canGoToNextWave)
        {
            NextWave();
        }
    }

    public void NextTurn()
    {
        turn++;
        turn = turn % entities;
    }

    IEnumerator resetTurnUpdate()
    {
        yield return new WaitForEndOfFrame();
        needUpdateTurns = false;
    }

    public void NewRound()
    {
        rounds++;
        for(int i = 0; i < TurnsLeftOnFire.Length; i++)
        {
            if (TurnsLeftOnFire[i] > 0)
            {
                TurnsLeftOnFire[i]--;
            }
            else
            {
                onFire[i] = false;
            }
        }
    }
    public void NextWave()
    {
        entities = 1;
        currentWave++;
        int j = 1;
        int varsetter = 0;
        for(int i = radius * 2; i >= 0; i--)
        {
            newEnemy = null;
            if (WavesInfo[currentWave] >= Mathf.Pow(2, i))
            {
                if (!occupied[i])
                {
                    varsetter = i;
                    occupied[i] = true;
                    newEnemy = Instantiate(EnemyPrefab, new Vector3(i - radius, -.25f, 0), Quaternion.identity, transform);
                }
                else if (!occupied[i+1])
                {
                    varsetter = i + 1;
                    occupied[i+1] = true;
                    newEnemy = Instantiate(EnemyPrefab, new Vector3(i + 1 - radius, -.25f, 0), Quaternion.identity, transform);
                }else if (!occupied[i - 1])
                {
                    varsetter = i - 1;
                    occupied[i - 1] = true;
                    newEnemy = Instantiate(EnemyPrefab, new Vector3(i - 1 - radius, -.25f, 0), Quaternion.identity, transform);
                }
                if (newEnemy != null)
                {
                    enemyMovement enemyScript = newEnemy.GetComponent<enemyMovement>();
                    enemyScript.myTurn = j;
                    j++;
                    entities++;
                    deadEnemies = 0;
                    enemyScript.TilePos = varsetter - radius;
                }
                WavesInfo[currentWave] -= (int)Mathf.Pow(2, i);
            }
        }
    }

    IEnumerator resetRounds()
    {
        yield return new WaitForEndOfFrame();
        roundsLastFrame = rounds;
    }
}
