//   ---------------------------------------------------
//   File          : cr.cs
//   Authors       : ccmywish <ccmywish@qq.com>
//   Created on    : <2021-1-12>
//   Last modified : <2022-1-12>
//
//   This file is used to explain a CRyptic command
//   or an acronym's real meaning in computer world or
//   orther fileds.
//
//  ---------------------------------------------------


// We don't use it for color anymore
// using Pastel;
// using System.Drawing;   // For `Color` class

using Carbon.Toml;
using System.Text.RegularExpressions;

// cannot be const
string CRYPTIC_RESOLVER_HOME = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.cryptic-resolver";

// or use var
Dictionary<string,string> CRYPTIC_DEFAULT_SHEETS = new Dictionary<string, string> {
	["computer"]= "https://github.com/cryptic-resolver/cryptic_computer.git",
	["common"]=   "https://github.com/cryptic-resolver/cryptic_common.git",
	["science"]=  "https://github.com/cryptic-resolver/cryptic_science.git",
	["economy"]=  "https://github.com/cryptic-resolver/cryptic_economy.git",
	["medicine"]= "https://github.com/cryptic-resolver/cryptic_medicine.git"
};

const string CRYPTIC_VERSION = "1.0.0";

//
// helper: for color
// 
// It's same as in NodeJS

string bold(string str)       { return String.Format("\x1b[1m{0}\x1b[0m",str); }
string underline(string str)  { return String.Format("\x1b[4m${0}\x1b[0m",str); }
string red(string str)        { return String.Format("\x1b[31m${0}\x1b[0m",str); }
string green(string str)      { return String.Format("\x1b[32m${0}\x1b[0m",str); }
// string yellow(string str)     { return String.Format("\x1b[33m${0}\x1b[0m",str); }
string blue(string str)       { return String.Format("\x1b[34m${0}\x1b[0m",str); }
string purple(string str)     { return String.Format("\x1b[35m${0}\x1b[0m",str); }
// string cyan(string str)       { return String.Format("\x1b[36m${0}\x1b[0m",str); }


//
// core: logic
//

bool is_there_any_sheet() {

	string path = CRYPTIC_RESOLVER_HOME;

	if (! Directory.Exists(path)) {
		Directory.CreateDirectory(path);
	}
	var dirnum = Directory.GetDirectories(path).Length;

	if (dirnum == 0)
		return false;
	else 
		return true;
}





void add_default_sheet_if_none_exist(){

    if (!is_there_any_sheet()) {
		Console.WriteLine("cr: Adding default sheets...");

		foreach(var (key, value) in CRYPTIC_DEFAULT_SHEETS) {
			Console.WriteLine("cr: Pulling cryptic_" + key + "...");
            
            string command = $"-C {CRYPTIC_RESOLVER_HOME} clone {value} -q";
            var proc = System.Diagnostics.Process.Start("git", command);    // no need to use "cmd" /C git ...." 
            // must wait
            proc.WaitForExit();
            var exitCode = proc.ExitCode;
            if (exitCode!=0){
                // Read the standard error and write it on to console.
                // Console.WriteLine("Pull failed!");
                // Git will automatically output its error
            }
		}

		Console.WriteLine("cr: Add done");
	}

}


void update_sheets(string sheet_repo){

	add_default_sheet_if_none_exist();

	if(sheet_repo == "") {
		Console.WriteLine("cr: Updating all sheets...");

		var dirs = Directory.GetDirectories(CRYPTIC_RESOLVER_HOME);

		foreach(var dir in dirs) {
            // path to dirname
			string sheet = Path.GetFileName(dir);   // This api name is shit
			Console.WriteLine("cr: Wait to update {0}...", sheet);
            string command = $"-C  {CRYPTIC_RESOLVER_HOME}/{sheet} pull -q";
			var proc = System.Diagnostics.Process.Start("git",command);
			proc.WaitForExit();
		}
		Console.WriteLine("cr: Update done");
	
	} else {
		Console.WriteLine("cr: Adding new sheet...");
        string command = $"-C ${CRYPTIC_RESOLVER_HOME} clone {sheet_repo} -q";
		var proc = System.Diagnostics.Process.Start("git", command);
		proc.WaitForExit();
		Console.WriteLine("cr: Add new sheet done");
	}


}


// path: sheet name, eg. cryptic_computer
// file: dict(file) name, eg. a,b,c,d
// dict: the concrete dict
// 		 var dict map[string]interface{}
// 
// We can't use pointer of an managed type
Carbon.Json.JsonObject load_dictionary(string path, string file) {

	string toml_file = $"{CRYPTIC_RESOLVER_HOME}/{path}/{file}.toml";

	if (! File.Exists(toml_file)) {
		var emptyDoc = new Carbon.Json.JsonObject{}; // return null will invoke a warning when built
        return emptyDoc;
	} else {
        string str = File.ReadAllText(toml_file);
		var doc = Toml.Parse(str);
		return doc;
	}

}




