using UnityEngine;

namespace ChessUI
{
    public class ResetUIController : MonoBehaviour
    {
        [SerializeField] private GameObject resetConfirmScreen;
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
            if (resetConfirmScreen != null)
            {
                resetConfirmScreen.SetActive(true);
            }
        }

        public void Close()
        {
            if (resetConfirmScreen != null)
            {
                resetConfirmScreen.SetActive(false);
            }
        }

        public void OnResetButtonClicked()
        {
            controller.Reset();
            Close();
        }

        public void OnCancelButtonClicked()
            => Close();
    }
}
