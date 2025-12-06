using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditorInternal;
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

    private static readonly bool[][] brailleList = new string[26] { "100000", "110000", "100100", "100110", "100010", "110100", "110110", "110010", "010100", "010110", "101000", "111000", "101100", "101110", "101010", "111100", "111110", "111010", "011100", "011110", "101001", "111001", "010111", "101101", "101111", "101011" }.Select(i => i.Select(j => j == '1').ToArray()).ToArray(); //braille list stolen from angel hernandez

    static readonly string[] wordBank = new string[]
       {
                "ABACK", "ABIDE", "ABORT", "ABOUT", "ABOVE", "ABUSE", "ABYSS", "ACIDS", "ACORN", "ACRES", "ACTED", "ACTOR", "ACUTE", "ADAPT", "ADDED", "ADIEU", "ADIOS", "ADMIN", "ADOPT", "ADORE", "ADORN", "ADULT", "AFFIX", "AFTER", "AGAIN", "AGENT", "AGILE", "AGING", "AGONY", "AGORA", "AGREE", "AHEAD", "AIDED", "AIMED", "AIOLI", "AIRED", "AISLE", "ALARM", "ALBUM", "ALERT", "ALGAE", "ALIAS", "ALIBI", "ALIEN", "ALIGN", "ALIKE", "ALIVE", "ALLAY", "ALLEY", "ALLOT", "ALLOW", "ALLOY", "ALOFT", "ALONE", "ALONG", "ALOOF", "ALOUD", "ALPHA", "ALTAR", "ALTER", "AMASS", "AMAZE", "AMBER", "AMBLE", "AMEND", "AMISH", "AMISS", "AMONG", "AMPLE", "AMUSE", "ANGEL", "ANGER", "ANGLE", "ANGLO", "ANGRY", "ANGST", "ANIME", "ANION", "ANISE", "ANKLE", "ANNEX", "ANNOY", "ANNUL", "ANTIC", "ANVIL", "AORTA", "APART", "APNEA", "APPLE", "APPLY", "APRON", "AREAS", "ARENA", "ARGUE", "ARISE", "ARMED", "ARMOR", "AROMA", "AROSE", "ARRAY", "ARROW", "ARSON", "ASHEN", "ASHES", "ASIAN", "ASIDE", "ASKED", "ASSAY", "ASSET", "ASTER", "ASTIR", "ATOLL", "ATOMS", "ATONE", "ATTIC", "AUDIO", "AUDIT", "AUGUR", "AUNTY", "AVAIL", "AVIAN", "AVOID", "AWAIT", "AWAKE", "AWARD", "AWARE", "AWASH", "AWFUL", "AWOKE", "AXIAL", "AXIOM", "AXION", "AZTEC",
                "BACKS", "BACON", "BADGE", "BADLY", "BAKED", "BAKER", "BALLS", "BANDS", "BANKS", "BARGE", "BARON", "BASED", "BASES", "BASIC", "BASIL", "BASIN", "BASIS", "BATCH", "BATHS", "BATTY", "BEACH", "BEADS", "BEAMS", "BEANS", "BEARD", "BEARS", "BEAST", "BEECH", "BEERS", "BEGAN", "BEGIN", "BEGUN", "BEING", "BELLS", "BELLY", "BELOW", "BELTS", "BENCH", "BERRY", "BIBLE", "BIDET", "BIGHT", "BIKES", "BILGE", "BILLS", "BINGE", "BINGO", "BIOME", "BIRCH", "BIRDS", "BIRTH", "BISON", "BITCH", "BITER", "BLACK", "BLADE", "BLAME", "BLAND", "BLANK", "BLARE", "BLAST", "BLAZE", "BLEAK", "BLEAT", "BLEED", "BLEEP", "BLEND", "BLESS", "BLIMP", "BLIND", "BLING", "BLINK", "BLISS", "BLITZ", "BLOCK", "BLOKE", "BLOND", "BLOOD", "BLOOM", "BLOOP", "BLOWN", "BLOWS", "BLUES", "BLUFF", "BLUNT", "BLUSH", "BOARD", "BOATS", "BOGGY", "BOGUS", "BOLTS", "BOMBS", "BONDS", "BONED", "BONES", "BONNY", "BONUS", "BOOKS", "BOOST", "BOOTH", "BOOTS", "BORAX", "BORED", "BORER", "BORNE", "BORON", "BOTCH", "BOUGH", "BOULE", "BOUND", "BOWED", "BOWEL", "BOWLS", "BOXED", "BOXER", "BOXES", "BRACE", "BRAID", "BRAIN", "BRAKE", "BRAND", "BRASH", "BRASS", "BRAVE", "BRAWL", "BRAWN", "BRAZE", "BREAD", "BREAK", "BREAM", "BREED", "BRIAR", "BRIBE", "BRICK", "BRIDE", "BRIEF", "BRIER", "BRINE", "BRING", "BRINK", "BRINY", "BRISK", "BROAD", "BROIL", "BROKE", "BROOK", "BROOM", "BROTH", "BROWN", "BROWS", "BRUNT", "BRUSH", "BRUTE", "BUCKS", "BUDDY", "BUDGE", "BUGGY", "BUILD", "BUILT", "BULBS", "BULGE", "BULKY", "BULLS", "BUMPY", "BUNCH", "BUNNY", "BURNS", "BURNT", "BURST", "BUSES", "BUYER", "BUZZY", "BYLAW", "BYWAY",
                "CABBY", "CABIN", "CABLE", "CACHE", "CAIRN", "CAKES", "CALLS", "CALVE", "CAMPS", "CAMPY", "CANAL", "CANDY", "CANED", "CANNY", "CANOE", "CANON", "CARDS", "CARED", "CARER", "CARES", "CARGO", "CAROL", "CARRY", "CARVE", "CASED", "CASES", "CASTE", "CATCH", "CATER", "CAULK", "CAUSE", "CAVES", "CEASE", "CEDED", "CELLS", "CENTS", "CHAFE", "CHAFF", "CHAIN", "CHAIR", "CHALK", "CHAMP", "CHANT", "CHAOS", "CHAPS", "CHARM", "CHART", "CHARY", "CHASE", "CHASM", "CHEAP", "CHEAT", "CHECK", "CHEEK", "CHEER", "CHEMO", "CHESS", "CHEST", "CHICK", "CHIDE", "CHIEF", "CHILD", "CHILI", "CHILL", "CHIME", "CHINA", "CHIPS", "CHOIR", "CHORD", "CHORE", "CHOSE", "CHUCK", "CHUNK", "CHUTE", "CIDER", "CIGAR", "CINCH", "CITED", "CITES", "CIVET", "CIVIC", "CIVIL", "CLADE", "CLAIM", "CLANK", "CLASH", "CLASS", "CLAWS", "CLEAN", "CLEAR", "CLEAT", "CLERK", "CLICK", "CLIFF", "CLIMB", "CLING", "CLOAK", "CLOCK", "CLONE", "CLOSE", "CLOTH", "CLOUD", "CLOUT", "CLOVE", "CLOWN", "CLUBS", "CLUCK", "CLUES", "CLUNG", "CLUNK", "COACH", "COAST", "COATS", "COCOA", "CODES", "COINS", "COLIC", "COLON", "COLOR", "COMAL", "COMES", "COMIC", "COMMA", "CONCH", "CONIC", "CORAL", "CORGI", "CORNY", "CORPS", "COSTS", "COTTA", "COUCH", "COUGH", "COULD", "COUNT", "COURT", "COVEN", "COVER", "COYLY", "CRACK", "CRAFT", "CRANE", "CRANK", "CRASH", "CRASS", "CRATE", "CRAVE", "CRAWL", "CRAZY", "CREAK", "CREAM", "CREED", "CREEK", "CREPT", "CREST", "CREWS", "CRIED", "CRIES", "CRIME", "CRISP", "CRONE", "CROPS", "CROSS", "CROWD", "CROWN", "CRUDE", "CRUEL", "CRUSH", "CRUST", "CRYPT", "CUBAN", "CUBBY", "CUBIC", "CUBIT", "CUMIN", "CURLS", "CURLY", "CURRY", "CURSE", "CURVE", "CUTIE", "CYCLE", "CYNIC", "CZECH",
                "DADDY", "DAILY", "DAIRY", "DAISY", "DALLY", "DANCE", "DARED", "DATED", "DATES", "DATUM", "DEALS", "DEALT", "DEATH", "DEBIT", "DEBTS", "DEBUG", "DEBUT", "DECAF", "DECAL", "DECAY", "DECOR", "DECOY", "DEEDS", "DEIST", "DEITY", "DELAY", "DELFT", "DELVE", "DEMUR", "DENIM", "DENSE", "DEPOT", "DEPTH", "DERBY", "DERRY", "DESKS", "DETER", "DETOX", "DEUCE", "DEVIL", "DIARY", "DICED", "DIETS", "DIGIT", "DIMLY", "DINAR", "DINER", "DINGY", "DIRTY", "DISCO", "DISCS", "DISKS", "DITCH", "DITTY", "DITZY", "DIVAN", "DIVED", "DIVER", "DIVOT", "DIVVY", "DIZZY", "DOCKS", "DODGE", "DODGY", "DOGGY", "DOGMA", "DOING", "DOLLS", "DOMED", "DONOR", "DONUT", "DOORS", "DORIC", "DOSED", "DOSES", "DOTTY", "DOUBT", "DOUGH", "DOUSE", "DOWNS", "DOZEN", "DRAFT", "DRAIN", "DRAMA", "DRANK", "DRAWN", "DRAWS", "DREAD", "DREAM", "DRESS", "DRIED", "DRIER", "DRIFT", "DRILL", "DRILY", "DRINK", "DRIVE", "DROLL", "DRONE", "DROPS", "DROVE", "DROWN", "DRUGS", "DRUMS", "DRUNK", "DRYER", "DUCAT", "DUCHY", "DUCKS", "DUMMY", "DUNCE", "DUNES", "DUSTY", "DUTCH", "DUVET", "DWARF", "DWELL", "DYING",
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
                "VAGUE", "VALET", "VALID", "VALOR", "VALUE", "VALVE", "VAPOR", "VAULT", "VAUNT", "VEINS", "VEINY", "VENOM", "VENUE", "VERBS", "VERGE", "VERSE", "VICAR", "VIDEO", "VIEWS", "VIGIL", "VIGOR", "VILLA", "VINES", "VINYL", "VIRAL", "VIRUS", "VISIT", "VISOR", "VITAL", "VIVID", "VIXEN", "VOCAL", "VODKA", "VOGUE", "VOICE", "VOTED", "VOTER", "VOTES", "VOUCH", "VOWED", "VOWEL", "VROOM",
                "WAGES", "WAGON", "WAIST", "WAITS", "WAIVE", "WALKS", "WALLS", "WALTZ", "WANTS", "WARDS", "WARES", "WARNS", "WASTE", "WATCH", "WATER", "WAVED", "WAVES", "WAXEN", "WEARS", "WEARY", "WEAVE", "WEBBY", "WEDGE", "WEEDS", "WEEKS", "WEIGH", "WEIRD", "WELLS", "WELSH", "WETLY", "WHALE", "WHEAT", "WHEEL", "WHERE", "WHICH", "WHILE", "WHINE", "WHISK", "WHITE", "WHOLE", "WHORL", "WHOSE", "WIDEN", "WIDER", "WIDOW", "WIDTH", "WIELD", "WILLS", "WIMPY", "WINCE", "WINCH", "WINDS", "WINDY", "WINES", "WINGS", "WIPED", "WIRED", "WIRES", "WISER", "WITCH", "WITTY", "WIVES", "WOKEN", "WOMAN", "WOMEN", "WOODS", "WORDS", "WORKS", "WORLD", "WORMS", "WORMY", "WORRY", "WORSE", "WORST", "WORTH", "WOULD", "WOUND", "WOVEN", "WRATH", "WRECK", "WRIST", "WRITE", "WRONG", "WROTE",
                "YACHT", "YARDS", "YAWNS", "YEARN", "YEARS", "YEAST", "YELLS", "YIELD", "YODEL", "YOUNG", "YOURS", "YOUTH", "YUMMY",
                "ZEBRA", "ZILCH", "ZONES"
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

    }

    void Activate()
    {
        toggleLight(true);
    }

    int li_sp_ix;
    char[] li_sp_letters;

    int li_le_ix;
    char li_le_letter;
    int li_le_flashes;

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
        string finalSolution = null;
        int attempts = 0;

        while (attempts++ < 1000 && finalSolution == null)
        {
            chosenWord = wordBank[Rnd.Range(0, wordBank.Length)];

            // Light–Speaker: two adjacent letters (order unknown)
            li_sp_ix = Rnd.Range(0, chosenWord.Length - 1);
            li_sp_letters = new[] { chosenWord[li_sp_ix], chosenWord[li_sp_ix + 1] }.Shuffle();

            // Light–Letter: letter & its position
            li_le_ix = Rnd.Range(0, chosenWord.Length);
            li_le_letter = chosenWord[li_le_ix];
            li_le_flashes = li_le_ix + 1;

            // Speaker–Light: 5 letters, exactly 2 correct positions, ALL shifted backward 5
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
                sp_li_adjustedLetters[i] = (c < 'F') ? (char)(c + 21) : (char)(c - 5);
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
            le_sp_segments = _segmentArragements[le_sp_letter - 'A'].ToArray().Shuffle();

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
            Debug.LogFormat("[Circuit Charging #{0}] Speaker-Light: OG letters = {1}, Adjusted letters = {2}, Correct positions = {3}", ModuleId,
                sp_li_ogLetters.Join(""),
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