// Pretty print the info of the given word
void pp_info(Carbon.Json.JsonNode infonode){

    var info = infonode.As<Carbon.Json.JsonObject>();
	// We should convert disp, desc, full into string

	// can't directly cast TOMLValue to string
	string disp;
	if (info.ContainsKey("disp")){
		disp = (string)info["disp"];
	}else {
        disp = red("No name!");
	}

	Console.WriteLine("\n  {0}: {1}", disp, (string)info["desc"]);

    if (info.ContainsKey("full")){
		Console.WriteLine("\n  {0}", (string)info["full"]);
	}

	// see is string[]
    if (info.ContainsKey("see")){
		string[] see = info["see"].ToArrayOf<string>(); // Notice here

		Console.Write("\n{0} ", purple("SEE ALSO "));

		foreach(var val in see) {
			Console.Write(underline(val) );
		}

		Console.WriteLine();
	}
	Console.WriteLine();
}


// Print default cryptic_ sheets
void pp_sheet(string sheet) {
	Console.WriteLine(green("From: " + sheet));
}




//  Used for synonym jump
//  Because we absolutely jump to a must-have word
//  So we can directly lookup to it
//
//  Notice that, we must jump to a specific word definition
//  So in the toml file, you must specify the precise word.
//  If it has multiple meanings, for example
//
//    [blah]
//    same = "XDG"  # this is wrong
//
//    [blah]
//    same = "XDG.Download" # this is right
bool directly_lookup(string sheet, string file, string word) {

	Carbon.Json.JsonObject dict;

	dict = load_dictionary(sheet, file.ToLower()); // std.string: toLower

	if(dict == new Carbon.Json.JsonObject(){}) {
		Console.WriteLine("WARN: Synonym jumps to a wrong place");
		Environment.Exit(0);
	}

	string[] words = word.Split("."); // [XDG Download]
	string dictword = words[0].ToLower();       // XDG [Download]
    
    Carbon.Json.JsonNode info;

	if (words.Length == 1) { // [HEHE]
		info = dict[dictword];

	} else { //  [XDG Download]
		string explain = words[1];
		Carbon.Json.JsonNode indirect_info = dict[dictword];
		info = indirect_info[explain];
	}

	// Warn user this is the toml maintainer's fault
	// the info map is empty
	if (info == null) {
		string str = @"WARN: Synonym jumps to a wrong place at `%s` \n 
	Please consider fixing this in `%s.toml` of the sheet `%s`";

		string redstr = red(String.Format(str, word, file.ToLower(), sheet));

		Console.WriteLine(redstr);
		Environment.Exit(0); // not portable??
	}

	pp_info(info);
	return true; // always true
}




