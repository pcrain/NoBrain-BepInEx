using UnityEngine;

public class NBInteractableBehaviour : AbstractNBInteractableBehaviour {

    private const string SEPARATOR_TEXT = "[color #bada55][/color]";
    private const int PAGE_AMMO = 0;
    private const int PAGE_DESC = 1;
    private const int PAGE_STATS = 2;
    private const int PAGE_SYNERGIES = 3;
    private const int PAGE_LENGTH = 4;

    private int currentPage = PAGE_AMMO;

    private DefaultLabelController labelControllerCache;
    
    private void Start() {
        NoBrain.LogFine("Start of NoBrainBehaviour");
    }

    protected override void onNewInteractable(IPlayerInteractable interactable) {
        if (interactable is ShopItemController sic) {
            with(sic.transform, sic.item, onNewShopItemController);
        } else if (interactable is PickupObject po) {
            if (NoBrainDB.ITEM_BLACKLIST.Contains(po.PickupObjectId)) {
                return;
            }

            // spawn the label here first
            spawnPickupObjectLabel(po, po.transform);
            with(po.transform, po, onNewShopItemController);
        } else if (interactable is RewardPedestal rp) {
            PickupObject ipo = rp.contents;
            if (NoBrainDB.ITEM_BLACKLIST.Contains(ipo.PickupObjectId)) {
                return;
            }

            // spawn the label here first
            spawnPickupObjectLabel(ipo, rp.spawnTransform);
            with(rp.spawnTransform, ipo, onNewShopItemController);
        } else if (interactable is Chest c && NoBrain.SHOW_CHEST_CONTENTS) {
            spawnChestLabel(c);
        }
    }

    private void spawnPickupObjectLabel(PickupObject po, Transform poTransform) {
        var offset = new Vector3(po.sprite.GetBounds().max.x + 3f / 16f, po.sprite.GetBounds().min.y, 0.0f);
        var text = po.getDisplayName();
        var labelGameObject = GameUIRoot.Instance.RegisterDefaultLabel(poTransform, offset, text);
        labelControllerCache = labelGameObject.GetComponent<DefaultLabelController>();
        var dfLabel = labelGameObject.GetComponentInChildren<dfLabel>();
        dfLabel.ColorizeSymbols = false;
        dfLabel.ProcessMarkup = true;
    }
    
    private void spawnChestLabel(Chest c) {
        var offset = new Vector3(c.sprite.GetBounds().max.x + 3f / 16f, c.sprite.GetBounds().min.y, 0.0f);
        var chestContents = c.PredictContents(getPrimaryPlayer());
        var text = chestContents.join(po => po.getDisplayName(), "\n");
        var labelGameObject = GameUIRoot.Instance.RegisterDefaultLabel(c.transform, offset, text);
        labelControllerCache = labelGameObject.GetComponent<DefaultLabelController>();
        var dfLabel = labelGameObject.GetComponentInChildren<dfLabel>();
        dfLabel.ColorizeSymbols = false;
        dfLabel.ProcessMarkup = true;
//        dfLabel.AutoSize = true;
//        labelControllerCache.panel.FitToContents();
    }

    protected override void onCleanupInteractable(IPlayerInteractable interactable) {
        if (interactable is PickupObject || interactable is Chest || interactable is RewardPedestal) {
            if (labelControllerCache == null) {
                return;
            } 
            Destroy(labelControllerCache.gameObject);
            GameUIRoot.Instance.extantBasicLabels.Remove(labelControllerCache);
        }
    }

    protected override void onReloadPressed(IPlayerInteractable interactable) {
        if (interactable is ShopItemController sic) {
            currentPage = (currentPage + 1) % PAGE_LENGTH;
            with(sic.transform, sic.item, onUpdateShopItemController);
        } else if (interactable is PickupObject po) {
            currentPage = (currentPage + 1) % PAGE_LENGTH;
            with(po.transform, po, onUpdateShopItemController);
        } else if (interactable is RewardPedestal rp) {
            currentPage = (currentPage + 1) % PAGE_LENGTH;
            with(rp.spawnTransform, rp.contents, onUpdateShopItemController);
        }
    }

    private delegate void UseLabelAndItem(DefaultLabelController labelController,
        EncounterTrackable encounter, PickupObject item);

    private void with(Transform transform, PickupObject po, UseLabelAndItem action) {
        if (NoBrainDB.ITEM_BLACKLIST.Contains(po.PickupObjectId)) {
            return;
        }

        DefaultLabelController labelController = getLabelController(transform);
        if (labelController == null) {
            return;
        }
        var encounterTrackable = po.GetComponent<EncounterTrackable>();
        if (encounterTrackable == null) {
            return;
        }

        var item = po;
        if (item is ItemBlueprintItem) {
            var newItem = PickupObjectDatabase.GetByEncounterName(
                encounterTrackable.journalData.GetPrimaryDisplayName());
            if (newItem != null) {
                item = newItem;
            } else {
                NoBrain.Log("Couldn't find the real item behind the ItemBlueprintItem.");
            }
        }
        
        action.Invoke(labelController, encounterTrackable, item);
    }

