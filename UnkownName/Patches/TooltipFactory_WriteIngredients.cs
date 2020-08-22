﻿using System.Collections.Generic;
using HarmonyLib;

namespace UnKnownName.Patches
{
    [HarmonyPatch(typeof(TooltipFactory), nameof(TooltipFactory.WriteIngredients))]
    public class TooltipFactory_WriteIngredients
    {
#if SUBNAUTICA

        [HarmonyPostfix]
        public static void Postfix(ITechData data, ref List<TooltipIcon> icons)
        {
	        if (data == null)
	        {
		        return;
	        }
            int ingredientCount = data.ingredientCount;
            for (int i = 0; i < ingredientCount; i++)
            {
                IIngredient ingredient = data.GetIngredient(i);
                TechType techType = ingredient.techType;
                if (!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType);
                    continue;
                }
                if (!CrafterLogic.IsCraftRecipeUnlocked(techType))
                {
                    TooltipIcon icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                    if (icons.Contains(icon))
                    {
                        icons.Remove(icon);
                        var tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = Main.config.UnKnownTitle };
                        icons.Add(tooltipIcon);
                    }
                }
            }
        }

#elif BELOWZERO

        [HarmonyPostfix]
        public static void Postfix(IList<Ingredient> ingredients, ref List<TooltipIcon> icons)
        {
            if (ingredients == null)
            {
                return;
            }

            int ingredientCount = ingredients.Count;
            for(int i = 0; i < ingredientCount; i++)
            {
                TechType techType = ingredients[i].techType;
                if(!KnownTech.Contains(techType) && PDAScanner.ContainsCompleteEntry(techType))
                {
                    KnownTech.Add(techType, true);
                }
                if(!KnownTech.Contains(techType) && GameModeUtils.RequiresBlueprints())
                {
                    TooltipIcon icon = icons.Find((TooltipIcon) => TooltipIcon.sprite == SpriteManager.Get(techType) && TooltipIcon.text.Contains(Language.main.GetOrFallback(TooltipFactory.techTypeIngredientStrings.Get(techType), techType)));
                    if(icons.Contains(icon))
                    {
                        icons.Remove(icon);
                        TooltipIcon tooltipIcon = new TooltipIcon() { sprite = SpriteManager.Get(TechType.None), text = Main.config.UnKnownTitle };
                        icons.Add(tooltipIcon);
                    }
                }
            }
        }

#endif
    }

}