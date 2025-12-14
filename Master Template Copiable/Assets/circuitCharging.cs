using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Rnd = UnityEngine.Random;

public class circuitCharging : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMBombModule Module;
    public KMAudio Audio;

    private int ModuleId;
    private static int ModuleIdCounter = 1;
    private bool ModuleSolved;

    public KMSelectable[] buttons;

    private static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public GameObject[] segments;
    public Light Light;
    public Material lightOff;
    public Material segmentOff;
    public Material on;
    public GameObject speaker;

    private static readonly bool[][] _segmentArragements = new bool[26][] {
        new bool[14] { true, true, false, false, false, true, true, true, true, false, false, false, true, false },      //A
        new bool[14] { true, false, false, true, false, true, false, true, false, false, true, false, true, true },      //B
        new bool[14] { true, true, false, false, false, false, false, false, true, false, false, false, false, true },   //C
        new bool[14] { true, false, false, true, false, true, false, false, false, false, true, false, true, true },     //D
        new bool[14] { true, true, false, false, false, false, true, true, true, false, false, false, false, true },     //E
        new bool[14] { true, true, false, false, false, false, true, true, true, false, false, false, false, false },    //F
        new bool[14] { true, true, false, false, false, false, false, true, true, false, false, false, true, true },     //G
        new bool[14] { false, true, false, false, false, true, true, true, true, false, false, false, true, false },     //H
        new bool[14] { true, false, false, true, false, false, false, false, false, false, true, false, false, true },   //I
        new bool[14] { false, false, false, false, false, true, false, false, true, false, false, false, true, true },   //J
        new bool[14] { false, true, false, false, true, false, true, false, true, false, false, true, false, false },    //K
        new bool[14] { false, true, false, false, false, false, false, false, true, false, false, false, false, true },  //L
        new bool[14] { false, true, true, false, true, true, false, false, true, false, false, false, true, false },     //M
        new bool[14] { false, true, true, false, false, true, false, false, true, false, false, true, true, false },     //N
        new bool[14] { true, true, false, false, false, true, false, false, true, false, false, false, true, true },     //O
        new bool[14] { true, true, false, false, false, true, true, true, true, false, false, false, false, false },     //P
        new bool[14] { true, true, false, false, false, true, false, false, true, false, false, true, true, true },      //Q
        new bool[14] { true, true, false, false, false, true, true, true, true, false, false, true, false, false },      //R
        new bool[14] { true, true, false, false, false, false, true, true, false, false, false, false, true, true },     //S
        new bool[14] { true, false, false, true, false, false, false, false, false, false, true, false, false, false },  //T
        new bool[14] { false, true, false, false, false, true, false, false, true, false, false, false, true, true },    //U
        new bool[14] { false, true, false, false, true, false, false, false, true, true, false, false, false, false },   //V
        new bool[14] { false, true, false, false, false, true, false, false, true, true, false, true, true, false },     //W
        new bool[14] { false, false, true, false, true, false, false, false, false, true, false, true, false, false },   //X
        new bool[14] { false, false, true, false, true, false, false, false, false, false, true, false, false, false },  //Y
        new bool[14] { true, false, false, false, true, false, false, false, false, true, false, false, false, true }    //Z
    };

    private static readonly bool[] allOff = { false, false, false, false, false, false, false, false, false, false, false, false, false, false };

    private static readonly bool[][] brailleList = new string[26] { "100000", "110000", "100100", "100110", "100010", "110100", "110110", "110010", "010100", "010110", "101000", "111000", "101100", "101110", "101010", "111100", "111110", "111010", "011100", "011110", "101001", "111001", "010111", "101101", "101111", "101011" }.Select(i => i.Select(j => j == '1').ToArray()).ToArray(); //braille list stolen from angel hernandez

    static readonly string[] wordBank = new string[]
       {
                "ABOUT",
                "OTHER",
                "WHICH",
                "THEIR",
                "THERE",
                "FIRST",
                "WOULD",
                "THESE",
                "CLICK",
                "PRICE",
                "STATE",
                "EMAIL",
                "WORLD",
                "MUSIC",
                "AFTER",
                "VIDEO",
                "WHERE",
                "BOOKS",
                "LINKS",
                "YEARS",
                "ORDER",
                "ITEMS",
                "GROUP",
                "UNDER",
                "GAMES",
                "COULD",
                "GREAT",
                "HOTEL",
                "STORE",
                "TERMS",
                "RIGHT",
                "LOCAL",
                "THOSE",
                "USING",
                "PHONE",
                "FORUM",
                "BASED",
                "BLACK",
                "CHECK",
                "INDEX",
                "BEING",
                "WOMEN",
                "TODAY",
                "SOUTH",
                "PAGES",
                "FOUND",
                "HOUSE",
                "PHOTO",
                "POWER",
                "WHILE",
                "THREE",
                "TOTAL",
                "PLACE",
                "THINK",
                "NORTH",
                "POSTS",
                "MEDIA",
                "SINCE",
                "GUIDE",
                "BOARD",
                "WHITE",
                "SMALL",
                "TIMES",
                "SITES",
                "LEVEL",
                "HOURS",
                "IMAGE",
                "TITLE",
                "SHALL",
                "CLASS",
                "STILL",
                "MONEY",
                "EVERY",
                "VISIT",
                "TOOLS",
                "REPLY",
                "VALUE",
                "PRESS",
                "LEARN",
                "PRINT",
                "STOCK",
                "POINT",
                "SALES",
                "LARGE",
                "TABLE",
                "START",
                "MODEL",
                "HUMAN",
                "MOVIE",
                "MARCH",
                "YAHOO",
                "GOING",
                "STUDY",
                "STAFF",
                "AGAIN",
                "APRIL",
                "NEVER",
                "USERS",
                "TOPIC",
                "BELOW",
       };

    private static readonly string[] _hintTypes = new string[6]
    {
        "Light-Speaker",
        "Light-Letter",
        "Speaker-Light",
        "Speaker-Letter",
        "Letter-Light",
        "Letter-Speaker"
    };

    string chosenWord;

    public TextMesh[] inputDisplays;

    string[] currentInput = new string[5];
    int lettersInputted = 0;
    public KMBombModule modSelf;
    bool modFocused = false;

    public GameObject[] resistors;
    public GameObject[] resistorLocations;
    int[] currentSelection = { -1, -1 };
    bool showingHint = false;
    bool[] morsePlaying = new bool[] { false, false };
    bool[] caesarPlaying = { false, false };
    private string[] morseLetters = { ".-", "-...", "-.-.", "-..", ".", "..-.", "--.", "....", "..", ".---", "-.-", ".-..", "--", "-.", "---", ".--.", "--.-", ".-.", "...", "-", "..-", "...-", ".--", "-..-", "-.--", "--.." };

    private KeyCode[] typableKeys =
{
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M,
        KeyCode.Backspace, KeyCode.Return
    };
    const string keyboardLettersInOrder = "QWERTYUIOPASDFGHJKLZXCVBNM";

    void Awake()
    {
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        for (int i = 0; i < buttons.Length; i++)
        {
            int j = i;
            buttons[i].OnInteract += delegate ()
            {
                ButtonPress(j);
                return false;
            };
        }
    }

    void ButtonPress(int button)
    {
        //interaction punch and stuff
        buttons[button].AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, buttons[button].transform);

        if (button > 0) //player places or removes a resistor
        {
            if (Array.IndexOf(currentSelection, button) == -1 && (currentSelection[0] == -1 || currentSelection[1] == -1)) //should check if the player is placing a resistor on an empty spot and both resistors haven't been places
            {
                int resistorToPlace = Array.IndexOf(currentSelection, -1); //get the first unplaced resistor
                currentSelection[resistorToPlace] = button;

                //visually move the resistor to the spot pressed
                resistors[resistorToPlace].transform.position = resistorLocations[button - 1].transform.position;
                resistors[resistorToPlace].SetActive(true);
            }
            else if (Array.IndexOf(currentSelection, button) != -1)
            {
                //basically the same stuff as above
                int resistorToRemove = Array.IndexOf(currentSelection, button);
                currentSelection[resistorToRemove] = -1;
                resistors[resistorToRemove].SetActive(false);
            }
        }
        else if (ModuleSolved == false && showingHint == false) //player presses the charge button and module not solved (keeping the unsolved part for potential souv support)
        {
            StartCoroutine(ShowHint());
        }
    }

    IEnumerator ShowHint()
    {
        int[][] bannedHintsConversion = new int[6][]; //quinn made this a lot harder than it should've been
        bannedHintsConversion[0] = new int[] { 1, 2 };
        bannedHintsConversion[1] = new int[] { 1, 3 };
        bannedHintsConversion[2] = new int[] { 2, 1 };
        bannedHintsConversion[3] = new int[] { 2, 3 };
        bannedHintsConversion[4] = new int[] { 3, 1 };
        bannedHintsConversion[5] = new int[] { 3, 2 };

        showingHint = true;
        if ((currentSelection[0] == 2 && currentSelection[1] == -1) || (currentSelection[0] == -1 && currentSelection[1] == 2)) //player selects just the speaker
        {
            string[] componentList = { "light", "speaker", "letterdisplay" };
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Audio.PlaySoundAtTransform(componentList[bannedHintsConversion[bannedHints[i]][j] - 1], speaker.transform);
                    yield return new WaitForSeconds(1);
                }
            }
        }
        else if (!currentSelection.Contains(-1)) //player tries to play one of the hints
        {
            if ((currentSelection[0] == bannedHintsConversion[bannedHints[0]][0] && currentSelection[1] == bannedHintsConversion[bannedHints[0]][1]) || (currentSelection[0] == bannedHintsConversion[bannedHints[1]][0] && currentSelection[1] == bannedHintsConversion[bannedHints[1]][1])) //banned hint check
            {
                Debug.LogFormat("[Circuit Charging #{0}] You tried to play a banned hint. Strike!", ModuleId);
                Module.HandleStrike();
            }
            else
            {
                if (currentSelection[0] == 1 && currentSelection[1] == 2) //double morse flash
                {
                    morsePlaying = new bool[] { true, true };
                    StartCoroutine(LightMorse());
                    StartCoroutine(SpeakerMorse());

                    while (morsePlaying[0] || morsePlaying[1]) //wait until both finish the morse thing
                    {
                        yield return null;
                    }
                }
                else if (currentSelection[0] == 1 && currentSelection[1] == 3) //show letter and flash position
                {
                    displaySegments(_segmentArragements[alphabet.IndexOf(li_le_letter)]);
                    for (int i = 0; i < li_le_flashes; i++)
                    {
                        toggleLight(true);
                        yield return new WaitForSeconds(0.2f);
                        toggleLight(false);
                        yield return new WaitForSeconds(0.2f);
                    }
                    yield return new WaitForSeconds(0.5f);
                    displaySegments(allOff);
                }
                else if (currentSelection[0] == 2 && currentSelection[1] == 1) //five letters and caesar shift (quinn didn't implement this right)
                {
                    StartCoroutine(LightFlash());
                    StartCoroutine(SpeakerCaesar());
                    while (caesarPlaying[0] || caesarPlaying[1]) //wait until both finish
                    {
                        yield return null;
                    }
                }
                else if (currentSelection[0] == 2 && currentSelection[1] == 3) //beep toggles
                {
                    displaySegments(sp_le_adjustedSegments);
                    for (int i = 0; i < 14; i++)
                    {
                        Audio.PlaySoundAtTransform(sp_le_randomBeeps[i] == true ? "high" : "low", speaker.transform);
                        yield return new WaitForSeconds(0.5f); //can't get a good read off of my own module smh
                    }
                    displaySegments(allOff);
                }
                else if (currentSelection[0] == 3 && currentSelection[1] == 1) //braille
                {
                    displaySegments(_segmentArragements[alphabet.IndexOf(le_li_randomLetter)]);
                    for (int i = 0; i < 6; i++)
                    {
                        toggleLight(true);
                        yield return new WaitForSeconds(le_li_lightsToToggle[i] == false ? 0.2f : 0.4f);
                        toggleLight(false);
                        yield return new WaitForSeconds(0.2f);
                    }
                    displaySegments(allOff);
                }
                else if (currentSelection[0] == 3 && currentSelection[1] == 2) //segment flash
                {
                    morsePlaying = new bool[]{ false, false}; //realized that i didn't need to make caesarPlaying at all but whatever
                    StartCoroutine(SpeakerBeep());
                    StartCoroutine(LetterShuffle());
                    while (morsePlaying[0] || morsePlaying[1]) //wait
                    {
                        yield return null;
                    }
                }
            }
        }
        showingHint = false;
        yield return null; //silence the compiler
    }

    IEnumerator LightMorse()
    {
        foreach (char symbol in morseLetters[alphabet.IndexOf(li_sp_letters[0])])
        {
            toggleLight(true);
            yield return new WaitForSeconds(symbol == '.' ? 0.15f : 0.3f);
            toggleLight(false);
            yield return new WaitForSeconds(0.09f);
        }
        morsePlaying[0] = false;
    }
    IEnumerator SpeakerMorse()
    {
        foreach (char symbol in morseLetters[alphabet.IndexOf(li_sp_letters[1])])
        {
            Audio.PlaySoundAtTransform(symbol == '.' ? "dot" : "dash", speaker.transform);
            yield return new WaitForSeconds(symbol == '.' ? 0.2f : 0.4f);
        }
        morsePlaying[1] = false;
        yield return null;
    }
    IEnumerator LightFlash()
    {
        for (int i = 0; i < sp_li_shift; i++)
        {
            toggleLight(true);
            yield return new WaitForSeconds(0.2f);
            toggleLight(false);
            yield return new WaitForSeconds(0.2f);
        }
        caesarPlaying[0] = false;
    }
    IEnumerator SpeakerCaesar()
    {
        for (int i = 0; i < 5; i++)
        {
            Audio.PlaySoundAtTransform(sp_li_adjustedLetters[i].ToString().ToLower(), speaker.transform);
            yield return new WaitForSeconds(0.65f);
        }
        caesarPlaying[1] = false;
    }
    IEnumerator SpeakerBeep()
    {
        int currentPosition = Rnd.Range(0, 2);
        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i <= (currentPosition == 1 ? le_sp_pos1 : le_sp_pos2); i++)
            {
                Audio.PlaySoundAtTransform("low", speaker.transform);
                yield return new WaitForSeconds(0.5f);
            }
            yield return new WaitForSeconds(1);
            currentPosition = 1 - currentPosition;
        }
        morsePlaying[0] = false;
    }
    IEnumerator LetterShuffle()
    {
        List<int> segmentPositions = new List<int> { };
        bool[] currentDisplay = { false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        for (int i = 0; i < 14; i++)
        {
            if (_segmentArragements[alphabet.IndexOf(le_sp_letter.ToString())][i] == true) //ARRAGEMENTS. HE MADE A TYPO
            {
                segmentPositions.Add(i);
            }
        }
        segmentPositions = segmentPositions.Shuffle();
        for (int i = 0; i < segmentPositions.Count(); i++)
        {
            currentDisplay = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false };
            currentDisplay[segmentPositions[i]] = true;
            displaySegments(currentDisplay);
            yield return new WaitForSeconds(0.5f);
        }
        displaySegments(allOff);
        morsePlaying[1] = false;
    }

    void Activate()
    {
        toggleLight(false);
    }

    int li_sp_ix;
    char[] li_sp_letters;

    int li_le_ix;
    char li_le_letter;
    int li_le_flashes;

    int sp_li_shift;
    int[] sp_li_correctLetterIxs;
    char[] sp_li_ogLetters;
    char[] sp_li_adjustedLetters;

    char sp_le_lastLetter;
    bool[] sp_le_correctSegments;
    bool[] sp_le_randomBeeps;
    bool[] sp_le_adjustedSegments;

    char le_li_firstLetter;
    char le_li_randomLetter;
    bool[] le_li_firstBraille;
    bool[] le_li_randomBraille;
    bool[] le_li_lightsToToggle;

    int le_sp_letterIndex;
    char le_sp_letter;
    bool[] le_sp_segments;
    int le_sp_otherPosition;
    int le_sp_pos1;
    int le_sp_pos2;

    int[] bannedHints;

    void Start()
    {
        modSelf.GetComponent<KMSelectable>().OnFocus += delegate { modFocused = true; };
        modSelf.GetComponent<KMSelectable>().OnDefocus += delegate { modFocused = false; };
        string finalSolution = null;
        int attempts = 0;

        while (attempts++ < 1000 && finalSolution == null) //i honestly have no idea what quinn did in here - r3ked
        {
            chosenWord = wordBank[Rnd.Range(0, wordBank.Length)];

            // Light–Speaker: two adjacent letters (order unknown)
            li_sp_ix = Rnd.Range(0, chosenWord.Length - 1);
            li_sp_letters = new[] { chosenWord[li_sp_ix], chosenWord[li_sp_ix + 1] }.Shuffle();

            // Light–Letter: letter & its position
            li_le_ix = Rnd.Range(0, chosenWord.Length);
            li_le_letter = chosenWord[li_le_ix];
            li_le_flashes = li_le_ix + 1;

            // Speaker–Light: 5 letters, exactly 2 correct positions, ALL shifted backward a random number
            sp_li_shift = Rnd.Range(1, 6);
            sp_li_correctLetterIxs = Enumerable.Range(0, 5).ToArray()
                .Shuffle().Take(2).OrderBy(i => i).ToArray();

            sp_li_ogLetters = new char[5];
            sp_li_adjustedLetters = new char[5];

            for (int i = 0; i < 5; i++)
            {
                if (sp_li_correctLetterIxs.Contains(i))
                {
                    sp_li_ogLetters[i] = chosenWord[i];
                }
                else
                {
                    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Except(new[] { chosenWord[i] });
                    sp_li_ogLetters[i] = possible.PickRandom();
                }

                char c = sp_li_ogLetters[i];
                sp_li_adjustedLetters[i] = (c - sp_li_shift < 'A') ? (char)((c - sp_li_shift) + 26) : (char)(c - sp_li_shift);
            }

            // Speaker–Letter: last letter from segments + beeps
            sp_le_lastLetter = chosenWord.Last();
            sp_le_correctSegments = _segmentArragements[sp_le_lastLetter - 'A'];
            sp_le_randomBeeps = Enumerable.Range(0, 14)
                .Select(ix => Rnd.Range(0, 2) == 0).ToArray();
            sp_le_adjustedSegments = sp_le_correctSegments
                .Select((seg, idx) => seg ^ sp_le_randomBeeps[idx]).ToArray();

            // Letter–Light: first letter via braille inversion
            le_li_firstLetter = chosenWord.First();
            le_li_randomLetter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                .Except(new[] { le_li_firstLetter }).PickRandom();

            le_li_firstBraille = brailleList[le_li_firstLetter - 'A'];
            le_li_randomBraille = brailleList[le_li_randomLetter - 'A'];

            le_li_lightsToToggle = Enumerable.Range(0, 6)
                .Select(ix => le_li_firstBraille[ix] ^ le_li_randomBraille[ix]).ToArray();

            // Letter–Speaker: letter with one real and one faulty position
            le_sp_letterIndex = Rnd.Range(0, 5);
            le_sp_letter = chosenWord[le_sp_letterIndex];
            le_sp_segments = _segmentArragements[le_sp_letter - 'A'].ToArray().Shuffle(); //bad line of code.

            le_sp_otherPosition = Enumerable.Range(0, 5)
                .Except(new[] { le_sp_letterIndex }).PickRandom();  // decoy position

            le_sp_pos1 = le_sp_letterIndex;
            le_sp_pos2 = le_sp_otherPosition;

            // Filter all combos
            var allCombos = wordBank.AsEnumerable();

            bannedHints = Enumerable.Range(0, 6).ToArray()
                .Shuffle().Take(2).OrderBy(x => x).ToArray();

            for (int clue = 0; clue < 6; clue++)
            {
                if (bannedHints.Contains(clue))
                    continue;

                // Light–Speaker (two adjacent letters in either order)
                if (clue == 0)
                {
                    char a = li_sp_letters[0];
                    char b = li_sp_letters[1];

                    allCombos = allCombos.Where(ix =>
                        (ix[li_sp_ix] == a && ix[li_sp_ix + 1] == b) ||
                        (ix[li_sp_ix] == b && ix[li_sp_ix + 1] == a));
                }

                // Light–Letter (letter at known position)
                if (clue == 1)
                {
                    allCombos = allCombos.Where(ix => ix[li_le_ix] == li_le_letter);
                }

                // Speaker–Light (exactly 2 positions match the shifted-forward result)
                if (clue == 2)
                {
                    var target = sp_li_ogLetters;

                    allCombos = allCombos.Where(ix =>
                        Enumerable.Range(0, 5).Count(p => ix[p] == target[p]) == 2);
                }

                // Speaker–Letter (last letter)
                if (clue == 3)
                {
                    allCombos = allCombos.Where(ix => ix[4] == sp_le_lastLetter);
                }

                // Letter–Light (first letter)
                if (clue == 4)
                {
                    allCombos = allCombos.Where(ix => ix[0] == le_li_firstLetter);
                }

                // Letter–Speaker
                if (clue == 5)
                {
                    allCombos = allCombos.Where(ix => ix[le_sp_letterIndex] == le_sp_letter);
                }
            }

            // Uniqueness check
            var remaining = allCombos.Take(2).ToList();

            if (remaining.Count == 1)
                finalSolution = remaining[0];
        }

        if (finalSolution == null)
        {
            Debug.LogFormat("[Circuit Charging #{0}] Puzzle failed to generate after 1000 attempts!", ModuleId);
            Module.HandlePass();
            return;
        }

        Debug.LogFormat("[Circuit Charging #{0}] Chosen word: {1}", ModuleId, chosenWord);
        Debug.LogFormat("[Circuit Charging #{0}] Banned hints: {1}", ModuleId, bannedHints.Select(i => _hintTypes[i]).Join(", "));

        if (!bannedHints.Contains(0))
            Debug.LogFormat("[Circuit Charging #{0}] Light-Speaker: Positions = {1} & {2}, Letters = {3}", ModuleId,
                li_sp_ix + 1, li_sp_ix + 2, li_sp_letters.Join(" "));

        if (!bannedHints.Contains(1))
            Debug.LogFormat("[Circuit Charging #{0}] Light-Letter: Letter = {1}, Position = {2}", ModuleId,
                li_le_letter, li_le_flashes);

        if (!bannedHints.Contains(2))
            Debug.LogFormat("[Circuit Charging #{0}] Speaker-Light: OG letters = {1}, Shift = {2} Adjusted letters = {3}, Correct positions = {4}", ModuleId,
                sp_li_ogLetters.Join(""),
                sp_li_shift,
                sp_li_adjustedLetters.Join(""),
                sp_li_correctLetterIxs.Join(" "));

        if (!bannedHints.Contains(3))
            Debug.LogFormat("[Circuit Charging #{0}] Speaker-Letter: Segments = {1}, Beeps = {2} (Last letter = {3})", ModuleId,
                sp_le_adjustedSegments.Select(i => i ? "1" : "0").Join(""),
                sp_le_randomBeeps.Select(i => i ? "1" : "0").Join(""),
                sp_le_lastLetter);

        if (!bannedHints.Contains(4))
            Debug.LogFormat("[Circuit Charging #{0}] Letter-Light: Random letter = {1}, Flashes = {2} (First letter = {3})", ModuleId,
                le_li_randomLetter,
                le_li_lightsToToggle.Select(i => i ? "1" : "0").Join(""),
                le_li_firstLetter);

        if (!bannedHints.Contains(5))
            Debug.LogFormat("[Circuit Charging #{0}] Letter-Speaker: Random position = {1}, Faulty position = {2}, Letter = {3}", ModuleId,
                le_sp_letterIndex + 1,
                le_sp_otherPosition + 1,
                le_sp_letter);
    }

    void Update()
    {
        if (modFocused)
        {
            for (int i = 0; i < typableKeys.Count(); i++) //this for loop and if loop combined check the key the user pressed
            {
                if (Input.GetKeyDown(typableKeys[i]) && ModuleSolved == false)
                {
                    if (i < 26 && lettersInputted < 5) //player presses a letter key
                    {
                        currentInput[lettersInputted] = keyboardLettersInOrder[i].ToString();
                        inputDisplays[lettersInputted].text = keyboardLettersInOrder[i].ToString();
                        lettersInputted++;
                    }
                    else if (i == 26 && lettersInputted > 0) //player presses backspace
                    {
                        lettersInputted--;
                        currentInput[lettersInputted] = "";
                        inputDisplays[lettersInputted].text = "";
                    }
                    else if (i == 27 && lettersInputted == 5) //player presses enter
                    {
                        Debug.LogFormat("[Circuit Charging #{0}] You submitted {1}.", ModuleId, currentInput[0] + currentInput[1] + currentInput[2] + currentInput[3] + currentInput[4]); //for loops are for losers
                        if (currentInput[0] + currentInput[1] + currentInput[2] + currentInput[3] + currentInput[4] == chosenWord)
                        {
                            Module.HandlePass();
                            Debug.LogFormat("[Circuit Charging #{0}] That is correct. Module solved.", ModuleId);
                            ModuleSolved = true;
                        }
                        else
                        {
                            Module.HandleStrike();
                            Debug.LogFormat("[Circuit Charging #{0}] That is wrong. Strike!", ModuleId);
                        }
                    }
                }
            }
        }
    }


    public static IEnumerable<string> GenerateAllFiveLetterCombos()
    {
        for (char a = 'A'; a <= 'Z'; a++)
            for (char b = 'A'; b <= 'Z'; b++)
                for (char c = 'A'; c <= 'Z'; c++)
                    for (char d = 'A'; d <= 'Z'; d++)
                        for (char e = 'A'; e <= 'Z'; e++)
                            yield return new string(new[] { a, b, c, d, e });
    }

    void displaySegments(bool[] litSegments)
    {
        for (int i = 0; i < litSegments.Length; i++)
        {
            segments[i].GetComponent<MeshRenderer>().material = litSegments[i] ? on : segmentOff;
        }
    }

    void toggleLight(bool isOn)
    {
        Light.enabled = isOn;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
