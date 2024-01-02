using UnityEngine;

namespace StarSquad.Game.Mode
{
    public abstract class GameMode
    {
        public GameState gameState = GameState.Starting;

        public virtual void Tick()
        {
        }

        public void UpdateServerTick(int serverTick)
        {
        }
        
        public abstract void Render();
        
        public abstract DynamicGameData GetDynamicGameData();

        private void OnEnd()
        {
            GameManager.instance.gameWorld.cameraFollow.ZoomOut();
        }
        
        public void UpdateState(GameState gameState)
        {
            if (this.gameState != gameState)
            {
                this.gameState = gameState;

                if (gameState != GameState.Starting)
                {
                    GameManager.instance.gameStartUI.Hide();
                }
                
                switch (gameState)
                {
                    case GameState.Starting:
                        break;
                    
                    case GameState.Started:
                        break;
                    
                    case GameState.Ending:
                        this.OnEnd();
                        break;
                    
                    case GameState.Ended:
                        break;
                    
                    default:
                        break;
                }
            }
        }
    }
}