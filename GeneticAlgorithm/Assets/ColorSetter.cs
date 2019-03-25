using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSetter : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

    Color col = new Color(Random.Range(0.4f, 1), Random.Range(0.4f, 1), Random.Range(0.4f, 1));
    GetComponent<SpriteRenderer>().color = col;

  }

  // Update is called once per frame
  void Update()
  {

  }
}
