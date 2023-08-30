using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSelector : Node
{
  private bool bIsShuffled = false;

  // Constructor
  public RSelector(string n)
  {
    name = n;
  }

  public override EStatus Process()
  {
    if(!bIsShuffled)
    {
      children.Shuffle();
      bIsShuffled = true;
    } 

    EStatus childStatus = children[currentChild].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.SUCCESS)
    {
      currentChild = 0;
      bIsShuffled = false;
      return EStatus.SUCCESS;
    }

    currentChild++;
    if(currentChild >= children.Count)
    {
      currentChild = 0;
      bIsShuffled = false;
      return EStatus.FAILURE;
    }

    return EStatus.RUNNING;
  }
}
