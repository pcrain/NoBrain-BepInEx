using System;
using UnityEngine;

public class NoBrain : ETGModule {

    public static bool FINE_LOGGING = true;
    
    public override void Init() {
        Log("ModInit");

//        ETGMod.Objects.AddHook<ETGModMainBehaviour>(d => HookNoBrainBehaviour(d));
        
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
                Log("nobrain finelogging (true/false) - ");
            })
            .AddUnit("finelogging", delegate(string[] args) {
                bool flag2 = args.Length == 0;
                if (flag2) {
                    string arg = FINE_LOGGING
                        ? "<color=#00ff00ff>on</color>"
                        : "<color=#ff0000ff>off</color>";
                    Log(string.Format("finelogging is turned {0}!", arg));
                } else {
                    bool flag4;
                    bool flag3 = !bool.TryParse(args[0], out flag4);
                    if (flag3) {
                        Log(string.Format(
                                "This argument only supports \"true\" or \"false\" as values. (Given: {0})",
                                args[0]));
                    } else {
                        FINE_LOGGING = flag4;
                        Log(string.Format("finelogging has been {0}!",
                                flag4
                                    ? "<color=#00ff00ff>enabled</color>"
                                    : "<color=#ff0000ff>disabled</color>"));
                    }
                }
            });
    }

    private void HookNoBrainBehaviour(Component component) {
//        var etgModMainBehaviour = component as ETGModMainBehaviour;
//        if (etgModMainBehaviour != null) {
//            etgModMainBehaviour.gameObject
//        } 
    }

    public override void Start() {
        Log("ModStart");
        ETGModMainBehaviour.Instance.gameObject.AddComponent<NoBrainBehaviour>();
    }

    public override void Exit() {
        Log("ModExit");
    }

    public static void Log(string message) {
        var text = "@NoBrain[" + DateTime.Today.ToShortTimeString() + "] " + message;
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