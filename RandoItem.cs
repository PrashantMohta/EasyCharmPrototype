using ItemChanger;
using ItemChanger.Locations;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace EasyCharmTest
{
    public static class Constants
    {
        /// <summary>
        /// Name of your Item that will get randomized
        /// </summary>
        public const string ITEM_NAME = "potato_item";

        /// <summary>
        /// Name of your location to which some item will be randomized to
        /// </summary>
        public const string LOCATION_NAME = "potato_location";

        /// <summary>
        /// Scene Name for your location
        /// </summary>
        public const string LOCATION_SCENE = "Tutorial_01";

        /// <summary>
        /// X coordinate for your location
        /// </summary>
        public const float LOCATION_X = 38f;

        /// <summary>
        /// Y coordinate for your location
        /// </summary>
        public const float LOCATION_Y = 11f;

        /// <summary>
        /// The Logic defining the constraints that a player must meet to be able to access your location
        /// </summary>
        public const string LOCATION_LOGIC = "Tutorial_01[right1] | Tutorial_01[top1] | Tutorial_01[top2]";

    }

    public class CustomItem : AbstractItem
        {
            public override void GiveImmediate(GiveInfo info)
            {
                // do whatever your mod needs to do to give the item here
                Modding.Logger.Log("YOU GOT THE ITEM!");
            }
        }

    public class RandoItem
    {
        // call this in Initialize of your mod after checking if ItemChanger is available
        public static void InitRandoConnection()
        {
            // Define your Custom Location and Custom item
            Finder.DefineCustomLocation(new CoordinateLocation {
                name = Constants.LOCATION_NAME,
                sceneName = Constants.LOCATION_SCENE,
                x = Constants.LOCATION_X,
                y= Constants.LOCATION_Y, 
                flingType = FlingType.DirectDeposit 
            });
            Finder.DefineCustomItem(new CustomItem { name = Constants.ITEM_NAME });
                
            // Add item to rando (and define how it impacts logic) & the logic required to consider the location "reachable"
            RCData.RuntimeLogicOverride.Subscribe(50f, AddItemAndLogic);

            // Add the item & location to RequestBuilder the thing that makes it do the randomizing
            RequestBuilder.OnUpdate.Subscribe(0.3f, AddLocationAndItemToRandomization);
        }

        private static void AddLocationAndItemToRandomization(RequestBuilder rb)
        {
            rb.AddItemByName(Constants.ITEM_NAME);
            rb.AddLocationByName(Constants.LOCATION_NAME);
        }

        private static void AddItemAndLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            //Here we define that our item does not make any changes to the reachability of locations
            lmb.AddItem(new EmptyItem(Constants.ITEM_NAME));

            //Here we define that our location is accessible when certain logic is met, you will need to modify this for your location
            lmb.DeserializeJson(LogicManagerBuilder.JsonType.Locations, "[{\"name\": \""+ Constants.LOCATION_NAME + "\",\"logic\": \""+ Constants.LOCATION_LOGIC + "\"}]");
        }
    }
 }

