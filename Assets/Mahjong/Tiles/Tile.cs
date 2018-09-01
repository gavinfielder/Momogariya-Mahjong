using UnityEngine;

namespace Mahjong
{
    public enum TileVisibility : byte
    {
        //Note: the first item in this list should match the state of the TileBase prefab.
        FaceUp,
        FaceDown,
        InHand
    }

    public class Tile : MonoBehaviour
    {
        private TileID id;
        private bool set = false;
        private TileRenderer _tileRenderer;
        public TileRenderer Renderer
        {
            get
            {
                if (_tileRenderer == null)
                    _tileRenderer = gameObject.GetComponent<TileRenderer>();
                return _tileRenderer;
            }
        }
        private int _accessKey = 0;
        private TileVisibility visibility;
        public bool StolenDiscard;
        public int StolenFrom = 0;
        public bool InOpenMeld = false;
        private bool initialized = false;

        //Stores a reference to the last tile that was clicked
        public static Tile LastClickedTile;

        //Initialization
        private void Start()
        {
            Initialize();
        }
        private void Initialize()
        {
            if (initialized) return;
            initialized = true;
        }

        //Sets the tile. Can only be set once. 
        public void Set(TileID.Suits suit, byte number, bool aka = false)
        {
            if (set) return;
            id = new TileID(suit, number, aka);
            set = true;
        }

        //Sets the owner of this tile by setting the access key for read permission
        //only if there is no current owner
        public void SetOwner(int newAccessKey)
        {
            if (_accessKey != 0) return;
            _accessKey = newAccessKey;
        }

        //Releases ownership of this tile by resetting the access key for read permission
        public void ReleaseOwnership(int accessKey)
        {
            if (accessKey != _accessKey) return;
            _accessKey = 0;
        }

        //Reveals the tile and releases ownership (all objects will have read permission)
        public void SetVisibility(TileVisibility value, int accessKey = 0)
        {
            //If the value is the same, don't bother
            if (visibility == value) return;
            //Check if locked for ownership permissions and if so, check access key
            if (_accessKey != 0 && accessKey != _accessKey)
            {
                Debug.Log("Wrong acess key: Tile.SetVisiblity(value, accessKey)");
                return;
            }
            //Set visibility
            visibility = value;
            //If it's face up, there's no point in restricting read permissions anymore.
            if (visibility == TileVisibility.FaceUp) ReleaseOwnership(_accessKey);
            //Pass visibility setting to renderer
            if (!(initialized)) Initialize();
            Renderer.Visibility = visibility;
        }

        //Queries the tile. If the tile is locked and wrong access key is passed, return Hidden
        public TileID Query(int accessKey = 0)
        {
            if (_accessKey == 0 || accessKey == _accessKey || visibility == TileVisibility.FaceUp)
                return id.Copy();
            else
                return TileID.Hidden;
        }

        //Occurs when the user clicks the tile
        private void OnMouseUpAsButton()
        {
            LastClickedTile = this;
            EventManager.FlagEvent("Tile Clicked");
        }
    }

}
