using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemyMovement : MonoBehaviour
{
    bool registered;
    Vector3 HealthBarScale;
    public Slider HealthBar;
    public GameObject SliderObject;
    public int entitiesLastFrame = 1;
    Animator Anim;
    public float maxHealth;
    public float Health;
    public float DesiredDistance;
    public int myTurn;
    public int TilePos;
    public float damage;
    public LayerMask attackLayer;
    public TurnManager TurnManager;
    int distance;
    int dir;
    bool charged;
    bool dead;
    bool readied;
    bool Attacking;
    // Start is called before the first frame update
    void Awake()
    {
        
        

    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.value = Health / maxHealth;
        Anim = GetComponent<Animator>();
        TurnManager = GetComponentInParent<TurnManager>();

        if (!registered)
        {
            HealthBarScale = SliderObject.transform.localScale;
            registered = true;
        }

        if (TurnManager.needUpdateTurns)
        {
            myTurn--;
        }

        if (Health <= 0 && !dead)
        {
            Die();
        }

        if(TurnManager.entities < entitiesLastFrame)
        {
            myTurn--;
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
                        else if (!TurnManager.occupied[TilePos + TurnManager.radius + 1])
                        {
                            MoveRight();
                        }
                        else
                        {
                            Wait();
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
                        else if (!TurnManager.occupied[TilePos + TurnManager.radius - 1])
                        {
                            MoveLeft();
                        }
                        else
                        {
                            Wait();
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
        TurnManager.entities--;
        TurnManager.deadEnemies++;
        TurnManager.needUpdateTurns = true;
        Destroy(gameObject);
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
        if(transform.localScale.x > 0)
        {
            SliderObject.transform.localScale = HealthBarScale;
        }
        else
        {
            SliderObject.transform.localScale = new Vector3(HealthBarScale.x * -1, HealthBarScale.y, HealthBarScale.z);
        }
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
        RaycastHit2D AttackRay = Physics2D.Raycast(transform.position, Vector3.right * transform.lossyScale.x, 1, attackLayer);
        if(AttackRay.collider != null)
        {
            TurnManager.health -= damage;
        }
        TurnManager.NextTurn();
        Anim.SetInteger("state", 0);
        Attacking = false;
        charged = false;
    }

}
