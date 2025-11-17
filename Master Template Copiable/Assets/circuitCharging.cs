using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;
using Math = ExMath;

public class circuitCharging : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    static int ModuleIdCounter = 1;
    int ModuleId;
    private bool ModuleSolved;

    static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public GameObject[] segments;
    public GameObject light;
    public GameObject lightEmitter;
    public Material lightOff;
    public Material segmentOff;
    public Material on;

    void Awake() { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        /*
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };

    }

    void OnDestroy() { //Shit you need to do when the bomb ends

    }

    void Activate() { //Shit that should happen when the bomb arrives (factory)/Lights turn on
        toggleLight(true);
    }

    void Start() { //Shit that you calculate, usually a majority if not all of the module

    }

    void Update() { //Shit that happens at any point after initialization

    }

    void Solve() {
        GetComponent<KMBombModule>().HandlePass();
    }

    void Strike() {
        GetComponent<KMBombModule>().HandleStrike();
    }

    void displaySegments(string litSegments)
    {
        //takes 14 digits, either 0 or 1, or a letter. each digit corresponds to a different segment where 0 is off and 1 is on.

        if (litSegments.Length == 1)
        {
            //this is for displaying certain letters. doing this later bc this is probably gonna be tedious and i don't feel like this right now
        }
        else if (litSegments.Length == 0)
        {
            //clear the display
            displaySegments("00000000000000");
        }
        else if (litSegments.Length == 14)
        {
            for (int i = 0; i < 14; i++)
            {
                if (litSegments[i].Equals('0')) { //why can't i just use == this is stupid
                    segments[i].GetComponent<MeshRenderer>().material = segmentOff;
                }
                else
                {
                    segments[i].GetComponent<MeshRenderer>().material = on;
                }
            }
        }
        else
        {
            //failsafe in case i write the wrong number of digits on accident
            Debug.LogFormat("you fucked up lol");
        }
    }

    void toggleLight(bool isOn) {
        if(isOn) {
            light.GetComponent<MeshRenderer>().material = on;
        }
        else
        {
            light.GetComponent<MeshRenderer>().material = lightOff;
        }
        lightEmitter.SetActive(isOn);
    }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
   }
}
