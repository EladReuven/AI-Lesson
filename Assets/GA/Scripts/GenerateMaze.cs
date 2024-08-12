using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour {

	public GameObject blockPrefab;
	public GameObject eggPrefab;
	List<GameObject> eggs = new List<GameObject>();
	int width = 40;
	int depth = 40;

	// Use this for initialization
	void Awake ()
	{
		for(int w = 0; w < width; w++)
		{
			for(int d = 0; d < depth; d++)
			{
				GameObject block = null;
				if (w == 0 || d == 0)   //outside walls bottom and left
				{
					block = Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity);
				}
				else if (w == width - 1 || d == depth - 1) //outside walls top and right
				{
					block = Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity);
				}
				else if (w < 4 && d < 4 ||
						 w > (width - 4) && d < 4 ||
						 w > (width - 4) && d > (depth - 4) ||
						 w < 4 && d > (depth - 4)) //starting positions - clear some space
				{
					continue;
				}
				else if (Random.Range(0, 5) < 1)
				{
					block = Instantiate(blockPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity);
				}
				else if (Random.Range(0, 5) < 1)
				{
					block = Instantiate(eggPrefab, new Vector3(w + this.transform.position.x, this.transform.position.y, d + this.transform.position.z), Quaternion.identity);
					eggs.Add(block);
				}
				if (block)
					block.transform.parent = this.transform;

			}	
		}
	}

	//public void Reset ()
	//{
	//	//make all eggs visible again
	//	foreach (GameObject e in eggs)
	//		e.SetActive(true);
	//}
}
