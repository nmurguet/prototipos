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

public static class GameResources {

    public static event EventHandler OnResourceAmountChanged;

    public enum ResourceType {
        Gold,
        Wood,
    }

    private static Dictionary<ResourceType, int> resourceAmountDictionary;

    public static void Init() {
        resourceAmountDictionary = new Dictionary<ResourceType, int>();

        foreach (ResourceType resourceType in System.Enum.GetValues(typeof(ResourceType))) {
            resourceAmountDictionary[resourceType] = 0;
        }
    }


    public static void AddResourceAmount(ResourceType resourceType, int amount) {
        resourceAmountDictionary[resourceType] += amount;
        if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
    }

    public static void RemoveResourceAmount(ResourceType resourceType, int amount) {
        resourceAmountDictionary[resourceType] -= amount;
        if (OnResourceAmountChanged != null) OnResourceAmountChanged(null, EventArgs.Empty);
    }

    public static int GetResourceAmount(ResourceType resourceType) {
        return resourceAmountDictionary[resourceType];
    }
}
