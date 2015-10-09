/***********************************************
				2D MAP FACTORY
	Copyright © 2013-2014 THedgehog Team
	http://www.blitz3dfr.com/teamtalk/index.php
		
	the.hedgehog.team@gmail.com
		
**********************************************/
using UnityEngine;
using System.Collections;

public class SortingLayerFilter{

	public int id;
	public bool show;
	public bool allOrder;
	public int order;
	public bool generate;

	public SortingLayerFilter(){
		show = true;
		allOrder = true;
		generate = true;
		order=0;
	}
}
