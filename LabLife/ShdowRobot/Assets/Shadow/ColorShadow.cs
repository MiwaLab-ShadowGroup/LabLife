using UnityEngine;
using System.Collections;

public class ColorShadow : MonoBehaviour {

    public GameObject shadowColorPlanePrefab;

    void Start()
    {
        for (int h = 0; h <= 360; h++)
        {
            GameObject shadowColorPlane = Instantiate(this.shadowColorPlanePrefab) as GameObject;
            shadowColorPlane.name = this.shadowColorPlanePrefab.name;
            shadowColorPlane.transform.SetParent(this.gameObject.transform);
            shadowColorPlane.GetComponent<Renderer>().material.SetColor("_ShadowColor", new Color(h, 255, 255));
            shadowColorPlane.transform.position = new Vector3((float)h * shadowColorPlane.transform.localScale.x, 0, 0);
        }
    }
}
