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
    public static bool SHOW_SHRINES = true;
    public static bool SHOW_CHEST_CONTENTS = false;

    public override void Init() {
        LogFine("ModInit");
        
        Patcher.doPatch();
        
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
                Log("nobrain clearbasiclabels - Clears all the labels");
                Log("nobrain showlabels (true/false) - show the extended labels");
                Log("nobrain showshrines (true/false) - replace the default shrine stone tablet description with a custom description");
                Log("nobrain showchestcontents (true/false) - displays the name of the items contained in a chest");
                Log("nobrain finelogging (true/false) - logs more, only needed by the dev");
                Log("nobrain showitemids (true/false) - show the item id in the label");
            })
            .AddUnitFlag("showlabels", () => SHOW_LABELS, b => SHOW_LABELS = b)
            .AddUnitFlag("showshrines", () => SHOW_SHRINES, b => SHOW_SHRINES = b)
            .AddUnitFlag("finelogging", () => FINE_LOGGING, b => FINE_LOGGING = b)
            .AddUnitFlag("showitemids", () => SHOW_ITEM_IDS, b => SHOW_ITEM_IDS = b)
            .AddUnitFlag("showchestcontents", () => SHOW_CHEST_CONTENTS, b => SHOW_CHEST_CONTENTS = b)
            .AddUnit("clearbasiclabels", sa => GameUIRoot.Instance.ClearAllDefaultLabels())
            ;
    }

    public override void Start() {
        LogFine("ModStart");
        NoBrainDB.Load();
        ETGModMainBehaviour.Instance.gameObject.AddComponent<NBInteractableBehaviour>();
//        ETGModMainBehaviour.Instance.gameObject.AddComponent<AbstractNBInteractableBehaviour>();
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
