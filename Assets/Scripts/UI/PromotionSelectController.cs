using Chess;
using UnityEngine;
using UnityEngine.UI;

namespace ChessUI
{
    public class PromotionSelectController : MonoBehaviour
    {
        [SerializeField] private GameObject promotionSelectPanel;
        [SerializeField] private ImageProvider imageProvider;
        [SerializeField] private Image queenImage;
        [SerializeField] private Image rookImage;
        [SerializeField] private Image bishopImage;
        [SerializeField] private Image knightImage;

        private GameController controller;
        private Position pendingFrom;
        private Position pendingTo;
        private bool hasPendingPromotion;

        public bool IsOpen => promotionSelectPanel != null && promotionSelectPanel.activeSelf;

        void Awake()
        {
            Close();
        }

        public void Init(GameController gameController)
        {
            controller = gameController;
            Close();
        }

        public void Open(Position from, Position to, Team team)
        {
            pendingFrom = from;
            pendingTo = to;
            hasPendingPromotion = true;
            RefreshButtonImages(team);
            PositionPanel(to, team);

            if (promotionSelectPanel != null)
            {
                promotionSelectPanel.SetActive(true);
            }
        }

        public void Close()
        {
            hasPendingPromotion = false;

            if (promotionSelectPanel != null)
            {
                promotionSelectPanel.SetActive(false);
            }
        }

        public void SelectQueen()
        {
            SelectPromotion(PieceType.Queen);
        }

        public void SelectRook()
        {
            SelectPromotion(PieceType.Rook);
        }

        public void SelectBishop()
        {
            SelectPromotion(PieceType.Bishop);
        }

        public void SelectKnight()
        {
            SelectPromotion(PieceType.Knight);
        }

        public void SelectPromotion(PieceType pieceType)
        {
            if (!hasPendingPromotion || controller == null)
            {
                return;
            }

            Close();
            controller.ApplyPromotion(pendingFrom, pendingTo, pieceType);
        }

        private void RefreshButtonImages(Team team)
        {
            if (imageProvider == null)
            {
                Debug.LogWarning("ImageProvider is not assigned.");
                return;
            }

            if (queenImage != null)
                queenImage.sprite = imageProvider.GetSprite(PieceType.Queen, team);

            if (rookImage != null)
                rookImage.sprite = imageProvider.GetSprite(PieceType.Rook, team);

            if (bishopImage != null)
                bishopImage.sprite = imageProvider.GetSprite(PieceType.Bishop, team);

            if (knightImage != null)
                knightImage.sprite = imageProvider.GetSprite(PieceType.Knight, team);
        }

        private void PositionPanel(Position promotionPosition, Team team)
        {
            if (promotionSelectPanel == null || controller == null || controller.boardView == null)
            {
                return;
            }

            RectTransform panelRect = promotionSelectPanel.transform as RectTransform;
            RectTransform targetSquareRect = controller.boardView.GetSquareRect(promotionPosition);
            if (panelRect == null || targetSquareRect == null)
            {
                return;
            }

            Vector3[] squareCorners = new Vector3[4];
            targetSquareRect.GetWorldCorners(squareCorners);

            bool isWhitePromotion = team == Team.White;
            panelRect.pivot = isWhitePromotion ? new Vector2(0f, 1f) : new Vector2(0f, 0f);
            panelRect.position = isWhitePromotion ? squareCorners[1] : squareCorners[0];
        }
    }
}
