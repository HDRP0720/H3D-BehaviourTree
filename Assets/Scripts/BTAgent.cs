using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTAgent : MonoBehaviour
{
  public enum EActionState { IDLE, WORKING };

  public NavMeshAgent agent;
  public BehaviourTree tree;

  public EActionState state = EActionState.IDLE;  
  public Node.EStatus treeStatus = Node.EStatus.RUNNING;

  private WaitForSeconds waitForSeconds;
  private Vector3 savedLocation;

  public virtual void Start() 
  {
    agent = this.GetComponent<NavMeshAgent>();
    tree = new BehaviourTree();

    waitForSeconds = new WaitForSeconds(Random.Range(0.1f, 1f));
    StartCoroutine(Behave());
  }

  private IEnumerator Behave()
  {
    while(true)
    {
      treeStatus = tree.Process();
      yield return waitForSeconds;
    }
  }

  public Node.EStatus CanSee(Vector3 target, string tag, float distance, float maxAngle)
  {
    Vector3 directionToTarget = target - this.transform.position;
    float angle = Vector3.Angle(directionToTarget, this.transform.forward);
    if(angle <= maxAngle || directionToTarget.magnitude <= distance)
    {
      RaycastHit hitInfo;
      if(Physics.Raycast(this.transform.position, directionToTarget, out hitInfo))
      {
        if(hitInfo.collider.gameObject.CompareTag(tag))
        {
          return Node.EStatus.SUCCESS;
        }
      }
    }

    return Node.EStatus.FAILURE;
  }

  public Node.EStatus Flee(Vector3 location, float distance)
  {
    if(state == EActionState.IDLE)
    {
      savedLocation = this.transform.position + (transform.position - location).normalized * distance;
    }
    return GoToLocation(savedLocation);
  }

  public Node.EStatus GoToLocation(Vector3 destination)
  {
    float distanceToTarget = Vector3.Distance(destination, this.transform.position);

    if(state == EActionState.IDLE)
    {
      agent.SetDestination(destination);
      state = EActionState.WORKING;   
    }
    else if(Vector3.Distance(agent.pathEndPosition, destination) >= 2)
    {
      state = EActionState.IDLE;
      return Node.EStatus.FAILURE;
    }
    else if (distanceToTarget < 2)
    {
      state = EActionState.IDLE;
      return Node.EStatus.SUCCESS;
    }

    return Node.EStatus.RUNNING;
  }

  public Node.EStatus GoToDoor(GameObject door)
  {
    Node.EStatus s = GoToLocation(door.transform.position);
    if(s == Node.EStatus.SUCCESS)
    {
      if(!door.GetComponent<Lock>().isLocked)
      {
        door.GetComponent<NavMeshObstacle>().enabled = false;
        return Node.EStatus.SUCCESS;
      }

      return Node.EStatus.FAILURE;
    }
    else
    {
      return s;
    }
  }

  public Node.EStatus IsOpen()
  {
    if(Blackboard.Instance.timeOfDay < 9 || Blackboard.Instance.timeOfDay > 17)    
      return Node.EStatus.FAILURE;    
    else
      return Node.EStatus.SUCCESS;  
  }
}
