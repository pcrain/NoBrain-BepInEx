using UnityEngine;

public class PositionUpdater : MonoBehaviour {

    private DefaultLabelController labelController;
    
    private void Start() {
        labelController = gameObject.GetComponent<DefaultLabelController>();
    }

    private void LateUpdate() {
        labelController.UpdatePosition();
    }
}