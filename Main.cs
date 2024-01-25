using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;
using GTA.Math;
using System.IO;
using System.Windows.Forms;
using GTA.NaturalMotion;
using System.Drawing;

namespace NotUselessArmor
{
    public class Main : Script
    {
        public Main()
        {
            Helpers.ConfigRead();
            Tick += OnTick;
            if(WritheDisabled) Tick += NoWrithe;
            if (Debug) Tick += DebugTick;
        }

        public static int ApPerHeal, DelayTime, DamageDelay, RunShootJumpDelay, RagdollChance, VisibleArmorType, RagdollTime, RagdollMinimalTime;
        public static bool Degradation, VisibleArmorEnabled, PutOffArmorOnlyWhenDead, HealingEnabled, LocationalDamage, RagdollOnDamage, WritheDisabled;
        public static bool Debug=true;
        public static bool[] PlayerArmorWear;


        private static int ArmorType;
        private static bool Dead, NoArmor;
        private static int HealDelay;

        public static void DebugTick(object sender, EventArgs e)
        {
            string ArmorWearString;
            if (VisibleArmorEnabled) ArmorWearString = "\nP0: " + PlayerArmorWear[0] + " P1: " + PlayerArmorWear[1] + " P2: " + PlayerArmorWear[2] + " PutOffOnlyWhenDead: " + PutOffArmorOnlyWhenDead; else ArmorWearString = "\nArmorWear Disabled";
            GTA.UI.Screen.ShowSubtitle("Armor type: " + ArmorType + " Degradation: " + Degradation + " Armor: " + Game.Player.Character.Armor + "\nHealing: " + HealingEnabled + " Delay: " + (Math.Round((double)(HealDelay - Game.GameTime) / 100) / 10) + "s Can Regen: " + Helpers.CanRegenHP() + ArmorWearString);
           
            if (Game.IsKeyPressed(Keys.M))
            {
                Game.Player.Character.Armor++;
            }
            if (Game.IsKeyPressed(Keys.N))
            {
                Game.Player.Character.Armor--;
            }
        }

        public static void NoWrithe(object sender, EventArgs e)
        {
           Ped[] peds = World.GetAllPeds();
            foreach(Ped ped in peds)
            {
                if(ped.CanWrithe)ped.CanWrithe = false;
            }
        }

        public static void OnTick(object sender, EventArgs e)
        {
            OutputArgument boneOut = new OutputArgument();
            if (Game.Player.Character.Armor <= 0) NoArmor = true;
            if (NoArmor && (Helpers.GetArmorType() == 1 || Helpers.GetArmorType() == 2)) { PlayerArmorWear[Helpers.GetPlayerID()] = false; NoArmor = false; }
            if (Game.Player.Character.Armor > 0) NoArmor = false;
            if (Helpers.GetArmorType() > ArmorType) { ArmorType = Helpers.GetArmorType(); if (ArmorType >= VisibleArmorType && VisibleArmorEnabled) PlayerArmorWear[Helpers.GetPlayerID()] = true; };
            if (Game.Player.Character.Armor == 0) { ArmorType = 0; if (!PutOffArmorOnlyWhenDead) PlayerArmorWear[Helpers.GetPlayerID()] = false; }
            if (Game.Player.Character.IsDead) Dead = true; else if (Dead) { PlayerArmorWear[Helpers.GetPlayerID()] = false; Dead = false; }

            if (!Helpers.CanRegenHP() && HealDelay < Game.GameTime + RunShootJumpDelay) HealDelay = Game.GameTime +RunShootJumpDelay;
            if (Helpers.Damaged(boneOut))
            {
                int rnd = new Random().Next(10);
                HealDelay = Game.GameTime + DamageDelay;
                if (Game.Player.Character.Armor > 0)
                {
                    if (!Helpers.Armor.Contains(boneOut.GetResult<int>()) && LocationalDamage)
                    {
                        int dif = Helpers.ArmorPlayer - Game.Player.Character.Armor;
                        Game.Player.Character.Health -= dif;
                        Game.Player.Character.Armor += dif;
                        if (rnd <= RagdollChance && RagdollOnDamage)
                        {
                            Function.Call<bool>(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, RagdollTime, RagdollMinimalTime, 0, true, true, true);
                        }
                    }
                }
                else if (rnd <= RagdollChance && RagdollOnDamage)
                {
                    Function.Call<bool>(Hash.SET_PED_TO_RAGDOLL, Game.Player.Character, RagdollTime, RagdollMinimalTime, 0, true, true, true);
                }
            }
            if (ArmorType > 0)
            {
                if (Degradation)
                {
                    if (Game.Player.Character.Armor <= (ArmorType - 1) * 20)
                    {
                        ArmorType = Helpers.GetArmorType();
                    }
                }

                if (Game.Player.Character.Armor < ArmorType * 20 && HealDelay <= Game.GameTime && HealingEnabled)
                {
                    if (((ArmorType * 20) - Game.Player.Character.Armor) < ApPerHeal) Game.Player.Character.Armor += (ArmorType * 20) - Game.Player.Character.Armor;
                    else
                        Game.Player.Character.Armor += ApPerHeal;
                    HealDelay = Game.GameTime + DelayTime;
                }
            }

            if (VisibleArmorEnabled)
            {
                Helpers.ShowArmor(PlayerArmorWear[Helpers.GetPlayerID()]);
            }
            Helpers.ArmorPlayer = Game.Player.Character.Armor;

            bool Saved = Helpers.config.GetValue<bool>("SAVE", "PLAYER" + Helpers.GetPlayerID() + "ARMOR", false);
            Helpers.config.SetValue<bool>("SAVE", "PLAYER" + Helpers.GetPlayerID() + "ARMOR", PlayerArmorWear[Helpers.GetPlayerID()]);
            if(Saved!=PlayerArmorWear[Helpers.GetPlayerID()])Helpers.config.Save();
        }
    }
}