using NLog;

// Create instance of the Logger
NLog.Logger logger = UserInteractions.getLogger();
logger.Info("Main program is running and log mager is started, program is running on a " + (UserInteractions.IS_UNIX ? "" : "non-") + "unix-based device.\n");

// Scrub file TODO: Move into option
string scrubbedFile = FileScrubber.ScrubMovies("movies.csv", logger);
logger.Info(scrubbedFile);

MovieFile movieFile = new MovieFile(scrubbedFile, logger);

string[] MAIN_MENU_OPTIONS_IN_ORDER = { enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.View_Movies_No_Filter),
                                        enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.Add_Movie),
                                        enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.Exit)};

// MAIN LOOP MENU
do
{
    // TODO: Move to switch with integer of place value and also make not relient on index by switching to enum for efficiency
    string menuCheckCommand = UserInteractions.OptionsSelector(MAIN_MENU_OPTIONS_IN_ORDER);

    if (menuCheckCommand == enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.Exit))
    {//If user intends to exit the program
        logger.Info("Program quiting...");
        return;
    }
    else if (menuCheckCommand == enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.View_Movies_No_Filter))
    {
        UserInteractions.PrintMediaList<Movie>(movieFile.Movies);
    }
    // else if (menuCheckCommand == enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.View_Movies_Filter))
    // {
    // }
    else if (menuCheckCommand == enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS.Add_Movie))
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
    string movieTitle = UserInteractions.UserCreatedStringObtainer("Please enter the title of the new movie", 1, true, false);

    return new Movie
    {
        mediaId = 123,
        title = movieTitle,
        director = "Jeff Grissom",
        // timespan (hours, minutes, seconds)
        runningTime = new TimeSpan(2, 21, 23),
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

string enumToStringMainMenuWorkArround(MAIN_MENU_OPTIONS mainMenuEnum)
{
    return mainMenuEnum switch
    {
        MAIN_MENU_OPTIONS.Exit => "Quit program",
        MAIN_MENU_OPTIONS.View_Movies_No_Filter => $"View movies on file in order (display max ammount is {UserInteractions.PRINTOUT_RESULTS_MAX_TERMINAL_SPACE_HEIGHT:N0})",
        MAIN_MENU_OPTIONS.View_Movies_Filter => $"Filter movies on file",
        MAIN_MENU_OPTIONS.Add_Movie => "Add new movie to file",
        _ => "ERROR"
    };
}

public enum MAIN_MENU_OPTIONS
{
    Exit,
    View_Movies_No_Filter,
    View_Movies_Filter,
    Add_Movie
}