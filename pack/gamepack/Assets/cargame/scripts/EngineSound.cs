using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineSound : MonoBehaviour
{


    car playerCar;
    AudioSource source;

    [SerializeField]
    float modifier = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerCar = GetComponent<car>();
        source = GetComponent<AudioSource>(); 
    }

    // Update is called once per frame
    void Update()
    {
        float soundPitchDiff = 1;
        source.pitch = (playerCar.Speed * 35 / soundPitchDiff) * modifier + .3f;

        while (source.pitch >= 2f)
        {
            source.pitch -= 0.5f;
        }
    }
}
