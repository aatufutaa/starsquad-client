namespace StarSquad.Game.Mode
{
    public class LHSGame : GameMode
    {
        public LHSGame()
        {
        }

        public override void Tick()
        {
            base.Tick();
        }

        public override void Render()
        {
        }

        public override DynamicGameData GetDynamicGameData()
        {
            return new LHSDynamicGameData();
        }
    }
}