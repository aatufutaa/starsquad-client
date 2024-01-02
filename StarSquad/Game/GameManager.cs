using StarSquad.Game;
using StarSquad.Game.Misc;
using StarSquad.Game.UI;
using StarSquad.Loader;
using StarSquad.Lobby.Hero;
using StarSquad.Lobby.UI.Button;
using StarSquad.Net;
using StarSquad.Net.Packet.Game.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace StarSquad.Game
{
    public class GameManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler,
        IEndDragHandler
    {

        public Material heroMat;
        public Material heroMatTransparent;

        public static GameManager instance;

        public GameWorld gameWorld;

        private float lastTickTime;
        
        private bool stopped;

        public GameStartUI gameStartUI;
        public GameObject youDiedUI;
        public WinnersUI winnersUI;
        public GameResultsUI gameResultsUI;

        public CustomButton exitButton;

        public HeroPlayer heroPlayer;

        void Start()
        {
            instance = this;

            GameObject.Find("Map").transform.rotation = Quaternion.Euler(AngleUtil.Angle, 0f, 0f);

            Debug.Log("start move");
            if (!LoaderManager.IsUsingNet())
            {
                Application.targetFrameRate = 60;
                this.gameObject.AddComponent<EventSystem>();
                this.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            this.gameWorld = new GameWorld();

            if (!LoaderManager.IsUsingNet())
            {
                const int mapSizeX = 16;
                const int mapSizeY = 40;
            
                this.gameWorld.Init(GameType.Tutorial, "tree", mapSizeX, mapSizeY);

                //this.gameWorld.blockManager.Init(MapSizeX, MapSizeY);

                this.gameWorld.blockManager.tileManager.AddWater(0, 0);

                for (int i = 1; i < 5; i++)
                {
                    for (int j = 1; j < 5; j++)
                    {
                        this.gameWorld.blockManager.tileManager.AddWater(i, j);
                    }
                }

                this.gameWorld.blockManager.tileManager.AddWater(6, 6);
                this.gameWorld.blockManager.tileManager.AddWater(6, 7);
                this.gameWorld.blockManager.tileManager.AddWater(7, 7);
                this.gameWorld.blockManager.tileManager.AddWater(7, 8);
                this.gameWorld.blockManager.tileManager.AddWater(7, 9);


                /*for (var x = -5; x < 5; x++)
                {
                    for (var y = -10; y < -5; y++)
                    {
                        this.gameWorld.blockManager.AddGrass(x, y);
                    }
                }

                for (var x = -5; x < 3; x++)
                {
                    this.gameWorld.blockManager.AddBox(x, -11);
                }

                for (var x = -5; x < 3; x++)
                {
                    this.gameWorld.blockManager.AddStone1(x, -2);
                }*/

                this.gameWorld.blockManager.OnBlocksFinished();
            }
            
            this.exitButton.onClick.AddListener(() => this.gameResultsUI.Show());
        }

        void Update()
        {
            if (this.stopped) return;

            var partialTicks = (Time.time - this.lastTickTime) / 0.05f;

            this.gameWorld.Render(partialTicks);
        }

        private void FixedUpdate()
        {
            if (this.stopped) return;

            this.lastTickTime = Time.time;

            this.gameWorld.Tick();
        }

        // player input
        private bool pressed;

        public void OnDrag(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            this.pressed = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!this.pressed) return;
            
            var ray = this.gameWorld.cameraFollow.camera.ScreenPointToRay(eventData.position);
            var plane = new Plane(Vector3.up,
                new Vector3(0, this.gameWorld.thePlayer.hero.attackHeight, 0)); // TODO: store as member

            if (!plane.Raycast(ray, out var dist)) return;

            var worldPos = ray.GetPoint(dist);
            this.gameWorld.HandleAttackInput(new Vector2(worldPos.x, worldPos.z));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            this.pressed = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var dir = eventData.position - eventData.pressPosition;

            this.gameWorld.HandleMoveInput(dir);
        }

        public void SendHome()
        {
            LoaderManager.instance.networkManager.connectionManager.SendPacket(new HomeOutgoingPacket());
        }

        private void Stop0()
        {
            this.stopped = true;
        }

        public static void Stop()
        {
            if (!instance) return;

            instance.Stop0();
            instance = null;
        }

        public void PauseGame()
        {
            // TODO: aud
        }

        public void ResumeGame()
        {
            // TODO: au
        }
    }
}