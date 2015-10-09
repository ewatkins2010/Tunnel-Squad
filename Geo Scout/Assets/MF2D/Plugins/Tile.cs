/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using System;
using System.Collections;
//using UnityEditor;


[System.Serializable]
public class Tile : ICloneable {

	public enum TileType {Sprite,ComplexeSprite, PrefabSprite};
	public enum TileColliderType {Circle, Box, Polygon};

	// x,y tile position z = orderlayer
	public string name;
	public Vector3 position;
	public int sortingLayer;
	public Vector2 offset;
	public Vector2 scale;

	public float angle;

	public bool resize;
	public bool snap;

	public TileType tileType;
	public bool multiSprite;
	public string GUID;
	public int tileRefIndex;
	public Sprite sprite;
	public GameObject obj;
	public Texture2D texture;

	public Vector2 realSize;
	public int importerSize;
	public float pixels2Unity;
	
	public bool haveCollider;
	public TileColliderType colliderType;
	public int layer;
	
	public bool isWait2Delete;

	public Tile(){
		isWait2Delete = false;
		haveCollider = false;
		resize = true;
		snap = true;
		angle =0;
		scale = Vector2.one;
		name="Sprite";
	}

	public object Clone(){
	
		return this.MemberwiseClone();
	}

	public Rect GetUVRect(){

		Rect rect = new Rect(0,0,0,0);
		switch (tileType){
			case TileType.Sprite:
			case TileType.ComplexeSprite:
			case TileType.PrefabSprite:
				if (sprite !=null){
					rect= new Rect( sprite.rect.x/ sprite.texture.width,
				                sprite.rect.y/ sprite.texture.height,
				                sprite.rect.width/ sprite.texture.width,
				                sprite.rect.height/ sprite.texture.height);
				}
				break;
		}

		return rect;
	}

	public Rect GetTextureRect(){
		return sprite.rect;
	}

	public Texture2D GetTexture(){

		Texture2D tex = null;

		switch (tileType){
			case TileType.Sprite:
			case TileType.ComplexeSprite:
			case TileType.PrefabSprite:
				if (sprite !=null){
					tex =  sprite.texture;
				}
				break;
		}
		
		return tex;
	}

	public Vector2 GetSize(bool real=false){

		Vector2 size = Vector2.one;

		switch (tileType){
			case TileType.Sprite:
			case TileType.ComplexeSprite:
			case TileType.PrefabSprite:
				if (!real){
					size = new Vector2(sprite.rect.width,sprite.rect.height );
				}
				else{
					if (!multiSprite){
						size = realSize;
					}
					else{
						size = new Vector2(sprite.rect.width,sprite.rect.height );
					}
				}
				break;

		}


		return size;
	}

	public void UpdateRealSize(){
		realSize = GetOriginalTextureSize( sprite.texture );
	}
	private  Vector2 GetOriginalTextureSize(Texture2D texture) {

		#if UNITY_EDITOR
		string path = UnityEditor.AssetDatabase.GetAssetPath(texture);
		UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(path) as UnityEditor.TextureImporter;
		
		if (importer != null) {

			UnityEditor.TextureImporterNPOTScale lastScale = importer.npotScale;
			int lastSize = importer.maxTextureSize;
			importerSize = lastSize;
			//pixels2Unity = importer.spritePixelsToUnits;
			pixels2Unity = importer.spritePixelsPerUnit;
			importer.npotScale = UnityEditor.TextureImporterNPOTScale.None;
			importer.maxTextureSize = 4096;
			UnityEditor.AssetDatabase.ImportAsset( path, UnityEditor.ImportAssetOptions.ForceUpdate );
			
			int width = texture.width;
			int height = texture.height;

			importer.npotScale = lastScale;
			importer.maxTextureSize = lastSize;
			UnityEditor.AssetDatabase.ImportAsset( path, UnityEditor.ImportAssetOptions.ForceUpdate );
			
			return new Vector2(width, height);
		} else {
			Debug.LogError("TextureImporter is null!");
			return Vector2.zero;
		}
#else
		return Vector2.zero;
#endif

	}
}
