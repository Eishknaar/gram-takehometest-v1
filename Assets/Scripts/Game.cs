using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public static Game Instance { get; private set; }
	public MergableItem DraggableObjectPrefab;
	public GridHandler MainGrid;
	public MixingGridHandler MixingGrid;
	public GameObject SettingsMenu;

	private List<string> ActiveRecipes = new List<string>();

	private void Awake()
	{
		if(Instance == null)
        {
			Instance = this;
        }
        else
        {
			Destroy(gameObject);
        }

		Screen.fullScreen =
			false; // https://issuetracker.unity3d.com/issues/game-is-not-built-in-windowed-mode-when-changing-the-build-settings-from-exclusive-fullscreen

		if (ItemUtils.ItemsMap.Count == 0)
		{
			// load all item definitions
			ItemUtils.InitializeMap();
		}
	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		ReloadLevel(1);
	}

	public void EndGame()
	{
		PlayerPrefs.Save();
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}

	public void ReloadLevel(int difficulty = 1)
	{
		// clear the board
		MainGrid.ClearFullCells();

		// choose new recipes
		ActiveRecipes.Clear();
		ActiveRecipes = MainGrid.recipeRange.Where(recipe => recipe.NodeLinks.Count > 1).Select(recipe => recipe.MainNodeData.NodeGUID).ToList();

		// populate the board
		var emptyCells = MainGrid.GetEmptyCells.ToArray();
		foreach (var cell in emptyCells)
		{
			if (cell.DecideToSpawn())
			{
				var recipeIndex = Random.Range(0, ActiveRecipes.Count());
				var chosenRecipe = ActiveRecipes[recipeIndex];
				var ingredients = ItemUtils.RecipeMap[chosenRecipe].ToArray();
				var ingredient = ingredients[Random.Range(0, ingredients.Count())];
				var item = ItemUtils.ItemsMap[ingredient.NodeGUID];
				cell.SpawnItem(item);
			}
		}
	}
}
