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
using UnityEngine.UI;

public class Window_GameResources : MonoBehaviour {

    private void Awake() {
        GameResources.OnResourceAmountChanged += delegate (object sender, EventArgs e) {
            UpdateResourceTextObject();
        };
        UpdateResourceTextObject();
    }

    private void UpdateResourceTextObject() {
        transform.Find("goldAmount").GetComponent<Text>().text = 
            "GOLD: " + GameResources.GetResourceAmount(GameResources.ResourceType.Gold) + "\n" + 
            "WOOD: " + GameResources.GetResourceAmount(GameResources.ResourceType.Wood) + "\n";
    }

}
