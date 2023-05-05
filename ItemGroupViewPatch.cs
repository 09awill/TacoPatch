using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using KitchenTacoTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static Kitchen.ItemGroupView;

namespace KitchenTacoModFix
{
    
    [HarmonyPatch]
    public class ItemGroupViewPatch
    {
        [HarmonyPatch(typeof(ItemGroupView), "PerformUpdate",  typeof(int), typeof(ItemList))]
        [HarmonyPrefix]
        static bool PerformUpdate_Prefix(int item_id, ItemList components)
        {
            foreach(var item in components)
            {
                Mod.LogWarning($"Item added : {item.ToString()}");
                Mod.LogWarning($"Item ID : {GDOUtils.GetCustomGameDataObject(item_id)}");
            }
            foreach (int item2 in components)
            {
                if (-505249062 == item2)
                {
                    Mod.LogWarning("Component was the same, I should be adding to the colourblind string");
                }
            }
            
            return true;
        }
    }
    
}
