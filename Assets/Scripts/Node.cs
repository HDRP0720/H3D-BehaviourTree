using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
  public enum EStatus { SUCCESS, RUNNING, FAILURE };

  public EStatus status;
  public List<Node> children = new List<Node>();
  public int currentChild = 0;
  public int sortOrder;
  public string name;

  // Constructor
  public Node(){}
  public Node(string n)
  {
    name = n;
  }
  public Node(string n, int order)
  {
    name = n;
    sortOrder = order;
  }

  public virtual EStatus Process()
  {
    return children[currentChild].Process();
  }

  public void AddChild(Node n)
  {
    children.Add(n);
  }
}
