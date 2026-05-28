
using Quintessential;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using Quintessential.Serialization;

namespace ExtransmutationsJournal;

using PartType = class_139;
using PartTypes = class_191;
using Permissions = enum_149;
using AtomTypes = class_175;
using Font = class_1;
using Texture = class_256;
using Song = class_186;
using VanillaAtoms = Brimstone.API.VanillaAtoms;
using BF = System.Reflection.BindingFlags;

//dotnet build;rm ExtransmutationsJournal.dll;cp bin/Debug/net4.5.2/ExtransmutationsJournal.dll ./
public class ExtransmutationsJournalMod : QuintessentialMod {

  public override void Load() {

    //string modPathDirectory = QuintessentialLoader.Mods.First(static m => m.Name == "ExtransmutationsJournal").PathDirectory;
    //var modJournals = QuintessentialLoader.ModJournalModels;
    //var journalModel = new JournalModel {
    //  Title = "Compendium of affordable alchemy",
    //  Path = Path.Combine(modPathDirectory, "Puzzles"),
    //  Chapters = new List<JournalChapterModel> {
    //    new() {
    //      Title = "Volume V, Issue I: Neglected Transmutations (Part I)",
    //      Description =
    //       "Often neglected ever since the advent of Duplication, "
    //      +"cardinal transmutations are nonetheless useful in the modern age.\n"
    //      +"Easily incorporated into most alchemical engines with minimal "
    //      +"modification and providing lower-cost processes, convenient transformations, "
    //      +"and resilience in environments Duplication may struggle in, "
    //      +"these transmutations provide new possibilities to any alchemical engineer "
    //      +"still willing to make use of them.\n"
    //      +"In this issue you may find a few such processes meant to help "
    //      +"reignite even a skeptical alchemist's interest in these forgotten transmutations."
    //      ,
    //      Puzzles = new List<string>() {
    //        "j1-alcohol-via-inversion-two",
    //        "j1-desiccant-gel",
    //        "j1-abrasive-gel",
    //        "j1-harmonious-brass",
    //        "j1-water",
    //      }
    //    },
    //  },
    //};
    //
    //modJournals.Add(journalModel); 
  }
  internal Hook hook_JournalScreen_method_1040;
  public override void PostLoad() {
    WeirdPuzzle.EnsureSongListExists();
    hook_JournalScreen_method_1040 = new Hook(typeof(JournalScreen).GetMethod("method_1040", BF.NonPublic | BF.Instance), OnJournalScreen_Method_1040);
  }

  public override void Unload() {
    hook_JournalScreen_method_1040 = null;
  }

  internal record struct WeirdPuzzle {
    public string ID = "";
    public Dictionary<int, string> JournalPreview = new();
    public static Dictionary<string, Tuple<Song, Sound>> SongBank;
    public string song = "Solving1";
    public bool previewsInput = false;

    public WeirdPuzzle() { }

    public static void EnsureSongListExists() {
      var song = class_238.field_1992;
      var fanfare = class_238.field_1991;
      SongBank ??= new() {
        {"Map",         Tuple.Create(song.field_968, fanfare.field_1832) },
        {"Solitaire",   Tuple.Create(song.field_969, fanfare.field_1832) },
        {"Solving1",    Tuple.Create(song.field_970, fanfare.field_1830) },
        {"Solving2",    Tuple.Create(song.field_971, fanfare.field_1831) },
        {"Solving3",    Tuple.Create(song.field_972, fanfare.field_1832) },
        {"Solving4",    Tuple.Create(song.field_973, fanfare.field_1833) },
        {"Solving5",    Tuple.Create(song.field_974, fanfare.field_1834) },
        {"Solving6",    Tuple.Create(song.field_975, fanfare.field_1835) },
        {"Story1",      Tuple.Create(song.field_976, fanfare.field_1832) },
        {"Story2",      Tuple.Create(song.field_977, fanfare.field_1832) },
        {"Title",       Tuple.Create(song.field_978, fanfare.field_1832) },
      };
    }

    public readonly Dictionary<int, Vector2> GetJournalPreview() {
      Dictionary<int, Vector2> ret = new();
      if (this.JournalPreview != null) {
        foreach (var kvp in this.JournalPreview) {
          ret.Add(kvp.Key, Vector2FromString(kvp.Value));
        }
      }
      return ret;
    }
    public readonly Song FetchSong() => SongBank[this.song].Item1;
    public readonly Sound FetchSound() => SongBank[this.song].Item2;
  }
  private static readonly WeirdPuzzle[] weirdPuzzles = new WeirdPuzzle[] {
    new() {
      ID = "c698519006942833",
      JournalPreview = new(){ {0,  "65, 5"} },
      previewsInput = true,
    },
    new() {
      ID = "c944705183945596",
      JournalPreview = new(){ {1,  "65, 5"} },
      previewsInput = true,
    }
  };

