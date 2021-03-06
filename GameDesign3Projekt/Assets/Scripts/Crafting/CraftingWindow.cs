using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingWindow : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private CraftingRecipeUI recipeUIPrefab;
    [SerializeField] private RectTransform recipeUIParent;
    [SerializeField] private List<CraftingRecipeUI> craftingRecipeUIs;

    [Header("Public Variables")]
    public ItemContainer ItemContainer;
    public List<CraftingRecipe> CraftingRecipes;

    public event Action<BaseItemSlot> OnPointerEnterEvent;
    public event Action<BaseItemSlot> OnPointerExitEvent;

    private void OnValidate()
    {
        Init();
    }

    private void Start()
    {
        Init();

        foreach (CraftingRecipeUI craftingRecipeUI in craftingRecipeUIs)
        {
            craftingRecipeUI.OnPointerEnterEvent += OnPointerEnterEvent;
            craftingRecipeUI.OnPointerExitEvent += OnPointerExitEvent;
        }
    }

    private void Init()
    {
        recipeUIParent.GetComponentsInChildren<CraftingRecipeUI>(includeInactive: true, result: craftingRecipeUIs);
        UpdateCraftingRecipes();
    }

    private void UpdateCraftingRecipes()
    {
        for (int i = 0; i < CraftingRecipes.Count; i++)
        {
            if (craftingRecipeUIs.Count == i)
            {
                craftingRecipeUIs.Add(Instantiate(recipeUIPrefab, recipeUIParent, false));
            }
            else if (craftingRecipeUIs[i] == null)
            {
                craftingRecipeUIs[i] = Instantiate(recipeUIPrefab, recipeUIParent, false);
            }

            craftingRecipeUIs[i].ItemContainer = ItemContainer;
            craftingRecipeUIs[i].CraftingRecipe = CraftingRecipes[i];
        }

        for (int i = CraftingRecipes.Count; i < craftingRecipeUIs.Count; i++)
        {
            craftingRecipeUIs[i].CraftingRecipe = null;
        }
    }
}
