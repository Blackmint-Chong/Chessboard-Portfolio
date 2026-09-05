using Chess;
using UnityEngine;
using UnityEngine.UI;

namespace ChessUI
{
    public enum SquareHighlightState
    {
        Normal,
        Selected,
        LastMove,
        WinningKing,
        LosingKing,
        DrawKing
    }

    public class SquareView : MonoBehaviour
    {
        public Image background;
        public Image pieceImage;

        public Color lightColor;
        public Color darkColor;
        public Color selectedColor;
        public Color lastMoveColor;
        public Color winningKingColor;
        public Color losingKingColor;
        public Color drawKingColor;

        public Position position;

        public BoardView boardView;

        void Awake()
        {
            CacheReferences();
        }

        void Reset()
        {
            CacheReferences();
        }

        void CacheReferences()
        {
            if (background == null)
            {
                background = GetComponent<Image>();
            }

            if (pieceImage == null)
            {
                Transform pieceTransform = transform.Find("PieceImage");
                if (pieceTransform != null)
                {
                    pieceImage = pieceTransform.GetComponent<Image>();
                }
            }
        }

        public void Init(Position pos, BoardView bv)
        {
            CacheReferences();
            position = pos;
            boardView = bv;
            SetHighlight(SquareHighlightState.Normal);
        }

        public void OnClick()
        {
            boardView.OnSquareClicked(position);
        }

        public void SetPiece(Sprite sprite)
        {
            if (pieceImage == null)
            {
                Debug.LogError($"PieceImage is not assigned on {name}.", this);
                return;
            }

            pieceImage.sprite = sprite;
            pieceImage.enabled = sprite != null;
        }

        public void SetSelected(bool isSelected)
        {
            SetHighlight(isSelected ? SquareHighlightState.Selected : SquareHighlightState.Normal);
        }

        public void SetWinningKing()
        {
            SetHighlight(SquareHighlightState.WinningKing);
        }

        public void SetLosingKing()
        {
            SetHighlight(SquareHighlightState.LosingKing);
        }

        public void SetDrawKing()
        {
            SetHighlight(SquareHighlightState.DrawKing);
        }

        public void SetHighlight(SquareHighlightState highlightState)
        {
            if (background == null)
            {
                Debug.LogError($"Background is not assigned on {name}.", this);
                return;
            }

            if (highlightState == SquareHighlightState.Selected)
            {
                background.color = selectedColor;
                return;
            }

            if (highlightState == SquareHighlightState.LastMove)
            {
                background.color = lastMoveColor;
                return;
            }

            if (highlightState == SquareHighlightState.WinningKing)
            {
                background.color = winningKingColor;
                return;
            }

            if (highlightState == SquareHighlightState.LosingKing)
            {
                background.color = losingKingColor;
                return;
            }

            if (highlightState == SquareHighlightState.DrawKing)
            {
                background.color = drawKingColor;
                return;
            }

            bool isDark = (position.File + position.Rank) % 2 == 1;
            background.color = isDark ? darkColor : lightColor;
        }
    }

}
