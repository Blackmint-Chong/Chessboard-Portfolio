using Chess;
using TMPro;
using UnityEngine;

namespace ChessUI
{
    public class GameStatusView : MonoBehaviour
    {
        private static readonly Color NormalColor = new Color32(245, 242, 232, 255);
        private static readonly Color CheckColor = new Color32(239, 186, 73, 255);
        private static readonly Color CheckmateColor = new Color32(234, 104, 91, 255);
        private static readonly Color DrawColor = new Color32(173, 206, 255, 255);

        [SerializeField] private TMP_Text statusText;

        void Awake()
        {
            if (statusText == null)
            {
                statusText = GetComponentInChildren<TMP_Text>();
            }
        }

        public void Render(Game game)
        {
            if (game == null || statusText == null)
            {
                return;
            }

            var board = game.CurrentBoard;

            if (game.IsCheckMate())
            {
                Team winner = board.SideToMove == Team.White ? Team.Black : Team.White;
                SetStatus($"{FormatTeam(winner)} Wins\nCheckmate", CheckmateColor);
                return;
            }

            DrawReason drawReason = game.GetDrawReason();
            if (drawReason != DrawReason.None)
            {
                SetStatus($"Draw\n{FormatDrawReason(drawReason)}", DrawColor);
                return;
            }

            string sideToMove = $"{FormatTeam(board.SideToMove)} to Move";
            if (board.IsInCheck(board.SideToMove))
            {
                SetStatus($"{sideToMove}\nCheck!", CheckColor);
                return;
            }

            SetStatus(sideToMove, NormalColor);
        }

        private void SetStatus(string message, Color color)
        {
            statusText.text = message;
            statusText.color = color;
        }

        private static string FormatTeam(Team team)
            => team == Team.White ? "White" : "Black";

        private static string FormatDrawReason(DrawReason reason)
            => reason switch
            {
                DrawReason.Stalemate => "Stalemate",
                DrawReason.ThreefoldRepetition => "Threefold Repetition",
                DrawReason.FiftyMoveRule => "Fifty-move Rule",
                DrawReason.InsufficientMaterial => "Insufficient Material",
                _ => string.Empty
            };
    }
}
