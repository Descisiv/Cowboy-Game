using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyMovement : MonoBehaviour
{
    Animator Anim;
    public float Health;
    public float DesiredDistance;
    public int myTurn;
    public int TilePos;
    public TurnManager TurnManager;
    int distance;
    int dir;
    bool charged;
    bool dead;
    bool readied;
    bool Attacking;
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.entities++;
        Anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Health <= 0 && !dead)
        {
            Die();
        }

        if (TurnManager.turn == myTurn && !Attacking)
        {
            distance = TurnManager.PlayerPos - TilePos;
            dir = (int)transform.localScale.x;
            if (dead)
            {
                Wait();
            }
            else if (charged)
            {
                if (readied)
                {
                    Attack();
                }
                else if (distance > 0)
                {
                    if (dir > 0)
                    {
                        if (distance == DesiredDistance)
                        {
                            //switch to attack when implemented
                            Ready();
                        }
                        else
                        {
                            MoveRight();
                        }
                    }
                    else
                    {
                        Flip();
                    }
                }
                else
                {
                    if (dir > 0)
                    {
                        Flip();
                    }
                    else
                    {
                        if (distance == DesiredDistance * -1)
                        {
                            //swap with attack when possible
                            Ready();
                        }
                        else
                        {
                            MoveLeft();
                        }
                    }
                }
            }
            else
            {
                Wait();
            }
        }
    }

    void Attack()
    {
        Attacking = true;
        Anim.SetInteger("state", 2);
        readied = false;
        StartCoroutine(AttackTimer());
    }
    void Ready()
    {
        Anim.SetInteger("state", 1);
        readied = true;
        TurnManager.NextTurn();
    }
    void Die()
    {
        TurnManager.occupied[TilePos + TurnManager.radius] = false;
        transform.position += Vector3.down * 10;
        dead = true;
    }
    void MoveRight()
    {
        TurnManager.occupied[TilePos + TurnManager.radius] = false;
        TurnManager.occupied[TilePos + TurnManager.radius + 1] = true;
        TilePos++;
        transform.position += Vector3.right;
        TurnManager.NextTurn();
    }
    void MoveLeft()
    {
        TurnManager.occupied[TilePos + TurnManager.radius] = false;
        TurnManager.occupied[TilePos + TurnManager.radius - 1] = true;
        TilePos--;
        transform.position += Vector3.left;
        TurnManager.NextTurn();
    }

    void Flip()
    {
        charged = false;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        TurnManager.NextTurn();
    }

    void Wait()
    {
        TurnManager.NextTurn();
        charged = true;
    }

    IEnumerator AttackTimer()
    {
        yield return new WaitForSeconds(.5f);
        TurnManager.NextTurn();
        Anim.SetInteger("state", 0);
        Attacking = false;
        charged = false;
    }

}
