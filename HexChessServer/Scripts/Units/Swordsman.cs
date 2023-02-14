using Networking.Server;
using System;

namespace Units
{
    public class Swordsman : Melee
    {
        public Swordsman(Data.Unit unit) : base(unit)
        {

        }

        public override void Subscribe(UnitEvents _events)
        {
            _events.OnExitMovement += OnExitMovement;
        }
        public override void UnSubscribe(UnitEvents _events)
        {
            _events.OnExitMovement -= OnExitMovement;
        }

        private void OnExitMovement(Unit unit, Game game)
        {
            special_ability.UseAbility(unit,game);
        }
    }

    public class SwordsmanLight : Swordsman
    {

        public SwordsmanLight(Data.Unit unit) : base(unit)
        {
        }

    }

    public class SwordsmanDark : Swordsman
    {
        public SwordsmanDark(Data.Unit unit) : base(unit)
        {
        }

    }
}

