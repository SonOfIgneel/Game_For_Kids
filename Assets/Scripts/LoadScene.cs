using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Category List", menuName = "ScriptableObjects/LoadScene")]
public class LoadScene : ScriptableObject
{
    public List<string> assignedcards;
    public List<Vector3> assignedcardspos;
}
