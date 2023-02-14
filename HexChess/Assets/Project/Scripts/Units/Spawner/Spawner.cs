using Networking.Client;
using System.Collections.Generic;

public class Spawner
{
    public virtual Unit SpawnUnit(Hex hex, ClassType classType) { return null; }
    public Unit SpawnUnit(Data.Unit data) 
    {
        UnitType unitType = (UnitType)data.unit_type;
        ClassType classType = (ClassType)data.class_type;

        switch (unitType)
        {
            case UnitType.None:
                break;
            case UnitType.Swordsman:
                switch (classType)
                {
                    case ClassType.Light:
                        SwordsmanLight light = new SwordsmanLight(data);
                        return light;
                    case ClassType.Dark:
                        SwordsmanDark dark = new SwordsmanDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Tank:
                switch (classType)
                {
                    case ClassType.Light:
                        TankLight light = new TankLight(data);
                        return light;
                    case ClassType.Dark:
                        TankDark dark = new TankDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Knight:
                switch (classType)
                {
                    case ClassType.Light:
                        KnightLight light = new KnightLight(data);
                        return light;
                    case ClassType.Dark:
                        KnightDark dark = new KnightDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Archer:
                switch (classType)
                {
                    case ClassType.Light:
                        ArcherLight light = new ArcherLight(data);
                        return light;
                    case ClassType.Dark:
                        ArcherDark dark = new ArcherDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Jester:
                switch (classType)
                {
                    case ClassType.Light:
                        JesterLight light = new JesterLight(data);
                        return light;
                    case ClassType.Dark:
                        JesterDark dark = new JesterDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Wizzard:
                switch (classType)
                {
                    case ClassType.Light:
                        WizzardLight light = new WizzardLight(data);
                        return light;
                    case ClassType.Dark:
                        WizzardDark dark = new WizzardDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.Queen:
                switch (classType)
                {
                    case ClassType.Light:
                        QueenLight light = new QueenLight(data);
                        return light;
                    case ClassType.Dark:
                        QueenDark dark = new QueenDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.King:
                switch (classType)
                {
                    case ClassType.Light:
                        KingLight light = new KingLight(data);
                        return light;
                    case ClassType.Dark:
                        KingDark dark = new KingDark(data);
                        return dark;
                    default:
                        break;
                }
                break;
            case UnitType.JesterIllusion:
                switch (classType)
                {
                    case ClassType.Light:
                        JesterLightIllusion light = new JesterLightIllusion(data);
                        return light;
                    case ClassType.Dark:
                        JesterDarkIllusion dark = new JesterDarkIllusion(data);
                        return dark;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        return null; 
    }
    public static List<Unit> SpawnUnits(List<Data.Unit> units_data)
    {
        Spawner spawner = new Spawner();
        List<Unit> units = new List<Unit>();

        foreach (var unit_data in units_data)
        {
            units.Add(spawner.SpawnUnit(unit_data));
        }

        return units;
    }

    public static Ability CreateAbility(Unit unit, Data.Ability ability_data)
    {
        AbilityClass ability_class = (AbilityClass)ability_data.globalID;

        switch (ability_class)
        {
            case AbilityClass.Earthshaker:
                Earthshaker earthshaker = new Earthshaker(unit, ability_data);
                return earthshaker;
            case AbilityClass.Fear:
                Fear fear = new Fear(unit, ability_data);
                return fear;
            case AbilityClass.Joust:
                Joust joust = new Joust(unit, ability_data);
                return joust;
            case AbilityClass.Warstrike:
                Warstrike warstrike = new Warstrike(unit, ability_data);
                return warstrike;
            case AbilityClass.Powershot:
                Powershot poweshot = new Powershot(unit, ability_data);
                return poweshot;
            case AbilityClass.Trap:
                Trap trap = new Trap(unit, ability_data);
                return trap;
            case AbilityClass.Trics_of_the_trade:
                TricsOfTheTrade trics_of_the_trade = new TricsOfTheTrade(unit, ability_data);
                return trics_of_the_trade;
            case AbilityClass.The_Fool:
                TheFool the_fool = new TheFool(unit, ability_data);
                return the_fool;
            case AbilityClass.Blessing:
                Blessing blessing = new Blessing(unit, ability_data);
                return blessing;
            case AbilityClass.Skyfall:
                Skyfall skyfall = new Skyfall(unit, ability_data);
                return skyfall;
            case AbilityClass.Fireball:
                Fireball fireball = new Fireball(unit, ability_data);
                return fireball;
            case AbilityClass.Necromancy:
                Necromancy necromancy = new Necromancy(unit, ability_data);
                return necromancy;
            case AbilityClass.Curse:
                Curse curse = new Curse(unit, ability_data);
                return curse;
            case AbilityClass.Vampirism:
                Vampirism vampirism = new Vampirism(unit, ability_data);
                return vampirism;
            case AbilityClass.Illu_Trics_of_the_trade:
                TricsOfTheTradeIllusion illu_trics_of_the_trade = new TricsOfTheTradeIllusion(unit, ability_data);
                return illu_trics_of_the_trade;
            case AbilityClass.Illu_The_Fool:
                TheFoolIllusion illu_the_fool = new TheFoolIllusion(unit, ability_data);
                return illu_the_fool;
            default:
                break;
        }

        return null;
    }
    public static Ability CreateSpecialAbility(Unit unit, Data.Ability special_ability)
    {
        UnitType unit_type = (UnitType)special_ability.globalID;

        switch (unit_type)
        {
            case UnitType.None:
                break;
            case UnitType.Swordsman:
                Swordsman_SA swordsman_SA = new Swordsman_SA(unit, special_ability);
                return swordsman_SA;
            case UnitType.Tank:
                Tank_SA tank_SA = new Tank_SA(unit, special_ability);
                return tank_SA;
            case UnitType.Knight:
                Knight_SA knight_SA = new Knight_SA(unit, special_ability);
                return knight_SA;
            case UnitType.Archer:
                Archer_SA archer_SA = new Archer_SA(unit, special_ability);
                return archer_SA;
            case UnitType.Jester:
                Jester_SA jester_SA = new Jester_SA(unit, special_ability);
                return jester_SA;
            case UnitType.Wizzard:
                Wizzard_SA wizzard_SA = new Wizzard_SA(unit, special_ability);
                return wizzard_SA;
            case UnitType.Queen:
                Queen_SA queen_SA = new Queen_SA(unit, special_ability);
                return queen_SA;
            case UnitType.King:
                King_SA king_SA = new King_SA(unit, special_ability);
                return king_SA;
            case UnitType.JesterIllusion:
                Jester_Illusion_SA illusion_SA = new Jester_Illusion_SA(unit, special_ability);
                return illusion_SA;
            default:
                break;
        }

        return null;
    }
    public static CC CreateCC(Unit unit, Data.CC cc_data)
    {
        CC_Type cc_type = (CC_Type)cc_data.cc_type;
        switch (cc_type)
        {
            case CC_Type.STUN:
                Stun stun = new Stun(unit, cc_data);
                return stun;
            case CC_Type.DISARM:
                Disarm disarm = new Disarm(unit, cc_data);
                return disarm;
            default:
                break;
        }
        return null;
    }
}


public class SpawnSwordsman : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Swordsman;

        switch (classType)
        {
            case ClassType.Light:
                SwordsmanLight light = new SwordsmanLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 1,
                    current_health = 1,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                SwordsmanDark dark = new SwordsmanDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 1,
                    current_health = 1,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnTank : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Tank;

        switch (classType)
        {
            case ClassType.Light:
                TankLight light = new TankLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 4,
                    current_health = 4,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                TankDark dark = new TankDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 4,
                    current_health = 4,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnQueen : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Queen;

        switch (classType)
        {
            case ClassType.Light:
                QueenLight light = new QueenLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 2,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                QueenDark dark = new QueenDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 2,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}
public class SpawnKing : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.King;

        switch (classType)
        {
            case ClassType.Light:
                KingLight light = new KingLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 5,
                    current_health = 5,

                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                KingDark dark = new KingDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 5,
                    current_health = 5,

                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnArcher : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Archer;

        switch (classType)
        {
            case ClassType.Light:
                ArcherLight light = new ArcherLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 2,
                    current_health = 2,

                    damage = 1,
                    attack_range = 2,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                ArcherDark dark = new ArcherDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 2,
                    current_health = 2,

                    damage = 1,
                    attack_range = 2,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnWizzard : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Wizzard;

        switch (classType)
        {
            case ClassType.Light:
                WizzardLight light = new WizzardLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                WizzardDark dark = new WizzardDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 0,
                    attack_range = 0,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnJester : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Jester;

        switch (classType)
        {
            case ClassType.Light:
                JesterLight light = new JesterLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                JesterDark dark = new JesterDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }
}

public class SpawnKnight : Spawner
{
    public override Unit SpawnUnit(Hex hex, ClassType classType)
    {
        UnitType unitType = UnitType.Knight;

        switch (classType)
        {
            case ClassType.Light:
                KnightLight light = new KnightLight(hex, unitType, classType);
                light.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return light;
            case ClassType.Dark:
                KnightDark dark = new KnightDark(hex, unitType, classType);
                dark.stats = new UnitStats
                {
                    max_health = 3,
                    current_health = 3,

                    damage = 1,
                    attack_range = 1,
                    attack_speed = 0.25f
                };
                return dark;
            default:
                return null;
        }

    }

}


