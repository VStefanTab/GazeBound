using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frightness : MonoBehaviour
{
    [Header("Frightness Settings")]
    public float frightness = 0f;
    public float maxFrightness = 100f;
    public float increaseRate = 10f;
    public float decreaseRate = 5f;

    [Header("Detection Settings")]
    public NPCController monster;
    public float detectionAngle = 30f;   // degrees from center of screen
    public float detectionRange = 20f;   // max distance to trigger fright

    public Camera playerCamera;

    void Start()
    {
        frightness = 0f;
        playerCamera = Camera.main;

        // Add this check
        if (playerCamera == null)
            Debug.LogError("No Main Camera found! Make sure your camera is tagged as 'MainCamera'");
    }

    void Update()
    {
        if (IsLookingAtMonster())
            frightness += increaseRate * Time.deltaTime;
        else
            frightness -= decreaseRate * Time.deltaTime;

        frightness = Mathf.Clamp(frightness, 0f, maxFrightness);
    }

    bool IsLookingAtMonster()
    {
        if (monster == null) return false;
        print(monster.transform.position);
        Vector3 directionToMonster = monster.transform.position - playerCamera.transform.position;
        print(directionToMonster);
        float distance = directionToMonster.magnitude;

        // Too far away to be frightened
        if (distance > detectionRange) return false;

        // Check the angle between camera forward and direction to monster
        float angle = Vector3.Angle(playerCamera.transform.forward, directionToMonster);

        return angle < detectionAngle;
    }
}