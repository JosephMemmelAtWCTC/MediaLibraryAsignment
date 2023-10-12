public abstract class Media
{
    public const string DELIMETER_1 = ",";
    public const string DELIMETER_2 = "|";
    public const string START_END_SUMMARY_WITH_DELIMETER1_INDICATOR = "\"";

    // public properties
    public UInt64 mediaId { get; set; }
    public string title { get; set; }
    public List<GENRES> genres { get; set; }

    // constructor
    public Media()
    {
        genres = new List<GENRES>();
    }

    // public method
    public virtual string Display()
    {
        return $"Id: {mediaId}\nTitle: {title}\nGenres: {string.Join(", ", genres)}\n";
    }

    public static GENRES[] SortGenres(List<GENRES> toSortGenres){
        List<string> toSortAsStrings = new List<string>(){};
        GENRES[] convertedBackToENUM = new GENRES[toSortGenres.Count()];
        foreach(GENRES genre in toSortGenres){
            toSortAsStrings.Add(GenresEnumToString(genre));
        }
        toSortAsStrings.Sort();
        for(int i=0; i<convertedBackToENUM.Length; i++){
            convertedBackToENUM[i] = GetGenreEnumFromString(toSortAsStrings[i]);
        }
        return convertedBackToENUM;
    }

    public static GENRES GetGenreEnumFromString(string genreStr)
    {
        switch (genreStr)
        {
            case "Action": return GENRES.ACTION;
            case "Adventure": return GENRES.ADVENTURE;
            case "Animation": return GENRES.ANIMATION;
            case "Children's": return GENRES.CHILDRENS;
            case "Comedy": return GENRES.COMEDY;
            case "Crime": return GENRES.CRIME;
            case "Documentary": return GENRES.DOCUMENTARY;
            case "Drama": return GENRES.DRAMA;
            case "Fantasy": return GENRES.FANTASY;
            case "Film-Noir": return GENRES.FILM_NOIR;
            case "Horror": return GENRES.HORROR;
            case "Musical": return GENRES.MUSICAL;
            case "Mystery": return GENRES.MYSTERY;
            case "Romance": return GENRES.ROMANCE;
            case "Sci-Fi": return GENRES.SCI_FI;
            case "Thriller": return GENRES.THRILLER;
            case "War": return GENRES.WAR;
            case "Western": return GENRES.WESTERN;
            case "(no genres listed)": return GENRES.NO_GENRES_LISTED;
            default: return GENRES.ERROR_NOT_A_VALID_GENRE;
        }
    }
    public static string GenresEnumToString(GENRES genre)
    {
        switch (genre)
        {
            case GENRES.ACTION: return "Action";
            case GENRES.ADVENTURE: return "Adventure";
            case GENRES.ANIMATION: return "Animation";
            case GENRES.CHILDRENS: return "Children's";
            case GENRES.COMEDY: return "Comedy";
            case GENRES.CRIME: return "Crime";
            case GENRES.DOCUMENTARY: return "Documentary";
            case GENRES.DRAMA: return "Drama";
            case GENRES.FANTASY: return "Fantasy";
            case GENRES.FILM_NOIR: return "Film-Noir";
            case GENRES.HORROR: return "Horror";
            case GENRES.MUSICAL: return "Musical";
            case GENRES.MYSTERY: return "Mystery";
            case GENRES.ROMANCE: return "Romance";
            case GENRES.SCI_FI: return "Sci-Fi";
            case GENRES.THRILLER: return "Thriller";
            case GENRES.WAR: return "War";
            case GENRES.WESTERN: return "Western";
            case GENRES.NO_GENRES_LISTED: return "(no genres listed)";
            default: return "ERROR: NOT A VALID GENRE";
        }
    }

    public enum GENRES
    {
        ACTION,
        ADVENTURE,
        ANIMATION,
        CHILDRENS,
        COMEDY,
        CRIME,
        DOCUMENTARY,
        DRAMA,
        FANTASY,
        FILM_NOIR,
        HORROR,
        MUSICAL,
        MYSTERY,
        ROMANCE,
        SCI_FI,
        THRILLER,
        WAR,
        WESTERN,
        NO_GENRES_LISTED,
        ERROR_NOT_A_VALID_GENRE
    }
}

public class Movie : Media
{
    public string director { get; set; }
    public TimeSpan runningTime { get; set; }
    public override string Display()
    {
        return $"Id: {mediaId}\nTitle: {title}\nDirector: {director}\nRun time: {runningTime}\nGenres: {string.Join(", ", genres)}\n";
    }
}

public class Album : Media
{
    public string artist { get; set; }
    public string recordLabel { get; set; }

    public override string Display()
    {
        return $"Id: {mediaId}\nTitle: {title}\nArtist: {artist}\nLabel: {recordLabel}\nGenres: {string.Join(", ", genres)}\n";
    }
}

public class Book : Media
{
    public string author { get; set; }
    public UInt16 pageCount { get; set; }
    public string publisher { get; set; }

    public override string Display()
    {
        return $"Id: {mediaId}\nTitle: {title}\nAuthor: {author}\nPages: {pageCount}\nPublisher: {publisher}\nGenres: {string.Join(", ", genres)}\n";
    }
}
