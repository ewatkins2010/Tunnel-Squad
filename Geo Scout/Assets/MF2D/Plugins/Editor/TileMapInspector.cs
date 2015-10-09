/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapInspector : Editor {


	public override void OnInspectorGUI(){

		if (HTGUILayout.Button("Open map editor",Color.green,Screen.width-25)){
			TileMapEditor win = EditorWindow.GetWindow<TileMapEditor>(false, "Tile Map editor", true);
			win.map = (TileMap)target;
		}
	}
}
