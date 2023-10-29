using Microsoft.Extensions.DependencyInjection;
using NLog;

Console.ForegroundColor = UserInteractions.defaultColor;

// Create instance of the Logger
NLog.Logger logger = UserInteractions.getLogger();
logger.Info("Main program is running and log mager is started, program is running on a " + (UserInteractions.IS_UNIX ? "" : "non-") + "unix-based device.\n");

// Scrub file TODO: Move into option
string scrubbedFile = FileScrubber.ScrubMovies("movies.csv", logger);
logger.Info(scrubbedFile);

MovieFile movieFile = new MovieFile(scrubbedFile, logger);

string[] MAIN_MENU_OPTIONS_IN_ORDER = { enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.View_Movies_No_Filter),
                                        enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.View_Movies_Filter),
                                        enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.Add_Movie),
                                        enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.Exit)};

string[] FILTER_MENU_OPTIONS_IN_ORDER = { enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.ID),
                                          enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.GENRE),
                                          enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.DIRECTOR),
                                        //   enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.TIMESPAN),
                                          enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.Run_Filter)};

// MAIN LOOP MENU
do
{
    // TODO: Move to switch with integer of place value and also make not relient on index by switching to enum for efficiency
    string menuCheckCommand = UserInteractions.OptionsSelector(MAIN_MENU_OPTIONS_IN_ORDER);

    logger.Info($"User choice: \"{menuCheckCommand}\"");

    if (menuCheckCommand == enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.Exit))
    {//If user intends to exit the program
        logger.Info("Program quiting...");
        return;
    }
    else if (menuCheckCommand == enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.View_Movies_No_Filter))
    {
        UserInteractions.PrintMediaList<Movie>(movieFile.Movies);
    }
    else if (menuCheckCommand == enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.View_Movies_Filter))
    {
        List<Media.GENRES> filterRequiredGenres = new List<Media.GENRES>(){ };// Default - have no genres //((Media.GENRES[])Enum.GetValues(typeof(Media.GENRES))).ToList(); //Default get all enums genre type
        List<string> filterRequiredDirectorParts = new List<string>(){ };// Default - have no director parts

        List<string> filterSearchStrings = new List<string>() { };

        string choosenFilterOption;
        do
        {
            // Display existing settings
            Console.WriteLine("\n~<:{[ Current Filter Settings ]}:>~\n");
            //TODO: Move created strings to only be updated when changed, not every cycle
            string filterAllowGenresAsStr = "";
            string filterAllowDirectorsAsStr = "";

            foreach(Media.GENRES requiredGenre in filterRequiredGenres)
            {
                filterAllowGenresAsStr = $"{filterAllowGenresAsStr}, {Media.GenresEnumToString(requiredGenre)}";
            }
            if(filterAllowGenresAsStr.Length > 2){ filterAllowGenresAsStr = filterAllowGenresAsStr.Substring(2); }

            filterAllowDirectorsAsStr = (filterRequiredDirectorParts.Count < 2)? "" : filterRequiredDirectorParts.Aggregate((current,next) => $"{current}, \"{next}\"");


            string indentStr = new string[3].Aggregate((c, n) => $"{c} "); //Blank space to make it act as a single character, needs to be 1 more then desired ammount
            string phraseIndentStr = new string[12].Aggregate((c, n) => $"{c} "); //Blank space to make it act as a single character

            string builtUpPhrases = (filterSearchStrings.Count == 0)? "" : filterSearchStrings.Aggregate((current, next) => $"{current}\n{indentStr}{phraseIndentStr}{next}");

            Console.WriteLine($"{indentStr}Genres:    {filterAllowGenresAsStr}");
            Console.WriteLine($"{indentStr}Phrases:   {builtUpPhrases}");
            Console.WriteLine($"{indentStr}Directors: {filterAllowDirectorsAsStr}");



            // User chooses option
            choosenFilterOption = UserInteractions.OptionsSelector(FILTER_MENU_OPTIONS_IN_ORDER);

            if(choosenFilterOption == enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.GENRE))
            {
                filterRequiredGenres = UserInteractions.RepeatingGenreOptionsSelector(true,true);
            }
            else if(choosenFilterOption == enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.DIRECTOR))
            {
                string[] directorOptions = new string[] { "Add director keywords", "Remove director keywords", "Exit director" };

                string selectedOption;
                do{
                    selectedOption = UserInteractions.OptionsSelector(directorOptions);

                    if(selectedOption == directorOptions[1]){
                        Console.WriteLine("Select the following to remove");
                        string[] leftAfterRemovel = UserInteractions.RepeatingOptionsSelector(filterSearchStrings.ToArray());
                        
                        // filterRequiredDirectorParts = new List<string>() { }; //Reset options
                        foreach(string phrase in leftAfterRemovel)
                        {
                            filterRequiredDirectorParts.Add(phrase);
                        }
                    }else if(selectedOption == directorOptions[0]){
                        Console.WriteLine("Add phrases to the filter (it's case-insensitive)");
                        string newPhrase = null;
                        do{
                            newPhrase = UserInteractions.UserCreatedStringObtainer("Please input a phrase to add to the search, or leave blank to exit",-1,false,true);
                            if(newPhrase.Length == 0){
                                newPhrase = null;
                            }else{
                                filterRequiredDirectorParts.Add(newPhrase);
                            }
                        }while(newPhrase != null);
                    }
                }while(selectedOption != directorOptions[directorOptions.Length-1]);

                Console.WriteLine($"Current search phrases: {((filterRequiredDirectorParts.Count==0)? "" : filterRequiredDirectorParts.Aggregate((current,next) => $"{current}, {next}"))}");

            }

        }while(choosenFilterOption != enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS.Run_Filter));

        // Filter results
        // By genre
        List<Movie> filteredMovies = new List<Movie>();
        bool firstFilter = true;
        foreach(Media.GENRES genre in filterRequiredGenres)
        {
            if(firstFilter){
                firstFilter = false;
                filteredMovies = movieFile.Movies.Where(m => m.genres.Contains(genre)).ToList(); //Set first results
            }else{
                filteredMovies = filteredMovies.Where(m => m.genres.Contains(genre)).ToList(); //Tighten results
            }
        }
        // By director
        foreach(string directorStr in filterRequiredDirectorParts)
        {
            filteredMovies = filteredMovies.Where(m => m.director.Contains(directorStr)).ToList(); //Tighten results
        }

        Console.ForegroundColor = UserInteractions.resultsColor;
        UserInteractions.PrintMediaList(filteredMovies);
        Console.ForegroundColor = UserInteractions.defaultColor;
    }
    else if (menuCheckCommand == enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS.Add_Movie))
    {
        Movie newMovie = userCreateNewMovie();//TODO: Check movie title during creation
        if (movieFile.isUniqueTitle(newMovie.title))
        {
            if(movieFile.AddMovie(newMovie)){
                //Inform user that movie was created and added    
                Console.WriteLine($"Your movie with the title of \"{newMovie.title}\" was successfully added to the records.");
            }else{
                logger.Warn($"Your movie was unable to be saved.");
            }
        }
        else
        {
            logger.Warn($"Duplicate movie record on movie \"{newMovie.title}\" with id \"{newMovie.mediaId}\". Not adding to records.");
        }
    }
    else
    {
        logger.Fatal("Somehow, menuCheckCommand was slected that did not fall under the the existing commands, this should never have been triggered. Improper menuCheckCommand is getting through");
    }

} while (true);


