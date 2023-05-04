using KitchenLib;
using KitchenLib.Event;
using KitchenMods;
using System.Reflection;
using UnityEngine;
using KitchenTacoTime;
using KitchenTacoTime.Customs.Tacos.Fish;
using KitchenTacoTime.Customs;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Linq;
using Unity.Entities;
using UnityEngine.VFX;
using KitchenLib.Colorblind;
using Kitchen;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using IngredientLib.Ingredient.Items;
using KitchenTacoTime.Customs.Tacos.Chicken;
using KitchenTacoTime.Customs.Tacos.Pork;

// Namespace should have "Kitchen" in the beginning
namespace KitchenMyMod
{
    public class Mod : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "Madvion.PlateUp.TacoPatch";
        public const string MOD_NAME = "Taco Patch";
        public const string MOD_VERSION = "0.1.0";
        public const string MOD_AUTHOR = "Madvion";
        public const string MOD_GAMEVERSION = ">=1.1.4";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.3" current and all future
        // e.g. ">=1.1.3 <=1.2.3" for all from/until

        // Boolean constant whose value depends on whether you built with DEBUG or RELEASE mode, useful for testing
#if DEBUG
        public const bool DEBUG_MODE = true;
#else
        public const bool DEBUG_MODE = false;
#endif

        public const string VFX_NAME = "Freezer Vapour";

