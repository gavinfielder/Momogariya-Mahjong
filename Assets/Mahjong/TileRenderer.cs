using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mahjong
{
    class TileRenderer : MonoBehaviour
    {
        #pragma warning disable CS0649
        public SpriteRenderer BaseSpriteRenderer;
        public SpriteRenderer FaceSpriteRenderer;
        #pragma warning restore CS0649

        public enum TileOrientation : byte
        {
            Player = 0,
            Bottom = 1,
            Right = 2,
            Top = 3,
            Left = 4
        }
        private TileOrientation _orientation = TileOrientation.Bottom;
        public TileOrientation Orientation {
            get
            {
                return _orientation;
            }
            set
            {
                _orientation = value;
                switch (_orientation)
                {
                    case TileOrientation.Player:
                        gameObject.transform.SetPositionAndRotation(
                            gameObject.transform.position,
                            Quaternion.AngleAxis(0f, Vector3.forward));
                        break;
                    case TileOrientation.Bottom:
                        gameObject.transform.SetPositionAndRotation(
                            gameObject.transform.position,
                            Quaternion.AngleAxis(0f, Vector3.forward));
                        break;
                    case TileOrientation.Right:
                        gameObject.transform.SetPositionAndRotation(
                            gameObject.transform.position,
                            Quaternion.AngleAxis(90f, Vector3.forward));
                        break;
                    case TileOrientation.Top:
                        gameObject.transform.SetPositionAndRotation(
                            gameObject.transform.position,
                            Quaternion.AngleAxis(180f, Vector3.forward));
                        break;
                    case TileOrientation.Left:
                        gameObject.transform.SetPositionAndRotation(
                            gameObject.transform.position,
                            Quaternion.AngleAxis(270f, Vector3.forward));
                        break;
                }
            }
        }
        
        private TileVisibility _visibility = TileVisibility.FaceDown;
        public TileVisibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                switch (_visibility)
                {
                    case TileVisibility.FaceDown:
                        Face = TileID.Hidden;
                        break;
                    case TileVisibility.FaceUp:
                        Face = tile.Query();
                        break;
                    case TileVisibility.InHand:
                        Face = TileID.HiddenHand;
                        break;
                }
            }
        }

        private Tile tile;

        //Initialization
        private void Start()
        {
            tile = gameObject.GetComponent<Tile>();
        }

        //Gets or sets the tile that this renderer shows
        private TileID face;
        public TileID Face
        {
            get
            {
                return face.Copy();
            }
            private set
            {
                if (value == TileID.Invalid) return;
                if (value == TileID.Hidden)
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Back-Gold");
                    FaceSpriteRenderer.sprite = null;
                    face = value;
                }
                else if (value == TileID.HiddenHand)
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Top");
                    FaceSpriteRenderer.sprite = null;
                    face = value;
                }
                else
                {
                    BaseSpriteRenderer.sprite = Resources.Load<Sprite>("Tiles/TempDev/Front");
                    FaceSpriteRenderer.sprite = Resources.Load<Sprite>(GetAssetPath(value));
                    face = value.Copy();
                }
            }

        }

        //Sets the sorting orders in the sprite rendering layer
        public int Layer
        {
            set
            {
                BaseSpriteRenderer.sortingOrder = value * 2;
                FaceSpriteRenderer.sortingOrder = value * 2 + 1;
            }
        }
        
        //Returns the asset path of the face
        private string GetAssetPath(TileID tile)
        {
            string path = "Tiles/TempDev/";
            if (tile.Suit == TileID.Suits.Man)
            {
                path += "Man";
                path += tile.Number;
            }
            else if (tile.Suit == TileID.Suits.Pin)
            {
                path += "Pin";
                path += tile.Number;
            }
            else if (tile.Suit == TileID.Suits.Sou)
            {
                path += "Sou";
                path += tile.Number;
            }
            else if (tile.Suit == TileID.Suits.Kaze)
            {
                if (tile.Number == TileID.TON)
                    path += "Ton";
                else if (tile.Number == TileID.NAN)
                    path += "Nan";
                else if(tile.Number == TileID.SHAA)
                    path += "Shaa";
                else if(tile.Number == TileID.PEI)
                    path += "Pei";
            }
            else if (tile.Suit == TileID.Suits.Sangen)
            {
                if (tile.Number == TileID.CHUN)
                    path += "Chun";
                else if (tile.Number == TileID.HAKU)
                    path += "Haku";
                else if (tile.Number == TileID.HATSU)
                    path += "Hatsu";
            }
            return path;
        }
    }
}
