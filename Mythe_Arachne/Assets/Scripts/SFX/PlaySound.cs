﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour {

    public void Play(string name)
    {
        FindObjectOfType<SoundManager>().Play(name);
    }
}