    private DefaultLabelController getLabelController(Transform transform) {
        if (labelControllerCache != null && labelControllerCache.targetObject == transform) {
            return labelControllerCache;
        }
        
        DefaultLabelController labelController = null;
        foreach (var basicLabel in GameUIRoot.Instance.extantBasicLabels) {
            if (basicLabel.targetObject == transform) {
                labelController = basicLabel;
                break;
            }
        }

        labelControllerCache = labelController;
        return labelController;
    }

    private void onNewShopItemController(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        // add padding fix behaviour that only updates position and not the padding
        labelController.gameObject.AddComponent<PositionUpdater>();
        
        // Text change
        var newText = labelController.label.Text;
        newText += SEPARATOR_TEXT;
        newText += getTextForPage(encounter, item);
        labelController.label.Text = newText;
        
        // Layouting
        labelController.enabled = false;
        labelController.label.TextScale = 2f;
        labelController.label.AutoHeight = true;
        labelController.label.WordWrap = true;
        labelController.label.Width = Screen.width / 3f;
        labelController.label.Padding = new RectOffset(6,6,0,0);
        labelController.label.Invalidate();
        labelController.panel.FitToContents();
        labelController.panel.Invalidate();
        labelController.Trigger();
    }
    
    private void onUpdateShopItemController(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        
        var success = NoBrainDB.ITEMS.TryGetValue(item.PickupObjectId, out var noBrainJsonItem);
        if (!success) {
            return;
        }
        
        // Text change
        var newText = labelController.label.Text;
        var indexOf = newText.IndexOf(SEPARATOR_TEXT) + SEPARATOR_TEXT.Length;
        if (indexOf == -1) {
            newText = "ERROR FINDING SEPARATOR";
        } else {
            newText = newText.Substring(0, indexOf);
            newText += getTextForPage(encounter, item);
        }
        labelController.label.Text = newText;
        
        // Layouting
        labelController.label.AutoHeight = true;
        labelController.label.Invalidate();
        labelController.panel.FitToContents();
        labelController.panel.Invalidate();
        labelController.Trigger();
    }

    private string getTextForPage(EncounterTrackable encounter, PickupObject item) {
        string pageDescription;
        string text = "";

        var itemDictSuccess = NoBrainDB.ITEMS.TryGetValue(item.PickupObjectId, out var noBrainJsonItem);
        
        var passiveActiveString = item is PassiveItem ? "Passive" : "Active";
        text = "[color #7d7d7d]";
        if (NoBrain.SHOW_ITEM_IDS) {
            text += " " + item.PickupObjectId;
        }
        text += " " + item.quality.getUISpriteString()
                    + " " + passiveActiveString
                    + "[/color]";
        
        if (currentPage == PAGE_AMMO || !itemDictSuccess) {
            pageDescription = "Ammonomicon";
            var ammonomiconFullEntry = encounter.journalData.GetAmmonomiconFullEntry(false, false);
            if (ammonomiconFullEntry.Length > 0) {
                text += "\n[color #a0a0a0]" + ammonomiconFullEntry.TrimEnd()
                       + "[/color]";
            }
        } else if (currentPage == PAGE_DESC) {
            pageDescription = "Description";
            text += "\n[color #a0a0a0]" + noBrainJsonItem.desc + "[/color]";
        } else if (currentPage == PAGE_STATS) {
            pageDescription = "Stats";
            text += "\n[color #a0a0a0]" + noBrainJsonItem.stats + "[/color]";
        } else if (currentPage == PAGE_SYNERGIES) {
            pageDescription = "Synergies";
            var synergySuccess = NoBrainDB.SYNERGIES.TryGetValue(item.PickupObjectId, out var synergyList);
            if (synergySuccess) {
                foreach (var advancedSynergyEntry in synergyList) {
                    var mandatoryItemString = advancedSynergyEntry.getMandatoryString();
                    var optionalItemString = advancedSynergyEntry.getOptionalString();
                    text += "\n[color #98FAFF]" + advancedSynergyEntry.getName()
                                                + "[/color] " + advancedSynergyEntry.NumberObjectsRequired
                                                + " of (" + mandatoryItemString + ") [" + optionalItemString + "]";
                }
                text += "";
            } else {
                text = "\nNo Synergies found.";
            }
        } else {
            pageDescription = "ERROR";
            text = "INVALID PAGE";
        }

        if (!itemDictSuccess) {
            // skip footer, only one page available
            return text;
        }

        return text + "\n[color #3f704d]" + pageDescription + "(" + (currentPage+1) + "/"
               + PAGE_LENGTH + ") Press " + getReloadSpriteTag() + "[/color]";
    }
    
}