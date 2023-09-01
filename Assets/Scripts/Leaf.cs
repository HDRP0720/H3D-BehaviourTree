using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : Node
{
  public delegate EStatus Tick();
  public Tick ProcessMethod;

  public delegate EStatus TickM(int val);
  public TickM ProcessMethodM;

  public int index;

  // Constructor
  public Leaf() {}
  public Leaf(string n, Tick pm)
  {
    name = n;
    ProcessMethod = pm;
  }
  public Leaf(string n, int i,TickM pm)
  {
    name = n;
    index = i;
    ProcessMethodM = pm;
  }
  public Leaf(string n, Tick pm, int order)
  {
    name = n;
    ProcessMethod = pm;
    sortOrder = order;
  }

  public override EStatus Process()
  {
    if(ProcessMethod != null)
      return ProcessMethod();
    else if(ProcessMethodM != null)
      return ProcessMethodM(index);

    return EStatus.FAILURE;
  }
}