        public static AssetBundle Bundle;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void OnInitialise()
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
        }

        private void AddGameData()
        {
            LogInfo("Attempting to register game data...");

            // AddGameDataObject<MyCustomGDO>();

            LogInfo("Done loading game data.");
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            // TODO: Uncomment the following if you have an asset bundle.
            // TODO: Also, make sure to set EnableAssetBundleDeploy to 'true' in your ModName.csproj

            LogInfo("Attempting to load asset bundle...");
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).First();
            LogInfo("Done loading asset bundle.");

            // Register custom GDOs
            AddGameData();

            // Perform actions when game data is built
            Events.BuildGameDataEvent += delegate (object s, BuildGameDataEventArgs args)
            {
                if (!args.firstBuild) return;
                FixFishProvider();
                SetupFishTaco();
                SetupFishTacoPlated();
                SetupChickenTaco();
                SetupChickenTacoPlated();
                SetupPorkTaco();
                SetupPorkTacoPlated();

            };
        }

        private void FixFishProvider()
        {
            Appliance provider = GetModdedGDO<Appliance, FilletProvider>();
            provider.Properties.Clear();
            provider.Properties.Add(KitchenPropertiesUtils.GetUnlimitedCItemProvider(KitchenLib.References.ItemReferences.FishFillet));
            provider.Prefab = Bundle.LoadAsset<GameObject>("FilletProvider");
            GameObject freezer = provider.Prefab.GetChild("Freezer/Freezer");
            freezer.ApplyMaterialToChild("Body", "Metal- Shiny");
            freezer.ApplyMaterialToChild("PIping", "Plastic - Grey");

            GameObject Fillet = provider.Prefab.GetChild("Fish - Fillet Chopped");
            Fillet.ApplyMaterialToChild("Fish Fillet", "Raw Fillet Meat");
            Fillet = provider.Prefab.GetChild("Fish - Fillet Chopped 1");
            Fillet.ApplyMaterialToChild("Fish Fillet", "Raw Fillet Meat");
            Fillet = provider.Prefab.GetChild("Fish - Fillet Chopped 2");
            Fillet.ApplyMaterialToChild("Fish Fillet", "Raw Fillet Meat");
            Fillet = provider.Prefab.GetChild("Fish - Fillet Chopped 3");
            Fillet.ApplyMaterialToChild("Fish Fillet", "Raw Fillet Meat");

            VisualEffectAsset asset = Resources.FindObjectsOfTypeAll<VisualEffectAsset>().Where(vfx => vfx.name == VFX_NAME).FirstOrDefault();
            if (asset != default)
            {
                VisualEffect vfx = provider.Prefab.GetChild("Freezer/Effect").AddComponent<VisualEffect>();
                vfx.visualEffectAsset = asset;
            }
        }
        private void SetupFishTaco()
        {
            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Fish>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("fish tacos");
            GameObject tacos = ft.Prefab.GetChild("Fish Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Fish", "Cooked Fillet Meat");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Tail", "Raw Fillet Skin");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Head", "Raw Fillet Skin");

            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
        }
        private void SetupFishTacoPlated()
        {

            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Fish_Plated>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("fish tacos - plated");
            GameObject tacos = ft.Prefab.GetChild("Fish Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Fish","Cooked Fillet Meat");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Tail","Raw Fillet Skin");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Head", "Raw Fillet Skin");

            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder", "Plate", "Plate - Ring");
        }

        private void SetupChickenTaco()
        {
            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Chicken>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("chicken tacos");
            GameObject tacos = ft.Prefab.GetChild("Chicken Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 1", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 2", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 3", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 4", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
        }
        private void SetupChickenTacoPlated()
        {

            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Chicken_Plated>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("chicken tacos - plated");
            GameObject tacos = ft.Prefab.GetChild("Chicken Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 1", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 2", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 3", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Chicken 4", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Cooked Chicken\""].name);

            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder", "Plate", "Plate - Ring");
        }

        private void SetupPorkTaco()
        {
            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Pork>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("pork tacos");
            GameObject tacos = ft.Prefab.GetChild("Pork Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
        }
        private void SetupPorkTacoPlated()
        {

            ItemGroup ft = GetModdedGDO<ItemGroup, Tacos_Pork_Plated>();
            ft.Prefab = Bundle.LoadAsset<GameObject>("pork tacos - plated");
            GameObject tacos = ft.Prefab.GetChild("Pork Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
            }

            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder","Plate", "Plate - Ring");
        }

        internal class ComponentAccesserUtil : ItemGroupView
        {
            private static FieldInfo componentGroupField = ReflectionUtils.GetField<ItemGroupView>("ComponentGroups");
            private static FieldInfo componentLabelsField = ReflectionUtils.GetField<ItemGroupView>("ComponentLabels");



            public static void AddComponent(ItemGroupView viewToAddTo, params (Item item, GameObject gameObject)[] addedGroups)
            {
                List<ComponentGroup> componentGroups = componentGroupField.GetValue(viewToAddTo) as List<ComponentGroup>;

                foreach (var group in addedGroups)
                {
                    componentGroups.Add(new()
                    {
                        Item = group.item,
                        GameObject = group.gameObject
                    });
                }
                componentGroupField.SetValue(viewToAddTo, componentGroups);


            }
            public static void AddColourBlindLabels(ItemGroupView viewToAddTo, params (Item item, string colourBlindName)[] addedGroups)
            {
                List<ColourBlindLabel> componentGroups = componentLabelsField.GetValue(viewToAddTo) as List<ColourBlindLabel>;
                foreach (var group in addedGroups)
                {
                    componentGroups.Add(new()
                    {
                        Item = group.item,
                        Text = group.colourBlindName
                    });
                }
                componentLabelsField.SetValue(viewToAddTo, componentGroups);
            }
        }

        private static T1 GetModdedGDO<T1, T2>() where T1 : GameDataObject
        {
            return (T1)GDOUtils.GetCustomGameDataObject<T2>().GameDataObject;
        }
        private static T GetExistingGDO<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id);
        }
        internal static T Find<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id) ?? (T)GDOUtils.GetCustomGameDataObject(id)?.GameDataObject;
        }

        internal static T Find<T, C>() where T : GameDataObject where C : CustomGameDataObject
        {
            return GDOUtils.GetCastedGDO<T, C>();
        }

        internal static T Find<T>(string modName, string name) where T : GameDataObject
        {
            return GDOUtils.GetCastedGDO<T>(modName, name);
        }
        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
