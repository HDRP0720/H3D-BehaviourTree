using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSelector : Node
{
  private Node[] nodeArray;
  private bool bIsOrdered = false;

  // Constructor
  public PSelector(string n)
  {
    name = n;
  }

  private void OrderNodes()
  {
    nodeArray = children.ToArray();
    Sort(nodeArray, 0, children.Count - 1);
    children = new List<Node>(nodeArray);
  }

  public override EStatus Process()
  {
    if(!bIsOrdered)
    {
      OrderNodes();
      bIsOrdered = true;
    }

    EStatus childStatus = children[currentChild].Process();

    if(childStatus == EStatus.RUNNING) return EStatus.RUNNING;

    if(childStatus == EStatus.SUCCESS)
    {
      // children[currentChild].sortOrder = 1;
      currentChild = 0;
      bIsOrdered = false;
      return EStatus.SUCCESS;
    }
    // else
    // {
    //   children[currentChild].sortOrder = 10;
    // }

    currentChild++;
    if(currentChild >= children.Count)
    {
      currentChild = 0;
      bIsOrdered = false;
      return EStatus.FAILURE;
    }

    return EStatus.RUNNING;
  }

  int Partition(Node[] array, int low, int high)
  {    
    Node pivot = array[high];

    int lowIndex = (low - 1);

    //2. Reorder the collection.
    for (int j = low; j < high; j++)
    {
      if (array[j].sortOrder <= pivot.sortOrder)
      {
        lowIndex++;

        Node temp = array[lowIndex];
        array[lowIndex] = array[j];
        array[j] = temp;
      }
    }

    Node temp1 = array[lowIndex + 1];
    array[lowIndex + 1] = array[high];
    array[high] = temp1;

    return lowIndex + 1;
  }

  void Sort(Node[] array, int low, int high)
  {
    if (low < high)
    {
      int partitionIndex = Partition(array, low, high);

      //3. Recursively continue sorting the array
      Sort(array, low, partitionIndex - 1);
      Sort(array, partitionIndex + 1, high);
    }
  }
}