Movie userCreateNewMovie(){
    string movieTitle = UserInteractions.UserCreatedStringObtainer("Please enter the title of the new movie", 1, false, false);

    return new Movie
    {
        mediaId = 123,
        title = movieTitle,
        director = UserInteractions.UserCreatedStringObtainer("Please enter the director's name", 1, false, false),
        // timespan (hours, minutes, seconds)
        runningTime = new TimeSpan(
            UserInteractions.UserCreatedIntObtainer("Please enter the movie's runtime (hours)", 0, 24/*int.MaxValue*/, false, 0),
            UserInteractions.UserCreatedIntObtainer("Please enter the movie's runtime (minutes)", 0, 59, false, 0),
            UserInteractions.UserCreatedIntObtainer("Please enter the movie's runtime (seconds)", 0, 59, false, 0)
            ),
        genres = UserInteractions.RepeatingGenreOptionsSelector(false, false)
    };
}




// Console.WriteLine("--"+UserInteractions.userCreatedIntObtainer("Please enter an Id ", -50, 100, true, 42));

// Movie movie = new Movie
// {
//     mediaId = 123,
//     title = "Greatest Movie Ever, The (2023)",
//     director = "Jeff Grissom",
//     // timespan (hours, minutes, seconds)
//     runningTime = new TimeSpan(2, 21, 23),
//     genres = { "Comedy", "Romance" }
// };

