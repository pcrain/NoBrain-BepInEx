using UnityEngine;

public abstract class AbstractNBInteractableBehaviour : MonoBehaviour {

    private const float UPDATE_ACTIONS_EVERY_X_SECONDS = 2;
    
    private PlayerController primaryPlayer;
    private IPlayerInteractable lastInteractable;

    private float elapsedTimeSinceLastCheck;
    private GungeonActions gungeonActions;
    private string reloadSpriteTag;
    
    private void Update() {
        if (GameManager.Instance == null || GameManager.Instance.PrimaryPlayer == null
            || GameUIRoot.Instance == null) {
            return;
        }
        if (primaryPlayer != GameManager.Instance.PrimaryPlayer) {
            primaryPlayer = GameManager.Instance.PrimaryPlayer;
            NoBrain.LogFine("Setting new primaryPlayer");
        }

        elapsedTimeSinceLastCheck += BraveTime.DeltaTime;
        if (elapsedTimeSinceLastCheck > UPDATE_ACTIONS_EVERY_X_SECONDS) {
            var braveInput = BraveInput.GetInstanceForPlayer(primaryPlayer.PlayerIDX);
            if (gungeonActions != braveInput.ActiveActions) {
                gungeonActions = braveInput.ActiveActions;
                reloadSpriteTag = gungeonActions.ReloadAction.getUISpriteString(braveInput.IsKeyboardAndMouse());
                NoBrain.LogFine("Setting new gungeonActions");
            }
            elapsedTimeSinceLastCheck = 0;
        }
        
        var curInteractable = primaryPlayer.GetLastInteractable();
        if (lastInteractable == curInteractable) {
            if (gungeonActions?.ReloadAction.WasPressed ?? false) {
                onReloadPressed(lastInteractable);
            }
            return;
        }
        onCleanupInteractable(lastInteractable);
        onNewInteractable(curInteractable);
        lastInteractable = curInteractable;
        NoBrain.LogFine("New Interactable: " + lastInteractable.getSimpleTypeName());
    }

    protected abstract void onNewInteractable(IPlayerInteractable interactable);
    protected abstract void onCleanupInteractable(IPlayerInteractable interactable);
    protected abstract void onReloadPressed(IPlayerInteractable interactable);

    protected PlayerController getPrimaryPlayer() {
        return primaryPlayer;
    }
    
    protected string getReloadSpriteTag() {
        return reloadSpriteTag;
    }

    protected IPlayerInteractable getLastInteractable() {
        return lastInteractable;
    }
    
}