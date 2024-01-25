using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using System.IO;

namespace NotUselessArmor
{
    public static class Helpers
    {
        public static ScriptSettings config = ScriptSettings.Load("scripts\\Okoniewitz\\NotUselessArmor.ini");

        public static void ConfigRead()
        {
            string[] ConfigLines =
            { 
                "[FEATURES]",
                "DEGRADATION=false",
                "VISIBLEARMOR=true",
                "HEALING=true",
                "LOCATIONALDAMAGE=true",
                "RAGDOLLONDAMAGE=true",
                "WRITHEDISABLED=true",
                "[SETTINGS]",
                "PUTOFFARMORONLYWHENDEAD=true",
                "ARMORPERHEAL=3",
                "DELAYPERHEAL=3000",
                "DAMAGEDELAY=6000",
                "RUNSHOOTJUMPDELAY=2000",
                "RAGDOLLCHANCE=3",
                "VISIBLEARMORTYPE=3",
                "RAGDOLLTIME=1300",
                "RAGDOLLMINIMALTIME=500",
                "[DEBUG]",
                "DEBUG=false",
                "[SAVE]",
                "PLAYER0ARMOR=false",
                "PLAYER1ARMOR=false",
                "PLAYER2ARMOR=false",
                "[ARMOR]",
                "MICHAELCOMPONENTID=8",
                "MICHAELDRAWABLEID=17",
                "MICHAELCLEARDRAWABLEID=0",
                "FRANKLINCOMPONENTID=8",
                "FRANKLINDRAWABLEID=6",
                "FRANKLINCLEARDRAWABLEID=14",
                "TREVORCOMPONENTID=8",
                "TREVORDRAWABLEID=8",
                "TREVORCLEARDRAWABLEID=14"
            };
            if(!File.Exists("scripts\\Okoniewitz\\NotUselessArmor.ini")) { File.WriteAllLines("scripts\\Okoniewitz\\NotUselessArmor.ini",ConfigLines); }

            Main.Degradation = config.GetValue<bool>("FEATURES", "DEGRADATION", false);
            Main.VisibleArmorEnabled = config.GetValue<bool>("FEATURES", "VISIBLEARMOR", true);
            Main.HealingEnabled = config.GetValue<bool>("FEATURES", "HEALING", true);
            Main.LocationalDamage = config.GetValue<bool>("FEATURES", "LOCATIONALDAMAGE", true);
            Main.RagdollOnDamage = config.GetValue<bool>("FEATURES", "RAGDOLLONDAMAGE", true);
            Main.WritheDisabled = config.GetValue<bool>("FEATURES", "WRITHEDISABLED", true);
            Main.Debug = config.GetValue<bool>("DEBUG", "DEBUG", false);
            Main.PutOffArmorOnlyWhenDead = config.GetValue<bool>("SETTINGS", "PUTOFFARMORONLYWHENDEAD", true);
            Main.ApPerHeal = config.GetValue<int>("SETTINGS", "ARMORPERHEAL", 3);
            Main.DelayTime = config.GetValue<int>("SETTINGS", "DELAYPERHEAL", 3000);
            Main.DamageDelay = config.GetValue<int>("SETTINGS", "DAMAGEDELAY", 6000);
            Main.RunShootJumpDelay = config.GetValue<int>("SETTINGS", "RUNSHOOTJUMPDELAY", 2000);
            Main.RagdollChance = config.GetValue<int>("SETTINGS", "RAGDOLLCHANCE", 3);
            Main.VisibleArmorType = config.GetValue<int>("SETTINGS", "VISIBLEARMORTYPE", 3);
            Main.RagdollTime = config.GetValue<int>("SETTINGS", "RAGDOLLTIME", 1300);
            Main.RagdollMinimalTime = config.GetValue<int>("SETTINGS", "RAGDOLLMINIMALTIME", 500);
            Main.PlayerArmorWear = new bool[] {
                config.GetValue<bool>("SAVE", "PLAYER0ARMOR", false),
                config.GetValue<bool>("SAVE", "PLAYER1ARMOR", false),
                config.GetValue<bool>("SAVE", "PLAYER2ARMOR", false)
            };
            Modeles = new int[3, 3]
            {
                { config.GetValue<int>("ARMOR", "MICHAELCOMPONENTID", 8),config.GetValue<int>("ARMOR", "MICHAELDRAWABLEID", 17), config.GetValue<int>("ARMOR", "MICHAELCLEARDRAWABLEID", 0)},
                { config.GetValue<int>("ARMOR", "FRANKLINCOMPONENTID", 8),config.GetValue<int>("ARMOR", "FRANKLINDRAWABLEID", 6), config.GetValue<int>("ARMOR", "FRANKLINCLEARDRAWABLEID", 14)},
                { config.GetValue<int>("ARMOR", "TREVORCOMPONENTID", 8),config.GetValue<int>("ARMOR", "TREVORDRAWABLEID", 8), config.GetValue<int>("ARMOR", "TREVORCLEARDRAWABLEID", 14)}
            };
    }

        public static int GetArmorType()
        {
            int armor = Game.Player.Character.Armor;
            if (armor > 80) return 5;
            if (armor > 60) return 4;
            if (armor > 40) return 3;
            if (armor > 20) return 2;
            if (armor > 0) return 1;
            return 0;
        }

        public static int[] Armor =
        {

            (int)Bone.SkelLeftClavicle,
            (int)Bone.SkelRightClavicle,
            (int)Bone.SkelSpine0,
            (int)Bone.SkelSpine1,
            (int)Bone.SkelSpine2,
            (int)Bone.SkelSpine3,
            (int)Bone.SkelSpineRoot,
            (int)Bone.SkelRoot,
            (int)Bone.SkelPelvis,
            (int)Bone.IKRoot,
        };
        public static int ArmorPlayer = 0;

        public static bool CanRegenHP()
        {
            Ped Player = Game.Player.Character;
            return !Player.IsSprinting && !Player.IsSwimming && !Player.IsRagdoll && !Player.IsJumping && !Player.IsClimbing && !Player.IsDiving && !Player.IsGettingUp && !Player.IsShooting;
        }


        public static bool Damaged(OutputArgument bone)
        {
            bool damaged = Function.Call<bool>(Hash.GET_PED_LAST_DAMAGE_BONE, Game.Player.Character, bone);
            Function.Call(Hash.CLEAR_PED_LAST_DAMAGE_BONE, Game.Player.Character);
            return damaged;
        }

        public static int GetPlayerID()
        {
            if (Game.Player.Character.Model.Hash == GetHashKey("PLAYER_TWO")) return 2;
            if (Game.Player.Character.Model.Hash == GetHashKey("PLAYER_ONE")) return 1;
            if (Game.Player.Character.Model.Hash == GetHashKey("PLAYER_ZERO")) return 0;
            return 3;
        }

        private static int[,] Modeles;

        public static void ShowArmor(bool Show)
        {
            if(Show) Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character, Modeles[GetPlayerID(), 0], Modeles[GetPlayerID(), 1], 0);
            else Function.Call(Hash.SET_PED_COMPONENT_VARIATION, Game.Player.Character, Modeles[GetPlayerID(), 0], Modeles[GetPlayerID(), 2], 0);
        }

        private static int GetHashKey(string value)
        {
            return Function.Call<int>(Hash.GET_HASH_KEY, value);
        }
    }
}