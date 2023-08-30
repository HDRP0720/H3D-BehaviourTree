using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSelector : Node
{
  // Constructor
  public RSelector(string n)
  {
    name = n;
  }

  public override EStatus Process()
  {
    children.Shuffle();
    
    EStatus childStatus = children[currentChild].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.SUCCESS)
    {
      currentChild = 0;
      return EStatus.SUCCESS;
    }

    currentChild++;
    if(currentChild >= children.Count)
    {
      currentChild = 0;
      return EStatus.FAILURE;
    }

    return EStatus.RUNNING;
  }
}
