using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour
{
  public delegate void GhostDead();

  [Header("Genes")]
  [SerializeField]
  private int genesSize;
  [SerializeField]
  private float minTimeImpulse;
  [SerializeField]
  private float maxTimeImpulse;

  [Header("Behaviour")]
  [SerializeField]
  private float rotationSpeed;
  [SerializeField]
  private float forceMagnitude;
  [SerializeField]
  private float timeToDie;

  private GhostGenes genes;

  private Rigidbody2D rb2D;

  private float currentAngle;
  private Vector2 directionImpulse;

  private float timePassed;
  private float targetTime;

  private float timeWaitingToDie;

  private int currentGeneIndex;

  private bool dying;
  private bool dead;

  public GhostDead GhostDeadDelegate;

  private void Awake()
  {
    InitializeGhost();
  }

  public void InitializeGhost()
  {
    genes = new GhostGenes(genesSize, minTimeImpulse, maxTimeImpulse);

    rb2D = GetComponent<Rigidbody2D>();

    ResetGhost();
  }

  public void ResetGhost()
  {
    currentAngle = 0;
    directionImpulse = new Vector2();

    timePassed = 0;
    targetTime = genes.TimesForImpulses[0];
    
    timeWaitingToDie = 0;

    currentGeneIndex = 0;

    dying = false;
    dead = false;
  }

  private void FixedUpdate()
  {
    if (!dying && !dead)
    {
      RotateImpulseDirection();
      Move();
    }

    else if (dying && !dead)
    {
      WaitAndDie();
    }
  }

  private void RotateImpulseDirection()
  {
    currentAngle += rotationSpeed * Time.deltaTime;

    if (currentAngle >= 360f)
    {
      currentAngle -= 360f;
    }

    float currentAngleRadians = Mathf.Deg2Rad * currentAngle;

    directionImpulse.x = Mathf.Cos(currentAngleRadians) * transform.up.x - Mathf.Sin(currentAngleRadians) * transform.up.y;
    directionImpulse.y = Mathf.Sin(currentAngleRadians) * transform.up.x - Mathf.Cos(currentAngleRadians) * transform.up.y;

    directionImpulse.Normalize();

    Debug.DrawRay(transform.position, directionImpulse * forceMagnitude, Color.red);
  }

  private void Move()
  {
    timePassed += Time.deltaTime;

    if (timePassed >= targetTime)
    {
      timePassed = 0;

      rb2D.AddForce(directionImpulse * forceMagnitude, ForceMode2D.Impulse);

      if (++currentGeneIndex < genes.TimesForImpulses.Length)
      {
        targetTime = genes.TimesForImpulses[currentGeneIndex];
       // rotationSpeed = genes.TurnSpeed[currentGeneIndex];
      }

      else
      {
        dying = true;
      }
    }
  }

  private void WaitAndDie()
  {
    timeWaitingToDie += Time.deltaTime;

    if (timeWaitingToDie >= timeToDie)
    {
      dead = true;

      rb2D.velocity = Vector2.zero;

      GhostDeadDelegate.Invoke();
    }
  }

  public GhostGenes GetGenes()
  {
    return genes;
  }

  public float GetMinTimeImpulse()
  {
    return minTimeImpulse;
  }

  public float GetMaxTimeImpulse()
  {
    return maxTimeImpulse;
  }
}
