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

    [Header("Positioning")]
    [SerializeField] private Vector2 mouse1Pos;
    [SerializeField] private Vector2 mouse2Pos;
    [SerializeField] private Vector2 averagePos;
    
    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
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
    }
}