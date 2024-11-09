using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum Enemy_MeleeWeaponType { OneHand, Throw, Unarmed }
public enum Enemy_RangeWeaponType { Pistol, Revolver, Riffle, Shotgun, Sniper}

public class EnemyVisual : MonoBehaviour
{ 
    public GameObject currentWeaponModel { get; private set; }

    [Header("Corruption Visual")]
    [SerializeField] private GameObject[] corruptionCrystals;
    [SerializeField] private int corruptionAmount;

    [Header("Enemy Look")]
    [SerializeField] private Texture[] textures;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("IK Ref")]
    [SerializeField] private TwoBoneIKConstraint leftHandIkConstraint;
    [SerializeField] private MultiAimConstraint weaponAim;
    [SerializeField] private Transform leftHandIk_target;
    [SerializeField] private Transform leftElbowIk_hint;

    private void Awake()
    {
        
    }

    private void Start()
    {
        CollectCorruptionCrystals();

        SetupLook();
    }

    public void EnableTrailEffect(bool active)
    {
        EnemyWeaponModel enemyWeaponModel =  currentWeaponModel.GetComponent<EnemyWeaponModel>();
        enemyWeaponModel.EnableTrailEffect(active);
    }

    public void SetupLook()
    {
        SetupRandomTexture();
        SetupRandomWeapon();
        SetupRandomCorruption();
    }

    private void SetupRandomCorruption()
    {
        List<int> availableIndexes = new List<int>();
        corruptionCrystals = CollectCorruptionCrystals();

        for(int i = 0; i < corruptionCrystals.Length; i++)
        {
            availableIndexes.Add(i);
            corruptionCrystals[i].SetActive(false);
        }

        for(int i = 0; i < corruptionAmount; i++)
        {
            if(availableIndexes.Count == 0)
            {
                break;
            }
            int randomIndex = Random.Range(0, availableIndexes.Count);
            int objectIndex = availableIndexes[randomIndex];

            corruptionCrystals[objectIndex].SetActive(true);
            availableIndexes.RemoveAt(randomIndex);
        }
    }

    private void SetupRandomWeapon()
    {
        bool isEnemyMelee = GetComponent<EnemyMelee>() != null;
        bool isEnemyRange = GetComponent<EnemyRange>() != null;

        if(isEnemyRange)
        {
            currentWeaponModel = FindRangeWeaponModel();
        }

        if (isEnemyMelee)
        {
            currentWeaponModel = FindMeleeWeaponModel();
        }
        currentWeaponModel.SetActive(true);
        OverrideAnimatorControllerIfCan();

    }

    private GameObject FindRangeWeaponModel()
    {
        EnemyRangeWeaponModel[] weaponModels = GetComponentsInChildren<EnemyRangeWeaponModel>(true);
        Enemy_RangeWeaponType weaponType = GetComponent<EnemyRange>().weaponType;

        foreach(var weaponModel in weaponModels)
        {
            if(weaponModel.weaponType == weaponType)
            {
                SetLayerAnimator((int)weaponModel.weaponHoldType);
                SetupLeftHandTarget(weaponModel.leftHandTarget, weaponModel.leftHandHint);
                return weaponModel.gameObject;
            }
        }

        Debug.Log("Range Weapon Not Found!");
        return null;
    }

    private GameObject FindMeleeWeaponModel()
    {
        EnemyWeaponModel[] enemyWeaponModels = GetComponentsInChildren<EnemyWeaponModel>(true);
        Enemy_MeleeWeaponType weaponType = GetComponent<EnemyMelee>().weaponType;
        List<EnemyWeaponModel> filteredWeaponModels = new List<EnemyWeaponModel>();

        foreach (var weaponModel in enemyWeaponModels)
        {
            if (weaponModel.weaponType == weaponType)
            {
                filteredWeaponModels.Add(weaponModel);
            }
        }

        int randomIndex = Random.Range(0, filteredWeaponModels.Count);

        return filteredWeaponModels[randomIndex].gameObject;
    }

    private void OverrideAnimatorControllerIfCan()
    {
        AnimatorOverrideController animatorOverrideController = currentWeaponModel.GetComponent<EnemyWeaponModel>()?.overrideController;

        if (animatorOverrideController != null)
        {
            GetComponentInChildren<Animator>().runtimeAnimatorController = animatorOverrideController;
        }
    }

    private void SetupRandomTexture()
    {
        int randomTextureIndex = Random.Range(0, textures.Length);

        Material newMat = new Material(skinnedMeshRenderer.material);
        newMat.mainTexture = textures[randomTextureIndex];

        skinnedMeshRenderer.material = newMat;
    }

    private GameObject[] CollectCorruptionCrystals()
    {
        EnemyCorruptionCrystal[] enemyCorruptionCrystals = GetComponentsInChildren<EnemyCorruptionCrystal>(true);

        GameObject[] corruptionCrystals = new GameObject[enemyCorruptionCrystals.Length];

        for (int i = 0; i < enemyCorruptionCrystals.Length; i++)
        {
            corruptionCrystals[i] = enemyCorruptionCrystals[i].gameObject;
        }

        return corruptionCrystals;
    }

    private void SetLayerAnimator(int layerIndex)
    {
        Animator animator = GetComponentInChildren<Animator>();
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i, 0);
        }

        animator.SetLayerWeight(layerIndex, 1);
    }

    public void EnableIK(bool leftHandEnable, bool aimEnable)
    {
        leftHandIkConstraint.weight = leftHandEnable ? 1 : 0;
        weaponAim.weight = aimEnable ? 1 : 0;
    }

    public void SetupLeftHandTarget(Transform leftHand_target, Transform leftHand_hint)
    {
        this.leftHandIk_target.localPosition = leftHand_target.localPosition;
        this.leftElbowIk_hint.localPosition = leftHand_hint.localPosition;

        this.leftHandIk_target.localRotation = leftHand_target.localRotation;
        this.leftElbowIk_hint.localRotation = leftHand_hint.localRotation;
    }
}