// Album album = new Album
// {
//     mediaId = 321,
//     title = "Greatest Album Ever, The (2020)",
//     artist = "Jeff's Awesome Band",
//     recordLabel = "Universal Music Group",
//     genres = { "Rock" }
// };

// Book book = new Book
// {
//     mediaId = 111,
//     title = "Super Cool Book",
//     author = "Jeff Grissom",
//     pageCount = 101,
//     publisher = "",
//     genres = { "Suspense", "Mystery" }
// };

// Console.WriteLine(movie.Display());
// Console.WriteLine(album.Display());
// Console.WriteLine(book.Display());

logger.Info("Program ended");


// vvv UNUM STUFF vvv

string enumToStringMainMenuWorkarround(MAIN_MENU_OPTIONS mainMenuEnum)
{
    return mainMenuEnum switch
    {
        MAIN_MENU_OPTIONS.Exit => "Quit program",
        MAIN_MENU_OPTIONS.View_Movies_No_Filter => $"View movies on file in order (display max ammount is {UserInteractions.PRINTOUT_RESULTS_MAX_TERMINAL_SPACE_HEIGHT / 5:N0})",// Divide by 11 as 10 is the current max number of fields in a ticket and +1 for the empty spacing lines between
        MAIN_MENU_OPTIONS.View_Movies_Filter => $"Filter movies on file",
        MAIN_MENU_OPTIONS.Add_Movie => "Add new movie to file",
        _ => "ERROR_MAIN_MENU_OPTION_DOES_NOT_EXIST"
    };
}

string enumToStringFilterMenuWorkarround(FILTER_MENU_OPTIONS filterMenuEnum)
{
    return filterMenuEnum switch
    {
        FILTER_MENU_OPTIONS.ID => "Add filter tickets by id",
        FILTER_MENU_OPTIONS.GENRE => "Modify filter tickets by genre",
        FILTER_MENU_OPTIONS.DIRECTOR => "Select filter tickets by director",
        // FILTER_MENU_OPTIONS.TIMESPAN => "Select filter tickets by running time",
        FILTER_MENU_OPTIONS.Run_Filter => "Run the compleated filters",
        _ => "ERROR_FILTER_MENU_OPTION_DOES_NOT_EXIST"
    };

}

// FILTER_MENU_OPTIONS stringToEnumTicketTypeWorkArround(string ticketTypeStr)
// {
//     return ticketTypeStr switch
//     {
//         "Bug/Defect" => TICKET_TYPES.Bug_Defect,
//         "Enhancement" => TICKET_TYPES.Enhancment,
//         "Task" => TICKET_TYPES.Task,
//         _ => TICKET_TYPES.Bug_Defect //Default to orignal Bug/Defect when not found (should never happen if done correctly)
//     };
//     //TODO: Log error
// }



public enum MAIN_MENU_OPTIONS
{
    Exit,
    View_Movies_No_Filter,
    View_Movies_Filter,
    Add_Movie
}

public enum FILTER_MENU_OPTIONS
{
    ID,
    GENRE,
    DIRECTOR,
    TIMESPAN,
    Run_Filter
}