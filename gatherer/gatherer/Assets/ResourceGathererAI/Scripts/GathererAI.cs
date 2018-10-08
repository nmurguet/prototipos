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

public class GathererAI : MonoBehaviour {
    
    public static event EventHandler OnGathererClicked;

    private enum State {
        Idle,
        MovingToResourceNode,
        GatheringResources,
        MovingToStorage,
    }

    private IUnit unit;
    private State state;
    private ResourceNode resourceNode;
    private Transform storageTransform;
    private Dictionary<GameResources.ResourceType, int> inventoryAmountDictionary;
    private TextMesh inventoryTextMesh;

    private void Awake() {
        unit = gameObject.GetComponent<IUnit>();
        state = State.Idle;

        inventoryAmountDictionary = new Dictionary<GameResources.ResourceType, int>();
        foreach (GameResources.ResourceType resourceType in System.Enum.GetValues(typeof(GameResources.ResourceType))) {
            inventoryAmountDictionary[resourceType] = 0;
        }

        inventoryTextMesh = transform.Find("inventoryTextMesh").GetComponent<TextMesh>();
        UpdateInventoryText();

        transform.GetComponent<Button_Sprite>().ClickFunc = () => {
            if (OnGathererClicked != null) OnGathererClicked(this, EventArgs.Empty);
        };
    }

    private int GetTotalInventoryAmount() {
        int total = 0;
        
        foreach (GameResources.ResourceType resourceType in System.Enum.GetValues(typeof(GameResources.ResourceType))) {
            total += inventoryAmountDictionary[resourceType];
        }

        return total;
    }

    private bool IsInventoryFull() {
        return GetTotalInventoryAmount() >= 3;
    }

    private void DropInventoryAmountIntoGameResources() {
        foreach (GameResources.ResourceType resourceType in System.Enum.GetValues(typeof(GameResources.ResourceType))) {
            GameResources.AddResourceAmount(resourceType, inventoryAmountDictionary[resourceType]);
            inventoryAmountDictionary[resourceType] = 0;
        }
    }

    private void UpdateInventoryText() {
        int inventoryAmount = GetTotalInventoryAmount();
        if (inventoryAmount > 0) {
            inventoryTextMesh.text = "" + inventoryAmount;
        } else {
            inventoryTextMesh.text = "";
        }
    }

    private void Update() {
        switch (state) {
        case State.Idle:
            //resourceNode = GameHandler.GetResourceNode_Static();
            if (resourceNode != null) {
                state = State.MovingToResourceNode;
            }
            break;
        case State.MovingToResourceNode:
            if (unit.IsIdle()) {
                unit.MoveTo(resourceNode.GetPosition(), 10f, () => {
                    state = State.GatheringResources;
                });
            }
            break;
        case State.GatheringResources:
            if (unit.IsIdle()) {
                if (IsInventoryFull() || !resourceNode.HasResources()) {
                    // Move to storage
                    storageTransform = GameHandler.GetStorage_Static();
                    resourceNode = GameHandler.GetResourceNodeNearPosition_Static(resourceNode.GetPosition(), resourceNode.GetResourceType());
                    state = State.MovingToStorage;
                } else {
                    // Gather resources
                    switch (resourceNode.GetResourceType()) {
                    case GameResources.ResourceType.Gold:
                        unit.PlayAnimationMine(resourceNode.GetPosition(), GrabResourceFromNode);
                        break;
                    case GameResources.ResourceType.Wood:
                        unit.PlayAnimationWoodChop(resourceNode.GetPosition(), GrabResourceFromNode);
                        break;
                    }
                }
            }
            break;
        case State.MovingToStorage:
            if (unit.IsIdle()) {
                unit.MoveTo(storageTransform.position, 10f, () => {
                    DropInventoryAmountIntoGameResources();
                    UpdateInventoryText();
                    state = State.Idle;
                });
            }
            break;
        }
    }

    public void SetResourceNode(ResourceNode resourceNode) {
        this.resourceNode = resourceNode;
    }

    public void ShowSelectedSprite() {
        transform.Find("SelectedCircle").gameObject.SetActive(true);
    }

    public void HideSelectedSprite() {
        transform.Find("SelectedCircle").gameObject.SetActive(false);
    }

    private void GrabResourceFromNode() {
        GameResources.ResourceType resourceType = resourceNode.GrabResource();
        inventoryAmountDictionary[resourceType]++;
        UpdateInventoryText();
    }
}
