using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;


public class TeamWeaponManager : NetworkBehaviour
{
    #region Serialized Fields

    [SerializeField] private List<BaseWeapon> P1WeaponsList = new List<BaseWeapon>();
    [SerializeField] private List<BaseWeapon> P2WeaponsList = new List<BaseWeapon>();

    [SerializeField] public MechScreen ammoCountScreenL;
    [SerializeField] public MechScreen ammoCountScreenR;
    [SerializeField] public SingleComboScript comboManager;

    //[SerializeField] private bool _enableStaggeredFire = true;
    //[SerializeField] private float staggeredFireTime = 0.25f;
    [SerializeField] private CinemachineImpulseSource shootingImpulseSource;

    #endregion

    #region Weapon State

    //public bool EnableStaggeredFire { get { return _enableStaggeredFire; } }

    private int _p1EquippedWeapon = 0;
    private int _p2EquippedWeapon = 0;

    

    public int P1EquippedWeapon { get { return _p1EquippedWeapon; } }
    public int P2EquippedWeapon { get { return _p2EquippedWeapon; } }

    //public Tuple<Transform,Transform> weaponTransforms = new Tuple<Transform, Transform>(null, null); //Item1 is P1 weapon transform, Item2 is P2 weapon transform
    [SerializeField] Transform weaponTranformOne;
    [SerializeField] Transform weaponTranformTwo;
    #endregion
    [Header("TESTING VARIABLES")]
    [SerializeField] ShopItemSO playerOneStartGun;
    [SerializeField] ShopItemSO playerTwoStartGun;

    #region Targeting Variables

    private RaycastHit hit;
    private Ray screenRay;
    private Vector3 rotDir;
    private float mouseDistance;

    private bool isStart = true;

    #endregion

    #region Unity Functions

    void Start()
    {
        GameManager.Instance.OnStartupSequence.AddListener(InitWeaponBuy);
        if (weaponTranformOne == null && P1WeaponsList.Count > 0)
        {
            weaponTranformOne = P1WeaponsList[_p1EquippedWeapon].transform;
        }

        if (weaponTranformTwo == null && P2WeaponsList.Count > 0)
        {
            weaponTranformTwo = P2WeaponsList[_p2EquippedWeapon].transform;
        }
    }
    public override void OnNetworkSpawn()
    {
        //if (IsOwner)
        //{
        //    PurchaseWeaponRpc(0, 0);
        //    PurchaseWeaponRpc(1, 1);
        //}

       

        
    }

    #endregion

    #region Input / External Setters

    public void SetScreenRay(Ray ray)
    {
        screenRay = ray;
        UpdateWeaponTarget();
    }

    public void SetMouseDistance(float distance)
    {
        mouseDistance = distance;
    }

    /// <summary>
    /// Changes the equipped weapon for the specified player and updates the weapon's position and rotation to match the player's weapon transform.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="item"></param>

    private void ChangeEquippedWeapon(int player, ShopItemSO item)
    {
        
            Transform mountPoint = (player == 0) ? weaponTranformOne : weaponTranformTwo;

            // Instantiate WITHOUT parent
            


            if (IsServer)
            {
                GameObject newWeapon; //= Instantiate(item.itemPrefab, mountPoint.position, mountPoint.rotation);
                if (player == 0)
                {
                    newWeapon = Instantiate(item.leftItemPrefab, mountPoint.position, mountPoint.rotation);
                }
                else if (player == 1)
                {
                    newWeapon = Instantiate(item.rightItemPrefab, mountPoint.position, mountPoint.rotation);
                }
                else
                {
                    Debug.LogError("Invalid player assignment: Player Number " + player);
                    newWeapon = null;
                }
                NetworkObject netObj = newWeapon.GetComponent<NetworkObject>();
                netObj.Spawn(true);
                if (isStart)
                {
                    AppendWeaponToList(player, newWeapon);
                    AppendWeaponToListRpc(player, netObj.NetworkObjectId);
                }
                ReplaceWeaponInList(player, newWeapon);
                ReplaceWeaponInListRpc(player, netObj.NetworkObjectId);
                newWeapon.transform.SetParent(mountPoint, true);
                BaseWeapon bW = newWeapon.GetComponent<WeaponCannon>();
                //Debug.Log(bW.name);
                bW.ammoCountScreen = (player == 0) ? ammoCountScreenL : ammoCountScreenR;
                //Debug.Log(bW.ammoCountScreen.name);
                bW.comboManager = comboManager;
                addReferencesRpc(player, netObj.NetworkObjectId);
            
            }

            // Parent after spawn
            

            
        
            
    }
    [Rpc(SendTo.NotServer)]
    private void addReferencesRpc(int player, ulong netObjId)
    {
        Debug.Log("adding ref");
        BaseWeapon weapon = NetworkManager.SpawnManager.SpawnedObjects[netObjId].gameObject.GetComponent<BaseWeapon>();
        BaseWeapon cannon = weapon.GetComponent<WeaponCannon>();
        cannon.ammoCountScreen = (player == 0) ? ammoCountScreenL : ammoCountScreenR;
        cannon.comboManager = comboManager;
    }

