/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// This is the main class of window editor
/// </summary>
[System.Serializable]
public class TileMapEditor : EditorWindow {

	#region Memeber

	#region Public memeber
	public TileMap map;
	#endregion

	#region Private Member
	static Texture2D[] proSkinIcon = new Texture2D[20];
	static Texture2D[] freeSkinIcon = new Texture2D[20];
	
	#region top toolbar
	private int zoom=100;

	// Sorting & order
	private string[] sortingLayersNames;
	private int[] sortingLayersId;
	private int sortingLayerIndex=0;
	private int order=0;

	// snap
	private bool snap=true;
	
	// Resize
	private bool resize=true;

	private bool allLayer;
	private bool allOrder;
	#endregion

	#region rightToolbar
	private Vector2 scrollLeftTool = Vector2.zero;
	private bool showRightToolBar = true;
	private int rightToolBatWidth;
	private List<SortingLayerFilter> layerFilters;
	private Color filterAlpha;
	private bool autoHide;
	private bool mapProperties;
	private bool gridProperties;
	private bool filterProperties;

	private bool tileProperties;
	private int tileLayerIndex;
	private bool tileResize;
	private int tileOrder;
	private float tileScaleX;
	private float tileScaleY;
	private bool tileHaveCollider;
	private Tile.TileColliderType colliderType;
	private int tileLayer;

	private bool generateProperties;
	private bool generateAllLayer;
	private int generatedLayerIndex;

	private int maxTextureSize;
	private string textureName;
	#endregion

	#region left toolbar
	private int toolIndex;
	#endregion

	#region sprite area
	private Vector2 scrollSprite = Vector2.zero;
	private int spriteIndex;
	#endregion

	#region work area
	private Vector2 scrollWork = Vector2.zero;
	private int oldXCell;
	private int oldYCell;

	// Selected
	private List<Vector2> selectedTiles = new List<Vector2>();
	private bool multiSelect;
	private Vector2 startCell;
	private Vector2 endCell;
	private Vector2 overNoResiezCell;

	// Move
	private bool startMoveOnWorkArea;
	private bool wait2Clear;

	// Move selection
	private List<Tile> moveTiles = new List<Tile>();
	private bool startMoveSelection;
	private Vector2 startMovePosition;

	// eyedropper
	private Tile EyedropperTile;
	#endregion

	private Mouse mouse = new Mouse();
	private int tileSelected;

	#endregion

	#endregion

	#region Editor Window
	void OnEnable(){
	
		InitEditor();
	}

	void OnGUI (){

		TopToolBar();

		if (map!=null){
			LeftToolBar();
			RightToolBar();
			DropAreaGUI();
			SpriteArea();
			WorkArea();
		}
		else{
			InitEditor();
		}

		try{
			Repaint();
		}
		catch{}

		mouse.UpdateGlobal(new Rect (39,111,Screen.width-rightToolBatWidth-40,Screen.height-132));

		if (map!=null){
			EditorUtility.SetDirty( map);
		}
	}

	void InitEditor(){
		spriteIndex=-1;
		toolIndex = -1;
		rightToolBatWidth = 200;
		startMoveOnWorkArea = false;
		wait2Clear = false;
		layerFilters = new List<SortingLayerFilter>();
		filterAlpha = new Color(1,1,1,0.1f);
		allLayer = false;
		allOrder = false;
		generateAllLayer = true;
		selectedTiles.Clear();
		resize = true;
		snap = true;
		tileSelected=0;
		tileScaleX=1;
		tileScaleY=1;
		tileResize = false;
		EyedropperTile = null;
		maxTextureSize = 2048;
		textureName = "NewTexture";
		autoHide = true;
	}

	void OnSelectionChange(){
		try{
			TileMap newMap = Selection.activeGameObject.GetComponent<TileMap>();
			if (newMap!=null) {
				InitEditor();
				map = newMap;

			}
		}
		catch{}


	}
	#endregion

	#region Interface
	private void TopToolBar(){  

		GUI.Box( new Rect(0,0,Screen.width,20),"");

		GUILayout.Space(3);
		EditorGUILayout.BeginHorizontal();

		// Sorting layer & order
		sortingLayersNames = GetSortingLayerName();
		sortingLayersId = GetSortingLayerId();

		UpdateSortingLayersFilter();

		// Sorting layer
		EditorGUILayout.LabelField(new GUIContent(GetIcon(8)),GUILayout.Width(18));
		int tmpLayer = EditorGUILayout.Popup(sortingLayerIndex,sortingLayersNames,GUILayout.Width(100));
		if (sortingLayerIndex != tmpLayer){
			oldXCell = -1000;
			oldYCell = -1000;
			sortingLayerIndex = tmpLayer;
			tileSelected = GetSelectionCount();
		   }

		// Order
		GUILayout.Space(15);
		EditorGUILayout.LabelField(new GUIContent(GetIcon(3)),GUILayout.Width(18));
		int newOrder = HTGUILayout.Order( order);
		if (newOrder!=order){
			oldXCell = -1000;
			oldYCell = -1000;
			order=newOrder;
			tileSelected = GetSelectionCount();
		}

		// snap
		GUILayout.Space(15);
		EditorGUILayout.LabelField(new GUIContent(GetIcon(9)),GUILayout.Width(18));
		snap = EditorGUILayout.Toggle("",snap,GUILayout.Width(18));

		// resize
		GUILayout.Space(15);
		EditorGUILayout.LabelField(new GUIContent(GetIcon(10)),GUILayout.Width(18));
		resize = EditorGUILayout.Toggle("",resize,GUILayout.Width(18));

		// space
		GUILayout.Space(15);

		EditorGUILayout.LabelField("All layers",GUILayout.Width(58));
		allLayer = EditorGUILayout.Toggle("",allLayer,GUILayout.Width(15));

		EditorGUILayout.LabelField("All orders",GUILayout.Width(58));
		allOrder = EditorGUILayout.Toggle("",allOrder,GUILayout.Width(15));

		// zoom
		GUILayout.Space(15);
		EditorGUILayout.LabelField(new GUIContent(GetIcon(4)),GUILayout.Width(20));
		zoom  = (int)GUILayout.HorizontalSlider(zoom,5,200,GUILayout.Width(100));

		
		EditorGUILayout.EndHorizontal();
	}

	private void LeftToolBar(){
		GUI.Box( new Rect(0,111,38,Screen.height-111),"");
		
		Texture2D[] toolBarIcons = new Texture2D[5];
		toolBarIcons[0] = GetIcon(0);
		toolBarIcons[1] = GetIcon(15);
		toolBarIcons[2] = GetIcon(1);
		toolBarIcons[3] = GetIcon(14);
		toolBarIcons[4] = GetIcon(2);

		int tmpToolIndex = GUI.SelectionGrid( new Rect(4,116,30, 150),toolIndex,toolBarIcons ,1);
		if (toolIndex != tmpToolIndex){
			toolIndex = tmpToolIndex;
			oldXCell = -1000;
			oldYCell = -1000;

			if (toolIndex==0 || toolIndex==2 || toolIndex==3 || toolIndex==4){
				tileSelected = 0;
				selectedTiles.Clear();
			}
		}

		// Erase all
		if (GUI.Button( new Rect(4,268,29,29),GetIcon(13))){
			EraseSelectedTiles();
		}

		// Fill
		if (GUI.Button( new Rect(4,315,29,29),GetIcon(12))){
			FillTiles();
		}

		// Rotate right
		if (GUI.Button( new Rect(4,360,29,29),GetIcon(6))){
			RotateTile(90);
		}

		// Rotate left
		if (GUI.Button( new Rect(4,360+32,29,29),GetIcon(5))){
			RotateTile(-90);
		}

	}

