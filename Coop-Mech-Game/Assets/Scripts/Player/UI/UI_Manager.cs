using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : NetworkBehaviour
{
    [Header("Object References")]
    [SerializeField] private Image p1Cursor;
    [SerializeField] private Image p2Cursor;
    [SerializeField] private Image averageCursor;
    [SerializeField] private TextMeshProUGUI countdownText;
    private bool isCountdownActive = false;
    private float countdownTime = 3f;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TeamWeaponManager weaponMgr;

    [Header("Bar Material References")]
    //[SerializeField] private GameObject mechInterior; //the mesh of the interior with the materials on it
    [SerializeField] private MechBarScreen healthBarObject;
    [SerializeField] private MechBarScreen comboBarObject;
    
    [Header("Positioning")]
    [SerializeField] private Vector2 mouse1Pos;
    [SerializeField] private Vector2 mouse2Pos;
    [SerializeField] private Vector2 averagePos;

    private void Awake()
    {


    }

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        weaponMgr = GetComponent<TeamWeaponManager>();
        HideCountdown();

        //Debug.Log("health index: " + _healthIdx + ", combo index: " + _comboIdx);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get mouse positions from Player Controller
        mouse1Pos = playerController.mouse1Pos.Value;
        mouse2Pos = playerController.mouse2Pos.Value;
        averagePos = (mouse1Pos + mouse2Pos) / 2;
        
        // Normalize values to screen space
        mouse1Pos.x *= Screen.width;
        mouse1Pos.y *= Screen.height;
        mouse2Pos.x *= Screen.width;
        mouse2Pos.y *= Screen.height;
        averagePos.x *= Screen.width;
        averagePos.y *= Screen.height;

        // Move p1 cursor
        Vector3 p1Pos = p1Cursor.rectTransform.position;
        p1Pos.x = mouse1Pos.x;
        p1Pos.y = mouse1Pos.y;
        p1Cursor.rectTransform.position = p1Pos;

        // Move p2 cursor
        Vector3 p2Pos = p2Cursor.rectTransform.position;
        p2Pos.x = mouse2Pos.x;
        p2Pos.y = mouse2Pos.y;
        p2Cursor.rectTransform.position = p2Pos;

        // Move average cursor
        Vector3 averagePlayerPos = averageCursor.rectTransform.position;
        averagePlayerPos.x = averagePos.x;
        averagePlayerPos.y = averagePos.y;
        averageCursor.rectTransform.position = averagePlayerPos;

        //set point in weapon manager
        Ray ray = playerCamera.ScreenPointToRay(averagePlayerPos);
        if (weaponMgr != null) weaponMgr.SetScreenRay(ray);
        
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetCountdownRpc()
    {
        if (isCountdownActive) return;

        isCountdownActive = true;
        countdownTime = 3f;
        // countdownText.gameObject.SetActive(true);
        countdownText.text = countdownTime.ToString("F0");
        InvokeRepeating(nameof(UpdateCountdown), 1f, 1f);
    }

    private void UpdateCountdown()
    {
        if (!isCountdownActive) return;

        countdownTime -= 1f;
        if (countdownTime <= 0f)
        {
            countdownText.text = "";
            CancelInvoke(nameof(UpdateCountdown));
            Invoke(nameof(HideCountdown), 1f);
        }
        else
        {
            countdownText.text = countdownTime.ToString("F0");
        }
    }

    private void HideCountdown()
    {
        countdownText.text = "";
        isCountdownActive = false;
    }

    public void SetHealthBarPercent(float MechMaxHealth, float MechCurrHealth)
    {
        if (healthBarObject != null)
        {
            float h = MechCurrHealth.MapRange(0f, MechMaxHealth, 1f, 0f);
            Debug.Log("setting health factor to: "+h);
            healthBarObject.SetGradientFactor(h);
        }
        return;

        #region Archive
        //float h = MechCurrHealth.MapRange(0f, MechMaxHealth, 1f, 0f);
        //_healthRenderer.GetPropertyBlock(_block, _healthIdx);
        //Debug.Log("Setting health bar _BarPercent to: " + h);
        //_block.SetFloat("_BarPercent", h);
        //_healthRenderer.SetPropertyBlock(_block, _healthIdx);
        #endregion
    }

    public void SetComboBarPercent(float factor01)
    {
        // if (comboBarObject != null)
        // {
        //     float h = factor01.MapRange(0f, 1f, 1f, 0f);
        //     Debug.Log("setting combo factor to: " +h);
        //     comboBarObject.SetGradientFactor(h);
        // }
        return;

        #region Archive
        //float h = factor01.MapRange(0f, 1f, 1f, 0f);
        //_comboRenderer.GetPropertyBlock(_block, _comboIdx);
        //Debug.Log("Setting combo bar _BarPercent to: " + factor01);
        //_block.SetFloat("_BarPercent", h);
        //_comboRenderer.SetPropertyBlock(_block, _comboIdx);
        #endregion
    }
}