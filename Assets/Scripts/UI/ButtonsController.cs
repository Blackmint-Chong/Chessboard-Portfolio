using UnityEngine;
using UnityEngine.UI;

namespace ChessUI
{
    public class ButtonsController : MonoBehaviour
    {
        [SerializeField] private GameObject undoButton;
        [SerializeField] private GameObject resetButton;

        private GameController GameController;
        public UndoUIController UndoUIController;
        public ResetUIController ResetUIController;

        public void Init(GameController controller)
        {
            GameController = controller;

            if (undoButton != null)
            {
                undoButton.SetActive(true);
            }

            if (resetButton != null)
            {
                resetButton.SetActive(true);
            }
        }

        public void OnUndoButtonClicked()
        {
            if (GameController.CanUndo())
            {
                UndoUIController.Open();
            }
        }

        public void OnResetButtonClicked()
        {
            ResetUIController.Open();
        }
    }
}
