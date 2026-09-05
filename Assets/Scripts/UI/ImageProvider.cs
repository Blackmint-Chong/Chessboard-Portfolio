using Chess;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChessUI
{
    public class ImageProvider : MonoBehaviour
    {
        [Header("White Piece Sprites")]
        [SerializeField] private Sprite whitePawn;
        [SerializeField] private Sprite whiteKnight;
        [SerializeField] private Sprite whiteBishop;
        [SerializeField] private Sprite whiteRook;
        [SerializeField] private Sprite whiteQueen;
        [SerializeField] private Sprite whiteKing;

        [Header("Black Piece Sprites")]
        [SerializeField] private Sprite blackPawn;
        [SerializeField] private Sprite blackKnight;
        [SerializeField] private Sprite blackBishop;
        [SerializeField] private Sprite blackRook;
        [SerializeField] private Sprite blackQueen;
        [SerializeField] private Sprite blackKing;

        private Dictionary<(PieceType, Team), Sprite> spriteLookup;

        void Awake()
        {
            spriteLookup = new Dictionary<(PieceType, Team), Sprite>
            {
                {(PieceType.Pawn, Team.White), whitePawn},
                {(PieceType.Knight, Team.White), whiteKnight},
                {(PieceType.Bishop, Team.White), whiteBishop},
                {(PieceType.Rook, Team.White), whiteRook},
                {(PieceType.Queen, Team.White), whiteQueen},
                {(PieceType.King, Team.White), whiteKing},
                {(PieceType.Pawn, Team.Black), blackPawn},
                {(PieceType.Knight, Team.Black), blackKnight},
                {(PieceType.Bishop, Team.Black), blackBishop},
                {(PieceType.Rook, Team.Black), blackRook},
                {(PieceType.Queen, Team.Black), blackQueen},
                {(PieceType.King, Team.Black), blackKing},
            };
        }

        public Sprite GetSprite(PieceType pieceType, Team team)
        {
            if (spriteLookup == null)
            {
                Awake();
            }

            if (spriteLookup.TryGetValue((pieceType, team), out Sprite sprite))
            {
                return sprite;
            }

            throw new NullReferenceException($"Image refrence missing for {team} {pieceType}");
        }
    }
}

