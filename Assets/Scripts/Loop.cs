using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loop : Node
{
  private BehaviourTree dependancy;

  // Constructor
  public Loop(string n, BehaviourTree d)
  {
    name = n;
    dependancy = d;
  }

  public override EStatus Process()
  {
    if(dependancy.Process() == EStatus.FAILURE) return EStatus.SUCCESS;

    EStatus childStatus = children[currentChild].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.FAILURE)
    {
      currentChild = 0;
      foreach (Node n in children)
      {
        n.Reset();
      }

      return EStatus.FAILURE;
    } 

    currentChild++;
    if(currentChild >= children.Count)    
      currentChild = 0;

    return EStatus.RUNNING;
  }
}
