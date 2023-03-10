using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SFCore;
using Modding;

namespace EasyCharmTest
{
    public class EasyCharmState
    {
        public bool IsEquipped = false;
        public bool GotCharm = false;
        public bool IsNew = false;
    }
    public abstract class EasyCharm
    {
        private Sprite Sprite;
        public int Id { get; private set; }
        public bool IsEquipped { get; protected set; } = false;
        public bool GotCharm { get; protected set; } = false;
        public bool IsNew { get; protected set; } = false;

        protected abstract Sprite GetSpriteInternal();
        protected abstract string GetName();
        protected abstract string GetDescription();
        protected abstract int GetCharmCost();

        public Sprite GetSprite()
        {
            if (Sprite == null)
            {
                Sprite = GetSpriteInternal();
            }
            return Sprite;
        }

        public EasyCharm() {
            Id = CharmHelper.AddSprites(GetSprite())[0];
            ModHooks.LanguageGetHook += OnLanguageGetHook;
            ModHooks.GetPlayerBoolHook += OnGetPlayerBoolHook;
            ModHooks.SetPlayerBoolHook += OnSetPlayerBoolHook;
            ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
        }

        private int OnGetPlayerIntHook(string target, int orig)
        {
            // Check if the charm cost is wanted
            if (target == $"charmCost_{Id}")
            {
                return GetCharmCost();
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        private bool OnSetPlayerBoolHook(string target, bool orig)
        {
            // Check if the charm gotten flag is wanted
            if (target == $"gotCharm_{Id}")
            {
                GotCharm = orig;
            }
            // Check if the charm new flag is wanted
            if (target == $"newCharm_{Id}")
            {
                IsNew = orig;
            }
            // Check if the charm equipped flag is wanted
            if (target == $"equippedCharm_{Id}")
            {
                IsEquipped = orig;
            }
            // Always return orig in set hooks, unless you specifically want to change what is saved
            return orig;
        }

        private bool OnGetPlayerBoolHook(string target, bool orig)
        {
            // Check if the charm gotten flag is wanted
            if (target == $"gotCharm_{Id}")
            {
                return GotCharm;
            }
            // Check if the charm new flag is wanted
            if (target == $"newCharm_{Id}")
            {
                return IsNew;
            }
            // Check if the charm equipped flag is wanted
            if (target == $"equippedCharm_{Id}")
            {
                return IsEquipped;
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        private string OnLanguageGetHook(string key, string sheetTitle, string orig)
        {
            // Check if the charm name is wanted
            if (key == $"CHARM_NAME_{Id}")
            {
                return GetName();
            }
            // Check if the charm description is wanted
            else if (key == $"CHARM_DESC_{Id}")
            {
                return GetDescription();
            }
            // Return orig if we don't want to make any changes
            return orig;
        }

        public void GiveCharm(bool IsNew = false)
        {
            PlayerData.instance.SetBool($"gotCharm_{Id}", true);
            PlayerData.instance.SetBool($"newCharm_{Id}", IsNew);
        }
        public void TakeCharm()
        {
            PlayerData.instance.SetBool($"gotCharm_{Id}", false);
            PlayerData.instance.SetBool($"newCharm_{Id}", false);
            PlayerData.instance.SetBool($"equippedCharm_{Id}", false);
        }

        public EasyCharmState GetCharmState() {
            return new EasyCharmState { GotCharm = GotCharm ,IsEquipped = IsEquipped , IsNew = IsNew };
        }
        public void RestoreCharmState(EasyCharmState state) {
            GotCharm = state.GotCharm;
            IsEquipped = state.IsEquipped;
            IsNew = state.IsNew;
        }

    }
}
