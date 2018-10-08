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
using CodeMonkey.MonoBehaviours;

public class GameHandler : MonoBehaviour {

    private static GameHandler instance;
    
    private GathererAI selectedGathererAI;
    
    [SerializeField] private Transform[] goldNodeTransformArray;
    [SerializeField] private Transform[] treeNodeTransformArray;
    [SerializeField] private Transform storageTransform;
    [SerializeField] private Transform towerPrefab;
    [SerializeField] private Transform gathererUnitPrefab;

    private List<ResourceNode> resourceNodeList;

    private void Awake() {
        instance = this;

        GameResources.Init();

        resourceNodeList = new List<ResourceNode>();
        foreach (Transform goldNodeTransform in goldNodeTransformArray) {
            resourceNodeList.Add(new ResourceNode(goldNodeTransform, GameResources.ResourceType.Gold));
        }
        foreach (Transform treeNodeTransform in treeNodeTransformArray) {
            resourceNodeList.Add(new ResourceNode(treeNodeTransform, GameResources.ResourceType.Wood));
        }

        ResourceNode.OnResourceNodeClicked += ResourceNode_OnResourceNodeClicked;
        GathererAI.OnGathererClicked += GathererAI_OnGathererClicked;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(1)) {
            // Construct a Tower
            if (Tower.TrySpendResourcesCost()) {
                Vector3 towerSpawnPosition = UtilsClass.GetMouseWorldPosition();
                Tower.Create(towerPrefab, towerSpawnPosition, () => {
                    SpawnGathererUnit(towerSpawnPosition + new Vector3(0, -3));
                });
            }
        }
    }

    private void SpawnGathererUnit(Vector3 position) {
        Instantiate(gathererUnitPrefab, position, Quaternion.identity);
    }

    private void GathererAI_OnGathererClicked(object sender, EventArgs e) {
        GathererAI gathererAI = sender as GathererAI;
        if (selectedGathererAI != null) {
            selectedGathererAI.HideSelectedSprite();
        }
        selectedGathererAI = gathererAI;
        selectedGathererAI.ShowSelectedSprite();
    }

    private void ResourceNode_OnResourceNodeClicked(object sender, EventArgs e) {
        ResourceNode resourceNode = sender as ResourceNode;
        if (selectedGathererAI != null) {
            // Order the selected Gatherer AI
            selectedGathererAI.SetResourceNode(resourceNode);
        } else {
            // Selected gatherer AI is null, no one is selected
        }
    }

    private ResourceNode GetResourceNode() {
        List<ResourceNode> tmpResourceNodeList = new List<ResourceNode>(resourceNodeList);
        for (int i = 0; i < tmpResourceNodeList.Count; i++) {
            if (!tmpResourceNodeList[i].HasResources()) {
                // No more resources
                tmpResourceNodeList.RemoveAt(i);
                i--;
            }
        }
        if (tmpResourceNodeList.Count > 0) {
            return tmpResourceNodeList[UnityEngine.Random.Range(0, tmpResourceNodeList.Count)];
        } else {
            return null;
        }
    }

    public static ResourceNode GetResourceNode_Static() {
        return instance.GetResourceNode();
    }
    
    private ResourceNode GetResourceNodeNearPosition(Vector3 position, GameResources.ResourceType resourceType) {
        float maxDistance = 20f;
        List<ResourceNode> tmpResourceNodeList = new List<ResourceNode>(resourceNodeList);
        for (int i = 0; i < tmpResourceNodeList.Count; i++) {
            if (!tmpResourceNodeList[i].HasResources() || 
                Vector3.Distance(position, tmpResourceNodeList[i].GetPosition()) > maxDistance ||
                tmpResourceNodeList[i].GetResourceType() != resourceType) {
                // No more resources, or too far, or different resource type
                tmpResourceNodeList.RemoveAt(i);
                i--;
            }
        }
        if (tmpResourceNodeList.Count > 0) {
            return tmpResourceNodeList[UnityEngine.Random.Range(0, tmpResourceNodeList.Count)];
        } else {
            return null;
        }
    }

    public static ResourceNode GetResourceNodeNearPosition_Static(Vector3 position, GameResources.ResourceType resourceType) {
        return instance.GetResourceNodeNearPosition(position, resourceType);
    }

    private Transform GetStorage() {
        return storageTransform;
    }

    public static Transform GetStorage_Static() {
        return instance.GetStorage();
    }
}
