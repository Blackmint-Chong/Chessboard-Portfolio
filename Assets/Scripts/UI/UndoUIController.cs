using UnityEngine;

namespace ChessUI
{
    public class UndoUIController : MonoBehaviour
    {
        [SerializeField] private GameObject undoConfirmScreen;
        private GameController controller;

        void Awake()
        {
            Close();
        }

        public void Init(GameController gameController)
        {
            controller = gameController;
            Close();
        }

        public void Open()
        {
            if (undoConfirmScreen != null)
            {
                undoConfirmScreen.SetActive(true);
            }
        }

        public void Close()
        {
            if (undoConfirmScreen != null)
            {
                undoConfirmScreen.SetActive(false);
            }
        }

        public void OnUndoButtonClicked()
        {
            controller.Undo();
            Close();
        }

        public void OnCancelButtonClicked()
            => Close();
    }
}
