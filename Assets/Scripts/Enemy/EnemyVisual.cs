using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_MeleeWeaponType { OneHand, Throw }

public class EnemyVisual : MonoBehaviour
{
    [Header("Weapon Visual")]
    [SerializeField] private EnemyWeaponModel[] enemyWeaponModels;
    [SerializeField] private Enemy_MeleeWeaponType weaponType;

    public GameObject currentWeaponModel { get; private set; }

    [Header("Corruption Visual")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Enemy Look")]
    [SerializeField] private Texture[] textures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        enemyWeaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
    }

    private void Start()
    {
        CollectCorruptionCrystals();

        InvokeRepeating(nameof(SetupLook), 1, 1.5f);
    }

    public void SetupLook()
    {
        SetupRandomTexture();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndexs = new List<int>();
        for(int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexs.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for(int i = 0; i < corruptionAmount; i++)
        {
            if(availableIndexs.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, availableIndexs.Count);
            int objectIndex = availableIndexs[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexs.RemoveAt(randomIndex);
        }
    }

    public void SetupWeaponType(Enemy_MeleeWeaponType weaponType)
    {
        this.weaponType = weaponType;
    }

    private void SetupRandomWeapon()
    {
        foreach(EnemyWeaponModel model in enemyWeaponModels)
        {
            model.gameObject.SetActive(false);
        }

        List<EnemyWeaponModel> filteredWeaponModels = new List<EnemyWeaponModel>();

        foreach(var weaponModel in enemyWeaponModels)
        {
            if(weaponModel.weaponType == weaponType)
            {
                filteredWeaponModels.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        currentWeaponModel = filteredWeaponModels[randomIndex].gameObject; 
        currentWeaponModel.SetActive(true);
    }

    private void SetupRandomTexture()
    {
        int randomTextureIndex = Random.Range(0, textures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);
        newMat.mainTexture = textures[randomTextureIndex];

        skinnedMeshRenderer.material = newMat;
    }

    private void CollectCorruptionCrystals()
    {
        EnemyCorruptionCrystal[] enemyCorruptionCrystals = GetComponentsInChildren<EnemyCorruptionCrystal>(true);

        corruptionCrystals = new GameObject[enemyCorruptionCrystals.Length];

        for (int i = 0; i < enemyCorruptionCrystals.Length; i++)
        {
            corruptionCrystals[i] = enemyCorruptionCrystals[i].gameObject;
        }
    }
}