	private void RightToolBar(){

		GUI.Box( new Rect(Screen.width-rightToolBatWidth,111,rightToolBatWidth,20),"");

		// Anchor
		if (GUI.Button( new Rect(Screen.width-rightToolBatWidth + 2, 111+2, 16,16),"")){
			showRightToolBar = !showRightToolBar;
			if (showRightToolBar)rightToolBatWidth = 200; else rightToolBatWidth = 23;
		}
		GUI.DrawTexture( new Rect(Screen.width-rightToolBatWidth + 2, 111+2, 14,14), GetIcon(7));
		
		if (showRightToolBar){
		
			GUI.Label( new Rect(Screen.width-rightToolBatWidth + 18, 111+2, 100,14),"Advanced Options");

			scrollLeftTool = GUI.BeginScrollView (new Rect(Screen.width-rightToolBatWidth , 111+20, rightToolBatWidth,Screen.height-132-20),scrollLeftTool, new Rect (0,0, rightToolBatWidth-18, 1000),false,false);

			#region Tile properties
			// Tile properties
			HTGUILayout.FoldOut(ref tileProperties,"Tiles properties (" + tileSelected.ToString() +")"); 

			if (tileSelected ==0){
				GUI.enabled = false;
			}

			int index=-1;
			if (tileProperties && toolIndex==0){
				// Single selection
				if (tileSelected==1 ){
					 index = map.FindTile( new Vector3(selectedTiles[0].x,selectedTiles[0].y,order), sortingLayersId[sortingLayerIndex]);
					if (index>-1){
						tileResize = map.tiles[index].resize;
						tileScaleX = map.tiles[index].scale.x;
						tileScaleY = map.tiles[index].scale.y;
						tileOrder = (int)map.tiles[index].position.z;
						tileLayerIndex = GetSortingLayerIndexById(map.tiles[index].sortingLayer);
						tileHaveCollider = map.tiles[index].haveCollider;
						colliderType = map.tiles[index].colliderType;
						tileLayer = map.tiles[index].layer;
					}
				}


				// Resize to tile
				tileResize = EditorGUILayout.ToggleLeft("Resize to tile",tileResize);
				if (!tileResize){
					EditorGUI.indentLevel++;
					tileScaleX = EditorGUILayout.FloatField("X",tileScaleX, GUILayout.Width( 185));
					tileScaleY = EditorGUILayout.FloatField("Y",tileScaleY, GUILayout.Width( 185));
					EditorGUI.indentLevel--;
				}

				HTGUILayout.DrawSeparatorLine();

				// layer
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Layer",GUILayout.Width(50));
				tileLayerIndex = EditorGUILayout.Popup(tileLayerIndex,sortingLayersNames, GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();

				// Order
				EditorGUILayout.BeginHorizontal();
				tileOrder = HTGUILayout.Order( tileOrder);
				EditorGUILayout.EndVertical();

				HTGUILayout.DrawSeparatorLine();

				// Collider
				tileHaveCollider = EditorGUILayout.ToggleLeft("Collider",tileHaveCollider);
				if (tileHaveCollider){
					EditorGUI.indentLevel++;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("type",GUILayout.Width(50));
					colliderType = (Tile.TileColliderType) EditorGUILayout.EnumPopup("",colliderType, GUILayout.Width(125));
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Layer",GUILayout.Width(50));
					tileLayer = EditorGUILayout.LayerField(tileLayer, GUILayout.Width(125));
					EditorGUILayout.EndHorizontal();

					EditorGUI.indentLevel--;
				}

				HTGUILayout.DrawSeparatorLine();

				if (selectedTiles.Count<2){
					GUI.enabled = false;
					GUI.color = new Color(0,0,0,0);
				}


				// Validate for multi tiles
				if (HTGUILayout.Button("Apply",Color.green,180)){
					foreach (Vector2 cell in selectedTiles){
						foreach(Tile tile in map.tiles){
							
							if (tile.position == new Vector3(cell.x,cell.y,order) && tile.sortingLayer == sortingLayersId[sortingLayerIndex]){
								tile.resize = tileResize;
								tile.scale = new Vector2( tileScaleX,tileScaleY);
								tile.haveCollider = tileHaveCollider;
								tile.colliderType = colliderType;
								tile.layer = tileLayer;

								if (ChangeTileLayerOrOrder( tile.position, sortingLayersId[tileLayerIndex],false)){
									tile.sortingLayer = sortingLayersId[tileLayerIndex];
								}


								if (ChangeTileLayerOrOrder( new Vector3(tile.position.x,tile.position.y,tileOrder), sortingLayersId[tileLayerIndex],false)){
									tile.position = new Vector3(tile.position.x,tile.position.y,tileOrder);
								}
							}
						}
					}
					sortingLayerIndex = tileLayerIndex;
					order = tileOrder;
					map.SetLayerOrder( sortingLayersId[tileLayerIndex].ToString(), order);
				}
				GUI.enabled = true;
				GUI.color = Color.white;


				// Validate for single tile
				if (tileSelected == 1 && index>-1){

					// resize
					map.tiles[index].resize = tileResize;
					map.tiles[index].scale = new Vector2( tileScaleX,tileScaleY);

					// layer
					if (tileLayerIndex != GetSortingLayerIndexById(map.tiles[index].sortingLayer)){
						if (ChangeTileLayerOrOrder( map.tiles[index].position, sortingLayersId[tileLayerIndex])){
							index = map.FindTile( new Vector3(selectedTiles[0].x,selectedTiles[0].y,order), sortingLayersId[sortingLayerIndex]);
							map.tiles[index].sortingLayer = sortingLayersId[tileLayerIndex];
							
							sortingLayerIndex = tileLayerIndex;
							map.SetLayerOrder( sortingLayersId[tileLayerIndex].ToString(), order);
							
						}
					}

					// order
					if (tileOrder != map.tiles[index].position.z){
						if (ChangeTileLayerOrOrder( new Vector3(map.tiles[index].position.x,map.tiles[index].position.y,tileOrder), sortingLayersId[tileLayerIndex])){

							index = map.FindTile( new Vector3(selectedTiles[0].x,selectedTiles[0].y,order), sortingLayersId[sortingLayerIndex]);
							map.tiles[index].position = new Vector3(map.tiles[index].position.x,map.tiles[index].position.y,tileOrder);
							
							order = tileOrder;
							map.SetLayerOrder( sortingLayersId[tileLayerIndex].ToString(), order);
						}
					}

					// Collider
					map.tiles[index].haveCollider = tileHaveCollider;
					map.tiles[index].colliderType = colliderType;
					map.tiles[index].layer = tileLayer;

				}
			}
			GUI.enabled = true;
			#endregion

			#region Sorting Layers
			// soring Layers
			HTGUILayout.FoldOut(ref filterProperties,"Sorting layers");
			if (filterProperties){

				autoHide = EditorGUILayout.ToggleLeft("Auto hide",autoHide);
				for (int i=0;i<sortingLayersNames.Length;i++){
					SortingLayerFilter filter = GetLayerFilter( sortingLayersId[i]);
					if (filter!=null){

						Color backColor = Color.white;

						if (sortingLayerIndex == i) backColor = Color.green; else backColor = Color.white;


						EditorGUILayout.BeginHorizontal();
						if (HTGUILayout.Button("",backColor,20,false,65)){
							oldXCell = -1000;
							oldYCell = -1000;
							sortingLayerIndex = i;
						}
						GUI.backgroundColor = backColor;
						GUILayout.Box("",GUILayout.Height(62), GUILayout.Width(155));
						GUI.backgroundColor = Color.white;
						EditorGUILayout.EndHorizontal();

						Rect lastRect = GUILayoutUtility.GetLastRect();
						lastRect.height = 20;
						lastRect.x += 30;
						lastRect.y += 5;
						lastRect.width = 100;
						filter.show = EditorGUI.ToggleLeft( lastRect,sortingLayersNames[i],filter.show);

						lastRect.x+=15;
						lastRect.y +=20;
						filter.allOrder = EditorGUI.ToggleLeft( lastRect,"All order",filter.allOrder);


						if (!filter.allOrder){
							//lastRect.x+=15;
							lastRect.y +=20;
							lastRect.height = 15;
							lastRect.width = 43;


							if (GUI.Button(lastRect,"-")){
								filter.order--;	
							}
							lastRect.x+=45;
							lastRect.width = 32;

							filter.order = EditorGUI.IntField(lastRect,filter.order);

							lastRect.x+=34;
							lastRect.width = 43;
							if (GUI.Button(lastRect,"+")){
								filter.order++;	
							}

						}

					}
				}
			}
			#endregion

			#region grid properties
			// Grid properties
			HTGUILayout.FoldOut(ref gridProperties,"Grid properties");
			if (gridProperties){

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Grid size", GUILayout.Width(120));
				map.GridSize = EditorGUILayout.IntField("",map.GridSize, GUILayout.Width( 55));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Grid Color", GUILayout.Width(120));
				map.gridColor = EditorGUILayout.ColorField("",map.gridColor,GUILayout.Width(60));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Hide color", GUILayout.Width(120));
				filterAlpha = EditorGUILayout.ColorField("",filterAlpha,GUILayout.Width(60));
				EditorGUILayout.EndHorizontal();				
			}
			#endregion

			#region Map properties
			// Map properties
			HTGUILayout.FoldOut(ref mapProperties,"Map properties");
			if (mapProperties){

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Cell X", GUILayout.Width(120));
				map.CellX = EditorGUILayout.IntField("",map.CellX, GUILayout.Width( 55));
				EditorGUILayout.EndHorizontal();	

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Cell y", GUILayout.Width(120));
				map.CellY = EditorGUILayout.IntField("",map.CellY, GUILayout.Width( 55));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				Color buttonClearColor = Color.red;
				if (wait2Clear)buttonClearColor = Color.gray;

				if (HTGUILayout.Button("Clear",buttonClearColor,70)){
					wait2Clear = !wait2Clear;
				}
				if (wait2Clear){

					if (HTGUILayout.Button("Clear map",Color.red,100)){
						map.tiles.Clear();
						wait2Clear = false;
					}
				}


				EditorGUILayout.EndHorizontal();

			}
			#endregion

			#region Generate properties
			// Generate properties
			HTGUILayout.FoldOut(ref generateProperties,"Generate");
			if (generateProperties){

				generateAllLayer = EditorGUILayout.ToggleLeft("Generate all layers",generateAllLayer);
				if (!generateAllLayer){
					for (int i=0;i<sortingLayersNames.Length;i++){
						SortingLayerFilter filter = GetLayerFilter( sortingLayersId[i]);
						if (filter!=null){
							EditorGUI.indentLevel++;
							filter.generate = EditorGUILayout.ToggleLeft(sortingLayersNames[i], filter.generate);
							EditorGUI.indentLevel--;
						}
					}
				}


				if (HTGUILayout.Button("Generate tilemap",Color.green,180)){
					GenerateTiledMap();
				}

				EditorGUILayout.Space();
				HTGUILayout.DrawSeparatorLine();
				EditorGUILayout.Space();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Layer" ,GUILayout.Width(75));
				generatedLayerIndex = EditorGUILayout.Popup(generatedLayerIndex,sortingLayersNames,GUILayout.Width(100));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Textures size" ,GUILayout.Width(120));
				maxTextureSize = EditorGUILayout.IntField("",maxTextureSize,GUILayout.Width( 55));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Texture name" ,GUILayout.Width(85));
				textureName = EditorGUILayout.TextField("",textureName,GUILayout.Width(90));
				EditorGUILayout.EndHorizontal();
		
				if (HTGUILayout.Button("Generate texture",Color.green,180)){
					if (map.tiles.Count>0){
						GenerateTexture();
					}
				}

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("Rotate tile is not supported");
				HTGUILayout.DrawSeparatorLine();
				EditorGUILayout.Space();

			
			}
			#endregion

			GUI.EndScrollView();

		}

	}
	


	private void SpriteArea(){

		string spriteName="";

		GUI.Box( new Rect(0,20,Screen.width,89),"");

		if (map != null){

			// Scroll view
			float scrollWith = Mathf.Clamp(  70*map.refTiles.Count, Screen.width, Mathf.Infinity);
			scrollSprite = GUI.BeginScrollView (new Rect(0,21,Screen.width,89),scrollSprite, new Rect (0,0, scrollWith, 70),true,false);

			mouse.UpdateLocal(0,scrollWork, false);

			// Select sprite
			if (mouse.leftButton == Mouse.ButtonState.Down && map.refTiles.Count>0){
				spriteIndex =  Mathf.FloorToInt((mouse.position.x + scrollSprite.x/70 ) / (70));

				if (spriteIndex>=  map.refTiles.Count) spriteIndex=-1;

				if (toolIndex==1 || toolIndex==3 || toolIndex==4){
					toolIndex=2;
				}
				EyedropperTile = null;
				oldXCell = -1000;
				oldYCell = -1000;
			}

			// Remove sprite
			if (mouse.rightButton == Mouse.ButtonState.Up && map.refTiles.Count>0){
				int oldSpritindex = spriteIndex;
				spriteIndex =  Mathf.FloorToInt((mouse.position.x + scrollSprite.x/70 ) / (70));

				if (spriteIndex<map.refTiles.Count){
					map.refTiles[spriteIndex].isWait2Delete = true;
				}
				else{
					spriteIndex = oldSpritindex;
				}

			}

			// Draw reference sprite
			int x=0;
			int i=0;
			while (i<map.refTiles.Count){ 

				Tile tile = map.refTiles[i];

				Rect rect = new Rect(x,5,64,64);
				Color color = Color.black;
				switch (tile.tileType){
					case Tile.TileType.ComplexeSprite:
						color = new Color(0,0,0.5f);
						break;
					case Tile.TileType.PrefabSprite:
						color = new Color(0.5f,0,0);
						break;
				}
				if (i==spriteIndex){
					color = Color.green;
				}

				if (tile.GetTexture() != null){
					HTGUILayout.DrawTextureRectPreview( rect,tile.GetUVRect(),tile.GetTexture(),color);

					if (tile.isWait2Delete){
						rect.x +=3;
						rect.y += 22;
						rect.width = 20;
						rect.height = 20;
						if (GUI.Button( rect,"X")){
							map.refTiles[i].isWait2Delete = false;
						}
						rect.x += 22;
						rect.width = 36;
						GUI.backgroundColor = Color.red;
						if (GUI.Button( rect,"Del")){
							map.refTiles.RemoveAt(i);

							// On supprimer les tiles correspondant
							for (int t=0;t<map.tiles.Count;t++){
								if (map.tiles[t].tileRefIndex == i){
									map.tiles[t] = null;
									map.tiles.RemoveAt(t);
									t--;
								}
							}
							spriteIndex = -1;
						}
						GUI.backgroundColor = Color.white;
					}

					GUI.color = Color.green;
					if (rect.Contains( mouse.position) ){
						spriteName = tile.name;
					}
					GUI.color = Color.white;

					x+= 69;

				}
				else{
					map.refTiles.RemoveAt(i);
					for (int t=0;t<map.tiles.Count;t++){
						if (map.tiles[t].tileRefIndex == i){
							map.tiles[t] = null;
							map.tiles.RemoveAt(t);
							t--;
						}
					}
					spriteIndex = -1;
					toolIndex = 0;

				}
				i++;
			}
			GUI.EndScrollView();


			GUIStyle labelStyle =  new GUIStyle( EditorStyles.label);
			labelStyle.fontSize = 12;
			labelStyle.fontStyle = FontStyle.Bold;
			labelStyle.onActive.textColor = Color.black;
			labelStyle.onFocused.textColor = Color.black;
			labelStyle.onHover.textColor = Color.black;
			labelStyle.onNormal.textColor = Color.black;
			labelStyle.active.textColor = Color.black;
			labelStyle.focused.textColor = Color.black;
			labelStyle.hover.textColor = Color.black;
			labelStyle.normal.textColor = Color.black;

			EditorGUI.LabelField( new Rect(mouse.position.x+20 - scrollSprite.x, mouse.position.y+10,300,20), spriteName, labelStyle);

			labelStyle.fontSize = 12;
			labelStyle.onActive.textColor = Color.white;
			labelStyle.onFocused.textColor = Color.white;
			labelStyle.onHover.textColor = Color.white;
			labelStyle.onNormal.textColor = Color.white;
			labelStyle.active.textColor = Color.white;
			labelStyle.focused.textColor = Color.white;
			labelStyle.hover.textColor = Color.white;
			labelStyle.normal.textColor = Color.white;

			EditorGUI.LabelField( new Rect(mouse.position.x+21 - scrollSprite.x, mouse.position.y+11,300,20), spriteName, labelStyle);
		}


	}
	
	private void WorkArea(){

		Color gridColor = map.gridColor;
		float gridSize = map.GridSize;

		Vector2 scrollSize = new Vector2(map.CellX,map.CellY) * gridSize * zoom/100f;
		float cellSize = gridSize * zoom/100f;
	

		GUI.Box(new Rect (37,111,Screen.width-rightToolBatWidth-40,Screen.height-132),"");
		scrollWork = GUI.BeginScrollView (new Rect (39,111,Screen.width-rightToolBatWidth-40,Screen.height-132),scrollWork, new Rect (0, 0, scrollSize.x*1.5f, scrollSize.y*1.5f));
			
			mouse.UpdateLocal(cellSize,scrollWork);

			// Zoom
			if (mouse.scrollWheel){

				float zx = Screen.width - (mouse.tiledPosition.x * cellSize);
				float zy = Screen.height - (mouse.tiledPosition.y * cellSize);

				zoom += Mathf.FloorToInt( mouse.wheelDelta*-1);
				zoom = Mathf.Clamp( zoom, 5,200);

				// Compute the new tile under mouse;
				scrollSize = new Vector2(map.CellX,map.CellY) * gridSize * zoom/100f;
				cellSize = gridSize * zoom/100f;

				float x2= (Screen.width - (mouse.tiledPosition.x * cellSize));
				float y2 = Screen.height - (mouse.tiledPosition.y * cellSize);
				scrollWork.x += (x2-zx)*-1;
				scrollWork.y += (y2-zy)*-1;
			}


			// Move
			if (mouse.wheelButton == Mouse.ButtonState.Down){
				startMoveOnWorkArea = true;
			}
			else if (mouse.wheelButton == Mouse.ButtonState.Pressed && startMoveOnWorkArea){
				scrollWork -= mouse.delta;
			}

			// Eyedropper
			if (mouse.rightButton == Mouse.ButtonState.Up && toolIndex ==2){
				toolIndex=3;
			}

			// Draw map
			DrawMap(cellSize);

			// grid
			DrawGrid(gridSize,cellSize,gridColor);


			// Action
			switch(toolIndex ){
				case 0:
					EyedropperTile = null;
					SelectCell(cellSize);
					break;
				case 1:
					EyedropperTile = null;
					MoveSelect(cellSize);
					break;
				case 2:
					if (spriteIndex==-1 && map.refTiles.Count>0) spriteIndex=0;
					PaintTile(cellSize);
					break;
				case 3:
					EyeDropper(cellSize);
					break;
				case 4:
					EyedropperTile = null;
					EraseSimpleTile(cellSize);
					break;
				default:
					oldXCell = -1000;
					oldYCell = -1000;
					spriteIndex = -1;
				break;
			}
			


		GUI.EndScrollView();

	}

	private void DropAreaGUI (){
		
		if (map != null){
			Event evt = Event.current;
			
			switch (evt.type) {
			case EventType.DragUpdated:
			case EventType.DragPerform:
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
				if (evt.type == EventType.DragPerform) {
					
					foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences) {
						DoDrop(draggedObject);
					}
				}
				break;
			}
		}
	}
	#endregion

