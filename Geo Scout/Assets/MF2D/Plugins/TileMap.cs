/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TileMap : MonoBehaviour {

	#region Members
	public List<Tile> tiles;
	public List<Tile> refTiles;
	public List<LayerOrderFilter> layerOrders;

	[SerializeField]
	private int cellX;
	public int CellX {
		get {
			return cellX;
		}
		set {
			cellX = Mathf.Clamp(value,1,2000);
		}
	}

	[SerializeField]
	private int cellY;
	public int CellY{
		get {
			return cellY;
		}
		set {
		    cellY = Mathf.Clamp(value,1,2000);
		}
	}

	[SerializeField]
	private int gridSize;
	public int GridSize {
		get {
			return gridSize;
		}
		set {
			gridSize = Mathf.Clamp(value,16,1024);
		}
	}
	
	public Color gridColor;
	#endregion

	public TileMap(){
		tiles = new List<Tile>();
		refTiles = new List<Tile>();
		layerOrders = new List<LayerOrderFilter>();

		gridSize = 128;
		cellX = 100;
		cellY = 100;
		gridColor = new Color(39f/255f,95f/255f,130f/255f);

	}


	public int FindTile( Vector3 position, int layer){

		int result = tiles.FindIndex(
			delegate(Tile t)
			{
			return  t.position == position && t.sortingLayer == layer;
			}
		);
		
		return result;
	}
	
	public int[] FindTileFromLayer( Vector2 position, int layer){


		List<int> findedTiles = new List<int>();
	
		for(int i=0; i<tiles.Count;i++){
			if (position.x == tiles[i].position.x && position.y == tiles[i].position.y && tiles[i].sortingLayer == layer){
				findedTiles.Add(i);
			}
		}

		return findedTiles.ToArray();
	}

	public int[] FindTileFromPosition( Vector2 position){
		
		List<int> findedTiles = new List<int>();
		
		int i=0;
		foreach (Tile tile in tiles){
			if (position == new Vector2(tile.position.x, tile.position.y)){
				findedTiles.Add(i);
			}
			i++;
		}
		
		return findedTiles.ToArray();
	}

	public void SetLayerOrder(string layer, int order){

		LayerOrderFilter min = FindLayerOrder( layer+"min");
		if (min == null){
			min = new LayerOrderFilter();
			min.key = layer+"min";
			min.order = order;
			layerOrders.Add(min);
		}
		else{
			if (order < min.order) min.order = order;
		}

		LayerOrderFilter max = FindLayerOrder( layer+"max");
		if (max == null){
			max = new LayerOrderFilter();
			max.key = layer+"max";
			max.order = order;
			layerOrders.Add(max);
		}
		else{
			if (order > max.order) max.order = order;
		}
	}

	public int GetMinLayerOrder( string layer){

		LayerOrderFilter min = FindLayerOrder( layer+"min");
		if (min != null){
			return min.order;
		}
		else{
			return 0;
		}
	}

	public int GetMaxLayerOrder(string layer){
		LayerOrderFilter max = FindLayerOrder( layer+"max");
		if (max != null){
			return max.order;
		}
		else{
			return 0;
		}
	}

	private LayerOrderFilter FindLayerOrder( string key){

		int result = layerOrders.FindIndex(
			delegate(LayerOrderFilter l)
			{
			return  l.key == key;
		}
		);

		if (result>-1){
			return layerOrders[result];
		}
		else{
			return null;
		}
	}


}
