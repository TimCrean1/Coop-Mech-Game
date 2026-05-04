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
    [SerializeField] private List<BaseUtility> P1UtilitiesList = new List<BaseUtility>();
    [SerializeField] private List<BaseUtility> P2UtilitiesList = new List<BaseUtility>();


    //[SerializeField] private bool _enableStaggeredFire = true;
    //[SerializeField] private float staggeredFireTime = 0.25f;
    [SerializeField] private CinemachineImpulseSource shootingImpulseSource;
    [SerializeField] private UtilityManagerScript utilityManager;
    [SerializeField] private PlayerAudioManager audioManager;

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
    /// Changes the equipped weapon for the specified player by destroying the old weapon,
    /// instantiating and spawning the new weapon, updating weapon lists, and setting references.
    /// </summary>
    /// <param name="player">Player index (0 or 1)</param>
    /// <param name="item">The ShopItemSO representing the new weapon</param>
    private void ChangeEquippedWeapon(int player, ShopItemSO item)
    {
        // Select the correct weapon mount point based on player
        Transform mountPoint = (player == 0) ? weaponTranformOne : weaponTranformTwo;

        // Only the server should handle spawning and replacing weapons
        if (IsServer)
        {
            // Destroy the old weapon if it exists
            BaseWeapon oldWeapon = null;
            if (player == 0 && P1WeaponsList.Count > 0)
                oldWeapon = P1WeaponsList[_p1EquippedWeapon];
            else if (player == 1 && P2WeaponsList.Count > 0)
                oldWeapon = P2WeaponsList[_p2EquippedWeapon];

            if (oldWeapon != null)
            {
                NetworkObject _netObj = oldWeapon.GetComponent<NetworkObject>();
                if (_netObj != null && _netObj.IsSpawned)
                    _netObj.Despawn(true); // Despawn and destroy on clients
            }

            // Instantiate the new weapon at the mount point's position and rotation
            GameObject newWeapon = Instantiate(item.itemPrefab, mountPoint.position, mountPoint.rotation);

            // Spawn the new weapon as a networked object
            NetworkObject netObj = newWeapon.GetComponent<NetworkObject>();
            netObj.Spawn(true);

            // If this is the initial startup, append to the weapon list
            if (isStart)
            {
                AppendWeaponToList(player, newWeapon);
                AppendWeaponToListRpc(player, netObj.NetworkObjectId);
            }

            // Replace the weapon in the list for this player
            ReplaceWeaponInList(player, newWeapon);
            ReplaceWeaponInListRpc(player, netObj.NetworkObjectId);

            // Parent the new weapon to the mount point
            newWeapon.transform.SetParent(mountPoint, true);

            // Set references for ammo screen and combo manager
            BaseWeapon bW = newWeapon.GetComponent<BaseWeapon>();
            bW.ammoCountScreen = (player == 0) ? ammoCountScreenL : ammoCountScreenR;
            bW.comboManager = comboManager;

            // Notify clients to update references
            addWeaponReferencesRpc(player, netObj.NetworkObjectId);
        }
    }

    /// <summary>
    /// Changes the equipped utility for the specified player by destroying the old utility,
    /// instantiating and spawning the new utility, updating utility lists, and setting references.
    /// </summary>
    /// <param name="player">Player index (0 or 1)</param>
    /// <param name="item">The ShopItemSO representing the new utility</param>
    private void ChangeEquippedUtility(int player, ShopItemSO item)
    {
        Transform mountPoint = gameObject.transform;

        if (!IsServer) return;

        List<BaseUtility> list = (player == 0) ? P1UtilitiesList : P2UtilitiesList;

        // Ensure list is safe to index
        if (list.Count == 0)
        {
            list.Add(null);
        }

        // Destroy old utility safely
        BaseUtility oldUtility = list[0];

        if (oldUtility != null)
        {
            NetworkObject netObj = oldUtility.GetComponent<NetworkObject>();
            if (netObj != null && netObj.IsSpawned)
                netObj.Despawn(true);
        }

        // Spawn new utility
        GameObject newUtility = Instantiate(item.itemPrefab, mountPoint.position, mountPoint.rotation);
        NetworkObject newNetObj = newUtility.GetComponent<NetworkObject>();
        newNetObj.Spawn(true);

        BaseUtility utility = newUtility.GetComponent<BaseUtility>();
        utility.SetUtilityManager(utilityManager);

        utilityManager.SetPlayerUtility(player, utility);

        // Replace locally (server)
        list[0] = utility;

        // Parent (optional)
        // newUtility.transform.SetParent(mountPoint, true);

        // Sync clients
        ReplaceUtilityInListRpc(player, newNetObj.NetworkObjectId);
        addUtilityReferencesRpc(player, newNetObj.NetworkObjectId);
    }

    [Rpc(SendTo.Server)]
    private void requestChangeWeaponRpc(int player, int index)
    {
        ShopItemSO item = ShopManager.Instance.allItems[index];
        ChangeEquippedWeapon(player, item);
        Debug.Log("Client requests item purchase " + item);
    }
    [Rpc(SendTo.Server)]
    private void requestChangeUtilityRpc(int player, int index)
    {
        ShopItemSO item = ShopManager.Instance.allItems[index];
        ChangeEquippedUtility(player, item);
        Debug.Log("Client requests item purchase " + item);
    }
    [Rpc(SendTo.NotServer)]
    private void addWeaponReferencesRpc(int player, ulong netObjId)
    {
        Debug.Log("adding ref");
        BaseWeapon weapon = NetworkManager.SpawnManager.SpawnedObjects[netObjId].gameObject.GetComponent<BaseWeapon>();
        BaseWeapon cannon = weapon.GetComponent<BaseWeapon>();
        cannon.ammoCountScreen = (player == 0) ? ammoCountScreenL : ammoCountScreenR;
        cannon.comboManager = comboManager;

        // if (player == 0)
        // {
        //     audioManager.SetP1GunClip(cannon.weaponAudioClip);
        //     audioManager.p1GunSource = cannon.audioSource;
        // }
        // else
        // {
        //     audioManager.SetP2GunClip(cannon.weaponAudioClip);
        //     audioManager.p2GunSource = cannon.audioSource;
        // }
    }
    [Rpc(SendTo.NotServer)]
    private void addUtilityReferencesRpc(int player, ulong netObjId)
    {
        Debug.Log("adding utility ref");

        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(netObjId, out var netObj))
        {
            Debug.LogWarning($"Utility netObj {netObjId} not spawned yet on client");
            return;
        }

        BaseUtility utility = netObj.GetComponent<BaseUtility>();

        if (utility == null)
        {
            Debug.LogWarning("Utility component missing");
            return;
        }

        utility.SetUtilityManager(utilityManager);
        utilityManager.SetPlayerUtility(player, utility);

        // if (player == 0)
        // {
        //     audioManager.SetP1UtilityClip(utility.utilityAudioClip);
        //     audioManager.p1UtilitySource = utility.audioSource;
        // }
        // else
        // {
        //     audioManager.SetP2UtilityClip(utility.utilityAudioClip);
        //     audioManager.p2UtilitySource = utility.audioSource;
        // }
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
        if (player == 0)
        {
            ChangeEquippedWeapon(player, item);
        }
        else if (player == 1)
        {
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
        Debug.Log("client is buying this index of weapon " + index);
        //ShopItemSO item = ShopManager.Instance.allItems[index];
        
        if (player == 0)
        {
            requestChangeWeaponRpc(player,index);
        }
        else if (player == 1)
        {
            requestChangeWeaponRpc(player, index);
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
        GameObject weapon = NetworkManager.SpawnManager.SpawnedObjects[networkObjId].gameObject;
        Debug.Log("Replacing Weapon for player " + player + " with " + weapon);
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

    #endregion

    #region Utility Purchasing

    public void PurchaseUtility(int player, ShopItemSO item) //TODO: Implement utility purchasing logic
    {
        if (player == 0)
        {
            ChangeEquippedUtility(player, item);
        }
        else if (player == 1)
        {
            ChangeEquippedUtility(player, item);
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    [Rpc(SendTo.NotServer)]
    public void PurchaseUtilityRpc(int player, int index)
    {
        Debug.Log("client is buying this index of utility " + index);
        //ShopItemSO item = ShopManager.Instance.allItems[index];
        
        if (player == 0)
        {
            requestChangeUtilityRpc(player,index);
        }
        else if (player == 1)
        {
            requestChangeUtilityRpc(player, index);
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
        Debug.Log("end" + ShopManager.Instance.allItems.Count);
    }

    public void ReplaceUtilityInList(int player, GameObject utility)
    {
        // Debug.Log(utility.name);
        if (player == 0)
        {
            P1UtilitiesList[0] = utility.GetComponent<BaseUtility>();
            utilityManager.SetPlayerUtility(player, utility.GetComponent<BaseUtility>());
        }
        else if (player == 1)
        {
            P2UtilitiesList[0] = utility.GetComponent<BaseUtility>();
            utilityManager.SetPlayerUtility(player, utility.GetComponent<BaseUtility>());
        }
        else
        {
            Debug.LogError("Player " + player + " does not exist!");
        }
    }

    [Rpc(SendTo.NotServer)]
    public void ReplaceUtilityInListRpc(int player, ulong networkObjId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjId, out var netObj))
        {
            Debug.LogWarning($"Utility {networkObjId} not spawned yet on client");
            return;
        }

        BaseUtility utility = netObj.GetComponent<BaseUtility>();

        List<BaseUtility> list = (player == 0) ? P1UtilitiesList : P2UtilitiesList;

        if (list.Count == 0)
            list.Add(null);

        list[0] = utility;
        utilityManager.SetPlayerUtility(player, utility);
    }

    public void AppendUtilityToList(int player, GameObject utility)
    {
        List<BaseUtility> list = (player == 0) ? P1UtilitiesList : P2UtilitiesList;

        list.Add(utility.GetComponent<BaseUtility>());
    }

    [Rpc(SendTo.NotServer)]
    public void AppendUtilityToListRpc(int player, ulong networkObjId)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjId, out var netObj))
        {
            Debug.LogWarning($"Utility {networkObjId} not spawned yet");
            return;
        }

        GameObject utilityObj = netObj.gameObject;
        BaseUtility utility = utilityObj.GetComponent<BaseUtility>();

        List<BaseUtility> list = (player == 0) ? P1UtilitiesList : P2UtilitiesList;

        if (list.Count == 0)
            list.Add(null);

        list[0] = utility;
        utilityManager.SetPlayerUtility(player, utility);
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
            // audioManager.PlayP1GunSound();
        }
        else if (input == 0.75f) //P2 fire
        {
            P2WeaponsList[_p2EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();
            // audioManager.PlayP2GunSound();
        }
        else if (input == 1f) //Both P1 & P2 fire
        {
            P1WeaponsList[_p1EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();

            P2WeaponsList[_p2EquippedWeapon].Fire(mouseDistance);
            shootingImpulseSource.GenerateImpulse();

            // audioManager.PlayP1GunSound();
            // audioManager.PlayP2GunSound();
        }

        shootingImpulseSource.GenerateImpulse();
    }

    #endregion

    #region Reloading

    // public void P1Reload()
    // {
    //     P1WeaponsList[_p1EquippedWeapon].Reload();
    // }
    // public void P2Reload()
    // {
    //     P2WeaponsList[_p2EquippedWeapon].Reload();
    // }

    [Rpc(SendTo.Server)]
    private void RequestReloadRpc(int player)
    {
        if (player == 0)
        {
            P1WeaponsList[_p1EquippedWeapon].Reload();
        }
        else if (player == 1)
        {
            P2WeaponsList[_p2EquippedWeapon].Reload();
        }
    }

    public void P1Reload()
    {
        RequestReloadRpc(0);
    }

    public void P2Reload()
    {
        RequestReloadRpc(1);
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