using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SnapPointType { Start, End }

public class SnapPoint : MonoBehaviour
{
    [SerializeField] public SnapPointType pointType;

    private void OnValidate()
    {
        gameObject.name = "SnapPoint - " + pointType.ToString();
    }
}
