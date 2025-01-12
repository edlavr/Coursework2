﻿using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Snapper : MonoBehaviour
{
    [SerializeField] private GameObject destination;
    private float distance;
    private AccessManager accessManager;
    private MoveAndSync moveAndSync;
    [SerializeField] private Material snapMaterial;
    public bool isMotherboard;

    [CanBeNull] public Transform brokenPart;

    private void Start()
    {
        accessManager = GetComponent<AccessManager>();
        moveAndSync = GetComponent<MoveAndSync>();
        if (destination != null)
        {
            ChangeMaterials(destination, snapMaterial);
            destination.SetActive(false);
        }
    }
    
    void ChangeMaterials(GameObject target, Material material)
    {
        var children = target.GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (var j = 0; j < rend.materials.Length; j++) 
            { 
                mats[j] = material; 
            }
            rend.materials = mats;
        }
    }


    private void Update()
    {
        if (brokenPart != null)
        {
            if (Vector3.Distance(destination.transform.position, brokenPart.position) < 1.0f)
            {
                return;
            }
        }
        
        if (destination == null)
        {
            return;
        }
        if (!moveAndSync.grasped || !accessManager.available || accessManager.locked)
        {
            return;
        }
        distance = Vector3.Distance(transform.position, destination.transform.position);
        if (distance < .5f)
        {
                destination.GetComponent<MeshRenderer>().enabled = false;
                
                if (isMotherboard)
                {
                    var motherboard = FindObjectOfType<Motherboard>();
                    Debug.Log("mb part replaced");
                    motherboard.PartReplaced();
                }

                var mr = GetComponent<MeshRenderer>();
                transform.position = destination.transform.position;
                transform.rotation = destination.transform.rotation;
                
                if (mr != null)
                {
                    //ChangeMaterials(mr.material);
                    GetComponent<MoveAndSync>().ForceRelease();
                    GetComponent<AccessManager>().locked = true;
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    GetComponent<MoveAndSync>().ForceRelease();
                    gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                    GetComponent<AccessManager>().locked = true;
                }

        }
        else if (distance < 10.0f)
        {
            destination.SetActive(true);
        }
        else
        {
            destination.SetActive(false);
        }
    }
    
    private void ChangeMaterials(Material material)
    {
        var children = destination.GetComponentsInChildren<MeshRenderer>();
        foreach (var rend in children)
        {
            var mats = new Material[rend.materials.Length];
            for (int j = 0; j < rend.materials.Length; j++) 
            { 
                mats[j] = material; 
            }
            rend.materials = mats;
        }

    }
}
