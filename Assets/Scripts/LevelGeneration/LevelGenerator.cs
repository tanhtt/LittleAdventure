using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private List<Transform> parts;
    [SerializeField] private SnapPoint nextSnapPoint;

    private void Start()
    {
        GenerateNextLevelPart();
    }

    [ContextMenu("Create next level part")]
    private void GenerateNextLevelPart()
    {
        Transform nextLevelPart = Instantiate(ChooseRandomPart());
        LevelPart levelPart = nextLevelPart.GetComponent<LevelPart>();

        levelPart.SnapAndAlignPartTo(nextSnapPoint);
        nextSnapPoint = levelPart.GetEndPoint();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, parts.Count);

        Transform choosenPart = parts[randomIndex];
        parts.RemoveAt(randomIndex);

        return choosenPart;
    }
}
