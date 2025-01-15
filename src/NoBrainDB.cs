using System.Collections.Generic;
using Newtonsoft.Json;

public static class NoBrainDB {
    
    public static readonly Dictionary<int, NoBrainJsonItem> ITEMS =
        new Dictionary<int, NoBrainJsonItem>();
    
    public static readonly MultiValueDictionary<int, AdvancedSynergyEntry> SYNERGIES =
        new MultiValueDictionary<int, AdvancedSynergyEntry>();

    public static readonly MultiValueDictionary<string, string> SHRINE_KEY_MAPPING =
        new MultiValueDictionary<string, string>();
    
    public static readonly Dictionary<string, string> SHRINES =
        new Dictionary<string, string>();
    
    public static readonly List<int> ITEM_BLACKLIST = new List<int>(new [] {
        GlobalItemIds.Key, GlobalItemIds.RatKey,
        GlobalItemIds.SmallHeart, GlobalItemIds.FullHeart,
        GlobalItemIds.AmmoPickup, GlobalItemIds.SpreadAmmoPickup,
        120 // armor
    });

    static NoBrainDB() {        
        SHRINE_KEY_MAPPING.Add("Ammo Shrine", "#SHRINE_AMMO_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Angel Shrine", "#SHRINE_FALLEN_ANGEL_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Blank Shrine", "#SHRINE_BLANK_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Blood Shrine", "#SHRINE_BLOOD_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Blood Shrine", "#SHRINE_NEWBLOOD_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Challenge Shrine", "#SHRINE_CHALLENGE_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Cleansing Shrine", "#SHRINE_CLEANSE_DISPLAY_01");
        SHRINE_KEY_MAPPING.Add("Cleansing Shrine", "#SHRINE_CLEANSE_DISPLAY_02");
        SHRINE_KEY_MAPPING.Add("Cleansing Shrine", "#SHRINE_CLEANSE_DISPLAY_03");
        SHRINE_KEY_MAPPING.Add("Cleansing Shrine", "#SHRINE_CLEANSE_DISPLAY_04");
        SHRINE_KEY_MAPPING.Add("Cleansing Shrine", "#SHRINE_CLEANSE_DISPLAY_05");
        SHRINE_KEY_MAPPING.Add("Dice Shrine", "#SHRINE_DICE_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Familiar Shrine", "#SHRINE_COMPANION_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Glass Shrine", "#SHRINE_GLASS_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Hero Shrine", "#SHRINE_HERO_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Junk Shrine", "#SHRINE_JUNK_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Peace Shrine", "#SHRINE_HEALTH_DISPLAY");
        SHRINE_KEY_MAPPING.Add("Y.V Shrine", "#SHRINE_YV_DISPLAY");
    }
    
    public static void Load() {
        // LOAD JSON ITEM DICT
        List<NoBrainJsonItem> items = JsonConvert.DeserializeObject<List<NoBrainJsonItem>>(
            NoBrainJsonDB.ITEM_JSON);
        foreach (var noBrainJsonItem in items) {
            if (ITEM_BLACKLIST.Contains(noBrainJsonItem.id)) {
                continue;
            }
            ITEMS[noBrainJsonItem.id] = noBrainJsonItem;
        }
        
        // LOAD JSON SHRINES DICT
        List<NoBrainJsonShrine> shrines = JsonConvert.DeserializeObject<List<NoBrainJsonShrine>>(
            NoBrainJsonDB.SHRINE_JSON);
        foreach (var noBrainJsonShrine in shrines) {
            foreach (var key in SHRINE_KEY_MAPPING[noBrainJsonShrine.name]) {
                SHRINES[key] = noBrainJsonShrine.desc;
            }
        }
        
        // FILL SYNERGY DICT
        AdvancedSynergyEntry[] synergies = GameManager.Instance.SynergyManager.synergies;
        foreach (var advancedSynergyEntry in synergies) {
            if (advancedSynergyEntry.ActivationStatus == SynergyEntry.SynergyActivation.DEMO
                || advancedSynergyEntry.ActivationStatus ==
                SynergyEntry.SynergyActivation.INACTIVE) {
                continue; // skip Demo and inactive ones...
            }
            advancedSynergyEntry.MandatoryGunIDs.ForEach(i => SYNERGIES.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.MandatoryItemIDs.ForEach(i => SYNERGIES.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.OptionalGunIDs.ForEach(i => SYNERGIES.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.OptionalItemIDs.ForEach(i => SYNERGIES.Add(i, advancedSynergyEntry));
        }
    }
    
}

public class NoBrainJsonItem {
    public int id;
    public string desc;
    public string stats;

    public override string ToString() {
        return "NoBrainJsonItem{" + id + ", " + desc + ", " + stats + "}";
    }
}

public class NoBrainJsonShrine {
    public string name;
    public string desc;

    public override string ToString() {
        return "NoBrainJsonShrine{" + name + ", " + desc + ", " + desc + "}";
    }
}