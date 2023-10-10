using NLog;

const bool IS_UNIX = true;

string loggerPath = Directory.GetCurrentDirectory() + (IS_UNIX ? "/" : "\\") + "nlog.config";
string readWriteFilePath = Directory.GetCurrentDirectory() + (IS_UNIX ? "/" : "\\") + "Tickets.csv";


// Create instance of the Logger
NLog.Logger logger = LogManager.Setup().LoadConfigurationFromFile(loggerPath).GetCurrentClassLogger();
logger.Info("Main program is running and log mager is started, program is running on a " + (IS_UNIX ? "" : "non-") + "unix-based device.\n");


Movie movie = new Movie
{
    mediaId = 123,
    title = "Greatest Movie Ever, The (2023)",
    director = "Jeff Grissom",
    // timespan (hours, minutes, seconds)
    runningTime = new TimeSpan(2, 21, 23),
    genres = { "Comedy", "Romance" }
};

Console.WriteLine(movie.Display());

logger.Info("Program ended");