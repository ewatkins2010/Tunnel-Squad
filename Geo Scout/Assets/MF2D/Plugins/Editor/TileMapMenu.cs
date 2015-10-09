/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEditor;

public class TileMapMenu : Editor {

	// Editor window
	[MenuItem ("Window/TileMap editor",false,1)]
	static void TiledMapFactoryWindow(){
		EditorWindow.GetWindow<TileMapEditor>(false, "Map editor", true);
	}

	[MenuItem ("Tools/Hedgehog Team/TileMap Factory/Create map",false,1)]
	static void TiledMapFactoryToolsCreate(){
		GameObject map = new GameObject("NewTileMap");
		map.AddComponent<TileMap>();

		GameObject mapPrefab = PrefabUtility.CreatePrefab("Assets/NewTileMap.prefab",map,ReplacePrefabOptions.ReplaceNameBased);
		DestroyImmediate( map);

		TileMapEditor win = EditorWindow.GetWindow<TileMapEditor>(false, "Map editor", true);
		win.map = (TileMap)mapPrefab.GetComponent<TileMap>();

		Selection.activeGameObject = mapPrefab;
	}
}
