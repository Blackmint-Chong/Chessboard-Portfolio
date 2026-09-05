using Chess;
using UnityEngine;

namespace ChessUI
{
    public class BoardView : MonoBehaviour
    {
        [Header("Prefabs")]
        public SquareView squarePrefab;

        [Header("Game Controller")]
        public GameController controller;

        [Header("Board Container")]
        public Transform boardRoot;

        [Header("Image Provider")]
        public ImageProvider imageProvider;

        private SquareView[,] squares = new SquareView[8, 8];

        Position? selectedSquare = null;

        void Awake()
        {
            CreateBoard();
        }

        void CreateBoard()
        {
            for (int rank = 7; rank >= 0; rank--)
            {
                for (int file = 0; file < 8; file++)
                {
                    var square = Instantiate(squarePrefab, boardRoot);

                    Position pos = new(file, rank);
                    square.Init(pos, this);

                    squares[file, rank] = square;
                }
            }
        }

        public void OnSquareClicked(Position pos)
        {
            if (controller.IsPromotionSelectionOpen())
            {
                return;
            }

            if (selectedSquare == null)
            {
                selectedSquare = pos;
                GetSquare(pos).SetSelected(true);
                return;
            }

            controller.TryApplyMove(selectedSquare.Value, pos);
        }

        public void Draw(Board board, Move? lastMove = null)
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    squares[file, rank].SetHighlight(SquareHighlightState.Normal);
                    squares[file, rank].SetPiece(null);
                }
            }

            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    Position pos = new Position(file, rank);

                    if (board.TryGetPiece(pos, out Piece piece))
                    {
                        Sprite sprite = imageProvider.GetSprite(piece.Type, piece.Team);
                        squares[file, rank].SetPiece(sprite);
                    }
                }
            }

            if (lastMove.HasValue)
            {
                GetSquare(lastMove.Value.From).SetHighlight(SquareHighlightState.LastMove);
                GetSquare(lastMove.Value.To).SetHighlight(SquareHighlightState.LastMove);
            }

            if (selectedSquare != null)
            {
                GetSquare(selectedSquare.Value).SetSelected(true);
            }
        }

        public SquareView GetSquare(Position pos)
        {
            return squares[pos.File, pos.Rank];
        }

        public RectTransform GetSquareRect(Position pos)
        {
            return GetSquare(pos).GetComponent<RectTransform>();
        }

        public void ShowCheckmateKings(Position winningKing, Position losingKing)
        {
            GetSquare(winningKing).SetWinningKing();
            GetSquare(losingKing).SetLosingKing();
        }

        public void ShowDrawKings(Position whiteKing, Position blackKing)
        {
            GetSquare(whiteKing).SetDrawKing();
            GetSquare(blackKing).SetDrawKing();
        }

        public void UnselectAllSquare()
        {
            if (selectedSquare != null)
            {
                GetSquare(selectedSquare.Value).SetSelected(false);
                selectedSquare = null;
            }
        }
    }
}
