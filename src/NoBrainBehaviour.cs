using System.Collections.Generic;
using UnityEngine;

public class NoBrainBehaviour : MonoBehaviour {

    private static readonly List<int> ITEM_BLACKLIST = new List<int>(new [] {
        67, 73, 85, 120 // key, half heart, full heart, armor
    });

    private const int PAGE_AMMO = 0;
    private const int PAGE_DESC = 1;
    private const int PAGE_STATS = 2;
    private const int PAGE_SYNERGIES = 3;
    private const int PAGE_LENGTH = 4;

    private const string SEPARATOR_TEXT = "[color #bada55][/color]";
    
    private List<DefaultLabelController> lastLabels = new List<DefaultLabelController>();
    private int lastHashcode;
    private int currentPage = PAGE_AMMO;
    private int nextPage = PAGE_AMMO;

    private GungeonActions gungeonActions;
    private string reloadSpriteTag;
    private float lastGungeonActionsUpdate;
    
    private void Start() {
        NoBrain.LogFine("Start of NoBrainBehaviour");
    }

    private void Update() {
        lastGungeonActionsUpdate += Time.deltaTime;
        if ((gungeonActions == null || lastGungeonActionsUpdate > 2f)
            && GameManager.Instance != null && GameManager.Instance.PrimaryPlayer != null) {
            // TODO even more expensive here, every couple seconds we check if there is new input
            var braveInput = BraveInput.PrimaryPlayerInstance;
            gungeonActions = braveInput.ActiveActions;
            reloadSpriteTag = gungeonActions.ReloadAction.getUISpriteString(braveInput.IsKeyboardAndMouse());
            lastGungeonActionsUpdate = 0;
        }
        if (gungeonActions?.ReloadAction.WasPressed ?? false) {
            nextPage = (currentPage + 1) % PAGE_LENGTH;
        }
        
        if (GameUIRoot.Instance == null) {
            return;
        }

        bool updateLabels = nextPage != currentPage;
        if (updateLabels) {
            currentPage = nextPage;
        }
        var labels = GameUIRoot.Instance.extantBasicLabels;
        var hashCode = labels.deepHashCode();
        if (hashCode == lastHashcode && !updateLabels) {
            return;
        }
        lastHashcode = hashCode;
        foreach (var label in labels) {
            if (lastLabels.Contains(label)) {
                if (updateLabels) {
                    withItem(label, onUpdateLabel);
                }
            } else {
                withItem(label, onNewLabel);
            }
        }

        // TODO expensive, could do with a little dynamic programming here...
        lastLabels = new List<DefaultLabelController>(labels);
    }
    
    private delegate void UseLabelAndItem(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item);

    private void withItem(DefaultLabelController labelController, UseLabelAndItem action) {
        
        var shopItemController = labelController.targetObject.gameObject.GetComponent<ShopItemController>();
        if (shopItemController == null) {
//            NoBrain.LogFine("Not containing ShopItemController " + labelController.label.Text);
            return;
        }
        
        var encounterTrackable = shopItemController.item.GetComponent<EncounterTrackable>();
        if (encounterTrackable == null) {
//            NoBrain.LogFine("Not containing EncounterTrackable " + labelController.label.Text);
            return;
        }

        var item = shopItemController.item;
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
    
    private void onNewLabel(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        if (ITEM_BLACKLIST.Contains(item.PickupObjectId)) {
            return;
        }
        
        // add padding fix behaviour that only updates position and not the padding
        labelController.gameObject.AddComponent<PositionUpdater>();
        
        // Text change
        var newText = labelController.label.Text;
        newText += SEPARATOR_TEXT;
        newText += getTextForPage(labelController, encounter, item);
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
    
    private void onUpdateLabel(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        
        NoBrainJsonItem noBrainJsonItem;
        var success = NoBrain.jsonItemDict.TryGetValue(item.PickupObjectId, out noBrainJsonItem);
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
            newText += getTextForPage(labelController, encounter, item);
        }
        labelController.label.Text = newText;
//        NoBrain.LogFine("NoBrainBehaviour updating text to: " + newText);
        
        // Layouting
        labelController.label.AutoHeight = true;
        labelController.label.Invalidate();
        labelController.panel.FitToContents();
        labelController.panel.Invalidate();
        labelController.Trigger();
    }

    private string getTextForPage(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        string pageDescription;
        string text = "";

        var itemDictSuccess = NoBrain.jsonItemDict.TryGetValue(item.PickupObjectId, out var noBrainJsonItem);
        
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
            var synergySuccess = NoBrain.synergyDict.TryGetValue(item.PickupObjectId, out var synergyList);
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
               + PAGE_LENGTH + ") Press " + reloadSpriteTag + "[/color]";
    }
    
}