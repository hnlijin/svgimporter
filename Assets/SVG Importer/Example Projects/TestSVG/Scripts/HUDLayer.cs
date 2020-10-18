using UnityEngine;
using UnityEngine.UI;

public class HUDLayer : MonoBehaviour {
	public GameMap gameMap;
	public GameObject loadingLayer;
	public Text txtLoading;
	private float _time;

	public void showLoading() {
		this.loadingLayer.SetActive(true);
	}

	public void hideLoading() {
		this.loadingLayer.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		// LoadTest3();
		txtLoading.text = "加载中...";
		hideLoading();
	}
	
	// Update is called once per frame
	void Update () {
		this._time += Time.deltaTime;
	}
}
