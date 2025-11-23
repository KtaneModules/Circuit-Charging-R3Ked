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

    public KMSelectable[] buttons;

    static string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public GameObject[] segments;
    public GameObject Light;
    public GameObject lightEmitter;
    public Material lightOff;
    public Material segmentOff;
    public Material on;
    static string[] letterToSegment = //FUUUUUCK I GOTTA DO THIS NOW
    {
        "11000111100010",
        "10010101001011",
        "11000000100001",
        "10010100001011",
        "11000010100001", //5
        "11000010100000",
        "11000001100011",
        "01000111100010",
        "10010000001001",
        "00000100100011", //10
        "01001010100100",
        "01000000100001",
        "01101100100010",
        "01100100100110",
        "11000100100011", //15
        "11000111100000",
        "11000100100111",
        "11000111100100",
        "11000011000011",
        "10010000001000", //20
        "01000100100011", 
        "01001000110000",
        "01000100110110",
        "00101000010100",
        "00101000001000", //25
        "10001000010001" //ok that wasn't as bad as i thought it would be
    };
    int currentLetter = 0;

    //the first digit after the c is the first component and the second is the second component
    string[] c01characters;
    char c02character;
    int c02position;
    string[] c10characters;
    int[] c10positions;
    int c10shift;
    string c12segments;
    string c12flips = "";

    static readonly string[] wordBank = new string[]
       {
                //this is the same word bank linked wordle uses, however i cut a few obscure words for the sake of making it easier to generate a solution and less annoying to find the right word when solving
                //i don't wanna comb through all of this to find the bad ones so i'm just gonna hope this is fine, i've been removing the bs ones as they pop up
                "ABACK", "ABIDE", "ABORT", "ABOUT", "ABOVE", "ABUSE", "ABYSS", "ACIDS", "ACORN", "ACRES", "ACTED", "ACTOR", "ACUTE", "ADAPT", "ADDED", "ADIEU", "ADIOS", "ADMIN", "ADOPT", "ADORE", "ADORN", "ADULT", "AFFIX", "AFTER", "AGAIN", "AGENT", "AGILE", "AGING", "AGONY", "AGORA", "AGREE", "AHEAD", "AIDED", "AIMED", "AIOLI", "AIRED", "AISLE", "ALARM", "ALBUM", "ALERT", "ALGAE", "ALIAS", "ALIBI", "ALIEN", "ALIGN", "ALIKE", "ALIVE", "ALLAY", "ALLEY", "ALLOT", "ALLOW", "ALLOY", "ALOFT", "ALONE", "ALONG", "ALOOF", "ALOUD", "ALPHA", "ALTAR", "ALTER", "AMASS", "AMAZE", "AMBER", "AMBLE", "AMEND", "AMISH", "AMISS", "AMONG", "AMPLE", "AMUSE", "ANGEL", "ANGER", "ANGLE", "ANGLO", "ANGRY", "ANGST", "ANIME", "ANION", "ANISE", "ANKLE", "ANNEX", "ANNOY", "ANNUL", "ANTIC", "ANVIL", "AORTA", "APART", "APNEA", "APPLE", "APPLY", "APRON", "AREAS", "ARENA", "ARGUE", "ARISE", "ARMED", "ARMOR", "AROMA", "AROSE", "ARRAY", "ARROW", "ARSON", "ASHEN", "ASHES", "ASIAN", "ASIDE", "ASKED", "ASSAY", "ASSET", "ASTER", "ASTIR", "ATOLL", "ATOMS", "ATONE", "ATTIC", "AUDIO", "AUDIT", "AUGUR", "AUNTY", "AVAIL", "AVIAN", "AVOID", "AWAIT", "AWAKE", "AWARD", "AWARE", "AWASH", "AWFUL", "AWOKE", "AXIAL", "AXIOM", "AXION", "AZTEC",
                "BACKS", "BACON", "BADGE", "BADLY", "BAKED", "BAKER", "BALLS", "BANDS", "BANKS", "BARGE", "BARON", "BASAL", "BASED", "BASES", "BASIC", "BASIL", "BASIN", "BASIS", "BATCH", "BATHS", "BATTY", "BEACH", "BEADS", "BEAMS", "BEANS", "BEARD", "BEARS", "BEAST", "BEECH", "BEERS", "BEGAN", "BEGIN", "BEGUN", "BEING", "BELLS", "BELLY", "BELOW", "BELTS", "BENCH", "BERRY", "BIBLE", "BIDET", "BIGHT", "BIKES", "BILGE", "BILLS", "BINGE", "BINGO", "BIOME", "BIRCH", "BIRDS", "BIRTH", "BISON", "BITCH", "BITER", "BLACK", "BLADE", "BLAME", "BLAND", "BLANK", "BLARE", "BLAST", "BLAZE", "BLEAK", "BLEAT", "BLEED", "BLEEP", "BLEND", "BLESS", "BLIMP", "BLIND", "BLING", "BLINK", "BLISS", "BLITZ", "BLOCK", "BLOKE", "BLOND", "BLOOD", "BLOOM", "BLOOP", "BLOWN", "BLOWS", "BLUES", "BLUFF", "BLUNT", "BLUSH", "BOARD", "BOATS", "BOGGY", "BOGUS", "BOLTS", "BOMBS", "BONDS", "BONED", "BONER", "BONES", "BONNY", "BONUS", "BOOKS", "BOOST", "BOOTH", "BOOTS", "BORAX", "BORED", "BORER", "BORNE", "BORON", "BOTCH", "BOUGH", "BOULE", "BOUND", "BOWED", "BOWEL", "BOWLS", "BOXED", "BOXER", "BOXES", "BRACE", "BRAID", "BRAIN", "BRAKE", "BRAND", "BRASH", "BRASS", "BRAVE", "BRAWL", "BRAWN", "BRAZE", "BREAD", "BREAK", "BREAM", "BREED", "BRIAR", "BRIBE", "BRICK", "BRIDE", "BRIEF", "BRIER", "BRINE", "BRING", "BRINK", "BRINY", "BRISK", "BROAD", "BROIL", "BROKE", "BROOK", "BROOM", "BROTH", "BROWN", "BROWS", "BRUNT", "BRUSH", "BRUTE", "BUCKS", "BUDDY", "BUDGE", "BUGGY", "BUILD", "BUILT", "BULBS", "BULGE", "BULKY", "BULLS", "BUMPY", "BUNCH", "BUNNY", "BURNS", "BURNT", "BURST", "BUSES", "BUYER", "BUZZY", "BYLAW", "BYWAY",
                "CABBY", "CABIN", "CABLE", "CACHE", "CAIRN", "CAKES", "CALLS", "CALVE", "CAMPS", "CAMPY", "CANAL", "CANDY", "CANED", "CANNY", "CANOE", "CANON", "CARDS", "CARED", "CARER", "CARES", "CARGO", "CAROL", "CARRY", "CARVE", "CASED", "CASES", "CASTE", "CATCH", "CATER", "CAULK", "CAUSE", "CAVES", "CEASE", "CEDED", "CELLS", "CENTS", "CHAFE", "CHAFF", "CHAIN", "CHAIR", "CHALK", "CHAMP", "CHANT", "CHAOS", "CHAPS", "CHARM", "CHART", "CHARY", "CHASE", "CHASM", "CHEAP", "CHEAT", "CHECK", "CHEEK", "CHEER", "CHEMO", "CHESS", "CHEST", "CHICK", "CHIDE", "CHIEF", "CHILD", "CHILI", "CHILL", "CHIME", "CHINA", "CHIPS", "CHOIR", "CHORD", "CHORE", "CHOSE", "CHUCK", "CHUNK", "CHUTE", "CIDER", "CIGAR", "CINCH", "CITED", "CITES", "CIVET", "CIVIC", "CIVIL", "CLADE", "CLAIM", "CLANK", "CLASH", "CLASS", "CLAWS", "CLEAN", "CLEAR", "CLEAT", "CLERK", "CLICK", "CLIFF", "CLIMB", "CLING", "CLOAK", "CLOCK", "CLONE", "CLOSE", "CLOTH", "CLOUD", "CLOUT", "CLOVE", "CLOWN", "CLUBS", "CLUCK", "CLUES", "CLUNG", "CLUNK", "COACH", "COAST", "COATS", "COCOA", "CODES", "COINS", "COLIC", "COLON", "COLOR", "COMAL", "COMES", "COMIC", "COMMA", "CONCH", "CONIC", "CORAL", "CORGI", "CORNY", "CORPS", "COSTS", "COTTA", "COUCH", "COUGH", "COULD", "COUNT", "COURT", "COVEN", "COVER", "COYLY", "CRACK", "CRAFT", "CRANE", "CRANK", "CRASH", "CRASS", "CRATE", "CRAVE", "CRAWL", "CRAZY", "CREAK", "CREAM", "CREED", "CREEK", "CREPT", "CREST", "CREWS", "CRIED", "CRIES", "CRIME", "CRISP", "CRONE", "CROPS", "CROSS", "CROWD", "CROWN", "CRUDE", "CRUEL", "CRUSH", "CRUST", "CRYPT", "CUBAN", "CUBBY", "CUBIC", "CUBIT", "CUMIN", "CURLS", "CURLY", "CURRY", "CURSE", "CURVE", "CUTIE", "CYCLE", "CYNIC", "CZECH",
                "DACHA", "DADDY", "DAILY", "DAIRY", "DAISY", "DALLY", "DANCE", "DARED", "DATED", "DATES", "DATUM", "DEALS", "DEALT", "DEATH", "DEBIT", "DEBTS", "DEBUG", "DEBUT", "DECAF", "DECAL", "DECAY", "DECOR", "DECOY", "DEEDS", "DEIST", "DEITY", "DELAY", "DELFT", "DELVE", "DEMUR", "DENIM", "DENSE", "DEPOT", "DEPTH", "DERBY", "DERRY", "DESKS", "DETER", "DETOX", "DEUCE", "DEVIL", "DIARY", "DICED", "DIETS", "DIGIT", "DIMLY", "DINAR", "DINER", "DINGY", "DIRTY", "DISCO", "DISCS", "DISKS", "DITCH", "DITTY", "DITZY", "DIVAN", "DIVED", "DIVER", "DIVOT", "DIVVY", "DIZZY", "DOCKS", "DODGE", "DODGY", "DOGGY", "DOGMA", "DOING", "DOLLS", "DOMED", "DONOR", "DONUT", "DOORS", "DORIC", "DOSED", "DOSES", "DOTTY", "DOUBT", "DOUGH", "DOUSE", "DOWNS", "DOZEN", "DRAFT", "DRAIN", "DRAMA", "DRANK", "DRAWN", "DRAWS", "DREAD", "DREAM", "DRESS", "DRIED", "DRIER", "DRIFT", "DRILL", "DRILY", "DRINK", "DRIVE", "DROLL", "DRONE", "DROPS", "DROVE", "DROWN", "DRUGS", "DRUMS", "DRUNK", "DRYER", "DUCAT", "DUCHY", "DUCKS", "DUMMY", "DUNCE", "DUNES", "DUSTY", "DUTCH", "DUVET", "DWARF", "DWELL", "DYING",
                "EAGER", "EAGLE", "EARED", "EARLY", "EARTH", "EASED", "EASEL", "EATEN", "EDGES", "EERIE", "EIGHT", "ELATE", "ELBOW", "ELDER", "ELECT", "ELITE", "ELUDE", "ELVES", "EMOTE", "EMPTY", "ENACT", "ENDED", "ENEMY", "ENJOY", "ENSUE", "ENTER", "ENTRY", "ENVOY", "EQUAL", "EQUIP", "ERASE", "ERECT", "ERROR", "ESSAY", "ETHIC", "ETHOS", "ETUDE", "EVADE", "EVENT", "EVERY", "EVICT", "EXACT", "EXALT", "EXAMS", "EXERT", "EXILE", "EXIST", "EXTRA", "EXUDE",
                "FACED", "FACES", "FACTS", "FADED", "FAILS", "FAINT", "FAIRS", "FAIRY", "FAITH", "FALLS", "FALSE", "FAMED", "FANCY", "FARES", "FARMS", "FATAL", "FATED", "FATTY", "FATWA", "FAULT", "FAUNA", "FAVOR", "FEARS", "FEAST", "FECAL", "FEELS", "FEINT", "FELLA", "FENCE", "FERRY", "FETAL", "FETCH", "FEVER", "FEWER", "FIBER", "FIBRE", "FIELD", "FIERY", "FIFTH", "FIFTY", "FIGHT", "FILCH", "FILED", "FILES", "FILET", "FILLE", "FILLS", "FILLY", "FILMS", "FILMY", "FILTH", "FINAL", "FINDS", "FINED", "FINER", "FINES", "FINNY", "FIRED", "FIRES", "FIRMS", "FIRST", "FISTS", "FIVER", "FIXED", "FLAGS", "FLAIL", "FLAIR", "FLAME", "FLANK", "FLARE", "FLASH", "FLASK", "FLATS", "FLAWS", "FLEET", "FLESH", "FLIES", "FLING", "FLIRT", "FLOAT", "FLOCK", "FLOOD", "FLOOR", "FLORA", "FLOUR", "FLOUT", "FLOWN", "FLOWS", "FLUID", "FLUNG", "FLUNK", "FLUSH", "FLUTE", "FLYBY", "FOCAL", "FOCUS", "FOGGY", "FOIST", "FOLDS", "FOLIC", "FOLIO", "FOLKS", "FOLLY", "FONTS", "FOODS", "FOOLS", "FORAY", "FORCE", "FORGE", "FORGO", "FORMS", "FORTE", "FORTH", "FORTY", "FORUM", "FOUND", "FOUNT", "FOURS", "FOVEA", "FOXES", "FOYER", "FRAIL", "FRAME", "FRANC", "FRANK", "FRAUD", "FREAK", "FREED", "FRESH", "FRIED", "FRILL", "FRISK", "FROGS", "FRONT", "FROST", "FROWN", "FROZE", "FRUIT", "FUDGE", "FUELS", "FULLY", "FUMES", "FUNDS", "FUNNY", "FUSED", "FUTON", "FUZZY",
                "GAINS", "GAMES", "GANGS", "GASES", "GATES", "GAUGE", "GAZED", "GEESE", "GENES", "GENIE", "GENRE", "GENUS", "GHOST", "GHOUL", "GIANT", "GIDDY", "GIFTS", "GIMPY", "GIRLS", "GIRLY", "GIRTH", "GIVEN", "GIVES", "GIZMO", "GLAND", "GLARE", "GLASS", "GLEAM", "GLEAN", "GLIAL", "GLIDE", "GLINT", "GLOBE", "GLOOM", "GLORY", "GLOSS", "GLOVE", "GLUED", "GOALS", "GOATS", "GOING", "GOLLY", "GOODS", "GOOFY", "GOOSE", "GORGE", "GRACE", "GRAFT", "GRAIN", "GRAMS", "GRAND", "GRANT", "GRAPE", "GRAPH", "GRASP", "GRASS", "GRATE", "GRAVE", "GRAVY", "GREAT", "GREED", "GREEK", "GREEN", "GREET", "GRIEF", "GRILL", "GRIME", "GRIMY", "GRIND", "GRIPS", "GROIN", "GROOM", "GROSS", "GROUP", "GROUT", "GROWN", "GROWS", "GRUEL", "GRUMP", "GRUNT", "GUANO", "GUARD", "GUAVA", "GUESS", "GUEST", "GUIDE", "GUILD", "GUILT", "GUISE", "GULLS", "GULLY", "GUMMY", "GUNKY", "GUNNY", "GUSHY", "GUSTY", "GUTSY", "GYPSY", "GYRUS",
                "HABIT", "HAIKU", "HAIRS", "HAIRY", "HALAL", "HALLS", "HALVE", "HAMMY", "HANDS", "HANDY", "HANGS", "HAPPY", "HARDY", "HAREM", "HARPY", "HARSH", "HASTE", "HASTY", "HATCH", "HATED", "HATES", "HAUNT", "HAVEN", "HAVOC", "HAZEL", "HEADS", "HEADY", "HEARD", "HEARS", "HEART", "HEATH", "HEAVE", "HEAVY", "HEDGE", "HEELS", "HEFTY", "HEIRS", "HEIST", "HELIX", "HELLO", "HELPS", "HENCE", "HENRY", "HERBS", "HERDS", "HILLS", "HILLY", "HINDU", "HINGE", "HINTS", "HIPPO", "HIRED", "HITCH", "HOBBY", "HOIST", "HOLDS", "HOLES", "HOLLY", "HOMED", "HOMES", "HONEY", "HONOR", "HOOKS", "HOPED", "HOPES", "HORNS", "HORSE", "HOSEL", "HOSTS", "HOTEL", "HOTLY", "HOUND", "HOURS", "HOUSE", "HUBBY", "HUGGY", "HULLO", "HUMAN", "HUMID", "HUMOR", "HUMUS", "HURRY", "HURTS", "HUSKY", "HYENA", "HYMNS",
                "ICHOR", "ICILY", "ICING", "ICONS", "IDEAL", "IDEAS", "IDIOM", "IDIOT", "IDLED", "IDYLL", "IGLOO", "IMAGE", "IMBUE", "IMPLY", "INANE", "INDEX", "INDIA", "INDIE", "INERT", "INFER", "INFRA", "INGOT", "INLET", "INNER", "INPUT", "INTRO", "IRISH", "IRONY", "ISSUE", "ITCHY", "ITEMS", "IVORY",
                "JAPAN", "JEANS", "JELLY", "JEWEL", "JOINS", "JOINT", "JOKER", "JOKES", "JOLLY", "JOULE", "JOUST", "JUDGE", "JUICE", "JUICY", "JUMBO", "JUMPS", "JUNTA",
                "KABOB", "KANJI", "KARAT", "KARMA", "KAYAK", "KAZOO", "KEEPS", "KICKS", "KIDDO", "KILLS", "KINDA", "KINDS", "KINGS", "KITTY", "KNAVE", "KNEAD", "KNEEL", "KNEES", "KNELT", "KNIFE", "KNOBS", "KNOCK", "KNOLL", "KNOTS", "KNOWN", "KNOWS", "KOALA", "KUDOS",
                "LABEL", "LABOR", "LACED", "LACKS", "LADLE", "LAKES", "LAMBS", "LAMPS", "LANDS", "LANES", "LAPIN", "LAPSE", "LARGE", "LARVA", "LASER", "LASSO", "LASTS", "LATCH", "LATER", "LATHE", "LATIN", "LATTE", "LAUGH", "LAWNS", "LAYER", "LAYUP", "LEACH", "LEADS", "LEAFY", "LEAKY", "LEANT", "LEAPT", "LEARN", "LEASE", "LEASH", "LEAST", "LEAVE", "LEDGE", "LEECH", "LEGAL", "LEGGY", "LEMMA", "LEMON", "LEMUR", "LEVEL", "LEVER", "LIANA", "LIBEL", "LIDAR", "LIEGE", "LIFTS", "LIGHT", "LIKED", "LIKEN", "LIKES", "LILAC", "LIMBO", "LIMBS", "LIMIT", "LINED", "LINEN", "LINER", "LINES", "LINGO", "LINKS", "LIONS", "LIPID", "LISTS", "LITER", "LITRE", "LIVED", "LIVEN", "LIVER", "LIVES", "LIVID", "LLAMA", "LOADS", "LOANS", "LOBBY", "LOCAL", "LOCKS", "LOCUS", "LODGE", "LOFTY", "LOGIC", "LOGIN", "LOGON", "LOLLY", "LONER", "LOOKS", "LOONY", "LOOPS", "LOOPY", "LOOSE", "LORDS", "LORRY", "LOSER", "LOSES", "LOTTO", "LOTUS", "LOUSE", "LOUSY", "LOVED", "LOVER", "LOVES", "LOWER", "LOYAL", "LUCID", "LUCKY", "LUCRE", "LUMEN", "LUMPS", "LUMPY", "LUNAR", "LUNCH", "LUNGE", "LUNGS", "LUSTY", "LYING", "LYMPH", "LYNCH", "LYRIC",
                "MACHO", "MADAM", "MADLY", "MAGIC", "MAGMA", "MAINS", "MAIZE", "MAJOR", "MAKER", "MAKES", "MALES", "MAMBO", "MANGO", "MANGY", "MANIA", "MANIC", "MANLY", "MANOR", "MAPLE", "MARCH", "MARKS", "MARRY", "MARSH", "MASKS", "MATCH", "MATED", "MATES", "MATHS", "MATTE", "MAVEN", "MAXIM", "MAYAN", "MAYBE", "MAYOR", "MEALS", "MEANS", "MEANT", "MEATY", "MEDAL", "MEDIA", "MEDIC", "MEETS", "MELON", "MENUS", "MERCY", "MERGE", "MERIT", "MERRY", "MESON", "MESSY", "METAL", "METER", "METRE", "MICRO", "MIDST", "MIGHT", "MILES", "MILLS", "MIMIC", "MINCE", "MINDS", "MINED", "MINER", "MINES", "MINOR", "MINTY", "MINUS", "MIRED", "MIRTH", "MISTY", "MITRE", "MIXED", "MIXER", "MODEL", "MODEM", "MODES", "MOGUL", "MOIST", "MOLAR", "MOLDY", "MOLES", "MONEY", "MONKS", "MONTH", "MOODS", "MOONY", "MOORS", "MOOSE", "MORAL", "MORAY", "MORPH", "MOTEL", "MOTIF", "MOTOR", "MOTTO", "MOULD", "MOUND", "MOUNT", "MOUSE", "MOUTH", "MOVED", "MOVER", "MOVES", "MOVIE", "MUCUS", "MUDDY", "MUMMY", "MUNCH", "MURKY", "MUSED", "MUSIC", "MUSTY", "MUTED", "MUZZY", "MYTHS",
                "NACHO", "NADIR", "NAILS", "NAIVE", "NAKED", "NAMED", "NAMES", "NANNY", "NASAL", "NASTY", "NATTY", "NECKS", "NEEDS", "NEEDY", "NEIGH", "NERVE", "NESTS", "NEVER", "NEWER", "NEWLY", "NEXUS", "NICER", "NICHE", "NIECE", "NIFTY", "NIGHT", "NINJA", "NINTH", "NITRO", "NOBLE", "NOBLY", "NODES", "NOISE", "NOISY", "NOMAD", "NOMES", "NONCE", "NOOSE", "NORMS", "NORTH", "NOSES", "NOTCH", "NOTED", "NOTES", "NOVEL", "NUDGE", "NURSE", "NUTTY", "NYLON", "NYMPH",
                "OASIS", "OCCUR", "OCEAN", "ODDLY", "ODOUR", "OFFER", "OFTEN", "OILED", "OLDER", "OLDIE", "OLIVE", "ONION", "ONSET", "OOMPH", "OPENS", "OPERA", "OPINE", "OPIUM", "OPTIC", "ORBIT", "ORDER", "ORGAN", "OTHER", "OTTER", "OUGHT", "OUNCE", "OUTDO", "OUTER", "OVERS", "OWNED", "OWNER", "OXBOW", "OXIDE", "OZONE",
                "PACKS", "PADDY", "PAGES", "PAINS", "PAINT", "PAIRS", "PALMS", "PANDA", "PANEL", "PANIC", "PANTS", "PAPER", "PARKS", "PARTS", "PARTY", "PASTA", "PASTE", "PATCH", "PATHS", "PATIO", "PAUSE", "PEACE", "PEACH", "PEAKS", "PEARL", "PEARS", "PEDAL", "PEERS", "PENNY", "PERIL", "PESTS", "PETTY", "PHASE", "PHONE", "PHOTO", "PIANO", "PICKS", "PIECE", "PIERS", "PIGGY", "PILAF", "PILED", "PILES", "PILLS", "PILOT", "PINCH", "PINTS", "PIOUS", "PIPES", "PISTE", "PITCH", "PIVOT", "PIXEL", "PIXIE", "PIZZA", "PLACE", "PLAIN", "PLAIT", "PLANE", "PLANK", "PLANS", "PLANT", "PLATE", "PLAYS", "PLAZA", "PLEAD", "PLEAS", "PLEAT", "PLOTS", "PLUMB", "PLUME", "PLUMP", "POEMS", "POETS", "POINT", "POKER", "POLAR", "POLES", "POLIO", "POLLS", "POLYP", "PONDS", "POOLS", "PORCH", "PORES", "PORTS", "POSED", "POSES", "POSIT", "POSTS", "POUCH", "POUND", "POWER", "PREEN", "PRESS", "PRICE", "PRICY", "PRIDE", "PRIMA", "PRIME", "PRIMP", "PRINT", "PRION", "PRIOR", "PRISE", "PRISM", "PRIVY", "PRIZE", "PROBE", "PROMO", "PRONE", "PRONG", "PROOF", "PROSE", "PROUD", "PROVE", "PROXY", "PRUDE", "PRUNE", "PUDGY", "PULLS", "PULSE", "PUMPS", "PUNCH", "PUPIL", "PUPPY", "PURSE", "PYLON",
                "QUACK", "QUAIL", "QUALM", "QUARK", "QUART", "QUASI", "QUEEN", "QUELL", "QUERY", "QUEST", "QUEUE", "QUICK", "QUIET", "QUILL", "QUILT", "QUINT", "QUIRK", "QUITE", "QUOTA", "QUOTE",
                "RABBI", "RACED", "RACES", "RADAR", "RADIO", "RAGGY", "RAIDS", "RAILS", "RAINY", "RAISE", "RALLY", "RAMPS", "RANCH", "RANGE", "RANGY", "RANKS", "RAPID", "RATED", "RATES", "RATIO", "RATTY", "RAVEN", "RAZOR", "REACH", "REACT", "READS", "READY", "REALM", "REARM", "REBEL", "RECAP", "RECON", "RECTO", "REDLY", "REEDY", "REFER", "REHAB", "REIGN", "REINS", "RELAX", "RELAY", "RELIC", "REMIT", "REMIX", "RENAL", "RENEW", "RENTS", "REPAY", "REPLY", "RESIN", "RESTS", "RETRO", "REUSE", "RHINO", "RHYME", "RIDER", "RIDGE", "RIFLE", "RIGHT", "RIGID", "RIGOR", "RILED", "RINGS", "RINSE", "RIOTS", "RISEN", "RISES", "RISKS", "RISKY", "RITES", "RITZY", "RIVAL", "RIVEN", "RIVER", "RIVET", "ROADS", "ROAST", "ROBES", "ROBOT", "ROCKS", "ROCKY", "ROGUE", "ROILY", "ROLES", "ROLLS", "ROMAN", "ROOFS", "ROOMS", "ROOMY", "ROOTS", "ROPES", "ROSES", "ROSIN", "ROTOR", "ROUGE", "ROUGH", "ROUND", "ROUTE", "ROVER", "ROYAL", "RUDDY", "RUGBY", "RUINS", "RULED", "RULER", "RULES", "RUMBA", "RUMMY", "RUMOR", "RUNIC", "RUNNY", "RUNTY", "RURAL", "RUSTY",
                "SABLE", "SADLY", "SAFER", "SAGGY", "SAILS", "SAINT", "SALAD", "SALES", "SALLY", "SALON", "SALSA", "SALTS", "SALTY", "SALVE", "SAMBA", "SANDS", "SANDY", "SATED", "SATIN", "SATYR", "SAUCE", "SAUCY", "SAUNA", "SAVED", "SAVER", "SAVES", "SAVOR", "SAVVY", "SCALD", "SCALE", "SCALP", "SCALY", "SCAMP", "SCANT", "SCAPE", "SCARE", "SCARF", "SCARP", "SCARS", "SCARY", "SCENE", "SCENT", "SCHMO", "SCOFF", "SCOLD", "SCONE", "SCOOP", "SCOOT", "SCOPE", "SCORE", "SCORN", "SCOTS", "SCOUR", "SCOUT", "SCRAM", "SCRAP", "SCREW", "SCRIM", "SCRIP", "SCRUB", "SCRUM", "SCUBA", "SEALS", "SEAMS", "SEATS", "SEEDS", "SEEDY", "SEEKS", "SEEMS", "SEGUE", "SEIZE", "SELLS", "SENDS", "SENSE", "SERUM", "SERVE", "SETUP", "SEVEN", "SHADE", "SHADY", "SHAFT", "SHAKE", "SHAKY", "SHALE", "SHALL", "SHAME", "SHANK", "SHAPE", "SHARD", "SHARE", "SHARP", "SHAVE", "SHAWL", "SHEAF", "SHEAR", "SHEEN", "SHEEP", "SHEER", "SHEET", "SHELF", "SHELL", "SHIFT", "SHILL", "SHINE", "SHINY", "SHIPS", "SHIRE", "SHIRT", "SHOCK", "SHOES", "SHONE", "SHOOK", "SHOOT", "SHOPS", "SHORE", "SHORN", "SHORT", "SHOTS", "SHOUT", "SHOVE", "SHOWN", "SHOWS", "SHRUG", "SHUNT", "SHUSH", "SIDES", "SIDLE", "SIEGE", "SIGHT", "SIGIL", "SIGNS", "SILLY", "SILTY", "SINCE", "SINEW", "SINGE", "SINGS", "SINUS", "SITAR", "SITES", "SIXTH", "SIXTY", "SIZED", "SIZES", "SKALD", "SKANK", "SKATE", "SKEIN", "SKIER", "SKIES", "SKIFF", "SKILL", "SKIMP", "SKINS", "SKIRT", "SKULL", "SLABS", "SLAIN", "SLAKE", "SLANG", "SLANT", "SLASH", "SLATE", "SLAVE", "SLEEK", "SLEEP", "SLEET", "SLEPT", "SLICE", "SLIDE", "SLIME", "SLIMY", "SLING", "SLINK", "SLOPE", "SLOSH", "SLOTH", "SLOTS", "SLUMP", "SLUSH", "SLYLY", "SMALL", "SMART", "SMASH", "SMEAR", "SMELL", "SMELT", "SMILE", "SMITE", "SMOKE", "SNAIL", "SNAKE", "SNARE", "SNARL", "SNEER", "SNIDE", "SNIFF", "SNIPE", "SNOOP", "SNORE", "SNORT", "SNOUT", "SOBER", "SOCKS", "SOFTY", "SOGGY", "SOILS", "SOLAR", "SOLID", "SOLVE", "SONAR", "SONGS", "SONIC", "SOOTH", "SORRY", "SORTS", "SOUGH", "SOULS", "SOUND", "SOUTH", "SPACE", "SPADE", "SPAIN", "SPARE", "SPARK", "SPATE", "SPAWN", "SPEAK", "SPEED", "SPELL", "SPEND", "SPENT", "SPIES", "SPINE", "SPLAT", "SPLIT", "SPOIL", "SPOKE", "SPOON", "SPORT", "SPOTS", "SPRAY", "SPURS", "SQUAD", "STACK", "STAFF", "STAGE", "STAIN", "STAIR", "STAKE", "STALE", "STALL", "STAMP", "STAND", "STARE", "STARK", "STARS", "START", "STASH", "STATE", "STAYS", "STEAD", "STEAK", "STEAL", "STEAM", "STEEL", "STEEP", "STEER", "STEMS", "STENO", "STEPS", "STERN", "STICK", "STIFF", "STILE", "STILL", "STILT", "STING", "STINK", "STINT", "STOCK", "STOIC", "STOKE", "STOLE", "STOMP", "STONE", "STONY", "STOOD", "STOOL", "STOOP", "STOPS", "STORE", "STORK", "STORM", "STORY", "STOUT", "STOVE", "STRAP", "STRAW", "STRAY", "STREP", "STREW", "STRIP", "STRUM", "STRUT", "STUCK", "STUDY", "STUFF", "STUMP", "STUNT", "STYLE", "SUAVE", "SUEDE", "SUGAR", "SUITE", "SUITS", "SULLY", "SUNNY", "SUNUP", "SUPER", "SURGE", "SUSHI", "SWALE", "SWAMI", "SWAMP", "SWANK", "SWANS", "SWARD", "SWARM", "SWASH", "SWATH", "SWEAR", "SWEAT", "SWEEP", "SWEET", "SWELL", "SWEPT", "SWIFT", "SWILL", "SWINE", "SWING", "SWIPE", "SWIRL", "SWISH", "SWISS", "SWOON", "SWOOP", "SWORD", "SWORE", "SWORN", "SWUNG",
                "TABLE", "TACIT", "TAFFY", "TAILS", "TAINT", "TAKEN", "TAKES", "TALES", "TALKS", "TALLY", "TALON", "TAMED", "TANGO", "TANGY", "TANKS", "TAPES", "TARDY", "TAROT", "TARRY", "TASKS", "TASTE", "TASTY", "TATTY", "TAUNT", "TAWNY", "TAXED", "TAXES", "TAXIS", "TAXON", "TEACH", "TEAMS", "TEARS", "TEARY", "TEASE", "TECHY", "TEDDY", "TEENS", "TEENY", "TEETH", "TELLS", "TELLY", "TEMPO", "TENDS", "TENOR", "TENSE", "TENTH", "TENTS", "TERMS", "TESTS", "TEXAS", "TEXTS", "THANK", "THEFT", "THEIR", "THEME", "THERE", "THESE", "THETA", "THICK", "THIEF", "THIGH", "THINE", "THING", "THINK", "THIRD", "THONG", "THORN", "THOSE", "THREE", "THREW", "THROW", "THUMB", "TIARA", "TIBIA", "TIDAL", "TIDES", "TIGER", "TIGHT", "TILDE", "TILED", "TILES", "TILTH", "TIMED", "TIMER", "TIMES", "TIMID", "TINES", "TINNY", "TIPSY", "TIRED", "TITLE", "TOAST", "TODAY", "TOKEN", "TOMMY", "TONAL", "TONED", "TONES", "TONGS", "TONIC", "TONNE", "TOOLS", "TOONS", "TOOTH", "TOPAZ", "TOPIC", "TORCH", "TORSO", "TORTE", "TORUS", "TOTAL", "TOTEM", "TOUCH", "TOUGH", "TOURS", "TOWEL", "TOWER", "TOWNS", "TOXIC", "TOXIN", "TRACE", "TRACK", "TRACT", "TRADE", "TRAIL", "TRAIN", "TRAIT", "TRAMP", "TRAMS", "TRASH", "TRAWL", "TRAYS", "TREAD", "TREAT", "TREES", "TREND", "TRIAD", "TRIAL", "TRIBE", "TRICK", "TRIED", "TRIES", "TRIKE", "TRILL", "TRIPS", "TRITE", "TROLL", "TROOP", "TROUT", "TRUCE", "TRUCK", "TRULY", "TRUNK", "TRUST", "TRUTH", "TUBBY", "TUBES", "TULIP", "TUMMY", "TUNED", "TUNES", "TUNIC", "TURKS", "TURNS", "TUTEE", "TUTOR", "TWANG", "TWEAK", "TWICE", "TWINS", "TWIRL", "TWIST", "TYING", "TYPES", "TYRES",
                "UDDER", "ULCER", "ULTRA", "UNBAN", "UNCAP", "UNCLE", "UNCUT", "UNDER", "UNDUE", "UNFED", "UNFIT", "UNHIP", "UNIFY", "UNION", "UNITE", "UNITS", "UNITY", "UNLIT", "UNMET", "UNSAY", "UNTIE", "UNTIL", "UNZIP", "UPPER", "UPSET", "URBAN", "URGED", "URINE", "USAGE", "USERS", "USHER", "USING", "USUAL", "UTTER", "UVULA",
                "VAGUE", "VALET", "VALID", "VALOR", "VALUE", "VALVE", "VAPOR", "VAULT", "VAUNT", "VEINS", "VEINY", "VENAL", "VENOM", "VENUE", "VERBS", "VERGE", "VERSE", "VICAR", "VIDEO", "VIEWS", "VIGIL", "VIGOR", "VILLA", "VINES", "VINYL", "VIRAL", "VIRUS", "VISIT", "VISOR", "VITAL", "VIVID", "VIXEN", "VOCAL", "VODKA", "VOGUE", "VOICE", "VOTED", "VOTER", "VOTES", "VOUCH", "VOWED", "VOWEL", "VROOM",
                "WAGES", "WAGON", "WAIST", "WAITS", "WAIVE", "WALKS", "WALLS", "WALTZ", "WANTS", "WARDS", "WARES", "WARNS", "WASTE", "WATCH", "WATER", "WAVED", "WAVES", "WAXEN", "WEARS", "WEARY", "WEAVE", "WEBBY", "WEDGE", "WEEDS", "WEEKS", "WEIGH", "WEIRD", "WELLS", "WELSH", "WETLY", "WHALE", "WHEAT", "WHEEL", "WHERE", "WHICH", "WHILE", "WHINE", "WHISK", "WHITE", "WHOLE", "WHORL", "WHOSE", "WIDEN", "WIDER", "WIDOW", "WIDTH", "WIELD", "WILLS", "WIMPY", "WINCE", "WINCH", "WINDS", "WINDY", "WINES", "WINGS", "WIPED", "WIRED", "WIRES", "WISER", "WITCH", "WITTY", "WIVES", "WOKEN", "WOMAN", "WOMEN", "WOODS", "WORDS", "WORKS", "WORLD", "WORMS", "WORMY", "WORRY", "WORSE", "WORST", "WORTH", "WOULD", "WOUND", "WOVEN", "WRATH", "WRECK", "WRIST", "WRITE", "WRONG", "WROTE",
                "YACHT", "YARDS", "YAWNS", "YEARN", "YEARS", "YEAST", "YELLS", "YIELD", "YODEL", "YOUNG", "YOURS", "YOUTH", "YUMMY",
                "ZEBRA", "ZILCH", "ZONES"
       };
    string chosenWord;
    List<string> possibleWords;
    List<string> possibleWordsThisGeneration = new List<string> {"adfahdsjgasldkfghasdgf"}; //this random keysmash is added to the list at the start so that c# doesn't shit its pants later down the line when i need to clear it to generate hints
    int wordsLeft;
    int wordsLeftThisGeneration = 9999;

    void Awake() { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
        ModuleId = ModuleIdCounter++;
        GetComponent<KMBombModule>().OnActivate += Activate;
        foreach (KMSelectable Button in buttons)
        {
            Button.OnInteract += delegate () { ButtonPress(Button); return false; };
        }
        /*
        foreach (KMSelectable object in keypad) {
            object.OnInteract += delegate () { keypadPress(object); return false; };
        }
        */

        //button.OnInteract += delegate () { buttonPress(); return false; };

    }

    void ButtonPress(KMSelectable button)
    {

    }

    void OnDestroy() { //Shit you need to do when the bomb ends

    }

    void Activate() { //Shit that should happen when the bomb arrives (factory)/Lights turn on
        toggleLight(true);
    }

    void Start() { //Shit that you calculate, usually a majority if not all of the module
        chosenWord = wordBank[UnityEngine.Random.Range(0, wordBank.Length)]; //pick a random word for the solution
        Debug.LogFormat("[Circuit Charging #{0}] The word chosen is {1}.", ModuleId, chosenWord);
        possibleWords = wordBank.ToList(); //copy the word bank into the possible words list
        wordsLeft = wordBank.Length;

        while (wordsLeftThisGeneration >= wordBank.Length / 2)
        {
            generateHint(1, 0);
        }
        Debug.Log(wordsLeftThisGeneration);
        foreach (string i in possibleWordsThisGeneration)
        {
            Debug.Log(i);
        }
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
            //display a certain letter
            displaySegments(letterToSegment[alphabet.IndexOf(litSegments)]);
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
                if (litSegments[i] == '0') {
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
            Light.GetComponent<MeshRenderer>().material = on;
        }
        else
        {
            Light.GetComponent<MeshRenderer>().material = lightOff;
        }
        lightEmitter.SetActive(isOn);
    }

    void generateHint(int component1, int component2)
    {
        //reset stuff from last hint generation
        possibleWordsThisGeneration.Clear();
        wordsLeftThisGeneration = 0;

        //0 is light, 1 is speaker, and 2 is letter display
        if (component1 == 0 && component2 == 1)
        {
            int startingCharacter = UnityEngine.Random.Range(0, 4);
            c01characters = new string[] { chosenWord[startingCharacter].ToString(), chosenWord[startingCharacter + 1].ToString() }; // i don't want to use c# anymore. this language fucking sucks

            //loop through each word left and if it's a possibility
            foreach (string i in possibleWords)
            {
                if (i.Contains(c01characters[0] + c01characters[1]) || i.Contains(c01characters[1] + c01characters[0]))
                {
                    possibleWordsThisGeneration.Add(i);
                    wordsLeftThisGeneration++;
                }
            }

            //50% chance to flip the two characters so the order doesn't matter
            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                c01characters = new string[] { c01characters[1], c01characters[0] };
            }
        }
        else if (component1 == 0 && component2 == 2)
        {
            c02position = UnityEngine.Random.Range(0, 5);
            c02character = chosenWord[c02position];
            foreach (string i in possibleWords)
            {
                if (i[c02position] == c02character)
                {
                    possibleWordsThisGeneration.Add(i);
                    wordsLeftThisGeneration++;
                }
            }
            c02position++;
        }
        else if (component1 == 1 && component2 == 0)
        {
            c10characters = new string[] {"", "", "", "", ""};
            c10positions = new int[] { -1, -1 };
            
            //get two random positions for the letters to be in
            while (c10positions[0] == c10positions[1])
            {
                c10positions[0] = UnityEngine.Random.Range(0, 5);
                c10positions[1] = UnityEngine.Random.Range(0, 5);
            }
            c10characters[c10positions[0]] = chosenWord[c10positions[0]].ToString();
            c10characters[c10positions[1]] = chosenWord[c10positions[1]].ToString();

            //fill the remaining slots with decoy letters
            for(int i = 0; i < 5; i++)
            {
                if (c10characters[i] == "")
                {
                    while (c10characters[i] == chosenWord[i].ToString() || c10characters[i] == "")
                    {
                        c10characters[i] = alphabet[UnityEngine.Random.Range(0, 26)].ToString();
                    }
                }
            }
            //check for valid words
            foreach (string i in possibleWords)
            {
                int matches = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (i[j].ToString() == c10characters[j])
                    {
                        matches++;
                    }
                }
                if (matches >= 2)
                {
                    possibleWordsThisGeneration.Add(i);
                    wordsLeftThisGeneration++;
                }
            }
            //caesar shift
            Debug.Log(c10characters[0] + c10characters[1] + c10characters[2] + c10characters[3] + c10characters[4]);
            c10shift = UnityEngine.Random.Range(1, 6);
            for (int i = 0; i < 5; i++)
            {
                int position = alphabet.IndexOf(c10characters[i]); // get the position of the character being shifted in the alphabet
                position -= c10shift;
                if (position < 0)
                {
                    position += 26;
                }
                c10characters[i] = alphabet[position].ToString();
            } //everything so far has worked first try somehow. am i dreaming or something
        } //cry me a river, never nesters.
        else if (component1 == 1 && component2 == 2)
        { 
            //convert the last letter to segments
            c12segments = letterToSegment[alphabet.IndexOf(chosenWord[4].ToString())];

            //randomly generate flipped segments
            for (int i = 0; i < 15; i++)
            {
                c12flips += UnityEngine.Random.Range(0, 2).ToString();
            }
        }
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
