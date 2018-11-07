using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimteSlider : MonoBehaviour {

    private Slider slider;
	void Start () {

        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener((a) => { slider.value = (int)a; });
	}
}
