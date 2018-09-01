using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class NoBrain : ETGModule {

#if DEBUG
    public static bool FINE_LOGGING = true;
#else
    public static bool FINE_LOGGING = false;
#endif
    
    public static bool SHOW_ITEM_IDS = true;
    public static bool SHOW_LABELS = true;
    
    public static Dictionary<int, NoBrainJsonItem> jsonItemDict =
        new Dictionary<int, NoBrainJsonItem>();
    
    public static MultiValueDictionary<int, AdvancedSynergyEntry> synergyDict =
        new MultiValueDictionary<int, AdvancedSynergyEntry>();
    
    public override void Init() {
        LogFine("ModInit");

        // LOAD JSON ITEM DICT
        List<NoBrainJsonItem> items = JsonConvert.DeserializeObject<List<NoBrainJsonItem>>(
            NoBrainDB.ITEM_JSON);
        foreach (var noBrainJsonItem in items) {
            jsonItemDict[noBrainJsonItem.id] = noBrainJsonItem;
        }
        
        ETGModConsole.Commands.AddGroup("nobrain", delegate {
            Log("<size=100>NoBrain v1 by markusmo3!</size>");
            Log("Use \"nobrain help\" for help!");
        });
        ETGModConsole.Commands.GetGroup("nobrain")
            .AddUnit("help", delegate {
                Log("<size=100>NoBrain v1 by markusmo3!</size>");
                Log("");
                Log("NoBrain Command Reference:");
                Log("(arg) = Optional argument, [arg] = Mandatory argument");
                Log("");
                Log("nobrain help - Displays this help");
                Log("nobrain showlabels (true/false) - show the extended labels");
                Log("nobrain finelogging (true/false) - logs more, only needed by the dev");
                Log("nobrain showitemids (true/false) - show the item id in the label");
            })
            .AddUnitFlag("showlabels", () => SHOW_LABELS, b => SHOW_LABELS = b)
            .AddUnitFlag("finelogging", () => FINE_LOGGING, b => FINE_LOGGING = b)
            .AddUnitFlag("showitemids", () => SHOW_ITEM_IDS, b => SHOW_ITEM_IDS = b)
            ;
    }

    public override void Start() {
        LogFine("ModStart");
        ETGModMainBehaviour.Instance.gameObject.AddComponent<NoBrainBehaviour>();
        
        // FILL SYNERGY DICT
        AdvancedSynergyEntry[] synergies = GameManager.Instance.SynergyManager.synergies;
        foreach (var advancedSynergyEntry in synergies) {
            advancedSynergyEntry.MandatoryGunIDs.ForEach(i => synergyDict.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.MandatoryItemIDs.ForEach(i => synergyDict.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.OptionalGunIDs.ForEach(i => synergyDict.Add(i, advancedSynergyEntry));
            advancedSynergyEntry.OptionalItemIDs.ForEach(i => synergyDict.Add(i, advancedSynergyEntry));
        }
    }

    public override void Exit() {
        LogFine("ModExit");
    }

    public static void Log(string message) {
//        var text = "@NoBrain[" + DateTime.Today.ToShortTimeString() + "] " + message;
        var text = "@NoBrain " + message;
        if (ETGModConsole.Instance.GUI != null && ETGModConsole.Instance.GUI[0] != null) {
            ETGModConsole.Log(text);
        } else {
            Debug.Log(text);
        }
    }
    
    public static void LogFine(string message) {
        if (FINE_LOGGING) {
            Log("FINE " + message);
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