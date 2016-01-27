using UnityEngine;
using System.Collections;

public class Cylindermove : MonoBehaviour {

    public float vellocity = 0.1f;
    public GameObject ciylinder;
    

    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {


        if (Input.GetButton("Fire3"))
        {
            this.vellocity = 0.3f;
        }
        else
        {
            this.vellocity = 0.1f;
        }

        //this.transform.Translate(new Vector3(Input.GetAxis("Horizontal") * this.vellocity, 0, Input.GetAxis("Vertical") * this.vellocity));
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 vec = new Vector3(h * this.vellocity, 0, v * this.vellocity);
        this.ciylinder.transform.Translate(vec);

        if (Input.GetButton("Fire1"))
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x + 0.01f, this.transform.localScale.y, this.transform.localScale.z + 0.01f);
        }
        if (Input.GetButton("Fire2"))
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x - 0.01f, this.transform.localScale.y, this.transform.localScale.z - 0.01f);
        }
        //Debug.Log(h +":" + v);
    }

    
}
