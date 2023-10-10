using NLog;
/**
    UserInteractions is a class to assist in getting input from the user (and sometimes displaying infromation) in other to standardize common requests
*/
public sealed class UserInteractions{ //Sealed to prevent inheritance, set up as a singleton
    public const bool IS_UNIX = true;

    static string loggerPath = Directory.GetCurrentDirectory() + (IS_UNIX ? "/" : "\\") + "nlog.config";
    static string readWriteFilePath = Directory.GetCurrentDirectory() + (IS_UNIX ? "/" : "\\") + "Tickets.csv";
    static NLog.Logger logger;


// SINGLETON PATTERN START (Hope I got this right)
    private static readonly UserInteractions instance = new UserInteractions();
    static UserInteractions(){ 
        logger = LogManager.Setup().LoadConfigurationFromFile(loggerPath).GetCurrentClassLogger();
    }
    private UserInteractions(){
        logger = LogManager.Setup().LoadConfigurationFromFile(loggerPath).GetCurrentClassLogger();
    }
    public static UserInteractions Instance{ get{ return instance; } }
// SINGLETON PATTERN END


    public static NLog.Logger getLogger(){
        return logger;
    }

    static string optionsSelector(string[] options)
    {
        string userInput;
        int selectedNumber;
        bool userInputWasImproper = true;
        List<int> cleanedListIndexs = new List<int> { };
        string optionsTextAsStr = ""; //So only created once. Requires change if adjustable width is added

        for (int i = 0; i < options.Length; i++)
        {
            // options[i] = options[i].Trim();//Don't trim so when used, spaces can be used to do spaceing
            if (options[i] != null && options[i].Replace(" ", "").Length > 0)
            {//Ensure that not empty or null
                cleanedListIndexs.Add(i);//Add index to list
                optionsTextAsStr = $"{optionsTextAsStr}\n{string.Format($" {{0,{options.Length.ToString().Length}}}) {{1}}", cleanedListIndexs.Count(), options[i])}";//Have to use this as it prevents the constents requirment.
            }
        }
        optionsTextAsStr = optionsTextAsStr.Substring(1); //Remove first \n

        // Seprate from rest by adding a blank line
        Console.WriteLine();
        do
        {
            Console.WriteLine("Please select an option from the following...");
            Console.WriteLine(optionsTextAsStr);
            Console.Write("Please enter an option from the list: ");
            userInput = Console.ReadLine().Trim();

            //TODO: Move to switch without breaks instead of ifs or if-elses?
            if (!int.TryParse(userInput, out selectedNumber))
            {// User response was not a integer
                logger.Error("Your selector choice was not a integer, please try again.");
            }
            else if (selectedNumber < 1 || selectedNumber > cleanedListIndexs.Count()) //Is count because text input index starts at 1
            {// User response was out of bounds
                logger.Error($"Your selector choice was not within bounds, please try again. (Range is 1-{cleanedListIndexs.Count()})");
            }
            else
            {
                userInputWasImproper = false;
            }
        } while (userInputWasImproper);
        // Seprate from rest by adding a blank line
        Console.WriteLine();
        return options[cleanedListIndexs[selectedNumber - 1]];
    }

    static string userCreatedStringObtainer(string message, int minimunCharactersAllowed, bool showMinimum, bool keepRaw)
    {
        if (minimunCharactersAllowed < 0)
        {
            minimunCharactersAllowed = 0;
        }
        string userInput = null;

        do
        {
            Console.Write($"\n{message}{(showMinimum ? $" (must contain at least {minimunCharactersAllowed} character{(minimunCharactersAllowed==1?"":"s")})" : "")}: ");
            userInput = Console.ReadLine().ToString();
            if (!keepRaw)
            {
                userInput = userInput.Trim();
            }
            if (minimunCharactersAllowed > 0 && userInput.Length == 0)
            {
                userInput = null;
                logger.Warn($"Entered input was blank, input not allowed to be empty, please try again.");
            }
            else if (userInput.Length < minimunCharactersAllowed)
            {
                userInput = null;
                logger.Warn($"Entered input was too short, it must be at least {minimunCharactersAllowed} characters long, please try again.");
            }
        } while (userInput == null);

        return userInput;
    }

    public static int userCreatedIntObtainer(string message, int minValue, int maxValue, bool showRange){
        return userCreatedIntObtainer(message, minValue, maxValue, showRange, "");
    }
    public static int userCreatedIntObtainer(string message, int minValue, int maxValue, bool showRange, int defaultValue){
        return userCreatedIntObtainer(message, minValue, maxValue, showRange, defaultValue.ToString());
    }
    private static int userCreatedIntObtainer(string message, int minValue, int maxValue, bool showRange, string defaultValue){//=""){
        string userInputRaw = null;
        int userChoosenInteger;
        int defaultAsInt = 0;

        if(defaultValue != "" && !int.TryParse(defaultValue, out defaultAsInt)){
            logger.Error($"Could not use defaultValue of \"{defaultValue}\" as an int. Argument exception error!");
            defaultValue = "";
        }
        do{
            Console.Write($"\n{message}{(showRange? $" ({minValue} to {maxValue})" : "")}{(defaultValue==""? "" : $" or leave blank to use \"{defaultValue}\"")}: ");
            userInputRaw = Console.ReadLine().Trim();
            if (int.TryParse(userInputRaw, out userChoosenInteger) || userInputRaw.Length == 0) //Duplicate .Length == 0 checking to have code in the same location
            {
                if(defaultValue != null && userInputRaw.Length == 0) //Was blank and allowed
                {
                    userChoosenInteger = defaultAsInt;
                                        Console.WriteLine("A");
                }
                else if(defaultValue == null && userInputRaw.Length == 0)
                {
                    logger.Error("Your choosen integer was empty, please try again.");
                    userInputRaw = null; //Was blank and not allowed
                }
                else if(userChoosenInteger < minValue)
                {
                    logger.Error($"Your choosen integer choice was below \"{minValue}\", the range is ({minValue} to {maxValue}), please try again.");
                    userInputRaw = null; //Under min
                }
                else if(userChoosenInteger > maxValue)
                {
                    logger.Error($"Your choosen integer choice was above \"{maxValue}\", the range is ({minValue} to {maxValue}), please try again.");
                    userInputRaw = null; //Above max
                }else{
                    Console.WriteLine("+");
                }
                    Console.WriteLine("D");
            }
            else
            {
                //User response was not a integer
                logger.Error("Your choosen id choice was not a possible integer, please try again.");
                userInputRaw = null; //Was not an integer
            }
        }while(userInputRaw == null);
        return userChoosenInteger;
    }
}