  internal delegate void origJournal(JournalScreen screen_self, Puzzle puzzle, Vector2 basePosition, bool isLargePuzzle);
  internal static void OnJournalScreen_Method_1040(origJournal orig, JournalScreen screen_self, Puzzle puzzle, Vector2 basePosition, bool isLargePuzzle) {
    var puzzleID = puzzle.field_2766;
    if (weirdPuzzles.Where(x => x.ID == puzzleID).Count() <= 0) {
      orig(screen_self, puzzle, basePosition, isLargePuzzle);
      return;
    }
    var puzzleModel = weirdPuzzles.Where(x => x.ID == puzzleID).First();

    bool puzzleSolved = GameLogic.field_2434.field_2451.method_573(puzzle);
    Font crimson_15 = class_238.field_1990.field_2144;
    bool authorExists = puzzle.field_2768.method_1085();
    string authorName() => puzzle.field_2768.method_1087();
    string displayString = authorExists ? string.Format("{0} ({1})", puzzle.field_2767, authorName()) : (string)puzzle.field_2767;

    Texture moleculeBackdrop = isLargePuzzle ? class_238.field_1989.field_88.field_894 : class_238.field_1989.field_88.field_895;
    Texture divider = isLargePuzzle ? class_238.field_1989.field_88.field_892 : class_238.field_1989.field_88.field_893;
    Texture solvedCheckbox = puzzleSolved ? class_238.field_1989.field_96.field_879 : class_238.field_1989.field_96.field_882;
    class_135.method_290(displayString, basePosition + new Vector2(9f, -19f), crimson_15, class_181.field_1718, (enum_0)0, 1f, 0.6f, float.MaxValue, float.MaxValue, 0, new Color(), null, int.MaxValue, false, true);
    Vector2 vector2_1 = basePosition + new Vector2(moleculeBackdrop.field_2056.X - 27, -23f);
    class_135.method_272(solvedCheckbox, vector2_1);
    class_135.method_272(divider, basePosition + new Vector2(isLargePuzzle ? 7f : 7f, -34f));
    class_135.method_272(moleculeBackdrop, basePosition);

    Bounds2 bounds2 = Bounds2.WithSize(basePosition, moleculeBackdrop.field_2056.ToVector2());
    bool mouseHover = bounds2.Contains(Input.MousePos());

    Vector2 moleculeOffset = isLargePuzzle ? new Vector2(470f, 365f) : new Vector2(280f, 200f);
    Texture textureFromMolecule(Molecule molecule, Vector2 offset) => Editor.method_928(molecule, false, mouseHover, offset, isLargePuzzle, (Maybe<float>)struct_18.field_1431).method_1351().field_937;
    Texture textureFromIndex(int i, Vector2 offset) => textureFromMolecule(puzzle.field_2771[i].field_2813, offset);
    Texture textureFromIndexInput(int i, Vector2 offset) => textureFromMolecule(puzzle.field_2770[i].field_2813, offset);

    if (puzzleModel.GetJournalPreview().Count() > 0) {
      foreach (var kvp in puzzleModel.GetJournalPreview()) {
        if (!puzzleModel.previewsInput) {
          class_135.method_272(textureFromIndex(kvp.Key, moleculeOffset), bounds2.Min + kvp.Value);
        }
        else {
          class_135.method_272(textureFromIndexInput(kvp.Key, moleculeOffset), bounds2.Min + kvp.Value);
        }
      }
    }
    else {
      var molecules = puzzle.field_2771.Select(x => x.field_2813).OrderByDescending(x => x.method_1100().Count);
      Texture moleculeTexture = textureFromMolecule(molecules.First(), moleculeOffset);
      Vector2 vector2_4 = (moleculeTexture.field_2056.ToVector2() / 2).Rounded();
      class_135.method_272(moleculeTexture, bounds2.Center.Rounded() - vector2_4 + new Vector2(2f, 2f));
    }
    if (mouseHover && Input.IsLeftClickPressed()) {
      Song song = puzzleModel.FetchSong();
      Sound fanfare = puzzleModel.FetchSound();
      Maybe<class_264> maybeStoryPanel = new();//puzzleModel.NoStoryPanel ? struct_18.field_1431 : new class_264(puzzleModel.ID);

      GameLogic.field_2434.method_946(new PuzzleInfoScreen(puzzle, song, fanfare, maybeStoryPanel));
      class_238.field_1991.field_1821.method_28(1f);
    }
  }


  internal static float FloatFromString(string str, float defaulF = 0f) {
    if (!string.IsNullOrEmpty(str)) {
      return float.Parse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
    }
    else {
      return defaulF;
    }
  }

  internal static Vector2 Vector2FromString(string pos, float defaultX = 0f, float defaultY = 0f) {
    float x = FloatFromString(pos?.Split(',')[0], defaultX);
    float y = FloatFromString(pos?.Split(',')[1], defaultY);
    return new Vector2(x, y);
  }
}