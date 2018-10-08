using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Tower {

    public static bool TrySpendResourcesCost() {
        if (GameResources.GetResourceAmount(GameResources.ResourceType.Wood) >= 10 && 
            GameResources.GetResourceAmount(GameResources.ResourceType.Gold) >= 5) {
            // We do have resources
            GameResources.RemoveResourceAmount(GameResources.ResourceType.Wood, 10);
            GameResources.RemoveResourceAmount(GameResources.ResourceType.Gold, 5);
            // Spawn Tower
            return true;
        } else {
            // Not enough resources
            CMDebug.TextPopupMouse("Not enough resources!\nWood: 10; Gold: 5");
            return false;
        }
    }

    public static Tower Create(Transform towerPrefab, Vector3 position, Action onTowerConstructedAction) {
        Transform towerTransform = UnityEngine.Object.Instantiate(towerPrefab, position, Quaternion.identity);

        Tower tower = new Tower(towerTransform, onTowerConstructedAction);
        return tower;
    }




    private Transform towerTransform;
    private SpriteRenderer spriteRenderer;
    private int constructionTick;
    private int constructionTickMax;
    private World_Bar constructionBar;
    private Action onTowerConstructedAction;

    private Tower(Transform towerTransform, Action onTowerConstructedAction) {
        this.towerTransform = towerTransform;
        this.onTowerConstructedAction = onTowerConstructedAction;
        spriteRenderer = towerTransform.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = GameAssets.i.towerConstruction_1;
        constructionTick = 0;
        constructionTickMax = 10;

        FunctionPeriodic.Create(IncreaseConstructionTick, .5f);

        constructionBar = new World_Bar(towerTransform, new Vector3(0, -5), new Vector3(10, 1.5f), null, Color.yellow, 0f, 0, new World_Bar.Outline { color=Color.black, size=1f });
    }

    private void IncreaseConstructionTick() {
        if (IsConstructing()) {
            constructionTick++;
            switch (constructionTick) {
            case 3:  spriteRenderer.sprite = GameAssets.i.towerConstruction_2;      break;
            case 6:  spriteRenderer.sprite = GameAssets.i.towerConstruction_3;      break;
            case 10: spriteRenderer.sprite = GameAssets.i.towerConstruction_Built;  break;
            }
            float constructionNormalized = constructionTick * 1f / constructionTickMax;
            constructionBar.SetSize(constructionNormalized);

            if (constructionTick >= constructionTickMax) {
                // Tower is fully constructed
                constructionBar.Hide();
                onTowerConstructedAction();
            }
        }
    }

    private bool IsConstructing() {
        return constructionTick < constructionTickMax;
    }

}
