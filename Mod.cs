﻿using KitchenLib;
using KitchenLib.Event;
using KitchenMods;
using System.Reflection;
using UnityEngine;
using KitchenData;
using KitchenLib.Customs;
using KitchenLib.Utils;
using System.Linq;
using UnityEngine.VFX;
using KitchenLib.Colorblind;
using Kitchen;
using System.Collections.Generic;
using System;
using KitchenLib.References;

// Namespace should have "Kitchen" in the beginning
namespace KitchenPatchmaster
{
    public class Mod : BaseMod, IModSystem
    {
        // GUID must be unique and is recommended to be in reverse domain name notation
        // Mod Name is displayed to the player and listed in the mods menu
        // Mod Version must follow semver notation e.g. "1.2.3"
        public const string MOD_GUID = "Madvion.PlateUp.Patchmaster";
        public const string MOD_NAME = "Patchmaster";
        public const string MOD_VERSION = "0.1.9";
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
                ItemGroup fishTacoPlated = (ItemGroup)GDOUtils.GetCustomGameDataObject(834629987)?.GameDataObject;
                if (fishTacoPlated != null)
                {
                    FixFishProvider();
                    SetupFishTaco();
                    SetupFishTacoPlated(args.gamedata);
                    SetupChickenTaco();
                    SetupChickenTacoPlated(args.gamedata);
                    SetupPorkTaco();
                    SetupPorkTacoPlated(args.gamedata);
                    SetupSteakTaco();
                    SetupSteakTacoPlated(args.gamedata);
                    FishTacoShellProvider();
                }
                AdjustLocoMoco();
                RemoveCroquettePot();
                //MakeGrilledToppingsActive();
                //RemoveModdedKitchenSidesCB();
            };
        }
        
        private void RemoveModdedKitchenSidesCB()
        {
            Item baconSide = (Item)GDOUtils.GetCustomGameDataObject(-1802504995)?.GameDataObject;
            if (baconSide != null)
            {
                UnityEngine.Object.Destroy(baconSide.Prefab.GetChild("Colour Blind"));
                Item cornbreadSide = (Item)GDOUtils.GetCustomGameDataObject(962219110)?.GameDataObject;
                UnityEngine.Object.Destroy(cornbreadSide.Prefab.GetChild("Colour Blind"));
                Item macAndCheese = (Item)GDOUtils.GetCustomGameDataObject(1052987967)?.GameDataObject;
                UnityEngine.Object.Destroy(macAndCheese.Prefab.GetChild("Colour Blind"));
                Item milkSide = (Item)GDOUtils.GetCustomGameDataObject(-142832655)?.GameDataObject;
                UnityEngine.Object.Destroy(milkSide.Prefab.GetChild("Colour Blind"));
            }
        }
        
        private void RemoveCroquettePot()
        {
            Dish Croquette = (Dish)GDOUtils.GetCustomGameDataObject(-1708408715)?.GameDataObject;
            if (Croquette != null)
            {
                Croquette.MinimumIngredients.Remove((Item)GDOUtils.GetExistingGDO(ItemReferences.Pot));
                Croquette.IsUnlockable = true;  
            }
        }
        private void MakeGrilledToppingsActive()
        {
            Dish toppingsCard = (Dish)GDOUtils.GetCustomGameDataObject(-444144252)?.GameDataObject;
            if (toppingsCard != null)
            {
                toppingsCard.IsUnlockable = true;
                ItemGroup platedGrilledToppings = (ItemGroup)GDOUtils.GetCustomGameDataObject(-312817740)?.GameDataObject;
                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced/Liquid").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato Flesh") };
                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced/Inner").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato Flesh 2") };
                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced/Skin").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato") };

                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced (1)/Liquid").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato Flesh") };
                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced (1)/Inner").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato Flesh 2") };
                platedGrilledToppings.Prefab.GetChild("Tomato/Tomato Sliced (1)/Skin").GetComponent<MeshRenderer>().materials = new Material[] { MaterialUtils.GetExistingMaterial("Tomato") };
            }

        }

        private void AdjustLocoMoco()
        {
            ItemGroup LM = (ItemGroup)GDOUtils.GetCustomGameDataObject(-180553593)?.GameDataObject;
            if (LM != null)
            {
                LM.Prefab.GetChild("BurgerPatty").transform.localPosition = new Vector3(-0.1084f, -0.0284f, -0.1193f);
                LM.Prefab.GetChild("BurgerPatty").transform.rotation = Quaternion.Euler(new Vector3(23.1885f, 171.9505f, 188.0495f));
                LM.Prefab.GetChild("BurgerPatty").transform.localScale = new Vector3(125f, 125f, 125f);
            }
            ItemGroup ES = (ItemGroup)GDOUtils.GetCustomGameDataObject(451680149)?.GameDataObject;
            if (ES != null)
            {
                UnityEngine.Object.Destroy(ES.Prefab.GetChild("Colour Blind"));
                UnityEngine.Object.Destroy(ES.Prefab.GetChild("Colour Blind"));
            }
        }

        private void FixFishProvider()
        {
            Appliance provider = (Appliance)GDOUtils.GetCustomGameDataObject(-1551076761)?.GameDataObject;
            provider.Name = "Fish Fillet Provider";
            provider.IsPurchasable = true;
            provider.SellOnlyAsDuplicate = true;
            provider.Description = "Provides Fish Fillets";
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
        private void FishTacoShellProvider()
        {
            
            Appliance provider = (Appliance)GDOUtils.GetCustomGameDataObject(-769835599)?.GameDataObject;
            provider.IsPurchasable = true;
            provider.SellOnlyAsDuplicate = true;
            provider.Name = "Taco Shells Provider";
            provider.Description = "Provides Taco Shells";
            provider.Prefab = Bundle.LoadAsset<GameObject>("provider - shells");
            for(int i = 1; i < 4; i++)
            {
                provider.Prefab.ApplyMaterialToChild($"Crate/Crate{i}", "Wood - Default");
            }
            for (int i = 1; i < 8; i++)
            {
                provider.Prefab.ApplyMaterialToChild($"Shell{i}", "Pie - Mushroom");
            }
        }
        private void SetupFishTaco()
        {
            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(-590894475)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("fish tacos");
            GameObject tacos = ft.Prefab.GetChild("Fish Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Fish", "Cooked Fillet Meat");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Tail", "Raw Fillet Skin");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Head", "Raw Fillet Skin");

            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic - Blue");
            
            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if(grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, (ft, "FT"));
        }
        private void SetupFishTacoPlated(GameData pBuildGameData)
        {

            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(834629987)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("fish tacos - plated");
            GameObject tacos = ft.Prefab.GetChild("Fish Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Fish","Cooked Fillet Meat");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Tail","Raw Fillet Skin");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/FishVisual/Head", "Raw Fillet Skin");

            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic - Blue");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder", "Plate", "Plate - Ring");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, ((ItemGroup)GDOUtils.GetCustomGameDataObject(-590894475)?.GameDataObject, "FT"));
            ItemGroupViewUtils.AddSideContainer(pBuildGameData, ft, grp);

        }

        private void SetupChickenTaco()
        {
            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(1163340210)?.GameDataObject;
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
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Clothing Soft Pink");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, (ft, "CT"));
        }
        private void SetupChickenTacoPlated(GameData pBuildGameData)
        {

            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(423194712)?.GameDataObject;
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
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Clothing Soft Pink");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder", "Plate", "Plate - Ring");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, ((ItemGroup)GDOUtils.GetCustomGameDataObject(1163340210)?.GameDataObject, "CT"));
            ItemGroupViewUtils.AddSideContainer(pBuildGameData, ft, grp);


        }

        private void SetupPorkTaco()
        {
            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(735743015)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("pork tacos");
            GameObject tacos = ft.Prefab.GetChild("Pork Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic - Red");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, (ft, "PT"));

        }
        private void SetupPorkTacoPlated(GameData pBuildGameData)
        {

            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(59314697)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("pork tacos - plated");

            GameObject tacos = ft.Prefab.GetChild("Pork Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/pork", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon\""].name, CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Bacon Fat\""].name);
            }

            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", "Plastic - Red");
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder","Plate", "Plate - Ring");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, ((ItemGroup)GDOUtils.GetCustomGameDataObject(735743015)?.GameDataObject, "PT"));
            ItemGroupViewUtils.AddSideContainer(pBuildGameData, ft, grp);

        }

        private void SetupSteakTaco()
        {
            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(1939936514)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("steak tacos");
            GameObject tacos = ft.Prefab.GetChild("Steak Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Steak", "Soup - Meat", "Meat Piece Cooked");
            }
            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Chocolate Dark\""].name);

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, (ft, "ST"));
        }
        private void SetupSteakTacoPlated(GameData pBuildGameData)
        {

            ItemGroup ft = (ItemGroup)GDOUtils.GetCustomGameDataObject(1362052740)?.GameDataObject;
            ft.Prefab = Bundle.LoadAsset<GameObject>("steak tacos - plated");
            GameObject tacos = ft.Prefab.GetChild("Steak Tacos");
            for (int i = 1; i <= 3; i++)
            {
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}", "Pie - Mushroom");
                MaterialUtils.ApplyMaterialToChild(tacos, $"Shell{i}/Steak", "Soup - Meat", "Meat Piece Cooked");
            }

            MaterialUtils.ApplyMaterialToChild(tacos, "Shell_Holder", CustomMaterials.CustomMaterialsIndex["IngredientLib - \"Chocolate Dark\""].name);
            MaterialUtils.ApplyMaterialToChild(ft.Prefab, "Plate/Plate/Cylinder", "Plate", "Plate - Ring");

            ItemGroupView grp = ft.Prefab.GetComponent<ItemGroupView>();
            if (grp == null) grp = ft.Prefab.AddComponent<ItemGroupView>();
            GameObject clonedColourBlind = ColorblindUtils.cloneColourBlindObjectAndAddToItem(ft);
            ColorblindUtils.setColourBlindLabelObjectOnItemGroupView(grp, clonedColourBlind);
            ComponentAccesserUtil.AddColourBlindLabels(grp, ((ItemGroup)GDOUtils.GetCustomGameDataObject(1939936514)?.GameDataObject, "ST"));
            ItemGroupViewUtils.AddSideContainer(pBuildGameData, ft, grp);

        }

        internal class ComponentAccesserUtil : ItemGroupView
        {
            private static FieldInfo componentGroupField = ReflectionUtils.GetField<ItemGroupView>("ComponentGroups");
            private static FieldInfo componentLabelsField = ReflectionUtils.GetField<ItemGroupView>("ComponentLabels");
            private static FieldInfo labelsField = ReflectionUtils.GetField<ItemGroup>("Labels");

            public static void AddComponent(ItemGroupView viewToAddTo, params (Item item, GameObject gameObject)[] addedGroups)
            {
                List<ComponentGroup> componentGroups = componentGroupField.GetValue(viewToAddTo) as List<ComponentGroup>;
                Mod.LogWarning("Retrieved component groups from reflection utils");
                foreach (var group in addedGroups)
                {
                    Mod.LogWarning($"Adding component {group.item} with game object {group.gameObject}");
                    if(componentGroups == null) componentGroups = new List<ComponentGroup>();
                    componentGroups.Add(new()
                    {
                        Item = group.item,
                        GameObject = group.gameObject
                    });
                }
                Mod.LogWarning($"Setting value");

                componentGroupField.SetValue(viewToAddTo, componentGroups);
                Mod.LogWarning($"Set value, finishing");



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

            public static void AddLabels(ItemGroup groupToAddTo, params (Item item, string colourBlindName)[] addedGroups)
            {
                List<ColourBlindLabel> labels = labelsField.GetValue(groupToAddTo) as List<ColourBlindLabel>;
                foreach (var group in addedGroups)
                {
                    labels.Add(new()
                    {
                        Item = group.item,
                        Text = group.colourBlindName
                    });
                }
                componentLabelsField.SetValue(groupToAddTo, labels);
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
