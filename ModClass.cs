using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;
using Satchel;

namespace EasyCharmTest
{
    public class DandyCharm : EasyCharm
    {
        protected override int GetCharmCost() => 1;

        protected override string GetDescription() => "World's simplest charm";

        protected override string GetName() => "DandyCharm";

        protected override Sprite GetSpriteInternal()=> AssemblyUtils.GetSpriteFromResources("DandyCharm.png");
    }

    public class settings {
        public Dictionary<string,EasyCharmState> Charms; 
    }
    public class EasyCharmTest : Mod, ILocalSettings<settings>
    {
        internal static EasyCharmTest Instance;
        internal settings localSettings = new settings();
        internal Dictionary<string, EasyCharm> Charms = new Dictionary<string, EasyCharm>{ 
            {"DandyCharm",new DandyCharm()},
        };
        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");
            Instance = this;
            Log("Initialized");
            On.HeroController.Update += HeroController_Update;
        }

        private void HeroController_Update(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);
            if(Charms["DandyCharm"].GotCharm && Charms["DandyCharm"].IsEquipped)
            {
                Log("Dandy");
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                Log("Giving DandyCharm");
                Charms["DandyCharm"].GiveCharm(true);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                Log("Taking DandyCharm");
                Charms["DandyCharm"].TakeCharm();
            }
        }

        public void OnLoadLocal(settings s)
        {
            localSettings = s;
            if (s.Charms != null)
            {
                foreach(var kvp in s.Charms)
                {
                    if(Charms.TryGetValue(kvp.Key, out EasyCharm m))
                    {
                        m.RestoreCharmState(kvp.Value);
                    }
                }
            }
        }

        public settings OnSaveLocal()
        {
            localSettings.Charms = new Dictionary<string, EasyCharmState>();
            foreach (var kvp in Charms)
            {
                if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
                {
                    localSettings.Charms[kvp.Key] = m.GetCharmState();
                }
            }
            return localSettings;
        }
    }
}