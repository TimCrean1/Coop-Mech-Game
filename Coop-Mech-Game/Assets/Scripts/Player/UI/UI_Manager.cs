using System;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Image p1Cursor;
    [SerializeField] private Image p2Cursor;
    [SerializeField] private Image averageCursor;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private TeamWeaponManager weaponMgr;

    [Header("Bar Material References")]
    [SerializeField] private GameObject mechInterior; //the mesh of the interior with the materials on it
    [SerializeField] private Material healthMaterial;
    [SerializeField] private Material comboMaterial;
    
    private Renderer _mechInteriorRenderer;
    private MaterialPropertyBlock _block;
    private int _healthIdx = -1;
    private int _comboIdx = -1;

    [Header("Positioning")]
    [SerializeField] private Vector2 mouse1Pos;
    [SerializeField] private Vector2 mouse2Pos;
    [SerializeField] private Vector2 averagePos;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        weaponMgr = GetComponent<TeamWeaponManager>();
        
        _mechInteriorRenderer = mechInterior.GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();

        var mats = _mechInteriorRenderer.sharedMaterials;

        for(int i = 0; i < mats.Length; i++)
        {
            if(mats[i] == healthMaterial)
            {
                _healthIdx = i;
            }

            if(mats[i] == comboMaterial)
            {
                _comboIdx = i;
            }
        }
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
        weaponMgr.SetScreenRay(ray);
        
    }

    public void SetHealthBarPercent(float MechMaxHealth, float MechCurrHealth)
    {
        float h = MechCurrHealth.MapRange(0f, MechMaxHealth, 0f, 1f);
        //_mechInteriorRenderer.GetPropertyBlock(_block, _healthIdx);
        _block.SetFloat("_BarPercent", h);
        _mechInteriorRenderer.SetPropertyBlock(_block, _healthIdx);
    }

    public void SetComboBarPercent(float factor01)
    {
        //_mechInteriorRenderer.GetPropertyBlock(_block, _comboIdx);
        _block.SetFloat("_BarPercent", factor01);
        _mechInteriorRenderer.SetPropertyBlock(_block, _comboIdx);
    }
}