//  Lookup the given word in a dictionary (a toml file in a sheet) and also print.
//  The core idea is that:
//
//  1. if the word is `same` with another synonym, it will directly jump to
//    a word in this sheet, but maybe a different dictionary
//
//  2. load the toml file and check whether it has the only one meaning.
//    2.1 If yes, then just print it using `pp_info`
//    2.2 If not, then collect all the meanings of the word, and use `pp_info`
//
bool lookup(string sheet, string file, string word) {

	// Only one meaning

	Carbon.Json.JsonObject dict;

	dict = load_dictionary(sheet, file);

    if(dict == new Carbon.Json.JsonObject(){}) {
        return false;
	}

	//  We firstly want keys in toml be case-insenstive, but later in 2021/10/26 I found it caused problems.
	// So I decide to add a new must-have format member: `disp`
	// This will display the word in its traditional form.
	// Then, all the keywords can be downcase.

	Carbon.Json.JsonNode infonode;

	// check whether the key is in it
    if (dict.ContainsKey(word)){
        infonode = dict[word]; // Directly hash it
	} else {
		return false;
	}

	// Warn user if the info is empty. For example:
	//   emacs = { }
    var info = infonode.As<Carbon.Json.JsonObject>();
	if (info == new Carbon.Json.JsonObject(){}) {
		string str = String.Format(@"WARN: Lack of everything of the given word. \n
	Please consider fixing this in the sheet `%s`", sheet);
		Console.WriteLine(red(str));
		Environment.Exit(0);
	}

	// Check whether it's a synonym for anther word
	// If yes, we should lookup into this sheet again, but maybe with a different file
	
	// Console.WriteLine(info.table); //DEBUG

	string same;
	if(info.ContainsKey("same")){
		same = (string)info["same"];
		pp_sheet(sheet);
		// point out to user, this is a jump
		Console.WriteLine(blue(bold(word)) + " redirects to " + blue(bold(same)));

		// Explicitly convert it to downcase.
		// In case the dictionary maintainer redirects to an uppercase word by mistake.
		same = same.ToLower();
		
		// no need to load dictionary again
		if (word.ToLower()[0] == same[0]) {	// same is just "a" "b" "c" "d" , etc ...
			
			Carbon.Json.JsonNode same_info = dict[same];
			
			if (same_info == null) { // Need repair
				string str = "WARN: Synonym jumps to the wrong place at `" + same + "`\n" +
					"	Please consider fixing this in " + same[0] +
					".toml of the sheet `" + sheet + "`";

				Console.WriteLine(red(str));
				return false;
			} else {
				pp_info(same_info);
				return true;
			}
		} else {
			return directly_lookup(sheet, same[0].ToString(), same);
		}
	}

	// Check if it's only one meaning

    if (info.ContainsKey("desc")) {
		pp_sheet(sheet);
		pp_info(info);
		return true;
	}

	// Multiple meanings in one sheet

	// string[] info_names; 
    // arrays are statically allocated in C#, so the way above can't be done;(But you can do it in D)
    //

    List<string> info_names = new List<string>();   // must be intialized in C#
	foreach(var v in info.ToArrayOf<string>()) {// yes, info is TOMLValue and can transformed to a table(aa)
		info_names.Add(v);
	}

	if (info_names.Count != 0) {
		pp_sheet(sheet);

		foreach(var meaning in info_names) {
			Carbon.Json.JsonNode multi_ref = dict[word];
			Carbon.Json.JsonNode reference = multi_ref[meaning];
			pp_info(reference);
			// last meaning doesn't show this separate line
			if (info_names[-1] != meaning ){
				Console.Write(blue(bold("OR")), "\n");
			}
		}

		return true;

	} else {
		return false;
	}
}



//  The main logic of `cr`
//    1. Search the default's first sheet first
//    2. Search the rest sheets in the cryptic sheets default dir
//
//  The `search` procedure is done via the `lookup` function. It
//  will print the info while finding. If `lookup` always return
//  false then means lacking of this word in our sheets. So a wel-
//  comed contribution is prinetd on the screen.
void solve_word(string word_2_solve){

	add_default_sheet_if_none_exist();

	string word = word_2_solve.ToLower();
	// The index is the toml file we'll look into

	string index = word[0].ToString();

    
    Regex rg = new Regex(@"\d");  
    MatchCollection matched = rg.Matches(index); 
    if(matched.Count>0){
        index = "0123456789";
    }
	// Default's first should be 1st to consider
	string first_sheet = "cryptic_computer";

	// cache lookup results
	// bool slice
	List<bool> results = new List<bool>(); // must be intialized in C#
	results.Add(lookup(first_sheet, index, word));
	// return if result == true # We should consider all sheets

	// Then else
	var dirs = Directory.GetDirectories(CRYPTIC_RESOLVER_HOME);
	foreach(var dir in dirs){
		string sheet = Path.GetFileName(dir);
		if(sheet != first_sheet) {
			results.Add(lookup(sheet, index, word));
			// continue if result == false # We should consider all sheets
		}
	}


	bool result_flag = false;
	foreach(var v in results) {
		if(v == true) {
			result_flag = true;
		}
	}

	if(result_flag != true) {
		Console.WriteLine("cr: Not found anything.\n\n" +
			"You may use `cr -u` to update the sheets.\n" +
			"Or you could contribute to our sheets: Thanks!");

		Console.WriteLine("    1. computer:  {0}", CRYPTIC_DEFAULT_SHEETS["computer"]);
		Console.WriteLine("    2. common:    {0}", CRYPTIC_DEFAULT_SHEETS["common"]);
		Console.WriteLine("    3. science:	 {0}", CRYPTIC_DEFAULT_SHEETS["science"]);
		Console.WriteLine("    4. economy:   {0}", CRYPTIC_DEFAULT_SHEETS["economy"]);
		Console.WriteLine("    5. medicine:  {0}", CRYPTIC_DEFAULT_SHEETS["medicine"]);
		Console.WriteLine();

	} else {
		return;
	}

}



void help() 
{
    string help = @"cr: Cryptic Resolver version {0} in C#

usage:
    cr -h                     => print this help
    cr -u (xx.com//repo.git)  => update default sheet or add sheet from a git repo
    cr emacs                  => Edit macros: a feature-rich editor";
	
    Console.WriteLine(help, CRYPTIC_VERSION);
}



// No need to write
// static void Main(string args)
// and args are already in this Top-level environment
// And the project should only has this file as entry point

string arg;	
int arg_num = args.Length;	// can't be args.length

if(arg_num < 1) {
	arg = "";
} else {
	arg = args[0];
}

// Console.WriteLine($"{arg_num} {arg}"); // DEBUG

switch (arg) {
case "":
	help();
	add_default_sheet_if_none_exist();
	break;
case "-h":
	help();
	break;
case "-u":
	if (arg_num > 1) {
		update_sheets(args[1]);
	} else {
		update_sheets("");
	}
	break;
default:
	solve_word(arg);
    break;  // need this to make compiler happy
}