	#region Tools
	// Selection
	private void SelectCell(float cellSize){
		
		int xCell = (int)mouse.tiledPosition.x;
		int yCell = (int)mouse.tiledPosition.y;
		
		if (mouse.rightButton == Mouse.ButtonState.Up){
			selectedTiles.Clear();
			tileSelected = 0;
		}
		
		// Simple selection
		if (!Event.current.shift) {
			
			if (multiSelect){
				
				Vector2 realStartCell = Vector2.zero;
				Vector2 realEndCell = Vector2.zero;
				
				if (startCell.x < endCell.x){
					realStartCell.x = startCell.x; 
					realEndCell.x = endCell.x;
				}
				else{
					realStartCell.x = endCell.x;
					realEndCell.x = startCell.x;
				}
				
				if (startCell.y < endCell.y){
					realStartCell.y = startCell.y; 
					realEndCell.y = endCell.y;
				}
				else{
					realStartCell.y = endCell.y;
					realEndCell.y = startCell.y;
				}
				
				for(float x=realStartCell.x;x<=realEndCell.x;x++){
					for(float y=realStartCell.y;y<=realEndCell.y;y++){
						if (CellSelectdIndex(x,y)==-1 && x>=0 & y>=0 && x< map.CellX && y<map.CellY){
							selectedTiles.Add( new Vector2(x,y));
						}
					}
				}
				tileSelected = GetSelectionCount();
			}
			
			startCell = Vector2.one*-1;
			endCell= Vector2.one*-1;
			if((mouse.leftButton == Mouse.ButtonState.Pressed    && xCell>=0 && yCell>=0 && (xCell != oldXCell || yCell != oldYCell)) || (mouse.leftButton == Mouse.ButtonState.Down && xCell>=0 && yCell>=0)){

				if (overNoResiezCell.x!=-1 && overNoResiezCell.y !=-1 ){
					if (!selectedTiles.Contains( overNoResiezCell)){
						selectedTiles.Add( overNoResiezCell);
						tileSelected = GetSelectionCount();
					}
				}
				else{
					if (!selectedTiles.Contains( new Vector2(xCell,yCell)) && xCell<map.CellX && yCell<map.CellY){
						selectedTiles.Add( new Vector2(xCell,yCell));
						tileSelected = GetSelectionCount();
					}
				}
				oldXCell = xCell;
				oldYCell = yCell;
			}
			
			multiSelect= false;
		}
		else{
			multiSelect = true;

			if (mouse.leftButton == Mouse.ButtonState.Down){
				startCell = new Vector2( xCell,yCell);
				endCell =  new Vector2( xCell,yCell);
			}

			if (mouse.leftButton == Mouse.ButtonState.Pressed){
				endCell = new Vector2(xCell, yCell);
				
			}
			
			Vector2 realStartCell = Vector2.zero;
			Vector2 realEndCell = Vector2.zero;
			
			if (startCell.x < endCell.x){
				realStartCell.x = startCell.x; 
				realEndCell.x = endCell.x+1;
			}
			else{
				realStartCell.x = endCell.x;
				realEndCell.x = startCell.x+1;
			}
			
			if (startCell.y < endCell.y){
				realStartCell.y = startCell.y; 
				realEndCell.y = endCell.y+1;
			}
			else{
				realStartCell.y = endCell.y;
				realEndCell.y = startCell.y+1;
			}
			
			GUI.color = new Color(0,1,0,0.8f);
			GUI.DrawTexture( new Rect(realStartCell.x * cellSize , realStartCell.y * cellSize, (realEndCell.x * cellSize )- (realStartCell.x * cellSize ),1), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture( new Rect(realStartCell.x * cellSize , realEndCell.y * cellSize, (realEndCell.x * cellSize )- (realStartCell.x * cellSize ),1), EditorGUIUtility.whiteTexture);
			
			GUI.DrawTexture( new Rect(realEndCell.x * cellSize , realStartCell.y * cellSize,1, (realEndCell.y * cellSize )- (realStartCell.y * cellSize )), EditorGUIUtility.whiteTexture);
			GUI.DrawTexture( new Rect(realStartCell.x * cellSize , realStartCell.y * cellSize,1,(realEndCell.y * cellSize )- (realStartCell.y * cellSize )), EditorGUIUtility.whiteTexture);
			
			GUI.color = new Color(0,1,0,0.3f);
			GUI.DrawTexture( new Rect(realStartCell.x * cellSize , realStartCell.y * cellSize, (realEndCell.x * cellSize )- (realStartCell.x * cellSize ),(realEndCell.y * cellSize )- (realStartCell.y * cellSize )), EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
			
		}
		
		DrawSelection(cellSize);
	}
	private int CellSelectdIndex(float x, float y){
		int result = selectedTiles.FindIndex(
			delegate(Vector2 v)
			{
			return  v.x == x && v.y == y;
		}
		);
		
		return result;
	}


	// Move
	private void MoveSelect(float cellSize){
		DrawSelection(cellSize);

		if (mouse.rightButton == Mouse.ButtonState.Up){
			toolIndex=0;
			return;
		}

		if (mouse.leftButton == Mouse.ButtonState.Down){
			startMoveSelection = true;
			startMovePosition = mouse.tiledPosition;

			moveTiles.Clear();

			foreach (Vector2 cell in selectedTiles){
				if (!allLayer && !allOrder){
					int index = map.FindTile( new Vector3(cell.x,cell.y,order), sortingLayersId[sortingLayerIndex]);
					if (index>-1){
						moveTiles.Add( map.tiles[index]);
					}
				}

				if (!allLayer && allOrder){
					
					for(int i=0; i<map.tiles.Count;i++){
						if (cell.x == map.tiles[i].position.x && cell.y == map.tiles[i].position.y && map.tiles[i].sortingLayer == sortingLayersId[sortingLayerIndex]){
							moveTiles.Add( map.tiles[i]);
						}
					}
				}

				if (allLayer){
					for(int i=0; i<map.tiles.Count;i++){
						if (cell.x == map.tiles[i].position.x && cell.y == map.tiles[i].position.y ){
							moveTiles.Add( map.tiles[i]);
						}
					}
				}

			}

		}

		if (startMoveSelection){
			Vector2 realDelta = Vector2.zero;

			realDelta = mouse.tiledPosition - startMovePosition;
	
			foreach (Tile tile in moveTiles){
				if (!snap){
					tile.offset = new Vector2( (mouse.position.x- mouse.tiledPosition.x*cellSize) , (mouse.position.y-mouse.tiledPosition.y*cellSize) ) / (zoom/100f);
				}
				else{
					tile.offset = Vector2.zero;
				}

				tile.position = new Vector3( tile.position.x + realDelta.x , tile.position.y + realDelta.y, tile.position.z);
				tile.snap = snap;
			}

			for(int i=0;i<selectedTiles.Count;i++){
				selectedTiles[i] = new Vector2(selectedTiles[i].x + realDelta.x, selectedTiles[i].y + realDelta.y);
			}

			startMovePosition = mouse.tiledPosition;
	
		}

		if (startMoveSelection && mouse.leftButton== Mouse.ButtonState.Up){
			startMoveSelection = false;
		}
	}

	// AddTile
	private void PaintTile(float cellSize){

		int xCell = (int)mouse.tiledPosition.x;
		int yCell =(int)mouse.tiledPosition.y;

		if (snap){
			if (mouse.leftButton == Mouse.ButtonState.Down || mouse.leftButton == Mouse.ButtonState.Pressed){
				if (xCell<map.CellX && yCell<map.CellY && xCell>-1 && yCell>-1 && spriteIndex>-1){
					if (xCell != oldXCell || yCell != oldYCell) {
						CreateTile( xCell,yCell,cellSize,snap);
						oldXCell = xCell;
						oldYCell = yCell;
					}
				}
			}
		}
		else if (mouse.leftButton == Mouse.ButtonState.Up){
			if (xCell<map.CellX && yCell<map.CellY && xCell>-1 && yCell>-1 && spriteIndex>-1){
				CreateTile( xCell,yCell,cellSize,snap);
				oldXCell = -1;
				oldYCell = -1;
			}
		}

		Vector2 size = new Vector2(cellSize,cellSize);
		if (!resize && spriteIndex>-1){
			size = map.refTiles[spriteIndex].GetSize();
			size *= zoom/100f;
		}

		if (EyedropperTile!=null && !EyedropperTile.resize){
			size = map.refTiles[spriteIndex].GetSize();
			size = new Vector2(size.x * EyedropperTile.scale.x, size.y * EyedropperTile.scale.y);
			size *= zoom/100f;
		}

		if (spriteIndex>-1 && xCell>=0 && yCell >=0 && snap){
			GUI.DrawTextureWithTexCoords( new Rect(xCell*cellSize+1, yCell* cellSize+1,size.x, size.y), map.refTiles[spriteIndex].GetTexture(),map.refTiles[spriteIndex].GetUVRect() );
		}
		else if (spriteIndex>-1 && xCell>=0 && yCell >=0){
			GUI.DrawTextureWithTexCoords( new Rect(mouse.position.x,mouse.position.y,size.x, size.y), map.refTiles[spriteIndex].GetTexture(),map.refTiles[spriteIndex].GetUVRect() );
		}


	}

	// Fill tiles
	private void FillTiles(){

		if (spriteIndex>-1){
			foreach (Vector2 cell in selectedTiles){
				CreateTile( (int)cell.x,(int)cell.y,0,true);
			}
		}

	}
		
	// EyeDropper
	private void EyeDropper(float cellSize){
		
		int xCell = (int)mouse.tiledPosition.x;
		int yCell = (int)mouse.tiledPosition.y;
		
		if (mouse.leftButton == Mouse.ButtonState.Up && xCell>=0 && yCell>=0 ){

			if (overNoResiezCell.x>-1 && overNoResiezCell.y>-1){
				toolIndex=2;
				int result = map.FindTile( new Vector3( overNoResiezCell.x,overNoResiezCell.y,order), sortingLayersId[sortingLayerIndex]);
				if (result>-1){
					toolIndex=2;
					spriteIndex = map.tiles[result].tileRefIndex;
					EyedropperTile = map.tiles[result];
				}
			}
			else{
				int result = map.FindTile( new Vector3( xCell,yCell,order), sortingLayersId[sortingLayerIndex]);
				if (result>-1){
					toolIndex=2;
					spriteIndex = map.tiles[result].tileRefIndex;
					EyedropperTile = map.tiles[result];
				}
			}
		}
		
		GUI.color = new Color(0,0,1,0.3f);
		GUI.DrawTexture( new Rect(xCell*cellSize+1, yCell* cellSize+1,cellSize,cellSize) , EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;
	}
	
	// Erase
	private void EraseSimpleTile(float cellSize){

		int xCell = (int)mouse.tiledPosition.x;
		int yCell = (int)mouse.tiledPosition.y;


		if((mouse.leftButton == Mouse.ButtonState.Pressed && (xCell != oldXCell || yCell != oldYCell && xCell>=0 && yCell>=0)) ) {

			if (overNoResiezCell.x>-1 && overNoResiezCell.y>-1){
				DeleteTile((int)overNoResiezCell.x,(int)overNoResiezCell.y);
			}
			else{
				DeleteTile(xCell,yCell);
			}
			oldXCell = xCell;
			oldYCell = yCell;

		}
		GUI.color = new Color(1,0,0,0.3f);
		GUI.DrawTexture( new Rect(xCell*cellSize+1, yCell* cellSize+1,cellSize,cellSize) , EditorGUIUtility.whiteTexture);
		GUI.color = Color.white;

	}

	// Erase selected tile
	private void EraseSelectedTiles(){

		foreach (Vector2 cell in selectedTiles){
			DeleteTile((int)cell.x,(int)cell.y);
		}

	}

	private void DeleteTile(int xCell, int yCell){

		if (!allLayer && !allOrder){
			int index = map.FindTile( new Vector3(xCell,yCell,order), sortingLayersId[sortingLayerIndex]);
			if (index>-1){
				map.tiles[index] = null;
				map.tiles.RemoveAt(index);
			}
		}
		
		
		if (!allLayer && allOrder){
			
			for(int i=0; i<map.tiles.Count;i++){
				if (xCell == map.tiles[i].position.x && yCell == map.tiles[i].position.y && map.tiles[i].sortingLayer == sortingLayersId[sortingLayerIndex]){
					map.tiles[i] = null;
					map.tiles.RemoveAt(i);
					i--;
				}
			}
		}
		
		if (allLayer){
			for(int i=0; i<map.tiles.Count;i++){
				if (xCell == map.tiles[i].position.x && yCell == map.tiles[i].position.y ){
					map.tiles[i] = null;
					map.tiles.RemoveAt(i);
					i--;
				}
			}
		}
	}

	// Rotate
	private void RotateTile(float angle){

		foreach (Vector2 v in selectedTiles){

			if (!allLayer && !allOrder){

				int index = map.FindTile( new Vector3(v.x,v.y,order), sortingLayersId[sortingLayerIndex]);
				if (index>-1){

					map.tiles[index].angle += angle;
				}
			}
			
			
			if (!allLayer && allOrder){
				
				for(int i=0; i<map.tiles.Count;i++){
					if (v.x == map.tiles[i].position.x && v.y == map.tiles[i].position.y && map.tiles[i].sortingLayer == sortingLayersId[sortingLayerIndex]){
						map.tiles[i].angle += angle;
					}
				}
			}
			
			if (allLayer){
				for(int i=0; i<map.tiles.Count;i++){
					if (v.x == map.tiles[i].position.x && v.y == map.tiles[i].position.y ){
						map.tiles[i].angle += angle;
					}
				}
			}
		}
	}

	// Create a tile
	private void CreateTile(int xCell, int yCell, float cellSize, bool useSnap){

		Tile newTile = null;

		if (EyedropperTile == null){
			newTile =  (Tile)map.refTiles[spriteIndex].Clone();
			newTile.sortingLayer = sortingLayersId[sortingLayerIndex];
			newTile.tileRefIndex  = spriteIndex;
			newTile.snap = snap;
			newTile.resize = resize;
		}
		else{
			newTile = (Tile)EyedropperTile.Clone();
		}
		newTile.position = new Vector3(xCell,yCell,order);

		if (!useSnap){
			newTile.offset = new Vector2( (mouse.position.x- xCell*cellSize) , (mouse.position.y-yCell*cellSize) ) / (zoom/100f);

		}
		else{
			newTile.offset = Vector2.zero;
		}

		if (useSnap){
			int tileIndex = map.FindTile( newTile.position,newTile.sortingLayer);
			if (tileIndex>-1){
				map.tiles[tileIndex] = null;
				map.tiles[tileIndex] = newTile;
			}
			else{
				map.tiles.Add(newTile);
			}
		}
		else{
			map.tiles.Add(newTile);
		}
		
		map.SetLayerOrder( newTile.sortingLayer.ToString(), order);
	}
	#endregion
	
	#region Draw method
	private void DrawGrid(float gridSize,float cellSize,Color color){
		
		GUI.color = color;
		
		// Y line
		int yStart = Mathf.FloorToInt( scrollWork.x/ cellSize);
		int yLineCount = Mathf.CeilToInt( (Screen.width-rightToolBatWidth-40)/ cellSize );
		
		if (yStart + yLineCount > map.CellX) yLineCount = map.CellX-yStart;
		for( int y=0;y<=yLineCount;y++){
			float lineHeight = scrollWork.y + Screen.height;
			if (lineHeight > map.CellY * cellSize) lineHeight = (map.CellY * cellSize ) - scrollWork.y ;
			GUI.DrawTexture( new Rect(yStart * cellSize + y * gridSize * zoom/100f,scrollWork.y,1,lineHeight), EditorGUIUtility.whiteTexture);
		}
		
		
		// x Line
		int xStart = Mathf.FloorToInt( scrollWork.y/ cellSize);
		int xLineCount = Mathf.CeilToInt( (Screen.height-132)/ cellSize );
		
		if (xStart + xLineCount > map.CellY) xLineCount = map.CellY-xStart;
		for (int x=0;x<=xLineCount;x++){
			float lineWidth = scrollWork.x + (Screen.width-rightToolBatWidth-40);
			if (lineWidth > map.CellX * cellSize) lineWidth = (map.CellX * cellSize ) - scrollWork.x ;
			GUI.DrawTexture( new Rect(scrollWork.x,xStart * cellSize + x * gridSize * zoom/100f,lineWidth,1), EditorGUIUtility.whiteTexture);
		}
		
		
		GUI.color = Color.white;
		
	}
	
	private void DrawMap(float cellSize){
		
		overNoResiezCell = new Vector2(-1,-1);
		
		for (int l=0;l<sortingLayersNames.Length;l++){
			
			SortingLayerFilter filter = GetLayerFilter(sortingLayersId[l]);
			
			if (filter!=null && !filter.show){
				GUI.color = filterAlpha;
			}
			else{
				GUI.color = Color.white;
			}
			
			for (int o= map.GetMinLayerOrder(sortingLayersId[l].ToString());o<= map.GetMaxLayerOrder(sortingLayersId[l].ToString());o++){
				
				
				for (int i=0;i<map.tiles.Count;i++){
					Tile tile = map.tiles[i];
					if (sortingLayersId[l] == tile.sortingLayer && tile.GetTexture()!=null && tile.position.z==o ){
						
						if (!filter.allOrder && o != filter.order){
							GUI.color = filterAlpha;
						}
						else if(o == filter.order && filter.show ){
							GUI.color = Color.white;
						}
						
						GUI.depth = (int)tile.position.z;
						
						Vector2 size = new Vector2(cellSize,cellSize);
						
						if (!tile.resize) {
							size = new Vector2(tile.GetSize().x * tile.scale.x , tile.GetSize().y * tile.scale.y);
							size *= zoom/100f;
						}
						
						Vector2 pivot = new Vector2(tile.position.x*cellSize + size.x/2f ,tile.position.y * cellSize +  size.y/2f);
						
						
						bool fake = false;
						Matrix4x4 savMatrix =  GUI.matrix;
						if (tile.angle!=0){
							
							if (tile.position.x*cellSize + tile.offset.x >=scrollWork.x
							    && tile.position.x*cellSize + tile.offset.x + size.x <= scrollWork.x +Screen.width-rightToolBatWidth-40 
							    && tile.position.y*cellSize + tile.offset.y >=scrollWork.y
							    && tile.position.y*cellSize + tile.offset.y + size.y <= scrollWork.y+(Screen.height-132) ){
								
								GUIUtility.RotateAroundPivot( tile.angle,pivot );
								
							}
							else{
								fake = true;
								
							}
						}
						
						Color GUISavColor = GUI.color;
						if (autoHide && filter.show){
							if (tile.sortingLayer != sortingLayersId[sortingLayerIndex] ){
								GUI.color =  filterAlpha;
							}
							else{
								if (tile.position.z != order ){
									GUI.color = new Color( 1,0.1f,0.4f,0.8f);
								}
							}
						}
						
						if (tile.position.x*cellSize + tile.offset.x+ size.x >=scrollWork.x
						    && tile.position.x*cellSize + tile.offset.x  <=scrollWork.x +Screen.width-rightToolBatWidth-40
						    && tile.position.y*cellSize + tile.offset.y + size.y >= scrollWork.y
						    && tile.position.y*cellSize + tile.offset.y <= scrollWork.y+(Screen.height-132) ){
							if (!fake){
								Rect rect = new Rect(tile.position.x*cellSize + tile.offset.x * (zoom/100f), tile.position.y * cellSize+tile.offset.y * (zoom/100f),size.x,size.y);
								GUI.DrawTextureWithTexCoords(rect , tile.GetTexture(),tile.GetUVRect() );
								
								if (rect.Contains( mouse.position) && (!tile.resize || !tile.snap) && tile.sortingLayer == sortingLayersId[ sortingLayerIndex] && tile.position.z == order) {
									GUI.color = new Color(1,0.7f,0.1f,0.3f);
									HTGUILayout.DrawTileTexture(  new Rect(tile.position.x*cellSize + tile.offset.x * (zoom/100f), tile.position.y * cellSize+tile.offset.y* (zoom/100f) ,size.x,size.y), EditorGUIUtility.whiteTexture);
									
									overNoResiezCell = new Vector2( tile.position.x, tile.position.y);
								}
							}
							else{
								Rect rect = new Rect(tile.position.x*cellSize + tile.offset.x , tile.position.y * cellSize+tile.offset.y ,size.x,size.y);
								HTGUILayout.DrawTileTexture( rect , HTGUILayout.GetCheckerTexture());
								
								if (rect.Contains( mouse.position) && (!tile.resize || !tile.snap) && tile.sortingLayer == sortingLayersId[ sortingLayerIndex] && tile.position.z == order){
									GUI.color = new Color(1,0.7f,0.1f,0.3f);
									HTGUILayout.DrawTileTexture( rect, EditorGUIUtility.whiteTexture);
									GUI.color = Color.white;
									
									overNoResiezCell = new Vector2( tile.position.x, tile.position.y);
								}
							}
							
						}
						GUI.color = GUISavColor;
						
						GUI.matrix = savMatrix;
					}
				}
			}
			GUI.color = Color.white;
		}
		
	}

	private void DrawSelection(float cellSize){
		foreach (Vector2 cell in selectedTiles){
			GUI.color = new Color(0,1,0,0.3f);
			GUI.DrawTexture( new Rect(cell.x*cellSize+1, cell.y* cellSize+1,cellSize,cellSize) , EditorGUIUtility.whiteTexture);
			GUI.color = Color.white;
		}
	}
	#endregion
	
	#region Drag & Drop method
	private void DoDrop( UnityEngine.Object dragObject){

		// Gameobject
		if (dragObject.GetType() == typeof(GameObject)){
			GameObject obj = (GameObject)dragObject;
			SpriteRenderer srs = obj.GetComponent<SpriteRenderer>();

			// Complexe sprite
			if (srs!=null){
				string path = AssetDatabase.GetAssetPath( srs.sprite.texture);
				string guid = AssetDatabase.AssetPathToGUID( path);
				AddRefTile(guid,false,Tile.TileType.ComplexeSprite,srs.sprite,obj);
			}

			// Prefab
			if (srs==null){
				foreach( Transform t in obj.transform){
					srs = t.GetComponent<SpriteRenderer>();
					if (srs!=null){
						string path = AssetDatabase.GetAssetPath( srs.sprite.texture);
						string guid = AssetDatabase.AssetPathToGUID( path);
						AddRefTile(guid,false,Tile.TileType.PrefabSprite,srs.sprite,obj);
					}
				}
			}
		}

		// Sprite
		if (dragObject.GetType() == typeof(Sprite)){
			string path = AssetDatabase.GetAssetPath( dragObject);
			string guid = AssetDatabase.AssetPathToGUID( path);
			AddRefTile(guid,false,Tile.TileType.Sprite, (Sprite)dragObject );
			return;
		}

		// Multi sprite
		if (dragObject.GetType() == typeof(Texture2D)){

			Texture2D tex = (Texture2D)dragObject;

			// Get the path of the texture
			string path = AssetDatabase.GetAssetPath( tex.GetInstanceID());
			
			// Get the textureImporter object
			TextureImporter textureImporter = AssetImporter.GetAtPath(  path ) as TextureImporter;

			if (textureImporter.spriteImportMode != SpriteImportMode.None ){
				UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(path); 

				if (sprites.Length>0){
					if (sprites.Length>1){
						for (int i=1;i<sprites.Length;i++){
							AddRefTile("",true,Tile.TileType.Sprite,(Sprite)sprites[i],null );
						}
					}
				}
			}


			return;
		}

	}

	public void AddRefTile( string guid,bool multi,Tile.TileType tileType, Sprite sp=null,GameObject obj=null){
		
		Tile tile = new Tile();
		tile.tileType = tileType;
		tile.multiSprite = multi;

		if (obj == null){
			tile.name = sp.name;
		}
		else{
			tile.name = obj.name;
		}
		
		switch (tileType){
		case Tile.TileType.Sprite:
			tile.sprite = sp;
			break;
		case Tile.TileType.ComplexeSprite:	
		case Tile.TileType.PrefabSprite:
			tile.obj = obj;
			tile.sprite = sp;
			break;
		}

		tile.UpdateRealSize();
		map.refTiles.Add( tile);
	}
	#endregion
	
	#region Sorting layer management
	private void UpdateSortingLayersFilter(){

		for (int i=0;i<sortingLayersNames.Length;i++){

		
			int result = layerFilters.FindIndex(
				delegate(SortingLayerFilter l)
				{
				return  l.id == sortingLayersId[i];
			}
			);

			if (result==-1){
				SortingLayerFilter layer = new SortingLayerFilter();
				layer.id = sortingLayersId[i];
				layer.show = true;
				layer.allOrder = true;
				layer.order = 0;
				layerFilters.Add ( layer);
			}


		}
	}

	private SortingLayerFilter GetLayerFilter(int id){
		int result = layerFilters.FindIndex(
			delegate(SortingLayerFilter l)
			{
			return  l.id == id;
		}
		);

		if (result>-1){
			return layerFilters[result];
		}
		else{
			return null;
		}
	}

	private string[] GetSortingLayerName(){

		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
		return (string[])sortingLayersProperty.GetValue(null, new object[0]);
	}

	private string GetSortingLayerById(int id){
		string layerName="";

		for(int i=0;i<sortingLayersId.Length;i++){
			if (sortingLayersId[i]==id){
				layerName = sortingLayersNames[i];
			}
		}

		return layerName;
	}

	private int[] GetSortingLayerId(){
		Type internalEditorUtilityType = typeof(InternalEditorUtility);
		PropertyInfo sortingLayersIdProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
		return (int[])sortingLayersIdProperty.GetValue(null, new object[0]);
	}

	private int GetSortingLayerIndexById(int id){
		
		int index=-1;
		for(int i=0;i<sortingLayersId.Length;i++){
			if (sortingLayersId[i]==id){
				index = i;
			}
		}

		return index;
	}

	private bool ChangeTileLayerOrOrder(Vector3 position, int layer, bool ask=true){
		
		bool returnValue = false;
		
		int tileIndex = map.FindTile( position,layer);
		if (tileIndex==-1){
			returnValue = true;
		}
		else{
			if (ask){
				if ( EditorUtility.DisplayDialog("Tiled Map Factory","A tile already exit with this setting. Replace it ?","Replace","Cancel")){
					
					map.tiles[tileIndex]=null;
					map.tiles.RemoveAt(tileIndex);
					returnValue = true;
					
				}
				else{
					returnValue = false;
				}
			}
			else{
				returnValue = false;
			}
		}	
		
		return returnValue;
	}	
	#endregion
	
	#region Generation method
	private void GenerateTiledMap(){

		// Create list from option
		GameObject tiledMap = GameObject.Find( map.name +"_generated");
		DestroyImmediate( tiledMap);

		if (map.tiles.Count>0){
			tiledMap = new GameObject(map.name +"_generated");
		}

		for (int l=0;l<sortingLayersId.Length;l++){
			foreach (Tile tile in map.tiles){
				if ((generateAllLayer && sortingLayersId[l]== tile.sortingLayer ) || (!generateAllLayer && GetLayerFilter(sortingLayersId[l]).generate &&  sortingLayersId[l]== tile.sortingLayer)){

					#region GameObject
					// GameObject
					GameObject sp =null;
					switch (tile.tileType){
						case Tile.TileType.Sprite:
							sp =new GameObject(tile.name);
							break;
						case Tile.TileType.ComplexeSprite:
						case Tile.TileType.PrefabSprite:
							sp = (GameObject)Instantiate( tile.obj);
							break;
					}
					#endregion

					#region Sprite
					// New Sprite
					SpriteRenderer spr = null;
					switch (tile.tileType){
						case Tile.TileType.Sprite:
							spr = sp.AddComponent<SpriteRenderer>();
							spr.sprite = tile.sprite;
							break;
						case Tile.TileType.ComplexeSprite:
							spr = sp.GetComponent<SpriteRenderer>();
							break;
						case Tile.TileType.PrefabSprite:
							foreach( Transform t in sp.transform){
								spr = t.GetComponent<SpriteRenderer>();
							}
							break;
					}

					spr.sortingLayerName = GetSortingLayerById(tile.sortingLayer);
					spr.sortingOrder = (int)tile.position.z;
					#endregion

					#region sorting layer
					// Sorting layer
					string layerName = spr.sortingLayerName;
					if (string.IsNullOrEmpty(layerName)) layerName = "Default";
					
					Transform parentLayer = tiledMap.transform.FindChild(layerName);
					if (parentLayer == null){
						GameObject parentLayerGameObject = new GameObject(layerName);
						parentLayerGameObject.transform.parent = tiledMap.transform;
						parentLayer = parentLayerGameObject.transform;
					}
					#endregion

					#region Order in Layer
					// order in layer
					Transform parentOrder = parentLayer.FindChild("Order"+ spr.sortingOrder.ToString() );
					if (parentOrder == null){
						GameObject parentOrderGameObject = new GameObject( "Order" + spr.sortingOrder.ToString());
						parentOrderGameObject.transform.parent = parentLayer;
						parentOrder = parentOrderGameObject.transform;
					}
					
					sp.transform.parent = parentOrder;
					sp.transform.eulerAngles = new Vector3(0,0,-tile.angle);
					#endregion

					#region Position & scale
					// Position & scale
					float gridRatio = (map.GridSize / 100f);
					Vector2 tileOffset = tile.offset/100f;
					if (tile.resize){
						float gridOffset = map.GridSize/100f/2f;
						sp.transform.position = new Vector3( tile.position.x * gridRatio + tileOffset.x +gridOffset,0 - (tile.position.y* gridRatio + tileOffset.y + gridOffset ),0);
						sp.transform.localScale = new Vector3(map.GridSize/tile.GetSize(true).x, map.GridSize/tile.GetSize(true).y,1);
					
					}
					else{
						Vector2 textureOffset = tile.GetSize()/100f/2f;
						textureOffset = new Vector2( textureOffset.x * tile.scale.x, textureOffset.y * tile.scale.y);
						sp.transform.position = new Vector3( tile.position.x * gridRatio + tileOffset.x +textureOffset.x,0 - (tile.position.y* gridRatio + tileOffset.y + textureOffset.y ),0);

						//sp.transform.localScale = new Vector3((map.GridSize/tile.GetSize().x) * (tile.GetSize().x/ map.GridSize) * tile.scale.x , (map.GridSize/tile.GetSize().y) * (tile.GetSize().y/ map.GridSize)* tile.scale.y,1);
						sp.transform.localScale = new Vector3((tile.GetSize().x / tile.GetSize(true).x) * tile.scale.x , (tile.GetSize().y / tile.GetSize(true).y) * tile.scale.y,1);
					}
					#endregion

					#region Collider & layer
					if (tile.haveCollider && (tile.tileType == Tile.TileType.Sprite || tile.tileType == Tile.TileType.ComplexeSprite)){
						switch (tile.colliderType){
							case Tile.TileColliderType.Box:
								spr.gameObject.AddComponent<BoxCollider2D>();
								spr.gameObject.layer = tile.layer;
								break;
							case Tile.TileColliderType.Circle:
								spr.gameObject.AddComponent<CircleCollider2D>();
								spr.gameObject.layer = tile.layer;
								break;
							case Tile.TileColliderType.Polygon:
								spr.gameObject.AddComponent<PolygonCollider2D>();
								spr.gameObject.layer = tile.layer;
								break;
						}
					}
					#endregion
				}
			}
		}

	}
	
	private void GenerateTexture(){

		float minX=1000000;
		float minY=1000000;
		float maxX=0;
		float maxY=0;
		float progressValue=0;
		bool multiTexture = false;

		#region  Step 1 : Change texture read/write flag
		float step = 10f/map.refTiles.Count;

		foreach(Tile tile in map.refTiles){
			progressValue+=step;
			SetReadWrite( tile.sprite.texture,true);
			EditorUtility.DisplayProgressBar( "Tile Map Factory " + progressValue.ToString("f0") + "%","Change texture read/write flag",progressValue/100f);
		}
		#endregion

		#region Step 2 : Compute world size
		step = 10f/map.tiles.Count;

		foreach (Tile tile in map.tiles){
			
			// Compute world size
			minX = (tile.position.x + tile.offset.x/map.GridSize <minX) ? tile.position.x + tile.offset.x/map.GridSize :minX;
			minY = (tile.position.y + tile.offset.y/map.GridSize <minY) ? tile.position.y + tile.offset.y/map.GridSize :minY;

			if (tile.resize){
				maxX = (tile.position.x + 1 +  (tile.offset.x/map.GridSize) > maxX) ? tile.position.x  + 1 + (tile.offset.x /map.GridSize) :maxX;
			}
			else{
				maxX = (tile.position.x  + 1 +  (tile.offset.x/map.GridSize) + (tile.GetTextureRect().width * tile.scale.x)/map.GridSize > maxX) ? tile.position.x  + 1  + (tile.offset.x /map.GridSize)+ (tile.GetTextureRect().width * tile.scale.x)/map.GridSize :maxX;
			}


			if (tile.resize){
				maxY = (tile.position.y + 1 + (tile.offset.y/map.GridSize) > maxY) ? tile.position.y  + 1 + (tile.offset.y/map.GridSize) :maxY;
			}
			else{
				maxY = (tile.position.y  + 1 + (tile.offset.y/map.GridSize) + (tile.GetTextureRect().height * tile.scale.y)/map.GridSize > maxY) ? tile.position.y  + 1 + (tile.offset.y/map.GridSize) + (tile.GetTextureRect().height * tile.scale.y)/map.GridSize :maxY;
			}

			progressValue+=step;
			EditorUtility.DisplayProgressBar( "Tile Map Factory " + progressValue.ToString("f0") + "%" ,"Compute world size ...",progressValue/100f);
		}

		if ( (maxX-minX)*map.GridSize>=maxTextureSize && minX>0){
			minX=0;
		}
		if ((maxY-minY)*map.GridSize>=maxTextureSize && minY>0){
			minY=0;
		}

		Vector2 mapSize = new Vector2( Mathf.CeilToInt((maxX-minX)*map.GridSize), Mathf.CeilToInt((maxY-minY)*map.GridSize));


		int countTextureWidth = 1;
		int countTextureHeight = 1;

		if (mapSize.x > maxTextureSize || mapSize.y > maxTextureSize){
			countTextureWidth = Mathf.CeilToInt( mapSize.x / maxTextureSize);
			countTextureHeight = Mathf.CeilToInt( mapSize.y / maxTextureSize);
			multiTexture = true;
			mapSize = new Vector2(maxTextureSize, maxTextureSize);
		}
		#endregion

		#region Step 3 : Create empty texture
		Texture2D[,] mapTextures = new Texture2D[countTextureWidth,countTextureHeight];

		Color[] alpha = new Color[(int)mapSize.x*(int)mapSize.y];
		for(int i=0;i<alpha.Length;i++){
			alpha[i] = new Color(0,0,0,0);
		}
		#endregion

		#region  step 4 : Create texutre
		int minOrder = map.GetMinLayerOrder(sortingLayersId[generatedLayerIndex].ToString());
		int maxOrder = map.GetMaxLayerOrder(sortingLayersId[generatedLayerIndex].ToString());

		step = (60f/(map.tiles.Count * (Mathf.Clamp( (maxOrder+1)-minOrder,1, 1000))));
		for (int o= minOrder;o<= maxOrder;o++){
			for (int i=0;i<map.tiles.Count;i++){

				Tile tile = map.tiles[i];
				if (sortingLayersId[generatedLayerIndex] == tile.sortingLayer && tile.position.z==o ){
					
					// Copy
					Texture2D tileTexture = new Texture2D((int)tile.GetTextureRect().width, (int)tile.GetTextureRect().height,TextureFormat.ARGB32,false);
					Color[] pixels = tile.sprite.texture.GetPixels((int)tile.GetTextureRect().x,(int)tile.GetTextureRect().y,(int)tile.GetTextureRect().width,(int)tile.GetTextureRect().height);
					tileTexture.SetPixels(pixels);
					tileTexture.Apply();

					// resize
					int textureWidth = map.GridSize;
					int textureHeight = map.GridSize;

					if (!tile.resize){
						textureWidth = (int)(tile.GetTextureRect().width * tile.scale.x);
						textureHeight = (int)(tile.GetTextureRect().height * tile.scale.y);
					}

					TextureScale.Bilinear(tileTexture, textureWidth, textureHeight);
					tileTexture.Apply();

					// copy the new one
					pixels = tileTexture.GetPixels(0,0,textureWidth, textureHeight);

					if (multiTexture){
						int x = Mathf.FloorToInt(  tile.position.x*map.GridSize / mapSize.x);
						int y = Mathf.FloorToInt(  tile.position.y*map.GridSize / mapSize.y);

						if (mapTextures[x,y] == null){			
						    mapTextures[x,y] = new Texture2D((int)mapSize.x, (int)mapSize.y);
						    mapTextures[x,y].SetPixels( alpha);
						}

						int posX = (int)((tile.position.x * map.GridSize + tile.offset.x) - x*mapSize.x);
						int posY = (int)((tile.position.y * map.GridSize + tile.offset.y) - y*mapSize.y);

						if (((!tile.resize && posX + tile.GetTextureRect().width * tile.scale.x <= maxTextureSize) && 
						     ( !tile.resize && (mapTextures[x,y].height - (posY + tile.GetTextureRect().height * tile.scale.y )) >=0))
							|| tile.resize ){
								
							if (minOrder == maxOrder || o == minOrder){
								mapTextures[x,y].SetPixels( posX , mapTextures[x,y].height - (posY + textureHeight), textureWidth, textureHeight, pixels);
							}
							else{
								int px = posX;
								int	py = mapTextures[x,y].height - (posY + textureHeight);
								
								for (int xx=0;xx<textureWidth;xx++){
									for (int yy=0;yy<textureHeight;yy++){
										Color bg =  mapTextures[x,y].GetPixel( px+xx,py+yy);
										Color fg = tileTexture.GetPixel( xx,yy);
										
										Color r = new Color();
										r.a = 1 - (1 - fg.a) * (1 - bg.a);
										r.r = fg.r * fg.a / r.a + bg.r * bg.a * (1 - fg.a) / r.a;
										r.g = fg.g * fg.a / r.a + bg.g * bg.a * (1 - fg.a) / r.a;
										r.b = fg.b * fg.a / r.a + bg.b * bg.a * (1 - fg.a) / r.a;
										mapTextures[x,y].SetPixel(px+xx,py+yy, r);
										
									}
								}
							}
							mapTextures[x,y].Apply();
						}

					}
					else{
						if (mapTextures[0,0] == null){
							mapTextures[0,0] = new Texture2D((int)mapSize.x, (int)mapSize.y);
							mapTextures[0,0].SetPixels( alpha);
						}
						
						int posX = (int)(tile.position.x * map.GridSize + tile.offset.x);
						int posY = (int)(tile.position.y * map.GridSize + tile.offset.y);
						
						if (minOrder == maxOrder || o == minOrder){
							mapTextures[0,0].SetPixels( posX - (int)(minX*map.GridSize) , mapTextures[0,0].height - ((posY + textureHeight) - (int)(minY*map.GridSize)), textureWidth, textureHeight, pixels);
						}
						else{
							int px = posX - (int)(minX*map.GridSize);
							int	py = mapTextures[0,0].height - ((posY + textureHeight) - (int)(minY*map.GridSize));
							
							for (int x=0;x<textureWidth;x++){
								for (int y=0;y<textureHeight;y++){
									Color bg =  mapTextures[0,0].GetPixel( px+x,py+y );
									Color fg = tileTexture.GetPixel( x,y);
									
									Color r = new Color();
									r.a = 1 - (1 - fg.a) * (1 - bg.a);
									r.r = fg.r * fg.a / r.a + bg.r * bg.a * (1 - fg.a) / r.a;
									r.g = fg.g * fg.a / r.a + bg.g * bg.a * (1 - fg.a) / r.a;
									r.b = fg.b * fg.a / r.a + bg.b * bg.a * (1 - fg.a) / r.a;
									mapTextures[0,0].SetPixel(px+x,py+y, r);
									
								}
							}
						}
						mapTextures[0,0].Apply();
						
					}

					tileTexture = null;
				}

				progressValue+=step;
				EditorUtility.DisplayProgressBar( "Tile Map Factory " + progressValue.ToString("f0") + "%","Create final textures ..." ,progressValue/100f);

			}
		}
		#endregion

		#region step 5 : Save new texture
		step = (10f/(countTextureWidth*countTextureHeight));
		for(int x=0;x<countTextureWidth;x++){
			for(int y=0;y<countTextureHeight;y++){

				EditorUtility.DisplayProgressBar( "Tile Map Factory "+ progressValue.ToString("f0") + "%","Save textures ...",progressValue/100f);

				if (mapTextures[x,y] != null){
					Byte[] pnj = mapTextures[x,y].EncodeToPNG();
					System.IO.File.WriteAllBytes("Assets/" + textureName + x.ToString() + y.ToString() +".png", pnj);
				}
				progressValue+=step;

			}
		}

		AssetDatabase.Refresh();
		for(int x=0;x<countTextureWidth;x++){
			for(int y=0;y<countTextureHeight;y++){
				if (mapTextures[x,y] != null){
					SetNoPowerOfTwo( "Assets/" + textureName + x.ToString() + y.ToString() +".png");
				}
			}
		}
		#endregion

		#region Step 6 : Set read/write to false
	 	step = 10f/map.refTiles.Count;
		foreach(Tile tile in map.refTiles){
			SetReadWrite( tile.sprite.texture,false);
			progressValue+=step;
			EditorUtility.DisplayProgressBar( "Tile Map Factory " + progressValue.ToString("f0") + "%","Change texture read/write flag",progressValue/100f);
		}
		#endregion

		EditorUtility.ClearProgressBar();







	}
	#endregion

	#region Other
	private void SetReadWrite( Texture2D texture,bool readWrite){
		
		// Get the path of the texture
		string 	path = AssetDatabase.GetAssetPath( texture.GetInstanceID());

		// Get the textureImporter object
		TextureImporter textureImporter = AssetImporter.GetAtPath(  path ) as TextureImporter;
		
		// Texture type to advanced
		textureImporter.textureType = TextureImporterType.Advanced;	
		
		// Creat a new setting
		TextureImporterSettings st = new TextureImporterSettings();
		textureImporter.ReadTextureSettings(st);

		// Texture must be in ARgB32
		st.textureFormat = TextureImporterFormat.ARGB32;
		// Set write/read flag
		st.readable = readWrite;

		// Import the new setting
		textureImporter.SetTextureSettings(st);
		
		// Update the asset
		AssetDatabase.ImportAsset(path);	
	}

	private void SetNoPowerOfTwo(string path){

		// Get the textureImporter object
		TextureImporter textureImporter = AssetImporter.GetAtPath(  path ) as TextureImporter;
		
		// Texture type to advanced
		textureImporter.textureType = TextureImporterType.Advanced;	
		
		// Creat a new setting
		TextureImporterSettings st = new TextureImporterSettings();
		textureImporter.ReadTextureSettings(st);
		
		st.npotScale = TextureImporterNPOTScale.None; 
		st.mipmapEnabled = false;


		// Import the new setting
		textureImporter.SetTextureSettings(st);
		
		// Update the asset
		AssetDatabase.ImportAsset(path);
	}

	private Texture2D GetIcon(int index){
		
		if (EditorGUIUtility.isProSkin){
			switch (index){
			case 0:
				if (proSkinIcon[0]==null){
					proSkinIcon[0] = (Texture2D)Resources.Load("pointerPro");
				}
				break;
			case 1:
				if (proSkinIcon[1]==null){
					proSkinIcon[1] = (Texture2D)Resources.Load("AddPro");
				}
				break;
			case 2:
				if (proSkinIcon[2]==null){
					proSkinIcon[2] = (Texture2D)Resources.Load("eraserPro");
				}
				break;
			case 3:
				if (proSkinIcon[3]==null){
					proSkinIcon[3] = (Texture2D)Resources.Load("OrderPro");
				}
				break;
			case 4:
				if (proSkinIcon[4]==null){
					proSkinIcon[4] = (Texture2D)Resources.Load("ZoomPro");
				}
				break;
			case 5:
				if (proSkinIcon[5]==null){
					proSkinIcon[5] = (Texture2D)Resources.Load("RotateLeftPro");
				}
				break;
			case 6:
				if (proSkinIcon[6]==null){
					proSkinIcon[6] = (Texture2D)Resources.Load("RotateRightPro");
				}
				break;
			case 7:
				if (proSkinIcon[7]==null){
					proSkinIcon[7] = (Texture2D)Resources.Load("OpenClosePro");
				}
				break;
			case 8:
				if (proSkinIcon[8]==null){
					proSkinIcon[8] = (Texture2D)Resources.Load("LayerPro");
				}
				break;
			case 9:
				if (proSkinIcon[9]==null){
					proSkinIcon[9] = (Texture2D)Resources.Load("SnapPro");
				}
				break;
			case 10:
				if (proSkinIcon[10]==null){
					proSkinIcon[10] = (Texture2D)Resources.Load("ResizePro");
				}
				break;
			case 11:
				if (proSkinIcon[11]==null){
					proSkinIcon[11] = (Texture2D)Resources.Load("SelectPro");
				}
				break;
				
			case 12:
				if (proSkinIcon[12]==null){
					proSkinIcon[12] = (Texture2D)Resources.Load("FillPro");
				}
				break;
			case 13:
				if (proSkinIcon[13]==null){
					proSkinIcon[13] = (Texture2D)Resources.Load("EraserAllPro");
				}
				break;
			case 14:
				if (proSkinIcon[14]==null){
					proSkinIcon[14] = (Texture2D)Resources.Load("PipettePro");
				}
				break;
			case 15:
				if (proSkinIcon[15]==null){
					proSkinIcon[15] = (Texture2D)Resources.Load("MovePro");
				}
				break;
			}
			return proSkinIcon[index];
		}
		else{
			switch (index){
			case 0:
				if (freeSkinIcon[0]==null){
					freeSkinIcon[0] = (Texture2D)Resources.Load("pointerFree");
				}
				break;
			case 1:
				if (freeSkinIcon[1]==null){
					freeSkinIcon[1] = (Texture2D)Resources.Load("AddFree");
				}
				break;
			case 2:
				if (freeSkinIcon[2]==null){
					freeSkinIcon[2] = (Texture2D)Resources.Load("eraserFree");
				}
				break;
			case 3:
				if (freeSkinIcon[3]==null){
					freeSkinIcon[3] = (Texture2D)Resources.Load("OrderFree");
				}
				break;
			case 4:
				if (freeSkinIcon[4]==null){
					freeSkinIcon[4] = (Texture2D)Resources.Load("ZoomFree");
				}
				break;
				
			case 5:
				if (freeSkinIcon[5]==null){
					freeSkinIcon[5] = (Texture2D)Resources.Load("RotateLeftFree");
				}
				break;
			case 6:
				if (freeSkinIcon[6]==null){
					freeSkinIcon[6] = (Texture2D)Resources.Load("RotateRightFree");
				}
				break;
				
			case 7:
				if (freeSkinIcon[7]==null){
					freeSkinIcon[7] = (Texture2D)Resources.Load("OpenCloseFree");
				}
				break;	
			case 8:
				if (freeSkinIcon[8]==null){
					freeSkinIcon[8] = (Texture2D)Resources.Load("LayerFree");
				}
				break;
			case 9:
				if (freeSkinIcon[9]==null){
					freeSkinIcon[9] = (Texture2D)Resources.Load("SnapFree");
				}
				break;
			case 10:
				if (freeSkinIcon[10]==null){
					freeSkinIcon[10] = (Texture2D)Resources.Load("ResizeFree");
				}
				break;
			case 11:
				if (freeSkinIcon[11]==null){
					freeSkinIcon[11] = (Texture2D)Resources.Load("SelectFree");
				}
				break;
			case 12:
				if (freeSkinIcon[12]==null){
					freeSkinIcon[12] = (Texture2D)Resources.Load("FillFree");
				}
				break;
			case 13:
				if (freeSkinIcon[13]==null){
					freeSkinIcon[13] = (Texture2D)Resources.Load("eraserAllFree");
				}
				break;
			case 14:
				if (freeSkinIcon[14]==null){
					freeSkinIcon[14] = (Texture2D)Resources.Load("PipetteFree");
				}
				break;
			case 15:
				if (freeSkinIcon[15]==null){
					freeSkinIcon[15] = (Texture2D)Resources.Load("MoveFree");
				}
				break;
			}
			return freeSkinIcon[index];
		}
		
	}

	private int GetSelectionCount(){
		
		int count=0;
		foreach (Vector2 cell in selectedTiles){
			foreach(Tile tile in map.tiles){
				
				if (tile.position == new Vector3(cell.x,cell.y,order) && tile.sortingLayer == sortingLayersId[sortingLayerIndex]){
					count++;
				}
			}
		}
		
		return count++;
	}
	#endregion

}
