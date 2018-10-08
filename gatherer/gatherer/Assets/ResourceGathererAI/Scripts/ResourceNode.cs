/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class ResourceNode {

    public static event EventHandler OnResourceNodeClicked;

    private const RegenerationMethod regenerationMethod = RegenerationMethod.TimedFull;

    private enum RegenerationMethod {
        Constant,
        TimedFull,
    }

    private Transform resourceNodeTransform;
    private GameResources.ResourceType resourceType;
    
    private int resourceAmount;
    private int resourceAmountMax;

    public ResourceNode(Transform resourceNodeTransform, GameResources.ResourceType resourceType) {
        this.resourceNodeTransform = resourceNodeTransform;
        this.resourceType = resourceType;
        resourceAmountMax = 3;
        resourceAmount = resourceAmountMax;
        resourceNodeTransform.GetComponent<Button_Sprite>().ClickFunc = () => {
            if (OnResourceNodeClicked != null) OnResourceNodeClicked(this, EventArgs.Empty);
        };

        if (regenerationMethod == RegenerationMethod.Constant) {
            FunctionPeriodic.Create(RegenerateSingleResourceAmount, 2f);
        }

        //CMDebug.TextUpdater(() => ""+resourceAmount, Vector3.zero, resourceNodeTransform);
    }

    public Vector3 GetPosition() {
        return resourceNodeTransform.position;
    }

    public GameResources.ResourceType GetResourceType() {
        return resourceType;
    }

    public GameResources.ResourceType GrabResource() {
        resourceAmount -= 1;
        if (resourceAmount <= 0) {
            // Node is depleted
            UpdateSprite();

            if (regenerationMethod == RegenerationMethod.TimedFull) {
                FunctionTimer.Create(ResetResourceAmount, 4f);
            }
        }
        return resourceType;
    }

    public bool HasResources() {
        return resourceAmount > 0;
    }

    private void ResetResourceAmount() {
        resourceAmount = resourceAmountMax;
        UpdateSprite();
    }

    private void RegenerateSingleResourceAmount() {
        if (resourceAmount < resourceAmountMax) {
            resourceAmount++;
            UpdateSprite();
        }
    }

    private void UpdateSprite() {
        if (resourceAmount > 0) {
            // Node has resources
            switch (resourceType) {
            default:
            case GameResources.ResourceType.Gold:
                resourceNodeTransform.GetComponent<SpriteRenderer>().sprite = GameAssets.i.goldNodeSprite;
                break;
            case GameResources.ResourceType.Wood:
                resourceNodeTransform.GetComponent<SpriteRenderer>().sprite = GameAssets.i.treeNodeSprite;
                break;
            }
        } else {
            // Node is depleted
            switch (resourceType) {
            default:
            case GameResources.ResourceType.Gold:
                resourceNodeTransform.GetComponent<SpriteRenderer>().sprite = GameAssets.i.goldNodeDepletedSprite;
                break;
            case GameResources.ResourceType.Wood:
                resourceNodeTransform.GetComponent<SpriteRenderer>().sprite = GameAssets.i.treeNodeDepletedSprite;
                break;
            }
        }
    }
}
