using System;
using System.Collections.Generic;
using InControl;

public static class Extensions {
    
    private static readonly ToStringFunction<int> ITEM_ID_TO_DISPLAY_NAME = id =>
        PickupObjectDatabase.Instance.InternalGetById(id).getDisplayName();
    
    public delegate T Getter<out T>();
    public delegate void Setter<in T>(T t);
    public delegate string ToStringFunction<in T>(T t);

    public static ConsoleCommandGroup AddUnitFlag(this ConsoleCommandGroup group,
        string name, Getter<bool> getFunc, Setter<bool> setAction) {
        group.AddUnit(name, delegate(string[] args) {
            var noArguments = args.Length == 0;
            if (noArguments) {
                var stateString = getFunc()
                    ? "<color=#00ff00ff>on</color>"
                    : "<color=#ff0000ff>off</color>";
                NoBrain.Log($"{name} is turned {stateString}!");
            } else {
                var parseSuccess = !bool.TryParse(args[0], out var parsedValue);
                if (parseSuccess) {
                    NoBrain.Log($"This argument only supports \"true\" or \"false\" as values. (Given: {args[0]})");
                } else {
                    setAction(parsedValue);
                    var stateString = parsedValue
                        ? "<color=#00ff00ff>enabled</color>"
                        : "<color=#ff0000ff>disabled</color>";
                    NoBrain.Log($"{name} has been {stateString}!");
                }
            }
        });
        return group;
    }

    public static string getName(this AdvancedSynergyEntry e) {
        return StringTableManager.GetSynergyString(e.NameKey);
    }

    public static string getUISpriteString(this PlayerAction playerAction, bool keyboardAndMouse) {
        if (keyboardAndMouse) {
            var str = "-";
            foreach (var binding in playerAction.Bindings) {
                if (binding.BindingSourceType == BindingSourceType.KeyBindingSource ||
                     binding.BindingSourceType == BindingSourceType.MouseBindingSource) {
                    str = binding.Name;
                    break;
                }
            }
            return str.Trim();
        }
        
        var bindings = playerAction.Bindings;
        if (bindings.Count > 0) {
            foreach (var t in bindings) {
                var deviceBindingSource = t as DeviceBindingSource;
                if (deviceBindingSource != null &&
                    deviceBindingSource.Control != InputControlType.None) {
                    return UIControllerButtonHelper.GetUnifiedControllerButtonTag(
                        deviceBindingSource.Control, BraveInput.PlayerOneCurrentSymbology);
                }
            }
        }
        return playerAction.Name;
    }
    
    public static string getUISpriteString(this PickupObject.ItemQuality q) {
        string icon;
        switch (q) {
            case PickupObject.ItemQuality.SPECIAL:
                icon = "resourceful_rat_icon_001";
                break;
            case PickupObject.ItemQuality.COMMON:
                icon = "poopsack_001";
                break;
            case PickupObject.ItemQuality.D:
                icon = "master_token_icon_001";
                break;
            case PickupObject.ItemQuality.C:
                icon = "master_token_icon_002";
                break;
            case PickupObject.ItemQuality.B:
                icon = "master_token_icon_003";
                break;
            case PickupObject.ItemQuality.A:
                icon = "master_token_icon_004";
                break;
            case PickupObject.ItemQuality.S:
                icon = "master_token_icon_005";
                break;
            default:
                icon = "ps4_cross";
                break;
        }

        return "[sprite \"" + icon + "\"]";
    }

    public static string join<T>(this IEnumerable<T> list, ToStringFunction<T> toStringFunction,
        string separator = ", ") {
        var s = "";
        var first = true;
        foreach (var t in list) {
            if (first) {
                first = false;
            } else {
                s += separator;
            }
            s += toStringFunction(t);
        }
        return s;
    }
    
    public static int deepHashCode<T>(this IEnumerable<T> list) {
        int res = 0x2D2816FE;
        foreach(var item in list)
        {
            res = res * 31 + (item == null ? 0 : item.GetHashCode());
        }
        return res;
    }

    public static string getMandatoryString(this AdvancedSynergyEntry e) {
        var guns = join(e.MandatoryGunIDs, ITEM_ID_TO_DISPLAY_NAME);
        var items = join(e.MandatoryItemIDs, ITEM_ID_TO_DISPLAY_NAME);
        if (guns.Length > 0 && items.Length > 0) {
            guns += ", ";
        }
        return guns + items;
    }

    public static string getOptionalString(this AdvancedSynergyEntry e) {
        var guns = join(e.OptionalGunIDs, ITEM_ID_TO_DISPLAY_NAME);
        var items = join(e.OptionalItemIDs, ITEM_ID_TO_DISPLAY_NAME);
        if (guns.Length > 0 && items.Length > 0) {
            guns += ", ";
        }
        return guns + items;
    }

    public static string getDisplayName(this PickupObject item) {
        var component = item.GetComponent<EncounterTrackable>();
        return component == null ? item.DisplayName : component.journalData.GetPrimaryDisplayName();
    }
    
    public static string getSimpleTypeName(this Object o) {
        return o == null ? "Null" : o.GetType().Name;
    }
}