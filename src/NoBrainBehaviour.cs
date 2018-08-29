using System.Collections.Generic;
using System.Collections.ObjectModel;
using InControl;
using UnityEngine;

public class NoBrainBehaviour : MonoBehaviour {

    private static readonly int PAGE_AMMO = 0;
    private static readonly int PAGE_STATS = 1;
    private static readonly int PAGE_LENGTH = 2;

    private static readonly string ESCAPE_ROPE_SPRITE = "[sprite \"escape_rope_text_icon_001\"]";
    
    private int lastHashcode = 0;

    private List<DefaultLabelController> lastLabels = new List<DefaultLabelController>();
    private GungeonActions gungeonActions;

    private int currentPage = PAGE_AMMO;
    private int nextPage = PAGE_AMMO;

    private string reloadSpriteTag;
    
    private void Start() {
        NoBrain.LogFine("Start of NoBrainBehaviour");
    }

    private void Update() {
        if (gungeonActions == null && GameManager.Instance != null && GameManager.Instance.PrimaryPlayer != null) {
            var braveInput = BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX);
            gungeonActions = braveInput.ActiveActions;
            reloadSpriteTag = getUISpriteStringForAction(braveInput.IsKeyboardAndMouse(), gungeonActions.ReloadAction);
        }
        if (gungeonActions?.ReloadAction.WasPressed ?? false) {
            nextPage = (currentPage + 1) % PAGE_LENGTH;
        }
        
        if (GameUIRoot.Instance == null) {
            return;
        }
        var labels = GameUIRoot.Instance.extantBasicLabels;
        var hashCode = labels.GetHashCode();
        if (hashCode == lastHashcode) {
            return;
        }

        bool updateLabels = nextPage != currentPage;
        if (updateLabels) {
            currentPage = nextPage;
        }
        foreach (var label in labels) {
            if (lastLabels.Contains(label)) {
                if (updateLabels) {
                    withItem(label, onUpdateLabel);
                }
                continue;
            }
            withItem(label, onNewLabel);
        }

        // TODO expensive, could do with a little dynamic programming here...
        lastLabels = new List<DefaultLabelController>(labels);
    }

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
        
        action.Invoke(labelController, encounterTrackable, shopItemController.item);
    }
    
    private delegate void UseLabelAndItem(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item);
    
    private void onNewLabel(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        
        // Text change
        var newText = labelController.label.Text;
        newText += " " + ESCAPE_ROPE_SPRITE;
        newText += getTextForPage(labelController, encounter, item);
        labelController.label.Text = newText;
//        NoBrain.LogFine("NoBrainBehaviour setting text to: " + newText);
        
        // Layouting
        labelController.label.TextScale = 2f;
        labelController.label.AutoHeight = true;
        labelController.label.WordWrap = true;
        labelController.label.Width = Screen.width / 3f;
        labelController.panel.FitToContents();
    }
    
    private void onUpdateLabel(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        
        // Text change
        var newText = labelController.label.Text;
        var indexOf = newText.IndexOf(ESCAPE_ROPE_SPRITE) + ESCAPE_ROPE_SPRITE.Length;
        if (indexOf == -1) {
            newText = "ERROR FINDING ESCAPE ROPE";
        } else {
            newText = newText.Substring(0, indexOf);
            newText += getTextForPage(labelController, encounter, item);
        }
        labelController.label.Text = newText;
//        NoBrain.LogFine("NoBrainBehaviour updating text to: " + newText);
        
        // Layouting
        labelController.label.Invalidate();
        labelController.panel.FitToContents();
        labelController.panel.Invalidate();
        labelController.Trigger();
    }

    private string getTextForPage(DefaultLabelController labelController, EncounterTrackable encounter,
        PickupObject item) {
        string text;
        
        if (currentPage == PAGE_AMMO) {
            var passiveActiveString = item is PassiveItem ? "Passive" : "Active";
            text = "[color #7d7d7d]"
                   + " " + item.PickupObjectId
                   + " " + item.quality
                   + " " + passiveActiveString
                   + "[/color]"
                   + "\n[color #a0a0a0]" + encounter.journalData.GetAmmonomiconFullEntry(false, false)
                   + "[/color]";
        } else if (currentPage == PAGE_STATS) {
            text = "\n[color #ff69b4]wow[/color]\n" +
                   "[sprite \"master_token_icon_001\"][sprite \"master_token_icon_002\"][sprite \"master_token_icon_003\"]" +
                   "[sprite \"master_token_icon_004\"][sprite \"master_token_icon_005\"]";
        } else {
            text = "INVALID PAGE";
        }

        return text + "\n[color blue](" + (currentPage+1) + "/" + PAGE_LENGTH + ") Press " + reloadSpriteTag +
                " for next page.[/color]";
    }


    private static string getUISpriteStringForAction(bool keyboardAndMouse,
        PlayerAction playerAction) {
        if (keyboardAndMouse) {
            var str = "-";
            for (int index = 0; index < playerAction.Bindings.Count; ++index) {
                BindingSource binding = playerAction.Bindings[index];
                if ((binding.BindingSourceType == BindingSourceType.KeyBindingSource ||
                     binding.BindingSourceType == BindingSourceType.MouseBindingSource)) {
                    str = binding.Name;
                    break;
                }
            }
            return str.Trim();
        }
        
        var bindings = playerAction.Bindings;
        if (bindings.Count > 0) {
            for (int i = 0; i < bindings.Count; i++) {
                var deviceBindingSource = bindings[i] as DeviceBindingSource;
                if (deviceBindingSource != null &&
                    deviceBindingSource.Control != InputControlType.None) {
                    return UIControllerButtonHelper.GetUnifiedControllerButtonTag(
                        deviceBindingSource.Control, BraveInput.PlayerOneCurrentSymbology);
                }
            }
        }
        return playerAction.Name;
    }
}