    #endregion

    

    #region Weapon Purchasing
    private void InitWeaponBuy()
    {
        if(!IsServer) { return; }
        PurchaseWeapon(0, playerOneStartGun);
        PurchaseWeapon(1, playerTwoStartGun);
        isStart = false;
    }
    public void PurchaseWeapon(int player, ShopItemSO item)
    {
        //Debug.Log($"[{gameObject.name}] Buying {item?.name}");
        //Debug.LogError($"[{gameObject.name}] ITEM IS NULL");

        //Debug.Log($"Buying {item.name} with prefab {item.itemPrefab.name}");
        if (player == 0)
        {
            // if (P1WeaponsList.Count > 0)
            // {
            //     RemoveWeaponFromList(0, P1WeaponsList[0].gameObject);
            // }
            //Debug.Log(item.itemName);
            // AppendWeaponToList(0, item.itemPrefab);
            //AppendWeaponToListRpc(0, item.itemIndex);
            ChangeEquippedWeapon(player, item);
        }
        else if (player == 1)
        {
            // if (P2WeaponsList.Count > 0)
            // {
            //     RemoveWeaponFromList(1, P2WeaponsList[0].gameObject);
            //     // P2WeaponsList[0] = item.itemPrefab.GetComponent<BaseWeapon>();
            // }
            // AppendWeaponToList(1, item.itemPrefab);
            //AppendWeaponToListRpc(1, item.itemIndex);
            ChangeEquippedWeapon(player, item);
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    [Rpc(SendTo.NotServer)]
    public void PurchaseWeaponRpc(int player, int index)
    {
        Debug.Log("start" + ShopManager.Instance.allItems.Count);
        ShopItemSO item = ShopManager.Instance.allItems[index];
        
        if (player == 0)
        {
            // if (P1WeaponsList.Count > 0)
            // {
            //     RemoveWeaponFromList(0, P1WeaponsList[0].gameObject);
            // }
            //Debug.Log(item.itemName);
            // AppendWeaponToList(0, item.itemPrefab);
            //AppendWeaponToListRpc(0, item.itemIndex);
            ChangeEquippedWeapon(player, item);
        }
        else if (player == 1)
        {
            // if (P2WeaponsList.Count > 0)
            // {
            //     RemoveWeaponFromList(1, P2WeaponsList[0].gameObject);
            // }

            // AppendWeaponToList(1, item.itemPrefab);
            //AppendWeaponToListRpc(1, item.itemIndex);
            ChangeEquippedWeapon(player, item);
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
        Debug.Log("end" + ShopManager.Instance.allItems.Count);
    }

    public void AppendWeaponToList(int player, GameObject weapon)
    {
        // Debug.Log(weapon.name);
        if (player == 0)
        {
            P1WeaponsList.Add(weapon.GetComponent<BaseWeapon>());
        }
        else if (player == 1)
        {
            P2WeaponsList.Add(weapon.GetComponent<BaseWeapon>());
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    [Rpc(SendTo.NotServer)]
    public void AppendWeaponToListRpc(int player, ulong networkObjId)
    {

        
        //GameObject weapon = NetworkManager.NetworkConfig.Prefabs.NetworkPrefabsLists[0].PrefabList[index].Prefab.gameObject;
        //Debug.Log(weapon.name);
        GameObject weapon = NetworkManager.SpawnManager.SpawnedObjects[networkObjId].gameObject;
        Debug.Log("Appending Weapon to player" + player + weapon);
        if (player == 0)
        {
            P1WeaponsList.Add(weapon.GetComponent<BaseWeapon>());
        }
        else if (player == 1)
        {
            P2WeaponsList.Add(weapon.GetComponent<BaseWeapon>());
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    public void ReplaceWeaponInList(int player, GameObject weapon)
    {
        // Debug.Log(weapon.name);
        if (player == 0)
        {
            P1WeaponsList[0] = weapon.GetComponent<BaseWeapon>();
        }
        else if (player == 1)
        {
            P2WeaponsList[0] = weapon.GetComponent<BaseWeapon>();
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    [Rpc(SendTo.NotServer)]
    public void ReplaceWeaponInListRpc(int player, ulong networkObjId)
    {

        
        //GameObject weapon = NetworkManager.NetworkConfig.Prefabs.NetworkPrefabsLists[0].PrefabList[index].Prefab.gameObject;
        //Debug.Log(weapon.name);
        GameObject weapon = NetworkManager.SpawnManager.SpawnedObjects[networkObjId].gameObject;
        Debug.Log("Replacing Weapon for player " + player + " with " + weapon);
        if (player == 0)
        {
            P1WeaponsList[0] = weapon.GetComponent<BaseWeapon>();
        }
        else if (player == 1)
        {
            // P2WeaponsList.Add(weapon.GetComponent<BaseWeapon>());
            P2WeaponsList[0] = weapon.GetComponent<BaseWeapon>();
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    public void RemoveWeaponFromList(int player, GameObject weapon)
    {
        BaseWeapon baseWeapon = weapon.GetComponent<BaseWeapon>();

        if (player == 0)
        {
            if (P1WeaponsList.Count > 0)
            {
                P1WeaponsList.Remove(baseWeapon);
                baseWeapon.enabled = false;
            }
            else
            {
                return;
            }
            // Destroy(weapon);
        }
        else if (player == 1)
        {
            if (P2WeaponsList.Count > 0)
            {
                P2WeaponsList.Remove(baseWeapon);
                baseWeapon.enabled = false;
            }
            else
            {
                return;
            }
            // Destroy(weapon);
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    #endregion

    #region IMPLEMENT THIS!!!!!!!!




    #region IMPLEMENT THIS!!!!!!!!
    #endregion

    public void PurchaseUtility(int player, ShopItemSO item) //TODO: Implement utility purchasing logic
    {
        if (player == 0)
        {
            
        }
        else if (player == 1)
        {
            
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    #endregion


    #region Targeting

    private void UpdateWeaponTarget()
    {
        Physics.Raycast(screenRay, out hit);

        for (int i = 0; i < P1WeaponsList.Count; i++)
        {
            rotDir = hit.GetDirectionFromRaycastHit(P1WeaponsList[i].Muzzle.position);
            P1WeaponsList[i].SetMuzzleRotation(hit, rotDir);
        }

        for (int i = 0; i < P2WeaponsList.Count; i++)
        {
            rotDir = hit.GetDirectionFromRaycastHit(P2WeaponsList[i].Muzzle.position);
            P2WeaponsList[i].SetMuzzleRotation(hit, rotDir);
        }
    }

    #endregion


    #region Firing

    public void FireWeapons(float input)
    {
        //Debug.Log("Input is: " + input);

        if (input == 0.25f) //P1 fire
        {
            P1WeaponsList[_p1EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();
        }
        else if (input == 0.75f) //P2 fire
        {
            P2WeaponsList[_p2EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();
        }
        else if (input == 1f) //Both P1 & P2 fire
        {
            P1WeaponsList[_p1EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();

            P2WeaponsList[_p2EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();
        }

        shootingImpulseSource.GenerateImpulse();
    }

    #endregion


    #region Experimental / Old Code

    //private IEnumerator WeaponFireRoutine(float input)
    //{
    //    ...
    //}

    //public void SetTeamWeaponsFireMode(TeamFireModes fireMode)
    //{
    //    ...
    //}

    #endregion


    #region Debug

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawSphere(hit.point, 0.25f);
    }
#endif

    #endregion
}

#region Enums

public enum TeamFireModes
{
    Simultaneous,
    Staggered
}

#endregion